using Hampcoders.Electrolink.API.Monitoring.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Monitoring.Domain.Model.Commands;

public record SubmitClientReviewCommand(
    ServiceExecutionId ExecutionId,
    ClientIdentity ReviewerId,
    int Rating,
    string? Comment,
    Dictionary<EEvaluationCategory, int> Categories,
    DateTime SubmittedAt
);
