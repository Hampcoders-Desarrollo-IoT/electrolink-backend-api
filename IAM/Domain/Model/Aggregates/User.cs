using Hampcoders.Electrolink.API.IAM.Domain.Model.Events;
using Hampcoders.Electrolink.API.IAM.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.IAM.Domain.Model.Aggregates;

public class User : BaseAggregateRoot
{
    public UserId Id { get; private set; }
    public Email Email { get; private set; }
    public HashedPassword PasswordHash { get; private set; }
    public EUserRole Role { get; private set; }
    public EUserStatus Status { get; private set; }
    private User() { }

    public static User Create(Email email, HashedPassword passwordHash, EUserRole role)
    {
        var user = new User
        {
            Id = UserId.NewUserId(),
            Email = email,
            PasswordHash = passwordHash,
            Role = role,
            Status = EUserStatus.Active
        };

        user.RaiseDomainEvent(new UserRegisteredEvent(user.Id.Value, user.Email.Value, DateTime.UtcNow, user.Role.ToString()));

        return user;
    }

    public void UpdatePasswordHash(HashedPassword newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        RaiseDomainEvent(new UserPasswordChangedEvent(Id.Value, DateTime.UtcNow));
    }

    public void RecordSignIn()
    {
        RaiseDomainEvent(new UserSignedInEvent(Id.Value, DateTime.UtcNow));
    }

    public void Suspend(string reason)
    {
        if (Status == EUserStatus.Suspended) return;
        Status = EUserStatus.Suspended;
        RaiseDomainEvent(new UserAccountSuspendedEvent(Id.Value, reason, DateTime.UtcNow));
    }

    public void Activate()
    {
        if (Status == EUserStatus.Active) return;
        Status = EUserStatus.Active;
        RaiseDomainEvent(new UserAccountActivatedEvent(Id.Value, DateTime.UtcNow));
    }
}
