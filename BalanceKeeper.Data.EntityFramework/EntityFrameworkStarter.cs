using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Text;

namespace BalanceKeeper.Data.EntityFramework
{
    public abstract class EntityFrameworkStarter : DataStarter
    {
        public static Guid SeedGuid { get; protected set; } = new Guid("{9c7af9d1-021a-41bc-be30-42a726c9ef56}");

        public Lifestyle UseLifestyle { get; set; }

        public override void Start(Action initializeUserRepository)
        {
            base.Start(initializeUserRepository);
            // 1. Create a new Simple Injector container
            var container = RepositoryResolver.GetContainer();

            // 2. Configure the container (register)
            // See below for more configuration examples
            container.Register<ITransactionRepository, EFTransactionRepository>(UseLifestyle);
            container.Register<IRelationRepository, EFRelationRepository>(UseLifestyle);
            container.Register<ICategoryRepository, EFCategoryRepository>(UseLifestyle);
            container.Register<IMainCategoryRepository, EFMainCategoryRepository>(UseLifestyle);
            
        }

        public virtual void TestSeedDb()
        {

        }
    }
}
