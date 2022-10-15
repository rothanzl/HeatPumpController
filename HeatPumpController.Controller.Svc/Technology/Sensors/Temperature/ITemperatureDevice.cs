namespace HeatPumpController.Controller.Svc.Technology.Sensors.Temperature;

public interface ITemperatureDevice
{
    Task ReadAsync();
    float Value { get; }
}