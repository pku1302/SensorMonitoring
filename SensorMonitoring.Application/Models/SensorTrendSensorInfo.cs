using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Application.Models;

public class SensorTrendSensorInfo
{
    public string DeviceId { get; set; } = "";

    public double TemperatureDanger { get; set; }
    public double HumidityDanger { get; set; }

}
