namespace HeatPumpController.Controller.Svc.Models;

public record SetPointTemperatures(float WaterTemperature, float HeatingTemperature);
public record CurrentTemperatures(float OutTemperature, float WaterTemperature, float HeatingTemperature);

