using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Application.Common;

public static class KoreaDateRange
{
    private static readonly TimeZoneInfo KoreaTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Asia/Seoul");

    public static (DateTime StartUtc, DateTime EndUtc)
        ToUtc(DateOnly date)
    {
        var localStart = DateTime.SpecifyKind(
            date.ToDateTime(TimeOnly.MinValue),
            DateTimeKind.Unspecified);

        var startUtc = TimeZoneInfo.ConvertTimeToUtc(
            localStart,
            KoreaTimeZone);

        var endUtc = TimeZoneInfo.ConvertTimeToUtc(
            localStart.AddDays(1),
            KoreaTimeZone);

        return (startUtc, endUtc);
    }
}
