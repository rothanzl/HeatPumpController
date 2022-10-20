
namespace HeatPumpController.Controller.Svc.Models.Infra;

public interface IPersistentStateMediator
{
    void StateChanged();
    SetPointTemperatures SetPointTemperatures { get; }
    Task SetSetPointTemperatures(SetPointTemperatures temperatures);
    CurrentTemperatures CurrentTemperatures { get; set; }
    RelayState Relays { get; }
    ProcessState ProcessState { get; }

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

    public void StateChanged()
    {
        CurrentValuesChanged?.Invoke();
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
    public ProcessState ProcessState => _persistence.State.ProcessState;

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