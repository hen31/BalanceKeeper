using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BalanceKeeper.Core
{
    public class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            ViewCommands = new List<ViewCommand>();
        }

        public List<ViewCommand> ViewCommands
        {
            get;
            set;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
