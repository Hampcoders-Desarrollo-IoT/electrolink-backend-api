namespace Hampcoders.Electrolink.API.Processing.Domain.Model.ValueObjects;

public record MqttSettings
{
    public string Host { get; }
    public int Port { get; }
    public bool UseTls { get; }
    public string ClientId { get; }
    public string Username { get; }
    public string Password { get; }
    public string TopicPrefix { get; }
    public string TelemetryTopic { get; }
    public string AnomalyTopic { get; }
    public string StatusTopic { get; }
    public string CommandResponseTopic { get; }
    public int QoS { get; }
    public int ReconnectDelaySeconds { get; }
    public int MaxReconnectAttempts { get; }

    private MqttSettings(
        string host, int port, bool useTls,
        string clientId, string username, string password,
        string topicPrefix, string telemetryTopic, string anomalyTopic,
        string statusTopic, string commandResponseTopic,
        int qos, int reconnectDelaySeconds, int maxReconnectAttempts)
    {
        Host = host;
        Port = port;
        UseTls = useTls;
        ClientId = clientId;
        Username = username;
        Password = password;
        TopicPrefix = topicPrefix;
        TelemetryTopic = telemetryTopic;
        AnomalyTopic = anomalyTopic;
        StatusTopic = statusTopic;
        CommandResponseTopic = commandResponseTopic;
        QoS = qos;
        ReconnectDelaySeconds = reconnectDelaySeconds;
        MaxReconnectAttempts = maxReconnectAttempts;
    }

    public static MqttSettings Default() =>
        new("localhost", 1883, false,
            "electrolink-backend", "", "",
            "electrolink/device",
            "electrolink/device/+/telemetry",
            "electrolink/device/+/anomaly",
            "electrolink/device/+/status",
            "electrolink/device/+/command/response",
            1, 5, 0);

    public static MqttSettings Create(
        string host, int port, bool useTls,
        string clientId, string username, string password,
        string topicPrefix, string telemetryTopic, string anomalyTopic,
        string statusTopic, string commandResponseTopic,
        int qos, int reconnectDelaySeconds, int maxReconnectAttempts)
    {
        if (string.IsNullOrWhiteSpace(host))
            throw new ArgumentException("MQTT host is required.");
        if (port <= 0 || port > 65535)
            throw new ArgumentException("MQTT port must be between 1 and 65535.");
        if (qos < 0 || qos > 2)
            throw new ArgumentException("MQTT QoS must be 0, 1, or 2.");
        if (reconnectDelaySeconds < 1)
            throw new ArgumentException("Reconnect delay must be at least 1 second.");

        return new MqttSettings(
            host, port, useTls,
            clientId, username, password,
            topicPrefix, telemetryTopic, anomalyTopic,
            statusTopic, commandResponseTopic,
            qos, reconnectDelaySeconds, maxReconnectAttempts);
    }
}
