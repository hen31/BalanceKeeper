using BalanceKeeper.Data.Domain.Models;
using BalanceKeeper.Data.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Classes
{
    public class MainCategoriesStatistics
    {
        public MainCategory MainCategory { get; set; }
        public List<CategoryStatistic> CategoryStatistics { get; set; }
        public double Balance { get; set; }
        public double Spendings { get; set; }
        public double Income { get; set; }
    }
}
