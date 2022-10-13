using System.Device.Gpio;

namespace HeatPumpController.Controller.Svc.Technology.Relay;

public interface IRelayHandler : IDisposable
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

    private readonly GpioController _controller; 
    
    protected RelayHandlerBase(int pinNumber)
    {
        PinNumber = pinNumber;
        
        _controller = new GpioController();
        _controller.OpenPin(PinNumber, PinMode.Output);
        _controller.Write(pinNumber: PinNumber, PinValue.High);
    }
    
    
    public void Set(Level level)
    {
        PinValue value = level switch
        {
            Level.High => PinValue.High,
            Level.Low => PinValue.Low,
            _ => throw new ArgumentOutOfRangeException(nameof(level), level.ToString(), "Unsupported value")
        };
        
        _controller.Write(pinNumber: PinNumber, value);
    }

    public void Dispose()
    {
        _controller.Dispose();
    }
}