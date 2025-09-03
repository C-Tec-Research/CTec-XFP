using System;
using System.Globalization;
using System.Windows.Data;

namespace Xfp.UI.ViewHelpers
{
    [ValueConversion(typeof(string), typeof(int))]
    public sealed class IndexToGroupNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => string.Format(Cultures.Resources.Group_x, (int)value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
