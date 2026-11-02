using SensorMonitoring.Application.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Infrastructure.Caching;

public class SensorIdCache
{
    private readonly ConcurrentDictionary<string, SensorCacheItem> _cache
    = new();

    public void Load(IEnumerable<SensorCacheItem> sensors)
    {
        _cache.Clear();

        foreach (var sensor in sensors)
        {
            _cache[sensor.DeviceId] = sensor;
        }
    }

    public bool TryGet(
        string deviceId,
        out SensorCacheItem item)
    {
        return _cache.TryGetValue(deviceId, out item!);
    }

    public void Set(
        string deviceId,
        SensorCacheItem item)
    {
        _cache[deviceId] = item;
    }
}
