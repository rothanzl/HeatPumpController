using System.Device.Gpio;
using HeatPumpController.Controller.Svc.Config;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Actuators.Relay;

public interface IRelayUpperValve : IRelayHandler {}

public class RelayUpperValve : RelayHandlerBase, IRelayUpperValve
{
    public RelayUpperValve(IOptions<ControllerConfig> config, GpioController gpioController) : 
        base(GpioConfig.Pins.HandleUpperValve, config, gpioController)
    {
    }
}