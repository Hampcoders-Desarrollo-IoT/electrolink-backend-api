using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Monitoring.Domain.Model.Queries;

/// <summary>
/// Query to retrieve service execution history for a specific client.
/// </summary>
public record GetServiceHistoryByClientQuery(ClientIdentity Owner);


