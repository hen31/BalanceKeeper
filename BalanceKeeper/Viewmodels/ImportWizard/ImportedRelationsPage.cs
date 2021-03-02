using BalanceKeeper.Core;
using BalanceKeeper.Data.Domain.Models;
using BalanceKeeper.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Viewmodels.ImportWizard
{
    class ImportedRelationsPage : ImportPage
    {

        public ImportedRelationsPage(ImportWizardViewmodel wizardViewmodel, ImportResults importResult) : base(wizardViewmodel)
        {
            Content = new Views.ChildViews.ImportWizard.ImportedRelationsPage() { DataContext = this };
            _importResult = importResult;
            NextText = "Afronden";

            EditRelationCommand = new RelayCommand(EditRelation);
        }

        private void EditRelation(object obj)
        {
            Relation editRelation = (Relation)obj;
            _wizardViewmodel.CurrentPage = new EditRelationWizardPageViewmodel(_wizardViewmodel, editRelation, this);
        }

        public RelayCommand EditRelationCommand { get; set; }

        ImportResults _importResult;
        public ImportResults ImportResult
        {
            get
            {
                return _importResult;
            }
            set
            {
                _importResult = value;
                OnPropertyChanged(nameof(ImportResult));
            }
        }
        protected override bool CanGoToNextPage()
        {
            return true;
        }

        protected override void GoToNextPage()
        {
            _wizardViewmodel.MainViewmodel.StatisticsViewmodel.RefreshCommand.Execute(null);
            _wizardViewmodel.MainViewmodel.RelationsViewmodel.ReloadCurrentCollection();
            _wizardViewmodel.MainViewmodel.TransactionsViewmodel.ReloadCurrentCollection();
            _wizardViewmodel.MainViewmodel.TransactionsViewmodel.LoadRelations();
            _wizardViewmodel.CloseWizard();
        }
    }
}
