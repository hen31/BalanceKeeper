using BalanceKeeper.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BalanceKeeper.Viewmodels.ImportWizard
{
    abstract class ImportPage : ViewModel
    {
        protected ImportWizardViewmodel _wizardViewmodel;
        public ImportPage(ImportWizardViewmodel wizardViewmodel)
        {
            _wizardViewmodel = wizardViewmodel;
            NextPageCommand = new RelayCommand(obj => GoToNextPage(), obj => CanGoToNextPage());

        }
        protected abstract void GoToNextPage();
        protected abstract bool CanGoToNextPage();
        public RelayCommand NextPageCommand { get; set; }

        string _nextText;
        public string NextText
        {
            get
            {
                return _nextText;
            }
            set
            {
                _nextText = value;
                OnPropertyChanged(nameof(NextText));
            }
        }

        public FrameworkElement Content { get; set; }
    }
}
