

namespace HeatPumpController.Controller.Svc.Models;

public class SystemState
{
    public DateTime SavedAt { get; set; }

    public SetPointTemperatures SetPointTemperatures { get; set; } = new (40, 24);
    public CurrentTemperatures CurrentTemperatures { get; set; } = new(0, 0, 0);
    public RelayState RelayState { get; init; } = new();

}

public class RelayState
{
    public bool TestRelay { get; set; }
}