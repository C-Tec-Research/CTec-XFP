using System;
using System.Collections.Generic;
using System.Text;
using CTecDevices;
using CTecDevices.Protocol;
using CTecUtil.StandardPanelDataTypes;
using CTecUtil.Utils;
using Newtonsoft.Json;
using Xfp.IO;

namespace Xfp.DataTypes.PanelData
{
    public partial class DeviceData : ConfigData
    {
        public DeviceData(int loopNum)
        {
            LoopNum = loopNum;
            DeviceType = null;
        }

        internal DeviceData(DeviceData original) : this(original.LoopNum)
        {
            Index = original.Index;
            DeviceType = original.DeviceType;
            Zone = original.Zone;
            Group = original.Group;
            NameIndex = original.NameIndex;
            DaySensitivity = original.DaySensitivity;
            NightSensitivity = original.NightSensitivity;
            DayVolume = original.DayVolume;
            NightVolume = original.NightVolume;
            DayMode = original.DayMode;
            NightMode = original.NightMode;
            IOConfig = new(original.IOConfig);
            AncillaryBaseSounderGroup = original.AncillaryBaseSounderGroup;
            IsRealDevice = original.IsRealDevice;
            RemoteLEDEnabled = original.RemoteLEDEnabled;
            //TypeChanged = original.TypeChanged;
        }


        internal const int NumIOSettings = 4;
        internal const int NumSubNames = 3;
        internal const int NumIOSubaddresses = 4;
        internal const int NumIOChannels = 3;
        internal const int NumIOSets = 16;
        internal const int NumSharedData = 8;

        private int? _deviceType = null;


        public int? DeviceType
        {
            get => _deviceType;
            set
            {
                if (_deviceType != value)
                {
                    _deviceType = value;
                    IOConfig = new();
                    for (int i = 0; i < NumIOSettings; i++)
                        IOConfig.Add(new(i, value));
                }
            }
        }

        public int LoopNum { get; set; }
        public int Index { get; set; }
        public int Zone { get; set; } = 0;
        public int Group { get; set; } = 0;
        public int? DaySensitivity { get; set; }
        public int? NightSensitivity { get; set; }
        [JsonIgnore]
        public int DefaultSensitivity => IsSensitivityHighDevice ? DeviceConfigData.DefaultSensitivityHigh : DeviceConfigData.DefaultSensitivity;
        
        public int? DayVolume { get; set; }
        public int? NightVolume { get; set; }
        public int? DayMode { get; set; } = 1;
        public int? NightMode { get; set; } = 1;
        public List<IOSettingData> IOConfig { get; set; } = new();
        public int NameIndex { get; set; } = 0;
        internal bool HasAncillaryBaseSounder => (AncillaryBaseSounderGroup??0) > 0;
        public int? AncillaryBaseSounderGroup { get; set; } = null;
        public bool IsRealDevice { get; set; } = true;
        public bool? RemoteLEDEnabled { get; set; } = false;
        //public bool TypeChanged { get; set; }     //not used?

        internal bool IsGroupedDevice => DeviceTypes.IsGroupedDevice(DeviceType, DeviceTypes.CurrentProtocolType);
        internal bool IsZonalDevice => DeviceTypes.IsZonalDevice(DeviceType, DeviceTypes.CurrentProtocolType);
        internal bool IsIODevice => DeviceTypes.IsIODevice(DeviceType, DeviceTypes.CurrentProtocolType) && IsRealDevice;
        internal bool IOOutputIsGrouped => DeviceTypes.IOOutputIsGrouped(DeviceType, DeviceTypes.CurrentProtocolType); 
        internal bool IsSensitivityDevice => DeviceTypes.IsSensitivityDevice(DeviceType, DeviceTypes.CurrentProtocolType);
        internal bool IsSensitivityHighDevice => DeviceTypes.IsSensitivityHighDevice(DeviceType, DeviceTypes.CurrentProtocolType);
        internal bool IsVolumeDevice => DeviceTypes.IsVolumeDevice(DeviceType, DeviceTypes.CurrentProtocolType);
        internal bool IsModeDevice => DeviceTypes.IsModeDevice(DeviceType, DeviceTypes.CurrentProtocolType);


        /// <summary>
        /// Returns an initialised DeviceData object.
        /// </summary>
        internal static DeviceData InitialisedNew(int loopNum)
        {
            var result = new DeviceData(loopNum);
            for (int i = 0; i < NumIOSubaddresses; i++)
                result.IOConfig.Add(new(i, result.DeviceType));
            return result;
        }


