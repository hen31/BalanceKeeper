using BalanceKeeper.Classes;
using BalanceKeeper.Core;
using BalanceKeeper.Data;
using BalanceKeeper.Data.Entities;
using BalanceKeeper.Data.Repositories;
using BalanceKeeper.Viewmodels.ImportWizard;
using BalanceKeeper.Views.ChildViews.ImportWizard;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace BalanceKeeper.Viewmodels
{
    class TransActionsViewmodel : RepositoryOverviewViewmodel<Transaction>
    {
        private MainViewmodel _mainViewmodel;
        IRelationRepository _relationRepository;
        ICategoryRepository _categoryRepository;
        public TransActionsViewmodel(MainViewmodel mainViewmodel)
            : base(RepositoryResolver.GetRepository<ITransactionRepository>())
        {

            _mainViewmodel = mainViewmodel;
            _relationRepository = RepositoryResolver.GetRepository<IRelationRepository>();
            _categoryRepository = RepositoryResolver.GetRepository<ICategoryRepository>();
            FromDate = DateTime.Now.AddMonths(-1);
            ToDate = DateTime.Now;
            LoadRelations();
            LoadCategories();
            AddCategoryToTransactionCommand = new RelayCommand(AddCategoryToTransaction, CanAddCategoryToTransaction);
            ImportCommand = new RelayCommand(async (obj) => await ImportTransactionsAsync());
        }



        protected override async Task<ObservableCollection<Transaction>> CreateCurrentCollectionAsync()
        {
            if (FromDate != DateTime.MinValue || ToDate != DateTime.MinValue)
            {
                var collection = await (Repository as ITransactionRepository).GetCollectionAsync(FromDate, ToDate,
                    SelectedRelatie != null ? SelectedRelatie.ID : 0, FromAmount, ToAmount, SelectedFilterCategory != null ? SelectedFilterCategory.ID : 0);
                return new ObservableCollection<Transaction>(collection);
            }
            else
            {
                return new ObservableCollection<Transaction>();
            }
        }

        double _fromAmount;
        public double FromAmount
        {
            get
            {
                return _fromAmount;
            }
            set
            {
                _fromAmount = value;
                ReloadCurrentCollection();
            }
        }

        double _toAmount;
        public double ToAmount
        {
            get
            {
                return _toAmount;
            }
            set
            {
                _toAmount = value;
                ReloadCurrentCollection();
            }
        }

        DateTime _fromDate;
        public DateTime FromDate
        {
            get
            {
                return _fromDate;
            }
            set
            {
                _fromDate = value;
                OnPropertyChanged("FromDate");
                ReloadCurrentCollection();
            }
        }

        public void LoadRelations()
        {
            FilterRelations = new NotifyTaskCompletion<ICollection<Relation>>(_relationRepository.GetCollectionAsync().ContinueWith((tsk) =>
            {
                Relations = new ObservableCollection<Relation>(tsk.Result);
                var list = tsk.Result.ToList();
                var relation = new Relation() { Name = "Alle relaties" };
                list.Insert(0, relation);
                SelectedRelatie = relation;
                return (ICollection<Relation>)list;

            }));
        }

        public void LoadCategories()
        {
            Categories = new NotifyTaskCompletion<ICollection<Category>>(_categoryRepository.GetCollectionAsync(), (object sender, EventArgs e) =>
            {
                FilterCategories = new ObservableCollection<Category>(Categories.Result);
                var allCategories = new Category() { Name = "Alle categorieën", MainCategory = new MainCategory() { Name = " " } };
                FilterCategories.Insert(0, allCategories);
                var list = Categories.Result.ToList();
                SelectedCategory = list.FirstOrDefault();

            });
        }


        internal override bool CanSave()
        {
            return true;
        }



        DateTime _toDate;
        public DateTime ToDate
        {
            get
            {
                return _toDate;
            }
            set
            {
                _toDate = value;
                OnPropertyChanged("ToDate");
                ReloadCurrentCollection();

            }
        }


        NotifyTaskCompletion<ICollection<Relation>> _filterRelations;
        public NotifyTaskCompletion<ICollection<Relation>> FilterRelations
        {
            get
            {
                return _filterRelations;
            }
            set
            {
                _filterRelations = value;
                OnPropertyChanged("FilterRelations");
            }
        }
        ObservableCollection<Category> _filterCategories;
        public ObservableCollection<Category> FilterCategories
        {
            get
            {
                return _filterCategories;
            }
            set
            {
                _filterCategories = value;
                OnPropertyChanged("FilterCategories");
            }
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

        NotifyTaskCompletion<ICollection<Transaction>> _transactions;
        public NotifyTaskCompletion<ICollection<Transaction>> Transactions
        {
            get
            {
                return _transactions;
            }
            set
            {
                _transactions = value;
                OnPropertyChanged("Transactions");
            }
        }

        Relation _selectedRelatie;
        public Relation SelectedRelatie
        {
            get
            {
                return _selectedRelatie;
            }
            set
            {
                _selectedRelatie = value;
                ReloadCurrentCollection();
            }
        }

        Category _selectedFilterCategory;
        public Category SelectedFilterCategory
        {
            get
            {
                return _selectedFilterCategory;
            }
            set
            {
                _selectedFilterCategory = value;
                OnPropertyChanged("SelectedFilterCategory");
                ReloadCurrentCollection();
            }
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

        public RelayCommand AddCategoryToTransactionCommand { get; set; }
        public RelayCommand ImportCommand { get; set; }

        private async Task ImportTransactionsAsync()
        {
            var view = new ImportWizardView();
            view.IsModal = true;
            view.DataContext = new ImportWizardViewmodel(_mainViewmodel, view);
            await ChildWindowManager.ShowChildWindowAsync(MainWindow.CurrentMainWindow, view);
            ReloadCurrentCollection();
        }

        private bool CanAddCategoryToTransaction(object obj)
        {
            return !ReadonlyEditingItem;
        }

        private void AddCategoryToTransaction(object obj)
        {
            Transaction transaction = EditingItem.Result as Transaction;
            CategoryTransactionLink link = new CategoryTransactionLink();
            link.TransactionID = transaction.ID;
            link.Transaction = transaction;
            link.CategoryID = SelectedCategory.ID;
            link.Category = SelectedCategory;
            link.UserId = ServiceResolver.GetService<IUserProvider>().GetUserId();
            transaction.CategoryLinks.Add(link);

            LinkWrappers.Add(new CategoryTransactionLinkWrapper(transaction, link: link, percentage: 0, maxPercentage: 100 - EditingItem.Result.CategoryLinks.Where(c => c != link).Sum(c => c.Percentage), wrappers: LinkWrappers, links: transaction.CategoryLinks));
        }

        ObservableCollection<CategoryTransactionLinkWrapper> _linkWrappers;
        public ObservableCollection<CategoryTransactionLinkWrapper> LinkWrappers
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

        ObservableCollection<Relation> _relations;
        public ObservableCollection<Relation> Relations
        {
            get
            {
                return _relations;
            }
            set
            {
                _relations = value;
                OnPropertyChanged(nameof(Relations));
            }
        }

        protected override void SelectedItemChanged()
        {
            LinkWrappers = CreateWrappers(SelectedItem);
        }

        protected override void AfterCancelEdit()
        {
            LinkWrappers = CreateWrappers(SelectedItem);
        }

        internal override void OnItemAdded()
        {
            LinkWrappers = CreateWrappers(SelectedItem);
        }

        internal override void OnItemUpdated()
        {
            LinkWrappers = CreateWrappers(SelectedItem);
        }

        protected override void OnItemDeleted()
        {
            LinkWrappers = CreateWrappers(SelectedItem);
        }

        public ObservableCollection<CategoryTransactionLinkWrapper> CreateWrappers(Transaction transaction)
        {
            var newWrappers = new ObservableCollection<CategoryTransactionLinkWrapper>();
            if (transaction != null)
            {
                foreach (var wrapper in transaction.CategoryLinks.OrderByDescending(b => b.CategoryID).Select(b => new CategoryTransactionLinkWrapper(transaction, link: b, percentage: b.Percentage, maxPercentage: 100 - transaction.CategoryLinks.Where(c => c != b).Sum(c => c.Percentage), wrappers: newWrappers, links: transaction.CategoryLinks)).ToList())
                {
                    newWrappers.Add(wrapper);
                }
            }
            return newWrappers;
        }

        protected override void OnStartingEdit()
        {
            LinkWrappers = new ObservableCollection<CategoryTransactionLinkWrapper>();
            if (EditingItem.Result != null)
            {
                LinkWrappers = CreateWrappers(EditingItem.Result);
            }
        }

        protected override void StartingAdd()
        {
            InitializeLinkWrapperIfNonExists(LinkWrappers);
        }

        public  void InitializeLinkWrapperIfNonExists(ObservableCollection<CategoryTransactionLinkWrapper> linkWrappers)
        {
            if (linkWrappers.Count == 0)
            {
                Transaction transaction = EditingItem.Result;
                CategoryTransactionLink link = new CategoryTransactionLink();
                link.Transaction = transaction;
                link.TransactionID = transaction.ID;
                link.Category = null;
                link.CategoryID = null;
                linkWrappers.Add(new CategoryTransactionLinkWrapper(transaction: transaction, link: link, percentage: 100, maxPercentage: 100, wrappers: linkWrappers, links: EditingItem.Result.CategoryLinks));
            }
        }
    }
}
