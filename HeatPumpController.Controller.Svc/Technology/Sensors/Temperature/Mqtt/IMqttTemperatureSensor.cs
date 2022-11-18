using System.Text.Json;
using System.Text.Json.Serialization;
using HeatPumpController.Controller.Svc.Services;

namespace HeatPumpController.Controller.Svc.Technology.Sensors.Temperature.Mqtt;

public interface IMqttTemperatureSensorsHelper
{
    Task HandleMessage(string topic, byte[] message);
}

public class MqttTemperatureSensorsHelper : IMqttTemperatureSensorsHelper
{
    private readonly ILogger<MqttTemperatureSensorsHelper> _logger;
    private readonly IMqttTemperatureSensor[] _sensors;

    public MqttTemperatureSensorsHelper(ILogger<MqttTemperatureSensorsHelper> logger, 
        IBathRoomTemperatureSensor s1,
        ISmallRoomTemperatureSensor s2,
        IBedRoomTemperatureSensor s3,
        ILivingRoomTemperatureSensor s4,
        IKitchenTemperatureSensor s5,
        ITechRoomTemperatureSensor s6
        )
    {
        _logger = logger;
        _sensors = new IMqttTemperatureSensor[] { s1, s2, s3, s4, s5, s6 };
    }

    public async Task HandleMessage(string topic, byte[] message)
    {
        try
        {
            var parsedMessage = JsonSerializer.Deserialize<SensorMessage>(message)!;

            var sensor = _sensors.FirstOrDefault(s => s.Topic.Equals(topic));

            if (sensor == null)
                throw new ArgumentOutOfRangeException(nameof(topic), topic, "Cannot find sensor with that topic name");

            await sensor.ConsumeMessage(parsedMessage);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Cannot consume message[{Message}]", message == null ? null : Convert.ToString(message));
        }
    }
}

public class SensorMessage
{
    [JsonPropertyName("battery")]
    public float Battery { get; init; }

    [JsonPropertyName("humidity")]
    public float Humidity { get; init; }

    [JsonPropertyName("linkquality")]
    public float LinkQuality { get; init; }
    
    [JsonPropertyName("temperature")]
    public float Temperature { get; init; }
    
    [JsonPropertyName("voltage")]
    public float VoltageMilliVolts { get; init; }
}


public interface IMqttTemperatureSensor : IDevice<AnalogSensorValue, float>
{
    Task ConsumeMessage(SensorMessage message);
    string Topic { get; }
}

public abstract class MqttTemperatureSensorBase : IMqttTemperatureSensor
{
    protected MqttTemperatureSensorBase(string topic)
    {
        Topic = topic;

        var roomName = topic.Split("_")[1];
        Monitoring = new(roomName);
        
        Value = AnalogSensorValue.CreateInvalid();
    }
    
    public Task ConsumeMessage(SensorMessage message)
    {
        Monitoring.Set(message);
        Value = AnalogSensorValue.CreateValid(message.Temperature);
        return Task.CompletedTask;
    }

    private MqttTemperatureMonitoring Monitoring { get; }
    public string Topic { get; }
    public Task ReadAsync() => Task.CompletedTask;


    public bool ValidValue => Value.Valid;
    public AnalogSensorValue Value { get; private set; }
}

public interface ITechRoomTemperatureSensor : IMqttTemperatureSensor {}

class TechRoomTemperatureSensor : MqttTemperatureSensorBase, ITechRoomTemperatureSensor
{
    private const string TopicStatic = MqttTopics.TempTechRoom;
    
    public TechRoomTemperatureSensor() : base(TopicStatic)
    {
    }
}

public interface IBathRoomTemperatureSensor : IMqttTemperatureSensor{}

class BathRoomTemperatureSensor : MqttTemperatureSensorBase, IBathRoomTemperatureSensor
{
    private const string TopicStatic = MqttTopics.TempBathRoom;

    public BathRoomTemperatureSensor() : base(TopicStatic)
    {
    }
}

public interface ISmallRoomTemperatureSensor : IMqttTemperatureSensor{}

class SmallRoomTemperatureSensor : MqttTemperatureSensorBase, ISmallRoomTemperatureSensor
{
    private const string TopicStatic = MqttTopics.TempSmallRoom;

    public SmallRoomTemperatureSensor() : base(TopicStatic)
    {
    }
}

public interface IBedRoomTemperatureSensor : IMqttTemperatureSensor{}

class BedRoomTemperatureSensor : MqttTemperatureSensorBase, IBedRoomTemperatureSensor
{
    private const string TopicStatic = MqttTopics.TempBedRoom;

    public BedRoomTemperatureSensor() : base(TopicStatic)
    {
    }
}

public interface ILivingRoomTemperatureSensor : IMqttTemperatureSensor{}

class LivingRoomTemperatureSensor : MqttTemperatureSensorBase, ILivingRoomTemperatureSensor
{
    private const string TopicStatic = MqttTopics.TempLivingRoom;

    public LivingRoomTemperatureSensor() : base(TopicStatic)
    {
    }
}

public interface IKitchenTemperatureSensor : IMqttTemperatureSensor{}

class KitchenTemperatureSensor : MqttTemperatureSensorBase, IKitchenTemperatureSensor
{
    private const string TopicStatic = MqttTopics.TempKitchen;

    public KitchenTemperatureSensor() : base(TopicStatic)
    {
    }
}


