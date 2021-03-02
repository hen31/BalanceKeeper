using BalanceKeeper.Viewmodels;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BalanceKeeper.Views
{
    /// <summary>
    /// Interaction logic for StatisticsView.xaml
    /// </summary>
    public partial class StatisticsView : UserControl, IStatisticsView
    {
        public StatisticsView()
        {
            InitializeComponent();
        }



        private void PieChart_DataClick(object sender, LiveCharts.ChartPoint chartPoint)
        {
            var chart = (PieChart)chartPoint.ChartView;
            var selectedSeries = (PieSeries)chartPoint.SeriesView;
            (DataContext as StatisticsViewmodel).SelectedIncomeSeries = selectedSeries;
        }

        private void PieChart_DataClick_1(object sender, LiveCharts.ChartPoint chartPoint)
        {
            var chart = (PieChart)chartPoint.ChartView;
            var selectedSeries = (PieSeries)chartPoint.SeriesView;
            (DataContext as StatisticsViewmodel).SelectedSeries = selectedSeries;
        }
    }
}
