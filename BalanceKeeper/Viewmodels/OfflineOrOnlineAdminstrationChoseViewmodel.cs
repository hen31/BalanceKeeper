using BalanceKeeper.Core;
using BalanceKeeper.Data;
using BalanceKeeper.Data.EntityFramework.SQLite;
using BalanceKeeper.Repositories;
using BalanceKeeper.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Viewmodels
{
    class OfflineOrOnlineAdminstrationChoseViewmodel : ViewModel
    {
        private IView _view;
        public OfflineOrOnlineAdminstrationChoseViewmodel(IView view)
        {
            _view = view;


      
        

            OfflineCommand = new RelayCommand(async (object obj) =>
            {
                _view.Disable();
     
                SQLiteStarter starter = new SQLiteStarter();
                IUserProvider desktopUserProvider = ServiceResolver.GetContainer().GetInstance<IUserProvider>();
                desktopUserProvider.SetUserId(LoginRepository.Instance.CurrentUser.ID);
                starter.CreateDBIfNotExists();
               // starter.TestSeedDb();
                Properties.Settings settings = new Properties.Settings();
                if (RememberChoice)
                {
                    settings.ChoiceIsOnline = false;
                    settings.RememberOnlineChoice = true;
                    settings.Save();
                }
                else
                {
                    settings.RememberOnlineChoice = false;
                    settings.Save();
                }

                await RepositoryResolver.GetRepository<ITransactionRepository>().CreateInitialFilling();

                new MainWindow().Show();
                _view.Close();
            });

        }

        public bool RememberChoice { get; set; }

        public RelayCommand OnlineCommand { get; set; }
        public RelayCommand OfflineCommand { get; set; }
        public RelayCommand RightsCommand { get; set; }
    }
}
