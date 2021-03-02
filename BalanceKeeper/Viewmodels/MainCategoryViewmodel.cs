using BalanceKeeper.Data;
using BalanceKeeper.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Viewmodels
{
    class MainCategoryViewmodel : RepositoryOverviewViewmodel<MainCategory>
    {
        private MainViewmodel _mainViewmodel;
        private CategoriesViewmodel _categoryViewmodel;
        
        public MainCategoryViewmodel(MainViewmodel mainViewmodel, CategoriesViewmodel categoryViewmodel)
            : base(RepositoryResolver.GetRepository<IMainCategoryRepository>())
        {
            _mainViewmodel = mainViewmodel;
            _categoryViewmodel = categoryViewmodel;
        }


        protected override void OnItemDeleted()
        {
            _mainViewmodel.TransactionsViewmodel.LoadCategories();
            _categoryViewmodel.LoadMainCategories();
            _categoryViewmodel.ReloadCurrentCollection();
        }

        internal override bool CanSave()
        {
            return EditingItem != null && EditingItem.Result != null && !string.IsNullOrWhiteSpace(EditingItem.Result.Name);
        }

        internal override void OnItemAdded()
        {
            _mainViewmodel.TransactionsViewmodel.LoadCategories();
            _categoryViewmodel.LoadMainCategories();
            _categoryViewmodel.ReloadCurrentCollection();
        }

        internal override void OnItemUpdated()
        {
            _mainViewmodel.TransactionsViewmodel.LoadCategories();
            _categoryViewmodel.LoadMainCategories();
            _categoryViewmodel.ReloadCurrentCollection();
        }
    }
}
