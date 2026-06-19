using Hampcoders.Electrolink.API.Processing.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Processing.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hampcoders.Electrolink.API.Processing.Infrastructure.Persistence.EFC.Configurations;

public class AnomalyRecordConfiguration : IEntityTypeConfiguration<AnomalyRecord>
{
    public void Configure(EntityTypeBuilder<AnomalyRecord> builder)
    {
        builder.ToTable("iot_anomaly_records");
        builder.HasKey(a => a.AnomalyId);

        builder.Property(a => a.AnomalyId)
            .HasColumnName("anomaly_id").HasMaxLength(60)
            .HasConversion(id => id.Value, v => AnomalyRecordId.From(v))
            .IsRequired().ValueGeneratedNever();

        builder.Property(a => a.DeviceId)
            .HasColumnName("device_id").HasMaxLength(60)
            .HasConversion(id => id.Value, v => DeviceId.From(v))
            .IsRequired();

        builder.Property(a => a.PropertyId)
            .HasColumnName("property_id").HasMaxLength(60)
            .HasConversion(id => id.Value, v => PropertyId.From(v))
            .IsRequired();

        builder.OwnsOne(x => x.Owner, o => {
            o.WithOwner().HasForeignKey("AnomalyId");
            o.Property(c => c.ClientType).HasConversion<string>().HasColumnName("owner_type").HasMaxLength(20).IsRequired();
            o.Property(c => c.ClientId).HasColumnName("owner_id").HasMaxLength(100).IsRequired();
        });

        builder.Property(a => a.AnomalyType)
            .HasColumnName("anomaly_type").HasMaxLength(40)
            .HasConversion<string>().IsRequired();

        builder.Property(a => a.Severity)
            .HasColumnName("severity").HasMaxLength(20)
            .HasConversion<string>().IsRequired();

        builder.Property(a => a.DetectionLayer)
            .HasColumnName("detection_layer").HasMaxLength(10)
            .HasConversion<string>().IsRequired();

        builder.Property(a => a.Status)
            .HasColumnName("status").HasMaxLength(20)
            .HasConversion<string>().IsRequired();

        builder.Property(a => a.TriggerReadingId)
            .HasColumnName("trigger_reading_id").HasMaxLength(100)
            .HasConversion(id => id.Value, v => ReadingId.From(v))
            .IsRequired();

        builder.Property(a => a.DetectedAt).HasColumnName("detected_at").IsRequired();
        builder.Property(a => a.ResolvedAt).HasColumnName("resolved_at");
        builder.Property(a => a.AcknowledgedAt).HasColumnName("acknowledged_at");
        builder.Property(a => a.RelatedServiceSuggestionId).HasColumnName("related_suggestion_id").HasMaxLength(60);
        builder.Property(a => a.EdgeAlertPayloadJson).HasColumnName("edge_alert_payload").HasColumnType("jsonb");
        builder.Property(a => a.AutoRelayActivated).HasColumnName("auto_relay_activated").IsRequired();

        builder.Property(a => a.CreatedDate).HasColumnName("created_at").IsRequired();
        builder.Property(a => a.UpdatedDate).HasColumnName("updated_at");

        builder.HasIndex(a => a.DeviceId);
        builder.HasIndex(a => a.PropertyId);
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => new { a.DeviceId, a.Status, a.AnomalyType });

        builder.Ignore(a => a.DomainEvents);
    }
}
