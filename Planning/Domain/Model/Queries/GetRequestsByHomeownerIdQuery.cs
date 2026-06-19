using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Planning.Domain.Model.Queries;

public record GetRequestsByClientIdQuery(ClientIdentity Client);