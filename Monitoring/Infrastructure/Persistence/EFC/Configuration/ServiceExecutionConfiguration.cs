using Hampcoders.Electrolink.API.Monitoring.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Monitoring.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Monitoring.Infrastructure.Persistence.EFC.Configuration;

public class ServiceExecutionConfiguration : IEntityTypeConfiguration<ServiceExecution>
{
    public void Configure(EntityTypeBuilder<ServiceExecution> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => ServiceExecutionId.From(value))
            .IsRequired();

        builder.Property(x => x.AssignmentId)
            .HasConversion(id => id.Value, value => AssignmentId.From(value))
            .IsRequired();

        builder.Property(x => x.RequestId)
            .HasConversion(id => id.Value, value => RequestId.From(value))
            .IsRequired();

        builder.Property(x => x.TechnicianId)
            .HasConversion(id => id.Value, value => TechnicianId.From(value))
            .IsRequired();

        builder.OwnsOne(x => x.Owner, o =>
        {
            o.WithOwner().HasForeignKey("Id");
            o.Property(c => c.ClientType).HasConversion<string>().HasColumnName("owner_type").HasMaxLength(20).IsRequired();
            o.Property(c => c.ClientId).HasColumnName("owner_id").HasMaxLength(100).IsRequired();
        });

        builder.Property(x => x.PropertyId)
            .HasConversion(id => id.Value, value => PropertyId.From(value))
            .IsRequired();

        builder.Property(x => x.RecipeSnapshot)
            .HasConversion(
                recipe => JsonSerializer.Serialize(recipe, (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<RecipeSnapshot>(json, (JsonSerializerOptions?)null)!
            )
            .IsRequired();

        builder.Property(x => x.ServiceType)
            .HasConversion<string>()
            .IsRequired();

        builder.OwnsOne(x => x.IotContext, iot =>
        {
            iot.WithOwner().HasForeignKey("Id");
            iot.Property(i => i.DeviceId)
                .HasConversion(id => id.Value, v => DeviceId.From(v))
                .IsRequired(false);
            iot.Property(i => i.RelayState)
                .HasConversion<string>();
            iot.Property(i => i.Timestamp);
        });

        builder.OwnsMany(x => x.RelayActionRecords, ra =>
        {
            ra.ToTable("RelayActionRecords");
            ra.WithOwner().HasForeignKey("ExecutionId");
            ra.HasKey(x => x.Id);
            ra.HasIndex("ExecutionId");
            ra.Property(x => x.Id)
                .HasConversion(id => id.Value, v => RelayActionId.From(v))
                .IsRequired();
            ra.Property(x => x.DeviceId)
                .HasConversion(id => id.Value, v => DeviceId.From(v))
                .IsRequired();
            ra.Property(x => x.Status).HasConversion<string>().IsRequired();
            ra.Property(x => x.IssuedAt).IsRequired();
            ra.Property(x => x.ExecutedAt);
            ra.Property(x => x.FailureReason).HasMaxLength(500);
        });

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.ScheduledDateTime).IsRequired();
        builder.Property(x => x.StartedAt);
        builder.Property(x => x.CompletedAt);
        builder.Property(x => x.CancelledAt);

        builder.OwnsMany(x => x.WorkPhotos, wp =>
        {
            wp.WithOwner().HasForeignKey("ExecutionId");
            wp.HasKey(x => x.Id);
            wp.Property(x => x.Id).HasConversion(id => id.Value, v => WorkPhotoId.From(v)).IsRequired();
            wp.Property(p => p.ExecutionId).HasConversion(id => id.Value, v => ServiceExecutionId.From(v)).IsRequired();
            wp.Property(p => p.PhotoType).HasConversion<string>().IsRequired();
            wp.Property(p => p.PhotoUrl).IsRequired();
            wp.Property(p => p.ProviderId).HasMaxLength(500).IsRequired();
            wp.Property(p => p.ThumbnailUrl).HasMaxLength(500);
            wp.Property(p => p.SizeBytes).IsRequired();
            wp.Property(p => p.Format).HasMaxLength(10).IsRequired();
            wp.Property(p => p.TakenAt).IsRequired();
            wp.Property(p => p.Notes);
        });

        builder.OwnsMany(x => x.ComponentSubstitutions, cs =>
        {
            cs.WithOwner().HasForeignKey("ExecutionId");
            cs.HasKey(x => x.Id);
            cs.Property(x => x.Id).HasConversion(id => id.Value, v => ComponentUsageId.From(v)).IsRequired();
            cs.Property(c => c.ExecutionId).HasConversion(id => id.Value, v => ServiceExecutionId.From(v)).IsRequired();
            cs.Property(c => c.ComponentTypeId).IsRequired();
            cs.Property(c => c.ComponentTypeName).IsRequired();
            cs.Property(c => c.QuantityUsed).IsRequired();
            cs.Property(c => c.QuantityReserved).IsRequired();
            cs.Ignore(c => c.Delta);
        });

        builder.OwnsMany(x => x.Evaluations, ev =>
        {
            ev.ToTable("ServiceEvaluations");
            ev.WithOwner().HasForeignKey("ExecutionId");
            ev.HasKey(x => x.Id);
            ev.Property(x => x.Id)
                .HasConversion(id => id.Value, v => EvaluationId.From(v))
                .IsRequired();
            ev.Property(e => e.ExecutionId)
                .HasConversion(id => id.Value, v => ServiceExecutionId.From(v))
                .IsRequired();
            ev.Property(e => e.ReviewerId).IsRequired();
            ev.Property(e => e.ReviewedId).IsRequired();
            ev.Property(e => e.ReviewerRole).HasMaxLength(50).IsRequired();
            ev.Property(e => e.Rating).IsRequired();
            ev.Property(e => e.Comment);
            ev.Property(e => e.CategoriesJson).IsRequired();
            ev.Property(e => e.SubmittedAt).IsRequired();
        });
    }
}
