﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BalanceKeeper.Views.ChildViews.ImportWizard
{
    /// <summary>
    /// Interaction logic for INGDescription.xaml
    /// </summary>
    public partial class INGDescription : UserControl
    {
        public INGDescription()
        {
            InitializeComponent();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://mijn.ing.nl");
        }
    }
}
