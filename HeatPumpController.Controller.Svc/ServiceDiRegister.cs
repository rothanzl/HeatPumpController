using System.Drawing.Text;
using HeatPumpController.Controller.Svc.Config;
using HeatPumpController.Controller.Svc.Models.Infra;
using HeatPumpController.Controller.Svc.Services;
using HeatPumpController.Controller.Svc.Technology;
using HeatPumpController.Controller.Svc.Technology.Relay;
using HeatPumpController.Controller.Svc.Technology.Temperature;

namespace HeatPumpController.Controller.Svc;

public static class ServiceDiRegister
{
    public static IServiceCollection RegisterControllerSvc(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddSingleton<ITechnologyController, TechnologyController>();
        services.AddSingleton<IServiceLoopIteration, ServiceLoopIteration>();
        services.AddSingleton(typeof(IPersistentContext<>), typeof(PersistentContext<>));
        services.AddSingleton<IPersistentStateMediator, PersistentStateMediator>();
        services.AddSingleton<ITechnologyResources, TechnologyResources>();
        services.AddSingleton<IWeatherForecast, WeatherForecast>();
        services.AddSingleton<IWaterTemperature, WaterTemperature>();
        services.AddSingleton<IHeaterBackTemperature, HeaterBackTemperature>();
        services.AddSingleton<TestRelay>();

        services.Configure<ControllerConfig>(builder.Configuration.GetSection(ControllerConfig.SectionName));
        
        services.AddHostedService<HeatPumpControllerService>();
        
        return services;
    }
}