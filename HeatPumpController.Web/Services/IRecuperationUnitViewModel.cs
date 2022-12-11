using HeatPumpController.Controller.Svc.Models;
using HeatPumpController.Controller.Svc.Models.Infra;

namespace HeatPumpController.Web.Services;

public interface IRecuperationUnitViewModel
{
    bool AutomationMode { get; set; }
    bool Paused { get; }
    DateTime PausedUntil { get; }
    void Pause(TimeSpan pauseDelay);
}

public class RecuperationUnitViewModel : IRecuperationUnitViewModel
{
    private readonly IPersistentContext<SystemState> _persistent;

    public RecuperationUnitViewModel(IPersistentContext<SystemState> persistent)
    {
        _persistent = persistent;
    }

    public bool AutomationMode
    {
        get => _persistent.State.RecuperationUnit.AutomationMode;
        set
        {
            _persistent.State.RecuperationUnit.AutomationMode = value;
            _persistent.WriteIfChange();
        }
    }

    public bool Paused => _persistent.State.RecuperationUnit.Paused;
    public DateTime PausedUntil => _persistent.State.RecuperationUnit.PausedUntil;

    public void Pause(TimeSpan pauseDelay)
    {
        _persistent.State.RecuperationUnit.Paused = true;
        _persistent.State.RecuperationUnit.PausedUntil = DateTime.Now + pauseDelay;
        _persistent.WriteIfChange();
    }
}