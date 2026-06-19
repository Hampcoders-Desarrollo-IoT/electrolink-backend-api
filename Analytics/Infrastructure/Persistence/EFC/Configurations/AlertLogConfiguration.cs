using Hampcoders.Electrolink.API.Analytics.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Analytics.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceRequestId = Hampcoders.Electrolink.API.Analytics.Domain.Model.ValueObjects.ServiceRequestId;

namespace Hampcoders.Electrolink.API.Analytics.Infrastructure.Persistence.EFC.Configurations;

public class AlertLogConfiguration : IEntityTypeConfiguration<AlertLog>
{
    public void Configure(EntityTypeBuilder<AlertLog> builder)
    {
        builder.ToTable("analytics_alert_logs");

        builder.HasKey(l => l.LogId);
        builder.Property(l => l.LogId)
            .HasConversion(id => id.Value, v => AlertLogId.From(v))
            .HasColumnName("log_id");

        builder.OwnsOne(l => l.Owner, o =>
        {
            o.WithOwner().HasForeignKey("LogId");
            o.Property(c => c.ClientType).HasConversion<string>().HasColumnName("owner_type").HasMaxLength(20).IsRequired();
            o.Property(c => c.ClientId).HasColumnName("owner_id").HasMaxLength(100).IsRequired();
        });

        builder.OwnsMany(l => l.Entries, e =>
        {
            e.ToTable("analytics_alert_entries");
            e.HasKey(entry => entry.EntryId);

            e.Property(entry => entry.EntryId)
                .HasConversion(id => id.Value, v => AlertEntryId.From(v))
                .HasColumnName("entry_id");

            e.Property(entry => entry.SourceEventId)
                .HasConversion(id => id.Value, v => SourceEventId.From(v))
                .HasColumnName("source_event_id");

            e.Property(entry => entry.LinkedServiceRequestId)
                .HasConversion(
                    id => id != null ? id.Value : null,
                    v => v != null ? ServiceRequestId.From(v) : null)
                .HasColumnName("linked_service_request_id");

            e.Property(entry => entry.SourceBC).HasConversion<string>().HasColumnName("source_bc");
            e.Property(entry => entry.AlertType).HasConversion<string>().HasColumnName("alert_type");
            e.Property(entry => entry.Severity).HasConversion<string>().HasColumnName("severity");
            e.Property(entry => entry.Status).HasConversion<string>().HasColumnName("status");
            e.Property(entry => entry.CircuitId).HasColumnName("circuit_id");
            e.Property(entry => entry.TriggeredAt).HasColumnName("triggered_at");
            e.Property(entry => entry.AcknowledgedAt).HasColumnName("acknowledged_at");
            e.Property(entry => entry.ResolvedAt).HasColumnName("resolved_at");
        });

        builder.Ignore(l => l.DomainEvents);
    }
}
