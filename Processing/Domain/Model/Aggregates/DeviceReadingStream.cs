using Hampcoders.Electrolink.API.Processing.Domain.Model.Entities;
using Hampcoders.Electrolink.API.Processing.Domain.Model.Events;
using Hampcoders.Electrolink.API.Processing.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Processing.Domain.Model.Aggregates;

public class DeviceReadingStream : BaseAggregateRoot
{
    public StreamId     StreamId   { get; private set; }
    public DeviceId    DeviceId   { get; private set; }
    public PropertyId  PropertyId { get; private set; }
    public ClientIdentity Owner   { get; private set; }
    public HomeownerId HomeownerId => Owner.ToHomeownerId();

    public EStreamStatus StreamStatus { get; private set; }
    public DateTime      LastReceivedAt { get; private set; }

    private readonly List<Reading> _readings = new();
    public IReadOnlyCollection<Reading> Readings => _readings.AsReadOnly();

    public ThresholdConfig CustomThresholds { get; private set; }

    public int ConsecutiveAnomalyCount { get; private set; }
    public int ConsecutiveNormalCount  { get; private set; }

    private DeviceReadingStream() { }

    public static DeviceReadingStream Create(
        DeviceId deviceId,
        PropertyId propertyId,
        ClientIdentity owner,
        ThresholdConfig? customThresholds = null)
    {
        var stream = new DeviceReadingStream
        {
            StreamId              = StreamId.NewId(),
            DeviceId              = deviceId,
            PropertyId            = propertyId,
            Owner              = owner,
            StreamStatus          = EStreamStatus.Active,
            LastReceivedAt        = DateTime.UtcNow,
            CustomThresholds      = customThresholds ?? ThresholdConfig.Default(),
            ConsecutiveAnomalyCount = 0,
            ConsecutiveNormalCount  = 0,
        };
        return stream;
    }

    public bool IngestReading(Reading reading, int maxWindowSize = 120)
    {
        if (StreamStatus == EStreamStatus.Inactive)
            return false;

        if (_readings.Any(r => r.ReadingId == reading.ReadingId))
            return false;

        _readings.Add(reading);

        if (_readings.Count > maxWindowSize)
            _readings.RemoveAt(0);

        LastReceivedAt = reading.Timestamp > DateTime.UtcNow
            ? DateTime.UtcNow
            : reading.Timestamp;

        if (StreamStatus != EStreamStatus.Active)
        {
            var previous = StreamStatus;
            StreamStatus = EStreamStatus.Active;
            ConsecutiveAnomalyCount = 0;
            ConsecutiveNormalCount  = 0;

            RaiseDomainEvent(new StreamStatusChangedEvent(
                StreamId.Value, DeviceId.Value,
                previous.ToString(), EStreamStatus.Active.ToString(),
                DateTime.UtcNow));

            RaiseDomainEvent(new DeviceReconnectedEvent(
                DeviceId.Value, PropertyId.Value, Owner.ClientId, DateTime.UtcNow));
        }

        RaiseDomainEvent(new ReadingIngestedEvent(
            StreamId.Value, DeviceId.Value, PropertyId.Value,
            Owner.ClientId, reading.ReadingId.Value,
            reading.Timestamp, reading.Source.ToString(),
            DateTime.UtcNow));

        return true;
    }

    public void RejectReading(ReadingId readingId, string rejectionReason)
    {
        RaiseDomainEvent(new ReadingRejectedEvent(
            DeviceId.Value, readingId.Value, rejectionReason, DateTime.UtcNow));
    }

    public void MarkAsDegraded()
    {
        if (StreamStatus == EStreamStatus.Degraded) return;

        var previous = StreamStatus;
        StreamStatus = EStreamStatus.Degraded;

        RaiseDomainEvent(new StreamStatusChangedEvent(
            StreamId.Value, DeviceId.Value,
            previous.ToString(), EStreamStatus.Degraded.ToString(),
            DateTime.UtcNow));
    }

    public void MarkAsDisconnected()
    {
        if (StreamStatus == EStreamStatus.Inactive) return;

        var previous = StreamStatus;
        StreamStatus = EStreamStatus.Inactive;

        RaiseDomainEvent(new StreamStatusChangedEvent(
            StreamId.Value, DeviceId.Value,
            previous.ToString(), EStreamStatus.Inactive.ToString(),
            DateTime.UtcNow));

        RaiseDomainEvent(new DeviceDisconnectedEvent(
            DeviceId.Value, PropertyId.Value, Owner.ClientId,
            LastReceivedAt, DateTime.UtcNow));
    }

    public void Pause()
    {
        if (StreamStatus == EStreamStatus.Inactive) return;

        var previous = StreamStatus;
        StreamStatus = EStreamStatus.Inactive;
        RaiseDomainEvent(new StreamStatusChangedEvent(
            StreamId.Value, DeviceId.Value,
            previous.ToString(), EStreamStatus.Inactive.ToString(),
            DateTime.UtcNow));
    }

    public void Resume()
    {
        if (StreamStatus != EStreamStatus.Inactive)
            throw new InvalidOperationException("Stream must be INACTIVE to be resumed.");

        StreamStatus   = EStreamStatus.Active;
        LastReceivedAt = DateTime.UtcNow;

        RaiseDomainEvent(new StreamStatusChangedEvent(
            StreamId.Value, DeviceId.Value,
            EStreamStatus.Inactive.ToString(), EStreamStatus.Active.ToString(),
            DateTime.UtcNow));
    }

    public void UpdateCustomThresholds(ThresholdConfig newThresholds)
    {
        CustomThresholds = newThresholds
            ?? throw new ArgumentNullException(nameof(newThresholds));
    }

    public void RegisterAnomalyDetection()   => ConsecutiveAnomalyCount++;
    public void RegisterNormalReading()
    {
        ConsecutiveAnomalyCount = 0;
        ConsecutiveNormalCount++;
    }
    public void ResetNormalCount() => ConsecutiveNormalCount = 0;

    public bool ShouldTryAutoResolveAnomalies(int threshold)
        => ConsecutiveNormalCount >= threshold;
}
