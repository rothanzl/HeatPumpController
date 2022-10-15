using HeatPumpController.Controller.Svc.Config;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Actuators.Relay;

public interface IRelayHeatingCircuitBedRoom : IRelayHandler
{
    
}

public class RelayHeatingCircuitBedRoom : RelayHandlerBase, IRelayHeatingCircuitBedRoom
{
    public RelayHeatingCircuitBedRoom(IOptions<ControllerConfig> config) : 
        base(GpioConfig.Pins.HandleHeatingCircuitBedRoom, config)
    {
    }
}