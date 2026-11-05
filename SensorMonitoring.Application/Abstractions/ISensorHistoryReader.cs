using SensorMonitoring.Application.Models;
using SensorMonitoring.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Application.Abstractions;

public interface ISensorHistoryReader
{
    Task<List<SensorListItemDto>> GetSensorsAsync(
        DateTime startUtc,
        DateTime endUtc,
        CancellationToken ct);

    Task<List<SensorReadingDto>?> GetReadingsAsync(
        string deviceId,
        DateTime startUtc,
        DateTime endUtc,
        CancellationToken ct);

    Task<SensorHistoryChartDto?> GetChartAsync(
        string deviceId,
        DateTime startUtc,
        DateTime endUtc,
        CancellationToken ct);

    Task<List<SensorHistoryReadingData>> GetHourlyReadingAsync(
        DateTime startUtc,
        DateTime endUtc,
        string? deviceId,
        CancellationToken cancellationToken);
}
