using BalanceKeeper.Converters;
using BalanceKeeper.Core;
using BalanceKeeper.Data;
using BalanceKeeper.Data.Entities;
using BalanceKeeper.Views;
using MahApps.Metro.SimpleChildWindow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BalanceKeeper.Viewmodels
{
    internal class CategoriesViewmodel : RepositoryOverviewViewmodel<Category>
    {
        private MainViewmodel _mainViewmodel;
        private IMainCategoryRepository _mainCategoriesRepository;
        public CategoriesViewmodel(MainViewmodel mainViewmodel)
            : base(RepositoryResolver.GetRepository<ICategoryRepository>())
        {
            _mainViewmodel = mainViewmodel;
            _mainCategoriesRepository = RepositoryResolver.GetRepository<IMainCategoryRepository>();
            LoadMainCategories();
            _searchTerm = string.Empty;
            MaintainMainCategoriesCommand = new RelayCommand(async (obj) => await MaintainMainCategoriesAsync(), CanMaintainMainCategories);
        }

        protected override void SelectedItemChanged()
        {
            base.SelectedItemChanged();
            if (SelectedItem != null)
            {
                SelectedColor = (Color)TextToColorConverter.Instance.Convert(SelectedItem.ColorAsText, null, null, null);
            }
        }

        public void LoadMainCategories()
        {
            MainCategories = new NotifyTaskCompletion<ObservableCollection<MainCategory>>(_mainCategoriesRepository.GetCollectionAsync().ContinueWith(tsk =>
            {
                var collection = new ObservableCollection<MainCategory>(tsk.Result);
                collection.Insert(0, new MainCategory() { ID = 0, Name = "Alle hoofdcategorieen" });
                return collection;
            }
                ));
            MainCategoriesForEdit = new NotifyTaskCompletion<ObservableCollection<MainCategory>>(_mainCategoriesRepository.GetCollectionAsync().ContinueWith(tsk =>
                {
                    var collection = new ObservableCollection<MainCategory>(tsk.Result);
                    return collection;
                }
                ));
        }

        Color _selectedColor;
        public Color SelectedColor
        {
            get
            {
                return _selectedColor;
            }
            set
            {
                _selectedColor = value;
                if(EditingItem!=null && EditingItem.Result !=null)
                {
                    EditingItem.Result.ColorAsText = TextToColorConverter.Instance.ConvertBack(_selectedColor, null, null, null) as string;
                }
                OnPropertyChanged();
            }
        }

        protected override void OnItemDeleted()
        {
            _mainViewmodel.TransactionsViewmodel.LoadCategories();
            _mainViewmodel.RelationsViewmodel.LoadCategories();
        }

        internal override bool CanSave()
        {
            return EditingItem != null && EditingItem.Result != null && !string.IsNullOrWhiteSpace(EditingItem.Result.Name) && EditingItem.Result.MainCategory != null;
        }

        protected override void OnBeforeItemAdded()
        {
            EditingItem.Result.MainCategory = null;
        }

        protected override void OnStartingEdit()
        {
            base.OnStartingEdit();
            SelectedColor = (Color)TextToColorConverter.Instance.Convert(EditingItem.Result.ColorAsText, null, null, null);
        }


        internal override void OnItemAdded()
        {
            _mainViewmodel.TransactionsViewmodel.LoadCategories();
            _mainViewmodel.RelationsViewmodel.LoadCategories();
        }

        internal override void OnItemUpdated()
        {
            _mainViewmodel.TransactionsViewmodel.LoadCategories();
            _mainViewmodel.RelationsViewmodel.LoadCategories();
        }

        NotifyTaskCompletion<ObservableCollection<MainCategory>> _mainCategories;
        public NotifyTaskCompletion<ObservableCollection<MainCategory>> MainCategories
        {
            get
            {
                return _mainCategories;
            }
            set
            {
                _mainCategories = value;
                OnPropertyChanged(nameof(MainCategories));
            }
        }


        protected override async Task<ObservableCollection<Category>> CreateCurrentCollectionAsync()
        {
            ICollection<Category> collection = await (Repository as ICategoryRepository).GetCollectionAsync(SearchTerm, MainCategory != null ? MainCategory.ID : 0);
            return new ObservableCollection<Category>(collection);
        }

        NotifyTaskCompletion<ObservableCollection<MainCategory>> _mainCategoriesForEdit;
        public NotifyTaskCompletion<ObservableCollection<MainCategory>> MainCategoriesForEdit
        {
            get
            {
                return _mainCategoriesForEdit;
            }
            set
            {
                _mainCategoriesForEdit = value;
                OnPropertyChanged(nameof(MainCategoriesForEdit));
            }
        }

        MainCategory _mainCategory;
        public MainCategory MainCategory
        {
            get
            {
                return _mainCategory;
            }
            set
            {
                _mainCategory = value;
                OnPropertyChanged(nameof(MainCategory));
                ReloadCurrentCollection();
            }
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


        public RelayCommand MaintainMainCategoriesCommand { get; set; }

        private async Task MaintainMainCategoriesAsync()
        {
            await ChildWindowManager.ShowChildWindowAsync(MainWindow.CurrentMainWindow, new MainCategoryOverview() { IsModal = true, DataContext = new MainCategoryViewmodel(_mainViewmodel, this) });
        }

        private bool CanMaintainMainCategories(object obj)
        {
            return !ReadonlyEditingItem;
        }
    }
}
