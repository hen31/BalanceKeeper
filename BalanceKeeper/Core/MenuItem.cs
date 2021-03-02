using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BalanceKeeper.Core
{
    public class MenuItem : ViewModel
    {
        string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }
        ViewModel _viewmodel;
        public ViewModel Viewmodel
        {
            get
            {
                return _viewmodel;
            }
            set
            {
                _viewmodel = value;
                OnPropertyChanged("Viewmodel");
            }
        }

        FrameworkElement _screen;
        public FrameworkElement View
        {
            get
            {
                return _screen;
            }
            set
            {
                _screen = value;
                OnPropertyChanged("View");
            }
        }

        

    }
}
