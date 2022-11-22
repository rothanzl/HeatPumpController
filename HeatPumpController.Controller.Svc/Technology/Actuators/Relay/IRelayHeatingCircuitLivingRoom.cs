using System.Device.Gpio;
using HeatPumpController.Controller.Svc.Config;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Actuators.Relay;

public interface IRelayHeatingCircuitLivingRoom : IRelayHandler
{
    
}

public class RelayHeatingCircuitLivingRoom : RelayHandlerBase, IRelayHeatingCircuitLivingRoom
{
    public RelayHeatingCircuitLivingRoom(IOptions<ControllerConfig> config, GpioController gpioController) : 
        base(GpioConfig.Pins.HandleHeatingCircuitLivingRoom, config, gpioController)
    {
    }
}