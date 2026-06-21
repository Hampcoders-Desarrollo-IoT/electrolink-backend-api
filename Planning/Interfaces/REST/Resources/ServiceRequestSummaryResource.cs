using System;
using System.Collections.Generic;
namespace Hampcoders.Electrolink.API.Planning.Interfaces.REST.Resources;
/*public record ServiceRequestSummaryResource(
    string RequestId,
    string HomeownerId,
    string Status,
    bool IsPriority,
    string? PropertyId,
    double? PropertyLatitude,
    double? PropertyLongitude,
    string? SelectedRecipeId,
    string? SelectedTechnicianId,
    string? AssignedServiceId,
    string? ProblemDescription,
    int? ConsumptionKwh,
    decimal? AmountPaid,
    string? AmountCurrency,
    string? BillingPeriod,
    string? ReceiptNumber,
    List<string>? PreferredDates,
    string? TimePreference,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);*/


public record ServiceRequestSummaryResource(
    string RequestId,
    string OwnerId,
    string? PropertyId,
    string? SelectedRecipeId,
    string? SelectedTechnicianId,
    string Status,
    bool IsPriority,
    string CreatedAt);
