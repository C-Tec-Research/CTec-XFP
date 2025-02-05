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
    [ValueConversion(typeof(SetTriggerTypes), typeof(Brushes))]
    public sealed class SetTriggerTypeToColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (FindResource is not null)
                {
                    return (SetTriggerTypes)value switch
                    {
                        SetTriggerTypes.Pulsed     => (Brush)FindResource.Invoke("TriggerPulsedBrush"),
                        SetTriggerTypes.Continuous => (Brush)FindResource.Invoke("TriggerContinuousBrush"),
                        SetTriggerTypes.Delayed    => (Brush)FindResource.Invoke("TriggerDelayedBrush"),
                        _                          => (Brush)FindResource.Invoke("TriggerNotTriggeredBrush"),
                    };
                }
            }
            catch { }

            return (SetTriggerTypes)value switch
            {
                SetTriggerTypes.Pulsed     => Brushes.Orange,
                SetTriggerTypes.Continuous => Brushes.Red,
                SetTriggerTypes.Delayed    => Brushes.Blue,
                _                          => Brushes.DimGray,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;

        public delegate object ResourceFinder(object resourceKey);
        public static ResourceFinder FindResource;
    }
}