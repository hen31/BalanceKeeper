using BalanceKeeper.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Data
{
    public interface ICategoryRepository : IDatabaseRepository<Category>
    {
        Task<ICollection<Category>> GetCollectionAsync(string searchTerm, long mainCategoryId);
        Task FindCategoriesForRelation(Transaction transaction, Relation relation);
    }
}
