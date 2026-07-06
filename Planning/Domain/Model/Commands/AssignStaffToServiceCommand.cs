using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Planning.Domain.Model.Commands;

public record AssignStaffToServiceCommand(RequestId RequestId, string StaffMemberId);
