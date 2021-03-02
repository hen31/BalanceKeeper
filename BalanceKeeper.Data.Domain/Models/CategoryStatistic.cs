using BalanceKeeper.Data.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BalanceKeeper.Data.Domain.Models
{
    public class CategoryStatistic 
    {
        public Category Category { get; set; }
        public ObservableCollection<Transaction> Transactions { get; set; }
        public double Balance { get; set; }
        public double Spendings { get; set; }
        public double Income { get; set; }
    }
}
