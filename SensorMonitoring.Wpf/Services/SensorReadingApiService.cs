using SensorMonitoring.Contracts;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;

namespace SensorMonitoring.Wpf.Services;

public class SensorReadingApiService 
    : ISensorReadingApiService
{
    private readonly HttpClient _httpClient;

    public SensorReadingApiService(
        HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<SensorReadingDto>> GetRecentAsync(
        string deviceId,
        CancellationToken cancellationToken = default)
    {
        var result =
            await _httpClient.GetFromJsonAsync<
                List<SensorReadingDto>>(
                $"api/sensors/{deviceId}/readings/recent",
                cancellationToken);

        return result ?? [];
    }
}
