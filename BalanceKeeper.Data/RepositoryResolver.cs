using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Text;

namespace BalanceKeeper.Data
{
    public class RepositoryResolver
    {
        private Container _container;
        static private RepositoryResolver _instance;
        private RepositoryResolver()
        {
            
        }

        public static Container GetContainer()
        {
            if(GetInstance()._container == null)
            {
                GetInstance()._container = new Container();
            }
           return GetInstance()._container;
        }

        private static RepositoryResolver GetInstance()
        {
            if(_instance == null)
            {
                _instance = new RepositoryResolver();
            }
            return _instance;
        }

        public static T GetRepository<T>() where T : class
        {
           return GetContainer().GetInstance<T>();
        }

        public static void ResetContainer()
        {
            _instance = new RepositoryResolver();
        }
    }
}
