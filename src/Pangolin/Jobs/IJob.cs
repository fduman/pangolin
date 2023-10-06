namespace Pangolin;

public interface IJob : IGrainWithStringKey
{
    Task<JobState> GetState();
    Task Execute(JobData data);
    Task Complete();
    Task Failed(Exception e);
}