namespace Hampcoders.Electrolink.API.Processing.Domain.Model.Events;

public class MqttAnomalyReceivedEventArgs : EventArgs
{
    public string DeviceId { get; }
    public string Payload { get; }
    public DateTime ReceivedAt { get; }

    public MqttAnomalyReceivedEventArgs(string deviceId, string payload)
    {
        DeviceId = deviceId;
        Payload = payload;
        ReceivedAt = DateTime.UtcNow;
    }
}
