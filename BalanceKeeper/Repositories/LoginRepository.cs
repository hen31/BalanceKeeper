using BalanceKeeper.Classes;
using BalanceKeeper.Core;
using BalanceKeeper.Data;
using BalanceKeeper.Data.EntityFramework.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;

namespace BalanceKeeper.Repositories
{
    public class LoginRepository
    {
        private static LoginRepository _instance;
        public static LoginRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LoginRepository();
                }
                return _instance;
            }
        }

        internal User CurrentUser { get; private set; }

        public async Task<LoginResult> Login(string emailAdress, string password)
        {
            LoginResult result = new LoginResult();
            var credentials = CredentialUtil.GetCredential("BalanceKeeper.Desktop" + emailAdress);
            if (emailAdress != null && credentials != null && credentials.Username == emailAdress && password == credentials.Password)
            {
                result.IsError = false;
                SetCurrentUser(emailAdress);


                SQLiteStarter starter = new SQLiteStarter();
                starter.Start(new Action(() =>
                {
                    IUserProvider desktopUserProvider = ServiceResolver.GetContainer().GetInstance<IUserProvider>();
                    desktopUserProvider.SetUserId(LoginRepository.Instance.CurrentUser.ID);
                }));
                starter.CreateDBIfNotExists();

                await RepositoryResolver.GetRepository<ITransactionRepository>().CreateInitialFilling();
            }
            else
            {
                result.IsError = true;
                result.ErrorType = LoginErrorType.WrongCredentials;
            }
            return await Task<LoginResult>.FromResult(result);
        }

        internal void ResetLogin()
        {
            CurrentUser = null;
        }

        internal async Task<ConfirmEmailResult> ResetPasswordAsync(string emailAdress, string resetCode, string password)
        {
            return null;
        }

        internal async Task<RegisterResult> Register(string emailAdress, string password)
        {
            if (CredentialUtil.GetCredential("BalanceKeeper.Desktop" + emailAdress) != null)
            {
                return await Task<RegisterResult>.FromResult(new RegisterResult() { Succes = false, IsError = true, ErrorType = RegisterErrorType.ModelError, Errors = new List<IdentityError>() { new IdentityError() { Code = "UE", Description = "Gebruikersnaam bestaat al" } } });

            }
            CredentialUtil.SetCredentials("BalanceKeeper.Desktop" + emailAdress, emailAdress, password, CredentialManagement.PersistanceType.LocalComputer);

            return await Task<RegisterResult>.FromResult(new RegisterResult() { Succes = true });

        }


        public async Task<ConfirmEmailResult> ConfirmEmailAdress(string emailAdress, string code)
        {
            return null;
        }
        //SendResetCode

        public async Task<ConfirmEmailResult> SendPasswordResetCode(string emailAdress, bool changePassword)
        {
            return null;
        }

        public async Task<CheckLicenseResult> CheckLicense(string licenseKey)
        {
            return null;
        }

        private void SetCurrentUser(string username)
        {
            User user = new User();
            user.EmailAdress = username;
            user.ID = username;
            CurrentUser = user;
        }




    }

    //
    // Summary:
    //     Encapsulates an error from the identity subsystem.
    public class IdentityError
    {

        //
        // Summary:
        //     Gets or sets the code for this error.
        public string Code { get; set; }
        //
        // Summary:
        //     Gets or sets the description for this error.
        public string Description { get; set; }
    }
}
