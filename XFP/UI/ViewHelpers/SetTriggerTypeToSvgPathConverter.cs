using System;
using System.Globalization;
using System.Windows.Data;
using CTecControls;
using CTecControls.Util;
using Xfp.DataTypes;

namespace Xfp.UI.ViewHelpers
{
    [ValueConversion(typeof(SetTriggerTypes), typeof(string))]
    public sealed class SetTriggerTypeToSvgPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (SetTriggerTypes)value switch
            {
                SetTriggerTypes.Pulsed     => IconUtilities.IconSvgPathData(IconTypes.SounderPulsed),
                SetTriggerTypes.Continuous => IconUtilities.IconSvgPathData(IconTypes.SounderContinuous),
                SetTriggerTypes.Delayed    => IconUtilities.IconSvgPathData(IconTypes.SounderDelayed),
                _                          => IconUtilities.IconSvgPathData(IconTypes.SounderNone),
            };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}