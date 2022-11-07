using System.Device.Gpio;
using HeatPumpController.Controller.Svc.Config;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Sensors.Digital;

public interface IHdoIndicator : IDevice<DigitalSensorValue, bool>{}

public class HdoIndicator : IHdoIndicator
{
    private bool IsDummy { get; }
    
    public HdoIndicator(ILogger<HdoIndicator> logger, IOptions<ControllerConfig> globalConfig)
    {
        _logger = logger;
        IsDummy = globalConfig.Value.DummyTechnology;
    }

    private int PinNumber { get; } = GpioConfig.Pins.HdoInput;

    private readonly ILogger<HdoIndicator> _logger;

    public Task ReadAsync()
    {
        if (IsDummy)
            return ReadDummyAsync();
        
        try
        {
            using var controller = new GpioController();
            controller.OpenPin(PinNumber, PinMode.InputPullDown);
            var value = controller.Read(PinNumber);
            Value = DigitalSensorValue.CreateValid(value == PinValue.Low);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Cannot read async");
            Value = DigitalSensorValue.CreateInvalid();
        }
        
        return Task.CompletedTask;
    }

    private Task ReadDummyAsync()
    {
        Value = DigitalSensorValue.CreateValid(true);
        return Task.CompletedTask;
    }

    public bool ValidValue => Value.Valid;

    public DigitalSensorValue Value { get; private set; } = DigitalSensorValue.CreateInvalid();
}