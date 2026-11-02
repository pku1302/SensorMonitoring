using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Contracts;

public class SensorChartsDto
{
    public string DeviceId { get; set; } = "";
    public List<SensorTrendDto> Trends { get; set; } = [];
}
