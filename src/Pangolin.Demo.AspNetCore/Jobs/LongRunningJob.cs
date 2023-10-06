using Orleans.Runtime;

namespace Pangolin.Demo.AspNetCore.Jobs;

public class LongRunningJob : JobBase
{
    public LongRunningJob([PersistentState("jobState", "pangolin")] IPersistentState<JobState> state, ILogger<LongRunningJob> logger) : base(state, logger)
    {
    }

    protected override async Task Process(JobData data, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(1000, cancellationToken);
        }
    }
}