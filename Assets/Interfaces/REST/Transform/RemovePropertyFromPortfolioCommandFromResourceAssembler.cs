using Hampcoders.Electrolink.API.Assets.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Assets.Interfaces.REST.Transform;

public static class RemovePropertyFromPortfolioCommandFromResourceAssembler
{
    public static RemovePropertyFromPortfolioCommand ToCommandFromResource(string ownerId, string propertyId, string reason)
        => new RemovePropertyFromPortfolioCommand(
            ClientIdentity.FromOwnerId(ownerId),
            PropertyId.From(propertyId),
            reason);
}

