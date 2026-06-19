using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Planning.Domain.Services;

public interface IContextSnapshotService
{
    Task<IoTContextSnapshot?> CaptureAsync(string propertyId);
}
