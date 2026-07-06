using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Planning.Domain.Model.Commands;

public record SchedulePreventiveMaintenanceCommand(
    string CompanyId,
    string PropertyId,
    MaintenanceFrequency Frequency,
    DateTime NextVisitDate
);
