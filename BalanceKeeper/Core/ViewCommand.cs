using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Core
{
    public class ViewCommand : RelayCommand
    {
       public ViewCommand(string title, Action<object> execute) : base(execute, null)
        {

        }

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
    }
}
