using Hampcoders.Electrolink.API.Assets.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Assets.Domain.Services;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.Events;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using MediatR;

namespace Hampcoders.Electrolink.API.Assets.Application.Internal.EventHandlers;

public class ProfileCompletedEventHandler(
    IPropertyPortfolioCommandService portfolioCommandService,
    ITechnicianInventoryCommandService inventoryCommandService, 
    ILogger<ProfileCompletedEventHandler> logger)
    : INotificationHandler<ProfileCompletedEvent>
{
    private async Task HandleTechnician(ProfileCompletedEvent notification)
    {
        logger.LogInformation("[Assets BC] ProfileCompleted → Creating inventory for technician {Id}", notification.SubjectId);

        try {
            await inventoryCommandService.Handle(new CreateTechnicianInventoryCommand(TechnicianId.From(notification.SubjectId.ToString()!)));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
            logger.LogWarning(
                "[Assets BC] Inventory already exists for technician {Id}. Skipping.",
                notification.SubjectId);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "[Assets BC] Failed to create inventory for technician {Id}.",
                notification.SubjectId);
        }
    }

    private async Task HandleClient(ProfileCompletedEvent notification, ClientIdentity client)
    {
        logger.LogInformation(
            "[Assets BC] ProfileCompleted → Creating portfolio for client {Id}",
            notification.SubjectId);

        try
        {
            await portfolioCommandService.Handle(
                new CreatePropertyPortfolioCommand(client));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
            logger.LogWarning(
                "[Assets BC] Portfolio already exists for client {Id}. Skipping.",
                notification.SubjectId);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "[Assets BC] Failed to create portfolio for client {Id}.",
                notification.SubjectId);
        }
    }

    public async Task Handle(ProfileCompletedEvent notification, CancellationToken cancellationToken)
    {
        switch (notification.BusinessRole)
        {
            case EBusinessRole.Technician:
                await HandleTechnician(notification);
                break;

            case EBusinessRole.HomeOwner:
                await HandleClient(notification, ClientIdentity.FromHomeowner(notification.SubjectId.ToString()!));
                break;

            case EBusinessRole.Company:
                await HandleClient(notification, ClientIdentity.FromCompany(notification.SubjectId.ToString()!));
                break;

            default:
                logger.LogWarning(
                    "[Assets BC] ProfileCompleted with unsupported role {Role} for subject {Id}",
                    notification.BusinessRole,
                    notification.SubjectId);
                break;
        }
    }
}