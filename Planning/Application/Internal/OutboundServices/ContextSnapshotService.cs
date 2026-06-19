using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Planning.Domain.Services;
using Hampcoders.Electrolink.API.Processing.Interfaces.ACL;

namespace Hampcoders.Electrolink.API.Planning.Application.Internal.OutboundServices;

public class ContextSnapshotService(IProcessingContextFacade processing)
    : IContextSnapshotService
{
    public async Task<IoTContextSnapshot?> CaptureAsync(string propertyId)
    {
        var dto = await processing.GetIoTContextSnapshotAsync(propertyId);
        if (dto is null) return null;

        return new IoTContextSnapshot(
            DeviceId: dto.DeviceId,
            LatestReadings: dto.RecentReadings.Select(r => new DeviceReading(
                Timestamp: r.Timestamp,
                Voltage: (decimal)r.Voltage,
                Current: (decimal)r.Current,
                PowerFactor: (decimal)r.PowerFactor,
                Frequency: (decimal)r.Frequency)).ToList(),
            ActiveAnomalies: dto.ActiveAnomalies.Select(a => new ActiveAnomaly(
                AnomalyId: a.AnomalyId,
                AnomalyType: a.AnomalyType,
                Severity: a.Severity,
                DetectedAt: a.DetectedAt)).ToList(),
            CapturedAt: dto.CapturedAt);
    }
}
