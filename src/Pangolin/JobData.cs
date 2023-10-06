namespace Pangolin;

[GenerateSerializer]
public class JobData
{
    [Id(0)]
    public IDictionary<string, string> Data { get; } = new Dictionary<string, string>();
    [Id(1)]
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
}