using CTecControls.Config;
using CTecDevices.DeviceTypes;
using CTecDevices.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFP.Config
{
    internal class DeviceSelectorSettings
    {
        internal static DeviceSelectorConfig.DeviceSelectorMenu Menu
        {
            get => DeviceTypes.CurrentProtocolType switch
             {
                 CTecDevices.ObjectTypes.XfpApollo => ApolloDeviceSelector.Menu,
                 CTecDevices.ObjectTypes.XfpCast   => CastDeviceSelector.Menu,
                 //ProtocolTypes.Hochiki => HochikiDeviceSelector.Menu,
                 _ => null
             };
        }
    }
}
