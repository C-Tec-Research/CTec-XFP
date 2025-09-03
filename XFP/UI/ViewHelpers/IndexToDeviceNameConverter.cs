using System;
using System.Globalization;
using System.Windows.Data;

namespace Xfp.UI.ViewHelpers
{
    [ValueConversion(typeof(string), typeof(int?))]
    public sealed class IndexToDeviceNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => string.Format(Cultures.Resources.Device_x, value is null ? " ..." : (int?)value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
