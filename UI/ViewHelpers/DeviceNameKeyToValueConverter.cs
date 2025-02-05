using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Xfp.DataTypes.PanelData;

namespace Xfp.UI.ViewHelpers
{
    [ValueConversion(typeof(int?), typeof(string))]
    class DeviceNameKeyToValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is not null && value is int ? GetDeviceName?.Invoke((int)value)??"" : "";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;

        internal static Xfp.ViewModels.PanelTools.DeviceItemViewModel.DeviceNameGetter GetDeviceName;
    }
}