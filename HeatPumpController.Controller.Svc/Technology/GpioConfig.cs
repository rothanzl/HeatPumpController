using System.CodeDom;

namespace HeatPumpController.Controller.Svc.Technology;

public class GpioConfig
{

    public class Pins
    {
        public const int HdoInput = 2;
        public const int OneWireTemperatureSensors = 4;
        
        public const int HandleHeatPump = 14;
        public const int HandleUpperValve = 15;
        public const int HandleLowerValve = 18;
        
        public const int HandleHeatingCircuitBathRoomWall = 23;
        public const int HandleHeatingCircuitBathRoom = 24;
        public const int HandleHeatingCircuitSmallRoom = 25;
        public const int HandleHeatingCircuitBedRoom = 8;
        public const int HandleHeatingCircuitKitchen = 7;
        public const int HandleHeatingCircuitLivingRoom = 1;

        public const int HandleExtraHeating = 12;
    }
}