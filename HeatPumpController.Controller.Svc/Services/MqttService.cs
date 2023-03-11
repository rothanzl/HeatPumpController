using HeatPumpController.Controller.Svc.Config;
using HeatPumpController.Controller.Svc.Technology.Sensors.Temperature.Mqtt;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

namespace HeatPumpController.Controller.Svc.Services;

public class MqttTopics
{
    public const string TempLivingRoom = "zigbee2mqtt/TempSensor_LivingRoom";
    public const string TempSmallRoom = "zigbee2mqtt/TempSensor_SmallRoom";
    public const string TempBathRoom = "zigbee2mqtt/TempSensor_BathRoom";
    public const string TempKitchen = "zigbee2mqtt/TempSensor_Kitchen";
    public const string TempBedRoom = "zigbee2mqtt/TempSensor_BedRoom";
    public const string TempTechRoom = "zigbee2mqtt/TempSensor_TechRoom";
}

public class MqttService : IHostedService, IDisposable
{
    private readonly MqttConfig _config;
    private readonly IMqttTemperatureSensorsHelper _sensorsHelper;
    private readonly ILogger<MqttService> _logger;
    
    private IMqttClient? _mqttClient;

    public MqttService(IOptions<ControllerConfig> options, IMqttTemperatureSensorsHelper sensorsHelper, ILogger<MqttService> logger)
    {
        _sensorsHelper = sensorsHelper;
        _logger = logger;
        _config = options.Value.Mqtt;
    }

    public Task StartAsync(CancellationToken cancellationToken)
        => Connect();

    private async Task Connect()
    {
        var mqttFactory = new MqttFactory();

        _mqttClient = mqttFactory.CreateMqttClient();
        _mqttClient.ApplicationMessageReceivedAsync += MqttClientOnApplicationMessageReceivedAsync;
        _mqttClient.ConnectingAsync += args =>
        {
            _logger.LogInformation("Connecting");
            return Task.CompletedTask;
        };
        _mqttClient.ConnectedAsync += args =>
        {
            _logger.LogInformation("Connected");
            return Task.CompletedTask;
        };
        _mqttClient.DisconnectedAsync += async args =>
        {
            _logger.LogError(args.Exception, "Disconnected[{Reaosn}] - wait and reconnect", args.Reason.ToString());
            await Task.Delay(TimeSpan.FromSeconds(10));
            await Connect();
        };
            
        var builder = new MqttClientOptionsBuilder()
            .WithTcpServer(_config.BrokerAddress);

        if (!_config.Anonymous)
            builder.WithCredentials(_config.UserName, _config.Password);
        

        var mqttClientOptions = builder.Build();


        void SetMqttTopicFilterBuilder(MqttTopicFilterBuilder b, string topicName)
        {
            b.WithTopic(topicName);
            b.WithExactlyOnceQoS();
            b.WithRetainHandling(MqttRetainHandling.SendAtSubscribe);
        }
        // Create the subscribe options including several topics with different options.
        // It is also possible to all of these topics using a dedicated call of _SubscribeAsync_ per topic.
        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(
                f => SetMqttTopicFilterBuilder(f, MqttTopics.TempKitchen))
            .WithTopicFilter(
                f => SetMqttTopicFilterBuilder(f,MqttTopics.TempBathRoom))
            .WithTopicFilter(
                f => SetMqttTopicFilterBuilder(f, MqttTopics.TempBedRoom))
            .WithTopicFilter(
                f => SetMqttTopicFilterBuilder(f, MqttTopics.TempLivingRoom))
            .WithTopicFilter(
                f => SetMqttTopicFilterBuilder(f, MqttTopics.TempSmallRoom))
            .WithTopicFilter(
                f => SetMqttTopicFilterBuilder(f, MqttTopics.TempTechRoom))
            .Build();

        try
        {
            await _mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
            var response = await _mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to connect to Mqtt");
        }
    }

    private Task MqttClientOnApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        => ConsumeMessage(arg.ApplicationMessage);

    private Task ConsumeMessage(MqttApplicationMessage argApplicationMessage)
    {
        _logger.LogInformation("Consume message from {Topic}", argApplicationMessage.Topic);
        return _sensorsHelper.HandleMessage(argApplicationMessage.Topic, argApplicationMessage.Payload);
    }
    
    
    

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _mqttClient?.Dispose();
    }
}