using System.Text.RegularExpressions;
using Prometheus;

namespace HeatPumpController.Controller.Svc.Infrastructure;

public static class Monitoring
{
    private static readonly Dictionary<string, Gauge> Gauges = new();

    private static readonly object GaugesLocker = new();

    public static void UnpublishGauge(string name, string[]? labelNames)
    {
        var metric = GetGauge(name, labelNames);

        if (labelNames != null && labelNames.Length > 0)
        {
            metric.RemoveLabelled(labelNames);
        }
        else
        {
            metric.Unpublish();
        }
    }
    
    public static void SetGaugeValue(string name, Dictionary<string, string>? labels, double value, ILogger? logger = null)
    {
        logger?.LogInformation("Gauge[{Name}] Labels[{Labels}]", name, labels == null ? null : string.Join(",", labels.Select(kv => kv.Key+":"+kv.Value)));
        
        var labelNames = labels?.Keys.ToArray();
        var metric = GetGauge(name, labelNames);

        if (labels == null)
        {
            metric.Set(value);
        }
        else
        {
            var labelsArray = labels!.Values.ToArray();
            metric.WithLabels(labelsArray).Set(value);
        }
    }

    
    public static Gauge GetGauge(string name, string[]? labelNames)
    {
        if (labelNames == null)
            labelNames = Array.Empty<string>();
        
        lock (GaugesLocker)
        {
            if (Gauges.TryGetValue(name, out var res))
                return res;

            string metricName = NormalizeMetricName(name);
            res = Metrics.CreateGauge(metricName, name, new GaugeConfiguration()
            {
                LabelNames = labelNames
            });
            
            Gauges.Add(name, res);
            return res;
        }
    }
    
    
    private static readonly Regex ValidNameRegex = new(@"[^a-zA-Z0-9_:]");

    public static string NormalizeMetricName(string name) =>
        ValidNameRegex.Replace(name, "_");
}