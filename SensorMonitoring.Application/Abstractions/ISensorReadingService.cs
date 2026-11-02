using SensorMonitoring.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Application.Abstractions;

public interface ISensorReadingService
{
    Task<List<SensorReadingDto>?> GetRecentAsync(
        string deviceId,
        CancellationToken cancellationToken = default);
}
