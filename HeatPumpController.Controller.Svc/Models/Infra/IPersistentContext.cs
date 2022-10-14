
namespace HeatPumpController.Controller.Svc.Models.Infra;

public interface IPersistentContext<TState> where TState : new()
{
    TState State { get; }
    Task WriteIfChange();
}

public class PersistentContext<TState> : IPersistentContext<TState> where TState : new()
{
    public TState State { get; }
    private string SerializedState { get; set; }
    private readonly PersistAdapter _persistAdapter;
    
    public PersistentContext(string persistentName)
    {
        _persistAdapter = new(persistentName);
        string? serialized = _persistAdapter.Get();

        if (serialized == null)
        {
            State = new();
            SerializedState = Serializer.Serialize<TState>(State);
        }
        else
        {
            SerializedState = serialized;
            State = Serializer.Deserialize<TState>(serialized);
        }
        
    }
    
    public async Task WriteIfChange()
    {
        var currentSerializedState = Serializer.Serialize(State);
        if (currentSerializedState.Equals(SerializedState))
            return;

        await _persistAdapter.Persist(currentSerializedState);
        SerializedState = currentSerializedState;
    }


    private static class Serializer
    {
        public static T Deserialize<T>(string json) where T : new()
        {
            T result = System.Text.Json.JsonSerializer.Deserialize<T>(json)!;
            return result;
        }

        public static string Serialize<T>(T state)
        {
            string result = System.Text.Json.JsonSerializer.Serialize(state);
            return result;
        }
    }

    private class PersistAdapter
    {
        private readonly string _name;
        private string FileName => _name + ".json";

        public PersistAdapter(string name)
        {
            _name = name;
        }

        public string? Get()
        {
            if (!File.Exists(FileName))
                return null;
            
            return File.ReadAllText(FileName);
        }

        public Task Persist(string content)
        {
            return File.WriteAllTextAsync(FileName, content);
        }
    }
    
}