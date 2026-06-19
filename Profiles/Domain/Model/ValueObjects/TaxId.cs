using Hampcoders.Electrolink.API.Profiles.Domain.Model.Exceptions;

namespace Hampcoders.Electrolink.API.Profiles.Domain.Model.ValueObjects;

public record TaxId
{
    public string Value { get; init; }

    private TaxId(string value) => Value = value;

    public static TaxId From(string raw)
    {
        var cleaned = raw.Trim();
        if (string.IsNullOrWhiteSpace(cleaned))
            throw new InvalidTaxIdException(raw);
        return new TaxId(cleaned);
    }

    public override string ToString() => Value;
}
