using SensorMonitoring.Domain.Entities;

namespace SensorMonitoring.Application.Models;

public class SensorCacheItem
{
    public int SensorId { get; set; }
    public string DeviceId { get; set; } = "";
    public SensorAlarmSetting? SensorAlarmSetting { get; set; }
    public long TotalProduction { get; set; }
}
