using System.Text.Json;
using Hampcoders.Electrolink.API.Analytics.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Analytics.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Hampcoders.Electrolink.API.Analytics.Infrastructure.Persistence.EFC.Configurations;

public class ConsumptionDashboardConfiguration : IEntityTypeConfiguration<ConsumptionDashboard>
{
    private static readonly JsonSerializerOptions JsonOptions = new();

    public void Configure(EntityTypeBuilder<ConsumptionDashboard> builder)
    {
        builder.ToTable("analytics_consumption_dashboards");

        builder.HasKey(d => d.DashboardId);
        builder.Property(d => d.DashboardId)
            .HasConversion(id => id.Value, v => ConsumptionDashboardId.From(v))
            .HasColumnName("dashboard_id");

        builder.OwnsOne(d => d.Owner, o =>
        {
            o.WithOwner().HasForeignKey("DashboardId");
            o.Property(c => c.ClientType).HasConversion<string>().HasColumnName("owner_type").HasMaxLength(20).IsRequired();
            o.Property(c => c.ClientId).HasColumnName("owner_id").HasMaxLength(100).IsRequired();
        });

        builder.Property(d => d.PropertyId)
            .HasConversion(id => id.Value, v => PropertyId.From(v))
            .HasColumnName("property_id");

        builder.Property(d => d.PlanTier)
            .HasConversion<string>()
            .HasColumnName("plan_tier");

        builder.Property(d => d.LastUpdatedAt).HasColumnName("last_updated_at");

        builder.Property(d => d.DeviceIds)
            .HasConversion(new ValueConverter<List<DeviceId>, string>(
                v => SerializeDeviceIds(v),
                v => DeserializeDeviceIds(v)))
            .HasColumnType("jsonb")
            .HasColumnName("device_ids");

        builder.Property(d => d.AlertHistoryIds)
            .HasConversion(new ValueConverter<List<AlertLogId>, string>(
                v => SerializeAlertLogIds(v),
                v => DeserializeAlertLogIds(v)))
            .HasColumnType("jsonb")
            .HasColumnName("alert_history_ids");

        builder.Property(d => d.PeakHours)
            .HasConversion(new ValueConverter<List<HourRange>, string>(
                v => SerializePeakHours(v),
                v => DeserializePeakHours(v)))
            .HasColumnType("jsonb")
            .HasColumnName("peak_hours");

        builder.OwnsOne(d => d.CostProjection, m =>
        {
            m.WithOwner().HasForeignKey("DashboardId");
            m.Property(p => p.Amount).HasColumnName("cost_projection_amount");
            m.Property(p => p.Currency).HasColumnName("cost_projection_currency");
        });

        builder.OwnsOne(d => d.Savings, s =>
        {
            s.WithOwner().HasForeignKey("DashboardId");
            s.Property(p => p.PreventiveCostPerEvent).HasColumnName("savings_preventive_cost");
            s.Property(p => p.CorrectiveCostPerEvent).HasColumnName("savings_corrective_cost");
            s.Property(p => p.AnomaliesDetectedOnTime).HasColumnName("savings_anomalies_on_time");
            s.Property(p => p.AnomaliesDetectedLate).HasColumnName("savings_anomalies_late");
        });

        builder.Property<Dictionary<string, decimal>>("_consumptionThresholds")
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonOptions),
                v => JsonSerializer.Deserialize<Dictionary<string, decimal>>(v, JsonOptions) ?? new Dictionary<string, decimal>())
            .HasColumnType("jsonb")
            .HasColumnName("consumption_thresholds");

        builder.Ignore(d => d.DomainEvents);

        AddComparers(builder);
    }

    private static void AddComparers(EntityTypeBuilder<ConsumptionDashboard> builder)
    {
        var deviceComparer = new ValueComparer<List<DeviceId>>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

        builder.Property(d => d.DeviceIds).Metadata.SetValueComparer(deviceComparer);

        var alertComparer = new ValueComparer<List<AlertLogId>>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

        builder.Property(d => d.AlertHistoryIds).Metadata.SetValueComparer(alertComparer);

        var peakHourComparer = new ValueComparer<List<HourRange>>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToList());

        builder.Property(d => d.PeakHours).Metadata.SetValueComparer(peakHourComparer);

        var thresholdComparer = new ValueComparer<Dictionary<string, decimal>>(
            (c1, c2) => c1!.SequenceEqual(c2!),
            c => c.Aggregate(0, (a, kv) => HashCode.Combine(a, kv.Key.GetHashCode(), kv.Value.GetHashCode())),
            c => new Dictionary<string, decimal>(c!));

        builder.Property("_consumptionThresholds").Metadata.SetValueComparer(thresholdComparer);
    }

    private static string SerializeDeviceIds(List<DeviceId> ids)
        => JsonSerializer.Serialize(ids.Select(x => x.Value).ToList(), JsonOptions);

    private static List<DeviceId> DeserializeDeviceIds(string json)
    {
        var items = JsonSerializer.Deserialize<List<string>>(json, JsonOptions);
        if (items == null) return [];
        return items.Select(DeviceId.From).ToList();
    }

    private static string SerializeAlertLogIds(List<AlertLogId> ids)
        => JsonSerializer.Serialize(ids.Select(x => x.Value).ToList(), JsonOptions);

    private static List<AlertLogId> DeserializeAlertLogIds(string json)
    {
        var items = JsonSerializer.Deserialize<List<string>>(json, JsonOptions);
        if (items == null) return [];
        return items.Select(AlertLogId.From).ToList();
    }

    private static string SerializePeakHours(List<HourRange> hours)
        => JsonSerializer.Serialize(hours.Select(x => new { x.StartHour, x.EndHour }).ToList(), JsonOptions);

    private static List<HourRange> DeserializePeakHours(string json)
    {
        var items = JsonSerializer.Deserialize<List<HourRangeData>>(json, JsonOptions);
        if (items == null) return [];
        return items.Select(h => HourRange.Of(h.StartHour, h.EndHour)).ToList();
    }

    private record HourRangeData(int StartHour, int EndHour);
}
