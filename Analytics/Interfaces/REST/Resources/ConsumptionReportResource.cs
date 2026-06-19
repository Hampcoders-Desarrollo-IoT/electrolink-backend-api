namespace Hampcoders.Electrolink.API.Analytics.Interfaces.REST.Resources;

public record ConsumptionReportResource(
    string ReportId,
    string OwnerId,
    string PropertyId,
    DateTime PeriodStart,
    DateTime PeriodEnd,
    DateTime GeneratedAt,
    string ExportFormat,
    string? DownloadUrl,
    DateTime? ExpiresAt,
    bool IsGenerated);
