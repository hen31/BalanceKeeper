using BalanceKeeper.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Classes
{
    public class TaskExecutorWrapper : ViewModel
    {
        public string BusyText { get; set; }
        NotifyTaskCompletion _taskNotifier;
        public NotifyTaskCompletion TaskNotifier
        {
            get
            {
                return _taskNotifier;
            }
            set
            {
                _taskNotifier = value;
                OnPropertyChanged("TaskNotifier");
            }
        }

    }
}
