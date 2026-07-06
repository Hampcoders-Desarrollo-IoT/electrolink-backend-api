using Hampcoders.Electrolink.API.Planning.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Planning.Interfaces.REST.Resources;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Planning.Interfaces.REST.Transform;

public static class CreateServiceRecipeCommandFromResourceAssembler
{
    public static CreateServiceRecipeCommand ToCommand(
        CreateServiceRecipeResource resource,
        string catalogId,
        string technicianId) =>
        new(
            CatalogId.From(catalogId),
            TechnicianId.From(technicianId),
            resource.ServiceName,
            resource.ServiceDescription,
            resource.ServiceCategory,
            resource.RequiresIoTCertification,
            (resource.ComponentRequirements ?? [])
                .Select(cr => ComponentRequirementItem.Create(
                    cr.ComponentTypeId, string.Empty, cr.Quantity, cr.IsRequired))
                .ToList(),
            resource.EstimatedDurationHours,
            resource.EstimatedDurationMinutes,
            resource.Pricing.MaterialsEstimate,
            resource.Pricing.LaborCost,
            resource.Pricing.TotalPrice,
            resource.Pricing.Currency,
            resource.Prerequisites,
            resource.Deliverables,
            resource.WarrantyMonths);
}



