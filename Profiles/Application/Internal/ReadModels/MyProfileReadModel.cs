namespace Hampcoders.Electrolink.API.Profiles.Application.Internal.ReadModels;

public sealed record MyProfileReadModel(
    string ProfileId,
    string UserId,
    string? BusinessRole,
    string Status,
    PersonalDataReadModel? PersonalData,
    TechnicianReadModel? Technician,
    HomeownerReadModel? Homeowner,
    CompanyReadModel? Company
);