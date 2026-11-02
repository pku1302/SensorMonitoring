using CommunityToolkit.Mvvm.ComponentModel;
using SensorMonitoring.Contracts;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace SensorMonitoring.Wpf.ViewModels.Controls;

public partial class SensorSelectionViewModel : ObservableObject
{
    public ObservableCollection<SensorListItemDto> Sensors { get; } 
        = new();
    
    public ICollectionView FilteredSensors { get; }

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private SensorListItemDto? selectedSensor;

    public SensorSelectionViewModel()
    {
        FilteredSensors =
            CollectionViewSource.GetDefaultView(Sensors);

        FilteredSensors.Filter = FilterSensor;
    }

    private bool FilterSensor(object item)
    {
        if (item is not SensorListItemDto sensor)
            return false;

        if (string.IsNullOrWhiteSpace(SearchText))
            return true;

        return sensor.DeviceId.Contains(
            SearchText.Trim(),
            StringComparison.OrdinalIgnoreCase);
    }

    partial void OnSearchTextChanged(string value)
    {
        FilteredSensors.Refresh();
    }

    public void LoadSensors(
        IEnumerable<SensorListItemDto> sensors)
    {
        string? previousDeviceId = SelectedSensor?.DeviceId;

        Sensors.Clear();

        foreach (var sensor in sensors)
            Sensors.Add(sensor);

        SelectedSensor = Sensors.FirstOrDefault(
            x => x.DeviceId == previousDeviceId)
            ?? Sensors.FirstOrDefault();
    }
}
