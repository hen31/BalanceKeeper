using BalanceKeeper.Data.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BalanceKeeper.Converters
{
    class CategorieLinksToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableCollection<CategoryRelationLink> CategoryLinks)
            {
                string categoriesToText = string.Empty;
                foreach (var link in CategoryLinks)
                {
                    if (link.CategoryID != null && link.Category != null)
                    {
                        if (!string.IsNullOrWhiteSpace(categoriesToText))
                        {
                            categoriesToText += ", ";
                        }
                        categoriesToText += link.Category.Name + " " + link.Percentage + "%";
                    }
                }
                return categoriesToText;
            }
            else if (value is ObservableCollection<CategoryTransactionLink> transActionLinks)
            {
                string categoriesToText = string.Empty;
                foreach (var link in transActionLinks)
                {
                    if (link.CategoryID != null && link.Category != null)
                    {
                        if (!string.IsNullOrWhiteSpace(categoriesToText))
                        {
                            categoriesToText += ", ";
                        }
                        categoriesToText += link.Category.Name + " " + link.Percentage + "%";
                    }
                }
                return categoriesToText;
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
