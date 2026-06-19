using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Planning.Domain.Model.Commands;

public record DismissServiceSuggestionCommand(string SuggestionId, ClientIdentity Client);
