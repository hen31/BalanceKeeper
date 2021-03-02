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
    /// Interaction logic for ImportTransactionsView.xaml
    /// </summary>
    public partial class SelectFilePage : UserControl
    {
        public SelectFilePage()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            (sender as TextBox).CaretIndex = (sender as TextBox).Text.Length;
            var rect = (sender as TextBox).GetRectFromCharacterIndex((sender as TextBox).CaretIndex);
            (sender as TextBox).ScrollToHorizontalOffset(rect.Right);
        }
    }
}
