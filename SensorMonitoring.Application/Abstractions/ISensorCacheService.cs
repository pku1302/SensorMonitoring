using SensorMonitoring.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Application.Abstractions;

public interface ISensorCacheService
{
    Task<SensorCacheItem?> GetAsync(
        string deviceId,
        long producedQuantity,
        CancellationToken cancellationToken);
}
