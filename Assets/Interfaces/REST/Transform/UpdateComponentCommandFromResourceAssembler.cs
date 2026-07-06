using Hampcoders.Electrolink.API.Assets.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Assets.Interfaces.REST.Resources;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Assets.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Assets.Interfaces.REST.Transform;

public static class UpdateComponentCommandFromResourceAssembler
{
    public static UpdateComponentCommand ToCommandFromResource(UpdateComponentResource resource, string componentId)
    {
        if (resource is null) throw new ArgumentNullException(nameof(resource));

        return new UpdateComponentCommand(
            ComponentId.From(componentId),
            resource.Name,
            resource.Description,
            resource.IsActive
        );
    }
}