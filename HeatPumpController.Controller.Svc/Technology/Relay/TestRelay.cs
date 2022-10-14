using HeatPumpController.Controller.Svc.Config;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Technology.Relay;

public class TestRelay : RelayHandlerBase, IRelayHandler
{
    public TestRelay(IOptions<ControllerConfig> config) : base(18, config)
    {
    }
}