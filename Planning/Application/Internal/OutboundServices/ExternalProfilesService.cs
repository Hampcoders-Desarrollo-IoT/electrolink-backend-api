using Hampcoders.Electrolink.API.Planning.Domain.Model.Exceptions;
using Hampcoders.Electrolink.API.Profiles.Interfaces.ACL;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Planning.Application.Internal.OutboundServices;

/// <summary>
/// Service responsible for interacting with the external Profiles service to retrieve technician and homeowner information.
/// </summary>
public class ExternalProfilesService(IProfilesContextFacade profilesContextFacade)
{
    /// <summary>
    /// Gets a list of technicians in a specific area based on latitude and longitude.
    /// </summary>
    /// <param name="latitude">The latitude of the area to search for technicians.</param>
    /// <param name="longitude">The longitude of the area to search for technicians.</param>
    /// <returns>A list of tuples containing technician ID, profile ID, full name, and rating.</returns>
    public async Task<IEnumerable<(string technicianId, string profileId, string fullName)>>
        GetTechniciansInAreaAsync(double latitude, double longitude)
        => await profilesContextFacade.GetTechniciansInAreaAsync(latitude, longitude);

    public async Task<(string technicianId, double serviceAreaLat, double serviceAreaLon, int experienceYears, IEnumerable<string> specialties)>
        GetTechnicianDetailsAsync(string technicianId)
        => await profilesContextFacade.GetTechnicianDetailsAsync(technicianId);

    public async Task<string?> GetTechnicianIdByUserIdAsync(string userId)
        => await profilesContextFacade.GetTechnicianIdByUserIdAsync(userId);

    public async Task<bool> IsClientActiveAsync(ClientIdentity client)
        => client.ClientType switch
        {
            EClientType.Homeowner => await profilesContextFacade.IsHomeownerActiveAsync(client.ToHomeownerId().Value),
            EClientType.Company => await profilesContextFacade.IsCompanyActiveAsync(client.ToCompanyId().Value),
            _ => throw new ArgumentOutOfRangeException(nameof(client.ClientType))
        };

    public async Task EnsureClientIsActiveAsync(ClientIdentity client)
    {
        var isActive = await IsClientActiveAsync(client);
        if (!isActive)
            throw new InactiveHomeownerException(client.ToString());
    }

    public async Task<bool> IsHomeownerActiveAsync(string homeownerId)
        => await profilesContextFacade.IsHomeownerActiveAsync(homeownerId);
    
    public async Task EnsureHomeownerIsActiveAsync(string homeownerId)
    {
        var isActive = await profilesContextFacade.IsHomeownerActiveAsync(homeownerId);
        if (!isActive)
            throw new InactiveHomeownerException(homeownerId);
    }
    
}