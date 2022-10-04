namespace HeatPumpController.Controller.Svc.Models.Infra;

public interface IPersistentStateMediator
{
    SetPointTemperatures GetSetPointTemperatures();
    Task SetSetPointTemperatures(SetPointTemperatures temperatures);
    Task PersistIfTimeout(DateTime now);

    event DataChangedHandler DataChanged;
    public delegate void DataChangedHandler();
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

    public async Task SetSetPointTemperatures(SetPointTemperatures temperatures)
    {
        _persistence.State.SetPointTemperatures = temperatures;
        await _persistence.WriteIfChange();
        DataChanged?.Invoke();
    }
        


    public Task PersistIfTimeout(DateTime now)
    {
        if (_persistence.State.SavedAt + TimeSpan.FromMinutes(1) < now)
        {
            _persistence.State.SavedAt = now;
            return _persistence.WriteIfChange();            
        }
        
        return Task.CompletedTask;
    }

    public event IPersistentStateMediator.DataChangedHandler? DataChanged;
}