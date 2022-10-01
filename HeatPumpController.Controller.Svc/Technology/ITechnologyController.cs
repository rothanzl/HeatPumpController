namespace HeatPumpController.Controller.Svc.Technology;

public interface ITechnologyController
{
    ITechnologyResources Open();
}

public interface ITechnologyResources : IAsyncDisposable
{
    Task<Temperatures> GetTemperatures(CancellationToken ct);
}

public record Temperatures(float TOut, float TWather, float THeatherBack);

public class DemoTechnologyController : ITechnologyController
{
    public ITechnologyResources Open()
    {
        return new DemoTechnologyResources();
    }
}

public class DemoTechnologyResources : ITechnologyResources
{
    public Task<Temperatures> GetTemperatures(CancellationToken ct)
    {
        return Task.FromResult(new Temperatures(10, 33, 23));
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}