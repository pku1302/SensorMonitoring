using SensorMonitoring.Contracts;
using SensorMonitoring.Wpf.ViewModels.Chart;
using SensorMonitoring.Wpf.ViewModels.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SensorMonitoring.Wpf.Views.Chart;

public partial class SensorChart : UserControl
{
    private ObservableCollection<ChartData>? _currentValues;
    private List<ChartData> _displayedValues = new();
    private List<Point> _displayedPoints = new();
    private ChartViewModel? _currentChart;
    public SensorChart()
    {
        InitializeComponent();

        DataContextChanged += SensorChart_DataContextChanged;
        Unloaded += SensorChart_Unloaded;
    }

    private void SensorChart_DataContextChanged(
        object sender,
        DependencyPropertyChangedEventArgs e)
    {
        // 1. 이전 ViewModel의 이벤트 구독 해제
        if (_currentValues != null)
        {
            _currentValues.CollectionChanged -= Values_CollectionChanged;
        }

        // 2. 새로운 ViewModel 저장
        _currentChart = DataContext as ChartViewModel;

        // 3. 새로운 ViewModel의 이벤트 구독
        if (_currentChart != null)
        {
            _currentChart.PropertyChanged += Chart_PropertyChanged;
        }

        if (DataContext is ChartViewModel chart)
        {
            _currentValues = chart.Values;

            _currentValues.CollectionChanged += Values_CollectionChanged;
        }
        else
        {
            _currentValues = null;
        }

        DrawChart();
    }

