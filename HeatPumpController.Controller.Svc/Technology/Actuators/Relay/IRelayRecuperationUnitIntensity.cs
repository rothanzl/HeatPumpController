using System.Device.Gpio;
using HeatPumpController.Controller.Svc.Config;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Actuators.Relay;

public interface IRelayRecuperationUnitIntensity : IRelayHandler
{
    
}

public class RelayRecuperationUnitIntensity : RelayHandlerBase, IRelayRecuperationUnitIntensity
{
    public RelayRecuperationUnitIntensity(IOptions<ControllerConfig> config, GpioController gpioController) : 
        base(GpioConfig.Pins.Reserve, config, gpioController)
    {
    }
    
    protected override void SetMonitoring(bool value) => RelayMonitoring.RecuperationUnitIntensity.Set(value);

}