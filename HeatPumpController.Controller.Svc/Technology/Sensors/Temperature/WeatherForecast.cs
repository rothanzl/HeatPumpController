using System.Text.Json.Serialization;
using HeatPumpController.Controller.Svc.Config;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Sensors.Temperature;

public interface IWeatherForecast : ITemperatureDevice
{
    
}

public class WeatherForecast : IWeatherForecast
{
    private readonly string _apiKey;
    private readonly HttpClient _client = new();
    private readonly ILogger<WeatherForecast> _logger;
    private readonly TemperatureMonitoring _monitoring = new("out_ambience");

    private string Url => $"https://api.openweathermap.org/data/3.0/onecall?lat=50.02491690&lon=14.06218446&appid={_apiKey}&units=metric";
    private TimeSpan RefreshTimeout { get; } = TimeSpan.FromMinutes(10);
    private bool DummyTechnology { get; }
    private bool RefreshTimeoutExpired => _lastRefreshDt + RefreshTimeout < DateTime.Now; 
    
    private DateTime _lastRefreshDt = DateTime.MinValue;
    
    
    public WeatherForecast(IOptions<ControllerConfig> config, ILogger<WeatherForecast> logger)
    {
        if (string.IsNullOrWhiteSpace(config.Value.WeatherForecastApiKey))
            throw new NullReferenceException("Missing weather forecast api key");
        
        _apiKey = config.Value.WeatherForecastApiKey;
        _logger = logger;
        DummyTechnology = config.Value.DummyTechnology;
        
        Value = SensorValue.CreateInvalid();
    }

    public Task ReadAsync()
    {
        if (DummyTechnology)
            return SetDummyValue();
        
        if (RefreshTimeoutExpired)
        {
            _logger.LogInformation("Refresh data");
            return RefreshData();
        }
        
        return Task.CompletedTask;
    }

    private Task SetDummyValue()
    {
        Value = SensorValue.CreateValid(new Random().NextSingle() * 50);
        return Task.CompletedTask;
    }

    private async Task RefreshData()
    {
        HttpResponseMessage? response = null;
        try
        {
            ResponseObject.Root? responseObject = null;
            response = await _client.GetAsync(Url);
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    responseObject = await response.Content.ReadFromJsonAsync<ResponseObject.Root>();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Cannot parse response '{Response}'",
                        await response.Content.ReadAsStringAsync());
                    throw;
                }
            }
            else
            {
                _logger.LogError("Got response code {Code}", response.StatusCode);
                return;
            }

            if (responseObject == null)
            {
                _logger.LogError("Error pare response object");
                Value = SensorValue.CreateInvalid();
            }
            else
            {
                Value = SensorValue.CreateValid(responseObject.Current.Temp);
            }
        }
        catch (Exception e)
        {
            Value = SensorValue.CreateInvalid();
            _logger.LogError(e, "Error get forecast");
        }
        finally
        {
            _monitoring.Set(Value);
            _lastRefreshDt = DateTime.Now;
        }
    }
    
    

    public SensorValue Value { get; private set; }


    private class ResponseObject
    {
        
        public record Current
        {
            [JsonPropertyName("temp")] public float Temp { get; init; }
        }
        
        public record Root
        {
            [JsonPropertyName("current")] public Current Current { get; init; } = new();
        }
    
    }
}