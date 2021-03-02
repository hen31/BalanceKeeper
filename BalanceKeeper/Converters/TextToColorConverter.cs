using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace BalanceKeeper.Converters
{
    public class TextToColorConverter : MarkupExtension, IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color c;
            if (string.IsNullOrWhiteSpace(value as string))
            {
                c = Colors.Transparent;
            }
            else
            {
                c = (Color)ColorConverter.ConvertFromString(value as string);
            }
            if(targetType == typeof(Brush))
            {
                return new SolidColorBrush(c);
            }
            else
            {
                return c;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
            {
                return "";
            }
            Color c  = (Color)value;
            return HexConverter(c);
        }

        private static string HexConverter(Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        public static TextToColorConverter Instance { get; set; }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Instance == null)
            {
                Instance = new TextToColorConverter();
            }
            return Instance;
        }
    }
}
