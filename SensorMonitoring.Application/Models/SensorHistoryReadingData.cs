using SensorMonitoring.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Application.Models;

public class SensorHistoryReadingData
{
    public DateTime RecordedAt { get; set; }
    public string DeviceId { get; set; } = string.Empty;

    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public SensorAlarmSetting? AlarmSetting { get; set; }
}
