using Hampcoders.Electrolink.API.Planning.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Planning.Domain.Repositories;
using Hampcoders.Electrolink.API.Planning.Domain.Services;
using Hampcoders.Electrolink.API.Planning.Application.Internal.OutboundServices;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Entities;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Exceptions;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;
using MediatR;

namespace Hampcoders.Electrolink.API.Planning.Application.Internal.CommandServices;

public class ServiceAssignmentCommandService(
    IServiceAssignmentRepository assignmentRepository,
    IServiceRequestRepository requestRepository,
    ExternalProfilesService externalProfilesService,
    ExternalAssetsService externalAssetsService,
    IServiceCatalogRepository catalogRepository,
    IUnitOfWork unitOfWork,
    IMediator mediator,
    IMatchingService matchingService,
    ILogger<ServiceAssignmentCommandService> logger)
    : IServiceAssignmentCommandService
{
    public async Task Handle(AssignStaffToServiceCommand command)
    {
        var request = await requestRepository.FindByIdAsync(command.RequestId)
            ?? throw new InvalidOperationException($"ServiceRequest {command.RequestId.Value} not found.");

        var staffMemberId = Shared.Domain.Model.ValueObjects.StaffMemberId.From(command.StaffMemberId);

        var geo = request.Geolocation ?? Geolocation.Create(0, 0, null, "DEFAULT");
        var criteria = MatchingCriteria.Create(geo, new List<string>(), request.IsPriority, request.RequiresIoTCertifiedTechnician);

        var assignment = ServiceAssignment.AssignStaff(
            request.RequestId,
            staffMemberId,
            criteria);

        request.MarkAsAssignedToStaff(assignment.AssignmentId, staffMemberId);

        await assignmentRepository.AddAsync(assignment);
        requestRepository.Update(request);
        await unitOfWork.CompleteAsync();

        foreach (var domainEvent in request.DomainEvents)
            await mediator.Publish(domainEvent, CancellationToken.None);
        request.ClearDomainEvents();

        foreach (var domainEvent in assignment.DomainEvents)
            await mediator.Publish(domainEvent, CancellationToken.None);
        assignment.ClearDomainEvents();

        logger.LogInformation("[Planning] Staff member {StaffId} assigned to service request {RequestId}",
            command.StaffMemberId, command.RequestId.Value);
    }

    public async Task Handle(ExecuteMatchingAlgorithmCommand command)
    {
        var request = await requestRepository.FindByIdAsync(RequestId.From(command.RequestId.Value)) ?? throw new InvalidOperationException($"ServiceRequest with ID {command.RequestId.Value} not found.");

        if (request.Geolocation is null)
            throw new InvalidOperationException(
                $"ServiceRequest {command.RequestId.Value} has no geolocation. " +
                "The homeowner must select a property first.");

        if (request.RequestedCategory is null)
            throw new InvalidOperationException(
                $"ServiceRequest {command.RequestId.Value} has no category selected. " +
                "The homeowner must select a service category first.");
        
        var techniciansInArea = (await externalProfilesService.GetTechniciansInAreaAsync(
            request.Geolocation!.Latitude, request.Geolocation.Longitude)).ToList();

        var candidates = await FilterCandidatesAsync(techniciansInArea, request);

        if (!candidates.Any())
        {
            var existingRetries = await GetRetryCountAsync(request.RequestId);
            var failed = ServiceAssignment.Fail(
                request.RequestId, "NO_CANDIDATES_AVAILABLE", existingRetries);

            await assignmentRepository.AddAsync(failed);
            await unitOfWork.CompleteAsync();
        
            throw new NoCandidatesAvailableException("NO_CANDIDATES_AVAILABLE");
        }

        var enrichedCandidates = await EnrichCandidatesAsync(candidates, request);

        var result = await matchingService.FindBestCandidateAsync(
            request.RequestId, enrichedCandidates, request);

        var snapshot = RecipeSnapshot.FromRecipe(result.BestCandidate.Recipe);

        var criteria = MatchingCriteria.Create(
            request.Geolocation,
            result.BestCandidate.Recipe.ComponentRequirements.Select(c => c.ComponentTypeId).ToList(),
            request.IsPriority,
            request.RequiresIoTCertifiedTechnician,
            TechnicianId.From(result.BestCandidate.TechnicianId));

        var assignment = ServiceAssignment.Assign(
            request.RequestId,
            TechnicianId.From(result.BestCandidate.TechnicianId),
            snapshot,
            criteria,
            result.Score);

        request.MarkAsAssigned(
            assignment.AssignmentId,
            TechnicianId.From(result.BestCandidate.TechnicianId),
            snapshot);
        
        foreach (var domainEvent in request.DomainEvents)
        {
            await mediator.Publish(domainEvent, CancellationToken.None);
        }

        request.ClearDomainEvents();
        
        foreach (var domainEvent in assignment.DomainEvents)
        {
            await mediator.Publish(domainEvent, CancellationToken.None);
        }
        assignment.ClearDomainEvents(); 

        await assignmentRepository.AddAsync(assignment);
        requestRepository.Update(request);
        await unitOfWork.CompleteAsync();
    }

    private async Task<IReadOnlyList<Candidate>> FilterCandidatesAsync(
        IReadOnlyList<(string technicianId, string profileId, string fullName)> technicians,
        ServiceRequest request)
    {
        var technicianIds = technicians.Select(t => TechnicianId.From(t.technicianId)).ToList();

        var recipes = await catalogRepository.FindActiveRecipesByCategoryAndTechnicianIdsAsync(
            request.RequestedCategory!.Value, technicianIds);

        var stockTasks = new List<Task<(string technicianId, double rating, ServiceRecipe? recipe, bool hasStock)>>();

        foreach (var tech in technicians)
        {
            if (!recipes.TryGetValue(TechnicianId.From(tech.technicianId), out var recipe))
            {
                stockTasks.Add(Task.FromResult((tech.technicianId, 0.0, (ServiceRecipe?)null, false)));
                continue;
            }

            var requirements = recipe.ComponentRequirements
                .Select(cr => (cr.ComponentTypeId, cr.Quantity))
                .ToList();

            var technicianId = tech.technicianId;

            if (requirements.Count == 0)
            {
                stockTasks.Add(Task.FromResult((technicianId, 0.0, (ServiceRecipe?)recipe, true)));
                continue;
            }

            stockTasks.Add(externalAssetsService.CheckComponentStockAsync(technicianId, requirements)
                .ContinueWith(t => (technicianId, 0.0, (ServiceRecipe?)recipe, t.Result)));
        }

        var results = await Task.WhenAll(stockTasks);

        return results
            .Where(r => r.recipe is not null && r.hasStock)
            .Select(r => new Candidate(r.technicianId, r.rating, r.recipe!))
            .ToList();
    }

    private async Task<IReadOnlyList<EnrichedCandidate>> EnrichCandidatesAsync(
        IReadOnlyList<Candidate> candidates,
        ServiceRequest request)
    {
        var enrichmentTasks = candidates.Select(async candidate =>
        {
            try
            {
                var details = await externalProfilesService.GetTechnicianDetailsAsync(candidate.TechnicianId);

                var distanceKm = GeoDistanceService.CalculateDistanceKm(
                    request.Geolocation!.Latitude,
                    request.Geolocation.Longitude,
                    details.serviceAreaLat,
                    details.serviceAreaLon);

                return new EnrichedCandidate(
                    candidate.TechnicianId,
                    candidate.Rating,
                    candidate.Recipe,
                    distanceKm,
                    details.experienceYears,
                    details.specialties.ToList(),
                    IsIoTCertified: false,
                    HasRequiredComponents: true,
                    ResponseTimeMinutesAvg: 30
                );
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to enrich candidate {TechnicianId}, using defaults", candidate.TechnicianId);
                return new EnrichedCandidate(
                    candidate.TechnicianId,
                    candidate.Rating,
                    candidate.Recipe,
                    0.0,
                    0,
                    new List<string>(),
                    IsIoTCertified: false,
                    HasRequiredComponents: true,
                    ResponseTimeMinutesAvg: 30
                );
            }
        });

        var enriched = await Task.WhenAll(enrichmentTasks);
        return enriched.ToList();
    }

    private async Task<int> GetRetryCountAsync(RequestId requestId)
    {
        var existing = await assignmentRepository.FindByRequestIdAsync(requestId);
        return existing?.RetryCount ?? 0;
    }
}
