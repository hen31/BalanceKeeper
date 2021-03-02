using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Classes
{
    public enum LoginErrorType { NoConnection, WrongCredentials, NoIdentity}
    public class LoginResult
    {
        public LoginErrorType ErrorType { get; internal set; }
        public bool IsError { get; internal set; }
    }
}
