namespace HeatPumpController.Controller.Svc.Models;

public interface IPersistentContext<TState> where TState : new()
{
    TState State { get; }
    Task WriteIfChange();
}

public class PersistentContext<TState> : IPersistentContext<TState> where TState : new()
{
    public TState State { get; }
    public Task WriteIfChange()
    {
        return Task.CompletedTask;
    }

    public PersistentContext()
    {
        State = new();
    }
}