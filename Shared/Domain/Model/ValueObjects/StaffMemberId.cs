using Hampcoders.Electrolink.API.Shared.Domain.Model.Exceptions;

namespace Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

public record StaffMemberId
{
    public string Value { get; init; }

    private StaffMemberId(string value) => Value = value;

    public static StaffMemberId NewStaffMemberId() => new($"stf-{Guid.NewGuid()}");

    public static StaffMemberId From(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.StartsWith("stf-"))
            throw new InvalidIdException("StaffMemberId", value);
        return new StaffMemberId(value);
    }

    public override string ToString() => Value;
}
