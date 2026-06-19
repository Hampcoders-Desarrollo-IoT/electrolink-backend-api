using Hampcoders.Electrolink.API.Profiles.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Profiles.Domain.Model.Events;

public sealed class CompanyDataUpdatedEvent : IEvent
{
    public Guid EventId { get; }
    public ProfileId ProfileId { get; }
    public CompanyId CompanyId { get; }
    public string CompanyName { get; }
    public string? Industry { get; }
    public DateTime OccurredOn { get; }

    public CompanyDataUpdatedEvent(
        ProfileId profileId,
        CompanyId companyId,
        string companyName,
        string? industry)
    {
        EventId = Guid.NewGuid();
        ProfileId = profileId;
        CompanyId = companyId;
        CompanyName = companyName;
        Industry = industry;
        OccurredOn = DateTime.UtcNow;
    }
}
