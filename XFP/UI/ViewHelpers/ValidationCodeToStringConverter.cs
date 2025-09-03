using System;
using System.Globalization;
using System.Windows.Data;
using Xfp.DataTypes;

namespace Xfp.UI.ViewHelpers
{
    [ValueConversion(typeof(string), typeof(ErrorLevels))]
    public sealed class ValidationCodeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Enums.ValidationCodesToString((ValidationCodes)value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
