using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Windows.Security.Authentication.OnlineId;

namespace Xfp.UI.ViewHelpers
{
    [ValueConversion(typeof(string), typeof(int?))]
    class GroupToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
            => value is null  || (int)value < 0 ? Cultures.Resources.Unassigned : (int)value > 0 ? string.Format(Cultures.Resources.Group_x, (int)value) : (int)value == -1 ? Cultures.Resources.Use_In_Special_C_And_E : Cultures.Resources.Unassigned;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string val)
            {
                if (string.IsNullOrEmpty(val) || val == Cultures.Resources.Unassigned)
                    return null;
                
                return CTecUtil.TextProcessing.ExtractIntFromFormattedText(val, Cultures.Resources.Use_In_Special_C_And_E);
            }
            return null;
        }
    }
}