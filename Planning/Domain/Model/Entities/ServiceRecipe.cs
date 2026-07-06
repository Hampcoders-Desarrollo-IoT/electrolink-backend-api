using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Exceptions;
using Hampcoders.Electrolink.API.Planning.Domain.Services;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using ComponentRequirementItem = Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects.ComponentRequirementItem;

namespace Hampcoders.Electrolink.API.Planning.Domain.Model.Entities;

public class ServiceRecipe
{
    public RecipeId Id { get; private set; }
    public CatalogId CatalogId { get; private set; }
    public TechnicianId TechnicianId { get; private set; }
    public string ServiceName { get; private set; }
    public string ServiceDescription  { get; private set; }
    public EServiceCategory ServiceCategory { get; private set; }  
    private List<ComponentRequirementItem> _componentRequirements = new List<ComponentRequirementItem>();
    public IReadOnlyList<ComponentRequirementItem> ComponentRequirements => _componentRequirements.AsReadOnly();
    public EstimatedDuration EstimatedDuration { get; private set; }
    public ServicePricing Pricing { get; private set; }
    private List<string> _prerequisites = new List<string>();
    public IReadOnlyList<string> Prerequisites => _prerequisites.AsReadOnly();
    private List<string> _deliverables = new List<string>();
    public IReadOnlyList<string> Deliverables => _deliverables.AsReadOnly();
    public WarrantyPeriod WarrantyPeriod { get; private set; }
    public bool IsActive { get; private set; }
    public bool RequiresIoTCertification { get; private set; }
    public int TimesRequested { get; private set; }
    
    // Constructor para EF Core
    protected ServiceRecipe() { }
    
    // Constructor principal
    public static ServiceRecipe Create(
        CatalogId catalogId,
        TechnicianId technicianId,
        string serviceName,
        string serviceDescription,
        EServiceCategory serviceCategory,
        bool requiresIoTCertification,
        IReadOnlyList<ComponentRequirementItem> componentRequirements,
        EstimatedDuration estimatedDuration,
        ServicePricing pricing,
        IReadOnlyList<string> prerequisites,
        IReadOnlyList<string> deliverables,
        WarrantyPeriod warrantyPeriod)
    {
        return new ServiceRecipe
        {
            Id = RecipeId.NewId(),
            CatalogId = catalogId,
            TechnicianId = technicianId,
            ServiceName = serviceName.Trim(),
            ServiceDescription = serviceDescription.Trim(),
            ServiceCategory = serviceCategory,
            RequiresIoTCertification = requiresIoTCertification,
            _componentRequirements = componentRequirements.ToList(),
            EstimatedDuration = estimatedDuration,
            Pricing = pricing,
            _prerequisites = prerequisites.ToList(),
            _deliverables = deliverables.ToList(),
            WarrantyPeriod = warrantyPeriod,
            IsActive = true,
            TimesRequested = 0,
        };
    }
    
    public void Update(
        string? serviceName,
        string? serviceDescription,
        IReadOnlyList<ComponentRequirementItem>? componentRequirements,
        EstimatedDuration? estimatedDuration,
        ServicePricing? pricing,
        IReadOnlyList<string>? prerequisites,
        IReadOnlyList<string>? deliverables,
        WarrantyPeriod? warrantyPeriod,
        int activeServicesCount,
        IComponentTypeValidator componentTypeValidator)
    {
        if (!IsActive)
            throw new CannotUpdateInactiveRecipeException(Id);

        // Si hay servicios activos: no se pueden cambiar componentes
        if (componentRequirements is not null && activeServicesCount > 0)
            throw new CannotChangeComponentsWithActiveServicesException(Id, activeServicesCount);

        // Si hay servicios activos: precio puede subir máximo 20%
        if (pricing is not null && activeServicesCount > 0)
            if (Pricing.IsPriceIncreaseOver20Percent(pricing.TotalPrice))
                throw new PriceIncreaseTooLargeException(Id);

        if (componentRequirements is not null)
        {
            componentTypeValidator.ValidateAll(componentRequirements);
            _componentRequirements = componentRequirements.ToList();
        }

        if (serviceName    is not null) ServiceName        = serviceName.Trim();
        if (serviceDescription is not null) ServiceDescription = serviceDescription.Trim();
        if (estimatedDuration  is not null) EstimatedDuration  = estimatedDuration;
        if (pricing            is not null) Pricing            = pricing;
        if (prerequisites      is not null) _prerequisites      = prerequisites.ToList();
        if (deliverables       is not null) _deliverables        = deliverables.ToList();
        if (warrantyPeriod     is not null) WarrantyPeriod       = warrantyPeriod;
    }

    public void Deactivate(DeactivationReason reason, int inProgressCount)
    {
        if (!IsActive)
            throw new RecipeAlreadyInactiveException(Id);

        if (inProgressCount > 0)
            throw new CannotDeactivateRecipeWithInProgressServicesException(Id, inProgressCount);

        IsActive = false;
    }

    public void Reactivate()
    {
        if (IsActive)
            throw new RecipeAlreadyActiveException(Id);

        IsActive = true;
    }

    public void IncrementTimesRequested() => TimesRequested++;
    
}

