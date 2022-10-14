using HeatPumpController.Controller.Svc.Config;
using Iot.Device.OneWire;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Temperature;

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
    private bool DummyProbe { get; }
    protected abstract TemperatureMonitoring Monitoring { get; }
    
    public OneWireTemperature(OneWireDeviceConfig config, ILogger<OneWireTemperature> logger,
        IOptions<ControllerConfig> globalConfig)
    {
        BusId = config.BusId;
        DeviceId = config.DeviceId;
        _logger = logger;
        DummyProbe = globalConfig.Value.DummyProbes;
    }

    public string BusId { get; }
    public string DeviceId { get; }

    public float Value { get; private set; }
    
    public async Task ReadAsync()
    {
        if (DummyProbe) return;
        
        try
        {
            OneWireThermometerDevice dev = new(BusId, DeviceId);
            var temp = await dev.ReadTemperatureAsync();
            Value = (float)temp.Value;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error ReadAsync");
            Value = default;
        }
        
        Monitoring.Set(Value);
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