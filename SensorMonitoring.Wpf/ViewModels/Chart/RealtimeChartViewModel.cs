using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SensorMonitoring.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SensorMonitoring.Wpf.ViewModels.Chart;

public partial class RealtimeChartViewModel: ChartViewModel
{
    public const int MaxPointCount = 180;
    public const int VisiblePointCount = 60;
    public override Visibility ControlVisibility 
        => Visibility.Visible;

    private int _nextSequence;

    [ObservableProperty]
    private bool isPaused;

    // 최신 데이터로부터 몇 개 이전 구간을 보고 있는지
    [ObservableProperty]
    private int viewOffset;

    // 과거 탐색 시작 지점의 데이터 보관
    private List<ChartData>? _snapshot;

    public void Load(SensorTrendDto dto)
    {
        Name = dto.Name;
        Unit = dto.Unit;
        MinValue = dto.MinValue;
        MaxValue  = dto.MaxValue;
        DangerValue = dto.DangerValue;

        Values.Clear();

        _nextSequence = 0;

        foreach (var point in dto.Values.TakeLast(MaxPointCount))
        {
            AddPoint(point.Time, point.Value);
        }

        OnPropertyChanged(nameof(HasData));
    }
    public void AddPoint(DateTime time, double value)
    {
        if (IsPaused)
            return;

        Values.Add(new ChartData
        {
            Time = time.ToLocalTime(),
            Value = value,
            Sequence = _nextSequence
        });

        _nextSequence =
            (_nextSequence + 1);

        while (Values.Count > MaxPointCount)
        {
            Values.RemoveAt(0);
        }

        OnPropertyChanged(nameof(HasData));
    }

    [RelayCommand]
    private void Pause()
    {
        if (IsPaused)
            return;

        _snapshot = Values.ToList();
        IsPaused = true;
    }

    [RelayCommand]
    private void ReturnToLive()
    {
        if (!IsPaused)
            return;

        ViewOffset = 0;
        IsPaused = false;
        _snapshot = null;
    }

    // 차트에 표시할 데이터
    public List<ChartData> GetVisibleValues()
    {
        var source = IsPaused && _snapshot != null
            ? _snapshot
            : Values.ToList();

        int startIndex = Math.Max(
            0,
            source.Count - VisiblePointCount - ViewOffset);

        return source
            .Skip(startIndex)
            .Take(VisiblePointCount)
            .ToList();
    }

    public void Pan(int delta)
    {
        if (Values.Count <= VisiblePointCount && !IsPaused)
            return;

        if (!IsPaused)
        {
            if (delta <= 0)
                return;

            Pause();
        }

        int maxOffset = Math.Max(
            0,
            Values.Count - VisiblePointCount);

        ViewOffset = Math.Clamp(
            ViewOffset + delta,
            0,
            maxOffset);
    }
}
