using System.Device.Gpio;
using HeatPumpController.Controller.Svc.Config;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Actuators.Relay;

public interface IRelayHeatingCircuitBathRoomWall : IRelayHandler
{
    
}

public class RelayHeatingCircuitBathRoomWall : RelayHandlerBase, IRelayHeatingCircuitBathRoomWall
{
    public RelayHeatingCircuitBathRoomWall(IOptions<ControllerConfig> config, GpioController gpioController) : 
        base(GpioConfig.Pins.HandleHeatingCircuitBathRoomWall, config, gpioController)
    {
    }
}