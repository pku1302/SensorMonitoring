using SensorMonitoring.Domain.Entities;
using SensorMonitoring.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Domain.Services;

public class SensorStatusEvaluator
{
    public SensorStatus Evaluate(
        double temperature,
        double humidity,
        SensorAlarmSetting? setting)
    {
        double temperatureWarning =
            setting?.TemperatureWarning ?? 30;

        double temperatureDanger =
            setting?.TemperatureDanger ?? 35;

        double humidityWarning =
            setting?.HumidityWarning ?? 70;

        double humidityDanger =
            setting?.HumidityDanger ?? 80;

        if (temperature >= temperatureDanger ||
            humidity >= humidityDanger)
            return SensorStatus.Danger;

        if (temperature >= temperatureWarning ||
            humidity >= humidityWarning)
            return SensorStatus.Warning;

        return SensorStatus.Normal;
    }
}