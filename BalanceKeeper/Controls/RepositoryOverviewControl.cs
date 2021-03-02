using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BalanceKeeper.Controls
{
    public class RepositoryOverviewControl : ContentControl
    {
        public RepositoryOverviewControl()
        {
            Columns = new ObservableCollection<System.Windows.Controls.DataGridColumn>();
        }
        static RepositoryOverviewControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RepositoryOverviewControl),
                new FrameworkPropertyMetadata(typeof(RepositoryOverviewControl)));
        }

        public static readonly DependencyProperty ColumnsProperty =
      DependencyProperty.Register("Columns",
          typeof(ObservableCollection<System.Windows.Controls.DataGridColumn>),
          typeof(RepositoryOverviewControl),
            new FrameworkPropertyMetadata(null,
            FrameworkPropertyMetadataOptions.None));

        public ObservableCollection<System.Windows.Controls.DataGridColumn> Columns
        {
            get { return (ObservableCollection<System.Windows.Controls.DataGridColumn>)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public static readonly DependencyProperty FilterRegionProperty =
DependencyProperty.Register("FilterRegion",
   typeof(object),
   typeof(RepositoryOverviewControl),
     new FrameworkPropertyMetadata(null,
     FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public object FilterRegion
        {
            get { return (object)GetValue(FilterRegionProperty); }
            set { SetValue(FilterRegionProperty, value); }
        }

        public static readonly DependencyProperty RowDetailsTemplateProperty =
DependencyProperty.Register("RowDetailsTemplate",
typeof(DataTemplate),
typeof(RepositoryOverviewControl),
 new FrameworkPropertyMetadata(null,
 FrameworkPropertyMetadataOptions.None));

        public DataTemplate RowDetailsTemplate
        {
            get { return (DataTemplate)GetValue(RowDetailsTemplateProperty); }
            set { SetValue(RowDetailsTemplateProperty, value); }
        }


        public static readonly DependencyProperty TopCommandsProperty =
DependencyProperty.Register("TopCommands",
typeof(object),
typeof(RepositoryOverviewControl),
 new FrameworkPropertyMetadata(null,
 FrameworkPropertyMetadataOptions.None));

        public object TopCommands
        {
            get { return (object)GetValue(TopCommandsProperty); }
            set { SetValue(TopCommandsProperty, value); }
        }

        
    }
}
