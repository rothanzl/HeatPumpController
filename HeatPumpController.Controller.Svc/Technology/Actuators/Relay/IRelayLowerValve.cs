using HeatPumpController.Controller.Svc.Config;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Actuators.Relay;

public interface IRelayLowerValve : IRelayHandler {}

public class RelayLowerValve : RelayHandlerBase, IRelayLowerValve
{
    public RelayLowerValve(IOptions<ControllerConfig> config) :
        base(GpioConfig.Pins.HandleLowerValve, config)
    {
    }
}