using System;
using System.Globalization;
using System.Windows.Data;
using Xfp.DataTypes.PanelData;

namespace Xfp.UI.ViewHelpers
{
    [ValueConversion(typeof(int?), typeof(string))]
    class BaseSounderGroupToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => !GroupConfigData.IsValidGroup((int?)value) ? "" : (int?)value == 0 ? Cultures.Resources.None : GroupConfigData.GetGroupName((int?)value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