        /// <summary>
        /// Returns an initialised DeviceData object.
        /// </summary>
        internal static DeviceData InitialisedNew(int? deviceType, int loopNum, int index)
        {
            var result = InitialisedNew(loopNum);
            result.Index = index;
            result.DeviceType = deviceType;
            if (!DeviceTypes.ModeIsValid(result.DeviceType, result.DayMode))
                result.DayMode = DeviceTypes.DefaultDayMode(deviceType);
            if (!DeviceTypes.ModeIsValid(result.DeviceType, result.NightMode))
                result.NightMode = DeviceTypes.DefaultDayMode(deviceType);
            return result;
        }


        public override bool Equals(ConfigData otherData)
        {
            if (otherData is not DeviceData od)
                return false;

            if (od.LoopNum != LoopNum
             || od.Index != Index
             || od.DeviceType != DeviceType
             || od.Zone != Zone
             || od.Group != Group
             || od.NameIndex != NameIndex
             || od.DaySensitivity != DaySensitivity
             || od.DayVolume != DayVolume
             || od.NightSensitivity != NightSensitivity
             || od.NightVolume != NightVolume
             || od.DayMode != DayMode
             || od.NightMode != NightMode
             || od.AncillaryBaseSounderGroup != AncillaryBaseSounderGroup
             || od.IsRealDevice != IsRealDevice
             || od.RemoteLEDEnabled != RemoteLEDEnabled)
                return false;

            if (od.IOConfig.Count != IOConfig.Count)
                return false;
            for (int i = 0; i < od.IOConfig.Count; i++)
                if (!od.IOConfig[i].Equals(IOConfig[i]))
                    return false;

            return true;
        }


        public override bool Validate()
        {
            _errorItems = new(Index, string.Format(Cultures.Resources.Device_x, Index + 1));

            if (DeviceType is not null)
            {
                if (!DeviceTypes.IsValidDeviceType(DeviceType, DeviceTypes.CurrentProtocolType))
                {
                    _errorItems.ValidationCodes.Add(ValidationCodes.DeviceConfigDataInvalidDeviceType);
                }
                else
                {
                    if (IsZonalDevice)
                        if (Zone < -1 || Zone > ZoneConfigData.NumZones)
                            _errorItems.ValidationCodes.Add(ValidationCodes.DeviceConfigDataInvalidZone);

                    if (IsGroupedDevice)
                        if (Group < -1 || Group > GroupConfigData.NumSounderGroups)
                            _errorItems.ValidationCodes.Add(ValidationCodes.DeviceConfigDataInvalidGroup);

                    if (IsSensitivityHighDevice)
                    {
                        if (DaySensitivity == -1 || DaySensitivity > DeviceConfigData.MaxSensitivityHigh)
                            _errorItems.ValidationCodes.Add(ValidationCodes.DeviceConfigDataInvalidDaySensitivity);
                        if (NightSensitivity == -1 || NightSensitivity > DeviceConfigData.MaxSensitivityHigh)
                            _errorItems.ValidationCodes.Add(ValidationCodes.DeviceConfigDataInvalidNightSensitivity);
                    }
                    else if (IsSensitivityDevice)
                    {
                        if (DaySensitivity < -1 || DaySensitivity > DeviceConfigData.MaxSensitivity)
                            _errorItems.ValidationCodes.Add(ValidationCodes.DeviceConfigDataInvalidDaySensitivity);
                        if (NightSensitivity < -1 || NightSensitivity > DeviceConfigData.MaxSensitivity)
                            _errorItems.ValidationCodes.Add(ValidationCodes.DeviceConfigDataInvalidNightSensitivity);
                    }

                    if (IsVolumeDevice)
                    {
                        if (DayVolume < DeviceConfigData.MinVolume -1 || DayVolume > DeviceConfigData.MaxVolume - 1)
                            _errorItems.ValidationCodes.Add(ValidationCodes.DeviceConfigDataInvalidDayVolume);
                        if (NightVolume < DeviceConfigData.MinVolume - 1 || NightVolume > DeviceConfigData.MaxVolume - 1)
                            _errorItems.ValidationCodes.Add(ValidationCodes.DeviceConfigDataInvalidNightVolume);
                    }

                    if (IsModeDevice)
                    {
                        var dayModeIsValid = false;
                        var nightModeIsValid = false;
                        foreach (var m in DeviceTypes.ModeSettings(DeviceType))
                        {
                            if (m.Number == DayMode)
                                dayModeIsValid = true;
                            if (m.Number == NightMode)
                                nightModeIsValid = true;
                        }

                        if (!dayModeIsValid)
                            _errorItems.ValidationCodes.Add(ValidationCodes.DeviceConfigDataInvalidDayMode);
                        if (!nightModeIsValid)
                            _errorItems.ValidationCodes.Add(ValidationCodes.DeviceConfigDataInvalidNightMode);
                    }

                    if (IsIODevice)
                    {
                        foreach (var c in IOConfig)
                        {
                            if (!c.Validate())
                            {
                                var io = c.GetErrorItems();
                                var p = new ConfigErrorPageItems(Index, string.Format(Cultures.Resources.Subaddress_x, c.Index));
                                foreach (var e in io.ValidationCodes)
                                    p.ValidationCodes.Add(e);
                                _errorItems.ValidationCodes.AddRange(p.ValidationCodes);
                            }
                        }
                    }

                    if (DeviceTypes.CanHaveAncillaryBaseSounder(DeviceType, DeviceTypes.CurrentProtocolType))
                        if (AncillaryBaseSounderGroup < 0 || AncillaryBaseSounderGroup > GroupConfigData.NumSounderGroups)
                            _errorItems.ValidationCodes.Add(ValidationCodes.DeviceConfigDataInvalidSounderGroup);
                }
            }

            return _errorItems.ValidationCodes.Count == 0;
        }


