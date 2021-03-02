using BalanceKeeper.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Data
{
    public interface IRelationRepository : IDatabaseRepository<Relation>
    {
        Task<ICollection<Relation>> GetCollectionAsync(string searchTerm);
        Task<Relation> FindRelationByName(string name);
        Task<Relation> FindRelationByAccountNumber(string accountNumber);
        Task<Relation> FindRelationByDescription(string description);
    }
}
