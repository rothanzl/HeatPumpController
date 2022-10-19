using HeatPumpController.Controller.Svc.Config;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Actuators.Relay;

public interface IRelayHeatingCircuitBathRoom : IRelayHandler
{
    
}

public class RelayHeatingCircuitBathRoom : RelayHandlerBase, IRelayHeatingCircuitBathRoom
{
    public RelayHeatingCircuitBathRoom(IOptions<ControllerConfig> config) : 
        base(GpioConfig.Pins.HandleHeatingCircuitBathRoom, config)
    {
    }
}