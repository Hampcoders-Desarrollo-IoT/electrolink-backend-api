using Hampcoders.Electrolink.API.Profiles.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hampcoders.Electrolink.API.Profiles.Infrastructure.Persistence.EFC.Configurations;

public class StaffMemberConfiguration : IEntityTypeConfiguration<StaffMember>
{
    public void Configure(EntityTypeBuilder<StaffMember> builder)
    {
        builder.ToTable("prof_staff_members");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .HasConversion(id => id.Value, raw => StaffMemberId.From(raw))
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(s => s.UserId)
            .HasConversion(uid => uid.Value, raw => UserId.From(raw))
            .HasColumnName("user_id")
            .IsRequired();

        builder.HasIndex(s => s.UserId).IsUnique();

        builder.Property(s => s.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.PhoneNumber)
            .HasColumnName("phone_number")
            .HasMaxLength(20);

        builder.OwnsOne(s => s.AssignedZone, zone =>
        {
            zone.Property(z => z.Region).HasColumnName("zone_region").HasMaxLength(100);
            zone.Property(z => z.CenterLatitude).HasColumnName("zone_latitude");
            zone.Property(z => z.CenterLongitude).HasColumnName("zone_longitude");
            zone.Property(z => z.RadiusKm).HasColumnName("zone_radius_km");
        });

        builder.Property(s => s.IsIoTCertified)
            .HasColumnName("is_iot_certified")
            .HasDefaultValue(false);

        builder.Property(s => s.IoTCertifiedAt)
            .HasColumnName("iot_certified_at");

        builder.Property(s => s.HireDate)
            .HasColumnName("hire_date")
            .IsRequired();

        builder.Property(s => s.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Ignore(s => s.DomainEvents);
    }
}
