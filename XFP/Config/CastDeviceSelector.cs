using CTecControls.Config;
using CTecDevices;
using CTecDevices.DeviceTypes;
using CTecDevices.Protocol;

namespace XFP.Config
{
    public class CastDeviceSelector
    {
        public static DeviceSelectorConfig.DeviceSelectorMenu Menu = new DeviceSelectorConfig.DeviceSelectorMenu()
        {
            MenuItems = new DeviceSelectorConfig.DeviceMenuItem[]
            {
                new(ObjectTypes.XfpCast)
                {
                    MenuHeader = XfpCast.Tags.Detector,
                    Submenu = new DeviceSelectorConfig.DeviceMenuItem[]
                    {
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastOpticalDetector },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastHeatDetector },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastMultiDetector },
                    }
                },
                new(ObjectTypes.XfpCast)
                {
                    MenuHeader = XfpCast.Tags.Sounder,
                    Submenu = new DeviceSelectorConfig.DeviceMenuItem[]
                    {
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.SounderVAD },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.Sounder },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.VoiceSounder },
                    }
                },
                new(ObjectTypes.XfpCast)
                {
                    MenuHeader = XfpCast.Tags.Callpoint,
                    Submenu = new DeviceSelectorConfig.DeviceMenuItem[]
                    {
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.MCP },
                    }
                },
                new(ObjectTypes.XfpCast)
                {
                    MenuHeader = XfpCast.Tags.Interface,
                    Submenu = new DeviceSelectorConfig.DeviceMenuItem[]
                    {
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.IOUnit },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.MainsIOUnit },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.ZMU },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.NonLatchingIOUnit },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.HS2 },
                    }
                },
                new(ObjectTypes.XfpCast)
                {
                    MenuHeader = XfpCast.Tags.CastProOpticalDetector,
                    Icon = DeviceTypes.DeviceIcon((int?)XfpCastDeviceTypeIds.CastProOpticalDetector, ObjectTypes.XfpCast),
                    Submenu = new DeviceSelectorConfig.DeviceMenuItem[]
                    {
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastProOpticalDetector },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastProOpticalDetectorTone },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastProOpticalDetectorVoice },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastProOpticalDetectorToneVAD },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastProOpticalDetectorVoiceVAD },
                    }
                },
                new(ObjectTypes.XfpCast)
                {
                    MenuHeader = XfpCast.Tags.CastProHeatDetector,
                    Icon = DeviceTypes.DeviceIcon((int?)XfpCastDeviceTypeIds.CastProHeatDetector, ObjectTypes.XfpCast),
                    Submenu = new DeviceSelectorConfig.DeviceMenuItem[]
                    {
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastProHeatDetector },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastProHeatDetectorTone },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastProHeatDetectorVoice },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastProHeatDetectorToneVAD  },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastProHeatDetectorVoiceVAD },
                    }
                },
                new(ObjectTypes.XfpCast)
                {
                    MenuHeader = XfpCast.Tags.CastProOptHeatDetector,
                    Icon = DeviceTypes.DeviceIcon((int?)XfpCastDeviceTypeIds.CastProOpticalHeatDetector, ObjectTypes.XfpCast),
                    Submenu = new DeviceSelectorConfig.DeviceMenuItem[]
                    {
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastProOpticalHeatDetector },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastProOpticalHeatDetectorTone },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastProOpticalHeatDetectorToneVAD },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastProOpticalHeatDetectorVoice },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastProOpticalHeatDetectorVoiceVAD },
                    }
                },
                new(ObjectTypes.XfpCast)
                {
                    MenuHeader = XfpCast.Tags.CastProOptHeatCODetector,
                    Icon = DeviceTypes.DeviceIcon((int?)XfpCastDeviceTypeIds.CastProOpticalHeatCODetector, ObjectTypes.XfpCast),
                    Submenu = new DeviceSelectorConfig.DeviceMenuItem[]
                    {
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastProOpticalHeatCODetector },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastProOpticalHeatCODetectorTone },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastProOpticalHeatCODetectorToneVAD },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastProOpticalHeatCODetectorVoice },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastProOpticalHeatCODetectorVoiceVAD },
                    }
                },
                new(ObjectTypes.XfpCast)
                {
                    MenuHeader = XfpCast.Tags.CastProSounder,
                    Icon = DeviceTypes.DeviceIcon((int?)XfpCastDeviceTypeIds.CastProVAD, ObjectTypes.XfpCast),
                    Submenu = new DeviceSelectorConfig.DeviceMenuItem[]
                    {
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastProVAD },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastProSounderTone },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastProSounderToneVAD },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastProSounderVoice },
                        new(ObjectTypes.XfpCast) { DeviceType = (int)XfpCastDeviceTypeIds.CastProSounderVoiceVAD },
                    }
                },
            },
        };
    }
}
