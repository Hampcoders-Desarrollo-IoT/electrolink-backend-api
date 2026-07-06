using Hampcoders.Electrolink.API.Processing.Domain.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Hampcoders.Electrolink.API.Processing.Infrastructure.Services;

public class MqttHealthCheck : IHealthCheck
{
    private readonly IMqttSubscriptionService _mqttService;

    public MqttHealthCheck(IMqttSubscriptionService mqttService)
    {
        _mqttService = mqttService;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (_mqttService.IsConnected)
        {
            return Task.FromResult(HealthCheckResult.Healthy(
                "MQTT broker is connected and processing device telemetry."));
        }

        return Task.FromResult(HealthCheckResult.Unhealthy(
            "MQTT broker is disconnected. Devices cannot send telemetry."));
    }
}
