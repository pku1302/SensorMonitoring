using SensorMonitoring.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Wpf.Services;

public interface ISensorAlarmApiService
{
    Task<List<SensorAlarmChangedDto>> GetActiveAsync(
        );
}
