using Microsoft.EntityFrameworkCore;
using SensorMonitoring.Api.Models;
using SensorMonitoring.Infrastructure.Data;

namespace SensorMonitoring.Api.Services;

public class SensorCacheService
{
    private readonly SensorIdCache _cache;

    public SensorCacheService(SensorIdCache cache)
    {
        _cache = cache;
    }

    public async Task<SensorCacheItem?> GetAsync(
        string deviceId,
        MonitoringDbContext dbContext,
        CancellationToken cancellationToken)
    {
        // 1. 캐시 우선
        if (_cache.TryGet(deviceId, out var cached))
        {
            return cached;
        }

        // 2. 없으면 DB 조회
        var sensor = await dbContext.Sensors
            .AsNoTracking()
            .Include(x => x.AlarmSetting)
            .SingleOrDefaultAsync(
                x => x.DeviceId == deviceId,
                cancellationToken);

        if (sensor is null)
            return null;

        // 3. 캐시 모델 변환
        var item = new SensorCacheItem
        {
            SensorId = sensor.Id,

            SensorAlarmSetting = sensor.AlarmSetting
        };

        _cache.Set(deviceId, item);

        return item;
    }
}
