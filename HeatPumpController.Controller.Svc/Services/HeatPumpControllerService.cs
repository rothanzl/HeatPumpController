using System.Diagnostics;
using HeatPumpController.Controller.Svc.Models;
using HeatPumpController.Controller.Svc.Models.Infra;
using HeatPumpController.Controller.Svc.Technology;
using HeatPumpController.Controller.Svc.Technology.Actuators.Relay;

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
    private readonly IGarbageCollector _garbageCollector;

    private readonly IRelayHeatingCircuitBathRoom _heatingCircuitBathRoomRelay;
    private readonly IRelayHeatingCircuitBathRoomWall _heatingCircuitBathRoomWallRelay;
    private readonly IRelayHeatingCircuitBedRoom _heatingCircuitBedRoomRelay;
    private readonly IRelayHeatingCircuitKitchen _heatingCircuitKitchenRelay;
    private readonly IRelayHeatingCircuitLivingRoom _heatingCircuitLivingRoomRelay;
    private readonly IRelayHeatingCircuitSmallRoom _heatingCircuitSmallRoomRelay;
    private readonly IRelayHeatPump _heatPumpRelay;
    private readonly IRelayLowerValve _lowerValveRelay;
    private readonly IRelayUpperValve _upperValveRelay;

    public ServiceLoopIteration(ITechnologyController technologyController, IPersistentStateMediator stateMediator, 
        ILogger<ServiceLoopIteration> logger, IRelayHeatingCircuitBathRoom heatingCircuitBathRoomRelay, 
        IRelayHeatingCircuitBathRoomWall heatingCircuitBathRoomWallRelay, IRelayHeatingCircuitBedRoom heatingCircuitBedRoomRelay, 
        IRelayHeatingCircuitKitchen heatingCircuitKitchenRelay, IRelayHeatingCircuitLivingRoom heatingCircuitLivingRoomRelay, 
        IRelayHeatingCircuitSmallRoom heatingCircuitSmallRoomRelay, IRelayHeatPump heatPumpRelay, 
        IRelayLowerValve lowerValveRelay, IRelayUpperValve upperValveRelay, IGarbageCollector garbageCollector)
    {
        _technologyController = technologyController;
        _stateMediator = stateMediator;
        _logger = logger;
        _heatingCircuitBathRoomRelay = heatingCircuitBathRoomRelay;
        _heatingCircuitBathRoomWallRelay = heatingCircuitBathRoomWallRelay;
        _heatingCircuitBedRoomRelay = heatingCircuitBedRoomRelay;
        _heatingCircuitKitchenRelay = heatingCircuitKitchenRelay;
        _heatingCircuitLivingRoomRelay = heatingCircuitLivingRoomRelay;
        _heatingCircuitSmallRoomRelay = heatingCircuitSmallRoomRelay;
        _heatPumpRelay = heatPumpRelay;
        _lowerValveRelay = lowerValveRelay;
        _upperValveRelay = upperValveRelay;
        _garbageCollector = garbageCollector;
    }
    
    
    public async Task Run(CancellationToken ct)
    {

        var sw = Stopwatch.StartNew();
        var now = DateTime.Now;
        
        // Read values
        await using var resources = _technologyController.Open();
        var temperatures = await resources.GetTemperatures(ct);
        

        _stateMediator.CurrentTemperatures = new CurrentTemperatures(
            temperatures.TOut, temperatures.TWather, temperatures.THeatherBack);

        
        // Evaluate
        
        
        // Act
        _heatingCircuitBathRoomRelay.Set(_stateMediator.Relays.HeatingCircuitBathRoomRelay);
        _heatingCircuitBathRoomWallRelay.Set(_stateMediator.Relays.HeatingCircuitBathRoomWallRelay);
        _heatingCircuitBedRoomRelay.Set(_stateMediator.Relays.HeatingCircuitBedRoomRelay);
        _heatingCircuitKitchenRelay.Set(_stateMediator.Relays.HeatingCircuitKitchenRelay);
        _heatingCircuitLivingRoomRelay.Set(_stateMediator.Relays.HeatingCircuitLivingRoomRelay);
        _heatingCircuitSmallRoomRelay.Set(_stateMediator.Relays.HeatingCircuitSmallRoomRelay);
        _heatPumpRelay.Set(_stateMediator.Relays.HeatPumpRelay);
        _lowerValveRelay.Set(_stateMediator.Relays.LowerValveRelay);
        _upperValveRelay.Set(_stateMediator.Relays.UpperValveRelay);

        //_garbageCollector.Trigger();


        // Persist
        await _stateMediator.PersistIfChange();
        
        
        // Sleep
        sw.Stop();
        var sleepTime = TimeSpan.FromSeconds(1) - sw.Elapsed;
        if(sleepTime > TimeSpan.Zero)
            await Task.Delay(sleepTime, ct);
    }

    public ValueTask DisposeAsync()
    {
        
        
        return ValueTask.CompletedTask;
    }
}

