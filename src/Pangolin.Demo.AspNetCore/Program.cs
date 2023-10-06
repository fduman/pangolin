using Pangolin;
using Pangolin.Demo.AspNetCore.Jobs;
using Pangolin.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UsePangolinRedis("localhost:6379,ssl=False,abortConnect=False,channelPrefix=pangolin:");

using WebApplication app = builder.Build();

app.MapGet("/", async (ITriggerFactory triggerFactory) =>
{
    await triggerFactory.SetImmediate<TestJob>(Guid.NewGuid().ToString(), new JobData
    {
        Timeout = TimeSpan.FromSeconds(10)
    });
    /*await triggerFactory.SetDelayed<TestJob>(Guid.NewGuid().ToString(), TimeSpan.FromMinutes(1), new JobData());
    await triggerFactory.SetRecurring<TestJob>(Guid.NewGuid().ToString(), "* * * * *", new JobData
    {
        Timeout = TimeSpan.FromSeconds(3)
    });*/
    return Results.Ok("Finished!");
});

app.MapGet("/list", async (IGrainFactory grainFactory) =>
{
    var registry = grainFactory.GetGrain<ITriggerRegistry>("Registry");
    return Results.Ok(await registry.List());
});
app.MapGet("/triggers/{id}/state", async (IGrainFactory grainFactory, string id) =>
{
    var registry = grainFactory.GetGrain<ITriggerRegistry>("Registry");
    var list = await registry.List();
    var triggerRegistry = list.FirstOrDefault(p => p.Key == id);
    var trigger = grainFactory.GetGrain<ITrigger>(id, triggerRegistry.Value);
    return Results.Ok(await trigger.GetState());
});
app.MapGet("/jobs/{id}/state", async (IGrainFactory grainFactory, string id) =>
{
    var registry = grainFactory.GetGrain<ITriggerRegistry>("Registry");
    var list = await registry.List();
    var triggerRegistry = list.FirstOrDefault(p => p.Key == id);
    var trigger = grainFactory.GetGrain<ITrigger>(id, triggerRegistry.Value);
    var triggerState = await trigger.GetState();
    var job = grainFactory.GetGrain<IJob>(id, triggerState.JobClassName);
    return Results.Ok(await job.GetState());
});

app.Run();