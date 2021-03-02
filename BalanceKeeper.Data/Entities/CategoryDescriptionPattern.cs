using Newtonsoft.Json;
using System;

namespace BalanceKeeper.Data.Entities
{
    public class CategoryDescriptionPattern : IEntity
    {

        public long ID { get; set; }
        public string Pattern { get; set; }
        public long CategoryID { get; set; }
        public double Percentage { get; set; }
        [JsonIgnore]
        public Category Category { get; set; }
        public string UserId { get; set; }
        public string GetDescription()
        {
            return Pattern;
        }

        public long GetId()
        {
            return ID;
        }
    }
}