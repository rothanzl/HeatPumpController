using HeatPumpController.Controller.Svc.Models;
using HeatPumpController.Controller.Svc.Models.Infra;

namespace HeatPumpController.Web.Services;

public interface IRecuperationUnitViewModel 
{
    bool AutomationMode { get; set; }
    bool Paused { get; }
    DateTime PausedUntil { get; }
    void Pause(TimeSpan pauseDelay);
    
    RecuperationUnitCyclingViewModel Cycling { get; }
}

public class RecuperationUnitCyclingViewModel : IRecuperationUnitCycling
{
    private readonly IPersistentContext<SystemState> _persistent;
    private RecuperationUnitCyclingState State => _persistent.State.RecuperationUnit.Cycling;

    public RecuperationUnitCyclingViewModel(IPersistentContext<SystemState> persistent)
    {
        _persistent = persistent;
    }

    public bool Enabled
    {
        get => State.Enabled;
        set
        {
            if (State.Enabled == false && value) // Turning on
            {
                CycleChange = DateTime.Now;
            }
            
            
            State.Enabled = value;
        }
    }

    public DateTime CycleChange 
    { 
        get => State.CycleChange; 
        set => State.CycleChange = value; 
    }

    public TimeSpan Interval
    {
        get => State.Interval;
        set => State.Interval = value;
    }
}


public class RecuperationUnitViewModel : IRecuperationUnitViewModel
{
    private readonly IPersistentContext<SystemState> _persistent;
    public RecuperationUnitCyclingViewModel Cycling { get; }

    public RecuperationUnitViewModel(IPersistentContext<SystemState> persistent, RecuperationUnitCyclingViewModel cycling)
    {
        _persistent = persistent;
        Cycling = cycling;
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