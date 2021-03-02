using Newtonsoft.Json;
using System;

namespace BalanceKeeper.Data.Entities
{
    public class CategoryTransactionLink : IEntity
    {
        public long ID { get; set; }

        public long? CategoryID { get; set; }
        public Category Category { get; set; }

        public long TransactionID { get; set; }
        [JsonIgnore]
        public Transaction Transaction { get; set; }
        public string UserId { get; set; }
        public double Percentage { get; set; }

        public string GetDescription()
        {
            return Category.Name;
        }
    }
}