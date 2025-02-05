using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Xfp.UI.ViewHelpers
{
    [ValueConversion(typeof(string), typeof(int))]
    class ZoneToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is null  || (int)value < 0 ? Cultures.Resources.Unassigned : (int)value > 0 ? string.Format(Cultures.Resources.Zone_x, (int)value) : (int)value == -1 ? Cultures.Resources.Use_In_Special_C_And_E : Cultures.Resources.Unassigned;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string val ? val == Cultures.Resources.Unassigned || string.IsNullOrEmpty(val) ? -1 : (((string)value)[0] - '1') : -1;
        }
    }
}