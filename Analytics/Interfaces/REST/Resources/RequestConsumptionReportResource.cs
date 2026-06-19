namespace Hampcoders.Electrolink.API.Analytics.Interfaces.REST.Resources;

public record RequestConsumptionReportResource(
    string OwnerId,
    string PropertyId,
    DateTime PeriodStart,
    DateTime PeriodEnd,
    string ExportFormat);
