using System;
using System.Collections.Generic;
using System.Text;
using BalanceKeeper.Data.Entities;

namespace BalanceKeeper.Data.Domain.Models
{
    public class CategoriesStatistics
    {
        public CategoriesStatistics()
        {
            UsedTransactions = new List<Transaction>();
            CategoryStatistics = new List<CategoryStatistic>();
        }
        public List<Transaction> UsedTransactions { get; set; }
        public List<CategoryStatistic> CategoryStatistics { get; set; }
    }
}
