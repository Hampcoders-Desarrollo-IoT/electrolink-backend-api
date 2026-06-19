namespace Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;

public enum ERequestStatus
{
    Draft,
    ReadyToConfirm,
    PendingAssignment,
    Assigned,
    Cancelled,
    CategorySelected,
    PropertySelected,
    Expired,
    PendingInstallation
}