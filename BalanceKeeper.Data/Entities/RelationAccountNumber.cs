using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BalanceKeeper.Data.Entities
{
    public class RelationAccountNumber : IEntity
    {
        public long ID { get; set; }
        public string AccountNumber { get; set; }
        public long RelationID { get; set; }
        [JsonIgnore]
        public Relation Relation { get;set;}
        public string UserId { get; set; }
        public string GetDescription()
        {
            return AccountNumber;
        }

        public long GetId()
        {
            return ID;
        }
    }
}
