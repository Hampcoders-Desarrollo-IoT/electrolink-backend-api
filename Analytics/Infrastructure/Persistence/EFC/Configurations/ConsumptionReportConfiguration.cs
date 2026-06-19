using Hampcoders.Electrolink.API.Analytics.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Analytics.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hampcoders.Electrolink.API.Analytics.Infrastructure.Persistence.EFC.Configurations;

public class ConsumptionReportConfiguration : IEntityTypeConfiguration<ConsumptionReport>
{
    public void Configure(EntityTypeBuilder<ConsumptionReport> builder)
    {
        builder.ToTable("analytics_consumption_reports");

        builder.HasKey(r => r.ReportId);
        builder.Property(r => r.ReportId)
            .HasConversion(id => id.Value, v => ConsumptionReportId.From(v))
            .HasColumnName("report_id");

        builder.OwnsOne(r => r.RequestedBy, o =>
        {
            o.WithOwner().HasForeignKey("ReportId");
            o.Property(c => c.ClientType).HasConversion<string>().HasColumnName("owner_type").HasMaxLength(20).IsRequired();
            o.Property(c => c.ClientId).HasColumnName("owner_id").HasMaxLength(100).IsRequired();
        });

        builder.Property(r => r.PropertyId)
            .HasConversion(id => id.Value, v => PropertyId.From(v))
            .HasColumnName("property_id");

        builder.Property(r => r.PeriodStart).HasColumnName("period_start");
        builder.Property(r => r.PeriodEnd).HasColumnName("period_end");
        builder.Property(r => r.GeneratedAt).HasColumnName("generated_at");
        builder.Property(r => r.ExportFormat).HasConversion<string>().HasColumnName("export_format");
        builder.Property(r => r.DownloadUrl).HasColumnName("download_url");
        builder.Property(r => r.ExpiresAt).HasColumnName("expires_at");
        builder.Property(r => r.IsGenerated).HasColumnName("is_generated");

        builder.Ignore(r => r.DomainEvents);
    }
}
