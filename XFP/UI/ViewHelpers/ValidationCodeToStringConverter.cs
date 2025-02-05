using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;

namespace Xfp.UI.ViewHelpers
{
    [ValueConversion(typeof(string), typeof(ErrorLevels))]
    public sealed class ValidationCodeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Enums.ValidationCodesToString((ValidationCodes)value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
