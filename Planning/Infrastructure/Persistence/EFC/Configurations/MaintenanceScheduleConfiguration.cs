using Hampcoders.Electrolink.API.Planning.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hampcoders.Electrolink.API.Planning.Infrastructure.Persistence.EFC.Configurations;

public class MaintenanceScheduleConfiguration : IEntityTypeConfiguration<MaintenanceSchedule>
{
    public void Configure(EntityTypeBuilder<MaintenanceSchedule> builder)
    {
        builder.ToTable("plan_maintenance_schedules");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasConversion(id => id.Value, raw => ScheduleId.From(raw))
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(s => s.CompanyId)
            .HasConversion(id => id.Value, raw => CompanyId.From(raw))
            .HasColumnName("company_id")
            .IsRequired();

        builder.Property(s => s.PropertyId)
            .HasConversion(id => id.Value, raw => PropertyId.From(raw))
            .HasColumnName("property_id")
            .IsRequired();

        builder.Property(s => s.Frequency)
            .HasConversion<string>()
            .HasColumnName("frequency")
            .IsRequired();

        builder.Property(s => s.NextVisitDate)
            .HasColumnName("next_visit_date")
            .IsRequired();

        builder.Property(s => s.AssignedStaffMemberId)
            .HasConversion(id => id.Value, raw => StaffMemberId.From(raw))
            .HasColumnName("assigned_staff_member_id");

        builder.Property(s => s.LastVisitDate)
            .HasColumnName("last_visit_date");

        builder.Property(s => s.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Ignore(s => s.DomainEvents);
    }
}
