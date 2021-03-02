using BalanceKeeper.Classes;
using BalanceKeeper.Core;
using BalanceKeeper.Data;
using BalanceKeeper.Data.Entities;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Viewmodels
{
    class RelationsViewmodel : RepositoryOverviewViewmodel<Relation>
    {
        private MainViewmodel _mainViewmodel;
        private ICategoryRepository _categoryRepository;
        private ITransactionRepository _transactionRepository;
        private IDialogCoordinator dialogCoordinator;
        public RelationsViewmodel(MainViewmodel mainViewmodel, IDialogCoordinator instance)
            : base(RepositoryResolver.GetRepository<IRelationRepository>())
        {
            dialogCoordinator = instance;
            _mainViewmodel = mainViewmodel;
            _categoryRepository = RepositoryResolver.GetRepository<ICategoryRepository>();
            _transactionRepository = RepositoryResolver.GetRepository<ITransactionRepository>();
            AddCategoryToRelationCommand = new RelayCommand(AddCategoryToRelation, CanAddCategoryToRelation);
            LoadCategories();
        }

        public void LoadCategories()
        {
            Categories = new NotifyTaskCompletion<ICollection<Category>>(_categoryRepository.GetCollectionAsync().ContinueWith((tsk) =>
            {
                var list = tsk.Result.ToList();
                SelectedCategory = list.FirstOrDefault();
                return (ICollection<Category>)list;
            }));
        }

        NotifyTaskCompletion<ICollection<Category>> _categories;
        public NotifyTaskCompletion<ICollection<Category>> Categories
        {
            get
            {
                return _categories;
            }
            set
            {
                _categories = value;
                OnPropertyChanged("Categories");
            }
        }

        protected override async Task<ObservableCollection<Relation>> CreateCurrentCollectionAsync()
        {
            ICollection<Relation> collection;
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                collection = await (Repository as IRelationRepository).GetCollectionAsync();
            }
            else
            {
                collection = await (Repository as IRelationRepository).GetCollectionAsync(SearchTerm);
            }
            return new ObservableCollection<Relation>(collection);

        }

        protected override void OnItemDeleted()
        {
            _mainViewmodel.TransactionsViewmodel.LoadRelations();
            CreateWrappers();
        }

        internal override bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(EditingItem.Result.Name);
        }

        string _searchTerm;
        public string SearchTerm
        {
            get
            {
                return _searchTerm;
            }
            set
            {
                _searchTerm = value;
                OnPropertyChanged("SearchTerm");
                ReloadCurrentCollection();
            }
        }

        protected override void StartingAdd()
        {
            InitializeLinkWrapperIfNonExists();
        }
  
        internal override void OnItemAdded()
        {
            _mainViewmodel.TransactionsViewmodel.LoadRelations();
            CreateWrappers();
        }

        internal override void OnItemUpdated()
        {
            _mainViewmodel.TransactionsViewmodel.LoadRelations();
            CreateWrappers();
            dialogCoordinator.ShowMessageAsync(this,
                "Toepassen wijzigingen categorieën",
                "Wilt u de standaard categorieën toepassen op alle transacties van deze relatie?"
                + Environment.NewLine+"Tranacties van deze relatie die al gewijzigde categorieën heeft worden niet overschreven.",
                MessageDialogStyle.AffirmativeAndNegative,
                new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Ja",
                   NegativeButtonText= "Nee"
                })
                .ContinueWith(b =>
                {
                    if (b.Result == MessageDialogResult.Affirmative)
                    {
                        _transactionRepository.SynchronizeTransactionAndRelationCategoriesAsync(EditingItem.Result.ID);
                    }
                });

        }

        Category _selectedCategory;
        public Category SelectedCategory
        {
            get
            {
                return _selectedCategory;
            }
            set
            {
                _selectedCategory = value;
                OnPropertyChanged("SelectedCategory");
            }
        }

        public RelayCommand AddCategoryToRelationCommand { get; set; }


        private bool CanAddCategoryToRelation(object obj)
        {
            return !ReadonlyEditingItem;
        }

        private void AddCategoryToRelation(object obj)
        {
            Relation relation = EditingItem.Result as Relation;
            CategoryRelationLink link = new CategoryRelationLink();
            link.RelationID = relation.ID;
            link.Relation = relation;
            link.CategoryID = SelectedCategory.ID;
            link.Category = SelectedCategory;
            relation.CategoryLinks.Add(link);

            LinkWrappers.Add(new CategoryRelationLinkWrapper(link: link, percentage: 0, maxPercentage: 100 - EditingItem.Result.CategoryLinks.Where(c => c != link).Sum(c => c.Percentage), wrappers: LinkWrappers, links: relation.CategoryLinks));
        }

        ObservableCollection<CategoryRelationLinkWrapper> _linkWrappers;
        public ObservableCollection<CategoryRelationLinkWrapper> LinkWrappers
        {
            get
            {
                return _linkWrappers;
            }
            set
            {
                _linkWrappers = value;
                OnPropertyChanged(nameof(LinkWrappers));
            }
        }

        protected override void SelectedItemChanged()
        {
            CreateWrappers();
        }

        protected override void AfterCancelEdit()
        {
            CreateWrappers();
        }

        private void CreateWrappers()
        {
            LinkWrappers = new ObservableCollection<CategoryRelationLinkWrapper>();
            if (SelectedItem != null)
            {
                foreach (var wrapper in SelectedItem.CategoryLinks.Select(b => new CategoryRelationLinkWrapper(link: b, percentage: b.Percentage, maxPercentage: 100 - SelectedItem.CategoryLinks.Where(c => c != b).Sum(c => c.Percentage), wrappers: LinkWrappers, links: SelectedItem.CategoryLinks)).ToList())
                {
                    LinkWrappers.Add(wrapper);
                }
            }
            LinkWrappers = new ObservableCollection<CategoryRelationLinkWrapper>(LinkWrappers.OrderByDescending(b => b.Link.CategoryID));
        }

        protected override void OnStartingEdit()
        {
            LinkWrappers = new ObservableCollection<CategoryRelationLinkWrapper>();
            if (EditingItem.Result != null)
            {
                foreach (var wrapper in EditingItem.Result.CategoryLinks.Select(b => new CategoryRelationLinkWrapper(link: b, percentage: b.Percentage, maxPercentage: 100 - EditingItem.Result.CategoryLinks.Where(c => c != b).Sum(c => c.Percentage), wrappers: LinkWrappers, links: EditingItem.Result.CategoryLinks)).ToList())
                {
                    LinkWrappers.Add(wrapper);
                }
                InitializeLinkWrapperIfNonExists();
            }
        }

        private void InitializeLinkWrapperIfNonExists()
        {
            if (LinkWrappers.Count == 0)
            {
                Relation relation = EditingItem.Result;
                CategoryRelationLink link = new CategoryRelationLink();
                link.RelationID = relation.ID;
                link.Relation = relation;
                link.Category = null;
                link.CategoryID = null;
                LinkWrappers.Add(new CategoryRelationLinkWrapper(link: link, percentage: 100, maxPercentage: 100, wrappers: LinkWrappers, links: EditingItem.Result.CategoryLinks));
            }
        }
    }
}
