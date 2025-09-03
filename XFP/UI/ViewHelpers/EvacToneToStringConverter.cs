using System;
using System.Globalization;
using System.Windows.Data;
using CTecUtil.Utils;

namespace Xfp.UI.ViewHelpers
{
    [ValueConversion(typeof(string), typeof(int?))]
    public sealed class EvacToneToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)     => (value is int i) ? string.Format(Cultures.Resources.Tone_Message_Pair_x_Secondary, i + 1) : "";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string val)
                return TextUtil.ExtractIntFromFormattedText(val, Cultures.Resources.Tone_Message_Pair_x_Secondary);
            return null;
        }
    }
}