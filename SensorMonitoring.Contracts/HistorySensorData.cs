using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Contracts;

public class HistoryChartData : ChartData
{
    public double MinValue { get; set; }
    public double MaxValue { get; set; }
}
