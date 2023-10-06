namespace Pangolin;

public interface ITriggerRegistry : IGrainWithStringKey
{
    Task Add(string id, Type classType);
    Task Remove(string id);
    Task<IEnumerable<KeyValuePair<string, string>>> List();
}