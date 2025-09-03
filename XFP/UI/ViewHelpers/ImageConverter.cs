using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using CTecDevices.Protocol;

namespace Xfp.UI.ViewHelpers
{
    [ValueConversion(typeof(BitmapImage), typeof(int?))]
    public sealed class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? null : value ?? DeviceTypes.DeviceIcon((int?)value, DeviceTypes.CurrentProtocolType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
