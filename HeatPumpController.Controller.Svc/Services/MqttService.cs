using HeatPumpController.Controller.Svc.Config;
using HeatPumpController.Controller.Svc.Technology.Sensors.Temperature.Mqtt;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;

namespace HeatPumpController.Controller.Svc.Services;

public class MqttTopics
{
    public const string TempLivingRoom = "zigbee2mqtt/TempSensor_LivingRoom";
    public const string TempSmallRoom = "zigbee2mqtt/TempSensor_SmallRoom";
    public const string TempBathRoom = "zigbee2mqtt/TempSensor_BathRoom";
    public const string TempKitchen = "zigbee2mqtt/TempSensor_Kitchen";
    public const string TempBedRoom = "zigbee2mqtt/TempSensor_BedRoom";
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
        _mqttClient.DisconnectedAsync += args =>
        {
            _logger.LogError(args.Exception, "Disconnected[{Reaosn}]", args.Reason.ToString());
            return Task.CompletedTask;
        };
            
        var builder = new MqttClientOptionsBuilder()
            .WithTcpServer(_config.BrokerAddress);

        if (!_config.Anonymous)
            builder.WithCredentials(_config.UserName, _config.Password);
        

        var mqttClientOptions = builder.Build();
        await _mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
        
        
        // Create the subscribe options including several topics with different options.
        // It is also possible to all of these topics using a dedicated call of _SubscribeAsync_ per topic.
        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(
                f => { f.WithTopic(MqttTopics.TempKitchen); })
            .WithTopicFilter(
                f => { f.WithTopic(MqttTopics.TempBathRoom); })
            .WithTopicFilter(
                f => { f.WithTopic(MqttTopics.TempBedRoom); })
            .WithTopicFilter(
                f => { f.WithTopic(MqttTopics.TempLivingRoom); })
            .WithTopicFilter(
                f => { f.WithTopic(MqttTopics.TempSmallRoom); })
            .Build();

        var response = await _mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
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