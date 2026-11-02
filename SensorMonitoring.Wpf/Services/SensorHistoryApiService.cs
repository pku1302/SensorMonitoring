using SensorMonitoring.Contracts;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;

namespace SensorMonitoring.Wpf.Services;

public class SensorHistoryApiService : ISensorHistoryApiService
{
    private readonly HttpClient _httpClient;

    public SensorHistoryApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<SensorListItemDto>> GetSensorsByDateAsync(
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        string url =
            $"api/sensors/with-readings?date={date:yyyy-MM-dd}";

        var result = await _httpClient.GetFromJsonAsync<
            List<SensorListItemDto>>(
                url,
                cancellationToken);

        return result ?? [];
    }

    public async Task<SensorHistoryChartDto>
        GetChartByDateAsync(
            string deviceId,
            DateOnly date,
            CancellationToken cancellationToken = default)
    {
        string url =
            $"api/sensors/{Uri.EscapeDataString(deviceId)}" +
            $"/history/chart?date={date:yyyy-MM-dd}";

        return await _httpClient
            .GetFromJsonAsync<SensorHistoryChartDto>(url,cancellationToken) 
                ?? new SensorHistoryChartDto();
    }

}
