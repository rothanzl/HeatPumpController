using HeatPumpController.Controller.Svc.Config;
using HeatPumpController.Controller.Svc.Models.Infra;
using HeatPumpController.Controller.Svc.Services;
using HeatPumpController.Controller.Svc.Technology;
using HeatPumpController.Controller.Svc.Technology.Actuators.Relay;
using HeatPumpController.Controller.Svc.Technology.Sensors.Temperature;

namespace HeatPumpController.Controller.Svc;

public static class ServiceDiRegister
{
    public static IServiceCollection RegisterControllerSvc(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddSingleton<ITechnologyController, TechnologyController>();
        services.AddSingleton<IServiceLoopIteration, ServiceLoopIteration>();
        services.AddSingleton<IPersistentStateMediator, PersistentStateMediator>();
        services.AddSingleton<ITechnologyResources, TechnologyResources>();
        services.AddSingleton<IWeatherForecast, WeatherForecast>();
        services.AddSingleton<IWaterTemperature, WaterTemperature>();
        services.AddSingleton<IHeaterBackTemperature, HeaterBackTemperature>();
        services.AddSingleton<IRelayHeatPump, RelayHeatPump>();
        services.AddSingleton<IRelayUpperValve, RelayUpperValve>();
        services.AddSingleton<IRelayLowerValve, RelayLowerValve>();
        services.AddSingleton<IRelayHeatingCircuitBathRoomWall, RelayHeatingCircuitBathRoomWall>();
        services.AddSingleton<IRelayHeatingCircuitBathRoom, RelayHeatingCircuitBathRoom>();
        services.AddSingleton<IRelayHeatingCircuitSmallRoom, RelayHeatingCircuitSmallRoom>();
        services.AddSingleton<IRelayHeatingCircuitBedRoom, RelayHeatingCircuitBedRoom>();
        services.AddSingleton<IRelayHeatingCircuitKitchen, RelayHeatingCircuitKitchen>();
        services.AddSingleton<IRelayHeatingCircuitLivingRoom, RelayHeatingCircuitLivingRoom>();
        services.AddSingleton<IRelayExtraHeating, RelayExtraHeating>();

        services.Configure<ControllerConfig>(builder.Configuration.GetSection(ControllerConfig.SectionName));
        
        services.AddHostedService<HeatPumpControllerService>();
        
        return services;
    }
}