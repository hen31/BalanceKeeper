using BalanceKeeper.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BalanceKeeper.Controls
{
    public class AsyncTaskProgressControl : ContentControl
    {
        static AsyncTaskProgressControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AsyncTaskProgressControl),
                new FrameworkPropertyMetadata(typeof(AsyncTaskProgressControl)));
        }

        public static readonly DependencyProperty TaskNotifierProperty =
    DependencyProperty.Register("TaskNotifier", typeof(NotifyTaskCompletion), typeof(AsyncTaskProgressControl),
    new FrameworkPropertyMetadata(null,
          FrameworkPropertyMetadataOptions.AffectsRender));

        public NotifyTaskCompletion TaskNotifier
        {
            get { return (NotifyTaskCompletion)GetValue(TaskNotifierProperty); }
            set { SetValue(TaskNotifierProperty, value); }
        }


        public static readonly DependencyProperty BusyTextProperty =
DependencyProperty.Register("BusyText", typeof(string), typeof(AsyncTaskProgressControl),
new FrameworkPropertyMetadata("Bezig met laden...",
      FrameworkPropertyMetadataOptions.None));

        public string BusyText
        {
            get { return (string)GetValue(BusyTextProperty); }
            set { SetValue(BusyTextProperty, value); }
        }


        public static readonly DependencyProperty HideContentWhileExecutingProperty =
DependencyProperty.Register("HideContentWhileExecuting", typeof(bool), typeof(AsyncTaskProgressControl),
new FrameworkPropertyMetadata(false,
   FrameworkPropertyMetadataOptions.AffectsRender));

        public bool HideContentWhileExecuting
        {
            get { return (bool)GetValue(HideContentWhileExecutingProperty); }
            set { SetValue(HideContentWhileExecutingProperty, value); }
        }

        
    }
}
