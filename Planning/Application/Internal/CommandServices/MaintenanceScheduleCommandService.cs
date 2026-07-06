using Hampcoders.Electrolink.API.Planning.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Planning.Domain.Repositories;
using Hampcoders.Electrolink.API.Planning.Domain.Services;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;
using MediatR;

namespace Hampcoders.Electrolink.API.Planning.Application.Internal.CommandServices;

public class MaintenanceScheduleCommandService(
    IMaintenanceScheduleRepository scheduleRepository,
    IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<MaintenanceScheduleCommandService> logger)
    : IMaintenanceScheduleCommandService
{
    public async Task Handle(SchedulePreventiveMaintenanceCommand command)
    {
        var schedule = MaintenanceSchedule.Create(
            command.CompanyId,
            command.PropertyId,
            command.Frequency,
            command.NextVisitDate);

        await scheduleRepository.AddAsync(schedule);
        await unitOfWork.CompleteAsync();

        foreach (var domainEvent in schedule.DomainEvents)
            await mediator.Publish(domainEvent, CancellationToken.None);
        schedule.ClearDomainEvents();

        logger.LogInformation("[Planning] Preventive maintenance scheduled for Company {CompanyId}, Property {PropertyId}, frequency {Frequency}",
            command.CompanyId, command.PropertyId, command.Frequency);
    }

    public async Task Handle(CompletePreventiveVisitCommand command)
    {
        var schedule = await scheduleRepository.FindByIdAsync(ScheduleId.From(command.ScheduleId))
            ?? throw new ArgumentException($"Schedule {command.ScheduleId} not found.");

        schedule.CompleteVisit(command.CompletedAt);
        await unitOfWork.CompleteAsync();

        foreach (var domainEvent in schedule.DomainEvents)
            await mediator.Publish(domainEvent, CancellationToken.None);
        schedule.ClearDomainEvents();

        logger.LogInformation("[Planning] Preventive visit completed for schedule {ScheduleId}", command.ScheduleId);
    }
}
