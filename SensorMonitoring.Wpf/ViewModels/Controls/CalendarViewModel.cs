using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SensorMonitoring.Wpf.Models;
using System.Collections.ObjectModel;

namespace SensorMonitoring.Wpf.ViewModels.Controls;

public partial class CalendarViewModel : ObservableObject
{
    [ObservableProperty]
    private DateTime displayedMonth;

    [ObservableProperty]
    private DateTime? selectedDate = DateTime.Today;

    public ObservableCollection<CalendarDay> Days { get; } = new();
    public string MonthTitle => DisplayedMonth.ToString("yyyy년 MM월");

    public CalendarViewModel()
    {
        DisplayedMonth = new DateTime(
            DateTime.Today.Year,
            DateTime.Today.Month,
            1);
    }

    [RelayCommand]
    private void PreviousMonth()
    {
        DisplayedMonth = DisplayedMonth.AddMonths(-1);
    }

    [RelayCommand]
    private void NextMonth()
    {
        DisplayedMonth = DisplayedMonth.AddMonths(1);
    }

    [RelayCommand]
    private void SelectDate(CalendarDay? day)
    {
        if (day is null || !day.IsSelectable)
            return;

        SelectedDate = day.Date;

        if (!day.IsCurrentMonth)
        {
            DisplayedMonth = new DateTime(
                day.Date.Year,
                day.Date.Month,
                1);
        }
    }

    partial void OnDisplayedMonthChanged(DateTime value)
    {
        OnPropertyChanged(nameof(MonthTitle));
        GenerateDays();
    }

    partial void OnSelectedDateChanged(DateTime? value)
    {
        foreach (var day in Days)
        {
            day.IsSelected =
                value.HasValue && day.Date.Date == value.Value.Date;
        }
    }
    private void GenerateDays()
    {
        Days.Clear();

        var firstDay = new DateTime(
            DisplayedMonth.Year,
            DisplayedMonth.Month,
            1);

        // 일요일부터 시작하도록 첫 번째 셀 날짜 계산
        var startDate = firstDay.AddDays(-(int)firstDay.DayOfWeek);

        // 6주 * 7일 = 42칸
        for (int i = 0; i < 42; i++)
        {
            var date = startDate.AddDays(i);

            Days.Add(new CalendarDay
            {
                Date = date,
                IsCurrentMonth =
                    date.Month == DisplayedMonth.Month &&
                    date.Year == DisplayedMonth.Year,
                IsSelected =
                    SelectedDate.HasValue &&
                    SelectedDate.Value.Date == date.Date
            });
        }
    }
}
