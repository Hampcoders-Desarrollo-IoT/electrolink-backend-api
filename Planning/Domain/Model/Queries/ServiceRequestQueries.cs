using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Planning.Domain.Model.Queries;

// ServiceRequest Queries
public record GetServiceRequestByIdQuery(RequestId RequestId);
public record GetServiceRequestByClientAndIdQuery(ClientIdentity Client, RequestId RequestId);

public record GetAllRequestsByClientQuery(ClientIdentity Client);

public record GetRequestsByClientAndStatusQuery(ClientIdentity Client, string Status);

public record GetPendingAssignmentRequestsQuery(); // Para admin/sistema

