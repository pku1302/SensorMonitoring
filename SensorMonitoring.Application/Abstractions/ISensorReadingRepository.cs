using SensorMonitoring.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Application.Abstractions;

public interface ISensorReadingRepository
{
    Task<long> AddAsync(
        SensorReading reading,
        CancellationToken cancellationToken);
}
