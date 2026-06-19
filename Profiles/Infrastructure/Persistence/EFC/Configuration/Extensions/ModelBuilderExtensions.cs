using Hampcoders.Electrolink.API.Profiles.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.Entities;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Hampcoders.Electrolink.API.Profiles.Infrastructure.Persistence.EFC.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    // ── Conversores reutilizables ──────────────────────────────────────────
    private static readonly ValueConverter<ProfileId, string> ProfileIdConverter =
        new(id => id.Value, raw => ProfileId.From(raw));

    private static readonly ValueConverter<UserId, string> UserIdConverter =
        new(id => id.Value, raw => UserId.From(raw));

    private static readonly ValueConverter<HomeownerId, string> HomeownerIdConverter =
        new(id => id.Value, raw => HomeownerId.From(raw));

    private static readonly ValueConverter<TechnicianId, string> TechnicianIdConverter =
        new(id => id.Value, raw => TechnicianId.From(raw));

    private static readonly ValueConverter<CompanyId, string> CompanyIdConverter =
        new(id => id.Value, raw => CompanyId.From(raw));

    private const string ProfileIdFk = "profile_id";

    public static void ApplyProfilesConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Profile>(b =>
        {
            b.HasKey(p => p.ProfileId);
            b.Property(p => p.ProfileId)
                .HasConversion(ProfileIdConverter)
                .HasColumnName(ProfileIdFk)   // <-- nombre explícito en la PK
                .IsRequired();

            b.Property(p => p.UserId)
                .HasConversion(UserIdConverter)
                .HasColumnName("user_id")
                .IsRequired();

            b.Property(p => p.Status)
                .HasConversion<string>()
                .HasColumnName("status")
                .IsRequired();

            b.Property(p => p.BusinessRole)
                .HasConversion<string>()
                .HasColumnName("business_role");

            b.Property(p => p.SubscriptionTier)
                .HasColumnName("subscription_tier")
                .HasMaxLength(30);

            b.OwnsOne(p => p.Photo, ph =>
            {
                ph.WithOwner().HasForeignKey(ProfileIdFk);
                ph.Property(ph2 => ph2.PublicUrl)
                    .HasColumnName("photo_public_url")
                    .HasMaxLength(500);
                ph.Property(ph2 => ph2.ProviderId)
                    .HasColumnName("photo_provider_id")
                    .HasMaxLength(500);
                ph.Property(ph2 => ph2.UploadedAt)
                    .HasColumnName("photo_uploaded_at");
            });

            b.OwnsOne(p => p.PersonalData, n =>
            {
                n.WithOwner().HasForeignKey(ProfileIdFk); 

                n.Property(pd => pd.FirstName).HasColumnName("first_name").IsRequired();
                n.Property(pd => pd.LastName).HasColumnName("last_name").IsRequired();

                n.OwnsOne(pd => pd.PhoneNumber, pn =>
                {
                    pn.WithOwner().HasForeignKey(ProfileIdFk);
                    pn.Property(v => v.Value)
                        .HasColumnName("phone_number")
                        .IsRequired();
                });

                n.OwnsOne(pd => pd.Dni, d =>
                {
                    d.WithOwner().HasForeignKey(ProfileIdFk);
                    d.Property(v => v.Value)
                        .HasColumnName("dni")
                        .IsRequired();
                });

                n.OwnsOne(pd => pd.DateOfBirth, db =>
                {
                    db.WithOwner().HasForeignKey(ProfileIdFk);
                    db.Property(v => v.Value)
                        .HasColumnName("date_of_birth")
                        .IsRequired();
                });

                n.OwnsOne(pd => pd.Address, a =>
                {
                    a.WithOwner().HasForeignKey(ProfileIdFk);
                    a.Property(s => s.Street);
                    a.Property(s => s.Number);
                    a.Property(s => s.District);
                    a.Property(s => s.City);
                    a.Property(s => s.PostalCode);
                    a.Property(s => s.Country);
                });
            });

            b.HasOne(p => p.Homeowner)
                .WithOne()
                .HasForeignKey<HomeOwner>(ho => ho.ProfileId);

            b.HasOne(p => p.Technician)
                .WithOne()
                .HasForeignKey<Technician>(t => t.ProfileId);

            b.HasOne(p => p.Company)
                .WithOne()
                .HasForeignKey<Company>(c => c.ProfileId);
        });

        // ── HomeOwner ─────────────────────────────────────────────────────
        builder.Entity<HomeOwner>(b =>
        {
            b.HasKey(ho => ho.HomeownerId);
            b.Property(ho => ho.HomeownerId)
             .HasConversion(HomeownerIdConverter)
             .IsRequired();

            b.Property(ho => ho.ProfileId)
             .HasConversion(ProfileIdConverter)
             .IsRequired();

            b.Property(ho => ho.PreferredContactTime)
             .HasConversion<string>()
             .IsRequired();

            b.OwnsOne(ho => ho.CommunicationPreferences, cp =>
            {
                cp.WithOwner().HasForeignKey("HomeownerId");
                cp.Property(c => c.SmsNotifications);
                cp.Property(c => c.EmailNotifications);
                cp.Property(c => c.PushNotifications);
                cp.Property(c => c.PreferredContactTime).HasConversion<string>();
            });

            b.OwnsOne(ho => ho.EmergencyContact, ec =>
            {
                ec.WithOwner().HasForeignKey("HomeownerId");
            });

            b.Property(ho => ho.AverageRating)
                .HasColumnName("average_rating")
                .HasDefaultValue(0.0);
            b.Property(ho => ho.ActiveServiceCount)
                .HasColumnName("active_service_count")
                .HasDefaultValue(0);
        });

        // ── Technician ────────────────────────────────────────────────────
        builder.Entity<Technician>(b =>
        {
            b.HasKey(t => t.TechnicianId);
            b.Property(t => t.TechnicianId)
             .HasConversion(TechnicianIdConverter)
             .IsRequired();

            b.Property(t => t.ProfileId)
             .HasConversion(ProfileIdConverter)
             .IsRequired();

            b.Property(t => t.ExperienceYears).IsRequired();
            b.Property(t => t.AboutMe).HasMaxLength(2000);

            b.Property(t => t.AverageRating)
                .HasColumnName("average_rating")
                .HasDefaultValue(0.0);

            b.Property(t => t.ActiveServiceCount)
                .HasColumnName("active_service_count")
                .HasDefaultValue(0);

            b.Ignore(t => t.PortfolioItems);

            b.Property(t => t.IsIoTCertified)
                .HasColumnName("is_iot_certified")
                .IsRequired()
                .HasDefaultValue(false);

            b.OwnsOne(t => t.IoTCertification, cert =>
            {
                cert.WithOwner().HasForeignKey("TechnicianId");
                cert.Property(c => c.Name).HasColumnName("iot_cert_name").HasMaxLength(200);
                cert.Property(c => c.IssuerOrganization).HasColumnName("iot_cert_issuer").HasMaxLength(200);
                cert.Property(c => c.DateObtained).HasColumnName("iot_cert_date_obtained");
                cert.Property(c => c.ExpirationDate).HasColumnName("iot_cert_expiration");
                cert.Property(c => c.CredentialId).HasColumnName("iot_cert_credential_id").HasMaxLength(100);
                cert.Property(c => c.CredentialUrl).HasColumnName("iot_cert_credential_url").HasMaxLength(500);
            });
            
            b.OwnsOne(t => t.ServiceArea, sa =>
            {
                sa.WithOwner().HasForeignKey("TechnicianId");
                sa.Property(v => v.CenterLatitude)
                    .HasColumnName("service_area_lat")
                    .HasColumnType("decimal(9,6)")
                    .IsRequired();

                sa.Property(v => v.CenterLongitude)
                    .HasColumnName("service_area_lon")
                    .HasColumnType("decimal(9,6)")
                    .IsRequired();

                sa.Property(v => v.RadiusKm)
                    .HasColumnName("service_area_radius_km")
                    .HasColumnType("decimal(6,2)")
                    .IsRequired();

                sa.Property(v => v.Area)
                    .HasColumnName("service_area_geom")
                    .HasColumnType("geometry(Polygon, 4326)")
                    .IsRequired();
                
                sa.HasIndex(v => v.Area)
                    .HasMethod("GIST");
            });
            
            b.HasMany(typeof(TechnicianSpecialty), "_specialtyEntities")
             .WithOne(nameof(TechnicianSpecialty.Technician))
             .HasForeignKey("TechnicianId")
             .IsRequired();
        });

        // ── Company ──────────────────────────────────────────────────────
        builder.Entity<Company>(b =>
        {
            b.HasKey(c => c.CompanyId);
            b.Property(c => c.CompanyId)
             .HasConversion(CompanyIdConverter)
             .IsRequired();

            b.Property(c => c.ProfileId)
             .HasConversion(ProfileIdConverter)
             .IsRequired();

            b.OwnsOne(c => c.CompanyData, cd =>
            {
                cd.WithOwner().HasForeignKey("CompanyId");
                cd.Property(v => v.CompanyName)
                    .HasColumnName("company_name")
                    .HasMaxLength(200)
                    .IsRequired();

                cd.OwnsOne(v => v.TaxId, t =>
                {
                    t.WithOwner().HasForeignKey("CompanyId");
                    t.Property(p => p.Value)
                        .HasColumnName("company_tax_id")
                        .HasMaxLength(50)
                        .IsRequired();
                });

                cd.Property(v => v.Industry)
                    .HasColumnName("company_industry")
                    .HasMaxLength(100);

                cd.Property(v => v.Size)
                    .HasColumnName("company_size")
                    .HasConversion<string>()
                    .IsRequired();

                cd.Property(v => v.Website)
                    .HasColumnName("company_website")
                    .HasMaxLength(500);

                cd.OwnsOne(v => v.BillingAddress, a =>
                {
                    a.WithOwner().HasForeignKey("CompanyId");
                    a.Property(s => s.Street).HasColumnName("billing_street");
                    a.Property(s => s.Number).HasColumnName("billing_number");
                    a.Property(s => s.District).HasColumnName("billing_district");
                    a.Property(s => s.City).HasColumnName("billing_city");
                    a.Property(s => s.PostalCode).HasColumnName("billing_postal_code");
                    a.Property(s => s.Country).HasColumnName("billing_country");
                });
            });
        });

        // ── TechnicianSpecialty ───────────────────────────────────────────
        builder.Entity<TechnicianSpecialty>(b =>
        {
            b.HasKey(nameof(TechnicianSpecialty.TechnicianId), nameof(TechnicianSpecialty.Specialty));

            b.Property(ts => ts.TechnicianId)
             .HasConversion(TechnicianIdConverter)
             .IsRequired();

            b.Property(ts => ts.Specialty)
             .HasConversion<int>()
             .IsRequired();

            b.HasOne(ts => ts.Technician)
             .WithMany("_specialtyEntities")
             .HasForeignKey(ts => ts.TechnicianId);
        });
    }
}