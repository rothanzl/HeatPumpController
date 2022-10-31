using HeatPumpController.Controller.Svc.Config;
using HeatPumpController.Controller.Svc.Models;
using HeatPumpController.Controller.Svc.Models.Infra;
using HeatPumpController.Controller.Svc.Technology.Sensors.Digital;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Services;

public interface ITechnologyService
{
    void Evaluate();
}

public class TechnologyService : ITechnologyService
{
    private readonly IPersistentStateMediator _stateMediator;
    private readonly TechnologyConfig _config;
    private readonly ITemperaturesFacade _temperatures;
    private readonly IHdoIndicator _hdoIndicator;

    public TechnologyService(IPersistentStateMediator stateMediator, IOptions<ControllerConfig> options, IHdoIndicator hdoIndicator, ITemperaturesFacade temperatures)
    {
        _stateMediator = stateMediator;
        _hdoIndicator = hdoIndicator;
        _temperatures = temperatures;
        _config = options.Value.TechnologyConfig;
    }

    public void Evaluate()
    {
        var beginState = _stateMediator.ProcessState.State;
        
        EvaluateInternal();

        if (beginState != _stateMediator.ProcessState.State)
        {
            _stateMediator.ProcessState.ChangeTimeStamp = DateTime.Now;
            _stateMediator.StateChanged();
        }
    }

    private void EvaluateInternal()
    {
        if (!_stateMediator.ProcessState.Automation)
            return;

        if (ReadFailed())
        {
            _stateMediator.ProcessState.State = ProcessStateEnum.DoNothing;
            return;
        }
        

        if (!_hdoIndicator.Value.Value)
        {
            _stateMediator.ProcessState.State = ProcessStateEnum.DoNothing;
            return;
        }

        
        
        EvaluateHeater();
        EvaluateWaterTemp();
    }

    private bool ReadFailed() => !_hdoIndicator.ValidValue || _temperatures.ReadFailed();

    private void EvaluateHeater()
    {
        
    }

    private void EvaluateWaterTemp()
    {
        var current = _temperatures.WaterReservoir;
        var setPoint = _stateMediator.SetPointTemperatures.WaterTemperature;
        var lTol = _config.TemperatureTolerance.LowerTolerance;
        var hTol = _config.TemperatureTolerance.UpperTolerance;
        
        if (current < setPoint - lTol)
        {
            _stateMediator.ProcessState.State = ProcessStateEnum.HeatWaterReservoir;
        }
        else if(_stateMediator.ProcessState.State == ProcessStateEnum.HeatWaterReservoir &&
                current > setPoint + hTol)
        {
            _stateMediator.ProcessState.State = ProcessStateEnum.DoNothing;
        }
    }
}