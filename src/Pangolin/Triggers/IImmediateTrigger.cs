namespace Pangolin;

public interface IImmediateTrigger : ITrigger
{
    Task Set<TJob>(JobData data) where TJob : IJob;
}