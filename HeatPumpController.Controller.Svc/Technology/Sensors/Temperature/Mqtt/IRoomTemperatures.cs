namespace HeatPumpController.Controller.Svc.Technology.Sensors.Temperature.Mqtt;

public interface IRoomTemperatures
{
    ISensorValue<float> BathRoom { get; }
    ISensorValue<float> Kitchen { get; }
    ISensorValue<float> LivingRoom { get; }
    ISensorValue<float> BedRoom { get; }
    ISensorValue<float> SmallRoom { get; }
    ISensorValue<float> TechRoom { get; }
}

public class RoomTemperatures : IRoomTemperatures
{
    public RoomTemperatures(IBathRoomTemperatureSensor bathRoomTemperatureSensor, IKitchenTemperatureSensor kitchenTemperatureSensor, ILivingRoomTemperatureSensor livingRoomTemperatureSensor, IBedRoomTemperatureSensor bedRoomTemperatureSensor, ISmallRoomTemperatureSensor smallRoomTemperatureSensor, ITechRoomTemperatureSensor techRoomTemperatureSensor)
    {
        _bathRoomTemperatureSensor = bathRoomTemperatureSensor;
        _kitchenTemperatureSensor = kitchenTemperatureSensor;
        _livingRoomTemperatureSensor = livingRoomTemperatureSensor;
        _bedRoomTemperatureSensor = bedRoomTemperatureSensor;
        _smallRoomTemperatureSensor = smallRoomTemperatureSensor;
        _techRoomTemperatureSensor = techRoomTemperatureSensor;
    }

    public ISensorValue<float> BathRoom => _bathRoomTemperatureSensor.Value;
    public ISensorValue<float> Kitchen => _kitchenTemperatureSensor.Value;
    public ISensorValue<float> LivingRoom => _livingRoomTemperatureSensor.Value;
    public ISensorValue<float> BedRoom => _bedRoomTemperatureSensor.Value;
    public ISensorValue<float> SmallRoom => _smallRoomTemperatureSensor.Value;
    public ISensorValue<float> TechRoom => _techRoomTemperatureSensor.Value;

    private readonly IBathRoomTemperatureSensor _bathRoomTemperatureSensor;
    private readonly IKitchenTemperatureSensor _kitchenTemperatureSensor;
    private readonly ILivingRoomTemperatureSensor _livingRoomTemperatureSensor;
    private readonly IBedRoomTemperatureSensor _bedRoomTemperatureSensor;
    private readonly ISmallRoomTemperatureSensor _smallRoomTemperatureSensor;
    private readonly ITechRoomTemperatureSensor _techRoomTemperatureSensor;
}