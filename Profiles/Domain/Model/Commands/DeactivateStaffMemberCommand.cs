namespace Hampcoders.Electrolink.API.Profiles.Domain.Model.Commands;

public record DeactivateStaffMemberCommand(string StaffMemberId, bool Reactivate = false);
