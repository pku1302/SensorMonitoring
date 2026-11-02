using SensorMonitoring.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Application.Abstractions;

public interface ISensorChartCache
{
    void Add(SensorPacket packet);

    IReadOnlyList<SensorChartPointDto> Get(string deviceId);
}
