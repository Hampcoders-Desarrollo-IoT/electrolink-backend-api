using Hampcoders.Electrolink.API.Processing.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Processing.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hampcoders.Electrolink.API.Processing.Infrastructure.Persistence.EFC.Configurations;

public class DeviceReadingStreamConfiguration : IEntityTypeConfiguration<DeviceReadingStream>
{
    public void Configure(EntityTypeBuilder<DeviceReadingStream> builder)
    {
        builder.ToTable("iot_device_reading_streams");
        builder.HasKey(s => s.StreamId);

        builder.Property(s => s.StreamId)
            .HasColumnName("stream_id").HasMaxLength(60)
            .HasConversion(id => id.Value, v => StreamId.From(v))
            .IsRequired().ValueGeneratedNever();

        builder.Property(s => s.DeviceId)
            .HasColumnName("device_id").HasMaxLength(60)
            .HasConversion(id => id.Value, v => DeviceId.From(v))
            .IsRequired();

        builder.HasIndex(s => s.DeviceId).IsUnique();

        builder.Property(s => s.PropertyId)
            .HasColumnName("property_id").HasMaxLength(60)
            .HasConversion(id => id.Value, v => PropertyId.From(v))
            .IsRequired();

        builder.OwnsOne(x => x.Owner, o => {
            o.WithOwner().HasForeignKey("StreamId");
            o.Property(c => c.ClientType).HasConversion<string>().HasColumnName("owner_type").HasMaxLength(20).IsRequired();
            o.Property(c => c.ClientId).HasColumnName("owner_id").HasMaxLength(100).IsRequired();
        });

        builder.Property(s => s.StreamStatus)
            .HasColumnName("stream_status").HasMaxLength(20)
            .HasConversion<string>().IsRequired();

        builder.Property(s => s.LastReceivedAt)
            .HasColumnName("last_received_at").IsRequired();

        builder.Property(s => s.ConsecutiveAnomalyCount)
            .HasColumnName("consecutive_anomaly_count").IsRequired();

        builder.Property(s => s.ConsecutiveNormalCount)
            .HasColumnName("consecutive_normal_count").IsRequired();

        builder.OwnsOne(s => s.CustomThresholds, tc =>
        {
            tc.WithOwner().HasForeignKey("StreamId");
            tc.Property(x => x.NominalVoltage)
                .HasColumnName("threshold_nominal_voltage").IsRequired();
            tc.Property(x => x.MaxConsumptionWatts)
                .HasColumnName("threshold_max_consumption_watts").IsRequired();
            tc.Property(x => x.MaxCurrentAmps)
                .HasColumnName("threshold_max_current_amps").IsRequired();
            tc.Property(x => x.MinPowerFactor)
                .HasColumnName("threshold_min_power_factor").IsRequired();
            tc.Property(x => x.NominalFrequency)
                .HasColumnName("threshold_nominal_frequency").IsRequired();
            tc.Property(x => x.DisconnectionThresholdMin)
                .HasColumnName("threshold_disconnection_min").IsRequired();
        });

        builder.Property(s => s.CreatedDate).HasColumnName("created_at").IsRequired();
        builder.Property(s => s.UpdatedDate).HasColumnName("updated_at");

        builder.HasMany(s => s.Readings)
            .WithOne()
            .HasForeignKey("StreamId")
            .HasConstraintName("fk_iot_readings_stream")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(s => s.DomainEvents);
    }
}
