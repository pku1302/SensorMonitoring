using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Contracts;

// 서버 -> 클라이언트 주는 실시간 데이터 형태
public class SensorRealTimeDto
{
    public string DeviceId { get; set; } = string.Empty;
    public double Temeperature { get; set; }
    public double Humidity { get; set; }
    public long TotalProduction { get; set; }
    public SensorStatus Status { get; set; }
    public DateTime Timestamp { get; set; }
}
