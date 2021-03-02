using System;
using System.Collections.Generic;
using System.Text;

namespace BalanceKeeper.Data.Entities
{
    public interface IEntity
    {
        long ID { get; }
        string UserId { get; set; }

        string GetDescription();
    }
}
