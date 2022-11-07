
namespace HeatPumpController.Controller.Svc.Technology;

public class GpioConfig
{

    public class Pins
    {
        public const int HdoInput = 2;
        public const int OneWireTemperatureSensors = 4;
        
        // Right line  -  Top -> Down
        public const int HandleHeatPump = 14;
        public const int HandleUpperValve = 15;
        public const int HandleLowerValve = 18;
        public const int HandleExtraHeating = 23;
        
        public const int HandleHeatingCircuitBathRoom = 21;
        public const int HandleHeatingCircuitSmallRoom = 20;
        public const int HandleHeatingCircuitBedRoom = 16;
        public const int HandleHeatingCircuitKitchen = 12;
        public const int HandleHeatingCircuitLivingRoom = 1;
        public const int HandleHeatingCircuitBathRoomWall = 7;

        public const int HandleRecuperationUnitLow = 8;
        public const int Reserve = 25;
    }
}