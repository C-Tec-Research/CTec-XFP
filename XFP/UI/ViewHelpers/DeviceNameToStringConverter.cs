using System;
using System.Globalization;
using System.Windows.Data;

namespace Xfp.UI.ViewHelpers
{
    /// <summary>
    /// Visible if non-null, else Collapsed
    /// </summary>
    [ValueConversion(typeof(string), typeof(string))]
    public sealed class DeviceNameToStringConverter : IValueConverter
    {
        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => string.IsNullOrEmpty((string)value) ? "" : (string)value;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => string.IsNullOrEmpty((string)value) || (string)value == Cultures.Resources.No_Name_Allocated ? "" : (string)value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
