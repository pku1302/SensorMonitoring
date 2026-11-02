using CommunityToolkit.Mvvm.ComponentModel;
using SensorMonitoring.Contracts;

namespace SensorMonitoring.Wpf.ViewModels.Chart;

public partial class HistoryChartViewModel : ChartViewModel
{
    public const double MinZoomHours = 1;
    public const double MaxZoomHours = 24;
    public const int IntervalMinutes = 5;

    [ObservableProperty]
    private DateTime startTime = DateTime.Today;

    [ObservableProperty]
    private DateTime endTime = DateTime.Today.AddDays(1);

    [ObservableProperty]
    private DateTime visibleStartTime = DateTime.Today;

    [ObservableProperty]
    private DateTime visibleEndTime = DateTime.Today.AddDays(1);

    public void Load(SensorTrendDto dto)
    {
        Name = dto.Name;
        Unit = dto.Unit;
        MinValue = dto.MinValue;
        MaxValue = dto.MaxValue;
        DangerValue = dto.DangerValue;

        Values.Clear();

        Values = new System.Collections.ObjectModel.ObservableCollection<ChartData>(dto.Values);
    }

    public void Zoom(double factor, double mouseRatio)
    {
        mouseRatio = Math.Clamp(mouseRatio, 0.0, 1.0);

        TimeSpan currentDuration =
            VisibleEndTime - VisibleStartTime;

        if (currentDuration <= TimeSpan.Zero)
            return;

        double newHours = Math.Clamp(
            currentDuration.TotalHours * factor,
            MinZoomHours,
            MaxZoomHours);

        TimeSpan newDuration = TimeSpan.FromHours(newHours);

        // 마우스가 가리키는 실제 시간 계산
        DateTime anchorTime = VisibleStartTime.AddTicks(
            (long)(currentDuration.Ticks * mouseRatio));

        // 마우스가 가리키는 시간이 같은 X좌표에 남도록 계산
        DateTime newStart = anchorTime.AddTicks(
            -(long)(newDuration.Ticks * mouseRatio));

        DateTime newEnd = newStart + newDuration;

        // 조회 가능한 전체 범위를 벗어나지 않도록 보정
        if (newStart < StartTime)
        {
            newStart = StartTime;
            newEnd = newStart + newDuration;
        }

        if (newEnd > EndTime)
        {
            newEnd = EndTime;
            newStart = newEnd - newDuration;
        }

        VisibleStartTime = newStart;
        VisibleEndTime = newEnd;
    }
    public void Pan(double deltaRatio)
    {
        TimeSpan duration = VisibleEndTime - VisibleStartTime;

        if (duration <= TimeSpan.Zero)
            return;

        // 전체 범위가 보이는 상태라면 이동하지 않음
        if (duration >= EndTime - StartTime)
            return;

        // 마우스 이동 비율을 시간으로 변환
        long deltaTicks = (long)(duration.Ticks * deltaRatio);

        DateTime newStart = VisibleStartTime.AddTicks(-deltaTicks);
        DateTime newEnd = VisibleEndTime.AddTicks(-deltaTicks);

        // 전체 시작 시간보다 이전으로 이동 방지
        if (newStart < StartTime)
        {
            newStart = StartTime;
            newEnd = newStart + duration;
        }

        // 전체 종료 시간보다 이후로 이동 방지
        if (newEnd > EndTime)
        {
            newEnd = EndTime;
            newStart = newEnd - duration;
        }

        VisibleStartTime = newStart;
        VisibleEndTime = newEnd;
    }

    [ObservableProperty]
    private bool isLoading;

}
