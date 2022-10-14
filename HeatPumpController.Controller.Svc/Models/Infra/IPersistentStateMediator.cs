
namespace HeatPumpController.Controller.Svc.Models.Infra;

public interface IPersistentStateMediator
{
    SetPointTemperatures SetPointTemperatures { get; }
    Task SetSetPointTemperatures(SetPointTemperatures temperatures);
    CurrentTemperatures CurrentTemperatures { get; set; }
    RelayState Relays { get; }

    event CurrentValuesChangedHandler CurrentValuesChanged;
    public delegate void CurrentValuesChangedHandler();

    Task PersistIfChange();
}

public class PersistentStateMediator : IPersistentStateMediator
{
    private readonly IPersistentContext<SystemState> _persistence;
    
    public PersistentStateMediator()
    {
        _persistence = new PersistentContext<SystemState>(SystemState.Name);
    }

    public SetPointTemperatures SetPointTemperatures
        => _persistence.State.SetPointTemperatures;


    private CurrentTemperatures _currentTemperatures = new (0, 0, 0);
    public CurrentTemperatures CurrentTemperatures
    {
        get => _currentTemperatures;
        set
        {
            _currentTemperatures = value;
            CurrentValuesChanged?.Invoke();
        }
    }

    public RelayState Relays => _persistence.State.RelayState;

    public async Task SetSetPointTemperatures(SetPointTemperatures temperatures)
    {
        _persistence.State.SetPointTemperatures = temperatures;
        await _persistence.WriteIfChange();
    }

    public event IPersistentStateMediator.CurrentValuesChangedHandler? CurrentValuesChanged;
    public Task PersistIfChange()
    {
        return _persistence.WriteIfChange();
    }
}