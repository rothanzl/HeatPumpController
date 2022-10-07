namespace HeatPumpController.Controller.Svc.Config;

public class ControllerConfig
{
    public const string SectionName = nameof(ControllerConfig);
    
    public string WeatherForecastApiKey { get; init; } = string.Empty;
}