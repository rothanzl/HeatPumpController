using HeatPumpController.Controller.Svc.Config;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Actuators.Relay;

public interface IRelayHeatPump : IRelayHandler { }

public class RelayHeatPump : RelayHandlerBase, IRelayHeatPump
{
    public RelayHeatPump(IOptions<ControllerConfig> config) : 
        base(GpioConfig.Pins.HandleHeatPump, config)
    {
    }
}