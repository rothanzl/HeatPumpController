namespace HeatPumpController.Controller.Svc.Technology.Sensors;

public interface ISensorValue<TValue>
{
    TValue Value { get; }
    bool Valid { get; }
    DateTime ReadTimeStamp { get; }
}

public static class SensorValueExtensions
{
    public static TimeSpan ReadTimeStampElapsed<T>(this ISensorValue<T> v) 
        => DateTime.Now - v.ReadTimeStamp;

    public static string ToShortString(this TimeSpan ts) => ts.ToString(@"hh\:mm\:ss");
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