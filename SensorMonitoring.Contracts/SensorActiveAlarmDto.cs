using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Contracts;

public class SensorActiveAlarmDto
{
    public long Id { get; set; }
    public string DeviceId { get; set; } = "";
    public SensorStatusDto Status { get; set; }
    public string Message { get; set; } = "";
    public DateTime OccurredAt { get; set; }
}
