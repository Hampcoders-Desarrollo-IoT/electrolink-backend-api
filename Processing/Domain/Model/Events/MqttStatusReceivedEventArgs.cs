namespace Hampcoders.Electrolink.API.Processing.Domain.Model.Events;

public class MqttStatusReceivedEventArgs : EventArgs
{
    public string DeviceId { get; }
    public string Payload { get; }
    public DateTime ReceivedAt { get; }

    public MqttStatusReceivedEventArgs(string deviceId, string payload)
    {
        DeviceId = deviceId;
        Payload = payload;
        ReceivedAt = DateTime.UtcNow;
    }
}
