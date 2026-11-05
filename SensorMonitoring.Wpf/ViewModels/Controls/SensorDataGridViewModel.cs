using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SensorMonitoring.Contracts;
using SensorMonitoring.Wpf.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace SensorMonitoring.Wpf.ViewModels.Controls;

public partial class SensorDataGridViewModel : ObservableObject
{
    private CancellationTokenSource? _loadCts;
    private const int PageSize = 8;
    private string _sortColumn = nameof(SensorHistoryReadingDto.MeasuredAt);
    private ListSortDirection _sortDirection =
        ListSortDirection.Descending;

    public int AllCount => _allHistories.Count;
    private readonly List<SensorHistoryReadingDto> _allHistories = new();
    private readonly List<SensorHistoryReadingDto> _filteredHistories = new();
    public ObservableCollection<SensorHistoryReadingDto> Histories
        { get; } = new();

    [ObservableProperty]
    private string selectedFilter = "All";

    [ObservableProperty]
    private int currentPage = 1;

    [ObservableProperty]
    private int totalCount;

    [ObservableProperty]
    private int selectedHour = 9;

    [ObservableProperty]
    private string? selectedSensor;

    [ObservableProperty]
    private DateOnly? selectedDate;

    public List<int> HourOptions { get; } =
        Enumerable.Range(0, 24).ToList();

    public int TotalPages =>
        Math.Max(1, (int)Math.Ceiling((double)TotalCount / PageSize));

    public string PageInfo =>
    $"{CurrentPage} / {TotalPages}";

    private readonly ISensorHistoryApiService
    _sensorHistoryApiService;

    public string RangeInfo
    {
        get
        {
            if (TotalCount == 0)
                return "0 / 0건";

            int start = (CurrentPage - 1) * PageSize + 1;
            int end = Math.Min(CurrentPage * PageSize, TotalCount);

            return $"{start}-{end} / {TotalCount}건";
        }
    }

    public bool CanGoPrevious => CurrentPage > 1;
    public bool CanGoNext => CurrentPage < TotalPages;
    public SensorDataGridViewModel(
        ISensorHistoryApiService sensorHistoryApiService)
    {
        _sensorHistoryApiService = sensorHistoryApiService;

        ApplyFilter();
    }

