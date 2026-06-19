using Hampcoders.Electrolink.API.Analytics.Domain.Repositories;
using Hampcoders.Electrolink.API.Analytics.Domain.Services;

namespace Hampcoders.Electrolink.API.Analytics.Infrastructure.Jobs;

public class CostProjectionSchedulerJob(
    IConsumptionDashboardCommandService dashboardCommandService,
    IConsumptionDashboardRepository dashboardRepository,
    IConfiguration configuration)
{
    private readonly IConsumptionDashboardCommandService _dashboardCommandService = dashboardCommandService;
    private readonly IConsumptionDashboardRepository _dashboardRepository = dashboardRepository;
    private readonly IConfiguration _configuration = configuration;

    public async Task ExecuteAsync()
    {
        var electricityRate = _configuration.GetValue<decimal>("Analytics:ElectricityRatePerKWh");
        var activeDashboards = await _dashboardRepository.FindAllActiveAsync();

        foreach (var dashboard in activeDashboards
            .Where(d => d.PlanTier != Domain.Model.Enums.PlanTier.Free))
        {
            await _dashboardCommandService.GenerateCostProjectionAsync(
                dashboard.Owner.ClientId,
                electricityRate);
        }
    }
}
