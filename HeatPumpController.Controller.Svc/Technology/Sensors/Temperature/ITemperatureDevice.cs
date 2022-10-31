namespace HeatPumpController.Controller.Svc.Technology.Sensors.Temperature;

public interface ITemperatureDevice : IDevice<AnalogSensorValue, float>
{
    AnalogSensorValue Value { get; }
}