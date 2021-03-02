using System;
using System.Collections.Generic;
using System.Text;
using BalanceKeeper.Data.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BalanceKeeper.Data.EntityFramework.SQLite
{
    public class SQLiteDatabaseContext :  BalanceKeeperDatabaseContext
    {
        string _dataSource;
        public SQLiteDatabaseContext(string dataSource)
        {
            _dataSource = dataSource;
            InitializeDbContext();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlite(_dataSource);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
