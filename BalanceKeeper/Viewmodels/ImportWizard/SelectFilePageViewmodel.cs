using BalanceKeeper.Core;
using BalanceKeeper.Data;
using BalanceKeeper.Data.Domain.Models;
using BalanceKeeper.Data.Repositories;
using BalanceKeeper.Views.ChildViews.ImportWizard;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BalanceKeeper.Viewmodels.ImportWizard
{
    internal class SelectFilePageViewmodel : ImportPage
    {
        public Bank ING { get; } = new Bank() { ID = 1, Name = "ING", DescriptionControl= new INGDescription() };
        public Bank ASN { get; } = new Bank() { ID = 2, Name = "ASN", DescriptionControl= new UserControl() };
        public Bank CustomBank { get; } = new Bank() { ID = 999, Name = "Geavanceerd/andere bank", DescriptionControl= new AdvancedBankImport(), Seperator=",", HasHeaders=true };
        private IINGRepository _ingRepository;
        private IASNRepository _asnRepository;
        private ICustomImportRepository _customImportRepository;
        public SelectFilePageViewmodel(ImportWizardViewmodel wizardViewmodel) : base(wizardViewmodel)
        {
            Content = new SelectFilePage() { DataContext = this };
            OpenFileCommand = new RelayCommand(OpenFile);
            StartImportCommand = new RelayCommand(StartImport, CanStartImport);
            Banks = new List<Bank>();
            Banks.Add(ING);
            Banks.Add(ASN);
            CustomBank.DescriptionControl.DataContext = CustomBank;
            Banks.Add(CustomBank);
            _ingRepository = RepositoryResolver.GetRepository<IINGRepository>();
            _asnRepository = RepositoryResolver.GetRepository<IASNRepository>();
            _customImportRepository = RepositoryResolver.GetRepository<ICustomImportRepository>();
        }
        


        private bool CanStartImport(object obj)
        {
            return !string.IsNullOrWhiteSpace(SelectedFilePath) && SelectedBank != null && ImportingTask == null;
        }

        NotifyTaskCompletion<ImportResults> _importingTask;
        public NotifyTaskCompletion<ImportResults> ImportingTask
        {
            get
            {
                return _importingTask;
            }
            set
            {
                _importingTask = value;
                OnPropertyChanged("ImportingTask");
            }
        }

        private void StartImport(object obj)
        {
            string csvText = File.ReadAllText(SelectedFilePath);
            if (_selectedBank == ING)
            {
                ImportingTask = new NotifyTaskCompletion<ImportResults>(_ingRepository.ImportTransactionsFromCSVAsync(csvText), ImportCompleted, ImportFailed);
            }
            else if (_selectedBank == ASN)
            {
                ImportingTask = new NotifyTaskCompletion<ImportResults>(_asnRepository.ImportTransactionsFromCSVAsync(csvText), ImportCompleted, ImportFailed);
            }
            else if (_selectedBank == CustomBank)
            {
                ImportingTask = new NotifyTaskCompletion<ImportResults>(_customImportRepository.ImportTransactionsFromCSVAsync(csvText, SelectedBank.Seperator, SelectedBank.HasHeaders, SelectedBank.AddText, SelectedBank.ColumnStatement, SelectedBank.ColumnDate, SelectedBank.ColumnAmount, SelectedBank.ColumnAddOrMinus, SelectedBank.ColumnAccountNumberTo, SelectedBank.ColumnAccountNumberFrom, SelectedBank.ColumnRelationName), ImportCompleted, ImportFailed);

            }
        }

        private void ImportFailed(object sender, EventArgs e)
        {
            ErrorText = "Er is iets misgegaan met importeren probeer opnieuw" + Environment.NewLine + "Foutmelding:" +ImportingTask.ErrorMessage;
            ImportingTask = null;
        }

        private void ImportCompleted(object sender, EventArgs e)
        {
            if(ImportingTask.Result.Success)
            {
                CommandManager.InvalidateRequerySuggested();
                if (ImportingTask.Result.ImportedRelations.Count >0)
                {
                    NextText = "Nieuw aangemaakte relaties bekijken";
                }
                else
                {
                    NextText = "Afsluiten";
                }
            }
            else
            {
                NextText = "Afsluiten";
            }
        }

        //_ingRepository.ImportTransactionsFromCSV(txtTask.Result)


        private void OpenFile(object obj)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Bank export(*.csv) | *.csv";
            bool? result = dialog.ShowDialog();
            if ((result.HasValue) && (result.Value))
            {
                SelectedFilePath = dialog.FileName;
            }
        }

        protected override void GoToNextPage()
        {
            if (ImportingTask.Result.ImportedRelations.Count > 0)
            {
                _wizardViewmodel.CurrentPage = new ImportedRelationsPage(_wizardViewmodel, ImportingTask.Result);

            }
            else{

                _wizardViewmodel.MainViewmodel.RelationsViewmodel.ReloadCurrentCollection();
                _wizardViewmodel.MainViewmodel.TransactionsViewmodel.ReloadCurrentCollection();
                _wizardViewmodel.MainViewmodel.TransactionsViewmodel.LoadRelations();
                _wizardViewmodel.CloseWizard();
            }
        }

        protected override bool CanGoToNextPage()
        {
            return ImportingTask != null && ImportingTask.IsSuccessfullyCompleted && ImportingTask.Result.Success;

        }

        public List<Bank> Banks { get; }
        Bank _selectedBank;
        public Bank SelectedBank
        {
            get
            {
                return _selectedBank;
            }
            set
            {
                _selectedBank = value;
                OnPropertyChanged(nameof(SelectedBank));
            }
        }

        string _errorText;
        public string ErrorText
        {
            get
            {
                return _errorText;
            }
            set
            {
                _errorText = value;
                OnPropertyChanged(nameof(ErrorText));
            }
        }


        public RelayCommand OpenFileCommand { get; set; }
        public RelayCommand DownloadFileCommand { get; set; }
        public RelayCommand StartImportCommand { get; set; }


        string _selectedFilePath;

        public string SelectedFilePath
        {
            get
            {
                return _selectedFilePath;
            }
            set
            {
                _selectedFilePath = value;
                OnPropertyChanged("SelectedFilePath");
            }
        }

        internal class Bank
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Site { get; set; }
            public Control DescriptionControl { get; set; }
            public string Seperator { get;  set; }
            public bool HasHeaders { get; set; } = true;
            public string AddText { get; set; } = "Bij";

            public int ColumnDate { get; set; } = 1;
            public int ColumnAccountNumberFrom { get; set; } = 3;
            public int ColumnAccountNumberTo { get; set; } = 4;
            public int ColumnAmount { get; set; } = 7;
            public int ColumnAddOrMinus { get; set; } = 6;
            public int ColumnStatement { get; set; } = 9;
            public int ColumnRelationName { get; set; } = 2;
        }
    }
}