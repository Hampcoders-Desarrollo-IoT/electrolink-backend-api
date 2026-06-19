using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Assets.Domain.Model.Commands;

public record RegisterPropertyPhotoCommand(
    ClientIdentity Owner,
    PropertyId PropertyId,
    string ProviderId,
    string PublicUrl);
