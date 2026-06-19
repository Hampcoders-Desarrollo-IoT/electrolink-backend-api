using Hampcoders.Electrolink.API.Monitoring.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Monitoring.Domain.Model.Queries;
using Hampcoders.Electrolink.API.Monitoring.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Monitoring.Domain.Repositories;
using Hampcoders.Electrolink.API.Monitoring.Domain.Services;

namespace Hampcoders.Electrolink.API.Monitoring.Application.Internal.QueryServices;

public class ServiceExecutionQueryService(
    IServiceExecutionRepository executionRepository
) : IServiceExecutionQueryService
{
    public async Task<ServiceExecution?> Handle(GetServiceExecutionByIdQuery query)
    {
        return await executionRepository.FindByIdAsync(query.ExecutionId);
    }

    public async Task<IEnumerable<ServiceExecution>> Handle(GetAssignedServicesByTechnicianQuery query)
    {
        return await executionRepository.FindByTechnicianIdAsync(query.TechnicianId);
    }

    public async Task<ServiceExecution?> Handle(GetActiveServiceByClientQuery query)
    {
        return await executionRepository.FindActiveByOwnerAsync(query.Owner);
    }

    public async Task<IEnumerable<ServiceExecution>> Handle(GetServiceHistoryByClientQuery query)
    {
        return await executionRepository.FindByOwnerAsync(query.Owner);
    }

    public async Task<IEnumerable<ServiceExecution>> Handle(GetServiceHistoryByTechnicianQuery query)
    {
        return await executionRepository.FindByTechnicianIdAsync(query.TechnicianId);
    }

    public async Task<ServiceExecution?> Handle(GetWorkLogQuery query)
    {
        return await executionRepository.FindByIdAsync(query.ExecutionId);
    }

    public async Task<IEnumerable<ServiceExecution>> Handle(GetNoShowAlertQuery query)
    {
        var active = await executionRepository.FindActiveServiceExecutionAsync();
        var noShowThreshold = DateTime.UtcNow.AddMinutes(-30);

        return active.Where(e =>
            e.Status == EExecutionStatus.Notified &&
            e.ScheduledDateTime < noShowThreshold &&
            e.StartedAt == null);
    }
}
