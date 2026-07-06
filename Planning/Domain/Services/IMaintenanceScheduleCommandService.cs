using Hampcoders.Electrolink.API.Planning.Domain.Model.Commands;

namespace Hampcoders.Electrolink.API.Planning.Domain.Services;

public interface IMaintenanceScheduleCommandService
{
    Task Handle(SchedulePreventiveMaintenanceCommand command);
    Task Handle(CompletePreventiveVisitCommand command);
}
