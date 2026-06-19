using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Assets.Domain.Model.Queries;

/// <summary>
/// Query to retrieve for a single property by an unique identifier.
/// Corresponds to the need to view the details of a property. 
/// </summary>
public record GetPropertyByIdQuery(PropertyId PropertyId, ClientIdentity Owner);