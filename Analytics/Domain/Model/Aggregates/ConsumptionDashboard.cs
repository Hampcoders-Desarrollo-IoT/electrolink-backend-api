using Hampcoders.Electrolink.API.Analytics.Domain.Model.Enums;
using Hampcoders.Electrolink.API.Analytics.Domain.Model.Events;
using Hampcoders.Electrolink.API.Analytics.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Analytics.Domain.Model.Aggregates;

public class ConsumptionDashboard : BaseAggregateRoot
{
    public ConsumptionDashboardId DashboardId { get; private set; }
    public ClientIdentity Owner { get; private set; }
    public HomeownerId HomeownerId => Owner.ToHomeownerId();
    public PropertyId PropertyId { get; private set; }
    public List<DeviceId> DeviceIds { get; private set; }
    public PlanTier PlanTier { get; private set; }
    public List<HourRange> PeakHours { get; private set; }
    public Money CostProjection { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }
    public List<AlertLogId> AlertHistoryIds { get; private set; }
    public SavingsMetrics Savings { get; private set; }

    private Dictionary<string, decimal> _consumptionThresholds;

    private ConsumptionDashboard() { }

    public static ConsumptionDashboard Initialize(
        ClientIdentity owner,
        PropertyId propertyId,
        List<DeviceId> deviceIds,
        PlanTier planTier)
    {
        if (deviceIds == null || deviceIds.Count == 0)
            throw new ArgumentException("At least one DeviceId is required to initialize a dashboard.");

        var dashboard = new ConsumptionDashboard
        {
            DashboardId = ConsumptionDashboardId.New(),
            Owner = owner ?? throw new ArgumentNullException(nameof(owner)),
            PropertyId = propertyId ?? throw new ArgumentNullException(nameof(propertyId)),
            DeviceIds = [.. deviceIds],
            PlanTier = planTier,
            PeakHours = [],
            CostProjection = Money.Zero("PEN"),
            Savings = SavingsMetrics.Create(),
            LastUpdatedAt = DateTime.UtcNow,
            AlertHistoryIds = [],
            _consumptionThresholds = new Dictionary<string, decimal>()
        };

        dashboard.RaiseDomainEvent(new DashboardInitialized(
            dashboard.DashboardId.Value,
            dashboard.Owner.ClientId,
            dashboard.PropertyId.Value,
            dashboard.PlanTier.ToString(),
            DateTime.UtcNow));

        return dashboard;
    }

    public bool ApplyReading(
        DeviceId deviceId,
        string circuitId,
        decimal kWh,
        DateTime readingTimestamp)
    {
        if (!DeviceIds.Any(d => d.Value == deviceId.Value))
            return false;

        RecalculatePeakHours(readingTimestamp, kWh);
        LastUpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new DashboardUpdated(
            DashboardId.Value, Owner.ClientId, kWh, DateTime.UtcNow));

        if (CheckThresholdExceeded(circuitId, kWh, out var threshold))
        {
            RaiseDomainEvent(new ConsumptionThresholdExceeded(
                DashboardId.Value, Owner.ClientId, circuitId, kWh, threshold, DateTime.UtcNow));
        }

        return true;
    }

    public (DateTime NormalizedTimestamp, Granularity Granularity) ResolveReadingTimestamp(DateTime readingTimestamp)
    {
        var granularity = ResolveGranularity();
        return (NormalizeTimestamp(readingTimestamp, granularity), granularity);
    }

    public void UpdateCostProjection(decimal electricityRatePerKWh, decimal accumulatedKWhForMonth)
    {
        if (PlanTier == PlanTier.Free)
            return;

        var now = DateTime.UtcNow;
        var dayOfMonth = now.Day;
        var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);

        var projectedKWh = dayOfMonth > 0
            ? (accumulatedKWhForMonth / dayOfMonth) * daysInMonth
            : 0;

        CostProjection = Money.Of(projectedKWh * electricityRatePerKWh, "PEN");
        LastUpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new CostProjectionUpdated(
            DashboardId.Value, Owner.ClientId,
            CostProjection.Amount, CostProjection.Currency, DateTime.UtcNow));
    }

    public void UpgradeTier(PlanTier newTier)
    {
        if (PlanTier == newTier)
            return;
        PlanTier = newTier;
        LastUpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new DashboardTierUpgraded(
            DashboardId.Value, Owner.ClientId, newTier.ToString(), DateTime.UtcNow));
    }

    public void UpdateConsumptionThresholds(Dictionary<string, decimal> thresholds)
    {
        _consumptionThresholds = thresholds ?? new Dictionary<string, decimal>();
    }

    public void LinkAlertLog(AlertLogId alertLogId)
    {
        if (!AlertHistoryIds.Any(a => a.Value == alertLogId.Value))
            AlertHistoryIds.Add(alertLogId);
    }

    private Granularity ResolveGranularity() => PlanTier switch
    {
        PlanTier.Free => Granularity.Daily,
        PlanTier.PremiumIndividual => Granularity.Hourly,
        _ => Granularity.Raw
    };

    private static DateTime NormalizeTimestamp(DateTime timestamp, Granularity granularity) =>
        granularity switch
        {
            Granularity.Daily => timestamp.Date,
            Granularity.Hourly => new DateTime(timestamp.Year, timestamp.Month, timestamp.Day,
                timestamp.Hour, 0, 0, DateTimeKind.Utc),
            _ => timestamp
        };

    private void RecalculatePeakHours(DateTime readingTimestamp, decimal kWh)
    {
        if (kWh > 0)
        {
            var hour = readingTimestamp.Hour;
            var range = HourRange.Of(hour, (hour + 1) % 24);
            if (!PeakHours.Any(h => h.StartHour == range.StartHour))
                PeakHours.Add(range);
        }
    }

    private bool CheckThresholdExceeded(string circuitId, decimal kWh, out decimal threshold)
    {
        threshold = 0;
        if (!_consumptionThresholds.TryGetValue(circuitId, out threshold))
            return false;
        return kWh > threshold;
    }
}
