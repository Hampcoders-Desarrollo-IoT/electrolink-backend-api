using Hampcoders.Electrolink.API.Planning.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Queries;

namespace Hampcoders.Electrolink.API.Planning.Domain.Services;

public interface IServiceAssignmentQueryService
{
    Task<ServiceAssignment?> Handle(GetServiceAssignmentByIdQuery query);
    Task<IEnumerable<ServiceAssignment>> Handle(GetAssignmentsByTechnicianQuery query);
    Task<IEnumerable<ServiceAssignment>> Handle(GetAssignmentsByClientQuery query);
    Task<ServiceAssignment?> Handle(GetAssignmentByRequestIdQuery query);
}

