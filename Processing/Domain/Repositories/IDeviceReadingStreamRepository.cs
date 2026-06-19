using Hampcoders.Electrolink.API.Processing.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Processing.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;

namespace Hampcoders.Electrolink.API.Processing.Domain.Repositories;

public interface IDeviceReadingStreamRepository : IBaseRepository<DeviceReadingStream, StreamId>
{
    Task<DeviceReadingStream?> FindByDeviceIdAsync(string deviceId);
    Task<DeviceReadingStream?> FindByStreamIdAsync(string streamId);
    Task<IEnumerable<DeviceReadingStream>> FindActiveStreamsAsync();
    Task<IEnumerable<DeviceReadingStream>> FindByOwnerAsync(ClientIdentity owner);
    Task<IEnumerable<DeviceReadingStream>> FindStreamsExceedingThresholdAsync(DateTime lastSeenBefore);
    Task<DeviceReadingStream?> FindByPropertyIdAsync(string propertyId);
    Task UpdateAsync(DeviceReadingStream stream);
}
