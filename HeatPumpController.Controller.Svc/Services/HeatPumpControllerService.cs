using System.Diagnostics;
using HeatPumpController.Controller.Svc.Models;
using HeatPumpController.Controller.Svc.Models.Infra;
using HeatPumpController.Controller.Svc.Technology;
using HeatPumpController.Controller.Svc.Technology.Relay;

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
            }finally
            {
                await _serviceLoopIteration.DisposeAsync();
            }
        }
    }
}

public interface IServiceLoopIteration : IAsyncDisposable
{
    Task Run(CancellationToken ct);
}

public class ServiceLoopIteration : IServiceLoopIteration
{
    private readonly ITechnologyController _technologyController;
    private readonly IPersistentStateMediator _stateMediator;
    private readonly ILogger<ServiceLoopIteration> _logger;
    private readonly TestRelay _testRelay = new();

    public ServiceLoopIteration(ITechnologyController technologyController, IPersistentStateMediator stateMediator, ILogger<ServiceLoopIteration> logger)
    {
        _technologyController = technologyController;
        _stateMediator = stateMediator;
        _logger = logger;
    }

    private Level _level = Level.High;
    
    public async Task Run(CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        var now = DateTime.Now;
        
        // Read values
        await using var resources = _technologyController.Open();
        var temperatures = await resources.GetTemperatures(ct);
        

        await _stateMediator.SetCurrentTemperatures(new CurrentTemperatures(
            temperatures.TOut, temperatures.TWather, temperatures.THeatherBack));

        
        // Evaluate
        
        
        // Act
        _level = _level switch
        {
            Level.High => Level.Low,
            Level.Low => Level.High,
            _ => throw new ArgumentOutOfRangeException(nameof(_level), _level.ToString())
        };
        _testRelay.Set(_level);
        


        // Persist
        await _stateMediator.PersistIfTimeout(now);
        
        
        // Sleep
        sw.Stop();
        var sleepTime = TimeSpan.FromSeconds(1) - sw.Elapsed;
        if(sleepTime > TimeSpan.Zero)
            await Task.Delay(sleepTime, ct);
    }

    public ValueTask DisposeAsync()
    {
        _testRelay.Dispose();
        
        return ValueTask.CompletedTask;
    }
}

