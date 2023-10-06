using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Orleans.Timers;

namespace Pangolin;

public class DelayedTrigger : TimerTriggerBase<TriggerState>, IDelayedTrigger
{
    public DelayedTrigger(
        [PersistentState("delayedTriggerState", "pangolin")] IPersistentState<TriggerState> state,
        ILogger<DelayedTrigger> logger,
        IReminderRegistry reminderRegistry,
        IGrainContext grainContext) : base(state, logger, reminderRegistry, grainContext)
    {
    }

    protected override async Task CheckTime()
    {
        if (DateTime.UtcNow >= State.FireAt)
        {
            await Fire(null);
            await Stop();
        }
    }

    public async Task Set<TJob>(TimeSpan delay, JobData data) where TJob : IJob
    {
        if (delay < TimeSpan.FromMinutes(1))
        {
            throw new ArgumentOutOfRangeException(nameof(delay), "Cannot set a delay lower than 1 min.");
        }

        await base.Set<TJob>(data);
        State.FireAt = DateTime.UtcNow.Add(delay);
        await WriteStateAsync();
        await Start();
    }
}