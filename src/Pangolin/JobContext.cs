namespace Pangolin;

[GenerateSerializer]
public class JobContext
{
    [Id(0)]
    public string JobClassName { get; set; }
    [Id(1)]
    public JobData? Data { get; set; }
    [Id(2)]
    public bool Executed { get; set; }
}