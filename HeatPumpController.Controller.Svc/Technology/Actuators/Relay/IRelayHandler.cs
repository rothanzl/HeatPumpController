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
    private readonly GpioController _gpioController;
    
    protected RelayHandlerBase(int pinNumber, IOptions<ControllerConfig> config, GpioController gpioController)
    {
        PinNumber = pinNumber;
        _gpioController = gpioController;
        DummyTechnology = config.Value.DummyTechnology;
        
        _gpioController.OpenPin(PinNumber, PinMode.Output);
    }
    
    
    public void Set(bool set)
    {
        if (DummyTechnology) return;
        
        PinValue value = set ? PinValue.Low : PinValue.High;
        
        
        _gpioController.Write(pinNumber: PinNumber, value);
    }

}