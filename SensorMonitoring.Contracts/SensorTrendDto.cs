using SensorMonitoring.Contractsl;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Contracts;

public class SensorTrendDto
{
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;

    public double MinValue { get; set; }
    public double MaxValue { get; set; }
    public double DangerValue { get; set; }

    public List<ChartData> Values { get; set; } = [];
}
