namespace Hampcoders.Electrolink.API.Subscriptions.Domain.Model.ValueObjects;

public enum EBusinessRole
{
    Technician,
    Homeowner,
    Company
}

public record BusinessRole
{
    public EBusinessRole Value { get; }

    private BusinessRole(EBusinessRole value)
    {
        Value = value;
    }

    public static BusinessRole Technician => new(EBusinessRole.Technician);
    public static BusinessRole Homeowner => new(EBusinessRole.Homeowner);
    public static BusinessRole Company => new(EBusinessRole.Company);

    public static BusinessRole From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("BusinessRole cannot be empty.");

        return value.Trim().ToUpperInvariant() switch
        {
            "TECHNICIAN" => Technician,
            "HOMEOWNER" => Homeowner,
            "COMPANY" => Company,
            _ => throw new ArgumentException($"Invalid BusinessRole: {value}")
        };
    }

    public override string ToString() => Value.ToString().ToUpperInvariant();
}
