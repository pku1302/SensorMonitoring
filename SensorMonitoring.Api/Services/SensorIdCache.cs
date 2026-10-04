using SensorMonitoring.Api.Models;

namespace SensorMonitoring.Api.Services;

public class SensorIdCache
{
    private readonly Dictionary<string, SensorCacheItem> _cache = new();

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

    public void Remove(string deviceId)
    {
        _cache.Remove(deviceId);
    }
}
