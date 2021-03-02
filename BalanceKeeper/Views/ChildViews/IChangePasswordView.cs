using BalanceKeeper.Viewmodels;

namespace BalanceKeeper.Views.ChildViews
{
    internal interface IChangePasswordView : IView
    {
         System.Windows.Controls.PasswordBox PassBoxReg1 { get; }
         System.Windows.Controls.PasswordBox PassBoxReg2 { get; }
    }
}