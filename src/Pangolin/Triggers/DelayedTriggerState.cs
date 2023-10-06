namespace Pangolin;

[GenerateSerializer]
public class DelayedTriggerState : TriggerState
{
    [Id(7)]
    public DateTime? FireAt { get; set; }
}