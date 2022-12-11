using System.Device.Gpio;
using HeatPumpController.Controller.Svc.Config;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Sensors.Digital;

public interface IHdoIndicator : IDevice<DigitalSensorValue, bool>
{}

public class HdoIndicator : IHdoIndicator
{
    private bool IsDummy { get; }

    public HdoIndicator(ILogger<HdoIndicator> logger, IOptions<ControllerConfig> globalConfig, GpioController gpioController)
    {
        _logger = logger;
        _gpioController = gpioController;
        IsDummy = globalConfig.Value.DummyTechnology;
        _monitoring = new();
        
        if(!IsDummy)
            _gpioController.OpenPin(PinNumber, PinMode.InputPullDown);
    }

    private int PinNumber { get; } = GpioConfig.Pins.HdoInput;

    private readonly ILogger<HdoIndicator> _logger;
    private readonly HdoMonitoring _monitoring;
    private readonly GpioController _gpioController;

    public Task ReadAsync()
    {
        if (IsDummy)
            return ReadDummyAsync();
        
        try
        {
            var value = _gpioController.Read(PinNumber);
            Value = DigitalSensorValue.CreateValid(value == PinValue.Low);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Cannot read async");
            Value = DigitalSensorValue.CreateInvalid();
        }
        
        _monitoring.Set(Value);
        return Task.CompletedTask;
    }

    private Task ReadDummyAsync()
    {
        Value = DigitalSensorValue.CreateValid(true);
        _monitoring.Set(Value);
        return Task.CompletedTask;
    }

    public bool ValidValue => Value.Valid;

    public DigitalSensorValue Value { get; private set; } = DigitalSensorValue.CreateInvalid();
}