using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace SensorMonitoring.Wpf.Converters;

public class HourRangeConverter : IValueConverter
{
    public object Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        if (value is int hour && hour >= 0 && hour <= 23)
        {
            return $"{hour:00}:00 ~ {hour:00}:59";
        }

        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}