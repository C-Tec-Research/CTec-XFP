using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Xfp.UI.ViewHelpers
{
    /// <summary>
    /// Visible if non-null, else Collapsed
    /// </summary>
    [ValueConversion(typeof(string), typeof(string))]
    public sealed class BlankFactoryDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => string.IsNullOrWhiteSpace((string)value) ? "---------------" : value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