    [RelayCommand]
    private async Task SearchByHourAsync()
    {
        // 이전 API 요청 취소
        _loadCts?.Cancel();

        if (SelectedDate is null ||
            string.IsNullOrWhiteSpace(SelectedSensor))
        {
            Clear();
            return;
        }

        // 현재 조회 조건을 지역 변수로 보관
        var date = SelectedDate.Value;
        var hour = SelectedHour;
        var deviceId = SelectedSensor;

        var cts = new CancellationTokenSource();
        _loadCts = cts;

        Clear();

        try
        {
            await LoadAsync(
                date,
                hour,
                deviceId,
                cts.Token);
        }
        catch (OperationCanceledException)
            when (cts.IsCancellationRequested)
        {
            // 날짜 센서 변경 또는 새 조회로 취소됨
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        finally
        {
            if (ReferenceEquals(_loadCts, cts))
                _loadCts = null;

            cts.Dispose();
        }
    }

    public async Task LoadAsync(
        DateOnly date,
        int hour,
        string deviceId,
        CancellationToken cancellationToken = default)
    {
        var histories = await _sensorHistoryApiService
            .GetHourlyReadingsAsync(
                date,
                hour,
                deviceId,
                cancellationToken: cancellationToken);

        // 취소된 요청이면 화면에 반영하지 않음
        cancellationToken.ThrowIfCancellationRequested();

        _allHistories.Clear();
        _allHistories.AddRange(histories);

        CurrentPage = 1;

        OnPropertyChanged(nameof(AllCount));

        ApplyFilter();
    }
    public void SetConditions(DateOnly? date, string? sensor)
    {
        // 조회 조건이 동일하면 초기화하지 않음
        if (SelectedDate == date && SelectedSensor == sensor)
            return;

        CancelAndClear();

        SelectedFilter = "All";

        SelectedDate = date;
        SelectedSensor = sensor;
    }

    public ListSortDirection SortBy(string column)
    {
        // 같은 컬럼 클릭시 방향 전환
        if (_sortColumn == column)
        {
            _sortDirection =
                _sortDirection == ListSortDirection.Ascending
                ? ListSortDirection.Descending
                : ListSortDirection.Ascending;
        }
        else
        {
            // 새로운 컬럼 클릭 시 오름차순부터 시작
            _sortColumn = column;
            _sortDirection = ListSortDirection.Ascending;
        }

        // 정렬 변경 시 첫 페이지로 이동
        CurrentPage = 1;

        ApplyFilter();

        return _sortDirection;
    }

    private IEnumerable<SensorHistoryReadingDto> ApplySorting(
        IEnumerable<SensorHistoryReadingDto> source)
    {
        bool ascending =
            _sortDirection == ListSortDirection.Ascending;

        return _sortColumn switch
        {
            nameof(SensorHistoryReadingDto.MeasuredAt) =>
                ascending
                    ? source.OrderBy(x => x.MeasuredAt)
                    : source.OrderByDescending(x => x.MeasuredAt),

            nameof(SensorHistoryReadingDto.Temperature) =>
                ascending
                    ? source.OrderBy(x => x.Temperature)
                    : source.OrderByDescending(x => x.Temperature),

            nameof(SensorHistoryReadingDto.Humidity) =>
                ascending
                    ? source.OrderBy(x => x.Humidity)
                    : source.OrderByDescending(x => x.Humidity),

            _ => source.OrderByDescending(x => x.MeasuredAt)
        };
    }

    private void LoadPage()
    {
        Histories.Clear();

        var pageItems = _filteredHistories
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        foreach (var item in pageItems)
            Histories.Add(item);

        OnPropertyChanged(nameof(PageInfo));
        OnPropertyChanged(nameof(RangeInfo));
        OnPropertyChanged(nameof(CanGoPrevious));
        OnPropertyChanged(nameof(CanGoNext));
    }

    [RelayCommand]
    private void PreviousPage()
    {
        if (!CanGoPrevious)
            return;

        CurrentPage--;
        LoadPage();
    }

    [RelayCommand]
    private void NextPage()
    {
        if (!CanGoNext)
            return;

        CurrentPage++;
        LoadPage();
    }

    [RelayCommand]
    private void ChangeFilter(string filter)
    {
        SelectedFilter = filter;
        CurrentPage = 1;

        ApplyFilter();
    }
    private void ApplyFilter()
    {
        IEnumerable<SensorHistoryReadingDto> query = SelectedFilter switch
        {
            "Normal" => _allHistories.Where(
                x => x.Status == SensorStatusDto.Normal),

            "Danger" => _allHistories.Where(
                x => x.Status == SensorStatusDto.Danger),

            _ => _allHistories
        };

        // 필터링 후 정렬
        var sortedItems = ApplySorting(query);

        _filteredHistories.Clear();
        _filteredHistories.AddRange(sortedItems);

        TotalCount = _filteredHistories.Count;

        OnPropertyChanged(nameof(TotalPages));

        LoadPage();
    }
    public void Clear()
    {
        // 원본 데이터 초기화
        _allHistories.Clear();

        // 필터형된 데이터 초기화
        _filteredHistories.Clear();

        // 화면에 표시되는 데이터 초기화
        Histories.Clear();

        // 페이지 초기화
        CurrentPage = 1;
        TotalCount = 0;

        // 계산 속성 변경 알림
        OnPropertyChanged(nameof(TotalPages));
        OnPropertyChanged(nameof(PageInfo));
        OnPropertyChanged(nameof(RangeInfo));
        OnPropertyChanged(nameof(CanGoPrevious));
        OnPropertyChanged(nameof(CanGoNext));
        OnPropertyChanged(nameof(AllCount));
    }
    public void CancelAndClear()
    {
        _loadCts?.Cancel();
        Clear();
    }

}
