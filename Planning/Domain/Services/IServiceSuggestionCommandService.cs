using Hampcoders.Electrolink.API.Planning.Domain.Model.Commands;

namespace Hampcoders.Electrolink.API.Planning.Domain.Services;

public interface IServiceSuggestionCommandService
{
    Task<string> Handle(AcceptServiceSuggestionCommand command);
    Task Handle(DismissServiceSuggestionCommand command);
}
