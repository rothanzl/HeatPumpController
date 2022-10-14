using HeatPumpController.Controller.Svc.Models;
using HeatPumpController.Controller.Svc.Models.Infra;

namespace HeatPumpController.Web.Services;

public interface IViewModel : IDisposable
{
    SetPoint WaterTemperature { get; }
    SetPoint HeaterTemperature { get; }
    Measurement OutTemperature { get; }
    DateTime CurrentTime { get; }
    
    bool TestRelayState { get; set; }

    event Measurement.DataChangedHandler DataChanged;
}

public class ViewModel : IViewModel
{
    private readonly IPersistentStateMediator _stateMediator;
    
    public ViewModel(IPersistentStateMediator persistentStateMediator)
    {
        _stateMediator = persistentStateMediator;
        _stateMediator.CurrentValuesChanged += CurrentValuesChangedHandler;

        var currTemps = _stateMediator.CurrentTemperatures;
        var setPointTemps = _stateMediator.SetPointTemperatures;

        WaterTemperature = new SetPoint("Teplota vody", currTemps.WaterTemperature, setPointTemps.WaterTemperature);
        HeaterTemperature = new SetPoint("Teplota topení", currTemps.HeatingTemperature, setPointTemps.HeatingTemperature)
            {
                Editable = false
            };
        OutTemperature = new Measurement("Venkovní teplota", currTemps.OutTemperature);

        WaterTemperature.SetPointValueChanged += SetPointValuesChangedHandler;
        HeaterTemperature.SetPointValueChanged += SetPointValuesChangedHandler;
    }

    private void SetPointValuesChangedHandler()
    {
        _stateMediator.SetSetPointTemperatures(new SetPointTemperatures(
            WaterTemperature.SetPointValue, HeaterTemperature.SetPointValue));
    }

    private void CurrentValuesChangedHandler()
    {
        var temp = _stateMediator.CurrentTemperatures;
        WaterTemperature.Value = temp.WaterTemperature;
        HeaterTemperature.Value = temp.HeatingTemperature;
        OutTemperature.Value = temp.OutTemperature;
        CurrentTime = DateTime.Now;
        
        DataChanged?.Invoke();
    }
    
    public event Measurement.DataChangedHandler DataChanged;

    public SetPoint WaterTemperature { get; }
    public SetPoint HeaterTemperature { get; }
    public Measurement OutTemperature { get; }
    public DateTime CurrentTime { get; private set; }

    public bool TestRelayState
    {
        get => _stateMediator.Relays.TestRelay;
        set
        {
            _stateMediator.Relays.TestRelay = value;
            Task.Run(() => _stateMediator.PersistIfChange()).Wait();
        }
    }


    public void Dispose()
    {
        _stateMediator.CurrentValuesChanged -= CurrentValuesChangedHandler;
    }
    
}

public class SetPoint : Measurement
{
    public SetPoint(string name, float value, float setPointValue) : base(name, value)
    {
        SetPointValue = setPointValue;
    }

    public bool Editable { get; set; } = true;

    private float _setPointValue;
    public float SetPointValue
    {
        get => _setPointValue;
        set
        {
            _setPointValue = value;
            SetPointValueChanged?.Invoke();
        }
    }

    public event DataChangedHandler SetPointValueChanged;
}

public class Measurement
{
    public Measurement(string name, float value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; }

    private float _value;
    public float Value { 
        get => _value;
        set
        {
            _value = value;
            ValueChanged?.Invoke();
        } 
    }

    public delegate void DataChangedHandler();
    public event DataChangedHandler ValueChanged;
}