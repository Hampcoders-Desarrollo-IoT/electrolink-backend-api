using Hampcoders.Electrolink.API.Planning.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Planning.Interfaces.REST.Resources;

namespace Hampcoders.Electrolink.API.Planning.Interfaces.REST.Transform;

public static class ServiceSuggestionResourceFromEntityAssembler
{
    public static ServiceSuggestionResource ToResource(ServiceSuggestion entity)
        => new ServiceSuggestionResource(
            entity.SuggestionId,
            entity.PropertyId,
            entity.AnomalyType,
            entity.Severity,
            entity.Status.ToString(),
            entity.ExpiresAt,
            entity.ViewedAt);
}
