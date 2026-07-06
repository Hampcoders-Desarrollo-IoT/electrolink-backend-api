using Hampcoders.Electrolink.API.Planning.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Entities;
using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Exceptions;
using Hampcoders.Electrolink.API.Planning.Domain.Repositories;
using Hampcoders.Electrolink.API.Planning.Domain.Services;
using Hampcoders.Electrolink.API.Planning.Application.Internal.OutboundServices;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;
using ComponentRequirementItem = Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects.ComponentRequirementItem;


namespace Hampcoders.Electrolink.API.Planning.Application.Internal.CommandServices;

public class ServiceCatalogCommandService(
    IServiceCatalogRepository catalogRepository,
    ExternalMonitoringService externalMonitoringService,
    ExternalAssetsService externalAssetsService,
    IComponentTypeValidator componentTypeValidator,
    IUnitOfWork unitOfWork,
    ILogger<ServiceCatalogCommandService> logger)
    : IServiceCatalogCommandService
{
    public async Task<ServiceCatalog?> Handle(CreateServiceCatalogCommand command)
    {
        logger.LogInformation("Creating service catalog for Technician {TechnicianId}", command.TechnicianId);

        if (await catalogRepository.ExistsByTechnicianIdAsync(command.TechnicianId))
            throw new CatalogAlreadyExistsException(command.TechnicianId);

        var catalog = ServiceCatalog.Create(command.TechnicianId);

        await catalogRepository.AddAsync(catalog);
        await unitOfWork.CompleteAsync();

        return catalog;
    }

    public async Task<ServiceRecipe?> Handle(CreateServiceRecipeCommand command)
    {
        var catalog = await catalogRepository.FindByTechnicianIdAsync(
            command.TechnicianId) ?? throw new CatalogNotFoundException(command.TechnicianId);

        if (catalog.CatalogId != command.CatalogId)
            throw new UnauthorizedCatalogAccessException();

        // Resolver nombres desde Assets BC
        var componentRequirements = new List<ComponentRequirementItem>();
        foreach (var cr in command.ComponentRequirements)
        {
            var name = await externalAssetsService.GetComponentTypeNameAsync(cr.ComponentTypeId)
                       ?? throw new ComponentTypeNotFoundException(cr.ComponentTypeId);

            componentRequirements.Add(
                ComponentRequirementItem.Create(cr.ComponentTypeId, name, cr.Quantity, cr.IsRequired));
        }

        var currency = Enum.Parse<ECurrency>(command.Currency, ignoreCase: true);

        catalog.AddRecipe(
            command.ServiceName,
            command.ServiceDescription,
            Enum.Parse<EServiceCategory>(command.ServiceCategory, ignoreCase: true),
            command.RequiresIoTCertification,
            componentRequirements,
            EstimatedDuration.FromHoursAndMinutes(command.EstimatedDurationHours, command.EstimatedDurationMinutes),
            ServicePricing.Create(
                Money.Of(command.MaterialsEstimate, currency),
                Money.Of(command.LaborCost, currency),
                Money.Of(command.TotalPrice, currency)),
            command.Prerequisites,
            command.Deliverables,
            WarrantyPeriod.OfMonths(command.WarrantyMonths),
            componentTypeValidator);

        catalogRepository.Update(catalog);
        await unitOfWork.CompleteAsync();

        return catalog.Recipes.FirstOrDefault(r => r.ServiceName == command.ServiceName);
    }

    public async Task<ServiceRecipe?> Handle(UpdateServiceRecipeCommand command)
    {
        var catalog = await catalogRepository.FindByTechnicianIdAsync(command.TechnicianId)
            ?? throw new CatalogNotFoundException(command.TechnicianId);

        if (catalog.CatalogId != command.CatalogId)
            throw new UnauthorizedCatalogAccessException();

        var activeCount = await externalMonitoringService.CountActiveServicesForRecipeAsync(command.RecipeId.Value);

        IReadOnlyList<ComponentRequirementItem>? components = null;
        if (command.ComponentRequirements is not null)
        {
            var resolved = new List<ComponentRequirementItem>();
            foreach (var cr in command.ComponentRequirements)
            {
                var name = await externalAssetsService.GetComponentTypeNameAsync(cr.ComponentTypeId)
                    ?? throw new ComponentTypeNotFoundException(cr.ComponentTypeId);

                resolved.Add(
                    ComponentRequirementItem.Create(cr.ComponentTypeId, name, cr.Quantity, cr.IsRequired));
            }
            components = resolved;
        }

        ServicePricing? pricing = null;
        EstimatedDuration? duration = null;
        WarrantyPeriod? warranty = null;

        if (command.TotalPrice.HasValue)
        {
            var currency = Enum.Parse<ECurrency>(command.Currency!, ignoreCase: true);
            pricing = ServicePricing.Create(
                Money.Of(command.MaterialsEstimate!.Value, currency),
                Money.Of(command.LaborCost!.Value, currency),
                Money.Of(command.TotalPrice.Value, currency));
        }

        if (command.EstimatedDurationHours.HasValue)
            duration = EstimatedDuration.FromHoursAndMinutes(
                command.EstimatedDurationHours.Value, command.EstimatedDurationMinutes ?? 0);

        if (command.WarrantyMonths.HasValue)
            warranty = WarrantyPeriod.OfMonths(command.WarrantyMonths.Value);

        catalog.UpdateRecipe(
            command.RecipeId,
            command.ServiceName,
            command.ServiceDescription,
            components,
            duration,
            pricing,
            command.Prerequisites,
            command.Deliverables,
            warranty,
            activeCount,
            componentTypeValidator);

        catalogRepository.Update(catalog);
        await unitOfWork.CompleteAsync();

        return catalog.Recipes.FirstOrDefault(r => r.Id == command.RecipeId);
    }

    public async Task<bool> Handle(DeactivateServiceRecipeCommand command)
    {
        var catalog = await catalogRepository.FindByTechnicianIdAsync(command.TechnicianId) ?? throw new CatalogNotFoundException(command.TechnicianId);

        if (catalog.CatalogId != command.CatalogId)
            throw new UnauthorizedCatalogAccessException();

        var inProgressCount = await externalMonitoringService.CountInProgressServicesForRecipeAsync(command.RecipeId.Value);

        catalog.DeactivateRecipe(command.RecipeId,
            DeactivationReason.From(command.Reason),
            inProgressCount);

        catalogRepository.Update(catalog);
        await unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<bool> Handle(ReactivateServiceRecipeCommand command)
    {
        var catalog = await catalogRepository.FindByTechnicianIdAsync(
            command.TechnicianId) ?? throw new CatalogNotFoundException(command.TechnicianId);

        if (catalog.CatalogId != command.CatalogId)
            throw new UnauthorizedCatalogAccessException();

        catalog.ReactivateRecipe(command.RecipeId);

        catalogRepository.Update(catalog);
        await unitOfWork.CompleteAsync();

        return true;
    }
}