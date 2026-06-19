namespace Hampcoders.Electrolink.API.Subscriptions.Domain.Model.ValueObjects;

public enum EPlanType
{
    Basic,
    Premium,
    EnterpriseBasic,
    EnterprisePremium
}

public record PlanType
{
    public EPlanType Value { get; }

    private PlanType(EPlanType value)
    {
        Value = value;
    }

    public static PlanType Basic => new(EPlanType.Basic);
    public static PlanType Premium => new(EPlanType.Premium);
    public static PlanType EnterpriseBasic => new(EPlanType.EnterpriseBasic);
    public static PlanType EnterprisePremium => new(EPlanType.EnterprisePremium);

    public bool IsBasic => Value == EPlanType.Basic;
    public bool IsPremium => Value == EPlanType.Premium;
    public bool IsEnterpriseBasic => Value == EPlanType.EnterpriseBasic;
    public bool IsEnterprisePremium => Value == EPlanType.EnterprisePremium;
    public bool IsAnyEnterprise => Value is EPlanType.EnterpriseBasic or EPlanType.EnterprisePremium;

    public static PlanType From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("PlanType cannot be empty.");

        return value.Trim().ToUpperInvariant() switch
        {
            "BASIC" => Basic,
            "PREMIUM" => Premium,
            "ENTERPRISEBASIC" => EnterpriseBasic,
            "ENTERPRISE_BASIC" => EnterpriseBasic,
            "ENTERPRISEPREMIUM" => EnterprisePremium,
            "ENTERPRISE_PREMIUM" => EnterprisePremium,
            _ => throw new ArgumentException($"Invalid PlanType: {value}")
        };
    }

    public override string ToString() => Value switch
    {
        EPlanType.EnterpriseBasic => "ENTERPRISE_BASIC",
        EPlanType.EnterprisePremium => "ENTERPRISE_PREMIUM",
        _ => Value.ToString().ToUpperInvariant()
    };
}
