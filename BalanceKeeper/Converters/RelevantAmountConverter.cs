using BalanceKeeper.Data.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace BalanceKeeper.Converters
{
    class RelevantAmountConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Transaction transAction = values[0] as Transaction;
            Category category = values[1] as Category;
            double relevantAmount = 0.0;
            if (category == null || category.ID==0)
            {
                relevantAmount = transAction.Amount * ((100 -transAction.CategoryLinks.Where(b => b.CategoryID != null).Sum(b => b.Percentage)) / 100);
            }
            else
            {
                relevantAmount = transAction.Amount * (transAction.CategoryLinks.Where(b => b.CategoryID == category.ID).Sum(b => b.Percentage) / 100);
            }
                if (targetType == typeof(Brush))
            {
                return new AmountToColorConverter().Convert(relevantAmount, null, null, null);
            }
            else
            {
                return relevantAmount;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
