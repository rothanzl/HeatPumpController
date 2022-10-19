using System.Diagnostics;

namespace HeatPumpController.Controller.Svc.Services;

public interface IGarbageCollector
{
    void Trigger();
}

public class GarbageCollector : IGarbageCollector
{
    private readonly Stopwatch _stopWatch = new();
    private readonly ILogger<GarbageCollector> _logger;
    
    private TimeSpan TriggerTimeout { get; } = TimeSpan.FromMinutes(5);

    public GarbageCollector(ILogger<GarbageCollector> logger)
    {
        _logger = logger;
        _stopWatch.Start();
    }

    public void Trigger()
    {
        if(_stopWatch.Elapsed > TriggerTimeout)
        {
            TriggerInternal();
            _stopWatch.Restart();
        }
    }

    private void TriggerInternal()
    {
        var sp = Stopwatch.StartNew();
        GC.Collect();
        sp.Stop();
        _logger.LogInformation("GC Triggered [Elapsed:{Elapsed}]", sp.Elapsed.ToString());
    }
}