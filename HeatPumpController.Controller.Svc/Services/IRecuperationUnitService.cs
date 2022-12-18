using HeatPumpController.Controller.Svc.Models;
using HeatPumpController.Controller.Svc.Models.Infra;
using HeatPumpController.Controller.Svc.Technology.Sensors.Digital;

namespace HeatPumpController.Controller.Svc.Services;

public interface IRecuperationUnitService
{
    Task Act();
}

public class RecuperationUnitService : IRecuperationUnitService
{
    private readonly IPersistentContext<SystemState> _persistent;
    private readonly IHdoIndicator _hdo;
    private RecuperationUnitState State => _persistent.State.RecuperationUnit;
    
    private DateTime CyclingLastChange { get; set; }

    public RecuperationUnitService(IPersistentContext<SystemState> persistent, IHdoIndicator hdo)
    {
        _persistent = persistent;
        _hdo = hdo;
    }

    public Task Act()
    {
        // If not automation then exit
        if (!State.AutomationMode)
            return Task.CompletedTask;


        // Turn off if not HDO
        if (_hdo.ValidValue && !_hdo.Value.Value)
        {
            _persistent.State.RelayState.RecuperationUnitPower = false;
            return _persistent.WriteIfChangeAsync();
        }
            
        
        
        // If should run and paused
        if (State.Paused)
        {
            var now = DateTime.Now;
            if (now >= State.PausedUntil)
            {
                State.Paused = false;
                _persistent.State.RelayState.RecuperationUnitPower = true;
                return _persistent.WriteIfChangeAsync();
            }
            
            _persistent.State.RelayState.RecuperationUnitPower = false;
            return _persistent.WriteIfChangeAsync();
        }
        
        
        // Cycling
        if (State.Cycling.Enabled)
        {
            var now = DateTime.Now;

            if (CyclingLastChange != State.Cycling.CycleChange && now >= State.Cycling.CycleChange)
            {
                _persistent.State.RelayState.RecuperationUnitPower = !_persistent.State.RelayState.RecuperationUnitPower;
                CyclingLastChange = State.Cycling.CycleChange;
                State.Cycling.CycleChange += State.Cycling.Interval;
                return _persistent.WriteIfChangeAsync();
            }

            return Task.CompletedTask;
        }
        
        
        _persistent.State.RelayState.RecuperationUnitPower = true;
        return _persistent.WriteIfChangeAsync();
    }
}