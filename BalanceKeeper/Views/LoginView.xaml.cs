using BalanceKeeper.Viewmodels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace BalanceKeeper.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : MahApps.Metro.Controls.MetroWindow, IView
    {
        public LoginView()
        {
            InitializeComponent();
            this.DataContext = new LoginViewmodel(this);
        }

    
        public void Disable()
        {
            IsEnabled = false;
        }
    }
}
