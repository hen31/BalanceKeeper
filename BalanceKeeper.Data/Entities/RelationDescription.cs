using Newtonsoft.Json;
using System;

namespace BalanceKeeper.Data.Entities
{
    public class RelationDescription : IEntity
    {
        public long ID { get; set; }
        public string Description { get; set; }
        public long RelationID { get; set; }
        [JsonIgnore]
        public Relation Relation { get; set; }
        public string UserId { get; set; }

        public string GetDescription()
        {
            return Description;
        }

        public long GetId()
        {
            return ID;
        }
    }
}