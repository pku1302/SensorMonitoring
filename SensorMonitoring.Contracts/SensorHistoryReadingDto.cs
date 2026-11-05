using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Contracts;

public class SensorHistoryReadingDto
{
    public DateTime MeasuredAt { get; set; }

    // 화면에 표시할 센서 식별자(DeviceId)
    public string SensorId { get; set; } = string.Empty;
    public double Temperature { get; set; }
    public double Humidity { get; set; }

    // 과거 데이터에는 상태 정보가 없을 수 있음
    public SensorStatusDto Status { get; set; }
}
