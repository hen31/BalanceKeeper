using BalanceKeeper.Classes;
using BalanceKeeper.Core;
using BalanceKeeper.Repositories;
using BalanceKeeper.Views;
using BalanceKeeper.Views.ChildViews;
using MahApps.Metro.SimpleChildWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Viewmodels
{
    public class ForgottenPasswordViewmodel : ViewModel
    {

        IView _view;
        bool _changePassword;
        public ForgottenPasswordViewmodel(IView view, string emailAdress, bool changePassword = false)
        {
            ApiCallNotifier = new NotifyTaskCompletion<object>(Task.FromResult<object>(null));
            _changePassword = changePassword;
            _view = view;
            EmailAdress = emailAdress;
            SendEmailCodeCommand = new RelayCommand((object obj) =>
            {
                if (!string.IsNullOrWhiteSpace(EmailAdress))
                {
                    ApiCallNotifier = new NotifyTaskCompletion<object>(LoginRepository.Instance.SendPasswordResetCode(EmailAdress, _changePassword).ContinueWith(t => (object)t.Result), SendResetCodeCompleted);
                }
            });

            ConfirmCommand = new RelayCommand((object obj) =>
            {
                IChangePasswordView forgotView = (IChangePasswordView)obj;
                ErrorMessage = string.Empty;
                if(string.IsNullOrWhiteSpace(forgotView.PassBoxReg1.Password))
                {
                    ErrorMessage += "Geen wachwoord ingevuld";
                }
                if (forgotView.PassBoxReg1.Password != forgotView.PassBoxReg2.Password)
                {
                    if(ErrorMessage != string.Empty)
                    {
                        ErrorMessage += Environment.NewLine;
                    }
                    ErrorMessage += "Wachtwoorden komen niet overeen";
                }
                if (string.IsNullOrWhiteSpace(EmailAdress))
                {
                    if (ErrorMessage != string.Empty)
                    {
                        ErrorMessage += Environment.NewLine;
                    }
                    ErrorMessage += "Geen emailadres ingevuld";
                }
                if (string.IsNullOrWhiteSpace(ResetCode))
                {
                    if (ErrorMessage != string.Empty)
                    {
                        ErrorMessage += Environment.NewLine;
                    }
                    ErrorMessage += "Geen code ingevuld";
                }
                if (string.IsNullOrWhiteSpace(ErrorMessage))
                {
                    ApiCallNotifier = new NotifyTaskCompletion<object>(LoginRepository.Instance.ResetPasswordAsync(EmailAdress, ResetCode, forgotView.PassBoxReg1.Password).ContinueWith(t => (object)t.Result), async (object sender, EventArgs e) =>  await ResetPasswordCompletedAsync(sender, e));
                }
            });

            BackCommand = new RelayCommand(async (object obj) =>
            {
                if (!_changePassword)
                {
                    LoginView loginView = new LoginView();
                    loginView.DataContext = new LoginViewmodel(loginView) { EmailAdress = EmailAdress };
                    loginView.Show();
                    view.Close();
                }
                else
                {
                    view.Close();
                    await ChildWindowManager.ShowChildWindowAsync(MainWindow.CurrentMainWindow, new AccountView() { IsModal = true });
                }
            });
        }

        private async Task ResetPasswordCompletedAsync(object sender, EventArgs e)
        {
            if (((ConfirmEmailResult)ApiCallNotifier.Result).IsError)
            {
                if (((ConfirmEmailResult)ApiCallNotifier.Result).ErrorType == ConfirmEmailErrorType.NoConnection)
                {
                    ErrorMessage = "Geen verbinding met server";
                }
                else if(((ConfirmEmailResult)ApiCallNotifier.Result).Errors != null && ((ConfirmEmailResult)ApiCallNotifier.Result).Errors.Count > 0)
                {
                    ErrorMessage = string.Empty;
                    foreach (var error in ((ConfirmEmailResult)ApiCallNotifier.Result).Errors)
                    {
                        if (!string.IsNullOrWhiteSpace(error.Description))
                        {
                            if (!string.IsNullOrWhiteSpace(ErrorMessage))
                            {
                                ErrorMessage += Environment.NewLine;
                            }
                            ErrorMessage += error.Description;
                        }
                    }
                }
            }
            else
            {
                if (!_changePassword)
                {
                    LoginView loginView = new LoginView();
                    loginView.DataContext = new LoginViewmodel(loginView) { EmailAdress = EmailAdress };
                    loginView.Show();
                    _view.Close();
                }
                else
                {
                    _view.Close();
                    await ChildWindowManager.ShowChildWindowAsync(MainWindow.CurrentMainWindow, new AccountView() { IsModal = true });

                }
            }
        }

        private void SendResetCodeCompleted(object sender, EventArgs e)
        {
            if (((ConfirmEmailResult)ApiCallNotifier.Result).IsError)
            {
                if(((ConfirmEmailResult)ApiCallNotifier.Result).ErrorType == ConfirmEmailErrorType.NoConnection)
                {
                    ErrorMessage = "Geen verbinding met server";
                }
                else
                {
                    ErrorMessage = "Kan geen code aanmaken";
                }
            }
            else
            {
                CodeRequested = true;
            }
        }

        bool _codeRequested;
        public bool CodeRequested
        {
            get
            {
                return _codeRequested;
            }
            set
            {
                _codeRequested = value;
                OnPropertyChanged(nameof(CodeRequested));
            }
        }

        public RelayCommand SendEmailCodeCommand { get; set; }
        public RelayCommand BackCommand { get; set; }
        public RelayCommand ConfirmCommand { get; set; }
        
        public string ResetCode { get; set; }

        NotifyTaskCompletion<object> _apiCallNotifier;
        public NotifyTaskCompletion<object> ApiCallNotifier
        {
            get
            {
                return _apiCallNotifier;
            }
            set
            {
                _apiCallNotifier = value;
                OnPropertyChanged(nameof(ApiCallNotifier));
            }
        }

        string _emailAdress;
        public string EmailAdress
        {
            get
            {
                return _emailAdress;
            }
            set
            {
                _emailAdress = value;
                OnPropertyChanged(nameof(EmailAdress));
            }
        }

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
    }
}
