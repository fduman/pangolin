namespace Pangolin;

public interface ITriggerFactory
{
    Task SetRecurring<TJob>(string id, string cronExpression, JobData data) where TJob : IJob;
    Task SetDelayed<TJob>(string id, TimeSpan delay, JobData data) where TJob : IJob;
    Task SetImmediate<TJob>(string id, JobData data) where TJob : IJob;
}