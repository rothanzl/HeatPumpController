
namespace HeatPumpController.Controller.Svc.Models.Infra;

public interface IPersistentStateMediator
{
    SetPointTemperatures SetPointTemperatures { get; }
    Task SetSetPointTemperatures(SetPointTemperatures temperatures);
    CurrentTemperatures CurrentTemperatures { get; }
    RelayState Relays { get; }
    Task SetCurrentTemperatures(CurrentTemperatures temperatures);
    Task PersistIfTimeout(DateTime now);

    event CurrentValuesChangedHandler CurrentValuesChanged;
    public delegate void CurrentValuesChangedHandler();
}

public class PersistentStateMediator : IPersistentStateMediator
{
    private readonly IPersistentContext<SystemState> _persistence;
    
    public PersistentStateMediator(IPersistentContext<SystemState> persistence)
    {
        _persistence = persistence;
    }

    public SetPointTemperatures SetPointTemperatures
        => _persistence.State.SetPointTemperatures;

    public CurrentTemperatures CurrentTemperatures
        => _persistence.State.CurrentTemperatures;

    public RelayState Relays => _persistence.State.RelayState;

    public async Task SetCurrentTemperatures(CurrentTemperatures temperatures)
    {
        _persistence.State.CurrentTemperatures = temperatures;
        await _persistence.WriteIfChange();
        CurrentValuesChanged?.Invoke();
    }

    public async Task SetSetPointTemperatures(SetPointTemperatures temperatures)
    {
        _persistence.State.SetPointTemperatures = temperatures;
        await _persistence.WriteIfChange();
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

    public event IPersistentStateMediator.CurrentValuesChangedHandler? CurrentValuesChanged;
}