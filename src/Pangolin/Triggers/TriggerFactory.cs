namespace Pangolin;

public class TriggerFactory : ITriggerFactory
{
    private readonly IGrainFactory _grainFactory;

    public TriggerFactory(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory ?? throw new ArgumentNullException(nameof(grainFactory));
    }

    public Task SetRecurring<TJob>(string id, string cronExpression, JobData data) where TJob : IJob
    {
        var grain = _grainFactory.GetGrain<IRecurringTrigger>(id);
        return grain.Set<TJob>(cronExpression, data);
    }

    public Task SetDelayed<TJob>(string id, TimeSpan delay, JobData data) where TJob : IJob
    {
        var grain = _grainFactory.GetGrain<IDelayedTrigger>(id);
        return grain.Set<TJob>(delay, data);
    }

    public Task SetImmediate<TJob>(string id, JobData data) where TJob : IJob
    {
        var grain = _grainFactory.GetGrain<IImmediateTrigger>(id);
        return grain.Set<TJob>(data);
    }
}