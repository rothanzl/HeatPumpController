using HeatPumpController.Controller.Svc.Config;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Actuators.Relay;

public interface IRelayRecuperationUnitLow : IRelayHandler{}

public class RelayRecuperationUnitLow : RelayHandlerBase, IRelayRecuperationUnitLow
{
    public RelayRecuperationUnitLow(IOptions<ControllerConfig> config) : 
        base(GpioConfig.Pins.HandleRecuperationUnitLow, config)
    {
    }
}