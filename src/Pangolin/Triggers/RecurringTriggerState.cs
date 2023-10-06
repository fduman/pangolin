namespace Pangolin;

[GenerateSerializer]
public class RecurringTriggerState : TriggerState
{
    [Id(7)]
    public string? Expression { get; set; }
}