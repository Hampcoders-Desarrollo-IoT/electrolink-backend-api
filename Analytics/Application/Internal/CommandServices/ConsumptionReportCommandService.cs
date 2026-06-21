using Hampcoders.Electrolink.API.Analytics.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Analytics.Domain.Model.Enums;
using Hampcoders.Electrolink.API.Analytics.Domain.Repositories;
using Hampcoders.Electrolink.API.Analytics.Domain.Services;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using MediatR;

namespace Hampcoders.Electrolink.API.Analytics.Application.Internal.CommandServices;

public class ConsumptionReportCommandService(
    IConsumptionReportRepository reportRepository,
    IConsumptionDashboardRepository dashboardRepository,
    IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<ConsumptionReportCommandService> logger)
    : IConsumptionReportCommandService
{
    public async Task<string> RequestConsumptionReportAsync(
        string homeownerId,
        string propertyId,
        DateTime periodStart,
        DateTime periodEnd,
        string planTier,
        string exportFormat)
    {
        var plan = Enum.Parse<PlanTier>(planTier, ignoreCase: true);
        var format = Enum.Parse<ExportFormat>(exportFormat, ignoreCase: true);

        ValidatePeriodAgainstPlan(periodStart, plan);

        var report = ConsumptionReport.Request(
            ClientIdentity.FromOwnerId(homeownerId),
            PropertyId.From(propertyId),
            periodStart,
            periodEnd,
            format);

        await reportRepository.AddAsync(report);
        await unitOfWork.CompleteAsync();

        foreach (var domainEvent in report.DomainEvents)
            await mediator.Publish(domainEvent, CancellationToken.None);
        report.ClearDomainEvents();

        var downloadUrl = $"https://storage.electrolink.io/reports/{report.ReportId.Value}.{format.ToString().ToLower()}";
        report.MarkAsGenerated(downloadUrl);

        reportRepository.Update(report);
        await unitOfWork.CompleteAsync();

        foreach (var domainEvent in report.DomainEvents)
            await mediator.Publish(domainEvent, CancellationToken.None);
        report.ClearDomainEvents();

        logger.LogInformation("[Analytics BC] Report {ReportId} generated for homeowner {HomeownerId}",
            report.ReportId.Value, homeownerId);

        return report.ReportId.Value;
    }

    private static void ValidatePeriodAgainstPlan(DateTime periodStart, PlanTier planTier)
    {
        var maxHistoryMonths = planTier switch
        {
            PlanTier.Free => 1,
            PlanTier.PremiumIndividual => 12,
            PlanTier.EnterpriseBasic => 12,
            PlanTier.EnterprisePremium => 36,
            _ => 1
        };

        var earliestAllowed = DateTime.UtcNow.AddMonths(-maxHistoryMonths);
        if (periodStart < earliestAllowed)
            throw new InvalidOperationException(
                $"Your current plan only allows reports up to {maxHistoryMonths} month(s) back.");
    }
}
