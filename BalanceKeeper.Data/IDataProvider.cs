using System;

namespace BalanceKeeper.Data
{
    public interface IDataProvider
    {
        void SetCurrentUser(string userId);

    }
}