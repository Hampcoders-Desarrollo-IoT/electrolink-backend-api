using Hampcoders.Electrolink.API.Assets.Domain.Model.Exceptions;
using Hampcoders.Electrolink.API.Assets.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Assets.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Assets.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Assets.Domain.Repositories;
using Hampcoders.Electrolink.API.Assets.Domain.Services;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;
using MediatR;

namespace Hampcoders.Electrolink.API.Assets.Application.Internal.CommandServices;

public class PropertyPortfolioCommandService(
    IPropertyPortfolioRepository portfolioRepository,
    IPropertyRepository propertyRepository,
    IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<PropertyPortfolioCommandService> logger)
    : IPropertyPortfolioCommandService
{
    public async Task<PropertyPortfolio?> Handle(CreatePropertyPortfolioCommand command)
    {
        logger.LogInformation("[Assets BC] Creating portfolio for owner {Owner}", command.Owner);

        // Idempotencia: si ya existe, no crear uno nuevo
        if (await portfolioRepository.FindByOwnerAsync(command.Owner) is not null)
            throw new DuplicateAssetException("PropertyPortfolio", $"owner '{command.Owner}'");

        var portfolio = PropertyPortfolio.Create(command.Owner);

        await portfolioRepository.AddAsync(portfolio);
        await unitOfWork.CompleteAsync();
        await PublishAndClearEventsAsync(portfolio);

        logger.LogInformation("[Assets BC] Portfolio {PortfolioId} created for owner {Owner}",
            portfolio.Id, command.Owner);

        return portfolio;
    }

    public async Task<PropertyPortfolio?> Handle(AddPropertyToPortfolioCommand command)
    {
        var portfolio = await portfolioRepository.FindByOwnerWithEntriesAsync(command.Owner)
            ?? throw new AssetNotFoundException("PropertyPortfolio", $"owner '{command.Owner}'");

        // Validación de ownership: la propiedad debe pertenecer al mismo owner
        var property = await propertyRepository.FindByIdAsync(command.PropertyId)
            ?? throw new AssetNotFoundException("Property", command.PropertyId.Value);

        if (property.Owner != command.Owner)
            throw new UnauthorizedAccessException(
                $"Property {command.PropertyId} does not belong to owner {command.Owner}.");

        portfolio.AddProperty(
            command.PropertyId,
            command.Nickname,
            command.IsPrimary,
            Enum.Parse<EOccupancyStatus>(command.OccupancyStatus, ignoreCase: true));
        property.MarkAsInPortfolio();

        portfolioRepository.Update(portfolio);
        await unitOfWork.CompleteAsync();
        await PublishAndClearEventsAsync(portfolio);

        logger.LogInformation("[Assets BC] Property {PropertyId} added to portfolio of owner {Owner}",
            command.PropertyId, command.Owner);

        return portfolio;
    }

    public async Task<bool> Handle(RemovePropertyFromPortfolioCommand command)
    {
        var portfolio = await portfolioRepository.FindByOwnerWithEntriesAsync(command.Owner)
            ?? throw new AssetNotFoundException("PropertyPortfolio", $"owner '{command.Owner}'");

        portfolio.RemoveProperty(command.PropertyId, command.Reason);
        
        var property = await propertyRepository.FindByIdAsync(command.PropertyId);
        property?.MarkAsAvailable();

        portfolioRepository.Update(portfolio);
        await unitOfWork.CompleteAsync();
        await PublishAndClearEventsAsync(portfolio);

        logger.LogInformation("[Assets BC] Property {PropertyId} removed from portfolio of owner {Owner}. Reason: {Reason}",
            command.PropertyId, command.Owner, command.Reason);

        return true;
    }

    private async Task PublishAndClearEventsAsync(PropertyPortfolio portfolio)
    {
        var events = portfolio.DomainEvents.ToArray();
        portfolio.ClearDomainEvents();
        foreach (var domainEvent in events)
            await mediator.Publish(domainEvent, CancellationToken.None);
    }
}