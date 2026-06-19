using Hampcoders.Electrolink.API.Analytics.Domain.Model.Enums;
using Hampcoders.Electrolink.API.Analytics.Domain.Model.Queries;
using Hampcoders.Electrolink.API.Analytics.Domain.Services;
using Hampcoders.Electrolink.API.Shared.Application.Internal.EventHandler;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Events;

namespace Hampcoders.Electrolink.API.Analytics.Application.Internal.EventHandlers;

public class EnterpriseSubscriptionFullyActiveEventHandler(
    IConsumptionDashboardQueryService dashboardQueryService,
    IConsumptionDashboardCommandService dashboardCommandService,
    IUnitOfWork unitOfWork,
    ILogger<EnterpriseSubscriptionFullyActiveEventHandler> logger)
    : IEventHandler<EnterpriseSubscriptionFullyActiveEvent>
{
    public async Task Handle(EnterpriseSubscriptionFullyActiveEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[Analytics] EnterpriseSubscription fully active for User={UserId}, Plan={Plan}",
            @event.UserId, @event.EnterprisePlan);

        var dashboards = await dashboardQueryService.Handle(
            new GetDashboardsByOwnerIdQuery(@event.UserId));

        foreach (var dashboard in dashboards)
        {
            dashboard.UpgradeTier(PlanTier.EnterpriseBasic);
            await unitOfWork.CompleteAsync();
            logger.LogInformation(
                "[Analytics] Dashboard {DashboardId} upgraded to EnterpriseBasic",
                dashboard.DashboardId.Value);
        }
    }
}
