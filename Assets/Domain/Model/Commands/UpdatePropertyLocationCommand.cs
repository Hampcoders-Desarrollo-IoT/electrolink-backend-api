using Hampcoders.Electrolink.API.Assets.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Assets.Domain.Model.Commands;

public record UpdatePropertyLocationCommand(
    PropertyId PropertyId,
    Address? Address,
    Geolocation? Geolocation);
