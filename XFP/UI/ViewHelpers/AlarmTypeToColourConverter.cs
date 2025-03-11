using CTecControls.Cultures;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Xfp.DataTypes;

namespace Xfp.UI.ViewHelpers
{
    [ValueConversion(typeof(AlarmTypes), typeof(Brushes))]
    public sealed class AlarmTypeToColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (FindResource is not null)
                {
                    return (AlarmTypes)value switch
                    {
                        AlarmTypes.Alert    => (Brush)FindResource.Invoke("AlarmAlertBrush"),
                        AlarmTypes.Evacuate => (Brush)FindResource.Invoke("AlarmEvacBrush"),
                        _                   => (Brush)FindResource.Invoke("AlarmOffBrush"),
                    };
                }
            }
            catch { }

            //...in case the resources were not found
            return (AlarmTypes)value switch
            {
                AlarmTypes.Alert    => Brushes.Orange,
                AlarmTypes.Evacuate => Brushes.Red,
                _                   => Brushes.Transparent,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;

        public delegate object ResourceFinder(object resourceKey);
        public static ResourceFinder FindResource;
    }
}
