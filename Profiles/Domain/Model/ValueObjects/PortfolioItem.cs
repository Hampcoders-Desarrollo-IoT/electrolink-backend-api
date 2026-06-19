namespace Hampcoders.Electrolink.API.Profiles.Domain.Model.ValueObjects;

public record PortfolioItem(
    string PropertyId,
    string WorkSummary,
    string ServiceCategory,
    IReadOnlyList<string> PhotoUrls,
    DateTime CompletedAt);
