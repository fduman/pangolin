using Microsoft.Extensions.Logging;
using Orleans.Runtime;

namespace Pangolin;

public abstract class JobBase : Grain, IJob
{
    private readonly IPersistentState<JobState> _jobState;
    private readonly ILogger _logger;
    private Task? _processTask;

    protected JobBase(
        IPersistentState<JobState> state,
        ILogger logger)
    {
        _jobState = state ?? throw new ArgumentNullException(nameof(state));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected JobState State => _jobState.State;

    public Task<JobState> GetState()
    {
        return Task.FromResult(State);
    }

    public async Task Execute(JobData data)
    {
        State.Status = JobStatuses.Running;
        State.LastStartedAt = DateTime.UtcNow;
        State.Data = data;
        await _jobState.WriteStateAsync();

        var taskScheduler = TaskScheduler.Current;
        var tcs = new GrainCancellationTokenSource();
        var ts = new CancellationTokenSource(data.Timeout);
        var cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(tcs.Token.CancellationToken, ts.Token).Token;
        
        _processTask = Task.Run(async () =>
        {
            try
            {
                _logger.LogInformation("Job {Name} is executing", this.GetPrimaryKeyString());
                await Process(data, cancellationToken);
                _logger.LogInformation("Job {Name} executed", this.GetPrimaryKeyString());
                await InvokeGrainMethod(taskScheduler, job => job.Complete());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Job {Name} execution failed", this.GetPrimaryKeyString());
                await InvokeGrainMethod(taskScheduler, job => job.Failed(e));
            }
        }, cancellationToken);
    }

    private Task InvokeGrainMethod(TaskScheduler taskScheduler, Func<IJob, Task> jobAction)
    {
        return Task.Factory.StartNew(async () =>
        {
            var grain = GrainFactory.GetGrain<IJob>(this.GetPrimaryKeyString(), GetType().FullName);
            await jobAction(grain);
        }, CancellationToken.None, TaskCreationOptions.None, taskScheduler);
    }

    public Task Complete()
    {
        _processTask = null;
        
        State.Status = JobStatuses.Completed;
        State.LastCompletedAt = DateTime.UtcNow;
        return _jobState.WriteStateAsync();
    }

    public Task Failed(Exception e)
    {
        _processTask = null;
        
        State.Status = JobStatuses.Failed;
        State.LastFailedAt = DateTime.UtcNow;
        State.Error = e.ToString();
        return _jobState.WriteStateAsync();
    }

    protected abstract Task Process(JobData data, CancellationToken cancellationToken);
}