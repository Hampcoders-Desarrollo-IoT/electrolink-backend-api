using Hampcoders.Electrolink.API.Monitoring.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Monitoring.Domain.Model.Commands;

/// <summary>
/// Command to update the technical report with findings and recommendations.
/// </summary>
public record UpdateTechnicalReportCommand(
    ServiceExecutionId ExecutionId,
    TechnicianId TechnicianId,
    string ReportContent,
    string Findings,
    string Recommendations,
    string? IotFindings,
    DateTime UpdatedAt
);


