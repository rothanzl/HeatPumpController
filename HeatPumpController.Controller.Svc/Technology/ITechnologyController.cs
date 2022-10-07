using HeatPumpController.Controller.Svc.Technology.Temperature;

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

    private readonly IOneWireTemperature _waterTemperatureDevice =
        new OneWireTemperature("w1_bus_master1", "28-0214811c4bff");

    private readonly IOneWireTemperature _heatherBackTemperatureDevice =
        new OneWireTemperature("w1_bus_master1", "28-0214811c4bff");
    
    
    public async Task<Temperatures> GetTemperatures(CancellationToken ct)
    {
        float tolerantField = 2;

        await Task.WhenAll(
            _waterTemperatureDevice.ReadAsync(),
            _heatherBackTemperatureDevice.ReadAsync());
        
        
        return new Temperatures( 
             10 +_random.NextSingle() * tolerantField, 
             _waterTemperatureDevice.Value,
             _heatherBackTemperatureDevice.Value);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}