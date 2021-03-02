using BalanceKeeper.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Data.EntityFramework
{
    public class EFMainCategoryRepository : EFRepository<MainCategory>, IMainCategoryRepository
    {
        public override DbSet<MainCategory> DbSet => DbContext.MainCategories;

        public override Task<MainCategory> AddAsync(MainCategory entity)
        {
            return base.AddAsync(entity);
        }
    }
}
