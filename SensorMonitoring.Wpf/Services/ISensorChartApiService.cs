using SensorMonitoring.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Wpf.Services;

public interface ISensorChartApiService
{
    Task<List<SensorTrendDto>> GetAsync(
        string deviceId,
        CancellationToken cancellationToken = default);

    Task<List<SensorChartsDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

}
