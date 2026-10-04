using SensorMonitoring.Api.Models;
using SensorMonitoring.Api.Services;
using SensorMonitoring.Contracts;
using SensorMonitoring.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Tests.Services;

public class SensorStatusServiceTests
{
    private readonly SensorStatusService _service = new();

    [Fact]
    public void Evaluate_WhenNoSettingAndValuesAreNormal_ReturnsNormal()
    {
        var result = _service.Evaluate(
            temperature: 25,
            humidity: 50,
            setting: null);

        Assert.Equal(
            SensorStatus.Normal,
            result);
    }

    [Fact]
    public void Evaluate_WhenNoSettingAndTemperatureIsWarning_ReturnsWarning()
    {
        var result = _service.Evaluate(
            temperature: 30,
            humidity: 50,
            setting: null);

        Assert.Equal(
            SensorStatus.Warning,
            result);
    }

    [Fact]
    public void Evaluate_WhenNoSettingAndTemperatureIsDanger_ReturnsDanger()
    {
        var result = _service.Evaluate(
            temperature: 35,
            humidity: 50,
            setting: null);

        Assert.Equal(
            SensorStatus.Danger,
            result);
    }

    [Fact]
    public void Evaluate_WhenNoSettingAndHumidityIsWarning_ReturnsWarning()
    {
        var result = _service.Evaluate(
            temperature: 25,
            humidity: 70,
            setting: null);

        Assert.Equal(
            SensorStatus.Warning,
            result);
    }

    [Fact]
    public void Evaluate_WhenNoSettingAndHumidityIsDanger_ReturnsDanger()
    {
        var result = _service.Evaluate(
            temperature: 25,
            humidity: 80,
            setting: null);

        Assert.Equal(
            SensorStatus.Danger,
            result);
    }

    [Fact]
    public void Evaluate_WhenCustomSettingExists_UsesCustomWarningThreshold()
    {
        var setting = new SensorAlarmSetting
        {
            IsEnabled = true,

            TemperatureWarning = 28,
            TemperatureDanger = 32,

            HumidityWarning = 60,
            HumidityDanger = 75
        };

        var result = _service.Evaluate(
            temperature: 29,
            humidity: 50,
            setting: setting);

        Assert.Equal(
            SensorStatus.Warning,
            result);
    }

    [Fact]
    public void Evaluate_WhenCustomSettingExists_UsesCustomDangerThreshold()
    {
        var setting = new SensorAlarmSetting
        {
            IsEnabled = true,

            TemperatureWarning = 28,
            TemperatureDanger = 32,

            HumidityWarning = 60,
            HumidityDanger = 75
        };

        var result = _service.Evaluate(
            temperature: 33,
            humidity: 50,
            setting: setting);

        Assert.Equal(
            SensorStatus.Danger,
            result);
    }

    [Fact]
    public void Evaluate_WhenSettingDisabled_ReturnsNormal()
    {
        var setting = new SensorAlarmSetting
        {
            IsEnabled = false,

            TemperatureWarning = 10,
            TemperatureDanger = 20,

            HumidityWarning = 10,
            HumidityDanger = 20
        };

        var result = _service.Evaluate(
            temperature: 100,
            humidity: 100,
            setting: setting);

        Assert.Equal(
            SensorStatus.Normal,
            result);
    }

    [Fact]
    public void Evaluate_WhenOnlySomeCustomValuesExist_UsesDefaultForNullValues()
    {
        var setting = new SensorAlarmSetting
        {
            IsEnabled = true,

            TemperatureWarning = 28,
            TemperatureDanger = null,

            HumidityWarning = null,
            HumidityDanger = null
        };

        var result = _service.Evaluate(
            temperature: 34,
            humidity: 50,
            setting: setting);

        Assert.Equal(
            SensorStatus.Warning,
            result);
    }
}
