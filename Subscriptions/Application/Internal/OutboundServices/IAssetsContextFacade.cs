namespace Hampcoders.Electrolink.API.Subscriptions.Application.Internal.OutboundServices;

public interface IAssetsContextFacade
{
    Task<int> GetActiveDeviceCountAsync(string clientId);
}
