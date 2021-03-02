using BalanceKeeper.Data.Repositories.Database;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace BalanceKeeper.Data.EntityFramework.SQLite
{
    public class SQLiteStarter : EntityFrameworkStarter
    {
        public override void Start(Action initializeUserRepository)
        {
            UseLifestyle = Lifestyle.Singleton;

            base.Start(initializeUserRepository);

            Directory.CreateDirectory(BuildFolderPath());
            ServiceResolver.GetContainer().Register<IDataProvider>(() =>
            {
                initializeUserRepository.Invoke();
                DatabaseStarter.RegisterDatatabaseRepositories();
                return new SQLiteDatabaseContext(BuildConnectionString());
            }, Lifestyle.Singleton);

        }

        public void CreateDBIfNotExists()
        {
            SQLiteDatabaseContext context = ServiceResolver.GetContainer().GetInstance<IDataProvider>() as SQLiteDatabaseContext;
            //context.Database.ExecuteSqlCommand("DELETE FROM Transactions");
#if DEBUG
            context.Database.EnsureDeleted();
#endif
            context.Database.EnsureCreated();
            context.Database.Migrate();
            context.SaveChanges();
        }

        private string BuildConnectionString()
        {
            return string.Format("Data Source=\"{0}transactions98-{1}.db\"",
                BuildFolderPath(),
                ServiceResolver.GetService<IUserProvider>().GetUserId());
        }

        private string BuildFolderPath()
        {
            return string.Format("{0}\\BalanceKeeper\\",
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        }

        public override void TestSeedDb()
        {
            /*SQLiteDatabaseContext context = ServiceResolver.GetDataProvider<IDataProvider>() as SQLiteDatabaseContext;
            Entities.MainCategory wonen = new Entities.MainCategory() { Name = "Wonen", UserId = ServiceResolver.GetService<IUserProvider>().GetUserId() };
            Entities.MainCategory werken = new Entities.MainCategory() { Name = "Werken", UserId = ServiceResolver.GetService<IUserProvider>().GetUserId() };
            context.Categories.Add(new Entities.Category() { Name = "Hypotheek", MainCategoryID = wonen.ID, MainCategory = wonen, UserId = ServiceResolver.GetService<IUserProvider>().GetUserId() });
            context.Categories.Add(new Entities.Category() { Name = "Salaris", MainCategoryID = werken.ID, MainCategory = werken, UserId = ServiceResolver.GetService<IUserProvider>().GetUserId() });

            context.MainCategories.Add(wonen);
            context.MainCategories.Add(werken);
            context.SaveChanges();*/
        }

        public string ReadNewUserFile()
        {
            var assembly = typeof(SQLiteStarter).GetTypeInfo().Assembly;
            string file = string.Empty;
            using (var stream = assembly.GetManifestResourceStream("BalanceKeeper.Data.EntityFramework.SQLite.NewUserContent.json"))
            {
                using (var reader = new StreamReader(stream))
                {
                    file = reader.ReadToEnd();

                }
            }
            return file;
        }
    }
}
