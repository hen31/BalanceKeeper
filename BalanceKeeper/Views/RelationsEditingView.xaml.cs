using BalanceKeeper.Controls;
using BalanceKeeper.Data;
using BalanceKeeper.Data.Entities;
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

namespace BalanceKeeper.Views
{
    /// <summary>
    /// Interaction logic for RelationsEditingView.xaml
    /// </summary>
    public partial class RelationsEditingView : UserControl
    {
        public RelationsEditingView() 
        {
            
            InitializeComponent();
        }

        private void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new RelationDescription() { UserId = ServiceResolver.GetService<IUserProvider>().GetUserId() };
        }

        private void DataGrid_AddingNewItem_1(object sender, AddingNewItemEventArgs e)
        {
            e.NewItem = new RelationAccountNumber() { UserId = ServiceResolver.GetService<IUserProvider>().GetUserId() };
        }
    }
}
