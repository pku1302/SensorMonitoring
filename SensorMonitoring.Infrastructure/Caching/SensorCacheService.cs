using Microsoft.EntityFrameworkCore;
using SensorMonitoring.Application.Abstractions;
using SensorMonitoring.Application.Models;
using SensorMonitoring.Domain.Entities;
using SensorMonitoring.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Infrastructure.Caching;

public class SensorCacheService : ISensorCacheService
{
    private readonly SensorIdCache _cache;
    private readonly IDbContextFactory<MonitoringDbContext> _dbContextFactory;

    public SensorCacheService(
        SensorIdCache cache,
        IDbContextFactory<MonitoringDbContext> dbContextFactory)
    {
        _cache = cache;
        _dbContextFactory = dbContextFactory;
    }

    public async Task<SensorCacheItem?> GetAsync(
        string deviceId,
        long producedQuantity,
        CancellationToken cancellationToken)
    {
        // 1. 캐시 우선
        if (_cache.TryGet(deviceId, out var cached))
        {
            return cached;
        }

        // 2. 없으면 DB 조회
        await using var dbContext =
            await _dbContextFactory.CreateDbContextAsync(
                cancellationToken);

        var sensor = await dbContext.Sensors
            .AsNoTracking()
            .Include(x => x.AlarmSetting)
            .SingleOrDefaultAsync(
                x => x.DeviceId == deviceId,
                cancellationToken);

        // 3. DB에도 없다면 신규 센서 등록
        if (sensor is null)
        {
            sensor = new Sensor
            {
                DeviceId = deviceId,
                Name = deviceId,
                IsEnabled = true
            };

            dbContext.Sensors.Add(sensor);

            try
            {
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                // INSERT 충돌 후 추적 중인 신규 엔티티 제거
                dbContext.Entry(sensor).State = EntityState.Detached;

                // 다른 요청이 등록한 센서인지 다시 확인
                sensor = await dbContext.Sensors
                    .AsNoTracking()
                    .Include(x => x.AlarmSetting)
                    .SingleOrDefaultAsync(
                        x => x.DeviceId == deviceId,
                        cancellationToken);

                if (sensor is null)
                    throw;
            }
        }

        // 4. 기존 누적 생산량 조회
        var total = await dbContext.SensorReadings
            .AsNoTracking()
            .Where(x => x.Sensor.DeviceId == deviceId)
            .OrderByDescending(x => x.RecordedAt)
            .Select(x => x.TotalProduction)
            .FirstOrDefaultAsync(cancellationToken);

        // 5. 캐시 모델 변환
        var item = new SensorCacheItem
        {
            SensorId = sensor.Id,

            DeviceId = sensor.DeviceId,

            SensorAlarmSetting = sensor.AlarmSetting,

            TotalProduction = total
        };

        _cache.Set(deviceId, item);

        return item;
    }
}