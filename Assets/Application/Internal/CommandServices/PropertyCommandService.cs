using Hampcoders.Electrolink.API.Assets.Domain.Model.Exceptions;
using MediatR;
using Hampcoders.Electrolink.API.Assets.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Assets.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Assets.Domain.Repositories;
using Hampcoders.Electrolink.API.Assets.Domain.Services;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Domain.Model.Entities;
using Hampcoders.Electrolink.API.Shared.Infrastructure;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Interfaces;

namespace Hampcoders.Electrolink.API.Assets.Application.Internal.CommandServices;

using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

public class PropertyCommandService(
    IPropertyRepository propertyRepository,
    IUnitOfWork unitOfWork,
    IMediator mediator,
    IFileStorageService fileStorageService,
    IOrphanedFileRepository orphanedFileRepository) : IPropertyCommandService
{
    private const string PropertyPhotoFolder = "electrolink/assets/properties";
    public async Task<Property?> Handle(CreatePropertyCommand command)
    {
        var property = Property.Create(command.Owner, command.Address, command.Geolocation, command.PropertyType);
        await propertyRepository.AddAsync(property);
        await unitOfWork.CompleteAsync();
        
        foreach (var domainEvent in property.DomainEvents)
        {
            await mediator.Publish(domainEvent, CancellationToken.None);
        }
        property.ClearDomainEvents();
        
        return property;
    }

    public async Task<Property?> Handle(AddPhotoToPropertyCommand command)
    {
        var property = await propertyRepository.FindByIdAsync(command.PropertyId);
        if (property is null) throw new AssetNotFoundException("Property", command.PropertyId.Value);

        property.AddPhoto(command.PhotoUrl, "legacy-provider");

        await unitOfWork.CompleteAsync();
        return property;
    }

    public async Task<SignedUploadData> Handle(GetPropertyPhotoUploadUrlCommand command)
    {
        var property = await propertyRepository.FindByIdAndOwnerAsync(command.PropertyId, command.Owner);
        if (property is null) throw new AssetNotFoundException("Property", command.PropertyId.Value);

        return await fileStorageService.GetSignedUploadUrlForPropertyPhotoAsync(command.PropertyId.Value);
    }

    public async Task<Property?> Handle(RegisterPropertyPhotoCommand command)
    {
        var property = await propertyRepository.FindByIdAndOwnerAsync(command.PropertyId, command.Owner);
        if (property is null) throw new AssetNotFoundException("Property", command.PropertyId.Value);

        try
        {
            property.AddPhoto(command.PublicUrl, command.ProviderId);

            await unitOfWork.CompleteAsync();
            foreach (var domainEvent in property.DomainEvents)
                await mediator.Publish(domainEvent, CancellationToken.None);
            property.ClearDomainEvents();

            return property;
        }
        catch
        {
            var folder = $"{PropertyPhotoFolder}/{command.PropertyId.Value}";
            await orphanedFileRepository.AddAsync(
                OrphanedFileDeletion.Create(command.ProviderId, folder, "Property photo registration failed"));
            await unitOfWork.CompleteAsync();
            throw;
        }
    }

    public async Task<Property?> Handle(SetPropertyMainPhotoCommand command)
    {
        var property = await propertyRepository.FindByIdAndOwnerAsync(command.PropertyId, command.Owner);
        if (property is null) throw new AssetNotFoundException("Property", command.PropertyId.Value);

        property.SetMainPhoto(command.ProviderId);

        await unitOfWork.CompleteAsync();
        foreach (var domainEvent in property.DomainEvents)
            await mediator.Publish(domainEvent, CancellationToken.None);
        property.ClearDomainEvents();

        return property;
    }

    public async Task<Property?> Handle(UpdatePropertyAddressCommand command)
    {
        var property = await propertyRepository.FindByIdAsync(command.PropertyId);
        if (property is null) throw new AssetNotFoundException("Property", command.PropertyId.Value);

        property.UpdateAddress(command.NewAddress);
        await unitOfWork.CompleteAsync();
        
        foreach (var domainEvent in property.DomainEvents)
        {
            await mediator.Publish(domainEvent, CancellationToken.None);
        }
        property.ClearDomainEvents();
        return property;
    }

    public async Task<Property?> Handle(UpdatePropertyGeolocationCommand command)
    {
        var property = await propertyRepository.FindByIdAsync(command.PropertyId);
        if (property is null) throw new AssetNotFoundException("Property", command.PropertyId.Value);

        property.UpdateGeolocation(command.NewGeolocation);
        await unitOfWork.CompleteAsync();

        foreach (var domainEvent in property.DomainEvents)
            await mediator.Publish(domainEvent, CancellationToken.None);
        property.ClearDomainEvents();

        return property;
    }

    public async Task<Property?> Handle(DeactivatePropertyCommand command)
    {
        var property = await propertyRepository.FindByIdAsync(command.PropertyId);
        if (property == null) return null;

        property.Deactivate();
        await unitOfWork.CompleteAsync();
            
        // Publicar eventos de dominio
        foreach (var domainEvent in property.DomainEvents)
        {
            await mediator.Publish(domainEvent, CancellationToken.None);
        }
        property.ClearDomainEvents();
            
        return property;
    }

    public async Task<Property?> Handle(ArchivePropertyCommand command)
    {
        var property = await propertyRepository.FindByIdAsync(command.PropertyId);
        if (property is null) throw new AssetNotFoundException("Property", command.PropertyId.Value);

        property.Archive(command.Reason);
        await unitOfWork.CompleteAsync();

        foreach (var domainEvent in property.DomainEvents)
            await mediator.Publish(domainEvent, CancellationToken.None);
        property.ClearDomainEvents();

        return property;
    }


    public async Task<Property?> Handle(ActivatePropertyCommand command)
    {
        var property = await propertyRepository.FindByIdAsync(command.PropertyId);
        if (property == null) return null;

        property.Activate(); 
        await unitOfWork.CompleteAsync();
            
        // Publicar eventos de dominio
        foreach (var domainEvent in property.DomainEvents)
        {
            await mediator.Publish(domainEvent, CancellationToken.None);
        }
        property.ClearDomainEvents();
                
        return property;
    }
    
    public async Task<Property?> Handle(UpdatePropertyCommand command)
    {
        var property = await propertyRepository.FindByIdAndOwnerAsync(command.PropertyId, command.Owner);
        if (property is null) return null;

        property.UpdateAddress(command.Address);
        await unitOfWork.CompleteAsync();

        foreach (var domainEvent in property.DomainEvents)
            await mediator.Publish(domainEvent, CancellationToken.None);
        property.ClearDomainEvents();

        return property;
    }
    
    public async Task<bool> Handle(DeletePropertyCommand command)
    {
        var property = await propertyRepository.FindByIdAsync(command.PropertyId);
        if (property is null) return false;

        propertyRepository.Remove(property);
        await unitOfWork.CompleteAsync();
        return true;
    }

    public async Task<Property?> Handle(RecordMaintenanceForPropertyCommand command)
    {
        var property = await propertyRepository.FindByIdAsync(command.PropertyId);
        if (property is null) throw new AssetNotFoundException("Property", command.PropertyId.Value);

        property.RecordMaintenance(command.AssignmentId, command.TechnicianId, command.WorkSummary, command.CompletedAt);
        await unitOfWork.CompleteAsync();

        foreach (var domainEvent in property.DomainEvents)
            await mediator.Publish(domainEvent, CancellationToken.None);
        property.ClearDomainEvents();

        return property;
    }
}