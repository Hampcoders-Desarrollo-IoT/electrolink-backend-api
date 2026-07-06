namespace Hampcoders.Electrolink.API.IAM.Domain.Model.ValueObjects;

public record PasswordResetToken
{
    public string Value { get; }
    public DateTime ExpiresAt { get; }

    private PasswordResetToken(string value, DateTime expiresAt)
    {
        Value = value;
        ExpiresAt = expiresAt;
    }

    public static PasswordResetToken Generate() =>
        new($"reset-{Guid.NewGuid()}", DateTime.UtcNow.AddHours(1));

    public static PasswordResetToken FromHash(string value, DateTime expiresAt) =>
        new(value, expiresAt);

    public bool IsValid(string token) =>
        Value == token && DateTime.UtcNow < ExpiresAt;
}
