using Iot.Device.OneWire;

namespace HeatPumpController.Controller.Svc.Technology.Temperature;

public interface IOneWireTemperature
{
    string BusId { get; }
    string DeviceId { get; }

    Task ReadAsync();
    float Value { get; }
}

public class OneWireTemperature : IOneWireTemperature
{
    public OneWireTemperature(string busId, string deviceId)
    {
        BusId = busId;
        DeviceId = deviceId;
    }

    public string BusId { get; }
    public string DeviceId { get; }

    public float Value { get; private set; }
    
    public async Task ReadAsync()
    {
        OneWireThermometerDevice dev = new(BusId, DeviceId);
        var temp = await dev.ReadTemperatureAsync();
        Value = (float)temp.Value;
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