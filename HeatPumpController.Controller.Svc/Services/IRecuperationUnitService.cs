using HeatPumpController.Controller.Svc.Models;
using HeatPumpController.Controller.Svc.Models.Infra;

namespace HeatPumpController.Controller.Svc.Services;

public interface IRecuperationUnitService
{
    Task Act();
}

public class RecuperationUnitService : IRecuperationUnitService
{
    private readonly IPersistentContext<SystemState> _persistent;
    private RecuperationUnit State => _persistent.State.RecuperationUnit;

    public RecuperationUnitService(IPersistentContext<SystemState> persistent)
    {
        _persistent = persistent;
    }

    public Task Act()
    {
        // If not automation then exit
        if (!State.AutomationMode)
            return Task.CompletedTask;
        
        
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
        
        _persistent.State.RelayState.RecuperationUnitPower = true;
        return _persistent.WriteIfChangeAsync();
    }
}