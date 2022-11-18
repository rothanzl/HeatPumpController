using HeatPumpController.Controller.Svc.Technology.Sensors.Temperature;

namespace HeatPumpController.Controller.Svc.Technology.Sensors.Digital;

public abstract class DigitalMonitoringBase
{
    private const string Name = "digital_signal";
    private const string DeviceTypeLabel = TemperatureMonitoringBase.Label;
    private const string ValueTypeLabel = "value_type";
    private const string ValueTypeValueLabel = "value";
    private const string ValueTypeValidLabel = "valid";

    private Dictionary<string, string>? _valueLabels;
    private Dictionary<string, string>? _validLabels;

    private Dictionary<string, string> ValueLabels
    {
        get
        {
            if (_valueLabels == null)
                _valueLabels = CreateLabels(ValueTypeValueLabel);

            return _valueLabels;
        }
    }
    private Dictionary<string, string> ValidLabels
    {
        get
        {
            if (_validLabels == null)
                _validLabels = CreateLabels(ValueTypeValidLabel);

            return _validLabels;
        }
    }

    private Dictionary<string, string> CreateLabels(string valueTypeLabel) => new Dictionary<string, string>()
    {
        {DeviceTypeLabel, DeviceName},
        {ValueTypeLabel, valueTypeLabel}
    };
    
    protected abstract string DeviceName { get; }

    // protected void Unpublish(string[] labels)
    // {
    //     Infrastructure.Monitoring.UnpublishGauge(Name, labels);
    // }

    public void Set(DigitalSensorValue value)
    {
        Infrastructure.Monitoring.SetGaugeValue(Name, ValueLabels, Convert.ToDouble(value.Value));
        Infrastructure.Monitoring.SetGaugeValue(Name, ValidLabels, Convert.ToDouble(value.Valid));
    }

    
}

public class HdoMonitoring : DigitalMonitoringBase
{
    protected override string DeviceName => "HDO";
}