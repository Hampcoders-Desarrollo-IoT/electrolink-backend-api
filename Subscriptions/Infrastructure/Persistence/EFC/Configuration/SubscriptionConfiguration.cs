using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProfileId = Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects.ProfileId;

namespace Hampcoders.Electrolink.API.Subscriptions.Infrastructure.Persistence.EFC.Configuration;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("sp_subscriptions");

        builder.HasKey(s => s.SubscriptionId);

        builder.Property(s => s.SubscriptionId)
            .HasColumnName("id")
            .HasConversion(id => id.Value, v => SubscriptionId.From(v))
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.UserId)
            .HasColumnName("user_id")
            .HasConversion(id => id.Value, v => UserId.From(v))
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.ProfileId)
            .HasColumnName("profile_id")
            .HasConversion(id => id.Value, v => ProfileId.From(v))
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.BusinessRole)
            .HasColumnName("business_role")
            .HasConversion(r => r.ToString(), v => BusinessRole.From(v))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.PlanType)
            .HasColumnName("plan_type")
            .HasConversion(p => p.ToString(), v => PlanType.From(v))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.BillingCycle)
            .HasColumnName("billing_cycle")
            .HasConversion(
                c => c == null ? null : c.ToString(),
                v => v == null ? null : BillingCycle.From(v))
            .HasMaxLength(20);

        builder.Property(s => s.Status)
            .HasColumnName("status")
            .HasConversion(st => st.ToString(), v => SubscriptionStatus.From(v))
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(s => s.CancelAtPeriodEnd)
            .HasColumnName("cancel_at_period_end")
            .IsRequired();

        builder.Property(s => s.StripeCustomerId)
            .HasColumnName("stripe_customer_id")
            .HasConversion(id => id.Value, v => StripeCustomerId.From(v))
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.StripeSubscriptionId)
            .HasColumnName("stripe_subscription_id")
            .HasConversion(
                id => id == null ? null : id.Value,
                v => v == null ? null : StripeSubscriptionId.From(v))
            .HasMaxLength(100);

        builder.Property(s => s.GracePeriodEndsAt)
            .HasColumnName("grace_period_ends_at");

        builder.OwnsOne(s => s.BillingPeriod, period =>
        {
            period.WithOwner().HasForeignKey("id"); // 🔥 CLAVE

            period.Property(p => p.PeriodStart)
                .HasColumnName("period_start");

            period.Property(p => p.PeriodEnd)
                .HasColumnName("period_end");
        });

        builder.OwnsOne(s => s.UsageCounters, usage =>
        {
            usage.WithOwner().HasForeignKey("id"); // 🔥 MISMO FIX

            usage.Property(u => u.MonthlyRequestsUsed)
                .HasColumnName("monthly_requests_used")
                .HasDefaultValue(0);
            usage.Property(u => u.MonthlyRequestsLimit)
                .HasColumnName("monthly_requests_limit")
                .HasDefaultValue(2);
        });

        builder.Property(s => s.InstallationServiceRequestId)
            .HasColumnName("installation_service_request_id")
            .HasMaxLength(100);

        builder.Property(s => s.InstallationDeadlineAt)
            .HasColumnName("installation_deadline_at");

        builder.Property(s => s.ActiveDeviceCount)
            .HasColumnName("active_device_count");

        builder.Property(s => s.PricePerDevice)
            .HasColumnName("price_per_device");

        builder.Property(s => s.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property("UpdatedDate")
            .HasColumnName("updated_date");

        builder.HasIndex(s => s.UserId)
            .IsUnique()
            .HasDatabaseName("uix_sp_subscriptions_user");

        builder.HasIndex(s => s.ProfileId)
            .IsUnique()
            .HasDatabaseName("uix_sp_subscriptions_profile");

        builder.HasIndex(s => s.StripeCustomerId)
            .HasDatabaseName("ix_sp_subscriptions_stripe_cust");

        builder.HasIndex(s => s.StripeSubscriptionId)
            .HasDatabaseName("ix_sp_subscriptions_stripe_sub");

        builder.HasIndex(s => s.Status)
            .HasDatabaseName("ix_sp_subscriptions_status");

        builder.HasMany(s => s.PaymentRecords)
            .WithOne()
            .HasForeignKey(p => p.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

