

namespace HeatPumpController.Controller.Svc.Models;

public class SystemState
{
    public const string Name = "SystemState_v1";
    
    public SetPointTemperatures SetPointTemperatures { get; set; } = new (40, 24);
    public RelayState RelayState { get; init; } = new();

}

public class RelayState
{
    public bool TestRelay { get; set; }
}