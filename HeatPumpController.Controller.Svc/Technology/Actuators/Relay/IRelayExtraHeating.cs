using HeatPumpController.Controller.Svc.Config;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Actuators.Relay;

public interface IRelayExtraHeating : IRelayHandler {}

public class RelayExtraHeating : RelayHandlerBase, IRelayExtraHeating
{
    public RelayExtraHeating(IOptions<ControllerConfig> config) : 
        base(GpioConfig.Pins.HandleExtraHeating, config)
    {
    }
}