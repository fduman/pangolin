namespace Pangolin;

public interface IDelayedTrigger : ITrigger
{
    Task Set<TJob>(TimeSpan delay, JobData? data = null) where TJob : IJob;
}