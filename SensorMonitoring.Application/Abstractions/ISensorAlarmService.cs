using SensorMonitoring.Contracts;
using SensorMonitoring.Domain.Entities;
using SensorMonitoring.Domain.Enums;

namespace SensorMonitoring.Application.Abstractions;

public interface ISensorAlarmService
{
    SensorAlarmChangedDto? Update(
        SensorPacket packet,
        SensorStatus status,
        SensorAlarmSetting? setting);

    IReadOnlyList<SensorAlarmChangedDto> GetActive();
}
