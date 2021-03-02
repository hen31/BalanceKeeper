using BalanceKeeper.Viewmodels;
using BalanceKeeper.Views.ChildViews;
using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace BalanceKeeper.Views
{
    /// <summary>
    /// Interaction logic for ForgottenPasswordView.xaml
    /// </summary>
    public partial class ForgottenPasswordView : MetroWindow, IChangePasswordView
    {
        public ForgottenPasswordView()
        {
            InitializeComponent();
        }

        PasswordBox IChangePasswordView.PassBoxReg1 => PassBoxReg1;

        PasswordBox IChangePasswordView.PassBoxReg2 => PassBoxReg2;

        void IView.Close()
        {
            Close();
        }
        public void Disable()
        {
            IsEnabled = false;
        }
    }
}
