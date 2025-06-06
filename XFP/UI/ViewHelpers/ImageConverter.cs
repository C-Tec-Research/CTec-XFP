﻿using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using CTecControls;
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
