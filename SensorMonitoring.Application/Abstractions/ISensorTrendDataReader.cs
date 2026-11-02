using SensorMonitoring.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Application.Abstractions;

public interface ISensorTrendDataReader
{
    Task<SensorTrendSensorInfo?> GetAsync(
        string deviceId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<SensorTrendSensorInfo>> GetAllEnabledAsync(
        CancellationToken cancellationToken);
}
