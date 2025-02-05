//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using CTecControls.Config;
//using CTecDevices.DeviceTypes;
//using CTecDevices.Protocol;
//using CTecDevices.Protocol.Protocols;

//namespace XFP.Config
//{
//    public class HochikiDeviceSelector
//    {
//        public static DeviceSelectorConfig.DeviceSelectorMenu Menu = new DeviceSelectorConfig.DeviceSelectorMenu(ProtocolTypes.Hochiki)
//        {
//            MenuItems = new DeviceSelectorConfig.DeviceMenuItem[]
//            {
//                new()
//                {
//                    MenuHeader = Hochiki.Tags.OpticalDetector,
//                    DeviceType = (int)HochikiDeviceTypeIds.OpticalDetector,
//                },
//                new()
//                {
//                    MenuHeader = Hochiki.Tags.IonisationDetector,
//                    DeviceType = (int)HochikiDeviceTypeIds.IonisationDetector,
//                },
//                new()
//                {
//                    MenuHeader = Hochiki.Tags.HeatDetector,
//                    DeviceType = (int)HochikiDeviceTypeIds.HeatDetector,
//                },
//                new()
//                {
//                    MenuHeader = Hochiki.Tags.MultiDetector,
//                    DeviceType = (int)HochikiDeviceTypeIds.MultiDetector,
//                },
//                new()
//                {
//                    MenuHeader = Hochiki.Tags.Callpoint,
//                    DeviceType = (int)HochikiDeviceTypeIds.Callpoint,
//                },
//                new()
//                {
//                    MenuHeader = Hochiki.Tags.Module,
//                    Submenu = new DeviceSelectorConfig.DeviceMenuItem[]
//                    {
//                        new() { DeviceType = (int)HochikiDeviceTypeIds.AddressableBeacon },
//                        new() { DeviceType = (int)HochikiDeviceTypeIds.MiniZoneMonitor },
//                        new() { DeviceType = (int)HochikiDeviceTypeIds.RelayController },
//                        new() { DeviceType = (int)HochikiDeviceTypeIds.DualSwitchController },
//                        new() { DeviceType = (int)HochikiDeviceTypeIds.SingleIOModule },
//                        new() { DeviceType = (int)HochikiDeviceTypeIds.DualZoneMonitor },
//                        new() { DeviceType = (int)HochikiDeviceTypeIds.NonLatchingIOUnit },
//                        new() { DeviceType = (int)HochikiDeviceTypeIds.SounderController_Sets },
//                        new() { DeviceType = (int)HochikiDeviceTypeIds.SounderController_Groups },
//                        new() { DeviceType = (int)HochikiDeviceTypeIds.RemoteIndicator },
//                    }
//                },
//                new()
//                {
//                    MenuHeader = Hochiki.Tags.Base,
//                    Submenu = new DeviceSelectorConfig.DeviceMenuItem[]
//                    {
//                        new() { DeviceType = (int)HochikiDeviceTypeIds.BaseWallSounder },
//                        new() { DeviceType = (int)HochikiDeviceTypeIds.AddressableBase },
//                        new() { DeviceType = (int)HochikiDeviceTypeIds.MasterAddressableBase },
//                    }
//                },
//            },
//        };
//    }
//}
