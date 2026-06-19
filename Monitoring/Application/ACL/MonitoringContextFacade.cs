using Hampcoders.Electrolink.API.Monitoring.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Monitoring.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Monitoring.Domain.Repositories;
using Hampcoders.Electrolink.API.Monitoring.Domain.Services;
using Hampcoders.Electrolink.API.Monitoring.Interfaces.ACL;
using Hampcoders.Electrolink.API.Profiles.Interfaces.ACL;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;

namespace Hampcoders.Electrolink.API.Monitoring.Application.ACL;

public sealed class MonitoringContextFacade(
    IServiceExecutionCommandService commandService,
    IServiceExecutionRepository executionRepository,
    IProfilesContextFacade profilesContextFacade,
    IUnitOfWork unitOfWork)
    : IMonitoringContextFacade
{
    public async Task<int> CountActiveServicesForRecipeAsync(string recipeId)
    {
        var active = await executionRepository.FindActiveServiceExecutionAsync();
        return active.Count(e =>
            e.Status is EExecutionStatus.Notified or EExecutionStatus.InProgress);
    }

    public async Task<int> CountInProgressServicesForRecipeAsync(string recipeId)
    {
        var active = await executionRepository.FindActiveServiceExecutionAsync();
        return active.Count(e => e.Status == EExecutionStatus.InProgress);
    }

    public async Task<bool> IsServiceActiveAsync(string serviceId)
    {
        var execution = await executionRepository.FindByIdAsync(ServiceExecutionId.From(serviceId));
        return execution is not null &&
               execution.Status is EExecutionStatus.Notified or EExecutionStatus.InProgress;
    }

    public async Task<string?> GetServiceExecutionStatusAsync(string serviceId)
    {
        var execution = await executionRepository.FindByIdAsync(ServiceExecutionId.From(serviceId));
        return execution?.Status.ToString();
    }

    public async Task<string> CreateServiceExecutionAsync(
        string assignmentId,
        string requestId,
        string technicianId,
        string homeownerId,
        string propertyId,
        RecipeSnapshot recipeSnapshot,
        DateTime scheduledAt,
        bool isPriority)
    {
        var command = new CreateServiceExecutionCommand(
            AssignmentId.From(assignmentId), RequestId.From(requestId), TechnicianId.From(technicianId), ClientIdentity.FromHomeowner(homeownerId), PropertyId.From(propertyId),
            recipeSnapshot, scheduledAt, isPriority);

        var execution = await commandService.Handle(command);
        return execution.Id.Value;
    }

    public async Task RecordCircuitToggleAsync(
        string executionId,
        string deviceId,
        string targetState,
        string actionStatus,
        string? failureReason)
    {
        var command = new RecordCircuitToggleCommand(
            ServiceExecutionId.From(executionId),
            DeviceId.From(deviceId),
            Enum.Parse<ERelayState>(targetState, true),
            Enum.Parse<ERelayActionStatus>(actionStatus, true),
            failureReason);

        await commandService.Handle(command);
    }
}
