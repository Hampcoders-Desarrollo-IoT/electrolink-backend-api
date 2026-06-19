using Hampcoders.Electrolink.API.Assets.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Assets.Interfaces.REST.Transform;

public static class RemovePropertyFromPortfolioCommandFromResourceAssembler
{
    public static RemovePropertyFromPortfolioCommand ToCommandFromResource(string homeownerId, string propertyId, string reason)
        => new RemovePropertyFromPortfolioCommand(
            ClientIdentity.FromHomeowner(homeownerId),
            PropertyId.From(propertyId),
            reason);
}

