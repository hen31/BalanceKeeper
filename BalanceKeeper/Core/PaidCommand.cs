using BalanceKeeper.Repositories;
using BalanceKeeper.Views.ChildViews;
using MahApps.Metro.SimpleChildWindow;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalanceKeeper.Core
{
    public class PaidCommand : RelayCommand
    {
        public PaidCommand(Action<object> execute) : base(execute)
        {
        }

        public PaidCommand(Action<object> execute, Predicate<object> canExecute) : base(execute, canExecute)
        {
        }

        public override void Execute(object parameter)
        {

            base.Execute(parameter);

        }

    }
}
