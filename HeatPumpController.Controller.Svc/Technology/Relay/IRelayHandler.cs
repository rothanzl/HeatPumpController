using System.Device.Gpio;
using HeatPumpController.Controller.Svc.Config;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Relay;

public interface IRelayHandler
{
    void Set(bool level);
}

public abstract class RelayHandlerBase : IRelayHandler
{
    private int PinNumber { get; }
    private bool DummyProbe { get; }
    
    protected RelayHandlerBase(int pinNumber, IOptions<ControllerConfig> config)
    {
        PinNumber = pinNumber;
        DummyProbe = config.Value.DummyProbes;
    }
    
    
    public void Set(bool level)
    {
        if (DummyProbe) return;
        
        PinValue value = level ? PinValue.High : PinValue.Low;
        
        using var controller = new GpioController();
        controller.OpenPin(PinNumber, PinMode.Output);
        controller.Write(pinNumber: PinNumber, value);
    }

}