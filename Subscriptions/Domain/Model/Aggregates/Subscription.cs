using Hampcoders.Electrolink.API.Shared.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Events;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Services;

namespace Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Aggregates;

/// <summary>
/// Aggregate Root: Subscription
/// 
/// Manages the complete lifecycle of a user's subscription in the tactical design:
/// - BASIC tier (no billing, 2 requests/month for homeowners)
/// - PREMIUM tier (monthly or annual billing, unlimited requests)
/// 
/// States: ACTIVE, GRACE_PERIOD, CANCELLED_PENDING, DEGRADED
/// </summary>
public class Subscription : BaseAggregateRoot
{
    public SubscriptionId SubscriptionId { get; private set; }
    public UserId UserId { get; private set; }
    public ProfileId ProfileId { get; private set; }

    public BusinessRole BusinessRole { get; private set; }
    public PlanType PlanType { get; private set; }
    public BillingCycle? BillingCycle { get; private set; }

    public SubscriptionStatus Status { get; private set; }
    public bool CancelAtPeriodEnd { get; private set; }

    public StripeCustomerId StripeCustomerId { get; private set; }
    public StripeSubscriptionId? StripeSubscriptionId { get; private set; }

    public BillingPeriod? BillingPeriod { get; private set; }

    public DateTime? GracePeriodEndsAt { get; private set; }

    public UsageCounters? UsageCounters { get; private set; }

