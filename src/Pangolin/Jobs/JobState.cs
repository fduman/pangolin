namespace Pangolin;

[GenerateSerializer]
public class JobState
{
    [Id(0)]
    public JobData Data { get; set; }
    [Id(1)]
    public JobStatuses Status { get; set; }
    [Id(2)]
    public string Error { get; set; }
    [Id(3)]
    public DateTime? LastStartedAt { get; set; }
    [Id(4)]
    public DateTime? LastFailedAt { get; set; }
    [Id(5)]
    public DateTime? LastCompletedAt { get; set; }
}