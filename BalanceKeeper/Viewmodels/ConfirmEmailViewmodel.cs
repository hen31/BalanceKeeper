using BalanceKeeper.Classes;
using BalanceKeeper.Core;
using BalanceKeeper.Repositories;
using BalanceKeeper.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Viewmodels
{
    class ConfirmEmailViewmodel : ViewModel
    {
        private IView _view;

        string _emailAdres;
        public string EmailAdres
        {
            get
            {
                return _emailAdres;
            }
            set
            {
                _emailAdres = value;
                OnPropertyChanged(nameof(EmailAdres));
            }
        }
        public string ConfirmCode { get; set; }
        string _errorMessage;
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
        public RelayCommand BackCommand { get; }
        public RelayCommand ConfirmCommand { get; }

        public ConfirmEmailViewmodel(string emailAdress, IView view)
        {
            _view = view;
            EmailAdres = emailAdress;
            ConfirmEmailNotifier = new NotifyTaskCompletion<ConfirmEmailResult>(Task.FromResult<ConfirmEmailResult>(null));
            BackCommand = new RelayCommand((object obj) =>
            {
                LoginView loginView = new LoginView();
                loginView.DataContext = new LoginViewmodel(loginView) { EmailAdress = EmailAdres };
                loginView.Show();
                view.Close();
            });

            ConfirmCommand = new RelayCommand((object obj) =>
            {
                if (!string.IsNullOrWhiteSpace(ConfirmCode) && !string.IsNullOrWhiteSpace(EmailAdres))
                {
                    ConfirmEmailNotifier = new NotifyTaskCompletion<ConfirmEmailResult>(LoginRepository.Instance.ConfirmEmailAdress(EmailAdres, ConfirmCode), OnConfirmedCompleted);
                }
            });

        }

        private void OnConfirmedCompleted(object sender, EventArgs e)
        {
            if (ConfirmEmailNotifier.Result.IsError)
            {
                if (ConfirmEmailNotifier.Result.ErrorType == ConfirmEmailErrorType.NoConnection)
                {
                    ErrorMessage = "Geen verbinding met server";
                }
                else if (ConfirmEmailNotifier.Result.ErrorType == ConfirmEmailErrorType.ModelError)
                {
                    ErrorMessage = "Geen geldige combinatie email/code";
                }
            }
            else
            {
                LoginView loginView = new LoginView();
                loginView.DataContext = new LoginViewmodel(loginView);
                loginView.Show();
                _view.Close();
            }
        }

        NotifyTaskCompletion<ConfirmEmailResult> _confirmEmailNotifier;
        public NotifyTaskCompletion<ConfirmEmailResult> ConfirmEmailNotifier
        {
            get
            {
                return _confirmEmailNotifier;
            }
            set
            {
                _confirmEmailNotifier = value;
                OnPropertyChanged(nameof(ConfirmEmailNotifier));
            }
        }
    }
}
