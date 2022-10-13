namespace HeatPumpController.Controller.Svc.Technology.Relay;

public class TestRelay : RelayHandlerBase, IRelayHandler
{
    public TestRelay() : base(18)
    {
    }
}