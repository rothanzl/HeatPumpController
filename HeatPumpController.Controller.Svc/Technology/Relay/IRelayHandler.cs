using System.Device.Gpio;

namespace HeatPumpController.Controller.Svc.Technology.Relay;

public interface IRelayHandler
{
    void Set(Level level);
}

public enum Level
{
    High,
    Low,
}

public abstract class RelayHandlerBase : IRelayHandler
{
    private int PinNumber { get; }
    
    protected RelayHandlerBase(int pinNumber)
    {
        PinNumber = pinNumber;
        
        
    }
    
    
    public void Set(Level level)
    {
        PinValue value = level switch
        {
            Level.High => PinValue.High,
            Level.Low => PinValue.Low,
            _ => throw new ArgumentOutOfRangeException(nameof(level), level.ToString(), "Unsupported value")
        };
        
        using var controller = new GpioController();
        controller.OpenPin(PinNumber, PinMode.Output);
        controller.Write(pinNumber: PinNumber, value);
    }

}