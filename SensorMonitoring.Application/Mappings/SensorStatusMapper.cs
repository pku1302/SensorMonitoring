using SensorMonitoring.Contracts;
using SensorMonitoring.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Application.Mappings;

public static class SensorStatusMapper
{
    public static SensorStatusDto ToDto(SensorStatus status)
    {
        return status switch
        {
            SensorStatus.Normal => SensorStatusDto.Normal,
            SensorStatus.Warning => SensorStatusDto.Warning,
            SensorStatus.Danger => SensorStatusDto.Danger,
            SensorStatus.Offline => SensorStatusDto.Offline,

            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };
    }
}
