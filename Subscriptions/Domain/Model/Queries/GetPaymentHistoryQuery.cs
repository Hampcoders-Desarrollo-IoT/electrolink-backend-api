namespace Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Queries;

public record GetPaymentHistoryQuery(string ProfileId, int Page = 1, int PageSize = 20);
