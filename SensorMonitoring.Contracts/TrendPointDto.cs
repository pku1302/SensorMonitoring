using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Contractsl;

public class TrendPointDto
{
    public DateTime Timestamp { get; set; }
    public double Value { get; set; }

    // 이력 차트에서만 사용
    public double? Minimum { get; set; }
    public double? Maximum { get; set; }
}
