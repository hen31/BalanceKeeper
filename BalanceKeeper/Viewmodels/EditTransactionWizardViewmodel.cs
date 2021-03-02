using BalanceKeeper.Classes;
using BalanceKeeper.Core;
using BalanceKeeper.Data;
using BalanceKeeper.Data.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BalanceKeeper.Viewmodels
{
    internal class EditTransactionWizardViewmodel : ViewModel
    {
        private Transaction transaction;
        private IView _view;

        public bool Result { get; private set; }
        public ObservableCollection<Category> FilterCategories { get; private set; }
        public NotifyTaskCompletion<ICollection<Category>> Categories { get; private set; }
        public ObservableCollection<Relation> Relations { get; private set; }
        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }

        public EditTransactionWizardViewmodel(Transaction transaction, IView view, MainViewmodel mainViewmodel)
        {
            this.transaction = transaction;
            EditingItem = App.Mapper.Map<Transaction>(transaction);
            AnnulerenCommand = new RelayCommand((object obj) => { view.Close(); });
            SaveCommand = new RelayCommand(SaveTransaction);
            DeleteCommand = new RelayCommand(DeleteTransaction);
            EditTransactionNotifier = new NotifyTaskCompletion<Transaction>(Task.FromResult<Transaction>(null));
            _view = view;
            Result = false;

            FilterCategories = mainViewmodel.TransactionsViewmodel.FilterCategories;
            Categories = mainViewmodel.TransactionsViewmodel.Categories;
            Relations = mainViewmodel.TransactionsViewmodel.Relations;
            LinkWrappers = mainViewmodel.TransactionsViewmodel.CreateWrappers(EditingItem);
            mainViewmodel.TransactionsViewmodel.InitializeLinkWrapperIfNonExists(LinkWrappers);
        }

        private void DeleteTransaction(object obj)
        {
            EditTransactionNotifier = new NotifyTaskCompletion<bool>(RepositoryResolver.GetRepository<ITransactionRepository>().DeleteAsync(EditingItem),
                  (s, e) =>
                  {
                      Result = true;
                      _view.Close();
                  });
        }

        private void SaveTransaction(object obj)
        {
            EditTransactionNotifier = new NotifyTaskCompletion<Transaction>(RepositoryResolver.GetRepository<ITransactionRepository>().UpdateAsync(EditingItem.ID, EditingItem),
                    (s, e) =>
                    {
                        NotifyTaskCompletion<Transaction> sender = (NotifyTaskCompletion<Transaction>)s;
                        RepositoryResolver.GetRepository<ITransactionRepository>().UntrackItem(sender.Result);
                        Result = true;
                        _view.Close();
                    });
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

        private bool CanAddCategoryToTransaction(object obj)
        {
            return true;
        }

        private void AddCategoryToTransaction(object obj)
        {
            Transaction transaction = EditingItem as Transaction;
            CategoryTransactionLink link = new CategoryTransactionLink();
            link.TransactionID = transaction.ID;
            link.Transaction = transaction;
            link.CategoryID = SelectedCategory.ID;
            link.Category = SelectedCategory;
            link.UserId = ServiceResolver.GetService<IUserProvider>().GetUserId();
            transaction.CategoryLinks.Add(link);

            LinkWrappers.Add(new CategoryTransactionLinkWrapper(transaction, link: link, percentage: 0, maxPercentage: 100 - EditingItem.CategoryLinks.Where(c => c != link).Sum(c => c.Percentage), wrappers: LinkWrappers, links: transaction.CategoryLinks));
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

        public Transaction EditingItem { get; private set; }


        public RelayCommand AnnulerenCommand { get; set; }

        NotifyTaskCompletion _editTransactionNotifier;
        public NotifyTaskCompletion EditTransactionNotifier
        {
            get
            {
                return _editTransactionNotifier;
            }
            set
            {
                _editTransactionNotifier = value;
                OnPropertyChanged();
            }
        }

    }
}