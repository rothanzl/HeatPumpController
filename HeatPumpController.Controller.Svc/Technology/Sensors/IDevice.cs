namespace HeatPumpController.Controller.Svc.Technology.Sensors;

public interface IDevice
{
    Task ReadAsync();
    bool ValidValue { get; }
}

public interface IDevice<TSensorValue, TValue> : IDevice
    where TSensorValue : ISensorValue<TValue>
{
    
    TSensorValue Value { get; }
}