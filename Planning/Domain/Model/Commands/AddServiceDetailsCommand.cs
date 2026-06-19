using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Planning.Domain.Model.Commands;

public record AddServiceDetailsCommand(
    RequestId RequestId,
    ClientIdentity Client,
    string ProblemDescription,
    decimal ConsumptionKwh,
    decimal AmountPaid,
    string AmountCurrency,
    string BillingPeriod,
    string ReceiptNumber,
    bool IsPriority,
    IReadOnlyList<string> PreferredDates,
    string TimePreference
    );