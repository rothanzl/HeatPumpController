namespace HeatPumpController.Controller.Svc.Models.Infra;

public interface IPersistentStateMediator
{
    SetPointTemperatures GetSetPointTemperatures();
    Task PersistIfTimeout(DateTime now);
}

public class PersistentStateMediator : IPersistentStateMediator
{
    private readonly IPersistentContext<SystemState> _persistence;
    
    public PersistentStateMediator(IPersistentContext<SystemState> persistence)
    {
        _persistence = persistence;
    }

    public SetPointTemperatures GetSetPointTemperatures()
        => _persistence.State.SetPointTemperatures;

    public Task PersistIfTimeout(DateTime now)
    {
        if (_persistence.State.SavedAt + TimeSpan.FromMinutes(1) < now)
        {
            _persistence.State.SavedAt = now;
            return _persistence.WriteIfChange();            
        }
        
        return Task.CompletedTask;
    }
}