namespace Pangolin;

[GenerateSerializer]
public class TriggerState
{
    [Id(0)]
    public TriggerStatuses Status { get; set; } = TriggerStatuses.Created;
    [Id(1)]
    public DateTime? StartedAt { get; set; }
    [Id(2)]
    public DateTime? StoppedAt { get; set; }
    [Id(3)]
    public DateTime? LastFiredAt { get; set; }
    [Id(4)]
    public DateTime? FireAt { get; set; }
    [Id(5)]
    public string JobClassName { get; set; }
    [Id(6)]
    public JobData JobData { get; set; }
}