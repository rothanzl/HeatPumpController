using System.Diagnostics;
using HeatPumpController.Controller.Svc.Models;
using HeatPumpController.Controller.Svc.Models.Infra;
using HeatPumpController.Controller.Svc.Technology.Actuators.Relay;
using HeatPumpController.Controller.Svc.Technology.Sensors.Digital;

namespace HeatPumpController.Controller.Svc.Services;

public class HeatPumpControllerService : IHostedService, IDisposable
{
    private readonly IServiceLoopIteration _serviceLoopIteration;
    private readonly ILogger<HeatPumpControllerService> _logger;

    private Timer? _timer;
    private TimeSpan TimerPeriod { get; } = TimeSpan.FromSeconds(2.5);


    public HeatPumpControllerService(IServiceLoopIteration serviceLoopIteration, ILogger<HeatPumpControllerService> logger)
    {
        _serviceLoopIteration = serviceLoopIteration;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWord, null, TimerPeriod, TimerPeriod);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        _logger.LogInformation("Service stopped");
        return Task.CompletedTask;
    }

    private async void DoWord(object? o)
    {
        Stopwatch sw = Stopwatch.StartNew();
        try
        {
            await _serviceLoopIteration.Run(CancellationToken.None);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Service loop exception");
        }
        
        sw.Stop();
        if (sw.Elapsed >= TimerPeriod)
        {
            _logger.LogError("Service loop took {Elapse}", sw.Elapsed.ToString());
        }
        else if (sw.Elapsed >= TimerPeriod / 2)
        {
            _logger.LogWarning("Service loop took {Elapse}", sw.Elapsed.ToString());
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}

public interface IServiceLoopIteration
{
    Task Run(CancellationToken ct);
}

public class ServiceLoopIteration : IServiceLoopIteration
{
    private readonly IPersistentStateMediator _stateMediator;
    private readonly ILogger<ServiceLoopIteration> _logger;

    private readonly IRelayHeatingCircuitBathRoom _heatingCircuitBathRoomRelay;
    private readonly IRelayHeatingCircuitBathRoomWall _heatingCircuitBathRoomWallRelay;
    private readonly IRelayHeatingCircuitBedRoom _heatingCircuitBedRoomRelay;
    private readonly IRelayHeatingCircuitKitchen _heatingCircuitKitchenRelay;
    private readonly IRelayHeatingCircuitLivingRoom _heatingCircuitLivingRoomRelay;
    private readonly IRelayHeatingCircuitSmallRoom _heatingCircuitSmallRoomRelay;
    private readonly IRelayHeatPump _heatPumpRelay;
    private readonly IRelayLowerValve _lowerValveRelay;
    private readonly IRelayUpperValve _upperValveRelay;
    private readonly IRelayExtraHeating _extraHeatingRelay;
    private readonly ITechnologyService _technologyService;
    private readonly ITemperaturesFacade _temperatures;
    private readonly IHdoIndicator _hdoIndicator;

    public ServiceLoopIteration(IPersistentStateMediator stateMediator, 
        ILogger<ServiceLoopIteration> logger, IRelayHeatingCircuitBathRoom heatingCircuitBathRoomRelay, 
        IRelayHeatingCircuitBathRoomWall heatingCircuitBathRoomWallRelay, IRelayHeatingCircuitBedRoom heatingCircuitBedRoomRelay, 
        IRelayHeatingCircuitKitchen heatingCircuitKitchenRelay, IRelayHeatingCircuitLivingRoom heatingCircuitLivingRoomRelay, 
        IRelayHeatingCircuitSmallRoom heatingCircuitSmallRoomRelay, IRelayHeatPump heatPumpRelay, 
        IRelayLowerValve lowerValveRelay, IRelayUpperValve upperValveRelay, IRelayExtraHeating extraHeatingRelay, 
        ITechnologyService technologyService, IHdoIndicator hdoIndicator, ITemperaturesFacade temperatures)
    {
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
        _extraHeatingRelay = extraHeatingRelay;
        _technologyService = technologyService;
        _hdoIndicator = hdoIndicator;
        _temperatures = temperatures;
    }

    
    
    
    public async Task Run(CancellationToken ct)
    {
        // Read values
        await Task.WhenAll(
            _hdoIndicator.ReadAsync(),
            _temperatures.ReadAll()
            );
        
        if (ReadFailed())
            return;
        
        
        _stateMediator.CurrentTemperatures = new CurrentTemperatures(
            _temperatures.OutAmbient, 
            _temperatures.WaterReservoir,
            _temperatures.HeaterBack);
        
        // Evaluate
        _technologyService.Evaluate();
        
        
        // Act
        try
        {
            _heatingCircuitBathRoomRelay.Set(_stateMediator.Relays.HeatingCircuitBathRoomRelay);
            _heatingCircuitBathRoomWallRelay.Set(_stateMediator.Relays.HeatingCircuitBathRoomWallRelay);
            _heatingCircuitBedRoomRelay.Set(_stateMediator.Relays.HeatingCircuitBedRoomRelay);
            _heatingCircuitKitchenRelay.Set(_stateMediator.Relays.HeatingCircuitKitchenRelay);
            _heatingCircuitLivingRoomRelay.Set(_stateMediator.Relays.HeatingCircuitLivingRoomRelay);
            _heatingCircuitSmallRoomRelay.Set(_stateMediator.Relays.HeatingCircuitSmallRoomRelay);
            _heatPumpRelay.Set(_stateMediator.Relays.HeatPumpRelay);
            _lowerValveRelay.Set(_stateMediator.Relays.LowerValveRelay);
            _upperValveRelay.Set(_stateMediator.Relays.UpperValveRelay);
            _extraHeatingRelay.Set(_stateMediator.Relays.ExtraHeatingRelay);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Cannot set relay");
        }
        
        
        // Persist
        await _stateMediator.PersistIfChange();
    }

    bool ReadFailed() => !_hdoIndicator.ValidValue || _temperatures.ReadFailed();
}

