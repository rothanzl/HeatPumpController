using System.Device.Gpio;
using HeatPumpController.Controller.Svc.Config;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Actuators.Relay;

public interface IRelayHeatingCircuitBedRoom : IRelayHandler
{
    
}

public class RelayHeatingCircuitBedRoom : RelayHandlerBase, IRelayHeatingCircuitBedRoom
{
    public RelayHeatingCircuitBedRoom(IOptions<ControllerConfig> config, GpioController gpioController) : 
        base(GpioConfig.Pins.HandleHeatingCircuitBedRoom, config, gpioController)
    {
    }
    
    protected override void SetMonitoring(bool value) => RelayMonitoring.HeatingCircuitBedRoom.Set(value);

}