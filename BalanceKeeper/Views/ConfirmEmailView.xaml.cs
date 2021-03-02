using BalanceKeeper.Viewmodels;
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
    /// Interaction logic for ConfirmEmailView.xaml
    /// </summary>
    public partial class ConfirmEmailView : MetroWindow, IView
    {
        public ConfirmEmailView()
        {
            InitializeComponent();
        }
        public void Disable()
        {
            IsEnabled = false;
        }
    }
}
