using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Contracts;

public class ChartData
{
    public DateTime Time { get; set; }
    public double Value { get; set; }
    public long Sequence { get; set; }
}
