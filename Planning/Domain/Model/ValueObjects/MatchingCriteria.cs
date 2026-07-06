using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;

public record MatchingCriteria
{
    public Geolocation PropertyGeolocation { get; }
    public IReadOnlyList<string> RequiredComponentTypes { get; }
    public bool IsPriority { get; }
    public bool RequiresIoTCertifiedTechnician { get; }
    public TechnicianId? PreferredTechnicianId { get; }

    private MatchingCriteria(
        Geolocation geolocation,
        IReadOnlyList<string> componentTypeIds,
        bool isPriority,
        bool requiresIoTCertifiedTechnician,
        TechnicianId? preferredTechnicianId)
    {
        PropertyGeolocation = geolocation ?? throw new ArgumentNullException(nameof(geolocation));
        RequiredComponentTypes = componentTypeIds ?? throw new ArgumentNullException(nameof(componentTypeIds));
        IsPriority = isPriority;
        RequiresIoTCertifiedTechnician = requiresIoTCertifiedTechnician;
        PreferredTechnicianId = preferredTechnicianId;
    }

    public static MatchingCriteria Create(
        Geolocation geolocation,
        IReadOnlyList<string> componentTypeIds,
        bool isPriority,
        bool requiresIoTCertifiedTechnician = false,
        TechnicianId? preferredTechnicianId = null)
        => new(geolocation, componentTypeIds, isPriority, requiresIoTCertifiedTechnician, preferredTechnicianId);

    public override string ToString() 
        => $"Location: ({PropertyGeolocation.Latitude}, {PropertyGeolocation.Longitude}), Components: {RequiredComponentTypes.Count}, Priority: {IsPriority}, IoT: {RequiresIoTCertifiedTechnician}";
}

