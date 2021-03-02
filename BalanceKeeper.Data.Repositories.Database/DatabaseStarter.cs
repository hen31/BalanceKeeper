using BalanceKeeper.Data.Repositories;
using SimpleInjector;
using System.Reflection;

namespace BalanceKeeper.Data.Repositories.Database
{
    public  class DatabaseStarter
    { 
        public static void RegisterDatatabaseRepositories()
        {
            var container = RepositoryResolver.GetContainer();

            // 2. Configure the container (register)
            // See below for more configuration examples
            container.Register(typeof(IINGRepository), typeof(INGRepository), Lifestyle.Transient);
            container.Register(typeof(IASNRepository), typeof(ASNRepository), Lifestyle.Transient);
            container.Register(typeof(ICustomImportRepository), typeof(CustomImportRepository), Lifestyle.Transient);
            container.Register(typeof(ICategoriesStatisticsRepository), typeof(CategoriesStatisticsRepository), Lifestyle.Transient);
        }

    }
}