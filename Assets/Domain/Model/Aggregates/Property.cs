using Hampcoders.Electrolink.API.Assets.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Assets.Domain.Model.Events;
using Hampcoders.Electrolink.API.Shared.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Assets.Domain.Model.Aggregates;

public class Property : BaseAggregateRoot
{
    public PropertyId Id { get; private set; } = null!;
    public ClientIdentity Owner { get; private set; } = null!;
    public Address Address { get; private set; } = null!;
    public Geolocation Geolocation { get; private set; } = null!;
    public EPropertyStatus Status { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string? MainPhotoProviderId { get; private set; }
    public EPropertyType PropertyType { get; private set; }

    private readonly List<PropertyPhoto> _photos = new();
    public IReadOnlyCollection<PropertyPhoto> Photos => _photos.AsReadOnly();

    private readonly List<string> _installedDeviceIds = new();
    public IReadOnlyCollection<string> InstalledDeviceIds => _installedDeviceIds.AsReadOnly();
    public bool HasActiveIoTMonitoring { get; private set; }

    private Property() { }
    
    public static Property Create(ClientIdentity owner, Address address, Geolocation geolocation, EPropertyType propertyType = EPropertyType.Residential)
    {
        if (owner is null || string.IsNullOrWhiteSpace(owner.ClientId))
            throw new ArgumentException("Owner must be valid.");
        if (address is null)    throw new ArgumentNullException(nameof(address));
        if (geolocation is null) throw new ArgumentNullException(nameof(geolocation));

        var property = new Property
        {
            Id = PropertyId.NewPropertyId(),
            Owner = owner,
            Address = address,
            Geolocation = geolocation,
            Status = EPropertyStatus.Created,
            IsActive = true,
            HasActiveIoTMonitoring = false,
            PropertyType = propertyType,
        };

        property.RaiseDomainEvent(new PropertyCreatedEvent(property.Id, property.Owner, property.Address, property.Geolocation, property.PropertyType, DateTime.UtcNow));

        return property;
    }

    internal void UpdateAddress(Address newAddress)
    {
        if (Address.Equals(newAddress)) return;
        Address = newAddress;
        RaiseDomainEvent(new PropertyAddressUpdatedEvent(Id, newAddress, DateTime.UtcNow));
    }
    
    internal void UpdateGeolocation(Geolocation newGeolocation)
    {
        var previous = Geolocation;
        Geolocation  = newGeolocation;
        RaiseDomainEvent(new PropertyGeolocationUpdatedEvent(
            Id, Owner, previous, newGeolocation, DateTime.UtcNow));
    }
    internal void Activate()
    {
        if (IsActive) return;
        IsActive = true;
        RaiseDomainEvent(new PropertyActivatedEvent(Id, DateTime.UtcNow));
    }

    internal void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
        RaiseDomainEvent(new PropertyDeactivatedEvent(Id, DateTime.UtcNow));
    }
    
    internal void Archive(string reason)
    {
        if (_installedDeviceIds.Count > 0)
            throw new InvalidOperationException(
                "Property cannot be archived while IoT devices are assigned or installed. " +
                "Decommission all devices first.");

        if (Status == EPropertyStatus.Archived) return;
        Status   = EPropertyStatus.Archived;
        IsActive = false;
        RaiseDomainEvent(new PropertyArchivedEvent(Id, Owner, reason, DateTime.UtcNow));
    }
    
    internal void RecordMaintenance(AssignmentId assignmentId, TechnicianId technicianId, string workSummary, DateTime completedAt)
    {
        RaiseDomainEvent(new PropertyMaintenanceRecordedEvent(
            Id, assignmentId, technicianId, workSummary, completedAt, DateTime.UtcNow));
    }

    internal void AddPhoto(string publicUrl, string providerId)
    {
        if (string.IsNullOrWhiteSpace(publicUrl) || string.IsNullOrWhiteSpace(providerId)) return;
        _photos.Add(PropertyPhoto.Create(publicUrl, providerId));
    }

    internal void SetMainPhoto(string providerId)
    {
        var photo = _photos.FirstOrDefault(p => p.ProviderId == providerId)
            ?? throw new KeyNotFoundException($"Photo with provider ID {providerId} not found.");

        MainPhotoProviderId = providerId;
        RaiseDomainEvent(new PropertyMainPhotoUpdatedEvent(Id, providerId, photo.PublicUrl, DateTime.UtcNow));
    }

    internal void MarkAsInPortfolio()
    {
        if (Status == EPropertyStatus.InPortfolio) return;
        Status = EPropertyStatus.InPortfolio;
    }
    
    internal void MarkAsAvailable()
    {
        if (Status == EPropertyStatus.Created) return;
        Status = EPropertyStatus.Created;
    }

    internal void AddInstalledDevice(IoTDeviceId deviceId)
    {
        if (_installedDeviceIds.Contains(deviceId.Value))
            return;

        _installedDeviceIds.Add(deviceId.Value);
        HasActiveIoTMonitoring = true;
    }

    internal void RemoveInstalledDevice(IoTDeviceId deviceId)
    {
        _installedDeviceIds.Remove(deviceId.Value);
        HasActiveIoTMonitoring = _installedDeviceIds.Count > 0;
    }
}