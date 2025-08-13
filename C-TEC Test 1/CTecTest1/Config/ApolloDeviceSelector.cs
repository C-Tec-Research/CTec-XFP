using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CTecControls.Config;
using CTecDevices;
using CTecDevices.DeviceTypes;
using CTecDevices.Protocol;

namespace XFP.Config
{
    public class ApolloDeviceSelector
    {
        public static DeviceSelectorConfig.DeviceSelectorMenu Menu = new DeviceSelectorConfig.DeviceSelectorMenu()
        {
            MenuItems = new DeviceSelectorConfig.DeviceMenuItem[]
            {
                new(ObjectTypes.XfpApollo)
                {
                    MenuHeader = XfpApollo.Tags.OpticalDetector,
                    Submenu = new DeviceSelectorConfig.DeviceMenuItem[]
                    {
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.Discovery_OpticalDetector },
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.XP95_S90_OpticalDetector },
                    }
                },
                new(ObjectTypes.XfpApollo)
                {
                    MenuHeader = XfpApollo.Tags.IonisationDetector,
                    Submenu = new DeviceSelectorConfig.DeviceMenuItem[]
                    {
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.Discovery_IonisationDetector },
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.XP95_S90_IonisationDetector },
                    }
                },
                new(ObjectTypes.XfpApollo)
                {
                    MenuHeader = XfpApollo.Tags.HeatDetector,
                    Submenu = new DeviceSelectorConfig.DeviceMenuItem[]
                    {
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.Discovery_HeatDetector },
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.XP95_S90_HeatDetector },
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.XP95_S90_HighTempDetector },
                    }
                },
                new(ObjectTypes.XfpApollo)
                {
                    MenuHeader = XfpApollo.Tags.CODetector,
                    Submenu = new DeviceSelectorConfig.DeviceMenuItem[]
                    {
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.Discovery_CODetector },
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.Discovery_COHeatDetector },
                    }
                },
                new(ObjectTypes.XfpApollo)
                {
                    MenuHeader = XfpApollo.Tags.MultiDetector,
                    Submenu = new DeviceSelectorConfig.DeviceMenuItem[]
                    {
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.Discovery_MultiDetector },
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.XP95_MultiDetector },
                    }
                },
                new(ObjectTypes.XfpApollo)
                {
                    MenuHeader = XfpApollo.Tags.Callpoint,
                    Submenu = new DeviceSelectorConfig.DeviceMenuItem[]
                    {
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.Discovery_MCP },
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.XP95_MCP },
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.S90_MCP },
                    }
                },
                new(ObjectTypes.XfpApollo)
                {
                    MenuHeader = XfpApollo.Tags.Sounder,
                    Submenu = new DeviceSelectorConfig.DeviceMenuItem[]
                    {
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.SounderController },
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.SounderBeaconBase },
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.SounderBase },
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.VoiceSounderBeacon },
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.VoiceSounder },
                    }
                },
                new(ObjectTypes.XfpApollo)
                {
                    MenuHeader = XfpApollo.Tags.Interface,
                    Submenu = new DeviceSelectorConfig.DeviceMenuItem[]
                    {
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.XP95_S90_IOUnit },
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.XP95_SwitchMonitor },
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.IntelligentBeamDetector },
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.XP95_FlameDetector },
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.XP95_S90_ZoneMonitorUnit },
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.SounderIOUnit },
                        new(ObjectTypes.XfpApollo) { DeviceType = (int)XfpApolloDeviceTypeIds.NonLatchingIOUnit },
                    }
                },
            },
        };
    }
}
