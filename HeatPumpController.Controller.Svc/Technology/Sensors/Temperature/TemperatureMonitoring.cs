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
    
    
    public MqttTemperatureMonitoring(string topic)
    {
        topic = topic.Replace("/", "-");
        
        LabelsTemperature = new()
        {
            { Label, topic },
            { LabelUnit, "temperature" }
        };
        LabelsHumidity = new()
        {
            { Label, topic },
            { LabelUnit, "humidity" }
        };
        LabelsLinkQuality = new()
        {
            { Label, topic },
            { LabelUnit, "link_quality" }
        };
        LabelsBattery = new()
        {
            { Label, topic },
            { LabelUnit, "battery" }
        };
    }

    public void Set(SensorMessage message)
    {
        Infrastructure.Monitoring.SetGaugeValue(Name, LabelsTemperature, message.Temperature, MqttTemperatureSensorsHelper.Logger);
        Infrastructure.Monitoring.SetGaugeValue(Name, LabelsHumidity, message.Humidity, MqttTemperatureSensorsHelper.Logger);
        Infrastructure.Monitoring.SetGaugeValue(Name, LabelsLinkQuality, message.LinkQuality, MqttTemperatureSensorsHelper.Logger);
        Infrastructure.Monitoring.SetGaugeValue(Name, LabelsBattery, message.Battery, MqttTemperatureSensorsHelper.Logger);
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