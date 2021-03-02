using BalanceKeeper.Data.Domain.Models;
using BalanceKeeper.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Data.Repositories
{
    public interface ICategoriesStatisticsRepository
    {
        Task<CategoriesStatistics> GetStatisticsAsync(DateTime from, DateTime to, List<Category> categoriesToAnalyse);
    }
}
