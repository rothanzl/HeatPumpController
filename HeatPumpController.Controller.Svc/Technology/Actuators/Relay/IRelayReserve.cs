using HeatPumpController.Controller.Svc.Config;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Actuators.Relay;

public interface IRelayReserve : IRelayHandler
{
    
}

public class RelayReserve : RelayHandlerBase, IRelayReserve
{
    public RelayReserve(IOptions<ControllerConfig> config) : base(GpioConfig.Pins.Reserve, config)
    {
    }
}