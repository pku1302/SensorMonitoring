using SensorMonitoring.Application.Abstractions;
using SensorMonitoring.Contracts;
using SensorMonitoring.Domain.Entities;
using SensorMonitoring.Domain.Enums;
using System.Collections.Concurrent;

namespace SensorMonitoring.Api.Services;

public class SensorAlarmService : ISensorAlarmService
{
    private readonly ConcurrentDictionary<string, SensorAlarmChangedDto>
        _activeAlarms = new();

    private readonly object _lock = new();

    public SensorAlarmChangedDto? Update(
        SensorPacket packet,
        SensorStatus newStatus,
        SensorAlarmSetting? setting)
    {
        lock (_lock)
        {
            string deviceId = packet.DeviceId;

            _activeAlarms.TryGetValue(deviceId, out var existing);

            // Domain 상태를 Contracts 상태로 변환
            SensorStatusDto dtoStatus = ToDto(newStatus);

            // 정상 상태 -> 기존 알람 제거
            if (newStatus == SensorStatus.Normal)
            {
                if (existing is null)
                    return null;

                _activeAlarms.TryRemove(deviceId, out _);

                return new SensorAlarmChangedDto
                {
                    DeviceId = deviceId,
                    Status = SensorStatusDto.Normal,
                    IsActive = false,
                    OccurredAt = DateTime.UtcNow
                };
            }

            // 기존 상태와 동일하면 이벤트 발생시키지 않음
            if (existing?.Status == dtoStatus)
                return null;

            // 새 알람 생성 또는 기존 알람 교체
            var alarm = new SensorAlarmChangedDto
            {
                DeviceId = deviceId,
                Status = dtoStatus,
                Message = GetAlarmMessage(newStatus, packet, setting),
                OccurredAt = DateTime.UtcNow,
                IsActive = true
            };

            _activeAlarms[deviceId] = alarm;

            return alarm;
        }
    }

    private static SensorStatusDto ToDto(SensorStatus status)
    {
        return status switch
        {
            SensorStatus.Normal => SensorStatusDto.Normal,
            SensorStatus.Warning => SensorStatusDto.Warning,
            SensorStatus.Danger => SensorStatusDto.Danger,
            SensorStatus.Offline => SensorStatusDto.Offline,

            _ => throw new ArgumentOutOfRangeException(
                nameof(status))
        };
    }

    public IReadOnlyList<SensorAlarmChangedDto> GetActive()
    {
        lock (_lock)
        {
            return _activeAlarms.Values.ToArray();
        }
    }
    private static string GetAlarmMessage(
        SensorStatus status,
        SensorPacket packet,
        SensorAlarmSetting? setting)
    {
        if (status == SensorStatus.Offline)
            return "센서 연결 끊김";

        if (status == SensorStatus.Normal)
            return "";

        double temperatureLimit = status == SensorStatus.Danger
            ? setting?.TemperatureDanger ?? 35.0
            : setting?.TemperatureWarning ?? 30.0;

        double humidityLimit = status == SensorStatus.Danger
            ? setting?.HumidityDanger ?? 80.0
            : setting?.HumidityWarning ?? 70.0;

        string level = status == SensorStatus.Danger
            ? "위험"
            : "주의";

        if (packet.Temperature >= temperatureLimit)
        {
            return $"{packet.Temperature:F1} °C ·" +
                $"{level} 기준 {temperatureLimit:F1} °C 초과";
        }

        if (packet.Humidity >= humidityLimit)
        {
            return $"{packet.Humidity:F1} % · " +
                $"{level} 기준 {humidityLimit:F1} % 초과";
        }

        return "";
    }

}
