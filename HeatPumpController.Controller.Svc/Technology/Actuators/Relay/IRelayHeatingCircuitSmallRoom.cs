using System.Device.Gpio;
using HeatPumpController.Controller.Svc.Config;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Actuators.Relay;

public interface IRelayHeatingCircuitSmallRoom : IRelayHandler
{
    
}

public class RelayHeatingCircuitSmallRoom : RelayHandlerBase, IRelayHeatingCircuitSmallRoom
{
    public RelayHeatingCircuitSmallRoom(IOptions<ControllerConfig> config, GpioController gpioController) : 
        base(GpioConfig.Pins.HandleHeatingCircuitSmallRoom, config, gpioController)
    {
    }
}