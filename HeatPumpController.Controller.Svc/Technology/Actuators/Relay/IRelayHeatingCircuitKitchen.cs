using HeatPumpController.Controller.Svc.Config;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Actuators.Relay;

public interface IRelayHeatingCircuitKitchen : IRelayHandler
{
    
}

public class RelayHeatingCircuitKitchen : RelayHandlerBase, IRelayHeatingCircuitKitchen
{
    public RelayHeatingCircuitKitchen(IOptions<ControllerConfig> config) : 
        base(GpioConfig.Pins.HandleHeatingCircuitKitchen, config)
    {
    }
}