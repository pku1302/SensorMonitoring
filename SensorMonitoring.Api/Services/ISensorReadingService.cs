using SensorMonitoring.Contracts;

namespace SensorMonitoring.Api.Services;

public interface ISensorReadingService
{
    Task<List<SensorReadingDto>?> GetRecentAsync(
        string deviceId,
        CancellationToken cancellationToken);
}
