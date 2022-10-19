namespace HeatPumpController.Controller.Svc.Technology.Sensors;

public record SensorValue(float Value, bool Valid, DateTime ReadTimeStamp)
{
    public static SensorValue CreateInvalid() => new(default, false, DateTime.Now);
    public static SensorValue CreateValid(float value) => new(value, true, DateTime.Now);
}