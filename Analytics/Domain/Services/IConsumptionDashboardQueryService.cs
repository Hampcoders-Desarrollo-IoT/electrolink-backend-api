using Hampcoders.Electrolink.API.Analytics.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Analytics.Domain.Model.Queries;
using Hampcoders.Electrolink.API.Analytics.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Analytics.Domain.Services;

public interface IConsumptionDashboardQueryService
{
    Task<DashboardView?> GetDashboardViewAsync(string homeownerId);
    Task<DashboardView?> GetCostProjectionAsync(string homeownerId);
    Task<DashboardView?> GetRealTimeCircuitMonitorAsync(string homeownerId);
    Task<IEnumerable<ConsumptionDashboard>> Handle(GetDashboardsByOwnerIdQuery query);
}
