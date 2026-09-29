using Microsoft.EntityFrameworkCore;
using SensorMonitoring.Infrastructure.Data;

namespace SensorMonitoring.Api.Services;

public class ProductionCounterService
{
    private readonly Dictionary<string, long>
        _totals = new();

    public async Task<long> AddProductionAsync(
        string deviceId,
        long producedQuantity,
        MonitoringDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (!_totals.TryGetValue(
                deviceId,
                out var total))
        {
            total = await dbContext.SensorReadings
                .Where(x => x.DeviceId == deviceId)
                .OrderByDescending(x => x.RecordedAt)
                .Select(x => x.TotalProduction)
                .FirstOrDefaultAsync(cancellationToken);
        }

        total += producedQuantity;

        _totals[deviceId] = total;

        return total;
    }
}
