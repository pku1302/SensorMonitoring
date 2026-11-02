using SensorMonitoring.Contracts;
using SensorMonitoring.Wpf.ViewModels.Chart;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SensorMonitoring.Wpf.Views.Chart;

public partial class SensorChart
{
    private void UpdateXAxisLabels()
    {
        if (DataContext is RealtimeChartViewModel)
        {
            HistoryXAxisLayer.Children.Clear();

            UpdateRealtimeXAxisLabels();
        }
        else if (DataContext is HistoryChartViewModel history)
        {
            RealtimeXAxisLayer.Children.Clear();

            UpdateHistoryXAxisLabels(history);
        }
    }
    private void UpdateRealtimeXAxisLabels()
    {
        RealtimeXAxisLayer.Children.Clear();

        double width = ChartCanvas.ActualWidth;

        if (width <= 0 || _displayedValues.Count == 0)
            return;

        const int labelInterval = 20;

        int visibleCount = RealtimeChartViewModel.VisiblePointCount;

        double spacing = width / (visibleCount - 1);

        // 데이터가 60개 미만일 때 오른쪽 정렬
        int startSlot = visibleCount - _displayedValues.Count;

        for (int i = 0; i < _displayedValues.Count; i++)
        {
            ChartData data = _displayedValues[i];

            // 20개마다 시간 라벨 생성
            if (data.Sequence % labelInterval != 0)
            {
                continue;
            }

            double x = (startSlot + i) * spacing;

            AddRealtimeTimeLabel(
                data.Time.ToString("HH:mm:ss"),
                x);
        }
    }
    private void AddRealtimeTimeLabel(string text, double x)
    {
        var label = new TextBlock
        {
            Text = text,
            FontSize = 12,
            Foreground = (Brush)FindResource("MutedBrush"),
            IsHitTestVisible = false
        };

        label.Measure(new Size(
            double.PositiveInfinity,
            double.PositiveInfinity));

        Canvas.SetLeft(
            label,
            x - label.DesiredSize.Width / 2);

        RealtimeXAxisLayer.Children.Add(label);
    }
    private void UpdateYAxis()
    {
        if (DataContext is not ChartViewModel chart)
            return;

        double width = ChartCanvas.ActualWidth;
        double height = ChartCanvas.ActualHeight;

        if (width <= 0 || height <= 0)
            return;

        double min = chart.MinValue;
        double max = chart.MaxValue;
        double mid = (min + max) / 2.0;
        double danger = chart.DangerValue;

        UpdateTick(max, MaxLabel, MaxGridLine);
        UpdateTick(mid, MidLabel, MidGridLine);
        UpdateTick(min, MinLabel, MinGridLine);
        UpdateTick(danger, DangerLabel, DangerGridLine);

        void UpdateTick(double value, TextBlock label, Line line)
        {
            double y = GetY(value, min, max, height);

            // 눈금선 위치
            line.X1 = 0;
            line.X2 = width;
            line.Y1 = y;
            line.Y2 = y;

            // y축 라벨
            label.Text = value.ToString("0.##");

            // TextBlock의 실제 크기 측정
            label.Measure(new Size(
                double.PositiveInfinity,
                double.PositiveInfinity));

            double labelHeight = label.DesiredSize.Height;
            double labelWidth = label.DesiredSize.Width;

            // 라벨 중심을 눈금선과 일치
            Canvas.SetTop(label, y - labelHeight / 2);

            // 오른쪽 정렬
            Canvas.SetLeft(
                label,
                Math.Max(0, YAxisCanvas.ActualWidth - labelWidth));
        }
    }
    private void UpdateHistoryXAxisLabels(
    HistoryChartViewModel history)
    {
        HistoryXAxisLayer.Children.Clear();

        double width = ChartCanvas.ActualWidth;

        if (width <= 0)
            return;

        DateTime visibleStart = history.VisibleStartTime;
        DateTime visibleEnd = history.VisibleEndTime;

        TimeSpan duration = visibleEnd - visibleStart;

        if (duration <= TimeSpan.Zero)
            return;

        TimeSpan interval = GetHistoryLabelInterval(duration);

        // 전체 조회 시작 시간을 기준으로 눈금 정렬
        DateTime start = history.StartTime;

        // 현재 표시 범위에서 첫 번째 눈금 찾기
        long elapsedTicks = (visibleStart - start).Ticks;
        long intervalTicks = interval.Ticks;

        long firstIndex = Math.Max(
            0,
            (long)Math.Ceiling((double)elapsedTicks / intervalTicks));

        DateTime labelTime = start.AddTicks(
            firstIndex * intervalTicks);

        while (labelTime <= visibleEnd &&
            labelTime <= history.EndTime)
        {
            double x =
                (labelTime - visibleStart).TotalSeconds /
                duration.TotalSeconds * width;

            string text;

            // 다음 날 00:00을 24:00으로 표시
            if (labelTime == history.EndTime &&
                history.EndTime.Date > history.StartTime.Date &&
                history.EndTime.TimeOfDay == TimeSpan.Zero)
            {
                text = "24:00";
            }
            else
            {
                text = labelTime.ToString("HH:mm");
            }

            AddHistoryTimeLabel(text, x, width);

            labelTime = labelTime.Add(interval);
        }
    }
    private void AddHistoryTimeLabel(
         string text,
         double x,
         double width)
    {
        var label = new TextBlock
        {
            Text = text,
            FontSize = 12,
            Foreground = new SolidColorBrush(
                Color.FromRgb(136, 136, 136))
        };

        label.Measure(new Size(
            double.PositiveInfinity,
            double.PositiveInfinity));

        double labelWidth = label.DesiredSize.Width;

        // 해당 시간의 X좌표를 중심으로 배치
        double left = x - labelWidth / 2;

        left = Math.Clamp(
            left,
            0,
            Math.Max(0, width - labelWidth));

        Canvas.SetLeft(label, left);

        HistoryXAxisLayer.Children.Add(label);
    }
}
