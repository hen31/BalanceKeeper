using BalanceKeeper.Classes;
using BalanceKeeper.Core;
using BalanceKeeper.Data;
using BalanceKeeper.Repositories;
using BalanceKeeper.Views;
using BalanceKeeper.Views.ChildViews;
using MahApps.Metro.SimpleChildWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Viewmodels.ImportWizard
{
    class AccountViewmodel : ViewModel
    {
        IView _view;
        public AccountViewmodel(IView view)
        {
            _view = view;
            BackCommand = new RelayCommand((object obj) => view.Close());

            ChangePasswordCommand = new RelayCommand(async (object obj) => await ShowAccountAsync());

            LogoffCommand = new RelayCommand((object obj) =>
            {
                Properties.Settings autoLoginSettings = new Properties.Settings();
                autoLoginSettings.AutomaticallyLogin = false;
                autoLoginSettings.RememberOnlineChoice = false;
                autoLoginSettings.Save();
                ServiceResolver.ResetContainer();
                RepositoryResolver.ResetContainer();
                LoginRepository.Instance.ResetLogin();

                new LoginView().Show();
                MainWindow.CurrentMainWindow.Close();



            });
        }

        private async Task ShowAccountAsync()
        {
            _view.Close();
            await ChildWindowManager.ShowChildWindowAsync(MainWindow.CurrentMainWindow, new ChangePassswordView() { IsModal = true });
        }

        public User User
        {
            get
            {
                return LoginRepository.Instance.CurrentUser;
            }
        }

        public RelayCommand LogoffCommand { get; set; }
        public RelayCommand BackCommand { get; set; }
        public RelayCommand BuyLicenseCommand { get; }
        public RelayCommand ChangePasswordCommand { get; set; }


    }
}
