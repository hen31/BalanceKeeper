using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BalanceKeeper.Repositories;

namespace BalanceKeeper.Classes
{
    enum RegisterErrorType { NoConnection,
        ModelError
    }
    class RegisterResult
    {
        public bool IsError { get; internal set; }
        public RegisterErrorType ErrorType { get; set; }
        public List<IdentityError> Errors { get; internal set; }
        public bool Succes { get; internal set; }
    }
}
