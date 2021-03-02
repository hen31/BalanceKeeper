using BalanceKeeper.Core;
using BalanceKeeper.Views.ChildViews.ImportWizard;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Viewmodels.ImportWizard
{
    class ImportWizardViewmodel : ViewModel
    {
        MainViewmodel _mainViewmodel;
        private readonly IView _wizardView;

        public ImportWizardViewmodel(MainViewmodel mainViewmodel, IView wizardView)
        {
            _mainViewmodel = mainViewmodel;
            _wizardView = wizardView;
            CurrentPage = new SelectFilePageViewmodel(this);
            MainViewmodel = mainViewmodel;
        }
        ImportPage _currentPage;
        public ImportPage CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
            }
        }

        public MainViewmodel MainViewmodel { get; internal set; }

        internal void CloseWizard()
        {
            _wizardView.Close();
        }
    }
}
