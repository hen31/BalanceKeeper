using BalanceKeeper.Classes;
using BalanceKeeper.Core;
using BalanceKeeper.Data;
using BalanceKeeper.Data.EntityFramework.SQLite;
using BalanceKeeper.Data.Repositories.Database;
using BalanceKeeper.Repositories;
using BalanceKeeper.Services;
using BalanceKeeper.Views;
using CredentialManagement;
using Newtonsoft.Json.Linq;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BalanceKeeper.Viewmodels
{
    class LoginViewmodel : ViewModel
    {
        IView _view;
        public LoginViewmodel(IView view)
        {
            Properties.Settings settings = new Properties.Settings();
            EmailAdress = settings.LastUsername;
            ServiceResolver.GetContainer().Register<IUserProvider, DesktopUserProvider>(Lifestyle.Singleton);

            LoginTaskNotifier = new NotifyTaskCompletion<LoginResult>(Task.FromResult<LoginResult>(null));
            RegisterTaskNotifier = new NotifyTaskCompletion<RegisterResult>(Task.FromResult<RegisterResult>(null));
            _view = view;
            CloseCommand = new RelayCommand((object obj) =>
            {
                Environment.Exit(0);
            });


            LoginCommand = new RelayCommand((object obj) =>
            {
                PasswordBox passBox = obj as PasswordBox;

                if (!string.IsNullOrWhiteSpace(EmailAdress) && !string.IsNullOrWhiteSpace(passBox.Password))
                {
                    passBox.SelectAll();
                    StartLogin(EmailAdress, passBox.Password);
                }
            });

            RegisterCommand = new RelayCommand((object obj) =>
            {
                if (IsRegistering && !string.IsNullOrWhiteSpace(EmailAdress))
                {
                    var loginView = obj as LoginView;
                    ErrorMessage = string.Empty;
                    if (loginView.PassBoxReg1.Password != loginView.PassBoxReg2.Password)
                    {
                        ErrorMessage = "Wachtwoorden komen niet overeen";
                    }
                
                    if (ErrorMessage == string.Empty)
                    {
                        lastPassword = loginView.PassBoxReg1.Password;
                        RegisterTaskNotifier = new NotifyTaskCompletion<RegisterResult>(LoginRepository.Instance.Register(EmailAdress, loginView.PassBoxReg1.Password), CompletedRegistration);
                    }
                }
                else
                {
                    IsRegistering = true;
                }
            });

            CancelRegisterCommand = new RelayCommand((object obj) =>
            {
                IsRegistering = false;
            });

        }

        private void StartLogin(string email, string password)
        {
            lastPassword = password;
            LoginTaskNotifier = new NotifyTaskCompletion<LoginResult>(LoginRepository.Instance.Login(email, password), LoginTaskCompleted);
        }

        private void CompletedRegistration(object sender, EventArgs e)
        {
            if (RegisterTaskNotifier.Result.IsError)
            {
                if (RegisterTaskNotifier.Result.ErrorType == RegisterErrorType.ModelError)
                {
                    string errorMessage = string.Empty;
                    if (RegisterTaskNotifier.Result.Errors != null)
                    {
                        foreach (var error in RegisterTaskNotifier.Result.Errors)
                        {
                            if (!string.IsNullOrWhiteSpace(error.Description))
                            {
                                if (!string.IsNullOrWhiteSpace(errorMessage))
                                {
                                    errorMessage += Environment.NewLine;
                                }
                                errorMessage += error.Description;
                            }
                        }
                    }
                    ErrorMessage = errorMessage;
                }
                else
                {
                    ErrorMessage = "Geen verbinding met server";
                }
            }
            else
            {
                IsRegistering = false;
            }
        }

        private void LoginTaskCompleted(object sender, EventArgs e)
        {
            if (LoginTaskNotifier.Result.IsError)
            {
                if (LoginTaskNotifier.Result.ErrorType == LoginErrorType.NoConnection)
                {
                    ErrorMessage = "Geen verbinding met server";
                }
                else if (LoginTaskNotifier.Result.ErrorType == LoginErrorType.NoIdentity)
                {
                    ErrorMessage = "Geen verbinding met server";
                }
                else if (LoginTaskNotifier.Result.ErrorType == LoginErrorType.WrongCredentials)
                {
                    ErrorMessage = "Geen geldige gebruikersnaam/wachtwoord combinatie of email nog niet bevestigd" + Environment.NewLine + "Na 5 pogingen moet u 5 minuten wachten tot volgende poging.";
                }
            }
            else
            {


                Properties.Settings settings = new Properties.Settings();
                settings.LastUsername = EmailAdress;
                settings.Save();

  
                new MainWindow().Show();
                _view.Close();

            }
        }

        public bool AcceptTerms { get; set; }

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

        public bool _isRegistering;
        public bool IsRegistering
        {
            get
            {
                return _isRegistering;
            }
            set
            {
                ErrorMessage = string.Empty;
                _isRegistering = value;
                OnPropertyChanged(nameof(IsRegistering));
            }
        }

        NotifyTaskCompletion<LoginResult> _loginTaskNotifier;
        public NotifyTaskCompletion<LoginResult> LoginTaskNotifier
        {
            get
            {
                return _loginTaskNotifier;
            }
            set
            {
                _loginTaskNotifier = value;
                OnPropertyChanged(nameof(LoginTaskNotifier));
            }
        }


        NotifyTaskCompletion<RegisterResult> _registerTaskNotifier;
        private string lastPassword;

        public NotifyTaskCompletion<RegisterResult> RegisterTaskNotifier
        {
            get
            {
                return _registerTaskNotifier;
            }
            set
            {
                _registerTaskNotifier = value;
                OnPropertyChanged(nameof(RegisterTaskNotifier));
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
        public RelayCommand CancelRegisterCommand { get; set; }
        public RelayCommand RegisterCommand { get; set; }
        public RelayCommand ConfirmEmailCommand { get; set; }
        public RelayCommand LoginCommand { get; set; }
        public RelayCommand ShowTermsOfServiceCommand { get; set; }
        public RelayCommand CloseCommand { get; set; }
        public RelayCommand ForgottenPasswordCommand { get; set; }

    }
}
