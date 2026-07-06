using Hampcoders.Electrolink.API.Monitoring.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Monitoring.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Monitoring.Interfaces.REST.Resources;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Monitoring.Interfaces.REST.Transform;

public static class UpdateTechnicalReportCommandFromResourceAssembler
{
    public static UpdateTechnicalReportCommand ToCommandFromResource(string executionId, string technicianId, UpdateTechnicalReportResource resource)
    {
        return new UpdateTechnicalReportCommand(
            ServiceExecutionId.From(executionId),
            TechnicianId.From(technicianId),
            resource.ReportContent,
            resource.Findings,
            resource.Recommendations,
            resource.IotFindings,
            resource.UpdatedAt
        );
    }
}

