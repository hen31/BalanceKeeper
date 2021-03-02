using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Data
{
    public interface IDatabaseRepository<T> : IRepository
    {
        void CancelEdit(T entity);
        Task<T> AddAsync(T entity);
        Task<bool> DeleteAsync(T entity);
        Task<ICollection<T>> GetCollectionAsync();
        Task<T> GetItemByIdAsync(long id);
        Task<T> UpdateAsync(long id, T entity);
        Task<T> AddSlowAsync(T entity);
        void UntrackItem(T entity);
        T CreateNewObject();

    }
}
