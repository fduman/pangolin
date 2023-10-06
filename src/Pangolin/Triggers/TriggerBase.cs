using Orleans.Runtime;

namespace Pangolin;

public abstract class TriggerBase<TState> : Grain, ITrigger where TState: TriggerState
{
    private readonly IPersistentState<TState> _state;

    protected TriggerBase(IPersistentState<TState> state)
    {
        _state = state ?? throw new ArgumentNullException(nameof(state));
    }

    protected TState State => _state.State;

    protected Task WriteStateAsync()
    {
        return _state.WriteStateAsync();
    }
    
    public virtual async Task Fire(DateTime? nextFireAt)
    {
        if (nextFireAt != null && nextFireAt.Value.ToUniversalTime() < DateTime.UtcNow)
        {
            throw new ArgumentOutOfRangeException(nameof(nextFireAt), nextFireAt, "Must be higher than current time");
        }
        
        State.LastFiredAt = DateTime.UtcNow;
        State.FireAt = nextFireAt;
        await WriteStateAsync();
        
        var job = GrainFactory.GetGrain<IJob>(this.GetPrimaryKeyString(), State.JobClassName);
        await job.Execute(State.JobData);
    }

    protected virtual async Task Set<TJob>(JobData data) where TJob : IJob
    {
        State.JobClassName = typeof(TJob).FullName;
        State.JobData = data;
        await WriteStateAsync();
        
        var registry = GrainFactory.GetGrain<ITriggerRegistry>("Registry");
        await registry.Add(this.GetPrimaryKeyString(), GetType());
    }
    
    public virtual async Task Start()
    {
        State.Status = TriggerStatuses.Started;
        State.StartedAt = DateTime.UtcNow;
        await WriteStateAsync();
    }

    public virtual async Task Stop()
    {
        State.Status = TriggerStatuses.Stopped;
        State.StoppedAt = DateTime.UtcNow;
        await WriteStateAsync();
    }

    public Task<TriggerState> GetState()
    {
        return Task.FromResult<TriggerState>(State);
    }
}