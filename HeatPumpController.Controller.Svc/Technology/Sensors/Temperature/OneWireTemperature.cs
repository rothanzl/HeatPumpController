using HeatPumpController.Controller.Svc.Config;
using Iot.Device.OneWire;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Sensors.Temperature;

public interface IOneWireTemperature : ITemperatureDevice
{
    string BusId { get; }
    string DeviceId { get; }
}

public interface IWaterTemperature : IOneWireTemperature {}

public class WaterTemperature : OneWireTemperature, IWaterTemperature
{
    public WaterTemperature(IOptions<ControllerConfig> config, ILogger<WaterTemperature> logger) : 
        base(config.Value.WaterTemperatureConfig, logger, config)
    {
        
    }

    protected override TemperatureMonitoring Monitoring { get; } = new("water_reservoir");
}

public interface IHeaterBackTemperature : IOneWireTemperature {}

public class HeaterBackTemperature : OneWireTemperature, IHeaterBackTemperature
{
    public HeaterBackTemperature(IOptions<ControllerConfig> config, ILogger<HeaterBackTemperature> logger) : 
        base(config.Value.HeatherBackTemperatureConfig, logger, config)
    {
    }
    
    protected override TemperatureMonitoring Monitoring { get; } = new("heater_back");
}


public abstract class OneWireTemperature : IOneWireTemperature
{
    private readonly ILogger<OneWireTemperature> _logger;
    private bool DummyTechnology { get; }
    protected abstract TemperatureMonitoring Monitoring { get; }
    
    public OneWireTemperature(OneWireDeviceConfig config, ILogger<OneWireTemperature> logger,
        IOptions<ControllerConfig> globalConfig)
    {
        BusId = config.BusId;
        DeviceId = config.DeviceId;
        _logger = logger;
        DummyTechnology = globalConfig.Value.DummyTechnology;

        Value = SensorValue.CreateInvalid();
    }

    public string BusId { get; }
    public string DeviceId { get; }

    public SensorValue Value { get; private set; }
    
    public async Task ReadAsync()
    {
        if (DummyTechnology)
        {
            SetDummyValue();
            return;
        }
        
        try
        {
            OneWireThermometerDevice dev = new(BusId, DeviceId);
            var temp = await dev.ReadTemperatureAsync();
            Value = SensorValue.CreateValid((float)temp.Value);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error ReadAsync");
            Value = SensorValue.CreateInvalid();
        }
        
        Monitoring.Set(Value);
    }

    private void SetDummyValue()
    {
        Value = SensorValue.CreateValid(new Random().NextSingle() * 50);
    }
    
    
    
    public static IEnumerable<KeyValuePair<string, string>> EnumerateBusAndDeviceIds(ILogger logger)
    {
        List<KeyValuePair<string, string>> result = new();

        try
        {
            foreach (string busId in OneWireBus.EnumerateBusIds())
            {
                OneWireBus bus = new(busId);
                bus.ScanForDeviceChanges();
                foreach (var deviceId in bus.EnumerateDeviceIds())
                {
                    //OneWireDevice device = new(busId, deviceId);
                    
                    result.Add(new(busId, deviceId));
                }

            }

            logger.LogInformation("OneWire found[{BusDevices}]", 
                string.Join("",result.Select(kv => $"[{kv.Key}:{kv.Value}]")));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Cannot get OneWireBus");
        }


        return result;
    }


    
}