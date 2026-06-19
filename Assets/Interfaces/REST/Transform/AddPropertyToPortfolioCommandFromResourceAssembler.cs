using Hampcoders.Electrolink.API.Assets.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Assets.Interfaces.REST.Resources;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Assets.Interfaces.REST.Transform;

public static class AddPropertyToPortfolioCommandFromResourceAssembler
{
    public static AddPropertyToPortfolioCommand ToCommandFromResource(AddPropertyToPortfolioResource resource, string homeownerId)
        => new AddPropertyToPortfolioCommand(
            ClientIdentity.FromHomeowner(homeownerId),
            PropertyId.From(resource.PropertyId),
            resource.Nickname,
            resource.IsPrimary,
            resource.OccupancyStatus);
}

