using Hampcoders.Electrolink.API.Planning.Domain.Model.Events;
using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Planning.Domain.Model.Aggregates;

public class MaintenanceSchedule : BaseAggregateRoot
{
    public ScheduleId Id { get; private set; }
    public CompanyId CompanyId { get; private set; }
    public PropertyId PropertyId { get; private set; }
    public MaintenanceFrequency Frequency { get; private set; }
    public DateTime NextVisitDate { get; private set; }
    public StaffMemberId? AssignedStaffMemberId { get; private set; }
    public DateTime? LastVisitDate { get; private set; }
    public bool IsActive { get; private set; }

    private MaintenanceSchedule() { }

    public static MaintenanceSchedule Create(
        string companyId,
        string propertyId,
        MaintenanceFrequency frequency,
        DateTime nextVisitDate)
    {
        var schedule = new MaintenanceSchedule
        {
            Id = ScheduleId.NewId(),
            CompanyId = CompanyId.From(companyId),
            PropertyId = PropertyId.From(propertyId),
            Frequency = frequency,
            NextVisitDate = nextVisitDate,
            IsActive = true,
        };

        schedule.RaiseDomainEvent(new PreventiveMaintenanceScheduledEvent(
            schedule.Id.Value, companyId, propertyId, frequency.ToString(), nextVisitDate, DateTime.UtcNow));

        return schedule;
    }

    public void AssignStaffMember(string staffMemberId)
    {
        AssignedStaffMemberId = StaffMemberId.From(staffMemberId);
    }

    public void CompleteVisit(DateTime completedAt)
    {
        LastVisitDate = completedAt;
        NextVisitDate = Frequency switch
        {
            MaintenanceFrequency.Monthly => completedAt.AddMonths(1),
            MaintenanceFrequency.Quarterly => completedAt.AddMonths(3),
            MaintenanceFrequency.SemiAnnual => completedAt.AddMonths(6),
            MaintenanceFrequency.Annual => completedAt.AddYears(1),
            _ => completedAt.AddMonths(1)
        };

        RaiseDomainEvent(new PreventiveVisitCompletedEvent(
            Id.Value, CompanyId.Value, PropertyId.Value, completedAt, NextVisitDate, DateTime.UtcNow));
    }

    public void Pause()
    {
        if (!IsActive) return;
        IsActive = false;
    }

    public void Resume()
    {
        if (IsActive) return;
        IsActive = true;
    }
}
