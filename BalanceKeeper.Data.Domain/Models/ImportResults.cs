using BalanceKeeper.Data.Entities;
using System;
using System.Collections.ObjectModel;

namespace BalanceKeeper.Data.Domain.Models
{
    public class ImportResults
    {
        public bool Success { get; set; }
        public ObservableCollection<Transaction> ImportedTransactions { get; set; } = new ObservableCollection<Transaction>();
        public ObservableCollection<Relation> ImportedRelations { get; set; } = new ObservableCollection<Relation>();
        public int AlreadyImported { get; set; }
    }
}
