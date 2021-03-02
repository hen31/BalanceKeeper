using BalanceKeeper.Data;
using BalanceKeeper.Data.EntityFramework.SQLite;
using BalanceKeeper.Data.Repositories;
using BalanceKeeper.Viewmodels;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BalanceKeeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static MainWindow CurrentMainWindow;
        public MainWindow()
        {
            InitializeComponent();
            CurrentMainWindow = this;
          
       /*    string csvText = File.ReadAllText(@"C:\Users\Hendrik\Downloads\NL74INGB0008438561_01-03-2017_25-04-2017.csv");
           IINGRepository repository = RepositoryResolver.GetRepository<IINGRepository>();
            if (repository.ImportTransactionsFromCSV(csvText).Result.Success)
            {
                var transactionRepository = RepositoryResolver.GetRepository<ITransactionRepository>();
               // var alleTransactions = transactionRepository.GetCollection().Result;
                int a = 0;
            }*/
            
            this.DataContext = new MainViewmodel();
        }
    }
}
