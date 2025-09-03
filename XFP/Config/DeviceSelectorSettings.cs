using CTecControls.Config;
using CTecDevices.Protocol;

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