        internal byte[] ToByteArray()
        {
            //ancillary base sounders for Apollo devices are sent in 'dummy' device records, with device index += NumDevices
            //e.g. the record for device 11 has index 137; the base sounder group is in the zone/group byte.
            
            //NB: top bit is set on zone/group if is an output device - this applies to byte [3], plus bytes [5], [7] & [9] for I/O devices 

            var isBaseSounderRecord = DeviceTypes.CurrentProtocolIsXfpApollo && Index >= DeviceConfigData.NumDevices;

            byte[] result = new byte[11];

            result[0] = (byte)(Index + 1);
            result[1] = (byte)(LoopNum + 1);
            result[2] = (byte)(DeviceType ?? 0xfe);

            if (DeviceType is not null)
            {
                if (isBaseSounderRecord)
                {
                    //set top bit as base sounder is an output device
                    result[3] = (byte)(AncillaryBaseSounderGroup > 0 ? (byte)((AncillaryBaseSounderGroup ?? 0) | 0x80) : 0);
                }
                else if (IsIODevice)
                {
                    if (IOConfig[0].InputOutput == IOTypes.NotUsed)
                    {
                        result[3] = 0xff;
                    }
                    else
                    {
                        result[3] = channelAndZGSByte(IOConfig[0]);
                        result[4] = (byte)IOConfig[0].NameIndex;
                    }

                    if (IOConfig[1].InputOutput == IOTypes.NotUsed)
                    {
                        result[5] = 0xff;
                    }
                    else
                    {
                        result[5] = channelAndZGSByte(IOConfig[1]);
                        result[6] = (byte)IOConfig[1].NameIndex;
                    }

                    if (IOConfig[2].InputOutput == IOTypes.NotUsed)
                    {
                        result[7] = 0xff;
                    }
                    else
                    {
                        result[7] = channelAndZGSByte(IOConfig[2]);
                        result[8] = (byte)IOConfig[2].NameIndex;
                    }

                    if (IOConfig[3].InputOutput == IOTypes.NotUsed)
                    {
                        result[9] = 0xff;
                    }
                    else
                    {
                        result[9] = channelAndZGSByte(IOConfig[3]);
                        result[10] = (byte)IOConfig[3].NameIndex;
                    }
                }
                else
                {
                    result[3] = (byte)(DeviceType is not null ? IsZonalDevice ? Zone : (Group | 0x80) : 0);
                    result[4] = (byte)NameIndex;

                    if (IsModeDevice)
                    {
                        result[5] = (byte)(DayMode ?? 1);
                        result[6] = (byte)(NightMode ?? 1);
                    }
                    else if (IsSensitivityDevice)
                    {
                        result[5] = (byte)(DaySensitivity ?? 0);
                        result[6] = (byte)(NightSensitivity ?? 0);
                    }

                    if (IsVolumeDevice)
                    {
                        //CAST PRO has both mode and volume, so volume is offset from the usual bytes
                        if (IsModeDevice)
                        {
                            result[8] = (byte)((DayVolume ?? 0) + 1);
                            result[9] = (byte)((NightVolume ?? 0) + 1);
                        }
                        else
                        {
                            result[5] = (byte)((DayVolume ?? 1) + 1);
                            result[6] = (byte)((NightVolume ?? 1) + 1);
                        }
                    }

                    if (IsZonalDevice && IsGroupedDevice)
                        result[10] = (byte)(Group | 0x80);



                    //byte-10 can be remote LED for Apollo or group for CAST Pro
                    if (DeviceTypes.CurrentProtocolIsXfpApollo)
                    {
                        result[10] = (byte)(RemoteLEDEnabled ?? false ? 1 : 0);
                    }
                    else if (IsZonalDevice && IsGroupedDevice)
                    {
                        //CAST PRO can have group as well as zone;
                        //set top bit to denote output device (=grouped)
                        result[10] = (byte)(Group | 0x80);
                    }
                }
            }
            //CTecUtil.Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> device=" + (Index + 1) + "  data=[" + ByteArrayProcessing.ByteArrayToHexString(result) + "]");
            return result;
        }

