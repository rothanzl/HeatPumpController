using HeatPumpController.Controller.Svc.Technology.Sensors.Temperature.Mqtt;

namespace HeatPumpController.Controller.Svc.Technology.Sensors.Temperature;


public abstract class TemperatureMonitoringBase
{
    protected abstract string Name { get; }
    protected const string Label = "heatpump_device";

    protected void Unpublish(string[] labels)
    {
        Infrastructure.Monitoring.UnpublishGauge(Name, labels);
    }
}

public class MqttTemperatureMonitoring : TemperatureMonitoringBase
{
    private const string LabelUnit = "heatpump_unit";
    protected override string Name { get; } = "room_temperature";
    
    private Dictionary<string, string> LabelsTemperature { get; }
    private Dictionary<string, string> LabelsHumidity { get; }
    private Dictionary<string, string> LabelsLinkQuality { get; }
    private Dictionary<string, string> LabelsBattery { get; }
    
    
    public MqttTemperatureMonitoring(string room)
    {
        LabelsTemperature = new()
        {
            { Label, room },
            { LabelUnit, "temperature" }
        };
        LabelsHumidity = new()
        {
            { Label, room },
            { LabelUnit, "humidity" }
        };
        LabelsLinkQuality = new()
        {
            { Label, room },
            { LabelUnit, "link_quality" }
        };
        LabelsBattery = new()
        {
            { Label, room },
            { LabelUnit, "battery" }
        };
    }

    public void Set(SensorMessage message)
    {
        Infrastructure.Monitoring.SetGaugeValue(Name, LabelsTemperature, message.Temperature);
        Infrastructure.Monitoring.SetGaugeValue(Name, LabelsHumidity, message.Humidity);
        Infrastructure.Monitoring.SetGaugeValue(Name, LabelsLinkQuality, message.LinkQuality);
        Infrastructure.Monitoring.SetGaugeValue(Name, LabelsBattery, message.Battery);
    }
}

public class TemperatureMonitoring : TemperatureMonitoringBase
{
    private Dictionary<string, string> Labels { get; }
    protected override string Name { get; } = "heatpump_temperature";

    public TemperatureMonitoring(string labelTemperatureValue)
    {
        Labels = new Dictionary<string, string>
        {
            {Label, labelTemperatureValue}
        };
    }

    public void Set(SensorValue value)
    {
        if (value.Valid)
        {
            Infrastructure.Monitoring.SetGaugeValue(Name, Labels, value.Value);
        }
        else
        {
            Unpublish(Labels.Keys.ToArray());
        }
    }
}