using HeatPumpController.Controller.Svc.Technology.Sensors.Digital;

namespace HeatPumpController.Controller.Svc.Technology.Actuators.Relay;

public static class RelayMonitoring
{
    public static IDigitalMonitoring ExtraHeating { get; } = new ExtraHeatingMonitoring();
    public static IDigitalMonitoring HeatingCircuitBathRoom { get; } = new HeatingCircuitBathRoomMonitoring();
    public static IDigitalMonitoring HeatingCircuitBathRoomWall { get; } = new HeatingCircuitBathRoomWallMonitoring();
    public static IDigitalMonitoring HeatingCircuitBedRoom { get; } = new HeatingCircuitBedRoomMonitoring();
    public static IDigitalMonitoring HeatingCircuitKitchen { get; } = new HeatingCircuitKitchenMonitoring();
    public static IDigitalMonitoring HeatingCircuitLivingRoom { get; } = new HeatingCircuitLivingRoomMonitoring();
    public static IDigitalMonitoring HeatingCircuitSmallRoom { get; } = new HeatingCircuitSmallRoomMonitoring();
    public static IDigitalMonitoring HeatPump { get; } = new HeatPumpMonitoring();
    public static IDigitalMonitoring LowerValve { get; } = new LowerValveMonitoring();
    public static IDigitalMonitoring UpperValve { get; } = new UpperValveMonitoring();
    public static IDigitalMonitoring RecuperationUnitPower { get; } = new RecuperationUnitPowerMonitoring();
    public static IDigitalMonitoring RecuperationUnitIntensity { get; } = new RecuperationUnitIntensityMonitoring();


    private class RecuperationUnitIntensityMonitoring : DigitalMonitoringBase
    {
        protected override string DeviceName { get; } = "relay_recuperation_unit_intensity";
    }
    private class RecuperationUnitPowerMonitoring : DigitalMonitoringBase
    {
        protected override string DeviceName { get; } = "relay_recuperation_unit_power";
    }
    private class UpperValveMonitoring : DigitalMonitoringBase
    {
        protected override string DeviceName { get; } = "relay_upper_valve";
    }
    private class LowerValveMonitoring : DigitalMonitoringBase
    {
        protected override string DeviceName { get; } = "relay_lower_valve";
    }
    private class HeatPumpMonitoring : DigitalMonitoringBase
    {
        protected override string DeviceName { get; } = "relay_heat_pump";
    }
    private class HeatingCircuitSmallRoomMonitoring : DigitalMonitoringBase
    {
        protected override string DeviceName { get; } = "relay_heating_circuit_small_room";
    }
    private class HeatingCircuitLivingRoomMonitoring : DigitalMonitoringBase
    {
        protected override string DeviceName { get; } = "relay_heating_circuit_living_room";
    }
    private class HeatingCircuitKitchenMonitoring : DigitalMonitoringBase
    {
        protected override string DeviceName { get; } = "relay_heating_circuit_kitchen";
    }
    private class HeatingCircuitBedRoomMonitoring : DigitalMonitoringBase
    {
        protected override string DeviceName { get; } = "relay_heating_circuit_bed_room";
    }
    private class HeatingCircuitBathRoomWallMonitoring : DigitalMonitoringBase
    {
        protected override string DeviceName { get; } = "relay_heating_circuit_bath_room_wall";
    }
    private class HeatingCircuitBathRoomMonitoring : DigitalMonitoringBase
    {
        protected override string DeviceName { get; } = "relay_heating_circuit_bath_room";
    }
    private class ExtraHeatingMonitoring : DigitalMonitoringBase
    {
        protected override string DeviceName { get; } = "relay_extra_heating";
    }
}