using SensorMonitoring.Contracts;
using SensorMonitoring.Wpf.ViewModels.Chart;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SensorMonitoring.Wpf.Views.Chart;

public partial class SensorChart
{
    private void UpdateHover()
    {
        if (!ChartCanvas.IsMouseOver)
        {
            HideHover();
            return;
        }

        if (DataContext is not ChartViewModel chart)
        {
            HideHover();
            return;
        }

        double width = ChartCanvas.ActualWidth;
        double height = ChartCanvas.ActualHeight;

        if (width <= 0 || height <= 0)
        {
            HideHover();
            return;
        }

        var points = _displayedPoints;

        if (points.Count == 0 ||
            points.Count != _displayedValues.Count)
        {
            HideHover();
            return;
        }

        Point mouse = Mouse.GetPosition(ChartCanvas);

        double minDistance = double.MaxValue;
        int nearestIndex = -1;

        // 실제 그려진 포인트 중 가장 가까운 X좌표 탐색
        for (int i = 0; i < _displayedPoints.Count; i++)
        {
            double distance = Math.Abs(
                mouse.X - _displayedPoints[i].X);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestIndex = i;
            }
        }

        if (DataContext is HistoryChartViewModel)
        {
            const double maxHoverDistance = 8;

            if (minDistance > maxHoverDistance)
            {
                HideHover();
                return;
            }
        }

        if (nearestIndex == -1 || minDistance > 12)
        {
            HideHover();
            return;
        }

        // 동일한 인덱스로 데이터와 좌표 가져오기
        ChartData nearestData = _displayedValues[nearestIndex];
        Point nearestPoint = _displayedPoints[nearestIndex];

        if (DataContext is HistoryChartViewModel &&
            nearestData is HistoryChartData historyData)
        {
            HoverRangePanel.Visibility = Visibility.Visible;

            HoverMinText.Text =
                $"{historyData.MinValue:F1} {chart.Unit}";

            HoverMaxText.Text =
                $"{historyData.MaxValue:F1} {chart.Unit}";
        }
        else
        {
            HoverRangePanel.Visibility = Visibility.Collapsed;
        }

        // 1. 세로선
        HoverLine.X1 = nearestPoint.X;
        HoverLine.X2 = nearestPoint.X;
        HoverLine.Y1 = 0;
        HoverLine.Y2 = height;
        HoverLine.Visibility = Visibility.Visible;

        // 2. 포인트 원
        Canvas.SetLeft(
            HoverPoint,
            nearestPoint.X - HoverPoint.Width / 2);

        Canvas.SetTop(
            HoverPoint,
            nearestPoint.Y - HoverPoint.Height / 2);

        HoverPoint.Visibility = Visibility.Visible;

        // 3. 툴팁 텍스트
        HoverTimeText.Text =
            nearestData.Time.ToString("HH:mm:ss");

        if (DataContext is HistoryChartViewModel)
        {
            HoverValueText.Text =
                $"평균 {chart.Name}: {nearestData.Value:0.##}{chart.Unit}";
        }
        else
        {
            HoverValueText.Text =
                $"{chart.Name}: {nearestData.Value:0.##}{chart.Unit}";
        }

        // 4. 위험 수준에 따른 툴팁 원 색상
        TooltipStatusDot.Fill =
            nearestData.Value >= chart.DangerValue
                ? (Brush)FindResource("DangerBrush")
                : (Brush)FindResource("NormalBrush");

        HoverTooltip.Visibility = Visibility.Visible;

        // 5. 툴팁 크기 측정
        HoverTooltip.Measure(
            new Size(
                double.PositiveInfinity,
                double.PositiveInfinity));

        double tooltipWidth = HoverTooltip.DesiredSize.Width;
        double tooltipHeight = HoverTooltip.DesiredSize.Height;

        // 6. 툴팁 위치 계산
        double tooltipX = nearestPoint.X + 12;
        double tooltipY =
            nearestPoint.Y - tooltipHeight - 12;


        if (tooltipX + tooltipWidth > width)
            tooltipX = nearestPoint.X - tooltipWidth - 12;

        tooltipX = Math.Clamp(
            tooltipX, 0, Math.Max(0, width - tooltipWidth));

        tooltipY = Math.Clamp(
            tooltipY, 0, Math.Max(0, height - tooltipHeight));

        Canvas.SetLeft(HoverTooltip, tooltipX);
        Canvas.SetTop(HoverTooltip, tooltipY);
    }

    private void HideHover()
    {
        HoverLine.Visibility = Visibility.Collapsed;
        HoverPoint.Visibility = Visibility.Collapsed;
        HoverTooltip.Visibility = Visibility.Collapsed;
    }
}
