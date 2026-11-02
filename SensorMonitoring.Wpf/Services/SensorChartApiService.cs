using SensorMonitoring.Contracts;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;

namespace SensorMonitoring.Wpf.Services;

public class SensorChartApiService
    : ISensorChartApiService
{
    private readonly HttpClient _httpClient;

    public SensorChartApiService(
        HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<SensorTrendDto>> GetAsync(
        string deviceId,
        CancellationToken cancellationToken = default)
    {
        var result =
            await _httpClient.GetFromJsonAsync<
                List<SensorTrendDto>>(
                $"api/sensors/{deviceId}/chart",
                cancellationToken);

        return result ?? [];
    }

    public async Task<List<SensorChartsDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _httpClient
            .GetFromJsonAsync<List<SensorChartsDto>>(
                "api/sensors/charts",
                cancellationToken)
            ?? [];
    }
}
