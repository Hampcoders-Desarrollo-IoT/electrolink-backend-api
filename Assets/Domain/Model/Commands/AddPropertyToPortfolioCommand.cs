using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Assets.Domain.Model.Commands;

public record AddPropertyToPortfolioCommand(ClientIdentity Owner, PropertyId PropertyId, string Nickname, bool IsPrimary, string OccupancyStatus);