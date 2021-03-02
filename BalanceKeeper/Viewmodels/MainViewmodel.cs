using BalanceKeeper.Core;
using BalanceKeeper.Views;
using BalanceKeeper.Views.ChildViews;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Viewmodels
{
    class MainViewmodel : ViewModel
    {
        public MainViewmodel()
        {
            TransactionsViewmodel = new TransActionsViewmodel(this);
            RelationsViewmodel = new RelationsViewmodel(this, DialogCoordinator.Instance);
            MenuItems = new List<MenuItem>();
            MenuItems.Add(new MenuItem()
            {
                Title = "Transacties",
                View = new TransactionsView() { DataContext= TransactionsViewmodel}
            });
            SelectedMenuItem = MenuItems.First();

            var statisticsView = new StatisticsView();
            StatisticsViewmodel = new StatisticsViewmodel(this, statisticsView);
            statisticsView.DataContext = StatisticsViewmodel;
            MenuItems.Add(new MenuItem()
            {
                Title = "Statistieken",
                View = statisticsView
            });

            MenuItems.Add(new MenuItem()
            {
                Title = "Relaties",
                View = new RelationsEditingView()
            });
            MenuItems.Last().View.DataContext = RelationsViewmodel;

            MenuItems.Add(new MenuItem()
            {
                Title = "Categorieen",
                View = new CategoriesView()
            });
            MenuItems.Last().View.DataContext = new CategoriesViewmodel(this);

            ShowAccountCommand = new RelayCommand(async (object obj)=> await ShowAccountAsync());
        }

        private async Task ShowAccountAsync()
        {
            await ChildWindowManager.ShowChildWindowAsync(MainWindow.CurrentMainWindow, new AccountView() { IsModal = true });
        }

        List<MenuItem> _menuItems;
        public List<MenuItem> MenuItems
        {
            get
            {
                return _menuItems;
            }
            set
            {
                _menuItems = value;
                OnPropertyChanged("MenuItems");
            }
        }

        MenuItem _selectedMenuItem;
        public MenuItem SelectedMenuItem
        {
            get
            {
                return _selectedMenuItem;
            }
            set
            {
                _selectedMenuItem = value;
                OnPropertyChanged("SelectedMenuItem");
            }
        }

        public RelayCommand ShowAccountCommand { get; set; }
        public TransActionsViewmodel TransactionsViewmodel { get; internal set; }
        public RelationsViewmodel RelationsViewmodel { get; internal set; }
        public StatisticsViewmodel StatisticsViewmodel { get; }
    }
}
