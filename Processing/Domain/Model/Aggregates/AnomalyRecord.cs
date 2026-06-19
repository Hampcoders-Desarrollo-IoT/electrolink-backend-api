using Hampcoders.Electrolink.API.Processing.Domain.Model.Events;
using Hampcoders.Electrolink.API.Processing.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Processing.Domain.Model.Aggregates;

public class AnomalyRecord : BaseAggregateRoot
{
    public AnomalyRecordId AnomalyId   { get; private set; }
    public DeviceId        DeviceId    { get; private set; }
    public PropertyId      PropertyId  { get; private set; }
    public ClientIdentity Owner    { get; private set; }
    public HomeownerId HomeownerId => Owner.ToHomeownerId();

    public EAnomalyType     AnomalyType     { get; private set; }
    public EAnomalySeverity Severity        { get; private set; }
    public EDetectionLayer  DetectionLayer  { get; private set; }
    public EAnomalyStatus   Status          { get; private set; }

    public ReadingId  TriggerReadingId { get; private set; }
    public DateTime   DetectedAt       { get; private set; }
    public DateTime?  ResolvedAt       { get; private set; }
    public DateTime?  AcknowledgedAt   { get; private set; }

    public string?  RelatedServiceSuggestionId { get; private set; }
    public string?  EdgeAlertPayloadJson        { get; private set; }
    public bool     AutoRelayActivated          { get; private set; }

    private AnomalyRecord() { }

    public static AnomalyRecord Detect(
        DeviceId deviceId,
        PropertyId propertyId,
        ClientIdentity owner,
        EAnomalyType anomalyType,
        EAnomalySeverity severity,
        ReadingId triggerReadingId)
    {
        var record = new AnomalyRecord
        {
            AnomalyId      = AnomalyRecordId.NewId(),
            DeviceId       = deviceId,
            PropertyId     = propertyId,
            Owner       = owner,
            AnomalyType    = anomalyType,
            Severity       = severity,
            DetectionLayer = EDetectionLayer.Cloud,
            Status         = EAnomalyStatus.Active,
            TriggerReadingId = triggerReadingId,
            DetectedAt     = DateTime.UtcNow,
        };

        record.RaiseDomainEvent(new AnomalyDetectedEvent(
            record.AnomalyId.Value, deviceId.Value, propertyId.Value, owner.ClientId,
            anomalyType.ToString(), severity.ToString(),
            EDetectionLayer.Cloud.ToString(), false, record.DetectedAt));

        return record;
    }

    public static AnomalyRecord DetectFromEdge(
        DeviceId deviceId,
        PropertyId propertyId,
        ClientIdentity owner,
        EAnomalyType anomalyType,
        EAnomalySeverity severity,
        ReadingId triggerReadingId,
        string edgeAlertPayloadJson)
    {
        var record = new AnomalyRecord
        {
            AnomalyId            = AnomalyRecordId.NewId(),
            DeviceId             = deviceId,
            PropertyId           = propertyId,
            Owner             = owner,
            AnomalyType          = anomalyType,
            Severity             = severity,
            DetectionLayer       = EDetectionLayer.Edge,
            Status               = EAnomalyStatus.Active,
            TriggerReadingId     = triggerReadingId,
            DetectedAt           = DateTime.UtcNow,
            EdgeAlertPayloadJson = edgeAlertPayloadJson,
        };

        record.RaiseDomainEvent(new AnomalyDetectedEvent(
            record.AnomalyId.Value, deviceId.Value, propertyId.Value, owner.ClientId,
            anomalyType.ToString(), severity.ToString(),
            EDetectionLayer.Edge.ToString(), false, record.DetectedAt));

        return record;
    }

    public void Acknowledge()
    {
        if (Status != EAnomalyStatus.Active)
            throw new InvalidOperationException(
                $"Cannot acknowledge anomaly in status {Status}. Expected ACTIVE.");

        Status         = EAnomalyStatus.Acknowledged;
        AcknowledgedAt = DateTime.UtcNow;

        RaiseDomainEvent(new AnomalyAcknowledgedEvent(
            AnomalyId.Value, DeviceId.Value, PropertyId.Value, AcknowledgedAt.Value));
    }

    public void ResolveAutomatically()
    {
        if (Severity == EAnomalySeverity.Critical)
            throw new InvalidOperationException(
                "CRITICAL anomalies cannot be auto-resolved. Use ForceResolve after manual verification.");

        if (Status is not (EAnomalyStatus.Active or EAnomalyStatus.Acknowledged))
            throw new InvalidOperationException(
                $"Cannot resolve anomaly in status {Status}.");

        Status     = EAnomalyStatus.Resolved;
        ResolvedAt = DateTime.UtcNow;

        RaiseDomainEvent(new AnomalyResolvedEvent(
            AnomalyId.Value, DeviceId.Value, PropertyId.Value,
            "AUTO", ResolvedAt.Value));
    }

    public void ForceResolve(string resolvedByActorId)
    {
        if (Status != EAnomalyStatus.Acknowledged)
            throw new InvalidOperationException(
                "ForceResolve requires the anomaly to be ACKNOWLEDGED first.");

        Status     = EAnomalyStatus.Resolved;
        ResolvedAt = DateTime.UtcNow;

        RaiseDomainEvent(new AnomalyResolvedEvent(
            AnomalyId.Value, DeviceId.Value, PropertyId.Value,
            resolvedByActorId, ResolvedAt.Value));
    }

    public void Supersede()
    {
        if (Status != EAnomalyStatus.Active)
            throw new InvalidOperationException(
                $"Only ACTIVE anomalies can be superseded. Current status: {Status}.");

        Status = EAnomalyStatus.Superseded;
    }

    public void MarkAutoRelayActivated(string? serviceSuggestionId = null)
    {
        AutoRelayActivated         = true;
        RelatedServiceSuggestionId = serviceSuggestionId;

        RaiseDomainEvent(new AnomalyDetectedEvent(
            AnomalyId.Value, DeviceId.Value, PropertyId.Value, Owner.ClientId,
            AnomalyType.ToString(), Severity.ToString(),
            DetectionLayer.ToString(), true, DateTime.UtcNow));
    }
}
