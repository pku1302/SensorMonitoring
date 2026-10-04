using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Domain.Entities;

public class SensorAlarmHistory
{
    public long Id { get; set; }
    public int SensorId { get; set; }
    public Sensor Sensor { get; set; } = null!;
    public string AlarmCode { get; set; } = string.Empty;
    public string AlarmMessage { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public DateTime? CleareaAt { get; set; }

}
