using System;
using System.Globalization;
using System.Windows.Data;
using Xfp.DataTypes.PanelData;

namespace Xfp.UI.ViewHelpers
{
    [ValueConversion(typeof(string), typeof(int?))]
    public sealed class IndexToZoneNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ZoneConfigData.GetZoneName((int?)value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
