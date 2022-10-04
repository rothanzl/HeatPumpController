using System;
using System.Threading;
using System.Threading.Tasks;

namespace HeatPumpController.Controller.Svc.Technology;

public interface ITechnologyController
{
    ITechnologyResources Open();
}

public interface ITechnologyResources : IAsyncDisposable
{
    Task<Temperatures> GetTemperatures(CancellationToken ct);
}

public record Temperatures(float TOut, float TWather, float THeatherBack);

public class DemoTechnologyController : ITechnologyController
{
    public ITechnologyResources Open()
    {
        return new DemoTechnologyResources();
    }
}

public class DemoTechnologyResources : ITechnologyResources
{
    private readonly Random _random = new Random(DateTime.Now.Millisecond);
    
    public Task<Temperatures> GetTemperatures(CancellationToken ct)
    {
        
        return Task.FromResult(new Temperatures( 
            _random.Next(10, 15), 
            _random.Next(30,34), 
            _random.Next(20,24)));
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}