    // ── Enterprise IoT (solo COMPANY) ─────────────────────
    public string? InstallationServiceRequestId { get; private set; }
    public DateTime? InstallationDeadlineAt { get; private set; }
    public int? ActiveDeviceCount { get; private set; }
    public int? PricePerDevice { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    private readonly List<PaymentRecord> _paymentRecords = [];
    public IReadOnlyCollection<PaymentRecord> PaymentRecords => _paymentRecords.AsReadOnly();

    private Subscription() { }

    internal void AddPaymentRecord(PaymentRecord record)
    {
        _paymentRecords.Add(record);
    }

    public bool HasPaymentWithInvoice(string stripeInvoiceId)
        => _paymentRecords.Any(p => p.StripeInvoiceId.Value == stripeInvoiceId);

    public PaymentRecord? FindLastSuccessfulPayment()
        => _paymentRecords
            .Where(p => p.Status == PaymentStatus.Succeeded)
            .OrderByDescending(p => p.ProcessedAt)
            .FirstOrDefault();

    // ── Factory Method: Initialize ────────────────────────
    /// <summary>
    /// Factory method to initialize a new subscription for a user.
    /// Called by InitializeSubscriptionForNewUser policy after ProfileCompleted event.
    /// 
    /// State: BASIC, no Stripe subscription yet, usage counters for homeowners.
    /// </summary>
    public static Subscription Initialize(
        UserId userId,
        ProfileId profileId,
        BusinessRole businessRole,
        StripeCustomerId stripeCustomerId,
        IBillingPolicy billingPolicy)
    {
        var subscription = new Subscription
        {
            SubscriptionId = SubscriptionId.NewSubscriptionId(),
            UserId = userId,
            ProfileId = profileId,
            BusinessRole = businessRole,
            PlanType = PlanType.Basic,
            BillingCycle = null,
            Status = SubscriptionStatus.Active,
            CancelAtPeriodEnd = false,
            StripeCustomerId = stripeCustomerId,
            StripeSubscriptionId = null,
            BillingPeriod = null,
            GracePeriodEndsAt = null,
            UsageCounters = billingPolicy.InitializeCounters(),
            ActiveDeviceCount = billingPolicy.InitializeActiveDeviceCount(),
        };

        subscription.RaiseDomainEvent(new SubscriptionInitializedEvent(
            subscription.SubscriptionId.Value,
            subscription.UserId.Value,
            subscription.ProfileId.Value,
            subscription.BusinessRole.ToString(),
            subscription.PlanType.ToString(),
            subscription.StripeCustomerId.Value,
            DateTime.UtcNow));

        return subscription;
    }

    // ── Method: InitiateCheckout ──────────────────────────
    /// <summary>
    /// User initiates checkout for PREMIUM upgrade from BASIC.
    /// Validates state and emits CheckoutInitiatedEvent.
    /// Returns the session ID for Stripe redirection.
    /// </summary>
    public StripeCheckoutSessionId InitiateCheckout(
        BillingCycle billingCycle,
        StripeCheckoutSessionId sessionId)
    {
        if (!PlanType.IsBasic)
            throw new InvalidOperationException("User is already on a Premium plan.");
        
        if (Status.IsInGracePeriod)
            throw new InvalidOperationException("Cannot initiate checkout while in grace period.");

        RaiseDomainEvent(new CheckoutInitiatedEvent(
            SubscriptionId.Value,
            UserId.Value,
            BusinessRole.ToString(),
            billingCycle.ToString(),
            sessionId.Value,
            DateTime.UtcNow));

        return sessionId;
    }

    // ── Method: Activate ─────────────────────────────────
    /// <summary>
    /// Webhook: invoice.payment_succeeded (billing_reason: subscription_create)
    /// Transitions from BASIC to PREMIUM after successful payment.
    /// Idempotent: ignores if already activated with same stripeSubscriptionId.
    /// </summary>
    public void Activate(
        StripeSubscriptionId stripeSubscriptionId,
        BillingCycle billingCycle,
        BillingPeriod billingPeriod)
    {
        // Idempotency check
        if (PlanType.IsPremium && StripeSubscriptionId == stripeSubscriptionId)
            return;

        if (!PlanType.IsBasic)
            throw new InvalidOperationException("Subscription is not in BASIC plan to activate.");

        PlanType = PlanType.Premium;
        BillingCycle = billingCycle;
        StripeSubscriptionId = stripeSubscriptionId;
        BillingPeriod = billingPeriod;
        Status = SubscriptionStatus.Active;
        UsageCounters = null;  // No usage limits in PREMIUM

        RaiseDomainEvent(new SubscriptionActivatedEvent(
            SubscriptionId.Value,
            UserId.Value,
            BusinessRole.ToString(),
            PlanType.ToString(),
            billingCycle.ToString(),
            stripeSubscriptionId.Value,
            billingPeriod.PeriodStart,
            billingPeriod.PeriodEnd,
            DateTime.UtcNow));
    }

    // ── Method: RecordRenewal ────────────────────────────
    /// <summary>
    /// Webhook: invoice.payment_succeeded (billing_reason: subscription_cycle)
    /// Records successful renewal and recovers from grace period if applicable.
    /// </summary>
    public void RecordRenewal(BillingPeriod newPeriod, StripeInvoiceId invoiceId)
    {
        bool wasInGracePeriod = Status.IsInGracePeriod;

        BillingPeriod = newPeriod;
        Status = SubscriptionStatus.Active;
        GracePeriodEndsAt = null;

        RaiseDomainEvent(new PaymentProcessedEvent(
            SubscriptionId.Value,
            UserId.Value,
            invoiceId.Value,
            newPeriod.PeriodEnd,
            wasInGracePeriod,
            DateTime.UtcNow));
    }

    // ── Method: StartGracePeriod ────────────────────────
    /// <summary>
    /// Webhook: invoice.payment_failed
    /// Initiates 7-day grace period for PREMIUM subscriptions.
    /// Validates state: only PREMIUM ACTIVE subscriptions can enter grace period.
    /// </summary>
    public void StartGracePeriod(StripeInvoiceId invoiceId, DateTime failedAt)
    {
        if (!PlanType.IsPremium || !Status.IsActive)
            throw new InvalidOperationException(
                "Grace period only applies to active Premium subscriptions.");

        // Idempotency: if already in grace period, ignore retry
        if (Status.IsInGracePeriod) return;

        var gracePeriodEndsAt = failedAt.AddDays(7);
        Status = SubscriptionStatus.GracePeriod;
        GracePeriodEndsAt = gracePeriodEndsAt;

        RaiseDomainEvent(new GracePeriodStartedEvent(
            SubscriptionId.Value,
            UserId.Value,
            BusinessRole.ToString(),
            PlanType.ToString(),
            invoiceId.Value,
            gracePeriodEndsAt,
            failedAt));

        RaiseDomainEvent(new PaymentFailedEvent(
            SubscriptionId.Value,
            UserId.Value,
            ProfileId.Value,
            invoiceId.Value,
            failedAt));
    }

    // ── Method: Degrade ──────────────────────────────────
    /// <summary>
    /// Webhook: customer.subscription.deleted  /  Scheduled Job (grace period expiration)
    /// Downgrade from PREMIUM to BASIC.
    /// Valid transitions: GRACE_PERIOD → ACTIVE (BASIC) or CANCELLED_PENDING → ACTIVE (BASIC)
    /// </summary>
    public void Degrade(DegradationReason reason, DateTime degradedAt)
    {
        if (!Status.IsInGracePeriod && !Status.IsCancelledPending)
            throw new InvalidOperationException(
                "Subscription cannot be degraded from its current status.");

        PlanType = PlanType.Basic;
        Status = SubscriptionStatus.Active;
        StripeSubscriptionId = null;
        BillingCycle = null;
        BillingPeriod = null;
        GracePeriodEndsAt = null;
        CancelAtPeriodEnd = false;

        // Restore usage counters for HOMEOWNER
        if (BusinessRole.Value == EBusinessRole.Homeowner)
            UsageCounters = Hampcoders.Electrolink.API.Subscriptions.Domain.Model.ValueObjects.UsageCounters.Initial();

        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new SubscriptionDegradedEvent(
            SubscriptionId.Value,
            UserId.Value,
            BusinessRole.ToString(),
            "PREMIUM",
            PlanType.ToString(),
            reason.ToString(),
            degradedAt));
    }

