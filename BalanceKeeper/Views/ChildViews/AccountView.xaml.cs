using BalanceKeeper.Viewmodels;
using BalanceKeeper.Viewmodels.ImportWizard;
using MahApps.Metro.SimpleChildWindow;
using System;
using System.Collections.Generic;
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

namespace BalanceKeeper.Views.ChildViews
{
    /// <summary>
    /// Interaction logic for AccountView.xaml
    /// </summary>
    public partial class AccountView : ChildWindow, IView
    {
        public AccountView()
        {
            InitializeComponent();
            DataContext = new AccountViewmodel(this);
        }

        public void Close()
        {
            this.Close(true);
        }

        public void Disable()
        {
            IsEnabled = false;
        }
    }
}
