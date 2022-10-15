using System.Device.Gpio;
using HeatPumpController.Controller.Svc.Config;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Actuators.Relay;

public interface IRelayHandler
{
    void Set(bool set);
}

public abstract class RelayHandlerBase : IRelayHandler
{
    private int PinNumber { get; }
    private bool DummyTechnology { get; }
    
    protected RelayHandlerBase(int pinNumber, IOptions<ControllerConfig> config)
    {
        PinNumber = pinNumber;
        DummyTechnology = config.Value.DummyTechnology;
    }
    
    
    public void Set(bool set)
    {
        if (DummyTechnology) return;
        
        PinValue value = set ? PinValue.Low : PinValue.High;
        
        using var controller = new GpioController();
        controller.OpenPin(PinNumber, PinMode.Output);
        controller.Write(pinNumber: PinNumber, value);
    }

}