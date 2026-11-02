using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMonitoring.Wpf.Models;

public partial class CalendarDay : ObservableObject
{
    public DateTime Date { get; set; }
    public int Day => Date.Day;
    public bool IsCurrentMonth { get; set; }
    public bool IsToday => Date.Date == DateTime.Today;

    // 오늘을 포함한 과거 날짜만 선택 가능
    public bool IsSelectable => Date.Date <= DateTime.Today;

    [ObservableProperty]
    private bool isSelected;
}
