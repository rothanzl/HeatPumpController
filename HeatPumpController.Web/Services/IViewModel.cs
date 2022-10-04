using HeatPumpController.Controller.Svc.Models.Infra;

namespace HeatPumpController.Web.Services;

public interface IViewModel : IDisposable
{
    SetPoint WaterTemperature { get; }
    SetPoint HeaterTemperature { get; }
}

public class ViewModel : IViewModel
{
    private readonly IPersistentStateMediator _persistentStateMediator;
    
    public ViewModel(IPersistentStateMediator persistentStateMediator)
    {
        _persistentStateMediator = persistentStateMediator;
        _persistentStateMediator.DataChanged += PersistentStateMediatorOnDataChanged;
        
        WaterTemperature = new SetPoint("Teplota vody", 23, 30);
        HeaterTemperature = new SetPoint("Teplota topení", 40, 33);
    }

    private void PersistentStateMediatorOnDataChanged()
    {
        var temp = _persistentStateMediator.GetSetPointTemperatures();
        WaterTemperature.SetPointValue = temp.WaterTemperature;
        HeaterTemperature.SetPointValue = temp.HeatingTemperature;
    }

    public SetPoint WaterTemperature { get; }
    public SetPoint HeaterTemperature { get; }


    public void Dispose()
    {
        _persistentStateMediator.DataChanged -= PersistentStateMediatorOnDataChanged;
    }
    
}

public class SetPoint
{
    public SetPoint(string name, float currentValue, float setPointValue)
    {
        Name = name;
        CurrentValue = currentValue;
        SetPointValue = setPointValue;
    }

    public string Name { get; }
    public float CurrentValue { get; }

    private float _setPointValue;
    public float SetPointValue
    {
        get => _setPointValue;
        set
        {
            _setPointValue = value;
            DataChanged?.Invoke();
        }
    }

    public delegate void DataChangedHandler();
    public event DataChangedHandler DataChanged;
}