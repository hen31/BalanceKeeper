using BalanceKeeper.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BalanceKeeper.Data.EntityFramework
{
    public abstract class BalanceKeeperDatabaseContext : DbContext, IDataProvider
    {
        public void InitializeDbContext()
        {

        }

        public static readonly LoggerFactory MyLoggerFactory
    = new LoggerFactory();

        private string _currentUser;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //DebugLoggerFactoryExtensions.AddDebug(MyLoggerFactory);
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.EnableSensitiveDataLogging(true);
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            optionsBuilder.UseLoggerFactory(MyLoggerFactory);
            //ChangeTracker.AutoDetectChangesEnabled = false;
            //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Relation>()
                .HasQueryFilter(s => s.UserId == _currentUser)
                .HasMany(s => s.AccountNumbers)
                .WithOne(s => s.Relation)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Relation>()
                .HasKey(b => b.ID);

            modelBuilder.Entity<Relation>()
                         .HasQueryFilter(s => s.UserId == _currentUser)
                         .HasMany(s => s.Descriptions)
                         .WithOne(s => s.Relation)
                         .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transaction>()
             .HasQueryFilter(s => s.UserId == _currentUser)
             .HasOne(s => s.Relation)
             .WithMany()
             .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Transaction>()
                .HasQueryFilter(s => s.UserId == _currentUser)
              .HasKey(b => b.ID);



            modelBuilder.Entity<Category>()
                .HasQueryFilter(s => s.UserId == _currentUser)
            .HasOne(s => s.MainCategory)
            .WithMany()
            .HasForeignKey(b => b.MainCategoryID)
            .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Category>()
                .HasQueryFilter(s => s.UserId == _currentUser)
             .HasKey(b => b.ID);

            modelBuilder.Entity<MainCategory>().HasKey(b => b.ID);

            modelBuilder.Entity<MainCategory>()
                .HasQueryFilter(s => s.UserId == _currentUser)
            .HasMany<Category>()
            .WithOne(b => b.MainCategory)
            .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<CategoryTransactionLink>().HasKey(b => b.ID);
            modelBuilder.Entity<CategoryTransactionLink>()
                .HasQueryFilter(s => s.UserId == _currentUser)
            .HasOne(b => b.Transaction)
            .WithMany(s => s.CategoryLinks)
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey(b => b.TransactionID);

            modelBuilder.Entity<CategoryTransactionLink>()
                .HasQueryFilter(s => s.UserId == _currentUser)
            .HasOne(b => b.Category)
            .WithMany()
            .HasForeignKey(b => b.CategoryID);

            modelBuilder.Entity<CategoryRelationLink>()
                .HasKey(b => b.ID);

            modelBuilder.Entity<CategoryRelationLink>()
                .HasQueryFilter(s => s.UserId == _currentUser)
            .HasOne(b => b.Relation)
            .WithMany(s => s.CategoryLinks)
            .OnDelete(DeleteBehavior.Cascade)
            .HasForeignKey(b => b.RelationID);

            modelBuilder.Entity<CategoryRelationLink>()
            .HasOne(b => b.Category)
            .WithMany()
            .HasForeignKey(b => b.CategoryID);




        }

        public void SetCurrentUser(string userId)
        {
            _currentUser = userId;
        }

        public DbSet<Transaction> TransActions { get; set; }
        public DbSet<Relation> Relations { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<MainCategory> MainCategories { get; internal set; }


        public override int SaveChanges()
        {
            ThrowIfMultipleTenants();

            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ThrowIfMultipleTenants();

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfMultipleTenants();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfMultipleTenants();

            return base.SaveChangesAsync(cancellationToken);
        }

        private void ThrowIfMultipleTenants()
        {
            var ids = (from e in ChangeTracker.Entries()
                       where e.Entity is IEntity
                       select ((IEntity)e.Entity).UserId)
                       .Distinct()
                       .ToList();

            if (ids.Count == 0)
            {
                return;
            }

            if (ids.Count > 1)
            {
                throw new CrossTenantUpdateException();
            }

            if (ids.First() != _currentUser)
            {
                throw new CrossTenantUpdateException();
            }
        }
    }
}
