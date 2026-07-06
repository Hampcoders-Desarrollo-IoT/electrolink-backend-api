using Hampcoders.Electrolink.API.Planning.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Planning.Domain.Repositories;
using Hampcoders.Electrolink.API.Planning.Domain.Services;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;

namespace Hampcoders.Electrolink.API.Planning.Infrastructure.BackgroundServices;

public class PreventiveMaintenanceJob(
    IServiceProvider serviceProvider,
    ILogger<PreventiveMaintenanceJob> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("[Planning] PreventiveMaintenanceJob started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessDueSchedulesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[Planning] Error processing preventive maintenance schedules.");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task ProcessDueSchedulesAsync(CancellationToken ct)
    {
        using var scope = serviceProvider.CreateScope();
        var scheduleRepository = scope.ServiceProvider.GetRequiredService<IMaintenanceScheduleRepository>();
        var requestCommandService = scope.ServiceProvider.GetRequiredService<IServiceRequestCommandService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var dueSchedules = await scheduleRepository.FindDueSchedulesAsync(DateTime.UtcNow);

        foreach (var schedule in dueSchedules)
        {
            logger.LogInformation("[Planning] Processing due schedule {ScheduleId} for Company {CompanyId}",
                schedule.Id.Value, schedule.CompanyId.Value);

            schedule.Pause();
            scheduleRepository.Update(schedule);
            await unitOfWork.CompleteAsync();
        }
    }
}
