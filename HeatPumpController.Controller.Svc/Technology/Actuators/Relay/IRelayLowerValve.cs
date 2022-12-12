using System.Device.Gpio;
using HeatPumpController.Controller.Svc.Config;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Actuators.Relay;

public interface IRelayLowerValve : IRelayHandler {}

public class RelayLowerValve : RelayHandlerBase, IRelayLowerValve
{
    public RelayLowerValve(IOptions<ControllerConfig> config, GpioController gpioController) :
        base(GpioConfig.Pins.HandleLowerValve, config, gpioController)
    {
    }
    
    protected override void SetMonitoring(bool value) => RelayMonitoring.LowerValve.Set(value);

}