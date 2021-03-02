using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BalanceKeeper.Data.Entities
{
    public class CategoryRelationLink : IEntity
    {
        public long ID { get; set; }

        public long? CategoryID { get; set; }
        public Category Category { get; set; }
        public string UserId { get; set; }
        public long RelationID { get; set; }
        [JsonIgnore]
        public Relation Relation { get; set; }

        public double Percentage { get; set; }

        public string GetDescription()
        {
            return Category.Name + Percentage.ToString();
        }
    }
}
