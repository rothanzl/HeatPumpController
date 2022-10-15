using HeatPumpController.Controller.Svc.Config;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Actuators.Relay;

public interface IRelayHeatingCircuitSmallRoom : IRelayHandler
{
    
}

public class RelayHeatingCircuitSmallRoom : RelayHandlerBase, IRelayHeatingCircuitSmallRoom
{
    public RelayHeatingCircuitSmallRoom(IOptions<ControllerConfig> config) : 
        base(GpioConfig.Pins.HandleHeatingCircuitSmallRoom, config)
    {
    }
}