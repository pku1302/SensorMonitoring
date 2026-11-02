using SensorMonitoring.Wpf.ViewModels.Chart;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace SensorMonitoring.Wpf.Views.Chart;

public partial class SensorChart
{
    private bool _isDragging;
    private Point _lastMousePosition;
    private double _realtimeDragRemainder;

    private void ChartCanvas_MouseLeftButtonDown(
        object sender,
        MouseButtonEventArgs e)
    {
        if (DataContext is HistoryChartViewModel history)
        {
            // 전체 시간 범위가 보이면 드래그 불가능
            if (history.VisibleEndTime - history.VisibleStartTime
                >= history.EndTime - history.StartTime)
                return;
        }
        else if (DataContext is RealtimeChartViewModel realtime)
        {
            // 보관 데이터가 60개 이하면 드래그 불가능
            if (realtime.Values.Count <=
                RealtimeChartViewModel.VisiblePointCount)
                return;
        }
        else
        {
            return;
        }

        _isDragging = true;
        _lastMousePosition = e.GetPosition(ChartCanvas);
        _realtimeDragRemainder = 0;

        ChartCanvas.CaptureMouse();
        e.Handled = true;
    }

    private void ChartCanvas_MouseMove(
        object sender,
        MouseEventArgs e)
    {
        if (_isDragging)
        {
            Point currentPosition = e.GetPosition(ChartCanvas);

            double deltaX =
                currentPosition.X - _lastMousePosition.X;

            double width = ChartCanvas.ActualWidth;

            if (width > 0)
            {
                if (DataContext is HistoryChartViewModel history)
                {
                    // 히스토리: 시간 기준 이동
                    history.Pan(deltaX / width);
                }
                else if (DataContext is RealtimeChartViewModel realtime)
                {
                    // 실시간: 데이터 개수 기준 이동
                    double spacing = width /
                        (RealtimeChartViewModel.VisiblePointCount - 1);

                    _realtimeDragRemainder += deltaX / spacing;

                    int deltaPoints =
                        (int)Math.Truncate(_realtimeDragRemainder);

                    if (deltaPoints != 0)
                    {
                        realtime.Pan(deltaPoints);

                        _realtimeDragRemainder -= deltaPoints;
                    }
                }
            }

            _lastMousePosition = currentPosition;

            // 드래그 중에는 호버 숨기기
            HideHover();
            return;
        }

        UpdateHover();
    }

    private void ChartCanvas_MouseLeftButtonUp(
        object sender,
        MouseButtonEventArgs e)
    {
        if (!_isDragging)
            return;

        _isDragging = false;

        ChartCanvas.ReleaseMouseCapture();

        UpdateHover();

        e.Handled = true;
    }

    private void ChartCanvas_LostMouseCapture(
        object sender,
        MouseEventArgs e)
    {
        _isDragging = false;
        _realtimeDragRemainder = 0;
    }

    private void ChartCanvas_MouseWheel(
        object sender,
        MouseWheelEventArgs e)
    {
        if (DataContext is not HistoryChartViewModel history)
            return;

        double width = ChartCanvas.ActualWidth;

        if (width <= 0)
            return;

        // 차트 기준 마우스 위치
        Point mouse = e.GetPosition(ChartCanvas);

        // 0 = 왼쪽 끝, 1 = 오른쪽 끝
        double mouseRatio = Math.Clamp(mouse.X / width, 0, 1);

        // 휠 위 : 확대, 휠 아래: 축소
        double factor = e.Delta > 0 ? 0.8 : 1.25;

        history.Zoom(factor, mouseRatio);

        // 부모 ScrollViewer의 스크롤을 막음
        e.Handled = true;
    }
}
