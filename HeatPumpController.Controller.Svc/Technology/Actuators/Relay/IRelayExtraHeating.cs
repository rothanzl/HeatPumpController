using System.Device.Gpio;
using HeatPumpController.Controller.Svc.Config;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Actuators.Relay;

public interface IRelayExtraHeating : IRelayHandler {}

public class RelayExtraHeating : RelayHandlerBase, IRelayExtraHeating
{
    public RelayExtraHeating(IOptions<ControllerConfig> config, GpioController gpioController) : 
        base(GpioConfig.Pins.HandleExtraHeating, config, gpioController)
    {
    }

    protected override void SetMonitoring(bool value) => RelayMonitoring.ExtraHeating.Set(value);
}