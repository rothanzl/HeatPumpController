namespace HeatPumpController.Controller.Svc.Technology.Temperature;

public class TemperatureMonitoring
{
    private readonly Dictionary<string, string> _labels;

    private const string Name = "heatpump_temperature";
    private const string Label = "heatpump_device";
    

    public TemperatureMonitoring(string labelTemperatureValue)
    {
        _labels = new Dictionary<string, string>()
        {
            {Label, labelTemperatureValue}
        };
    }

    public void Set(float value)
    {
        Infrastructure.Monitoring.SetGaugeValue(Name, _labels, value);
    }
}