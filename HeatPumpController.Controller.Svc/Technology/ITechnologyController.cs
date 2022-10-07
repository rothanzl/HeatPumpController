using HeatPumpController.Controller.Svc.Config;
using HeatPumpController.Controller.Svc.Technology.Temperature;
using Microsoft.Extensions.Options;

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

public class TechnologyController : ITechnologyController
{
    private readonly ITechnologyResources _technologyResources;

    public TechnologyController(ITechnologyResources technologyResources)
    {
        _technologyResources = technologyResources;
    }

    public ITechnologyResources Open()
    {
        return _technologyResources;
    }
}

public class TechnologyResources : ITechnologyResources
{
    private readonly Random _random = new Random(DateTime.Now.Millisecond);
    private readonly IOptions<ControllerConfig> _config;

    private readonly IOneWireTemperature _waterTemperatureDevice =
        new OneWireTemperature("w1_bus_master1", "28-0214811c4bff");

    private readonly IOneWireTemperature _heatherBackTemperatureDevice =
        new OneWireTemperature("w1_bus_master1", "28-0214811c4bff");

    private readonly IWeatherForecast _weatherForecast;


    public TechnologyResources(IOptions<ControllerConfig> config)
    {
        _config = config;
        _weatherForecast = new WeatherForecast(config.Value.WeatherForecastApiKey);
    }

    public async Task<Temperatures> GetTemperatures(CancellationToken ct)
    {
        float tolerantField = 2;

        await Task.WhenAll(
            _waterTemperatureDevice.ReadAsync(),
            _heatherBackTemperatureDevice.ReadAsync(),
            _weatherForecast.ReadAsync());
        
        
        return new Temperatures( 
             _weatherForecast.Value, 
             _waterTemperatureDevice.Value,
             _heatherBackTemperatureDevice.Value);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}