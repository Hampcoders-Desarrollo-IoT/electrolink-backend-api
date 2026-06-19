namespace Hampcoders.Electrolink.API.Processing.Application.Internal.OutboundServices;

public interface IAssetsContextFacade
{
    Task<DeviceInfoDto?> GetInstalledDeviceInfoAsync(string deviceId);
    Task<bool> ValidateApiKeyAsync(string deviceId, string apiKeyHash);
}

public record DeviceInfoDto(string DeviceId, string Status, string PropertyId, string OwnerId);
