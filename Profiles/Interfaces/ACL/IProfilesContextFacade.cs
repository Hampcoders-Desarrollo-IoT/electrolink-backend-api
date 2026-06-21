namespace Hampcoders.Electrolink.API.Profiles.Interfaces.ACL;


/// <summary>
/// Facade for the profiles context
/// </summary>
public interface IProfilesContextFacade
{
    /// <summary>
    /// Gets technician ID (Guid as string) by profile ID.
    /// </summary>
    /// <returns>Technician GUID as string, or null if not found</returns>
    Task<string?> GetTechnicianIdByUserIdAsync(string userId);
    
    /// <summary>
    /// Gets technician information (TechnicianId and UserId).
    /// </summary>
    /// <returns>Tuple with (technicianId as string, userId as int), or null if not found</returns>
    Task<(string technicianId, string userId)?> GetTechnicianInfoByProfileIdAsync(string userId);
    
    /// <summary>
    /// Checks if a technician profile exists for a given user ID.
    /// </summary>
    Task<bool> ExistsTechnicianProfileByUserIdAsync(string userId);

    /// <summary>
    /// Gets profile full name by profile ID.
    /// </summary>
    /// <returns>Full name or null if not found</returns>
    Task<string?> GetProfileFullNameAsync(string profileId);

    /// <summary>
    /// Gets profile phone by profile ID.
    /// </summary>
    /// <returns>Phone or null if not found</returns>
    Task<string?> GetProfilePhoneAsync(string profileId);

    /// <summary>
    /// Gets profile role by profile ID.
    /// </summary>
    /// <returns>Role as string ("HomeOwner" or "Technician") or null if not found</returns>
    Task<string?> GetProfileRoleAsync(string profileId);

    /// <summary>
    /// Checks if a profile exists for a given profile ID.
    /// </summary>
    Task<bool> ProfileExistsAsync(string profileId);

    /// <summary>
    /// Checks if a homeowner profile is active.
    /// </summary>
    Task<bool> IsHomeownerActiveAsync(string homeownerId);

    /// <summary>
    /// Checks if a company profile is active.
    /// </summary>
    Task<bool> IsCompanyActiveAsync(string companyId);

    /// <summary>
    /// Gets all technicians whose service area contains the given coordinates.
    /// Returns a list of tuples with (technicianId, profileId, fullName, rating).
    /// Specialties are excluded to keep primitives — query separately if needed.
    /// </summary>
    Task<IEnumerable<(string technicianId, string profileId, string fullName)>> GetTechniciansInAreaAsync(double latitude, double longitude);
    
    /// <summary>
    /// Gets detailed technician information including service area coordinates, experience, and specialties.
    /// </summary>
    Task<(string technicianId, double serviceAreaLat, double serviceAreaLon, int experienceYears, IEnumerable<string> specialties)> GetTechnicianDetailsAsync(string technicianId);
    
    /// <summary>
    /// Gets specialties for a given technician.
    /// </summary>
    Task<IEnumerable<string>> GetTechnicianSpecialtiesAsync(string technicianId);
    
    /// <summary>
    /// Gets profile claims (full name, role, and profile ID) by user ID.
    /// </summary>
    Task<(string ProfileId, string ProfileStatus, string? BusinessRole, string? RoleSubjectId, string? SubscriptionTier)?> GetProfileClaimsAsync(string userId);

    /// <summary>
    /// Resolves the ProfileId from a HomeownerId (ho-xxx).
    /// </summary>
    Task<string?> GetProfileIdByHomeownerIdAsync(string homeownerId);

    /// <summary>
    /// Resolves the ProfileId from a CompanyId (comp-xxx).
    /// </summary>
    Task<string?> GetProfileIdByCompanyIdAsync(string companyId);

    /// <summary>
    /// Sets consumption thresholds for a company profile.
    /// </summary>
    Task SetConsumptionThresholdsAsync(string profileId, Dictionary<string, decimal> thresholds);
}


