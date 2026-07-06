using Hampcoders.Electrolink.API.Processing.Domain.Model.Events;

namespace Hampcoders.Electrolink.API.Processing.Domain.Services;

public interface IMqttSubscriptionService
{
    bool IsConnected { get; }

    event Func<MqttTelemetryReceivedEventArgs, Task> OnTelemetryReceived;
    event Func<MqttAnomalyReceivedEventArgs, Task> OnAnomalyReceived;
    event Func<MqttCommandResponseEventArgs, Task> OnCommandResponse;
    event Func<MqttStatusReceivedEventArgs, Task> OnStatusReceived;

    Task PublishCommandAsync(string deviceId, string payload);
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync(CancellationToken cancellationToken);
}
