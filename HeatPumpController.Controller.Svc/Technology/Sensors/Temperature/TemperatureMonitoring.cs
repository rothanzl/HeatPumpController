namespace HeatPumpController.Controller.Svc.Technology.Sensors.Temperature;

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

    public void Set(SensorValue value)
    {
        if (value.Valid)
        {
            Infrastructure.Monitoring.SetGaugeValue(Name, _labels, value.Value);
        }
        else
        {
            Infrastructure.Monitoring.UnpublishGauge(Name, _labels.Keys.ToArray());
        }
    }
}