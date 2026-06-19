using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Assets.Domain.Model.Queries;

/// <summary>
/// Query to retrieve all properties that belong to a specific owner.
/// </summary>
public record GetAllPropertiesByOwnerIdQuery(
    ClientIdentity Owner,
    string? City,
    string? Street,
    int Page = 1,
    int PageSize = 10
);