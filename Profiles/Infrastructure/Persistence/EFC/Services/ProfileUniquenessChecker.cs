using Hampcoders.Electrolink.API.Profiles.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.Exceptions;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Profiles.Domain.Services;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Hampcoders.Electrolink.API.Profiles.Infrastructure.Persistence.EFC.Services;

public class ProfileUniquenessChecker(AppDbContext context) : IProfileUniquenessChecker
{
    public async Task EnsureDniIsUniqueAsync(Dni dni, ProfileId excludedProfileId)
    {
        var exists = await context.Set<Profile>()
            .AnyAsync(p => p.PersonalData != null
                      && p.PersonalData.Dni.Value == dni.Value
                      && p.ProfileId != excludedProfileId);

        if (exists)
            throw new DniAlreadyInUseException(dni);
    }

    public async Task EnsureTaxIdIsUniqueAsync(TaxId taxId, ProfileId excludedProfileId)
    {
        var exists = await context.Set<Profile>()
            .AnyAsync(p => p.Company != null
                      && p.Company.CompanyData.TaxId.Value == taxId.Value
                      && p.ProfileId != excludedProfileId);

        if (exists)
            throw new TaxIdAlreadyInUseException(taxId);
    }
}