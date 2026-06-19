using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using AssignmentId = Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects.AssignmentId;

namespace Hampcoders.Electrolink.API.Planning.Domain.Model.Queries;

// ServiceAssignment Queries
public record GetServiceAssignmentByIdQuery(AssignmentId AssignmentId);

public record GetAssignmentsByTechnicianQuery(TechnicianId TechnicianId);

public record GetAssignmentsByClientQuery(ClientIdentity Client);

public record GetAssignmentByRequestIdQuery(RequestId RequestId);

