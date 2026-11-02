using SensorMonitoring.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Domain.Enums;

public class SensorStatusEvaluator
{
    public static SensorStatus Evaluate(
        double temperature,
        double humidity,
        SensorAlarmSetting? setting)
    {
        double dangerTemperature =
            setting?.TemperatureDanger ?? 35;

        double dangerHumidity =
            setting?.HumidityDanger ?? 80;

        if (temperature >= dangerTemperature ||
            humidity >= dangerHumidity)
        {
            return SensorStatus.Danger;
        }

        return SensorStatus.Normal;
    }
}
