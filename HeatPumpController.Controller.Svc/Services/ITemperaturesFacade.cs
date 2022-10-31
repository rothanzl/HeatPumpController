using HeatPumpController.Controller.Svc.Technology.Sensors;
using HeatPumpController.Controller.Svc.Technology.Sensors.Temperature;
using HeatPumpController.Controller.Svc.Technology.Sensors.Temperature.Mqtt;

namespace HeatPumpController.Controller.Svc.Services;

public interface ITemperaturesFacade
{
    Task ReadAll();
    bool ReadFailed();

    float WaterReservoir { get; }
    float HeaterBack { get; }
    float OutAmbient { get; }
    float BathRoom { get; }
    float Kitchen { get; }
    float LivingRoom { get; }
    float SmallRoom { get; }
    float BedRoom { get; }
}

public class TemperaturesFacade : ITemperaturesFacade
{
    private readonly IWaterTemperature _waterTemperatureDevice;
    private readonly IHeaterBackTemperature _heatherBackTemperatureDevice;
    private readonly IWeatherForecast _weatherForecast;

    private readonly IKitchenTemperatureSensor _kitchen;
    private readonly IBathRoomTemperatureSensor _bathRoom;
    private readonly ILivingRoomTemperatureSensor _livingRoom;
    private readonly ISmallRoomTemperatureSensor _smallRoom;
    private readonly IBedRoomTemperatureSensor _bedRoom;


    public TemperaturesFacade(IWaterTemperature waterTemperatureDevice, 
        IHeaterBackTemperature heatherBackTemperatureDevice, 
        IWeatherForecast weatherForecast, IKitchenTemperatureSensor kitchen, 
        IBathRoomTemperatureSensor bathRoom, ILivingRoomTemperatureSensor livingRoom, 
        ISmallRoomTemperatureSensor smallRoom, IBedRoomTemperatureSensor bedRoom)
    {
        _waterTemperatureDevice = waterTemperatureDevice;
        _heatherBackTemperatureDevice = heatherBackTemperatureDevice;
        _weatherForecast = weatherForecast;
        _kitchen = kitchen;
        _bathRoom = bathRoom;
        _livingRoom = livingRoom;
        _smallRoom = smallRoom;
        _bedRoom = bedRoom;

        Devices = new IDevice[]
        {
            _waterTemperatureDevice,
            _heatherBackTemperatureDevice,
            _weatherForecast,
            _kitchen,
            _bathRoom,
            _livingRoom,
            _smallRoom,
            _bedRoom
        };
    }
    
    private IDevice[] Devices { get; }

    //public Task ReadAll() => Task.WhenAll(Devices.Select(d => d.ReadAsync()));
    public Task ReadAll() => Task.Run(() => Task.WaitAll(Devices.Select(d => d.ReadAsync()).ToArray()));
    public bool ReadFailed() => Devices.Any(d => d.ValidValue == false);

    public float WaterReservoir => _waterTemperatureDevice.Value.Value;
    public float HeaterBack => _heatherBackTemperatureDevice.Value.Value;
    public float OutAmbient => _weatherForecast.Value.Value;
    public float BathRoom => _bathRoom.Value.Value;
    public float Kitchen => _kitchen.Value.Value;
    public float LivingRoom => _livingRoom.Value.Value;
    public float SmallRoom => _smallRoom.Value.Value;
    public float BedRoom => _bedRoom.Value.Value;
}