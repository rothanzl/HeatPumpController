namespace HeatPumpController.Controller.Svc.Technology.Sensors;

public interface ISensorValue<TValue>
{
    TValue Value { get; }
    bool Valid { get; }
    DateTime ReadTimeStamp { get; }
}

public record AnalogSensorValue(float Value, bool Valid, DateTime ReadTimeStamp) : ISensorValue<float>
{
    public static AnalogSensorValue CreateInvalid() => new(default, false, DateTime.Now);
    public static AnalogSensorValue CreateValid(float value) => new(value, true, DateTime.Now);
}public record DigitalSensorValue(bool Value, bool Valid, DateTime ReadTimeStamp) : ISensorValue<bool>
{
    public static DigitalSensorValue CreateInvalid() => new(false, false, DateTime.Now);
    public static DigitalSensorValue CreateValid(bool value) => new(value, true, DateTime.Now);
}