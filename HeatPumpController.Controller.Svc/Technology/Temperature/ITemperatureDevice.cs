namespace HeatPumpController.Controller.Svc.Technology.Temperature;

public interface ITemperatureDevice
{
    Task ReadAsync();
    float Value { get; }
}