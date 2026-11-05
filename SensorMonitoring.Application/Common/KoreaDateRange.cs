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

    public static (
        DateTime StartUtc, DateTime EndUtc
        ) ToUtcHour(DateOnly date, int hour)
    {
        if (hour is < 0 or > 23)
            throw new ArgumentOutOfRangeException(nameof(hour));

        var koreaTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("Asia/Seoul");

        var startKst = DateTime.SpecifyKind(
            date.ToDateTime(new TimeOnly(hour, 0)),
            DateTimeKind.Unspecified);

        var endKst = startKst.AddHours(1);

        return (
            TimeZoneInfo.ConvertTimeToUtc(startKst, koreaTimeZone),
            TimeZoneInfo.ConvertTimeToUtc(endKst, koreaTimeZone));
    }
}
