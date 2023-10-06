using Cronos;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Orleans.Timers;

namespace Pangolin;

public class RecurringTrigger : TimerTriggerBase<RecurringTriggerState>, IRecurringTrigger
{
    public RecurringTrigger(
        [PersistentState("recurringTriggerState", "pangolin")] IPersistentState<RecurringTriggerState> state,
        ILogger<RecurringTrigger> logger,
        IReminderRegistry reminderRegistry,
        IGrainContext grainContext) :
        base(state, logger, reminderRegistry, grainContext)
    {
    }

    protected override async Task CheckTime()
    {
        if (DateTime.UtcNow >= State.FireAt)
        {
            var expression = CronExpression.Parse(State.Expression);
            var nextFireAt = expression.GetNextOccurrence(DateTime.UtcNow)!.Value;
            await Fire(nextFireAt);
        }
    }

    public async Task Set<TJob>(string cronExpression, JobData data) where TJob : IJob
    {
        var expression = CronExpression.Parse(cronExpression);
        State.Expression = cronExpression;
        State.FireAt = expression.GetNextOccurrence(DateTime.UtcNow)!.Value;
        await WriteStateAsync();
        await base.Set<TJob>(data);
        await Start();
    }
}