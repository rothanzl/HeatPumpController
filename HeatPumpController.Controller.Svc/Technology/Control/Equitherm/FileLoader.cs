using System.Globalization;

namespace HeatPumpController.Controller.Svc.Technology.Control.Equitherm;

public static class FileLoader
{
    private const string FileName = "equitherm.json";
    
    public static EquithermStructure[] LoadedStructures { get; }

    static FileLoader()
    {
        if (!File.Exists(FileName))
            throw new IOException($"Missing {FileName} file");
        
        var fileData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Dictionary<string,string>>>(
            File.ReadAllText(FileName))!;

        float ToFloat(string s) => float.Parse(s, NumberFormatInfo.InvariantInfo);
        int ToInt(string s) => Int32.Parse(s, NumberFormatInfo.InvariantInfo);
        
        List<EquithermStructure> result = new();
        foreach (var structures in fileData)
        {
            List<EquithermTemperatureCorrection> data = new();
            foreach (var values in structures.Value)
            {
                data.Add(new(
                    temperature: ToFloat(values.Key), 
                    correction: ToFloat(values.Value)));
            }
            result.Add(new(id: ToInt(structures.Key), data: data.ToArray()));
        }

        LoadedStructures = result.ToArray();
    }


}

public class EquithermStructure
{
    public EquithermStructure(int id, EquithermTemperatureCorrection[] data)
    {
        Id = id;
        Data = data;
    }

    public int Id { get; }
    public EquithermTemperatureCorrection[] Data { get; }
}

public class EquithermTemperatureCorrection
{
    public EquithermTemperatureCorrection(float temperature, float correction)
    {
        Temperature = temperature;
        Correction = correction;
    }

    public float Temperature { get; }
    public float Correction { get; }
}