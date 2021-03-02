using BalanceKeeper.Core;
using BalanceKeeper.Data;
using BalanceKeeper.Data.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Classes
{
    class CategoryTransactionLinkWrapper : ViewModel
    {
        private ObservableCollection<CategoryTransactionLinkWrapper> _wrappers;
        private Transaction _transaction;
        ObservableCollection<CategoryTransactionLink> _links;
        public CategoryTransactionLinkWrapper(Transaction transaction, CategoryTransactionLink link, 
            double percentage, double maxPercentage, ObservableCollection<CategoryTransactionLinkWrapper> wrappers,
            ObservableCollection<CategoryTransactionLink> links)
        {
            _percentage = percentage;
            Link = link;
            _transaction = transaction;
            _links = links;
            _wrappers = wrappers;
            _selectedCategory = link.Category;
            if (_selectedCategory == null)
            {
                CanBeDeleted = false;
            }
            else
            {
                CanBeDeleted = true;
            }

            DeleteCommand = new RelayCommand((obj) =>
            {
                _links.Remove(link);
                wrappers.Remove(this);
                CalculateNewPercentages(true);
                CalculateCategories(true);

            });

        }

        public CategoryTransactionLink Link { get; set; }
        public RelayCommand DeleteCommand { get; set; }

        double _percentage;
        public double Percentage
        {
            get
            {
                return _percentage;
            }
            set
            {
                _percentage = value;
                Link.Percentage = value;
                OnPropertyChanged(nameof(Percentage));
                CalculateNewPercentages();
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
                Link.Category = value;
                OnPropertyChanged(nameof(SelectedCategory));
                CalculateCategories();
                if (value == null)
                {
                    CanBeDeleted = false;
                }
                else
                {
                    CanBeDeleted = true;
                }
            }
        }

        private void CalculateCategories(bool dontIgnoreSelf = false)
        {
            if (_wrappers.Where(b => b.SelectedCategory == null).Count() == 0)
            {
                Transaction transaction = _transaction;
                CategoryTransactionLink link = new CategoryTransactionLink();
                link.TransactionID = transaction.ID;
                link.Transaction = transaction;
                link.UserId = ServiceResolver.GetService<IUserProvider>().GetUserId();
                link.Category = null;
                link.CategoryID = null;
                _links.Add(link);

                _wrappers.Add(new CategoryTransactionLinkWrapper(_transaction, link: link, percentage: 0, maxPercentage: 100 - _links.Where(c => c != link || dontIgnoreSelf).Sum(c => c.Percentage), wrappers: _wrappers, links: _links));
            }
        }

        public void OnlySetPercentage(double percentage)
        {
            _percentage = percentage;
            Link.Percentage = percentage;
            OnPropertyChanged(nameof(Percentage));
        }

        private void CalculateNewPercentages(bool dontIgnoreSelf = false)
        {

            double totalPercentage = _wrappers.Sum(b => b.Percentage);
            if (totalPercentage > 100)
            {
                double hasToBeLess = totalPercentage - 100;
                var currentWrapper = _wrappers.Last();
                while (currentWrapper == this && !dontIgnoreSelf)
                {
                    currentWrapper = _wrappers.Skip(_wrappers.IndexOf(currentWrapper) - 1).FirstOrDefault();
                }
                while (currentWrapper != null && _wrappers.Sum(b => b.Percentage) > 100)
                {
                    if (hasToBeLess > currentWrapper.Percentage)
                    {
                        hasToBeLess -= currentWrapper.Percentage;
                        currentWrapper.OnlySetPercentage(0);
                    }
                    else
                    {
                        currentWrapper.OnlySetPercentage(currentWrapper.Percentage - hasToBeLess);
                    }

                    do
                    {
                        currentWrapper = _wrappers.Skip(_wrappers.IndexOf(currentWrapper) - 1).FirstOrDefault();
                    } while (currentWrapper == this && !dontIgnoreSelf);
                }
            }
        }

        bool _canBeDeleted;
        public bool CanBeDeleted
        {
            get
            {
                return _canBeDeleted;
            }
            set
            {
                _canBeDeleted = value;
                OnPropertyChanged(nameof(CanBeDeleted));
            }
        }

    }
}
