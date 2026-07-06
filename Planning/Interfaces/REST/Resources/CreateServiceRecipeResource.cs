using Hampcoders.Electrolink.API.Shared.Interfaces.REST.Resources;

namespace Hampcoders.Electrolink.API.Planning.Interfaces.REST.Resources;

public record CreateServiceRecipeResource(
    string ServiceName,
    string ServiceDescription,
    string ServiceCategory,
    bool RequiresIoTCertification,
    IReadOnlyList<ComponentRequirementResource> ComponentRequirements,
    int EstimatedDurationHours,
    int EstimatedDurationMinutes,
    PricingResource Pricing,
    IReadOnlyList<string> Prerequisites,
    IReadOnlyList<string> Deliverables,
    int WarrantyMonths);

