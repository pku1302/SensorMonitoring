using SensorMonitoring.Api.Models;
using SensorMonitoring.Contracts;
using SensorMonitoring.Domain.Entities;

namespace SensorMonitoring.Api.Services;

public class SensorStatusService
{
    private const double DefaultTemperatureWarning = 30.0;
    private const double DefaultTemperatureDanger = 35.0;

    private const double DefaultHumidityWarning = 70.0;
    private const double DefaultHumidityDanger = 80.0;

    public SensorStatus Evaluate(
        double temperature,
        double humidity,
        SensorAlarmSetting? setting)
    {
        if (setting is not null && !setting.IsEnabled)
        {
            return SensorStatus.Normal;
        }

        var temperatureWarning =
            setting?.TemperatureWarning
            ?? DefaultTemperatureWarning;

        var temperatureDanger =
            setting?.TemperatureDanger
            ?? DefaultTemperatureDanger;

        var humidityWarning =
            setting?.HumidityWarning
            ?? DefaultHumidityWarning;

        var humidityDanger =
            setting?.HumidityDanger
            ?? DefaultHumidityDanger;

        if (temperature >= temperatureDanger ||
            humidity >= humidityDanger)
        {
            return SensorStatus.Danger;
        }

        if (temperature >= temperatureWarning ||
            humidity >= humidityWarning)
        {
            return SensorStatus.Warning;
        }

        return SensorStatus.Normal;
    }
}
