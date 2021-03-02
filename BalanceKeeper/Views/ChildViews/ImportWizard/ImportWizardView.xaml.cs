using BalanceKeeper.Viewmodels;
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

namespace BalanceKeeper.Views.ChildViews.ImportWizard
{
    /// <summary>
    /// Interaction logic for ImportWizard.xaml
    /// </summary>
    public partial class ImportWizardView : ChildWindow, IView
    {
        public ImportWizardView()
        {
            InitializeComponent();
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
