using BalanceKeeper.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Data.EntityFramework
{
    public class EFRelationRepository : EFRepository<Relation>, IRelationRepository
    {

        public override Task<Relation> AddAsync(Relation entity)
        {
            ResetCategoriesLinkPercentages(entity);
            return base.AddAsync(entity);
        }

        public override Task<Relation> UpdateAsync(long id, Relation entity)
        {
            ResetCategoriesLinkPercentages(entity);
            return base.UpdateAsync(id, entity);
        }

       

        public override Task<Relation> GetItemByIdAsync(long id)
        {
            return GetItemById(id, new string[] { "CategoryLinks", "AccountNumbers", "CategoryLinks.Category", "Descriptions" });
        }

        public override DbSet<Relation> DbSet => DbContext.Relations;

        public Task<Relation> FindRelationByAccountNumber(string accountNumber)
        {
            return DbSet.Where(b => b.AccountNumbers.Where(ac => ac.AccountNumber == accountNumber).Count() > 0).SingleOrDefaultAsync();
        }

        public Task<Relation> FindRelationByDescription(string description)
        {
            return DbSet.Where(b => b.Descriptions.Where(ac => ac.Description == description).Count() == 1).SingleOrDefaultAsync();
        }

        public Task<Relation> FindRelationByName(string name)
        {
            return DbSet.Where(b => b.Name == name).SingleOrDefaultAsync();
        }

        public override async Task<ICollection<Relation>> GetCollectionAsync()
        {
            return await DbSet.AsNoTracking()
                .Include(b => b.AccountNumbers)
                .Include(b => b.Descriptions)
                .Include(b => b.CategoryLinks)
                .ThenInclude(cl => cl.Category).AsNoTracking().ToListAsync().ContinueWith(tsk => tsk.Result.OrderBy(b => b.Name).ToList());
        }

        public async Task<ICollection<Relation>> GetCollectionAsync(string searchTerm)
        {
            
            return await DbSet.Include(b => b.AccountNumbers).Include(b => b.Descriptions).Include(b => b.CategoryLinks).ThenInclude(cl => cl.Category).Where(b => EF.Functions.Like(b.Name, "%" + searchTerm + "%")
                                                                                                     || b.AccountNumbers.Any(ac => EF.Functions.Like(ac.AccountNumber, "%" + searchTerm + "%"))
                                                                                                     || b.Descriptions.Any(desc => EF.Functions.Like(desc.Description, "%" + searchTerm + "%"))).AsNoTracking().ToListAsync().ContinueWith(tsk => tsk.Result.OrderBy(b => b.Name).ToList());
        }

        private void ResetCategoriesLinkPercentages(Relation entity)
        {
            double totalPercentage = entity.CategoryLinks.Sum(b => b.Percentage);
            if (totalPercentage < 100)
            {
                CategoryRelationLink categoryTransactionLink = new CategoryRelationLink();
                categoryTransactionLink.Percentage = 100 - totalPercentage;
                categoryTransactionLink.Relation = entity;
                categoryTransactionLink.RelationID = entity.ID;
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
    }
}
