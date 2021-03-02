using System;
using System.Collections.Generic;
using System.Text;

namespace BalanceKeeper.Data
{
    public interface IUserProvider
    {
        string GetUserId();
        void SetUserId(string userId);
    }
}
