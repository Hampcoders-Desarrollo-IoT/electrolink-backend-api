using Hampcoders.Electrolink.API.Shared.Domain.Model.Exceptions;

namespace Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

public record CompanyId
{
    public string Value { get; init; }

    private CompanyId(string value) => Value = value;

    public static CompanyId NewCompanyId() => new($"comp-{Guid.NewGuid()}");

    public static CompanyId From(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.StartsWith("comp-"))
            throw new InvalidIdException("CompanyId", value);
        return new CompanyId(value);
    }

    public override string ToString() => Value;
}
