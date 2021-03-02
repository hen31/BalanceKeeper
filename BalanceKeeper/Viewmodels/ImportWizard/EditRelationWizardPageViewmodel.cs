using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BalanceKeeper.Classes;
using BalanceKeeper.Core;
using BalanceKeeper.Data;
using BalanceKeeper.Data.Entities;

namespace BalanceKeeper.Viewmodels.ImportWizard
{
    internal class EditRelationWizardPageViewmodel : ImportPage
    {
        private Relation editRelation;
        private ImportedRelationsPage importedRelationsPage;

        public EditRelationWizardPageViewmodel(ImportWizardViewmodel wizardViewmodel, Relation editRelation, ImportedRelationsPage importedRelationsPage)
            : base(wizardViewmodel)
        {
            UpdateTaskNotifier = new NotifyTaskCompletion<Relation>(Task.FromResult<Relation>(null));
            _wizardViewmodel = wizardViewmodel;
            this.editRelation = editRelation;
            this.importedRelationsPage = importedRelationsPage;
            Relation = new Relation();
            App.Mapper.Map<Relation, Relation>(editRelation, Relation);
            CancelCommand = new RelayCommand(Cancel);
            SaveCommand = new RelayCommand(Save);
            Categories = _wizardViewmodel.MainViewmodel.RelationsViewmodel.Categories.Result;
            CreateWrappers();
            InitializeLinkWrapperIfNonExists();
            Content = new Views.ChildViews.ImportWizard.EditRelationWizardPage() { DataContext = this };
        }

        private void CreateWrappers()
        {
            LinkWrappers = new ObservableCollection<CategoryRelationLinkWrapper>();
            if (Relation != null)
            {
                foreach (var wrapper in Relation.CategoryLinks.OrderBy(b=> b.Percentage).Select(b => new CategoryRelationLinkWrapper(link: b, percentage: b.Percentage, maxPercentage: 100 - Relation.CategoryLinks.Where(c => c != b).Sum(c => c.Percentage), wrappers: LinkWrappers, links: Relation.CategoryLinks)).ToList())
                {
                    LinkWrappers.Add(wrapper);
                }
            }
          //  LinkWrappers = new ObservableCollection<CategoryRelationLinkWrapper>(LinkWrappers.OrderByDescending(b => b.Link.CategoryID));
        }
        public ICollection<Category> Categories { get; set; }

        NotifyTaskCompletion _updateTaskNotifier;
        public NotifyTaskCompletion UpdateTaskNotifier
        {
            get
            {
                return _updateTaskNotifier;
            }
            set
            {
                _updateTaskNotifier = value;
                OnPropertyChanged(nameof(UpdateTaskNotifier));
            }
        }

        private void Save(object obj)
        {
            UpdateTaskNotifier = new NotifyTaskCompletion<Relation>(RepositoryResolver.GetRepository<IRelationRepository>().UpdateAsync(Relation.ID, Relation), OnRelationEdited);
        }

        private void OnRelationEdited(object sender, EventArgs e)
        {
            var indexOfItem = importedRelationsPage.ImportResult.ImportedRelations.IndexOf(editRelation);
            importedRelationsPage.ImportResult.ImportedRelations.Remove(editRelation);
            importedRelationsPage.ImportResult.ImportedRelations.Insert(indexOfItem, Relation);
            UpdateTaskNotifier = new NotifyTaskCompletion<bool>(RepositoryResolver.GetRepository<ITransactionRepository>().SynchronizeTransactionAndRelationCategoriesAsync(Relation.ID), OnRelationSynced);
        }

        private void OnRelationSynced(object sender, EventArgs e)
        {
            _wizardViewmodel.CurrentPage = importedRelationsPage;
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

        private void Cancel(object obj)
        {
            _wizardViewmodel.CurrentPage = importedRelationsPage;
        }

        public Relation Relation { get; set; }

        public RelayCommand CancelCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }

        protected override bool CanGoToNextPage()
        {
            return true;
        }

        protected override void GoToNextPage()
        {
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
        private void InitializeLinkWrapperIfNonExists()
        {
            if (LinkWrappers.Count == 0)
            {
                Relation relation = Relation;
                CategoryRelationLink link = new CategoryRelationLink();
                link.RelationID = relation.ID;
                link.Relation = relation;
                link.Category = null;
                link.CategoryID = null;
                LinkWrappers.Add(new CategoryRelationLinkWrapper(link: link, percentage: 100, maxPercentage: 100, wrappers: LinkWrappers, links: Relation.CategoryLinks));
            }
        }
    }
}