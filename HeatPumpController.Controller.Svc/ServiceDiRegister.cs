using HeatPumpController.Controller.Svc.Models;
using HeatPumpController.Controller.Svc.Services;
using HeatPumpController.Controller.Svc.Technology;
using Microsoft.Extensions.DependencyInjection;

namespace HeatPumpController.Controller.Svc;

public static class ServiceDiRegister
{
    public static IServiceCollection RegisterControllerSvc(this IServiceCollection services)
    {
        services.AddSingleton<ITechnologyController, DemoTechnologyController>();
        services.AddSingleton<IServiceLoopIteration, ServiceLoopIteration>();
        services.AddSingleton(typeof(IPersistentContext<>), typeof(PersistentContext<>));
        
        services.AddHostedService<HeatPumpControllerService>();
        
        return services;
    }
}