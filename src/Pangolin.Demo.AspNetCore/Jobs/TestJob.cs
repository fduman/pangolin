using Orleans.Runtime;

namespace Pangolin.Demo.AspNetCore.Jobs;

public class TestJob : JobBase
{
    public TestJob([PersistentState("jobState", "pangolin")] IPersistentState<JobState> state, ILogger<TestJob> logger) : base(state, logger)
    {
    }

    protected override async Task Process(JobData data, CancellationToken cancellationToken)
    {
        //throw new Exception("Test");
        await Task.Delay(1000, cancellationToken);
        Console.WriteLine("Running {0}", this.GetPrimaryKeyString());
    }
}