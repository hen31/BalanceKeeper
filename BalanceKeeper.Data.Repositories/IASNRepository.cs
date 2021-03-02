using BalanceKeeper.Data.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Data.Repositories
{
    public interface IASNRepository : IRepository
    {
        Task<ImportResults> ImportTransactionsFromCSVAsync(string fileContent);
    }
}
