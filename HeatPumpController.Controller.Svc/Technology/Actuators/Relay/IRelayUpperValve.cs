using HeatPumpController.Controller.Svc.Config;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Actuators.Relay;

public interface IRelayUpperValve : IRelayHandler {}

public class RelayUpperValve : RelayHandlerBase, IRelayUpperValve
{
    public RelayUpperValve(IOptions<ControllerConfig> config) : 
        base(GpioConfig.Pins.HandleUpperValve, config)
    {
    }
}