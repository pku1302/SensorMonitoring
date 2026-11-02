using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Contracts;

public class SensorHistoryChartDto
{
    public double TemperatureDangerValue { get; set; }
    public double HumidityDangerValue { get; set; }

    public List<SensorHistoryChartPointDto> Points { get; set; } = [];
}
