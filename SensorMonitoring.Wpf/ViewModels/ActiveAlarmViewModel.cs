using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SensorMonitoring.Contracts;
using SensorMonitoring.Wpf.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace SensorMonitoring.Wpf.ViewModels;

public partial class ActiveAlarmViewModel : ObservableObject
{
    private const int PageSize = 10;
    
    // 전체 데이터
    public ObservableCollection<ActiveAlarmItemViewModel> ActiveAlarms { get; }
        = new();

    // 현재 페이지에 표시할 데이터
    public ObservableCollection<ActiveAlarmItemViewModel> PagedAlarms { get; }
        = new();

    // 현재 페이지
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PageInfo))]
    [NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    private int currentPage = 1;

    // 전체 페이지 수
    public int TotalPages =>
            Math.Max(1, (ActiveAlarms.Count + PageSize - 1) / PageSize);

    // 페이지 표시
    public string PageInfo =>
        $"{CurrentPage} / {TotalPages}";

    // 전체 알람 수
    public string AlarmCountText =>
        $"{ActiveAlarms.Count}건";

    public ActiveAlarmViewModel(
        ISensorRealtimeService realtimeService)
    {
        realtimeService.AlarmChanged += OnAlarmChanged;

        ActiveAlarms.CollectionChanged += (_, _) =>
            OnPropertyChanged(nameof(AlarmCountText));

        UpdatePage();
    }

    private void OnAlarmChanged(SensorAlarmChangedDto dto)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            ApplyAlarm(dto);
            UpdatePage();
        });
    }
    private void ApplyAlarm(SensorAlarmChangedDto dto)
    {
        var existing = ActiveAlarms.FirstOrDefault(
            x => x.DeviceId == dto.DeviceId);

        if (!dto.IsActive)
        {
            if (existing is not null)
                ActiveAlarms.Remove(existing);

            return;
        }

        if (existing is null)
        {
            ActiveAlarms.Add(new ActiveAlarmItemViewModel
            {
                DeviceId = dto.DeviceId,
                Status = dto.Status,
                Message = dto.Message,
                OccurredAt = dto.OccurredAt
            });
        }
        else
        {
            existing.Status = dto.Status;
            existing.Message = dto.Message;
            existing.OccurredAt = dto.OccurredAt;
        }
    }

    // 이전 페이지
    [RelayCommand(CanExecute = nameof(CanPreviousPage))]
    private void PreviousPage()
    {
        CurrentPage--;
        UpdatePage();
    }
    private bool CanPreviousPage()
    {
        return CurrentPage > 1;
    }

    // 다음 페이지
    [RelayCommand(CanExecute = nameof(CanNextPage))]
    private void NextPage()
    {
        CurrentPage++;
        UpdatePage();
    }
    private bool CanNextPage()
    {
        return CurrentPage < TotalPages;
    }

    // 페이지 갱신
    private void UpdatePage()
    {
        CurrentPage = Math.Min(CurrentPage, TotalPages);

        var items = ActiveAlarms
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        PagedAlarms.Clear();

        foreach (var item in items)
        {
            PagedAlarms.Add(item);
        }

        OnPropertyChanged(nameof(TotalPages));
        OnPropertyChanged(nameof(PageInfo));
        OnPropertyChanged(nameof(AlarmCountText));

        PreviousPageCommand.NotifyCanExecuteChanged();
        NextPageCommand.NotifyCanExecuteChanged();
    }

}
