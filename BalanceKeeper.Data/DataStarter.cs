using System;
using System.Collections.Generic;
using System.Text;

namespace BalanceKeeper.Data
{
    public class DataStarter
    {
        public virtual void Start(Action initializeUserRepository)
        {
            var container = RepositoryResolver.GetContainer();

            // 2. Configure the container (register)
            // See below for more configuration examples
           // container.RegisterCollection(typeof(IRepository), AppDomain.CurrentDomain.GetAssemblies());
        }
    }
}
