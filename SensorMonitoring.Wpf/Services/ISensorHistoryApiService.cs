using SensorMonitoring.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Wpf.Services;

public interface ISensorHistoryApiService
{
    Task<List<SensorListItemDto>> GetSensorsByDateAsync(
       DateOnly date,
       CancellationToken cancellationToken = default);

    Task<SensorHistoryChartDto> GetChartByDateAsync(
        string deviceId,
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task<List<SensorHistoryReadingDto>> GetHourlyReadingsAsync(
         DateOnly date,
         int hour,
         string deviceId,
         CancellationToken cancellationToken = default);
}
