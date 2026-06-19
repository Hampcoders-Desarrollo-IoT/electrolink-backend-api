using Hampcoders.Electrolink.API.Assets.Domain.Model.Events;
using Hampcoders.Electrolink.API.Processing.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Processing.Domain.Repositories;
using Hampcoders.Electrolink.API.Processing.Application.Internal.OutboundServices;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;
using MediatR;

namespace Hampcoders.Electrolink.API.Processing.Application.Internal.EventHandlers;

public class DeviceInstalledEventHandler(
    IDeviceReadingStreamRepository streamRepository,
    IAssetsContextFacade           assetsFacade,
    IUnitOfWork                    unitOfWork,
    ILogger<DeviceInstalledEventHandler> logger)
    : INotificationHandler<DeviceInstalledEvent>
{
    public async Task Handle(DeviceInstalledEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[IoT] DeviceInstalled — DeviceId: {DeviceId} | PropertyId: {PropertyId}",
            notification.DeviceId, notification.PropertyId);

        var existing = await streamRepository.FindByDeviceIdAsync(notification.DeviceId);
        if (existing is not null)
        {
            existing.Resume();
            await streamRepository.UpdateAsync(existing);
        }
        else
        {
            // Obtener informacion del dispositivo para saber el HomeownerId
            var deviceInfo = await assetsFacade.GetInstalledDeviceInfoAsync(notification.DeviceId);
            if (deviceInfo is null) return;

            var stream = DeviceReadingStream.Create(
                DeviceId.From(notification.DeviceId),
                PropertyId.From(notification.PropertyId),
                ClientIdentity.FromHomeowner(deviceInfo.OwnerId));

            await streamRepository.AddAsync(stream);
        }

        await unitOfWork.CompleteAsync();
        logger.LogInformation("[IoT] DeviceReadingStream activated for device {DeviceId}.", notification.DeviceId);
    }
}