        private byte channelAndZGSByte(IOSettingData io)
        {
            var result = (byte)(io.ZoneGroupSet ?? 0);
            if (io.InputOutput == IOTypes.Output) result |= 0x80;
            if (io.Channel > 0) result |= 0x40;
            if (io.Channel > 1) result |= 0x20;
            return result;
        }


        internal static byte[] DeviceNameToByteArray(int key, string name)
        {
            var result = ByteArrayUtil.CombineByteArrays(ByteArrayUtil.IntToByteArray(key, 2), ByteArrayUtil.StringToByteArray(name), [0x00]);
            //CTecUtil.Debug.WriteLine("DeviceNameToByteArray: #" + key + ", " + name + " --> " + ByteArrayProcessing.ByteArrayToHexString(result));
            return result;
        }

        /// <summary>
        /// Indicates the end of the device names list
        /// </summary>
        public static byte[] DeviceNameameDataEndToByteArray()
            => ByteArrayUtil.CombineByteArrays(ByteArrayUtil.IntToByteArray(999, 2), [0x00]);


        internal static DeviceData Parse(byte[] data, Func<byte[], bool> responseTypeCheck, int? requestedIndex, int? requestedLoop)
        {
            if (!responseTypeCheck(data))
                return null;

            if (data.Length > 12)
            {
                DeviceData result = InitialisedNew(requestedLoop ?? 0);
                result.Index      = requestedIndex ?? 0;

                //CTecUtil.Debug.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< device=" + (result.Index + 1) + "  data=[" + ByteArrayProcessing.ByteArrayToHexString(data) + "]");
                              
                if (DeviceTypes.CurrentProtocolIsXfpApollo && result.Index >= DeviceConfigData.NumDevices)
                {   
                    result.Index -= DeviceConfigData.NumDevices + 1;
                    result.AncillaryBaseSounderGroup = data[5] & 0x1f;
                }
                else if (DeviceTypes.IsValidDeviceType(data[4], DeviceTypes.CurrentProtocolType))
                {
                    result.DeviceType = data[4];

                    var inUse0 = data[5]  != 0xff;
                    var inUse1 = data[7]  != 0xff;
                    var inUse2 = data[9]  != 0xff;
                    var inUse3 = data[11] != 0xff;

                    result.IOConfig[0].InputOutput = inUse0 ? (data[5] & 0x80)  > 0 ? IOTypes.Output : IOTypes.Input : IOTypes.NotUsed;
                    result.IOConfig[1].InputOutput = inUse1 ? (data[7] & 0x80)  > 0 ? IOTypes.Output : IOTypes.Input : IOTypes.NotUsed;
                    result.IOConfig[2].InputOutput = inUse2 ? (data[9] & 0x80)  > 0 ? IOTypes.Output : IOTypes.Input : IOTypes.NotUsed;
                    result.IOConfig[3].InputOutput = inUse3 ? (data[11] & 0x80) > 0 ? IOTypes.Output : IOTypes.Input : IOTypes.NotUsed;

                    if (inUse0) result.IOConfig[0].Channel = result.IOConfig[0].InputOutput == IOTypes.Output && (data[5]  & 0x60) == 0x60 ? 2 : ((data[5]  & 0x40) >> 6);
                    if (inUse1) result.IOConfig[1].Channel = result.IOConfig[1].InputOutput == IOTypes.Output && (data[7]  & 0x60) == 0x60 ? 2 : ((data[7]  & 0x40) >> 6);
                    if (inUse2) result.IOConfig[2].Channel = result.IOConfig[2].InputOutput == IOTypes.Output && (data[9]  & 0x60) == 0x60 ? 2 : ((data[9]  & 0x40) >> 6);
                    if (inUse3) result.IOConfig[3].Channel = result.IOConfig[3].InputOutput == IOTypes.Output && (data[11] & 0x60) == 0x60 ? 2 : ((data[11] & 0x40) >> 6);

                    if (inUse0)
                    {
                        if (result.IsZonalDevice)
                            result.Zone = data[5] & 0x3f;
                        else if (result.IsGroupedDevice)
                            result.Group = data[5] & 0x0f;
                    }

                    if (result.IsIODevice)
                    {
                        if (inUse0) result.IOConfig[0].ZoneGroupSet = result.IOConfig[0].InputOutput == IOTypes.Input ? data[5]  & 0x3f : data[5]  & 0x1f;
                        if (inUse1) result.IOConfig[1].ZoneGroupSet = result.IOConfig[1].InputOutput == IOTypes.Input ? data[7]  & 0x3f : data[7]  & 0x1f;
                        if (inUse2) result.IOConfig[2].ZoneGroupSet = result.IOConfig[2].InputOutput == IOTypes.Input ? data[9]  & 0x3f : data[9]  & 0x1f;
                        if (inUse3) result.IOConfig[3].ZoneGroupSet = result.IOConfig[3].InputOutput == IOTypes.Input ? data[11] & 0x3f : data[11] & 0x1f;
                    }

                    result.NameIndex = data[6];

                    if (result.IsIODevice)
                    {
                        if (inUse0) result.IOConfig[0].NameIndex = data[6];
                        if (inUse1) result.IOConfig[1].NameIndex = data[8];
                        if (inUse2) result.IOConfig[2].NameIndex = data[10];
                        if (inUse3) result.IOConfig[3].NameIndex = data[12];
                    }
                    else
                    {
                        if (result.IsModeDevice)
                        {
                            result.DayMode   = data[7];
                            result.NightMode = data[8];
                        }
                        else if (result.IsSensitivityDevice)
                        {
                            result.DaySensitivity   = data[7];
                            result.NightSensitivity = data[8];
                        }
                        
                        if (result.IsVolumeDevice)
                        {
                            //CAST PRO has both mode and volume, so volume is offset from the usual bytes
                            if (result.IsModeDevice)
                            {
                                result.DayVolume   = data[10] - 1;
                                result.NightVolume = data[11] - 1;
                            }
                            else
                            {
                                result.DayVolume   = data[7] - 1;
                                result.NightVolume = data[8] - 1;
                            }
                        }
                    }

                    //byte-12 can be remote LED for Apollo or group for CAST Pro
                    if (DeviceTypes.CurrentProtocolIsXfpApollo)
                        result.RemoteLEDEnabled = !result.IsIODevice && data[12] > 0;
                    else if (result.IsZonalDevice && result.IsGroupedDevice)
                        result.Group = data[12] & 0x0f;
                }
                return result;
            }

            return null;
        }


        internal static IndexedText ParseDeviceName(byte[] data, Func<byte[], bool> responseTypeCheck, int? requestedIndex)
        {
            if (!responseTypeCheck(data))
                return null;

            var stringValue = "";
            if (data.Length > 2)
            {
                try
                {
                    int length = data[1];

                    if (length == 1)
                    {
                        //a length of 1 denotes the end of the device names list, so cancel any remaining requests
                        PanelComms.TerminateSubqueue();
                        CTecUtil.CommsLog.AddText(Cultures.Resources.End_Of_Device_Names);
                        return null;
                    }
                    else
                    {
                        StringBuilder result = new();

                        if (data.Length > length + 2)
                        {
                            for (int i = 0; i < length && i < DeviceNamesConfigData.DeviceNameLength; i++)
                            {
                                if (data[i + 2] > 0)
                                    result.Append((char)data[i + 2]);
                                else
                                    break;
                            }
                        }
                        stringValue = result.ToString();
                    }
                }
                catch (Exception ex)
                {
                    CTecUtil.Debug.WriteLine("ParseDeviceName failed: " + ex.ToString());
                }
            }

            return new((int)requestedIndex, stringValue, DeviceNamesConfigData.DeviceNameLength);
        }
    }
}
