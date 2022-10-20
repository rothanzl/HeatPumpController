using HeatPumpController.Controller.Svc.Config;
using HeatPumpController.Controller.Svc.Models;
using HeatPumpController.Controller.Svc.Models.Infra;
using HeatPumpController.Controller.Svc.Technology;
using Microsoft.Extensions.Options;

namespace HeatPumpController.Controller.Svc.Services;

public interface ITechnologyService
{
    void Evaluate(Temperatures temperatures);
}

public class TechnologyService : ITechnologyService
{
    private readonly IPersistentStateMediator _stateMediator;
    private readonly TechnologyConfig _config;

    public TechnologyService(IPersistentStateMediator stateMediator, IOptions<ControllerConfig> options)
    {
        _stateMediator = stateMediator;
        _config = options.Value.TechnologyConfig;
    }

    public void Evaluate(Temperatures temperatures)
    {
        if (!_stateMediator.ProcessState.Automation)
            return;

        var beginState = _stateMediator.ProcessState.State;
        
        EvaluateHeater(temperatures);
        EvaluateWaterTemp(temperatures);

        if (beginState != _stateMediator.ProcessState.State)
            _stateMediator.StateChanged();
    }

    private void EvaluateHeater(Temperatures temperatures)
    {
        
    }

    private void EvaluateWaterTemp(Temperatures temperatures)
    {
        if (temperatures.TWather < _stateMediator.SetPointTemperatures.WaterTemperature -
            _config.TemperatureTolerance.LowerTolerance)
        {
            _stateMediator.ProcessState.State = ProcessStateEnum.HeatWaterReservoir;
        }
        else if(_stateMediator.ProcessState.State == ProcessStateEnum.HeatWaterReservoir &&
                temperatures.TWather >
                _stateMediator.SetPointTemperatures.WaterTemperature + _config.TemperatureTolerance.UpperTolerance)
        {
            _stateMediator.ProcessState.State = ProcessStateEnum.DoNothing;
        }
    }
}