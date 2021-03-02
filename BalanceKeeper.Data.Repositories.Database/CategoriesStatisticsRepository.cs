using BalanceKeeper.Data.Domain.Models;
using BalanceKeeper.Data.Entities;
using BalanceKeeper.Data.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BalanceKeeper.Data.Repositories.Database
{
    public class CategoriesStatisticsRepository : EFRepository, ICategoriesStatisticsRepository
    {

        public async Task<CategoriesStatistics> GetStatisticsAsync(DateTime from, DateTime to, List<Category> categoriesToAnalyse)
        {
            CategoriesStatistics statistics = new CategoriesStatistics();
            IQueryable<Transaction> transactions;
            if (categoriesToAnalyse == null || categoriesToAnalyse.Count <= 0)
            {
                transactions = DbContext.TransActions.Include(b => b.CategoryLinks).Include(b => b.Relation).Where(b => b.Date >= from && b.Date <= to && !b.Relation.OwnAccount);
            }
            else
            {
                transactions = DbContext.TransActions.Include(b => b.CategoryLinks).Include(b => b.Relation).Where(b => b.Date >= from && b.Date <= to & !b.Relation.OwnAccount && b.CategoryLinks.Where(l => categoriesToAnalyse.Where(ca => ca.ID == l.CategoryID).Count() > 0).Count() > 0);
            }

            var categories = transactions.SelectMany(b => b.CategoryLinks).Select(l => l.Category).Distinct().AsNoTracking();
            var categorieIDs = categories.Select(b => b.ID);
            categories = DbContext.Categories.Include(b => b.MainCategory).Where(b => categorieIDs.Contains(b.ID)).AsNoTracking();
            if (await transactions.Where(b => !b.CategoryLinks.Any(c => c.CategoryID != null && c.Percentage > 0)).CountAsync() > 0)
            {
                CategoryStatistic categoryStatistic = new CategoryStatistic();
                categoryStatistic.Category = new Category() { Name = "Niet gecategoriseerd" }; ;

                categoryStatistic.Transactions = new System.Collections.ObjectModel.ObservableCollection<Transaction>(transactions.Include(b => b.CategoryLinks).ThenInclude(b=> b.Category).Include(b => b.Relation).AsNoTracking().ToList().Where(b => b.CategoryLinks.Where(c => c.Category == null && c.Percentage > 0).Count() > 0).ToList());
                categoryStatistic.Balance = categoryStatistic.Transactions.Sum(b => b.Amount * (b.CategoryLinks.Where(c => c.CategoryID == null).Sum(c => c.Percentage) / 100));
                categoryStatistic.Spendings = categoryStatistic.Transactions.Where(b => b.Amount < 0).Sum(b => b.Amount * (b.CategoryLinks.Where(c => c.CategoryID == null).Sum(c => c.Percentage)/100));
                categoryStatistic.Income = categoryStatistic.Transactions.Where(b => b.Amount > 0).Sum(b => b.Amount * (b.CategoryLinks.Where(c => c.CategoryID == null).Sum(c => c.Percentage) / 100));
                statistics.CategoryStatistics.Add(categoryStatistic);
            }
            foreach (var category in categories)
            {
                CategoryStatistic categoryStatistic = new CategoryStatistic();
                categoryStatistic.Category = category;
                categoryStatistic.Transactions = new System.Collections.ObjectModel.ObservableCollection<Transaction>(transactions.Include(b => b.CategoryLinks).ThenInclude(b => b.Category).Include(b => b.Relation).AsNoTracking().ToList().Where(b => b.CategoryLinks.Where(c => c.CategoryID == category.ID).Count() > 0).ToList());
                categoryStatistic.Balance = categoryStatistic.Transactions.Sum(b => b.Amount * (b.CategoryLinks.Where(c => c.CategoryID == category.ID).Sum(c => c.Percentage) /100));
                categoryStatistic.Spendings = categoryStatistic.Transactions.Where(b => b.Amount < 0).Sum(b => b.Amount * (b.CategoryLinks.Where(c => c.CategoryID == category.ID).Sum(c => c.Percentage) / 100));
                categoryStatistic.Income = categoryStatistic.Transactions.Where(b => b.Amount > 0).Sum(b => b.Amount * (b.CategoryLinks.Where(c => c.CategoryID == category.ID).Sum(c => c.Percentage) / 100));
                //categoryStatistic.CalculateMonthlyStatistics();
                statistics.CategoryStatistics.Add(categoryStatistic);
            }
            statistics.UsedTransactions = await transactions.AsNoTracking().ToListAsync();

            return statistics;
        }
    }
}
