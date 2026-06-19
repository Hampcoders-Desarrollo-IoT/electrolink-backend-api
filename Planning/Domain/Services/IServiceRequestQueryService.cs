using Hampcoders.Electrolink.API.Planning.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Queries;

namespace Hampcoders.Electrolink.API.Planning.Domain.Services;

public interface IServiceRequestQueryService
{
    Task<ServiceRequest?> Handle(GetServiceRequestByIdQuery query);
    Task<IEnumerable<ServiceRequest>> Handle(GetAllRequestsByClientQuery query);
    Task<IEnumerable<ServiceRequest>> Handle(GetRequestsByClientAndStatusQuery query);
    Task<IEnumerable<ServiceRequest>> Handle(GetPendingAssignmentRequestsQuery query);
}

