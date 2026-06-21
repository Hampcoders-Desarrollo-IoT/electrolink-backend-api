using Hampcoders.Electrolink.API.Monitoring.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Monitoring.Interfaces.REST.Resources;
using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using ClientIdentity = Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects.ClientIdentity;
using RecipeSnapshot = Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects.RecipeSnapshot;
using RequestId = Hampcoders.Electrolink.API.Monitoring.Domain.Model.ValueObjects.RequestId;

namespace Hampcoders.Electrolink.API.Monitoring.Interfaces.REST.Transform;

public static class CreateServiceExecutionCommandFromResourceAssembler
{
    public static CreateServiceExecutionCommand ToCommandFromResource(CreateServiceExecutionResource resource)
    {
        var currency = Enum.Parse<ECurrency>(resource.RecipeSnapshot.Currency);

        return new CreateServiceExecutionCommand(
            AssignmentId.From(resource.AssignmentId),
            RequestId.From(resource.RequestId),
            TechnicianId.From(resource.TechnicianId),
            ClientIdentity.FromOwnerId(resource.OwnerId),
            PropertyId.From(resource.PropertyId),
            RecipeSnapshot.Create(
                RecipeId.From(resource.RecipeSnapshot.RecipeId),
                resource.RecipeSnapshot.ServiceName,
                Enum.Parse<EServiceCategory>(resource.RecipeSnapshot.ServiceCategory),
                resource.RecipeSnapshot.ComponentRequirements.Select(c => ComponentRequirementItem.Create(
                    c.ComponentTypeId,
                    string.Empty,
                    c.Quantity,
                    c.IsRequired
                )).ToList(),
                ServicePricing.Create(
                    new Money(resource.RecipeSnapshot.TotalPrice, currency), 
                    new Money(0m, currency),
                    new Money(0m, currency)
                ),
                new EstimatedDuration(resource.RecipeSnapshot.EstimatedDurationMinutes),
                new WarrantyPeriod(resource.RecipeSnapshot.WarrantyMonths),
                resource.RecipeSnapshot.SnapshotAt),
            resource.ScheduledDateTime,
            resource.IsPriority);
    }
    
    private static RecipeSnapshot BuildRecipeSnapshot(CreateServiceExecutionResource resource)
    {
        var snapshot = resource.RecipeSnapshot;

        return RecipeSnapshot.Create(
            RecipeId.From(snapshot.RecipeId),
            snapshot.ServiceName,
            ParseCategory(snapshot.ServiceCategory),
            BuildComponentRequirements(snapshot),
            BuildPricing(snapshot),
            new EstimatedDuration(snapshot.EstimatedDurationMinutes),
            new WarrantyPeriod(snapshot.WarrantyMonths),
            snapshot.SnapshotAt
        );
    }
    private static List<ComponentRequirementItem> BuildComponentRequirements(RecipeSnapshotResource snapshot) 
        => snapshot.ComponentRequirements
            .Select(cr => ComponentRequirementItem.Create(
                cr.ComponentTypeId,
                string.Empty,
                cr.Quantity,
                cr.IsRequired
            ))
            .ToList();
    
    private static ServicePricing BuildPricing(dynamic snapshot)
    {
        var basePrice = Money.Of(snapshot.TotalPrice, snapshot.Currency);
        var discount = Money.Of(0m, snapshot.Currency);
        var finalPrice = basePrice;

        return ServicePricing.Create(basePrice, discount, finalPrice);
    }
    private static EServiceCategory ParseCategory(string category) 
        => Enum.Parse<EServiceCategory>(category);
    
}
