using System.Device.Gpio;
using HeatPumpController.Controller.Svc.Config;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Actuators.Relay;

public interface IRelayRecuperationUnitPower : IRelayHandler{}

public class RelayRecuperationUnitPower : RelayHandlerBase, IRelayRecuperationUnitPower
{
    public RelayRecuperationUnitPower(IOptions<ControllerConfig> config, GpioController gpioController) : 
        base(GpioConfig.Pins.HandleRecuperationUnitLow, config, gpioController)
    {
    }
    
    protected override void SetMonitoring(bool value) => RelayMonitoring.RecuperationUnitPower.Set(value);

}