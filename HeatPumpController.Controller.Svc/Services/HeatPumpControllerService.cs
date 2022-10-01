using System.Diagnostics;
using HeatPumpController.Controller.Svc.Models;
using HeatPumpController.Controller.Svc.Technology;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HeatPumpController.Controller.Svc.Services;

public class HeatPumpControllerService : IHostedService
{
    private Task? _loopTask;
    private readonly IServiceLoopIteration _serviceLoopIteration;
    private readonly ILogger<HeatPumpControllerService> _logger;


    public HeatPumpControllerService(IServiceLoopIteration serviceLoopIteration, ILogger<HeatPumpControllerService> logger)
    {
        _serviceLoopIteration = serviceLoopIteration;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _loopTask = ServiceLoop(cancellationToken);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        while (_loopTask is { IsCompleted: false })
        {
            await Task.Delay(100);
        }
    }

    private async Task ServiceLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await _serviceLoopIteration.Run(ct);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Service loop error");
            }
        }
    }
}

public interface IServiceLoopIteration
{
    Task Run(CancellationToken ct);
}

public class ServiceLoopIteration : IServiceLoopIteration
{
    private readonly ITechnologyController _technologyController;
    private readonly IPersistentContext<SystemState> _persistention;

    private SystemState SystemState => _persistention.State;

    public ServiceLoopIteration(ITechnologyController technologyController, 
        IPersistentContext<SystemState> persistention)
    {
        _technologyController = technologyController;
        _persistention = persistention;
    }

    public async Task Run(CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        var now = DateTime.Now;
        
        // Read values
        await using var resources = _technologyController.Open();
        var temperatures = await resources.GetTemperatures(ct);

        
        // Evaluate
        
        
        // Act


        // Persist
        if (SystemState.SavedAt + TimeSpan.FromMinutes(1) < now)
        {
            SystemState.SavedAt = now;
            await _persistention.WriteIfChange();            
        }
        
        
        // Sleep
        sw.Stop();
        var sleepTime = TimeSpan.FromSeconds(1) - sw.Elapsed;
        if(sleepTime > TimeSpan.Zero)
            await Task.Delay(sleepTime, ct);
    }
}
