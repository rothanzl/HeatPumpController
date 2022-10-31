using System.Device.Gpio;

namespace HeatPumpController.Controller.Svc.Technology.Sensors.Digital;

public interface IHdoIndicator : IDevice<DigitalSensorValue, bool>{}

public class HdoIndicator : IHdoIndicator
{
    public HdoIndicator(ILogger<HdoIndicator> logger)
    {
        _logger = logger;
    }

    private int PinNumber { get; } = GpioConfig.Pins.HdoInput;

    private readonly ILogger<HdoIndicator> _logger;

    public Task ReadAsync()
    {
        try
        {
            using var controller = new GpioController();
            controller.OpenPin(PinNumber, PinMode.InputPullDown);
            var value = controller.Read(PinNumber);
            Value = DigitalSensorValue.CreateValid(value == PinValue.High);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Cannot read async");
            Value = DigitalSensorValue.CreateInvalid();
        }
        
        return Task.CompletedTask;
    }

    public DigitalSensorValue Value { get; private set; } = DigitalSensorValue.CreateInvalid();
}