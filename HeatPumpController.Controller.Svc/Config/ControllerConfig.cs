namespace HeatPumpController.Controller.Svc.Config;

public class ControllerConfig
{
    public const string SectionName = nameof(ControllerConfig);

    public bool DummyTechnology { get; init; } = false;
    
    public string WeatherForecastApiKey { get; init; } = string.Empty;

    public OneWireDeviceConfig WaterTemperatureConfig { get; init; } = new()
    {
        BusId = "w1_bus_master1",
        DeviceId = "28-0214811c4bff"
    };
    
    public OneWireDeviceConfig HeatherBackTemperatureConfig { get; init; } = new()
    {
        BusId = "w1_bus_master1",
        DeviceId = "28-0214811c4bff"
    };
    
    
}

public class OneWireDeviceConfig
{
    public string BusId { get; init; } = string.Empty;
    public string DeviceId { get; init; } = string.Empty;
}