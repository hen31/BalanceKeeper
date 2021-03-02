using BalanceKeeper.Data.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Data.Repositories
{
    public interface ICustomImportRepository : IRepository
    {
        Task<ImportResults> ImportTransactionsFromCSVAsync(string fileContent, string seperator, bool hasHeaders, string addText, int columnStatement, int columnDate, int columnAmount, int columnAddOrMinus, int columnAccountNumberTo, int columnAccountNumberFrom, int columnRelationName);
    }
}
