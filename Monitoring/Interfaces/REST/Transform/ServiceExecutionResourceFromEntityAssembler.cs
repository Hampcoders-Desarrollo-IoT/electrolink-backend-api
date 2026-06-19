using Hampcoders.Electrolink.API.Monitoring.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Monitoring.Interfaces.REST.Resources;

namespace Hampcoders.Electrolink.API.Monitoring.Interfaces.REST.Transform;

public static class ServiceExecutionResourceFromEntityAssembler
{
    public static ServiceExecutionResource ToResourceFromEntity(ServiceExecution execution) =>
        new(
            execution.Id.Value,
            execution.AssignmentId.Value,
            execution.TechnicianId.Value,
            execution.Owner.ClientId,
            execution.PropertyId.Value,
            execution.Status.ToString(),
            execution.ScheduledDateTime,
            execution.StartedAt,
            execution.CompletedAt,
            execution.RecipeSnapshot.ServiceName,
            execution.RecipeSnapshot.ServiceCategory.ToString(),
            execution.RecipeSnapshot.Pricing.TotalPrice.Amount,
            execution.RecipeSnapshot.EstimatedDuration.TotalMinutes,
            execution.IsPriority,
            new WorkLogSummaryResource(
                execution.WorkPhotos.Count,
                !string.IsNullOrWhiteSpace(execution.TechnicalReportContent),
                execution.ComponentSubstitutions.Count > 0));

    public static WorkLogResource ToWorkLogResourceFromEntity(ServiceExecution execution) =>
        new(
            execution.Id.Value,
            execution.WorkPhotos.Select(p => new PhotoResource(
                p.Id.Value, p.PhotoType.ToString(), p.PhotoUrl, p.ThumbnailUrl, p.Format, p.SizeBytes, p.TakenAt, p.Notes)).ToList(),
            execution.TechnicalReportContent,
            execution.TechnicalReportFindings,
            execution.TechnicalReportRecommendations,
            execution.TechnicalReportVersion,
            execution.ComponentSubstitutions.Select(c => new ComponentUsageResource(
                c.ComponentTypeId,
                c.QuantityUsed, c.QuantityReserved, c.Delta)).ToList());
}