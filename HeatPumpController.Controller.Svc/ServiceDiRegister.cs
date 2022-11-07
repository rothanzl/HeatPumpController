using HeatPumpController.Controller.Svc.Config;
using HeatPumpController.Controller.Svc.Models.Infra;
using HeatPumpController.Controller.Svc.Services;
using HeatPumpController.Controller.Svc.Technology;
using HeatPumpController.Controller.Svc.Technology.Actuators.Relay;
using HeatPumpController.Controller.Svc.Technology.Control.Equitherm;
using HeatPumpController.Controller.Svc.Technology.Sensors.Digital;
using HeatPumpController.Controller.Svc.Technology.Sensors.Temperature;
using HeatPumpController.Controller.Svc.Technology.Sensors.Temperature.Mqtt;

namespace HeatPumpController.Controller.Svc;

public static class ServiceDiRegister
{
    public static IServiceCollection RegisterControllerSvc(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddSingleton<IServiceLoopIteration, ServiceLoopIteration>();
        services.AddSingleton<IPersistentStateMediator, PersistentStateMediator>();
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
        services.AddSingleton<IRelayReserve, RelayReserve>();
        services.AddSingleton<IRelayRecuperationUnitLow, RelayRecuperationUnitLow>();
        services.AddSingleton<ITechnologyService, TechnologyService>();
        services.AddSingleton<IMqttTemperatureSensorsHelper, MqttTemperatureSensorsHelper>();
        services.AddSingleton<ITemperaturesFacade, TemperaturesFacade>();   
        services.AddSingleton<IBathRoomTemperatureSensor, BathRoomTemperatureSensor>();
        services.AddSingleton<ISmallRoomTemperatureSensor, SmallRoomTemperatureSensor>();
        services.AddSingleton<IBedRoomTemperatureSensor, BedRoomTemperatureSensor>();
        services.AddSingleton<ILivingRoomTemperatureSensor, LivingRoomTemperatureSensor>();
        services.AddSingleton<IKitchenTemperatureSensor, KitchenTemperatureSensor>();
        services.AddSingleton<IHdoIndicator, HdoIndicator>();
        
        services.Configure<ControllerConfig>(builder.Configuration.GetSection(ControllerConfig.SectionName));
        
        services.AddHostedService<HeatPumpControllerService>();
        services.AddHostedService<MqttService>();
        
        return services;
    }
}