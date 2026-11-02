using SensorMonitoring.Application.Abstractions;
using SensorMonitoring.Contracts;
using SensorMonitoring.Domain.Entities;
using SensorMonitoring.Domain.Enums;

namespace SensorMonitoring.Application.Services;

public class SensorDataProcessingService
{
    private readonly ISensorCacheService _sensorCacheService;
    private readonly ISensorReadingRepository _readingRepository;
    private readonly ISensorAlarmService _alarmService;
    private readonly ISensorRealtimeNotifier _notifier;
    private readonly ISensorChartCache _chartCache;

    public SensorDataProcessingService(
        ISensorCacheService sensorCacheService,
        ISensorReadingRepository readingRepository,
        ISensorAlarmService alarmService,
        ISensorRealtimeNotifier notifier,
        ISensorChartCache chartCache)
    {
        _sensorCacheService = sensorCacheService;
        _readingRepository = readingRepository;
        _alarmService = alarmService;
        _notifier = notifier;
        _chartCache = chartCache;
    }

    public async Task ProcessAsync(
        SensorPacket packet,
        CancellationToken cancellationToken)
    {
        // 1. 센서 정보 조회
        var sensorInfo = await _sensorCacheService.GetAsync(
            packet.DeviceId,
            packet.ProducedQuantity,
            cancellationToken);

        if (sensorInfo is null)
            return;

        // 2. 위험 상태 계산
        var status = SensorStatusEvaluator.Evaluate(
            packet.Temperature,
            packet.Humidity,
            sensorInfo.SensorAlarmSetting);

        // 3. 측정 데이터 생성
        var reading = new SensorReading
        {
            SensorId = sensorInfo.SensorId,
            Temperature = packet.Temperature,
            Humidity = packet.Humidity,
            TotalProduction =
                sensorInfo.TotalProduction + packet.ProducedQuantity,
            RecordedAt = packet.Timestamp
        };

        // 4. 저장
        await _readingRepository.AddAsync(
            reading,
            cancellationToken);

        // 5. 생산량 캐시 갱신 필요
        // 기존 캐시 갱신 메서드가 있다면 여기서 호출

        // 6. 실시간 차트 캐시 갱신
        _chartCache.Add(packet);

        // 7. 알람 상태 변경 확인
        var alarmChange = _alarmService.Update(
            packet,
            status,
            sensorInfo.SensorAlarmSetting);

        // 8. SignalR 전송 DTO
        var realtimeDto = new SensorRealTimeDto
        {
            DeviceId = packet.DeviceId,
            Temperature = packet.Temperature,
            Humidity = packet.Humidity,
            Status = ToDto(status),
            Timestamp = packet.Timestamp
        };

        await _notifier.SendSensorUpdatedAsync(
            realtimeDto,
            cancellationToken);

        if (alarmChange is not null)
        {
            await _notifier.SendAlarmChangedAsync(
                alarmChange,
                cancellationToken);
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
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };
    }
}