    // ── Method: ScheduleCancellation ─────────────────────
    /// <summary>
    /// User voluntarily cancels PREMIUM subscription.
    /// Marks cancel_at_period_end = true in Stripe.
    /// Subscription remains ACTIVE until period end, then degrades to BASIC.
    /// </summary>
    public void ScheduleCancellation(string cancellationReason, DateTime scheduledAt)
    {
        if (!PlanType.IsPremium)
            throw new InvalidOperationException("Only Premium subscriptions can be cancelled.");
        
        if (CancelAtPeriodEnd)
            throw new InvalidOperationException("Cancellation already scheduled.");

        CancelAtPeriodEnd = true;
        Status = SubscriptionStatus.CancelledPending;

        RaiseDomainEvent(new SubscriptionCancellationScheduledEvent(
            SubscriptionId.Value,
            UserId.Value,
            BusinessRole.ToString(),
            PlanType.ToString(),
            BillingPeriod?.PeriodEnd ?? throw new InvalidOperationException("BillingPeriod is required for a Premium subscription."),
            cancellationReason,
            scheduledAt));
    }

    // ── Method: IncrementRequestCounter ──────────────────
    /// <summary>
    /// Increment monthly request counter for BASIC HOMEOWNER subscriptions.
    /// Called by ServiceRequestCreatedEventHandler when Planning BC publishes ServiceRequestCreated.
    /// Emits MonthlyRequestLimitReachedEvent when limit is hit.
    /// </summary>
    public void IncrementRequestCounter()
    {
        if (!PlanType.IsBasic)
            throw new InvalidOperationException("Cannot increment request counter for a non-Basic subscription.");
        if (BusinessRole.Value != EBusinessRole.Homeowner)
            throw new InvalidOperationException("Cannot increment request counter for a non-Homeowner subscription.");

        if (UsageCounters is null)
            throw new InvalidOperationException("UsageCounters not initialized for this subscription.");

        UsageCounters = UsageCounters.Increment();

        if (!UsageCounters.HasCapacity)
        {
            RaiseDomainEvent(new MonthlyRequestLimitReachedEvent(
                SubscriptionId.Value,
                UserId.Value,
                UsageCounters.MonthlyRequestsUsed,
                UsageCounters.MonthlyRequestsLimit,
                DateTime.UtcNow));
        }
    }

    // ── Method: ResetMonthlyCounters ─────────────────────
    /// <summary>
    /// Reset monthly request counters on day 1 of each month (00:00 UTC).
    /// Called by MonthlyCounterResetJob scheduled background service.
    /// </summary>
    public void ResetMonthlyCounters()
    {
        if (PlanType.IsBasic && BusinessRole.Value == EBusinessRole.Homeowner && UsageCounters is not null)
            UsageCounters = UsageCounters.Reset();
    }

    // ── Method: UpdateBillingCycle ──────────────────────
    /// <summary>
    /// Webhook: customer.subscription.updated
    /// User changes billing cycle (Monthly ↔ Annual) from Stripe Customer Portal.
    /// </summary>
    public void UpdateBillingCycle(BillingCycle newCycle, BillingPeriod newPeriod)
    {
        var previousCycle = BillingCycle!;
        BillingCycle = newCycle;
        BillingPeriod = newPeriod;

        RaiseDomainEvent(new SubscriptionBillingCycleChangedEvent(
            SubscriptionId.Value,
            UserId.Value,
            previousCycle.ToString(),
            newCycle.ToString(),
            newPeriod.PeriodEnd,
            DateTime.UtcNow));
    }

