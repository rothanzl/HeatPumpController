using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using HeatPumpController.Controller.Svc.Models;
using HeatPumpController.Controller.Svc.Models.Infra;
using HeatPumpController.Controller.Svc.Technology;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HeatPumpController.Controller.Svc.Services;

public class HeatPumpControllerService : IHostedService
{
    private Task? _loopTask;
    private readonly IServiceLoopIteration _serviceLoopIteration;
    private readonly ILogger<HeatPumpControllerService> _logger;
    private readonly CancellationTokenSource _cts;


    public HeatPumpControllerService(IServiceLoopIteration serviceLoopIteration, ILogger<HeatPumpControllerService> logger)
    {
        _serviceLoopIteration = serviceLoopIteration;
        _logger = logger;

        _cts = new CancellationTokenSource();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _loopTask = ServiceLoop(_cts.Token);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        
        while (_loopTask is { IsCompleted: false })
        {
            await Task.Delay(100);
        }
        
        _logger.LogInformation("Service stopped");
    }

    private async Task ServiceLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await _serviceLoopIteration.Run(ct);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Service cancelled");
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
    private readonly IPersistentStateMediator _stateMediator;

    public ServiceLoopIteration(ITechnologyController technologyController, IPersistentStateMediator stateMediator)
    {
        _technologyController = technologyController;
        _stateMediator = stateMediator;
    }

    public async Task Run(CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        var now = DateTime.Now;
        
        // Read values
        await using var resources = _technologyController.Open();
        var temperatures = await resources.GetTemperatures(ct);

        await _stateMediator.SetSetPointTemperatures(new SetPointTemperatures(temperatures.TWather, temperatures.THeatherBack));

        
        // Evaluate
        
        
        // Act


        // Persist
        await _stateMediator.PersistIfTimeout(now);
        
        
        // Sleep
        sw.Stop();
        var sleepTime = TimeSpan.FromSeconds(1) - sw.Elapsed;
        if(sleepTime > TimeSpan.Zero)
            await Task.Delay(sleepTime, ct);
    }
}

