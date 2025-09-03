using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Xfp.DataTypes;

namespace Xfp.UI.ViewHelpers
{
    [ValueConversion(typeof(AlarmTypes), typeof(Brushes))]
    public sealed class AlarmTypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (AlarmTypes)value switch
            {
                AlarmTypes.Alert    => Styles.AlarmAlertBrush,
                AlarmTypes.Evacuate => Styles.AlarmEvacBrush,
                _                   => Styles.AlarmOffBrush,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
