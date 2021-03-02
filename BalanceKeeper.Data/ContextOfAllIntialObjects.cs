using BalanceKeeper.Data.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Data
{
    public class ContextOfAllIntialObjects
    {
        public ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();
        public ObservableCollection<MainCategory> MainCategories { get; set; } = new ObservableCollection<MainCategory>();
    }
}
