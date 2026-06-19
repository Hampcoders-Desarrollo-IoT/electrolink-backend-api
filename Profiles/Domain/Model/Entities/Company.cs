using Hampcoders.Electrolink.API.Profiles.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Profiles.Domain.Model.Entities;

public class Company
{
    public CompanyId CompanyId { get; private set; } = null!;
    public ProfileId ProfileId { get; private set; } = null!;
    public CompanyData CompanyData { get; private set; } = null!;

    private Company() { }

    public static Company Create(CompanyId id, ProfileId profileId, CompanyData companyData)
    {
        return new Company
        {
            CompanyId = id,
            ProfileId = profileId,
            CompanyData = companyData,
        };
    }

    public void UpdateCompanyData(CompanyData data)
    {
        CompanyData = data;
    }
}
