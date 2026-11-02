using SensorMonitoring.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace SensorMonitoring.Wpf.Controls;

public class StatusBadge : Control
{
    public static readonly DependencyProperty StatusProperty =
        DependencyProperty.Register(
            nameof(Status),
            typeof(SensorStatusDto),
            typeof(StatusBadge),
            new PropertyMetadata(SensorStatusDto.Normal));

    public SensorStatusDto Status
    {
        get => (SensorStatusDto)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }
    static StatusBadge()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(StatusBadge),
            new FrameworkPropertyMetadata(typeof(StatusBadge)));
    }
}
