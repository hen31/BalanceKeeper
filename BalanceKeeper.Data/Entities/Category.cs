using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BalanceKeeper.Data.Entities
{
    public class Category : IEntity
    {
        public long ID { get; set; }
        public string Name { get; set; } = "";
        public string UserId { get; set; }
        public long? MainCategoryID { get; set; }
        public MainCategory MainCategory { get; set; }
        public string ColorAsText { get; set; }

        public ObservableCollection<CategoryDescriptionPattern> MatchWithTransactionDescription { get; set; } = new ObservableCollection<CategoryDescriptionPattern>();

        
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
