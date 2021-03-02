using BalanceKeeper.Data.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Data
{
    public interface ITransactionRepository : IDatabaseRepository<Transaction>
    {
        void SetHttpClient(HttpClient httpClient);
        Task<ICollection<Transaction>> GetCollection(DateTime fromDate, DateTime toDate);
        Task<ICollection<Transaction>> GetCollectionAsync(DateTime fromDate, DateTime toDate, long relationId, double fromAmount, double toAmount, long categoryId);
        Task<bool> SynchronizeTransactionAndRelationCategoriesAsync(long relationId);
        Task<bool> AlreadyImported(string line);
        Task<bool> CreateInitialFilling();
    }
}
