namespace Pangolin;

public interface IRecurringTrigger : ITrigger
{
    Task Set<TJob>(string cronExpression, JobData? data = null) where TJob : IJob;
}