namespace HeatPumpController.Controller.Svc.Models;

public class SystemState
{
    public DateTime SavedAt { get; set; }

    public SetPointTemperatures SetPointTemperatures { get; set; } = new (40, 24);
}