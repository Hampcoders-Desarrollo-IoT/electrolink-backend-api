using Hampcoders.Electrolink.API.Analytics.Domain.Model.Events;
using Hampcoders.Electrolink.API.Analytics.Domain.Model.Enums;
using Hampcoders.Electrolink.API.Analytics.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Analytics.Domain.Model.Aggregates;

public class ConsumptionReport : BaseAggregateRoot
{
    public ConsumptionReportId ReportId { get; private set; }
    public ClientIdentity RequestedBy { get; private set; }
    public HomeownerId RequestedByHomeownerId => RequestedBy.ToHomeownerId();
    public PropertyId PropertyId { get; private set; }
    public DateTime PeriodStart { get; private set; }
    public DateTime PeriodEnd { get; private set; }
    public DateTime GeneratedAt { get; private set; }
    public ExportFormat ExportFormat { get; private set; }
    public string? DownloadUrl { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public bool IsGenerated { get; private set; }

    private ConsumptionReport() { }

    public static ConsumptionReport Request(
        ClientIdentity requestedBy,
        PropertyId propertyId,
        DateTime periodStart,
        DateTime periodEnd,
        ExportFormat exportFormat)
    {
        if (periodEnd <= periodStart)
            throw new ArgumentException("PeriodEnd must be after PeriodStart.");

        var report = new ConsumptionReport
        {
            ReportId = ConsumptionReportId.New(),
            RequestedBy = requestedBy ?? throw new ArgumentNullException(nameof(requestedBy)),
            PropertyId = propertyId ?? throw new ArgumentNullException(nameof(propertyId)),
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            GeneratedAt = DateTime.UtcNow,
            ExportFormat = exportFormat,
            IsGenerated = false
        };

        report.RaiseDomainEvent(new ReportRequested(
            report.ReportId.Value,
            report.RequestedBy.ClientId,
            propertyId.Value,
            DateTime.UtcNow));

        return report;
    }

    public void MarkAsGenerated(string downloadUrl)
    {
        if (IsGenerated)
            throw new InvalidOperationException("Report has already been generated and is immutable.");
        if (string.IsNullOrWhiteSpace(downloadUrl))
            throw new ArgumentException("DownloadUrl cannot be empty.");

        DownloadUrl = downloadUrl;
        ExpiresAt = DateTime.UtcNow.AddHours(72);
        IsGenerated = true;

        RaiseDomainEvent(new ReportGenerated(
            ReportId.Value,
            RequestedBy.ClientId,
            downloadUrl,
            ExpiresAt.Value,
            DateTime.UtcNow));
    }
}
