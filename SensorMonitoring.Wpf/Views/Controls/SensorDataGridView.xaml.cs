using SensorMonitoring.Wpf.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SensorMonitoring.Wpf.Views.Controls
{
    /// <summary>
    /// SensorDataGridView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SensorDataGridView : UserControl
    {
        public SensorDataGridView()
        {
            InitializeComponent();
        }

        private void HistoryDataGrid_Sorting(
            object sender,
            DataGridSortingEventArgs e)
        {
            // WPF 기본 정렬 방지
            e.Handled = true;

            if (DataContext is not SensorDataGridViewModel vm)
                return;

            string column = e.Column.SortMemberPath;

            if (string.IsNullOrWhiteSpace(column))
                return;

            // ViewModel에서 전체 데이터 정렬
            var direction = vm.SortBy(column);

            // 기존 정렬 표시 제거
            if (sender is DataGrid grid)
            {
                foreach (var item in grid.Columns)
                {
                    item.SortDirection = null;
                }
            }

            // 현재 컬럼에 정렬 방향 표시
            e.Column.SortDirection = direction;
        }
    }
}
