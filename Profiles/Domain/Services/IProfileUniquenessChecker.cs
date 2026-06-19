using Hampcoders.Electrolink.API.Profiles.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Profiles.Domain.Services;

public interface IProfileUniquenessChecker
{
    Task EnsureDniIsUniqueAsync(Dni dni, ProfileId excludedProfileId);
    Task EnsureTaxIdIsUniqueAsync(TaxId taxId, ProfileId excludedProfileId);
}