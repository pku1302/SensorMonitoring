using SensorMonitoring.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Application.Abstractions;

public interface ISensorRealtimeNotifier
{
    Task SendSensorUpdatedAsync(
        SensorRealTimeDto dto,
        CancellationToken cancellationToken);

    Task SendAlarmChangedAsync(
        SensorAlarmChangedDto dto,
        CancellationToken cancellationToken);
}