    private void Chart_PropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (sender is RealtimeChartViewModel)
        {
            if (e.PropertyName == nameof(RealtimeChartViewModel.ViewOffset) ||
                e.PropertyName == nameof(RealtimeChartViewModel.IsPaused))
            {
                DrawChart();
            }
        }
        else if (sender is HistoryChartViewModel)
        {
            if (e.PropertyName == nameof(ChartViewModel.Values))
            {
                SubscribeValues();
                DrawChart();
                return;
            }

            if (e.PropertyName == nameof(HistoryChartViewModel.StartTime) ||
                e.PropertyName == nameof(HistoryChartViewModel.EndTime) ||
                e.PropertyName == nameof(HistoryChartViewModel.VisibleStartTime) ||
                e.PropertyName == nameof(HistoryChartViewModel.VisibleEndTime))
            {
                DrawChart();
            }
        }
    }
    private void SubscribeValues()
    {
        if (_currentValues != null)
            _currentValues.CollectionChanged -= Values_CollectionChanged;

        if (DataContext is not ChartViewModel chart)
        {
            _currentValues = null;
            return;
        }

        _currentValues = chart.Values;
        _currentValues.CollectionChanged += Values_CollectionChanged;
    }

    private void Values_CollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        if (DataContext is RealtimeChartViewModel realtime &&
            realtime.IsPaused)
            return;

        DrawChart();
    }
    private void SensorChart_Unloaded(
        object sender,
        RoutedEventArgs e)
    {
        // 드래그 상태 초기화
        _isDragging = false;

        if (ChartCanvas.IsMouseCaptured)
            ChartCanvas.ReleaseMouseCapture();

        // ViewModel 이벤트 구독 해제
        if (_currentChart != null)
            _currentChart.PropertyChanged -= Chart_PropertyChanged;

        // 데이터 변경 이벤트 구독 해제
        if (_currentValues != null)
        {
            _currentValues.CollectionChanged -= Values_CollectionChanged;
        }

        Debug.WriteLine(
            $"Width: {ChartCanvas.ActualWidth}, " +
            $"Height: {ChartCanvas.ActualHeight}");
    }
    private void ChartCanvas_SizeChanged(
        object sender,
        SizeChangedEventArgs e)
    {
        DrawChart();
    }

    private void ChartCanvas_MouseLeave(
        object sender,
        MouseEventArgs e)
    {
        HideHover();
    }

    private double GetY(
        double value,
        double min,
        double max,
        double height)
    {
        const double padding = 10;

        double plotHeight = height - padding * 2;

        if (max <= min || plotHeight <= 0)
            return height / 2;

        double ratio = (value - min) / (max - min);

        return padding + (1 - ratio) * plotHeight;
    }

    private void DrawChart()
    {
        if (DataContext is not ChartViewModel chart)
        {
            ClearChart();
            return;
        }

        double width = ChartCanvas.ActualWidth;
        double height = ChartCanvas.ActualHeight;

        if (width <= 0 || height <= 0)
            return;

        if (chart.MaxValue <= chart.MinValue)
        {
            ClearChart();
            return;
        }

        List<ChartData> values;

        // 1. 모드에 따라 데이터 선택
        if (chart is RealtimeChartViewModel realtime)
        {
            values = realtime.GetVisibleValues();
        }
        else if (chart is HistoryChartViewModel historyChart)
        {
            if (historyChart.EndTime <= historyChart.StartTime)
            {
                ClearChart();
                return;
            }

            values = historyChart.Values
                .Where(v => v.Time >= historyChart.VisibleStartTime &&
                            v.Time <= historyChart.VisibleEndTime)
                .OrderBy(v => v.Time)
                .ToList();
        }
        else
        {
            ClearChart();
            return;
        }

        var points = new PointCollection();

        // 3. 모드에 따라 X좌표만 다르게 계산
        for (int i = 0; i < values.Count; i++)
        {
            ChartData item = values[i];

            double x;

            if (chart is RealtimeChartViewModel)
            {
                int visibleCount = RealtimeChartViewModel.VisiblePointCount;

                double spacing = width / (visibleCount - 1);
                int startIndex = visibleCount - values.Count;

                x = (startIndex + i) * spacing;
            }
            else if (chart is HistoryChartViewModel historyChart)
            {
                double duration =
                    (historyChart.VisibleEndTime - historyChart.VisibleStartTime)
                    .TotalSeconds;

                double elapsed =
                    (item.Time - historyChart.VisibleStartTime)
                    .TotalSeconds;

                x = elapsed / duration * width;
            }
            else
            {
                continue;
            }

            // 4. Y좌표는 두 모드 공통
            double y = GetY(
                item.Value,
                chart.MinValue,
                chart.MaxValue,
                height);

            points.Add(new Point(x, y));
        }

        // 5. 실제 표시 중인 데이터 저장
        _displayedValues = values.ToList();
        _displayedPoints = points.ToList();

        // 6. 그래프 업데이트
        if (chart is HistoryChartViewModel history)
        {
            //히스토리는 데이터 공백 시 끊어진 선분으로 표시
            ChartLine.Points = new PointCollection();
            DangerChartLine.Points = new PointCollection();

            DrawHistoryRanges(values, history, width, height);
            DrawHistorySegments(values, points, chart);
        }
        else
        {
            HistoryLineLayer.Children.Clear();

            ChartLine.Points = points;
            DangerChartLine.Points = points;

            double dangerY = GetY(
                chart.DangerValue,
                chart.MinValue,
                chart.MaxValue,
                height);

            DangerChartLine.Clip = new RectangleGeometry(
                new Rect(0, 0, width, Math.Clamp(dangerY, 0, height)));
        }

        // 8. 축 및 호버 갱신
        UpdateYAxis();
        UpdateXAxisLabels();
        UpdateHover();
    }

    private void DrawHistoryRanges(
        List<ChartData> values,
        HistoryChartViewModel history,
        double width,
        double height)
    {
        HistoryRangeLayer.Children.Clear();

        double duration =
            (history.VisibleEndTime - history.VisibleStartTime)
            .TotalSeconds;

        if (duration <= 0)
            return;

        foreach (ChartData data in values)
        {
            if (data is not HistoryChartData item)
                continue;

            double x =
                (item.Time - history.VisibleStartTime).TotalSeconds
                / duration * width;

            double minY = GetY(
                item.MinValue,
                history.MinValue,
                history.MaxValue,
                height);

            double maxY = GetY(
                item.MaxValue,
                history.MinValue,
                history.MaxValue,
                height);

            var rangeLine = new Line
            {
                X1 = x,
                X2 = x,
                Y1 = maxY,
                Y2 = minY,
                Stroke = (Brush)FindResource("AccentBrush"),
                StrokeThickness = 4,
                Opacity = 0.8,
                IsHitTestVisible = false
            };

            HistoryRangeLayer.Children.Add(rangeLine);
        }

    }

    private void ClearChart()
    {
        _displayedValues.Clear();
        _displayedPoints.Clear();

        ChartLine.Points = new PointCollection();
        DangerChartLine.Points = new PointCollection();

        HistoryLineLayer.Children.Clear();
        HistoryRangeLayer.Children.Clear();

        HideHover();
    }

    private void DrawHistorySegments(
        List<ChartData> values,
        PointCollection points,
        ChartViewModel chart)
    {
        HistoryLineLayer.Children.Clear();

        TimeSpan maxGap = TimeSpan.FromMinutes(10);

        PointCollection segment = new();

        for (int i = 0; i < values.Count; i++)
        {
            if (i > 0 &&
                values[i].Time - values[i - 1].Time > maxGap)
            {
                AddSegment(segment, chart);
                segment = new PointCollection();
            }

            segment.Add(points[i]);
        }

        AddSegment(segment, chart);
    }

    private void AddSegment(
        PointCollection points,
        ChartViewModel chart)
    {
        if (points.Count < 2)
            return;

        var normalLine = new Polyline
        {
            Points = points,
            Stroke = (Brush)FindResource("NormalBrush"),
            StrokeThickness = 2,
            StrokeLineJoin = PenLineJoin.Round,
            IsHitTestVisible = false
        };

        HistoryLineLayer.Children.Add(normalLine);

        // 위험 영역에 해당하는 부분만 빨간색으로 표시
        double dangerY = GetY(
            chart.DangerValue,
            chart.MinValue,
            chart.MaxValue,
            ChartCanvas.ActualHeight);

        var dangerLine = new Polyline
        {
            Points = points,
            Stroke = (Brush)FindResource("DangerBrush"),
            StrokeThickness = 2,
            StrokeLineJoin = PenLineJoin.Round,
            IsHitTestVisible = false,
            Clip = new RectangleGeometry(
                new Rect(
                    0,
                    0,
                    ChartCanvas.ActualWidth,
                    Math.Clamp(dangerY, 0, ChartCanvas.ActualHeight)))
        };

        HistoryLineLayer.Children.Add(dangerLine);
    }
    private TimeSpan GetHistoryLabelInterval(TimeSpan duration)
    {
        double hours = duration.TotalHours;

        if (hours >= 12)
            return TimeSpan.FromHours(6);

        if (hours >= 6)
            return TimeSpan.FromHours(3);

        if (hours >= 3)
            return TimeSpan.FromHours(1);

        if (hours >= 2)
            return TimeSpan.FromMinutes(30);

        return TimeSpan.FromMinutes(15);
    }
}
