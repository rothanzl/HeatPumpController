using HeatPumpController.Controller.Svc.Models;
using HeatPumpController.Controller.Svc.Models.Infra;

namespace HeatPumpController.Web.Services;

public interface IViewModel : IDisposable, IRelayState, IProcessState
{
    SetPoint WaterTemperature { get; }
    SetPoint HeaterTemperature { get; }
    Measurement OutTemperature { get; }
    DateTime CurrentTime { get; }
    ProcessStateEnum ProcessState { get; }

    event Measurement.DataChangedHandler DataChanged;
    Task SetAllRelays(bool val);
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
        HeaterTemperature = new SetPoint("Teplota topení", currTemps.HeatingTemperature, setPointTemps.HeatingTemperature);
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

    public ProcessStateEnum ProcessState => _stateMediator.ProcessState.State;
    public event Measurement.DataChangedHandler DataChanged;

    public SetPoint WaterTemperature { get; }
    public SetPoint HeaterTemperature { get; }
    public Measurement OutTemperature { get; }
    public DateTime CurrentTime { get; private set; }
    
    public void Dispose()
    {
        _stateMediator.CurrentValuesChanged -= CurrentValuesChangedHandler;
    }

    public bool HeatingCircuitBathRoomRelay {
        get => _stateMediator.Relays.HeatingCircuitBathRoomRelay;
        set
        {
            if (_stateMediator.ProcessState.Automation)
                return;
            
            _stateMediator.Relays.HeatingCircuitBathRoomRelay = value;
            Task.Run(() => _stateMediator.PersistIfChange()).Wait();
            DataChanged?.Invoke();
        } 
    }
    public bool HeatingCircuitBathRoomWallRelay {
        get => _stateMediator.Relays.HeatingCircuitBathRoomWallRelay;
        set
        {
            if (_stateMediator.ProcessState.Automation)
                return;
            
            _stateMediator.Relays.HeatingCircuitBathRoomWallRelay = value;
            Task.Run(() => _stateMediator.PersistIfChange()).Wait();
            DataChanged?.Invoke();
        } 
    }
    public bool HeatingCircuitBedRoomRelay {
        get => _stateMediator.Relays.HeatingCircuitBedRoomRelay;
        set
        {
            if (_stateMediator.ProcessState.Automation)
                return;
            
            _stateMediator.Relays.HeatingCircuitBedRoomRelay = value;
            Task.Run(() => _stateMediator.PersistIfChange()).Wait();
            DataChanged?.Invoke();
        } 
    }
    public bool HeatingCircuitKitchenRelay {
        get => _stateMediator.Relays.HeatingCircuitKitchenRelay;
        set
        {
            if (_stateMediator.ProcessState.Automation)
                return;
            
            _stateMediator.Relays.HeatingCircuitKitchenRelay = value;
            Task.Run(() => _stateMediator.PersistIfChange()).Wait();
            DataChanged?.Invoke();
        } 
    }
    public bool HeatingCircuitLivingRoomRelay {
        get => _stateMediator.Relays.HeatingCircuitLivingRoomRelay;
        set
        {
            if (_stateMediator.ProcessState.Automation)
                return;
            
            _stateMediator.Relays.HeatingCircuitLivingRoomRelay = value;
            Task.Run(() => _stateMediator.PersistIfChange()).Wait();
            DataChanged?.Invoke();
        } 
    }
    public bool HeatingCircuitSmallRoomRelay {
        get => _stateMediator.Relays.HeatingCircuitSmallRoomRelay;
        set
        {
            if (_stateMediator.ProcessState.Automation)
                return;
            
            _stateMediator.Relays.HeatingCircuitSmallRoomRelay = value;
            Task.Run(() => _stateMediator.PersistIfChange()).Wait();
            DataChanged?.Invoke();
        } 
    }
    public bool LowerValveRelay {
        get => _stateMediator.Relays.LowerValveRelay;
        set
        {
            if (_stateMediator.ProcessState.Automation)
                return;
            
            _stateMediator.Relays.LowerValveRelay = value;
            Task.Run(() => _stateMediator.PersistIfChange()).Wait();
            DataChanged?.Invoke();
        } 
    }
    public bool UpperValveRelay {
        get => _stateMediator.Relays.UpperValveRelay;
        set
        {
            if (_stateMediator.ProcessState.Automation)
                return;
            
            _stateMediator.Relays.UpperValveRelay = value;
            Task.Run(() => _stateMediator.PersistIfChange()).Wait();
            DataChanged?.Invoke();
        } 
    }
    public bool HeatPumpRelay {
        get => _stateMediator.Relays.HeatPumpRelay;
        set
        {
            if (_stateMediator.ProcessState.Automation)
                return;
            
            _stateMediator.Relays.HeatPumpRelay = value;
            Task.Run(() => _stateMediator.PersistIfChange()).Wait();
            DataChanged?.Invoke();
        } 
    }

    public bool ExtraHeatingRelay
    {
        get => _stateMediator.Relays.ExtraHeatingRelay;
        set
        {
            if (_stateMediator.ProcessState.Automation)
                return;

            _stateMediator.Relays.ExtraHeatingRelay = value;
            Task.Run(() => _stateMediator.PersistIfChange()).Wait();
            DataChanged?.Invoke();
        }
    }
    
    public async Task SetAllRelays(bool val)
    {
        if (_stateMediator.ProcessState.Automation)
            return;
        
        _stateMediator.Relays.HeatPumpRelay = val;
        _stateMediator.Relays.UpperValveRelay = val;
        _stateMediator.Relays.LowerValveRelay = val;
        _stateMediator.Relays.ExtraHeatingRelay = val;

        _stateMediator.Relays.HeatingCircuitBathRoomWallRelay = val;
        _stateMediator.Relays.HeatingCircuitBathRoomRelay = val;
        _stateMediator.Relays.HeatingCircuitKitchenRelay = val;
        _stateMediator.Relays.HeatingCircuitLivingRoomRelay = val;
        _stateMediator.Relays.HeatingCircuitBedRoomRelay = val;
        _stateMediator.Relays.HeatingCircuitSmallRoomRelay = val;

        await _stateMediator.PersistIfChange();
        DataChanged?.Invoke();
    }

    public bool Automation {
        get => _stateMediator.ProcessState.Automation;
        set
        {
            _stateMediator.ProcessState.Automation = value;
            Task.Run(() => _stateMediator.PersistIfChange()).Wait();
            DataChanged?.Invoke();
        } 
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