using Hampcoders.Electrolink.API.Assets.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Processing.Application.Internal.OutboundServices;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Hampcoders.Electrolink.API.Processing.Infrastructure.Services;

public class ExternalAssetsService(AppDbContext context) : IAssetsContextFacade
{
    public async Task<DeviceInfoDto?> GetInstalledDeviceInfoAsync(string deviceId)
    {
        var device = await context.IoTDevices
            .Where(d => d.SerialNumber.Value == deviceId && d.Status == EDeviceStatus.Installed)
            .FirstOrDefaultAsync();

        if (device is null) return null;

        return new DeviceInfoDto(
            DeviceId: device.Id.Value,
            Status: device.Status.ToString(),
            PropertyId: device.AssignedPropertyId?.Value ?? string.Empty,
            OwnerId: string.Empty);
    }

    public async Task<bool> ValidateApiKeyAsync(string deviceId, string apiKeyHash)
    {
        var device = await context.IoTDevices
            .Where(d => d.SerialNumber.Value == deviceId)
            .FirstOrDefaultAsync();

        if (device is null) return false;

        return device.ApiKeyHash.Verify(apiKeyHash);
    }
}
