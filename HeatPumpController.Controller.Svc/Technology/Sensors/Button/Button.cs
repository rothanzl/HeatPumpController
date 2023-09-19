using System.Text.Json;
using System.Text.Json.Serialization;
using HeatPumpController.Controller.Svc.Models.Infra;

namespace HeatPumpController.Controller.Svc.Technology.Sensors.Button;

public abstract class Button
{
    protected Func<ButtonMessage, ValueTask>? Listener { get; set; }

    public ValueTask Invoke(ButtonMessage msg)
    {
        return Listener?.Invoke(msg) ?? ValueTask.CompletedTask;
    }



}

public class BathRoomButton : Button
{
    private readonly IPersistentStateMediator _persistentStateMediator;

    public BathRoomButton(IPersistentStateMediator persistentStateMediator)
    {
        _persistentStateMediator = persistentStateMediator;

        Listener = msg =>
        {
            if (msg.Action == ButtonAction.Single)
            {
                _persistentStateMediator.Relays.RecuperationUnitIntensity =
                    !_persistentStateMediator.Relays.RecuperationUnitIntensity;
            }
            else if (msg.Action is ButtonAction.Double or ButtonAction.Long)
            {
                _persistentStateMediator.Relays.RecuperationUnitPower =
                    !_persistentStateMediator.Relays.RecuperationUnitPower;
            }

            return new ValueTask(_persistentStateMediator.PersistIfChange());
        };

    }
}

public interface IButtonListener
{
    Task HandleMessage(string topic, byte[] payload);
}

public class ButtonListener : IButtonListener
{
    private readonly ILogger<ButtonListener> _logger;
    private readonly BathRoomButton _bathRoomButton;

    public ButtonListener(ILogger<ButtonListener> logger, BathRoomButton bathRoomButton)
    {
        _logger = logger;
        _bathRoomButton = bathRoomButton;
    }

    public async Task HandleMessage(string topic, byte[] payload)
    {
        try
        {
            var parsedMessage = JsonSerializer.Deserialize<ButtonMessage>(payload)!;
            await _bathRoomButton.Invoke(parsedMessage);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Cannot consume message[{Message}]", payload == null ? null : Convert.ToString(payload));
        }
    }
}

public class ButtonMessage
{
    [JsonPropertyName("action")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ButtonAction Action { get; init; }
    
    [JsonPropertyName("battery")]
    public float Battery { get; init; }

    [JsonPropertyName("linkquality")]
    public float LinkQuality { get; init; }

    [JsonPropertyName("voltage")]
    public float VoltageMilliVolts { get; init; }
}

public enum ButtonAction
{
    Single,
    Double,
    Long
}