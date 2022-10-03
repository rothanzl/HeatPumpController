namespace HeatPumpController.Web.Services;

public interface IViewModel
{
    SetPoint WaterTemperature { get; }
    SetPoint HeaterTemperature { get; }
}

public class ViewModel : IViewModel
{
    public ViewModel()
    {
        WaterTemperature = new SetPoint("Teplota vody", 23, 30);
        HeaterTemperature = new SetPoint("Teplota topení", 40, 33);
    }

    public SetPoint WaterTemperature { get; }
    public SetPoint HeaterTemperature { get; }
}

public class SetPoint
{
    public SetPoint(string name, float currentValue, float setPointValue)
    {
        Name = name;
        CurrentValue = currentValue;
        SetPointValue = setPointValue;
    }

    public string Name { get; }
    public float CurrentValue { get; }

    private float _setPointValue = 0;
    public float SetPointValue
    {
        get => _setPointValue ;
        set
        {
            Console.WriteLine("new value is "+ value);
            _setPointValue = value;
        }
    }
}