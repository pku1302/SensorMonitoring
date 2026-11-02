using SensorMonitoring.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Wpf.Services;

public interface ISensorReadingApiService
{
    Task<List<SensorReadingDto>> GetRecentAsync(
        string deviceId,
        CancellationToken cancellationToken = default);
}
