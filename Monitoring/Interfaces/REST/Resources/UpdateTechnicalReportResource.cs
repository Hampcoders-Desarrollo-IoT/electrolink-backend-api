namespace Hampcoders.Electrolink.API.Monitoring.Interfaces.REST.Resources;

public record UpdateTechnicalReportResource(
    string ReportContent,
    string Findings,
    string Recommendations,
    DateTime UpdatedAt,
    string? IotFindings = null
);