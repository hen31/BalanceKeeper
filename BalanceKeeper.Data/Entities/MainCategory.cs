using System;
using System.Collections.Generic;
using System.Text;

namespace BalanceKeeper.Data.Entities
{
    public class MainCategory : IEntity
    {
        public long ID { get; set; }
        public string Name { get; set; } = "";
        public string UserId { get; set; }
        public string GetDescription()
        {
            return Name;
        }

        public long GetId()
        {
            return ID;
        }
    }
}
