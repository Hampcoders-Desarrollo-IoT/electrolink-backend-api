using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;

namespace Hampcoders.Electrolink.API.Analytics.Application.Internal.EventHandlers;

public record ReadingIngestedIntegrationEvent(
    string OwnerId,
    string DeviceId,
    string CircuitId,
    decimal KilowattHours,
    decimal Voltage,
    decimal Current,
    DateTime ReadingTimestamp) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = ReadingTimestamp;
}

public record AnomalyDetectedIntegrationEvent(
    string OwnerId,
    string AlertId,
    string DeviceId,
    string AnomalyType,
    string Severity,
    DateTime Timestamp,
    string Description) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = Timestamp;
}

public record AnomalyResolvedIntegrationEvent(
    string OwnerId,
    string AlertId,
    DateTime ResolutionTimestamp) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = ResolutionTimestamp;
}

public record DeviceDisconnectedIntegrationEvent(
    string OwnerId,
    string DeviceId,
    DateTime DisconnectedAt) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DisconnectedAt;
}

public record DeviceReconnectedIntegrationEvent(
    string OwnerId,
    string DeviceId,
    DateTime ReconnectedAt) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = ReconnectedAt;
}

public record RelayCommandExecutedIntegrationEvent(
    string OwnerId,
    string DeviceId,
    string CircuitId,
    string CommandType,
    DateTime ExecutedAt,
    bool Success) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = ExecutedAt;
}

public record ServiceCompletedIntegrationEvent(
    string OwnerId,
    string ServiceId,
    string ServiceType,
    DateTime CompletedAt,
    string TechnicianId,
    decimal ServiceRevenue,
    string Currency,
    TimeSpan ResponseTime,
    bool RequiresIoTCertification) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = CompletedAt;
}

public record TechnicianEvaluationSubmittedIntegrationEvent(
    string TechnicianId,
    int Score,
    string Feedback,
    DateTime SubmittedAt) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = SubmittedAt;
}

public record EnterpriseSubscriptionFullyActiveIntegrationEvent(
    string OwnerId,
    string SubscriptionId,
    string PropertyId,
    List<string> DeviceIds,
    DateTime ActivatedAt) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = ActivatedAt;
}

public record ConsumptionThresholdsUpdatedIntegrationEvent(
    string OwnerId,
    Dictionary<string, decimal> Thresholds,
    DateTime UpdatedAt) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = UpdatedAt;
}

public record ServiceSuggestionAcceptedIntegrationEvent(
    string OwnerId,
    string ServiceSuggestionId,
    DateTime AcceptedAt) : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = AcceptedAt;
}
