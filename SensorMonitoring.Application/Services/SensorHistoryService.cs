using SensorMonitoring.Application.Abstractions;
using SensorMonitoring.Application.Common;
using SensorMonitoring.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

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
}
