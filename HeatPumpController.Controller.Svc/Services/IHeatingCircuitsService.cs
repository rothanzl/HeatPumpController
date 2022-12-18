using HeatPumpController.Controller.Svc.Models;
using HeatPumpController.Controller.Svc.Models.Infra;
using HeatPumpController.Controller.Svc.Technology.Sensors.Temperature.Mqtt;

namespace HeatPumpController.Controller.Svc.Services;

public interface IHeatingCircuitsService
{
    Task Act();
}

public class HeatingCircuitsService : IHeatingCircuitsService
{
    private readonly IPersistentContext<SystemState> _persistence;
    private readonly IRoomTemperatures _roomTemperatures;

    private const bool ClosedValue = true;
    private const bool OpenedValue = !ClosedValue;

    private RelayState RelayState => _persistence.State.RelayState;

    public HeatingCircuitsService(IPersistentContext<SystemState> persistence, IRoomTemperatures roomTemperatures)
    {
        _persistence = persistence;
        _roomTemperatures = roomTemperatures;
    }

    public Task Act()
    {
        AutomationBranch();


        if (RelayState is
            {
                HeatingCircuitBathRoomRelay: ClosedValue,
                HeatingCircuitBathRoomWallRelay: ClosedValue,
                HeatingCircuitKitchenRelay: ClosedValue,
                HeatingCircuitLivingRoomRelay: ClosedValue,
                HeatingCircuitBedRoomRelay: ClosedValue,
                HeatingCircuitSmallRoomRelay: ClosedValue,
            })
        {
            RelayState.HeatingCircuitBathRoomRelay = OpenedValue;
            RelayState.HeatingCircuitBathRoomWallRelay = OpenedValue;
            RelayState.HeatingCircuitKitchenRelay = OpenedValue;
            RelayState.HeatingCircuitLivingRoomRelay = OpenedValue;
            RelayState.HeatingCircuitBedRoomRelay = OpenedValue;
            RelayState.HeatingCircuitSmallRoomRelay = OpenedValue;
        }

        return _persistence.WriteIfChangeAsync();
    }

    private void AutomationBranch()
    {
        if (_persistence.State.ProcessState.Automation == false)
            return;
        
        var bathRoomOverheated = _roomTemperatures.BathRoom is { Valid: true, Value: >= RoomTemperaturesSetPoints.BathRoom };
        var kitchenOverheated = _roomTemperatures.Kitchen is { Valid: true, Value: >= RoomTemperaturesSetPoints.Kitchen };
        var livingRoomOverheated = _roomTemperatures.LivingRoom is { Valid: true, Value: >= RoomTemperaturesSetPoints.LivingRoom };
        var bedRoomOverheated = _roomTemperatures.BedRoom is { Valid: true, Value: >= RoomTemperaturesSetPoints.BedRoom };
        var smallRoomOverheated = _roomTemperatures.SmallRoom is { Valid: true, Value: >= RoomTemperaturesSetPoints.SmallRoom };
        
        
        RelayState.HeatingCircuitBathRoomRelay = bathRoomOverheated ? ClosedValue : OpenedValue;
        RelayState.HeatingCircuitBathRoomWallRelay = bathRoomOverheated ? ClosedValue : OpenedValue;
        RelayState.HeatingCircuitKitchenRelay = kitchenOverheated ? ClosedValue : OpenedValue;;
        RelayState.HeatingCircuitLivingRoomRelay = livingRoomOverheated ? ClosedValue : OpenedValue;;
        RelayState.HeatingCircuitBedRoomRelay = bedRoomOverheated ? ClosedValue : OpenedValue;;
        RelayState.HeatingCircuitSmallRoomRelay = smallRoomOverheated ? ClosedValue : OpenedValue;;
    }
}