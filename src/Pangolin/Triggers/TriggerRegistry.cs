using Orleans.Runtime;

namespace Pangolin;

public class TriggerRegistry : Grain, ITriggerRegistry
{
    private readonly IPersistentState<Dictionary<string, string>> _triggerRegistry;

    public TriggerRegistry([PersistentState("triggerRegistry", "pangolin")] IPersistentState<Dictionary<string, string>> triggerRegistry)
    {
        _triggerRegistry = triggerRegistry;
    }
    
    public async Task Add(string id, Type classType)
    {
        _triggerRegistry.State.Add(id, classType.FullName);
        await _triggerRegistry.WriteStateAsync();
    }

    public async Task Remove(string id)
    {
        _triggerRegistry.State.Remove(id);
        await _triggerRegistry.WriteStateAsync();
    }

    public async Task<IEnumerable<KeyValuePair<string, string>>> List()
    {
        return _triggerRegistry.State.Select(p => p).ToList();
    }
}