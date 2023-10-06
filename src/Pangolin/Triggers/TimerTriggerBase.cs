using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Orleans.Timers;

namespace Pangolin;

public abstract class TimerTriggerBase<TState> : TriggerBase<TState>, IRemindable, IAsyncDisposable where TState: TriggerState
{
    private readonly ILogger _logger;
    private readonly IReminderRegistry _reminderRegistry;
    private readonly IGrainContext _grainContext;
    private IGrainReminder _reminder;

    protected TimerTriggerBase(IPersistentState<TState> state,
        ILogger logger,
        IReminderRegistry reminderRegistry,
        IGrainContext grainContext) : base(state)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _reminderRegistry = reminderRegistry;
        _grainContext = grainContext;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _reminder = await _reminderRegistry.GetReminder(_grainContext.GrainId, this.GetPrimaryKeyString());
        _logger.LogInformation("Trigger {Id} has been activated", this.GetPrimaryKeyString());
        await base.OnActivateAsync(cancellationToken);
    }

    protected abstract Task CheckTime();

    public async Task ReceiveReminder(string reminderName, TickStatus status)
    {
        _logger.LogInformation("Reminder {Name} fired with status {@Status}", reminderName, status);
        await CheckTime();
    }

    public override async Task Start()
    {
        await base.Start();
        
        if (_reminder != null)
        {
            _logger.LogWarning("Trigger already started");
            return;
        }

        _reminder = await _reminderRegistry.RegisterOrUpdateReminder(
            callingGrainId: _grainContext.GrainId,
            reminderName: this.GetPrimaryKeyString(),
            dueTime: TimeSpan.Zero,
            period: TimeSpan.FromMinutes(1));
    }

    public override async Task Stop()
    {
        await DisposeAsync();
        await base.Stop();
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_reminder != null)
        {
            await _reminderRegistry.UnregisterReminder(_grainContext.GrainId, _reminder);
            _logger.LogInformation("Reminder {Name} is unregistered", this.GetPrimaryKeyString());
            _reminder = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        GC.SuppressFinalize(this);
    }
}