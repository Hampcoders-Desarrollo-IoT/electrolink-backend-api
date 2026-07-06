namespace Hampcoders.Electrolink.API.Profiles.Domain.Model.Commands;

public record AssignStaffZoneCommand(
    string StaffMemberId,
    string Region,
    double CenterLatitude,
    double CenterLongitude,
    double RadiusKm
);
