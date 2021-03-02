using BalanceKeeper.Classes;
using BalanceKeeper.Converters;
using BalanceKeeper.Core;
using BalanceKeeper.Data;
using BalanceKeeper.Data.Domain.Models;
using BalanceKeeper.Data.Entities;
using BalanceKeeper.Data.Repositories;
using BalanceKeeper.Views;
using LiveCharts;
using LiveCharts.Wpf;
using MahApps.Metro.SimpleChildWindow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BalanceKeeper.Viewmodels
{
    class StatisticsViewmodel : ViewModel
    {
        private MainViewmodel _mainViewmodel;
        private readonly IStatisticsView _statisticsView;
        private ICategoriesStatisticsRepository _repository;


        public StatisticsViewmodel(MainViewmodel mainViewmodel, IStatisticsView statisticsView)
        {
            _mainViewmodel = mainViewmodel;
            this._statisticsView = statisticsView;
            CurrentExecutingTasks = new ObservableCollection<TaskExecutorWrapper>();
            _repository = RepositoryResolver.GetRepository<ICategoriesStatisticsRepository>();
            FromDate = DateTime.Now.AddMonths(-1);
            ToDate = DateTime.Now;
            RemoveActionIfFaultedCommand = new RelayCommand(RemoveActionIfFaulted);
            RefreshCommand = new RelayCommand(Refresh);
            Refresh(null);
            EditTransactionCommand = new RelayCommand(async (obj) => await EditTransactionAsync((Transaction)obj));
        }

        private async Task EditTransactionAsync(Transaction transaction)
        {
            var view = new Views.ChildViews.EditTransactionWizard();
            view.IsModal = true;
            var vm = new EditTransactionWizardViewmodel(transaction, view, _mainViewmodel);
            view.DataContext = vm;
            await ChildWindowManager.ShowChildWindowAsync(MainWindow.CurrentMainWindow, view);
            if (vm.Result)
            {
                Refresh(null);
            }
            //Refresh(null);
        }

        private void Refresh(object obj)
        {
            StatsticsTask = new NotifyTaskCompletion<CategoriesStatistics>(_repository.GetStatisticsAsync(FromDate, ToDate, null), CategoriesStatisticsCompleted);
            CurrentExecutingTasks.Add(new TaskExecutorWrapper() { TaskNotifier = StatsticsTask, BusyText = "Bezig met statistieken ophalen" });
        }
        Transaction _lastSelectedTransaction;

        Transaction _selectedTransaction;
        public Transaction SelectedTransaction
        {
            get
            {
                return _selectedTransaction;
            }
            set
            {
                _selectedTransaction = value;
                if (_selectedTransaction != null)
                {
                    _lastSelectedTransaction = _selectedTransaction;
                }
                OnPropertyChanged();
            }
        }

        private void CategoriesStatisticsCompleted(object sender, EventArgs e)
        {
            SeriesCollection = new SeriesCollection();
            IncomeSeriesCollection = new SeriesCollection();
            CreateMainCategoryStatistics(StatsticsTask.Result.CategoryStatistics);



        }

        private void CreateMainCategoryStatistics(List<CategoryStatistic> categoryStatistics)
        {
            MainCategoryStatistics = new ObservableCollection<MainCategoriesStatistics>();
            MainCategoryIncomeSeriesCollection = new SeriesCollection();
            MainCategorySeriesCollection = new SeriesCollection();

            AddNonCategorisedStatistics(categoryStatistics);
            var mainCategories = categoryStatistics.Where(b => b.Category.MainCategory != null).Select(b => b.Category.MainCategory).GroupBy(b => b.ID).Select(b => b.First()).ToList();
            foreach (var mainCategory in mainCategories)
            {
                MainCategoriesStatistics statistics = new MainCategoriesStatistics();
                statistics.MainCategory = mainCategory;
                statistics.CategoryStatistics = categoryStatistics.Where(b => b.Category.MainCategoryID == mainCategory.ID).ToList();
                statistics.Income = statistics.CategoryStatistics.Sum(b => b.Income);
                statistics.Spendings = statistics.CategoryStatistics.Sum(b => b.Spendings);
                statistics.Balance = statistics.CategoryStatistics.Sum(b => b.Balance);
                MainCategoryStatistics.Add(statistics);
                var withColor = statistics.CategoryStatistics.Where(b => !string.IsNullOrWhiteSpace(b.Category.ColorAsText));

                if (statistics.Spendings < 0)
                {
                    var series = new PieSeries()
                    {
                        Title = statistics.MainCategory.Name,
                        LabelPoint = chartPoint => string.Format("{0:C} ({1:P})", chartPoint.Y, chartPoint.Participation),
                        DataLabels = true,
                        Tag = categoryStatistics,
                        Foreground = new SolidColorBrush(Colors.Black),
                        Values = new ChartValues<double>(new double[] { statistics.Spendings })
                    };
                    if (withColor.Count() > 0)
                    {
                        series.Fill = series.Fill = TextToColorConverter.Instance.Convert(withColor.First().Category.ColorAsText, typeof(Brush), null, null) as Brush;
                    }
                    MainCategorySeriesCollection.Add(series);
                }
                if (statistics.Income > 0)
                {
                    var series = new PieSeries()
                    {
                        Title = statistics.MainCategory.Name,
                        LabelPoint = chartPoint => string.Format("{0:C} ({1:P})", chartPoint.Y, chartPoint.Participation),
                        DataLabels = true,
                        Tag = statistics,
                        Foreground = new SolidColorBrush(Colors.Black),
                        Values = new ChartValues<double>(new double[] { statistics.Income })
                    };
                    if (withColor.Count() > 0)
                    {
                        series.Fill = series.Fill = TextToColorConverter.Instance.Convert(withColor.First().Category.ColorAsText, typeof(Brush), null, null) as Brush;
                    }
                    MainCategoryIncomeSeriesCollection.Add(series);
                }
            }

            if (_lastSelectedCategorieStatistic != null)
            {
                var lastSelected = _lastSelectedCategorieStatistic;
                if (lastSelected.Category.MainCategoryID == null)
                {
                    SelectedMainCategorieStatistic = MainCategoryStatistics.Where(b => b.MainCategory.ID == -1).FirstOrDefault();
                }
                else
                {
                    SelectedMainCategorieStatistic = MainCategoryStatistics.Where(b => b.MainCategory.ID == lastSelected.Category.MainCategoryID).FirstOrDefault(); ;
                }
                SelectedCategorieStatistic = SelectedMainCategorieStatistic.CategoryStatistics.Where(b => b.Category.ID == lastSelected.Category.ID).FirstOrDefault(); ;
                if (_lastSelectedTransaction != null)
                {
                    foreach (var transaction in SelectedCategorieStatistic.Transactions)
                    {
                        if (transaction.ID == _lastSelectedTransaction.ID)
                        {
                            SelectedTransaction = transaction;
                            break;
                        }
                    }
                }
            }
            else
            {
                SelectedMainCategorieStatistic = MainCategoryStatistics.FirstOrDefault();

            }
        }

        private void AddNonCategorisedStatistics(List<CategoryStatistic> categoryStatistics)
        {
            MainCategoriesStatistics leftOverTransactions = new MainCategoriesStatistics();
            leftOverTransactions.MainCategory = new MainCategory() { Name = "Niet gecategoriseerd", ID = -1 };
            leftOverTransactions.CategoryStatistics = categoryStatistics.Where(b => b.Category.MainCategoryID == null).ToList();
            if (leftOverTransactions.CategoryStatistics.Count > 0)
            {
                leftOverTransactions.Income = leftOverTransactions.CategoryStatistics.Sum(b => b.Income);
                leftOverTransactions.Spendings = leftOverTransactions.CategoryStatistics.Sum(b => b.Spendings);
                leftOverTransactions.Balance = leftOverTransactions.CategoryStatistics.Sum(b => b.Balance);
                MainCategoryStatistics.Add(leftOverTransactions);


                if (leftOverTransactions.Spendings < 0)
                {
                    var series = new PieSeries()
                    {
                        Title = leftOverTransactions.MainCategory.Name,
                        LabelPoint = chartPoint => string.Format("{0:C} ({1:P})", chartPoint.Y, chartPoint.Participation),
                        DataLabels = true,
                        Foreground = new SolidColorBrush(Colors.Black),
                        Tag = categoryStatistics,
                        Values = new ChartValues<double>(new double[] { leftOverTransactions.Spendings })
                    };
                    MainCategorySeriesCollection.Add(series);
                }
                if (leftOverTransactions.Income > 0)
                {
                    var series = new PieSeries()
                    {
                        Title = leftOverTransactions.MainCategory.Name,
                        LabelPoint = chartPoint => string.Format("{0:C} ({1:P})", chartPoint.Y, chartPoint.Participation),
                        DataLabels = true,
                        Foreground = new SolidColorBrush(Colors.Black),
                        Tag = leftOverTransactions,
                        Values = new ChartValues<double>(new double[] { leftOverTransactions.Income })
                    };
                 MainCategoryIncomeSeriesCollection.Add(series);
                }
            }
        }

        DateTime _fromDate;
        public DateTime FromDate
        {
            get
            {
                return _fromDate;
            }
            set
            {
                _fromDate = value;
                OnPropertyChanged("FromDate");
                Refresh(null);
            }
        }

        DateTime _toDate;
        public DateTime ToDate
        {
            get
            {
                return _toDate;
            }
            set
            {
                _toDate = value;
                OnPropertyChanged("ToDate");
                Refresh(null);

            }
        }

        private void RemoveActionIfFaulted(object obj)
        {
            TaskExecutorWrapper wrapper = (TaskExecutorWrapper)obj;
            if (wrapper.TaskNotifier != null && wrapper.TaskNotifier.IsFaulted)
            {
                CurrentExecutingTasks.Remove(wrapper);
            }
        }

        public RelayCommand RemoveActionIfFaultedCommand { get; set; }
        public RelayCommand RefreshCommand { get; set; }

        public ObservableCollection<TaskExecutorWrapper> CurrentExecutingTasks { get; set; }

        NotifyTaskCompletion<CategoriesStatistics> _statsticsTask;
        public NotifyTaskCompletion<CategoriesStatistics> StatsticsTask
        {
            get
            {
                return _statsticsTask;
            }
            set
            {
                _statsticsTask = value;
                OnPropertyChanged(nameof(StatsticsTask));
            }
        }

        MainCategoriesStatistics _selectedMainCategorieStatistic;
        public MainCategoriesStatistics SelectedMainCategorieStatistic
        {
            get
            {
                return _selectedMainCategorieStatistic;
            }
            set
            {
                _selectedMainCategorieStatistic = value;
                OnPropertyChanged(nameof(SelectedMainCategorieStatistic));
                if (_selectedMainCategorieStatistic != null)
                {
                    SelectedCategorieStatistic = SelectedMainCategorieStatistic.CategoryStatistics.FirstOrDefault();
                }
                SeriesCollection.Clear();
                IncomeSeriesCollection.Clear();
                if (_selectedMainCategorieStatistic != null)
                {
                    foreach (var categoryStatistics in _selectedMainCategorieStatistic.CategoryStatistics)
                    {
                        if (categoryStatistics.Spendings < 0)
                        {
                            var series = new PieSeries()
                            {
                                Title = categoryStatistics.Category.Name,
                                LabelPoint = chartPoint => string.Format("{0:C} ({1:P})", chartPoint.Y, chartPoint.Participation),
                                DataLabels = true,
                                Foreground = new SolidColorBrush(Colors.Black),
                                Tag = categoryStatistics,
                                Values = new ChartValues<double>(new double[] { categoryStatistics.Spendings }),

                            };
                            if (!string.IsNullOrWhiteSpace(categoryStatistics.Category.ColorAsText))
                            {
                                series.Fill = TextToColorConverter.Instance.Convert(categoryStatistics.Category.ColorAsText, typeof(Brush), null, null) as Brush;
                            }
                            SeriesCollection.Add(series);
                        }
                        if (categoryStatistics.Income > 0)
                        {
                            var series = new PieSeries()
                            {
                                Title = categoryStatistics.Category.Name,
                                LabelPoint = chartPoint => string.Format("{0:C} ({1:P})", chartPoint.Y, chartPoint.Participation),
                                DataLabels = true,
                                Foreground = new SolidColorBrush(Colors.Black),
                                Tag = categoryStatistics,
                                Values = new ChartValues<double>(new double[] { categoryStatistics.Income }),

                            };
                            if (!string.IsNullOrWhiteSpace(categoryStatistics.Category.ColorAsText))
                            {
                                series.Fill = TextToColorConverter.Instance.Convert(categoryStatistics.Category.ColorAsText, typeof(Brush), null, null) as Brush;
                            }
                            IncomeSeriesCollection.Add(series);
                        }
                    }
                }
            }
        }

        ObservableCollection<MainCategoriesStatistics> _mainCategoryStatistics;
        public ObservableCollection<MainCategoriesStatistics> MainCategoryStatistics
        {
            get
            {
                return _mainCategoryStatistics;
            }
            set
            {
                _mainCategoryStatistics = value;
                OnPropertyChanged(nameof(MainCategoryStatistics));
            }
        }

        SeriesCollection _selectedCategorySeriesCollection;
        public SeriesCollection SelectedCategorySeriesCollection
        {
            get
            {
                return _selectedCategorySeriesCollection;
            }
            set
            {
                _selectedCategorySeriesCollection = value;
                OnPropertyChanged("SelectedCategorySeriesCollection");
            }
        }

        SeriesCollection _mainCategorySeriesCollection;
        public SeriesCollection MainCategorySeriesCollection
        {
            get
            {
                return _mainCategorySeriesCollection;
            }
            set
            {
                _mainCategorySeriesCollection = value;
                OnPropertyChanged("MainCategorySeriesCollection");
            }
        }

        SeriesCollection _mainCategoryIncomeSeriesCollection;
        public SeriesCollection MainCategoryIncomeSeriesCollection
        {
            get
            {
                return _mainCategoryIncomeSeriesCollection;
            }
            set
            {
                _mainCategoryIncomeSeriesCollection = value;
                OnPropertyChanged("MainCategoryIncomeSeriesCollection");
            }
        }



        SeriesCollection _seriesCollection;
        public SeriesCollection SeriesCollection
        {
            get
            {
                return _seriesCollection;
            }
            set
            {
                _seriesCollection = value;
                OnPropertyChanged("SeriesCollection");
            }
        }

        SeriesCollection _incomeSeriesCollection;
        public SeriesCollection IncomeSeriesCollection
        {
            get
            {
                return _incomeSeriesCollection;
            }
            set
            {
                _incomeSeriesCollection = value;
                OnPropertyChanged("IncomeSeriesCollection");
            }
        }
        SeriesCollection _categoryIncomeExpenseSeriesCollection;
        public SeriesCollection CategoryIncomeExpenseSeriesCollection
        {
            get
            {
                return _categoryIncomeExpenseSeriesCollection;
            }
            set
            {
                _categoryIncomeExpenseSeriesCollection = value;
                OnPropertyChanged("CategoryIncomeExpenseSeriesCollection");
            }
        }


        PieSeries _selectedSeries;
        public PieSeries SelectedSeries
        {
            get
            {
                return _selectedSeries;
            }
            set
            {
                _selectedSeries = value;
                OnPropertyChanged(nameof(SelectedSeries));
                SelectedCategorieStatistic = SelectedSeries.Tag as CategoryStatistic;
            }
        }

        PieSeries _selectedIncomeSeries;
        public PieSeries SelectedIncomeSeries
        {
            get
            {
                return _selectedIncomeSeries;
            }
            set
            {
                _selectedIncomeSeries = value;
                OnPropertyChanged(nameof(SelectedIncomeSeries));
                SelectedCategorieStatistic = SelectedIncomeSeries.Tag as CategoryStatistic;
            }
        }
        CategoryStatistic _lastSelectedCategorieStatistic;
        CategoryStatistic _selectedCategorieStatistic;
        public CategoryStatistic SelectedCategorieStatistic
        {
            get
            {
                return _selectedCategorieStatistic;
            }
            set
            {
                if (value != _selectedCategorieStatistic)
                {
                    if (value != null)
                    {
                        _lastSelectedCategorieStatistic = value;
                    }
                    _selectedCategorieStatistic = value;
                    OnPropertyChanged(nameof(SelectedCategorieStatistic));

                    var categorySerie = new SeriesCollection();
                    List<string> monthsAndYears = new List<string>();
                    DateTime from = FromDate;
                    while (from <= ToDate)
                    {
                        monthsAndYears.Add(from.ToString("MM-yyyy"));
                        from = from.AddMonths(1);
                    }
                    Labels = monthsAndYears.ToArray();

                    var incomeSeries = new ColumnSeries()
                    {
                        Title = "Inkomen",
                        Values = new ChartValues<double>(),
                        Stroke = new SolidColorBrush(Colors.DarkGreen),
                        LabelPoint = chartPoint => string.Format("€{0:0.00}", chartPoint.Y),
                        Fill = new SolidColorBrush(Colors.DarkGreen),
                        Foreground = new SolidColorBrush(Colors.DarkGreen),

                    };

                    var expenseSeries = new ColumnSeries()
                    {
                        Title = "Uitgaven",
                        Values = new ChartValues<double>(),
                        Stroke = new SolidColorBrush(Colors.Red),
                        LabelPoint = chartPoint => string.Format("€{0:0.00}", chartPoint.Y),
                        Fill = new SolidColorBrush(Colors.Red),
                        Foreground = new SolidColorBrush(Colors.Red),
                    };

                    CategoryIncomeExpenseSeriesCollection = new SeriesCollection();

                    if (_selectedCategorieStatistic != null)
                    {
                        CategoryIncomeExpenseSeriesCollection.Add(new PieSeries()
                        {
                            Title = "Uitgaven",
                            LabelPoint = chartPoint => string.Format("€-{0}", chartPoint.Y),
                            DataLabels = true,
                            Foreground = new SolidColorBrush(Colors.Black),
                            Values = new ChartValues<double>(new double[] { Math.Abs(_selectedCategorieStatistic.Spendings) }),
                            Fill = new SolidColorBrush(Colors.Red),
                        });

                        CategoryIncomeExpenseSeriesCollection.Add(new PieSeries()
                        {
                            Title = "Inkomsten",
                            LabelPoint = chartPoint => string.Format("{0:C}", chartPoint.Y),
                            DataLabels = true,
                            Foreground = new SolidColorBrush(Colors.Black),
                            Values = new ChartValues<double>(new double[] { _selectedCategorieStatistic.Income }),
                            Fill = new SolidColorBrush(Colors.DarkGreen),
                        });

                        foreach (string monthYear in Labels)
                        {
                            var income = _selectedCategorieStatistic.Transactions.Where(b => b.Date.ToString("MM-yyyy") == monthYear && b.Amount > 0).Sum(b => b.Amount);
                            incomeSeries.Values.Add(income);

                            var expense = _selectedCategorieStatistic.Transactions.Where(b => b.Date.ToString("MM-yyyy") == monthYear && b.Amount < 0).Sum(b => b.Amount);
                            expenseSeries.Values.Add(Math.Abs(expense));
                        }
                    }
                    categorySerie.Add(incomeSeries);
                    categorySerie.Add(expenseSeries);
                    SelectedCategorySeriesCollection = categorySerie;
                    //clear selected slice.
                    foreach (PieSeries series in IncomeSeriesCollection)
                    {
                        if (_selectedCategorieStatistic != null && series.Tag == _selectedCategorieStatistic)
                            series.PushOut = 8;
                        else
                            series.PushOut = 0;
                    }
                    //clear selected slice.
                    foreach (PieSeries series in SeriesCollection)
                    {
                        if (_selectedCategorieStatistic != null && series.Tag == _selectedCategorieStatistic)
                            series.PushOut = 8;
                        else
                            series.PushOut = 0;
                    }
                }
            }

        }

        public RelayCommand EditTransactionCommand { get; set; }

        string[] _labels;
        public string[] Labels
        {
            get
            {
                return _labels;
            }
            set
            {
                _labels = value;
                OnPropertyChanged(nameof(Labels));
            }
        }

    }
}
