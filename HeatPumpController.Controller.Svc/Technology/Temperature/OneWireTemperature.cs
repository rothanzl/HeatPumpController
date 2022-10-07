using Iot.Device.OneWire;

namespace HeatPumpController.Controller.Svc.Technology.Temperature;

public interface IOneWireTemperature
{
    
}

public class OneWireTemperature : IOneWireTemperature
{

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