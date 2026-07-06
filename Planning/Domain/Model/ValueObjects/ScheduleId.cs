namespace Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;

public record ScheduleId
{
    public string Value { get; }

    private ScheduleId(string value) => Value = value;

    public static ScheduleId NewId() => new($"sched-{Guid.NewGuid()}");

    public static ScheduleId From(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.StartsWith("sched-"))
            throw new ArgumentException("ScheduleId must be a valid schedule ID.");
        return new ScheduleId(value);
    }

    public override string ToString() => Value;
}
