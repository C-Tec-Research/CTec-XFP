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
    public sealed class SetTriggerTypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (SetTriggerTypes)value switch
            {
                SetTriggerTypes.Pulsed     => Styles.TriggerPulsedBrush,
                SetTriggerTypes.Continuous => Styles.TriggerContinuousBrush,
                SetTriggerTypes.Delayed    => Styles.TriggerDelayedBrush,
                _                          => Styles.TriggerNotTriggeredBrush,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}