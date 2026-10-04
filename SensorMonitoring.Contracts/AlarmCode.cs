using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Contracts;

public enum AlarmCode
{
    CommunicationLost,
    TemperatureWarning,
    TemperatureDanger,
    HumidityWarning,
    HumidityDanger
}
