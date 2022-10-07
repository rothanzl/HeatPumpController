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
        return new TechnologyResources();
    }
}

public class TechnologyResources : ITechnologyResources
{
    private readonly Random _random = new Random(DateTime.Now.Millisecond);
    
    public Task<Temperatures> GetTemperatures(CancellationToken ct)
    {
        float tolerantField = 2;
        
        return Task.FromResult(new Temperatures( 
             10 +_random.NextSingle() * tolerantField, 
             40 + _random.NextSingle() * tolerantField,
             25 + _random.NextSingle() * tolerantField));
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}