using SensorMonitoring.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Application.Abstractions;

public interface ISensorReadingReader
{
    Task<List<SensorReadingDto>?> GetRecentAsync(
        string deviceId,
        DateTime fromUtc,
        CancellationToken cancellationToken);
}
