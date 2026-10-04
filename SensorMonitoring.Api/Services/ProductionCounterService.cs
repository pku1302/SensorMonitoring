using Microsoft.EntityFrameworkCore;
using SensorMonitoring.Infrastructure.Data;

namespace SensorMonitoring.Api.Services;

public class ProductionCounterService
{
    private readonly Dictionary<int, long>
        _totals = new();

    public async Task<long> AddProductionAsync(
        int sensorId,
        long producedQuantity,
        MonitoringDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (!_totals.TryGetValue(
                sensorId,
                out var total))
        {
            total = await dbContext.SensorReadings
                .Where(x => x.SensorId == sensorId)
                .OrderByDescending(x => x.RecordedAt)
                .Select(x => x.TotalProduction)
                .FirstOrDefaultAsync(cancellationToken);
        }

        total += producedQuantity;

        _totals[sensorId] = total;

        return total;
    }
}
