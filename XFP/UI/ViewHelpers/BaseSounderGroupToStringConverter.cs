using System;
using System.Globalization;
using System.Windows.Data;

namespace Xfp.UI.ViewHelpers
{
    [ValueConversion(typeof(int?), typeof(string))]
    class BaseSounderGroupToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is null || value is int && (int)value < 1 ? CTecControls.Cultures.Resources.None : string.Format(Cultures.Resources.Group_x, (int)value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
