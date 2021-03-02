using BalanceKeeper.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Data.EntityFramework
{
    public abstract class EFRepository
    {
        private BalanceKeeperDatabaseContext _dbContext;
        public BalanceKeeperDatabaseContext DbContext
        {
            get
            {
                if(_dbContext == null)
                {
                    _dbContext = ServiceResolver.GetDataProvider<IDataProvider>() as BalanceKeeperDatabaseContext;
                }
                return _dbContext;
            }
        }
    }

    public abstract class EFRepository<T> : EFRepository, IDatabaseRepository<T> where T : class, IEntity, new()
    {


        public static void ChangeDbContext(BalanceKeeperDatabaseContext databaseContext)
        {

        }

        public virtual T CreateNewObject()
        {
            T entity = new T();
            entity.UserId = ServiceResolver.GetService<IUserProvider>().GetUserId();
            return entity;
        }

        public abstract DbSet<T> DbSet { get; }
        public virtual async Task<T> AddAsync(T entity)
        {
            entity.UserId = ServiceResolver.GetService<IUserProvider>().GetUserId();
            BuildChangeGraph(entity);
            //await DbSet.AddAsync(entity);
            await DbContext.SaveChangesAsync();
            UntrackItem(entity);
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(T entity)
        {
            if (entity.UserId != ServiceResolver.GetService<IUserProvider>().GetUserId())
            {
                throw new UnauthorizedAccessException();
            }
            BeforeItemDeleted(entity);
            //DbContext.Attach(entity);
            DbContext.Entry(entity).State = EntityState.Deleted;
            DbSet.Remove(entity);
            await DbContext.SaveChangesAsync();
            return true;
        }
        public virtual void BeforeItemDeleted(T entity)
        {

        }

        public virtual async Task<ICollection<T>> GetCollectionAsync()
        {
            return await DbSet.AsNoTracking().ToListAsync();
        }

        public virtual Task<T> GetItemByIdAsync(long id)
        {
            return GetItemById(id, null);
        }

        public virtual async Task<T> GetItemById(long id, string[] includes)
        {
            if (includes == null || includes.Length == 0)
            {
                return await DbSet.FindAsync(id);
            }
            else
            {
                var querable = DbSet.Include(includes[0]);
                foreach (string include in includes.Skip(1))
                {
                    querable = querable.Include(include);
                }
                return await querable.FirstOrDefaultAsync(b => b.ID == id);
            }

        }

        public virtual void CancelEdit(T entity)
        {
            UntrackItem(entity);
        }

        public virtual void UntrackItem(T entity)
        {
            DbContext.Entry(entity).State = EntityState.Detached;
            var entries = DbContext.ChangeTracker.Entries().ToList();
            foreach (EntityEntry dbEntityEntry in entries)
            {
                dbEntityEntry.State = EntityState.Detached;
            }
        }

        public virtual async Task<T> UpdateAsync(long id, T entity)
        {
            UntrackItem(entity);
            if (entity.UserId != ServiceResolver.GetService<IUserProvider>().GetUserId())
            {
                throw new UnauthorizedAccessException();
            }
            BuildChangeGraph(entity);
            //DbSet.Update(entity);
            foreach (var collection in DbContext.Entry(entity).Collections.ToList())
            {
                var loadedEntity = DbSet.Where(b => b.ID == entity.ID).Include(collection.Metadata.Name).AsNoTracking().ToList().FirstOrDefault();
                var dbCollection = (loadedEntity.GetType().GetProperty(collection.Metadata.Name).GetValue(loadedEntity) as IEnumerable<IEntity>).ToList();
                var currenentValues = new List<IEntity>(collection.CurrentValue.Cast<IEntity>());
                foreach (var itemInDb in dbCollection)
                {
                    if (currenentValues.Where(b => b.ID == itemInDb.ID).Count() == 0)
                    {
                        DbContext.Remove(itemInDb);
                    }
                }
            }
            await DbContext.SaveChangesAsync();
            UntrackItem(entity);
            return await GetItemByIdAsync(entity.ID);
        }

        private void BuildChangeGraph(T entity)
        {
            DbContext.ChangeTracker.TrackGraph(entity, node =>
            {
                var entry = node.Entry;
                var childEntity = entry.Entity;

                if (entry.IsKeySet)
                {
                    entry.State = EntityState.Modified;
                }
                else
                {
                    entry.State = EntityState.Added;
                }

            });
        }

        public async Task<T> AddSlowAsync(T entity)
        {
            await Task.Delay(2500);
            return await AddAsync(entity);
        }
    }
}
