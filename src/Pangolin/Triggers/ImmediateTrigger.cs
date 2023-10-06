using Orleans.Runtime;

namespace Pangolin;

public class ImmediateTrigger : TriggerBase<TriggerState>, IImmediateTrigger
{
    public ImmediateTrigger([PersistentState("immediateTriggerState", "pangolin")] IPersistentState<TriggerState> state) : base(state)
    {
    }

    public async Task Set<TJob>(JobData data) where TJob : IJob
    {
        await base.Set<TJob>(data);
        await Start();
        await Fire(null);
        await Stop();
    }
}