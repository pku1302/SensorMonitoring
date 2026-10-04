using SensorMonitoring.Domain.Entities;

namespace SensorMonitoring.Api.Models;

public class SensorCacheItem
{
    public int SensorId { get; init; }

    public SensorAlarmSetting? SensorAlarmSetting { get; init; }
}
