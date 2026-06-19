using Hampcoders.Electrolink.API.Analytics.Application.Internal.EventHandlers;
using MediatR;

namespace Hampcoders.Electrolink.API.Monitoring.Application.Internal.OutboundServices;

public interface IExternalAnalyticsService
{
    Task NotifyCompletionAsync(string executionId, string technicianId, DateTime completedAt);
    Task NotifyCancellationAsync(string executionId, string reason, DateTime cancelledAt);
}

public class ExternalAnalyticsService(
    IMediator mediator,
    ILogger<ExternalAnalyticsService> logger) : IExternalAnalyticsService
{
    public async Task NotifyCompletionAsync(string executionId, string technicianId, DateTime completedAt)
    {
        try
        {
            await mediator.Publish(new ServiceCompletedIntegrationEvent(
                OwnerId: string.Empty,
                ServiceId: executionId,
                ServiceType: string.Empty,
                CompletedAt: completedAt,
                TechnicianId: technicianId,
                ServiceRevenue: 0,
                Currency: "USD",
                ResponseTime: TimeSpan.Zero,
                RequiresIoTCertification: false));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to notify completion for service {ExecutionId}.", executionId);
        }
    }

    public async Task NotifyCancellationAsync(string executionId, string reason, DateTime cancelledAt)
    {
        logger.LogInformation("[Monitoring BC] Service cancelled: execution={ExecutionId}, reason={Reason}",
            executionId, reason);
    }
}
