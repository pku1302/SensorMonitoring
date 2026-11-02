using SensorMonitoring.Application.Abstractions;
using SensorMonitoring.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Application.Services;

public class SensorReadingService : ISensorReadingService
{
    private readonly ISensorReadingReader _reader;

    public SensorReadingService(ISensorReadingReader reader)
    {
        _reader = reader;
    }

    public Task<List<SensorReadingDto>?> GetRecentAsync(
        string deviceId,
        CancellationToken cancellationToken)
    {
        DateTime fromUtc = DateTime.UtcNow.AddMinutes(-1);

        return _reader.GetRecentAsync(
            deviceId,
            fromUtc,
            cancellationToken);
    }

}
