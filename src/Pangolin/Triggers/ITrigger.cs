namespace Pangolin;

public interface ITrigger : IGrainWithStringKey
{
    Task<TriggerState> GetState();
    Task Start();
    Task Stop();
    Task Fire(DateTime? nextFireAt);
}