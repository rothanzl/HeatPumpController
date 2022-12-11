

namespace HeatPumpController.Controller.Svc.Models;

public class SystemState
{
    public const string Name = "SystemState_v1";
    
    public SetPointTemperatures SetPointTemperatures { get; set; } = new (40, 24);
    public RelayState RelayState { get; init; } = new();
    public ProcessState ProcessState { get; init; } = new();

}

public interface IRelayState
{
    bool HeatingCircuitBathRoomRelay { get; set; }
    bool HeatingCircuitBathRoomWallRelay { get; set; }
    bool HeatingCircuitBedRoomRelay { get; set; }
    bool HeatingCircuitKitchenRelay { get; set; }
    bool HeatingCircuitLivingRoomRelay { get; set; }
    bool HeatingCircuitSmallRoomRelay { get; set; }
    bool LowerValveRelay { get; set; }
    bool UpperValveRelay { get; set; }
    bool HeatPumpRelay { get; set; }
    bool ExtraHeatingRelay { get; set; }
    bool RecuperationUnitPower { get; set; } 
    bool RecuperationUnitIntensity { get; set; }
}

public class RelayState : IRelayState
{
    public bool HeatingCircuitBathRoomRelay { get; set; }
    public bool HeatingCircuitBathRoomWallRelay { get; set; }
    public bool HeatingCircuitBedRoomRelay { get; set; }
    public bool HeatingCircuitKitchenRelay { get; set; }
    public bool HeatingCircuitLivingRoomRelay { get; set; }
    public bool HeatingCircuitSmallRoomRelay { get; set; }
    public bool LowerValveRelay { get; set; }
    public bool UpperValveRelay { get; set; }
    public bool HeatPumpRelay { get; set; }
    public bool ExtraHeatingRelay { get; set; }
    public bool RecuperationUnitPower { get; set; }
    public bool RecuperationUnitIntensity { get; set; }
}

public interface IProcessState
{
    bool Automation { get; set; }
}

public class ProcessState : IProcessState
{
    public bool Automation { get; set; }
    public ProcessStateEnum State { get; set; } = ProcessStateEnum.DoNothing;
    public DateTime ChangeTimeStamp { get; set; }
}

public enum ProcessStateEnum
{
    DoNothing,
    HeatWaterReservoir
}