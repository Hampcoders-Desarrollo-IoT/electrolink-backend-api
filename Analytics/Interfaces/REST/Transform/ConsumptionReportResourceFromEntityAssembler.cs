using Hampcoders.Electrolink.API.Analytics.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Analytics.Interfaces.REST.Resources;

namespace Hampcoders.Electrolink.API.Analytics.Interfaces.REST.Transform;

public static class ConsumptionReportResourceFromEntityAssembler
{
    public static ConsumptionReportResource ToResourceFromEntity(ConsumptionReport report)
    {
        return new ConsumptionReportResource(
            report.ReportId.Value,
            report.RequestedBy.ClientId,
            report.PropertyId.Value,
            report.PeriodStart,
            report.PeriodEnd,
            report.GeneratedAt,
            report.ExportFormat.ToString(),
            report.DownloadUrl,
            report.ExpiresAt,
            report.IsGenerated);
    }
}
