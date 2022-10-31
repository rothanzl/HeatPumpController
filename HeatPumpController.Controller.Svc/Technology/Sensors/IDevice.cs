namespace HeatPumpController.Controller.Svc.Technology.Sensors;

public interface IDevice<TSensorValue, TValue> where TSensorValue : ISensorValue<TValue>
{
    Task ReadAsync();
    TSensorValue Value { get; }
}