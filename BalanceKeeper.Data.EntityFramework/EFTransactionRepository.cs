using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BalanceKeeper.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BalanceKeeper.Data.EntityFramework
{
    public class EFTransactionRepository : EFRepository<Transaction>, ITransactionRepository
    {

        public override Task<Transaction> GetItemByIdAsync(long id)
        {
            return GetItemById(id, new string[] { "CategoryLinks", "CategoryLinks.Category", "Relation" });
        }

        public override DbSet<Transaction> DbSet => DbContext.TransActions;

        public override async Task<Transaction> UpdateAsync(long id, Transaction entity)
        {
            await CheckAndSetChangedCategoriesAsync(entity);
            ResetCategoriesLinkPercentages(entity);
            return await base.UpdateAsync(id, entity);
        }

        public override Task<Transaction> AddAsync(Transaction entity)
        {
            ResetCategoriesLinkPercentages(entity);
            return base.AddAsync(entity);
        }
        private async Task CheckAndSetChangedCategoriesAsync(Transaction entity)
        {
            bool changed = false;
            var fromDb = await GetItemByIdAsync(entity.ID);
            foreach (var dbLink in fromDb.CategoryLinks.ToList())
            {
                if (entity.CategoryLinks.Where(b => b.CategoryID == dbLink.ID && b.Percentage == dbLink.Percentage).Count() == 0)
                {
                    changed = true;
                    break;
                }
            }
            entity.ChangedCategories = changed;
        }
        private void ResetCategoriesLinkPercentages(Transaction entity)
        {
            double totalPercentage = entity.CategoryLinks.Sum(b => b.Percentage);
            if (totalPercentage < 100)
            {
                CategoryTransactionLink categoryTransactionLink = new CategoryTransactionLink();
                categoryTransactionLink.Percentage = 100 - totalPercentage;
                categoryTransactionLink.Transaction = entity;
                categoryTransactionLink.TransactionID = entity.ID;
                categoryTransactionLink.UserId = ServiceResolver.GetService<IUserProvider>().GetUserId();
                entity.CategoryLinks.Add(categoryTransactionLink);
            }
            else if (totalPercentage > 100)
            {
                double hasToBeLess = totalPercentage - 100;
                var link = entity.CategoryLinks.Last();
                while (link != null && entity.CategoryLinks.Sum(b => b.Percentage) > 100)
                {
                    if (hasToBeLess > link.Percentage)
                    {
                        hasToBeLess -= link.Percentage;
                        link.Percentage = 0;
                    }
                    else
                    {
                        link.Percentage -= hasToBeLess;
                    }

                    link = entity.CategoryLinks.Skip(entity.CategoryLinks.IndexOf(link) - 1).FirstOrDefault();
                }
            }
        }

        public override async Task<ICollection<Transaction>> GetCollectionAsync()
        {
            return await DbContext.TransActions.Include(b => b.Relation).Include(b => b.CategoryLinks).ThenInclude(cl => cl.Category)
    .AsNoTracking().ToListAsync().ContinueWith(tsk => tsk.Result.OrderByDescending(b => b.Date).ToList());
        }

        public async Task<ICollection<Transaction>> GetCollection(DateTime fromDate, DateTime toDate)
        {
            return await DbContext.TransActions.Include(b => b.Relation).Include(b => b.CategoryLinks).ThenInclude(cl => cl.Category).AsNoTracking().ToListAsync().ContinueWith(tsk => tsk.Result.OrderByDescending(b => b.Date).ToList());
        }

        public async Task<ICollection<Transaction>> GetCollectionAsync(DateTime fromDate, DateTime toDate, long relatieId, double fromAmount, double toAmount, long categoryId)
        {
            return await DbContext.TransActions.Include(b => b.Relation).Include(b => b.CategoryLinks).ThenInclude(cl => cl.Category)
                .Where(b => relatieId == 0 || b.RelationID == relatieId)
                .Where(b => categoryId == 0 || b.CategoryLinks.Where(cl => cl.CategoryID == categoryId).Count() > 0)
                .Where(b => (fromAmount == 0 && toAmount == 0) || (b.Amount >= fromAmount && b.Amount <= toAmount))
                .Where(b => (b.Date >= fromDate && b.Date <= toDate))
                .AsNoTracking().ToListAsync().ContinueWith(tsk => tsk.Result.OrderByDescending(b => b.Date).ToList());

        }

        public async Task<bool> SynchronizeTransactionAndRelationCategoriesAsync(long relationId)
        {
            Relation relation = await RepositoryResolver.GetRepository<IRelationRepository>().GetItemByIdAsync(relationId);
            var transactions = await DbSet.Include(b => b.CategoryLinks).Include(b => b.Relation).Where(b => b.RelationID == relationId && b.ChangedCategories == false).ToListAsync();
            foreach (Transaction transaction in transactions)
            {
                transaction.CategoryLinks.Clear();
                foreach (var relLink in relation.CategoryLinks)
                {
                    transaction.CategoryLinks.Add(new CategoryTransactionLink()
                    {
                        CategoryID = relLink.CategoryID,
                        Percentage = relLink.Percentage,
                        TransactionID = transaction.ID,
                        UserId = ServiceResolver.GetService<IUserProvider>().GetUserId()
                    });
                }
                await UpdateAsync(transaction.ID, transaction);
            }
            return true;
        }

        public async Task<bool> AlreadyImported(string line)
        {
            line = line.Trim();
            return await DbSet.AnyAsync(b => b.ImportLine == line);
        }

        public async Task<bool> CreateInitialFilling()
        {
            var userId = ServiceResolver.GetService<IUserProvider>().GetUserId();
            DbContext.SetCurrentUser(userId);

            if (await DbContext.MainCategories.CountAsync() == 0)
            {
                string contentOfUserFile = ReadNewUserFile();
   
                ContextOfAllIntialObjects initialObjects = JsonConvert.DeserializeObject<ContextOfAllIntialObjects>(contentOfUserFile);
                Dictionary<long, long> _mainCategoriesMapping = new Dictionary<long, long>();
                foreach (var mainCategory in initialObjects.MainCategories)
                {
                    var beforeId = mainCategory.ID;
                    mainCategory.ID = 0;
                    mainCategory.UserId = userId;
                    DbContext.MainCategories.Add(mainCategory);
                    await DbContext.SaveChangesAsync();
                    _mainCategoriesMapping.Add(beforeId, mainCategory.ID);
                }

                foreach (var category in initialObjects.Categories)
                {
                    category.UserId = userId;
                    category.ID = 0;
                    category.MainCategory = null;
                    if (category.MainCategoryID.HasValue)
                    {
                        category.MainCategoryID = _mainCategoriesMapping[category.MainCategoryID.Value];
                    }
                    foreach (var matchingThinh in category.MatchWithTransactionDescription)
                    {
                        matchingThinh.UserId = userId;
                        matchingThinh.Percentage = 100;
                    }
                    DbContext.Categories.Add(category);
                    await DbContext.SaveChangesAsync();
                }
            }
            return true;
        }
        private string ReadNewUserFile()
        {
            var assembly = typeof(EFTransactionRepository).GetTypeInfo().Assembly;
            string file = string.Empty;
            using (var stream = assembly.GetManifestResourceStream("BalanceKeeper.Data.EntityFramework.NewUserContent.json"))
            {
                using (var reader = new StreamReader(stream))
                {
                    file = reader.ReadToEnd();

                }
            }
            return file;
        }

        public void SetHttpClient(HttpClient httpClient)
        {
            throw new NotImplementedException();
        }
    }
}
