using Hampcoders.Electrolink.API.Processing.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Processing.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Processing.Domain.Repositories;
using Hampcoders.Electrolink.API.Processing.Infrastructure.Persistence.EFC.Configurations;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hampcoders.Electrolink.API.Processing.Infrastructure.Persistence.EFC.Repositories;

public class DeviceReadingStreamRepository(AppDbContext context)
    : BaseRepository<DeviceReadingStream, StreamId>(context), IDeviceReadingStreamRepository
{
    public async Task<DeviceReadingStream?> FindByDeviceIdAsync(string deviceId)
        => await Context.Set<DeviceReadingStream>()
            .Include(s => s.Readings.OrderByDescending(r => r.Timestamp).Take(120))
            .FirstOrDefaultAsync(s => s.DeviceId == DeviceId.From(deviceId));

    public async Task<DeviceReadingStream?> FindByStreamIdAsync(string streamId)
        => await Context.Set<DeviceReadingStream>()
            .Include(s => s.Readings.OrderByDescending(r => r.Timestamp).Take(120))
            .FirstOrDefaultAsync(s => s.StreamId == StreamId.From(streamId));

    public async Task<IEnumerable<DeviceReadingStream>> FindActiveStreamsAsync()
        => await Context.Set<DeviceReadingStream>()
            .Where(s => s.StreamStatus == EStreamStatus.Active)
            .AsNoTracking()
            .ToListAsync();

    public async Task<IEnumerable<DeviceReadingStream>> FindByOwnerAsync(ClientIdentity owner)
        => await Context.Set<DeviceReadingStream>()
            .Include(s => s.Readings.OrderByDescending(r => r.Timestamp).Take(120))
            .Where(s => s.Owner.ClientType == owner.ClientType && s.Owner.ClientId == owner.ClientId)
            .ToListAsync();

    public async Task<IEnumerable<DeviceReadingStream>> FindStreamsExceedingThresholdAsync(DateTime lastSeenBefore)
        => await Context.Set<DeviceReadingStream>()
            .Where(s => s.StreamStatus != EStreamStatus.Inactive
                     && s.LastReceivedAt < lastSeenBefore)
            .AsNoTracking()
            .ToListAsync();

    public async Task<DeviceReadingStream?> FindByPropertyIdAsync(string propertyId)
        => await Context.Set<DeviceReadingStream>()
            .Include(s => s.Readings.OrderByDescending(r => r.Timestamp).Take(120))
            .FirstOrDefaultAsync(s => s.PropertyId == PropertyId.From(propertyId));

    public async Task UpdateAsync(DeviceReadingStream stream)
    {
        Context.Set<DeviceReadingStream>().Update(stream);
    }
}
