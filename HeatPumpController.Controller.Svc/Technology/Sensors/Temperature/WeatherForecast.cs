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
        
        // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public record Current(
        [property: JsonPropertyName("dt")] int Dt,
        [property: JsonPropertyName("sunrise")] int Sunrise,
        [property: JsonPropertyName("sunset")] int Sunset,
        [property: JsonPropertyName("temp")] float Temp,
        [property: JsonPropertyName("feels_like")] double FeelsLike,
        [property: JsonPropertyName("pressure")] int Pressure,
        [property: JsonPropertyName("humidity")] int Humidity,
        [property: JsonPropertyName("dew_point")] double DewPoint,
        [property: JsonPropertyName("uvi")] double Uvi,
        [property: JsonPropertyName("clouds")] int Clouds,
        [property: JsonPropertyName("visibility")] int Visibility,
        [property: JsonPropertyName("wind_speed")] double WindSpeed,
        [property: JsonPropertyName("wind_deg")] int WindDeg,
        [property: JsonPropertyName("weather")] IReadOnlyList<Weather> Weather
    );

    public record Daily(
        [property: JsonPropertyName("dt")] int Dt,
        [property: JsonPropertyName("sunrise")] int Sunrise,
        [property: JsonPropertyName("sunset")] int Sunset,
        [property: JsonPropertyName("moonrise")] int Moonrise,
        [property: JsonPropertyName("moonset")] int Moonset,
        [property: JsonPropertyName("moon_phase")] double MoonPhase,
        [property: JsonPropertyName("temp")] Temp Temp,
        [property: JsonPropertyName("feels_like")] FeelsLike FeelsLike,
        [property: JsonPropertyName("pressure")] int Pressure,
        [property: JsonPropertyName("humidity")] int Humidity,
        [property: JsonPropertyName("dew_point")] double DewPoint,
        [property: JsonPropertyName("wind_speed")] double WindSpeed,
        [property: JsonPropertyName("wind_deg")] int WindDeg,
        [property: JsonPropertyName("wind_gust")] double WindGust,
        [property: JsonPropertyName("weather")] IReadOnlyList<Weather> Weather,
        [property: JsonPropertyName("clouds")] int Clouds,
        [property: JsonPropertyName("pop")] double Pop,
        [property: JsonPropertyName("uvi")] double Uvi,
        [property: JsonPropertyName("rain")] double? Rain
    );

    public record FeelsLike(
        [property: JsonPropertyName("day")] double Day,
        [property: JsonPropertyName("night")] double Night,
        [property: JsonPropertyName("eve")] double Eve,
        [property: JsonPropertyName("morn")] double Morn
    );

    public record Hourly(
        [property: JsonPropertyName("dt")] int Dt,
        [property: JsonPropertyName("temp")] double Temp,
        [property: JsonPropertyName("feels_like")] double FeelsLike,
        [property: JsonPropertyName("pressure")] int Pressure,
        [property: JsonPropertyName("humidity")] int Humidity,
        [property: JsonPropertyName("dew_point")] double DewPoint,
        [property: JsonPropertyName("uvi")] double Uvi,
        [property: JsonPropertyName("clouds")] int Clouds,
        [property: JsonPropertyName("visibility")] int Visibility,
        [property: JsonPropertyName("wind_speed")] double WindSpeed,
        [property: JsonPropertyName("wind_deg")] int WindDeg,
        [property: JsonPropertyName("wind_gust")] double WindGust,
        [property: JsonPropertyName("weather")] IReadOnlyList<Weather> Weather,
        [property: JsonPropertyName("pop")] double Pop,
        [property: JsonPropertyName("rain")] Rain Rain
    );

    public record Minutely(
        [property: JsonPropertyName("dt")] int Dt,
        [property: JsonPropertyName("precipitation")] float Precipitation
    );

    public record Rain(
        [property: JsonPropertyName("1h")] double _1h
    );

    public record Root(
        [property: JsonPropertyName("lat")] double Lat,
        [property: JsonPropertyName("lon")] double Lon,
        [property: JsonPropertyName("timezone")] string Timezone,
        [property: JsonPropertyName("timezone_offset")] int TimezoneOffset,
        [property: JsonPropertyName("current")] Current Current,
        [property: JsonPropertyName("minutely")] IReadOnlyList<Minutely> Minutely,
        [property: JsonPropertyName("hourly")] IReadOnlyList<Hourly> Hourly,
        [property: JsonPropertyName("daily")] IReadOnlyList<Daily> Daily
    );

    public record Temp(
        [property: JsonPropertyName("day")] double Day,
        [property: JsonPropertyName("min")] double Min,
        [property: JsonPropertyName("max")] double Max,
        [property: JsonPropertyName("night")] double Night,
        [property: JsonPropertyName("eve")] double Eve,
        [property: JsonPropertyName("morn")] double Morn
    );

    public record Weather(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("main")] string Main,
        [property: JsonPropertyName("description")] string Description,
        [property: JsonPropertyName("icon")] string Icon
    );



    
    }
}