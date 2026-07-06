using Hampcoders.Electrolink.API.Profiles.Domain.Model.Events;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Profiles.Domain.Model.Aggregates;

public class StaffMember : BaseAggregateRoot
{
    public StaffMemberId Id { get; private set; }
    public UserId UserId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string? PhoneNumber { get; private set; }
    public StaffZone? AssignedZone { get; private set; }
    public bool IsIoTCertified { get; private set; }
    public DateTime? IoTCertifiedAt { get; private set; }
    public DateTime HireDate { get; private set; }
    public bool IsActive { get; private set; }

    private StaffMember() { }

    public static StaffMember Create(string userId, string firstName, string lastName, string? phoneNumber)
    {
        var member = new StaffMember
        {
            Id = StaffMemberId.NewStaffMemberId(),
            UserId = UserId.From(userId),
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            AssignedZone = null,
            IsIoTCertified = false,
            IoTCertifiedAt = null,
            HireDate = DateTime.UtcNow,
            IsActive = true,
        };

        member.RaiseDomainEvent(new StaffMemberCreatedEvent(
            member.Id.Value, member.UserId.Value, member.FirstName, member.LastName, DateTime.UtcNow));

        return member;
    }

    public void AssignZone(StaffZone zone)
    {
        AssignedZone = zone;
        RaiseDomainEvent(new StaffMemberZoneAssignedEvent(Id.Value, zone, DateTime.UtcNow));
    }

    public void UpdatePersonalData(string firstName, string lastName, string? phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        if (phoneNumber is not null)
            PhoneNumber = phoneNumber;
    }

    public void GrantIoTCertification()
    {
        if (IsIoTCertified) return;
        IsIoTCertified = true;
        IoTCertifiedAt = DateTime.UtcNow;
        RaiseDomainEvent(new StaffIoTCertificationGrantedEvent(Id.Value, DateTime.UtcNow));
    }

    public void RevokeIoTCertification()
    {
        if (!IsIoTCertified)
            throw new InvalidOperationException("Staff member is not IoT certified.");
        IsIoTCertified = false;
        IoTCertifiedAt = null;
        RaiseDomainEvent(new StaffIoTCertificationRevokedEvent(Id.Value, DateTime.UtcNow));
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
        RaiseDomainEvent(new StaffMemberDeactivatedEvent(Id.Value, DateTime.UtcNow));
    }

    public void Reactivate()
    {
        if (IsActive) return;
        IsActive = true;
    }
}
