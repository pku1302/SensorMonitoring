using SensorMonitoring.Application.Abstractions;
using SensorMonitoring.Application.Common;
using SensorMonitoring.Application.Mappings;
using SensorMonitoring.Contracts;
using SensorMonitoring.Domain.Enums;


namespace SensorMonitoring.Application.Services;

public class SensorHistoryService
{
    private readonly ISensorHistoryReader _reader;

    public SensorHistoryService(ISensorHistoryReader reader)
    {
        _reader = reader;
    }

    public Task<List<SensorListItemDto>> GetSensorsByDateAsync(
        DateOnly date,
        CancellationToken ct = default)
    {
        var (start, end) = KoreaDateRange.ToUtc(date);

        return _reader.GetSensorsAsync(start, end, ct);
    }

    public Task<List<SensorReadingDto>?> GetReadingsByDateAsync(
        string deviceId,
        DateOnly date,
        CancellationToken ct = default)
    {
        var (start, end) = KoreaDateRange.ToUtc(date);

        return _reader.GetReadingsAsync(
            deviceId, start, end, ct);
    }
    public Task<SensorHistoryChartDto?> GetChartByDateAsync(
        string deviceId,
        DateOnly date,
        CancellationToken ct = default)
    {
        var (start, end) = KoreaDateRange.ToUtc(date);

        return _reader.GetChartAsync(
            deviceId, start, end, ct);
    }

    public async Task<List<SensorHistoryReadingDto>> GetHourlyReadingAsync(
        DateOnly date,
        int hour,
        string? deviceId,
        CancellationToken cancellationToken = default)
    {
        var (startUtc, endUtc) =
            KoreaDateRange.ToUtcHour(date, hour);

        var readings = await _reader.GetHourlyReadingAsync(
            startUtc,
            endUtc,
            deviceId,
            cancellationToken);

        return readings.Select(x =>
        {
            var status = SensorStatusEvaluator.Evaluate(
                x.Temperature,
                x.Humidity,
                x.AlarmSetting);

            return new SensorHistoryReadingDto
            {
                MeasuredAt = x.RecordedAt,
                SensorId = x.DeviceId,
                Temperature = x.Temperature,
                Humidity = x.Humidity,
                Status = SensorStatusMapper.ToDto(status)
            };
        }).ToList();
    }
}