    // ── Method: ActivateEnterprisePendingInstallation ──
    /// <summary>
    /// Webhook: checkout.session.completed (Enterprise)
    /// Transitions to PENDING_INSTALLATION state after enterprise payment.
    /// </summary>
    public void ActivateEnterprisePendingInstallation(
        StripeSubscriptionId stripeSubscriptionId,
        BillingPeriod billingPeriod,
        int initialDeviceCount,
        int pricePerDevice,
        PlanType planType)
    {
        if (!PlanType.IsBasic)
            throw new InvalidOperationException("Enterprise subscription can only be activated from BASIC plan.");

        PlanType = planType;
        StripeSubscriptionId = stripeSubscriptionId;
        BillingPeriod = billingPeriod;
        Status = SubscriptionStatus.PendingInstallation;
        ActiveDeviceCount = initialDeviceCount;
        PricePerDevice = pricePerDevice;
        InstallationDeadlineAt = billingPeriod.PeriodStart.AddDays(30);

        RaiseDomainEvent(new EnterpriseSubscriptionPendingInstallationEvent(
            SubscriptionId.Value,
            UserId.Value,
            stripeSubscriptionId.Value,
            initialDeviceCount,
            pricePerDevice,
            InstallationDeadlineAt.Value,
            DateTime.UtcNow));

        RaiseDomainEvent(new IoTInstallationRequiredEvent(
            SubscriptionId.Value,
            UserId.Value,
            initialDeviceCount,
            InstallationDeadlineAt.Value,
            DateTime.UtcNow));
    }

    // ── Method: CancelWithRefund ────────────────────────
    /// <summary>
    /// Enterprise activation policy: if installation doesn't occur in 30 days.
    /// Cancels subscription and marks as CANCELLED_REFUNDED.
    /// </summary>
    public void CancelWithRefund(string refundId, string reason)
    {
        if (!PlanType.IsAnyEnterprise)
            throw new InvalidOperationException("Only Enterprise subscriptions can be cancelled with refund.");

        Status = SubscriptionStatus.CancelledRefunded;
        StripeSubscriptionId = null;
        ActiveDeviceCount = null;
        PricePerDevice = null;
        InstallationDeadlineAt = null;

        RaiseDomainEvent(new EnterpriseSubscriptionCancelledRefundedEvent(
            SubscriptionId.Value,
            UserId.Value,
            refundId,
            reason,
            DateTime.UtcNow));
    }

    // ── Method: UpdateActiveDeviceCount ──────────────────
    public void UpdateActiveDeviceCount(int newCount, string source)
    {
        if (BusinessRole.Value != EBusinessRole.Company)
            throw new InvalidOperationException("Device count is only applicable to COMPANY subscriptions.");

        var previous = ActiveDeviceCount ?? 0;
        ActiveDeviceCount = newCount;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new ActiveDeviceCountUpdatedEvent(
            SubscriptionId.Value,
            UserId.Value,
            previous,
            newCount,
            PricePerDevice ?? 0,
            (newCount * (PricePerDevice ?? 0)),
            DateTime.UtcNow));
    }

    // ── Method: SetInstallationServiceRequestId ──────────
    public void SetInstallationServiceRequestId(string serviceRequestId)
    {
        InstallationServiceRequestId = serviceRequestId;
    }

    // ── Method: ActivateEnterpriseFullyActive ────────────
    public void ActivateEnterpriseFullyActive(int confirmedDeviceCount, string installationServiceRequestId)
    {
        if (Status != SubscriptionStatus.PendingInstallation)
            throw new InvalidOperationException("Enterprise subscription must be in PENDING_INSTALLATION to activate.");

        Status = SubscriptionStatus.Active;
        ActiveDeviceCount = confirmedDeviceCount;
        InstallationServiceRequestId = installationServiceRequestId;
        InstallationDeadlineAt = null;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new EnterpriseSubscriptionFullyActiveEvent(
            SubscriptionId.Value,
            UserId.Value,
            PlanType.ToString(),
            confirmedDeviceCount,
            PricePerDevice ?? 0,
            DateTime.UtcNow));
    }
}
