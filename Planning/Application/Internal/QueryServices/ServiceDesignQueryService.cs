using Hampcoders.Electrolink.API.Planning.Application.Internal.OutboundServices;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Entities;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Queries;
using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Planning.Domain.Repositories;
using Hampcoders.Electrolink.API.Planning.Domain.Services;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Planning.Application.Internal.QueryServices;

/// <summary>
/// Implementación de IServiceDesignQueryService.
/// Centraliza las consultas relacionadas con diseño de servicios, elegibilidad y matching.
/// </summary>
public class ServiceDesignQueryService : IServiceDesignQueryService
{
    private readonly IServiceCatalogRepository _catalogRepository;
    private readonly IServiceRequestRepository _requestRepository;
    private readonly IServiceAssignmentRepository _assignmentRepository;
    private readonly ExternalProfilesService _externalProfileService;
    private readonly ExternalSubscriptionsService _externalSubscriptionsService;
    private readonly ExternalAssetsService _externalAssetsService;
    private readonly ILogger<ServiceDesignQueryService> _logger;

    public ServiceDesignQueryService(
        IServiceCatalogRepository catalogRepository,
        IServiceRequestRepository requestRepository,
        IServiceAssignmentRepository assignmentRepository,
        ExternalProfilesService externalProfileService,
        ExternalSubscriptionsService externalSubscriptionsService,
        ExternalAssetsService externalAssetsService,
        ILogger<ServiceDesignQueryService> logger)
    {
        _catalogRepository = catalogRepository;
        _requestRepository = requestRepository;
        _assignmentRepository = assignmentRepository;
        _externalProfileService = externalProfileService;
        _externalSubscriptionsService = externalSubscriptionsService;
        _externalAssetsService = externalAssetsService;
        _logger = logger;
    }
    public async Task<ServiceCatalog?> Handle(GetServiceCatalogQuery query) 
        => await _catalogRepository.FindByTechnicianIdAsync(query.TechnicianId);
    
    public async Task<ServiceRecipe?> Handle(GetServiceRecipeDetailsQuery query)
    {
            var catalog = await _catalogRepository.FindByTechnicianIdAsync(
            query.TechnicianId);

        if (catalog is null) return null;

        var recipe = catalog.Recipes.FirstOrDefault(r =>
            r.Id == query.RecipeId);

        return recipe;
    }
    
    public async Task<RequestEligibility> Handle(GetRequestEligibilityQuery query)
    {
        var isActive = await _externalProfileService.IsClientActiveAsync(query.Client);

        if (!isActive)
            return new RequestEligibility(false, null, null, false, "PROFILE_INCOMPLETE");

        var eligibility = await _externalSubscriptionsService.GetRemainingRequestsAsync(query.Client.ClientId);

        return new RequestEligibility(
            eligibility.canCreate,
            eligibility.planType,
            eligibility.remainingRequests,
            eligibility.canMarkAsPriority,
            eligibility.canCreate ? null : "MONTHLY_LIMIT_REACHED");
    }
    
    public async Task<IEnumerable<AvailableService>> Handle(GetAvailableServicesQuery query) {
        var request = await _requestRepository.FindByIdAsync(query.RequestId);
        if (request is null || request.Geolocation is null) return [];

        var technicians = (await _externalProfileService.GetTechniciansInAreaAsync(
            request.Geolocation.Latitude, request.Geolocation.Longitude)).ToList();

        var technicianIds = technicians.Select(t => TechnicianId.From(t.technicianId)).ToList();
        var catalogs = await _catalogRepository.FindCatalogsByTechnicianIdsAsync(technicianIds);

        var stockTasks = new List<Task<(EServiceCategory Category, decimal Price, int Duration, bool HasStock)>>();

        foreach (var catalog in catalogs.Values)
        {
            if (catalog.Status != ECatalogStatus.Active) continue;

            foreach (var recipe in catalog.Recipes.Where(r => r.IsActive))
            {
                var requirements = recipe.ComponentRequirements
                    .Select(c => (c.ComponentTypeId, c.Quantity))
                    .ToList();

                var techId = recipe.TechnicianId.Value;
                stockTasks.Add(_externalAssetsService
                    .TechnicianHasStockForRecipeAsync(techId, requirements)
                    .ContinueWith(t => (recipe.ServiceCategory,
                        recipe.Pricing.TotalPrice.Amount,
                        recipe.EstimatedDuration.TotalMinutes,
                        t.Result)));
            }
        }

        var stockResults = await Task.WhenAll(stockTasks);

        var categoryData = new Dictionary<EServiceCategory, List<(decimal price, int duration)>>();
        foreach (var result in stockResults)
        {
            if (!result.HasStock) continue;
            if (!categoryData.ContainsKey(result.Category))
                categoryData[result.Category] = new List<(decimal, int)>();
            categoryData[result.Category].Add((result.Price, result.Duration));
        }

        return categoryData.Select(kvp => new AvailableService(
            ServiceCategory: kvp.Key.ToString(),
            CategoryDisplayName: GetCategoryDisplayName(kvp.Key),
            MinPrice: kvp.Value.Min(x => x.price),
            MaxPrice: kvp.Value.Max(x => x.price),
            EstimatedDurationMinutes: (int)kvp.Value.Average(x => x.duration),
            TechniciansAvailable: kvp.Value.Count));
    }

private static string GetCategoryDisplayName(EServiceCategory category) => category switch
{
    EServiceCategory.SolarInstallation    => "Instalación Solar",
    EServiceCategory.ElectricalMaintenance => "Mantenimiento Eléctrico",
    EServiceCategory.Repair               => "Reparación",
    EServiceCategory.Inspection           => "Inspección",
    EServiceCategory.Upgrade              => "Actualización",
    _                                     => category.ToString()
};


    public async Task<ServiceRequest?> Handle(GetServiceRequestSummaryQuery query)
    {
        var request = await _requestRepository.FindByIdAsync(query.RequestId);

        if (request is null || request.Client != query.Client)
            return null;

        return request;
    }

    public async Task<MatchingQueue> Handle(GetMatchingQueueQuery query)
    {
        var pending = await _requestRepository.FindPendingAssignmentAsync(query.Page, query.PageSize);

        var sorted = pending.OrderByDescending(r => r.IsPriority)
            .ThenBy(r => r.CreatedDate)
            .ToList();

        var priorityQueue = sorted.Where(r => r.IsPriority)
            .Select(r => new QueuedRequest(r.RequestId.Value, r.HomeownerId.Value, r.IsPriority, r.CreatedDate.GetValueOrDefault()))
            .ToList();

        var normalQueue = sorted.Where(r => !r.IsPriority)
            .Select(r => new QueuedRequest(r.RequestId.Value, r.HomeownerId.Value, r.IsPriority, r.CreatedDate.GetValueOrDefault()))
            .ToList();

        return new MatchingQueue(priorityQueue, normalQueue, sorted.Count);
    }

    public async Task<ServiceRequest?> Handle(GetServiceRequestByIdQuery query) 
        => await _requestRepository.FindByIdAsync(query.RequestId);
}


