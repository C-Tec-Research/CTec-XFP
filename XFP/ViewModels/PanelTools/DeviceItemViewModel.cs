using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using CTecUtil.ViewModels;
using CTecControls;
using CTecControls.ViewModels;
using Xfp.DataTypes.PanelData;
using Xfp.UI.Interfaces;
using System.Collections.ObjectModel;
using CTecControls.UI;
using CTecDevices.Protocol;
using System.Security.Policy;
using Xfp.DataTypes;

namespace Xfp.ViewModels.PanelTools
{
    public class DeviceItemViewModel : ViewModelBase, IPanelToolsViewModel
    {
        public DeviceItemViewModel() { }

        internal DeviceData DeviceData
        {
            get => _deviceData;
            set
            {
                _deviceData = value;
                RefreshView();
            }
        }
        
        internal ZoneConfigData        ZoneData        { set => _zoneData = value; }
        internal GroupConfigData       GroupData       { set => _groupData = value; }


        private DeviceData _deviceData;
        private ZoneConfigData _zoneData;
        private GroupConfigData _groupData;

        public delegate string DeviceNameGetter(int index);
        public delegate int    DeviceNameSetter(int index, string value);

        public DeviceNameGetter GetDeviceName;
        public DeviceNameSetter SetDeviceName;


        public int?   DeviceType  { get => _deviceData.DeviceType;   set { _deviceData.DeviceType = value; RefreshView(); OnPropertyChanged(nameof(DeviceTypeIsValid)); } }
        public int    Zone        { get => _deviceData.Zone; set { _deviceData.Zone = value; RefreshView(); OnPropertyChanged(nameof(ZoneIsValid)); } }
        public int    Group       { get => _deviceData.Group;        set { _deviceData.Group = value; RefreshView(); OnPropertyChanged(nameof(GroupIsValid)); } }
        
        public int DeviceNameIndex
        {
            get => _deviceData.NameIndex;
            set
            {
                _deviceData.NameIndex = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DeviceNameIsValid));
            }
        }
        
        public string DeviceName
        {
            get => GetDeviceName?.Invoke(_deviceData.NameIndex);    //_deviceNamesData?.DeviceNames[_deviceData.NameIndex].Name;
            set
            {
                if (SetDeviceName is not null)
                {
                    var newIdx = SetDeviceName.Invoke(_deviceData.NameIndex, value);

                    if (newIdx != _deviceData.NameIndex)
                        _deviceData.NameIndex = newIdx;
                }
            }
        }
        
        public bool?  RemoteLEDEnabled          { get => _deviceData.RemoteLEDEnabled;                                  set { _deviceData.RemoteLEDEnabled = value; OnPropertyChanged(); } }
        public bool?  HasAncillaryBaseSounder     => (_deviceData.AncillaryBaseSounderGroup??0) >= 0;
        public int?   AncillaryBaseSounderGroup { get => _deviceData.AncillaryBaseSounderGroup;                         set { _deviceData.AncillaryBaseSounderGroup = value; OnPropertyChanged(); OnPropertyChanged(nameof(AncillaryBaseSounderGroupIsValid)); } }
        public int?   DaySensitivity            { get => DaySensitivityIsValid ? _deviceData.DaySensitivity : null;     set { _deviceData.DaySensitivity = value; OnPropertyChanged(); OnPropertyChanged(nameof(SensitivityValue)); OnPropertyChanged(nameof(DaySensitivityIsValid)); OnPropertyChanged(nameof(SensitivityIsValid)); } }
        public int?   NightSensitivity          { get => NightSensitivityIsValid ? _deviceData.NightSensitivity : null; set { _deviceData.NightSensitivity = value; OnPropertyChanged(); OnPropertyChanged(nameof(SensitivityValue)); OnPropertyChanged(nameof(DaySensitivityIsValid)); OnPropertyChanged(nameof(SensitivityIsValid)); } }
        public int?   DayVolume                 { get => DayVolumeIsValid ? _deviceData.DayVolume : null;               set { _deviceData.DayVolume = value; OnPropertyChanged(); OnPropertyChanged(nameof(VolumeValue)); OnPropertyChanged(nameof(DayVolumeIsValid)); OnPropertyChanged(nameof(VolumeIsValid)); } }
        public int?   NightVolume               { get => NightVolumeIsValid ? _deviceData.NightVolume : null;           set { _deviceData.NightVolume = value; OnPropertyChanged(); OnPropertyChanged(nameof(VolumeValue)); OnPropertyChanged(nameof(DayVolumeIsValid)); OnPropertyChanged(nameof(VolumeIsValid)); } }
        public int?   DayMode                   { get => DayModeIsValid ? _deviceData.DayMode : null;                   set { _deviceData.DayMode = value; OnPropertyChanged(); OnPropertyChanged(nameof(ModeValue)); OnPropertyChanged(nameof(DayModeIsValid)); OnPropertyChanged(nameof(ModeIsValid)); } }
        public int?   NightMode                 { get => NightModeIsValid ? _deviceData.NightMode : null;               set { _deviceData.NightMode = value; OnPropertyChanged(); OnPropertyChanged(nameof(ModeValue)); OnPropertyChanged(nameof(NightModeIsValid)); OnPropertyChanged(nameof(ModeIsValid)); } }
        public string SensitivityDesc             => IsSensitivityDevice ? Cultures.Resources.Sensitivity : "--";
        public string VolumeDesc                  => IsVolumeDevice ? Cultures.Resources.Volume : "--";
        public string ModeDesc                    => IsModeDevice ? Cultures.Resources.Mode : "--";
        public string SensitivityValue            => IsSensitivityDevice ? string.Format("{0} : {1}", DaySensitivity, NightSensitivity) : null;
        public string VolumeValue                 => IsVolumeDevice      ? string.Format("{0} : {1}", DayVolume + 1, NightVolume + 1) : null;
        public string ModeValue                   => IsModeDevice        ? string.Format("{0} : {1}", DayMode, NightMode) : null;
        

        private int? _deviceSelectorDeviceType;
        public int?  DeviceSelectorDeviceType      { get => _deviceSelectorDeviceType; set { _deviceSelectorDeviceType = value; OnPropertyChanged(nameof(DeviceTypeToolTip)); } }

        internal List<IOSettingData> IOConfigItems { get => _deviceData.IOConfig;      set { _deviceData.IOConfig = value; OnPropertyChanged(); } }

        public string DeviceTypeToolTip            { get => DeviceSelectorDeviceType != null ? Cultures.Resources.ToolTip_Click_To_Change_Device_Type : Cultures.Resources.ToolTip_Click_To_Show_Device_Details; }

        private bool _showOnlyIfFitted;

        public int LoopNum                         { get => _deviceData.LoopNum;       set { _deviceData.LoopNum = value; OnPropertyChanged(); } }

        public int Index
        {
            get => _deviceData.Index;
            set
            {
                _deviceData.Index = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DeviceNum));
                OnPropertyChanged(nameof(DeviceTypeIsValid));
            }
        }

        public int DeviceNum                     => _deviceData.Index + 1;
        public string DeviceTypeName             => DeviceTypes.DeviceTypeName(_deviceData.DeviceType, DeviceTypes.CurrentProtocolType);
        public int  ZoneIndex                  { get => _deviceData.Zone; set { _deviceData.Zone = value; RefreshView(); } }
        public int  GroupIndex                 { get => _deviceData.Group;        set { _deviceData.Group = value; RefreshView(); } }
        public bool SoundersCanHaveRemoteDevices => DeviceTypes.SoundersCanHaveRemoteDevices(DeviceTypes.CurrentProtocolType);
        public bool CanHaveAncillaryBaseSounder  => DeviceTypes.CanHaveAncillaryBaseSounder(_deviceData.DeviceType, DeviceTypes.CurrentProtocolType);
        public bool ShowOnlyIfFitted           { get => _showOnlyIfFitted;        set { _showOnlyIfFitted = value; OnPropertyChanged(); OnPropertyChanged(nameof(ShowThisDevice)); } }
        public bool IsEditable                   => DeviceTypes.IsValidDeviceType(DeviceType, DeviceTypes.CurrentProtocolType);
        public bool ShowThisDevice               => !_showOnlyIfFitted || DeviceTypes.IsValidDeviceType(DeviceType, DeviceTypes.CurrentProtocolType);
        public bool IsValidDeviceType            => DeviceTypes.IsValidDeviceType(_deviceData.DeviceType, DeviceTypes.CurrentProtocolType);

        public bool IsEnabled { get; set; }


        //public string ZoneGroup
        //{
        //    get
        //    {
        //        var zga = ZoneGroupSetIndex;
        //        if (zga > 0 && DeviceTypes.IsValidDeviceType(_deviceData.DeviceType, DeviceTypes.CurrentProtocolType))
        //        {
        //            if (IsZonalDevice)
        //            {
        //                if (zga <= ZoneConfigData.NumZones)
        //                {
        //                    var defaultname = string.Format(Cultures.Resources.Zone_x, zga);
        //                    var name = _zoneData.Zones[zga - 1].Name;
        //                    //return defaultname + (name != defaultname ? " - " + name : "");
        //                    if (name != defaultname)
        //                        return defaultname + " - " + name;
        //                    return name;
        //                }
        //            }
        //            else if (IsGroupedDevice)
        //            {
        //                if (zga <= GroupConfigData.NumSounderGroups)
        //                    return string.Format(Cultures.Resources.Group_x, zga);
        //            }
        //        }

        //        if (zga == 0)
        //            return Cultures.Resources.Use_In_Special_C_And_E;

        //        return "";
        //    }
        //}
        public string ZoneDesc
        {
            get
            {
                if (IsZonalDevice)
                {
                    var z = ZoneIndex;
                    if (z > 0 && DeviceTypes.IsValidDeviceType(_deviceData.DeviceType, DeviceTypes.CurrentProtocolType)
                     && z <= ZoneConfigData.NumZones)
                    {
                        var defaultname = string.Format(Cultures.Resources.Zone_x, z);
                        var name = _zoneData.Zones[z - 1].Name;
                        //return defaultname + (name != defaultname ? " - " + name : "");
                        if (name != defaultname)
                            return defaultname + " - " + name;
                        return name;
                    }

                    if (z == 0)
                        return Cultures.Resources.Use_In_Special_C_And_E;
                }
                return null;
            }
        }
        public string GroupDesc
        {
            get
            {
                if (IsGroupedDevice)
                {
                    var g = GroupIndex;
                    if (g > 0 && DeviceTypes.IsValidDeviceType(_deviceData.DeviceType, DeviceTypes.CurrentProtocolType)
                     && g <= GroupConfigData.NumSounderGroups)
                        return string.Format(Cultures.Resources.Group_x, g);

                    if (g == 0)
                        //if (ZoneDesc != Cultures.Resources.Use_In_Special_C_And_E)
                            return Cultures.Resources.Use_In_Special_C_And_E;
                }
                return null;
            }
        }

        public bool DeviceTypeIsValid                => DeviceType is null || DeviceTypes.IsValidDeviceType(DeviceType, DeviceTypes.CurrentProtocolType);
        public bool ZoneIsValid                      => !IsZonalDevice || Zone >= 0 && Zone <= ZoneConfigData.NumZones;
        public bool GroupIsValid                     => !IsGroupedDevice || Group >= 0 && Group <= GroupConfigData.NumSounderGroups;
        public bool DeviceNameIsValid                => DeviceType is null || DeviceName is not null && DeviceName.Length <= DeviceNamesConfigData.DeviceNameLength;
        public bool AncillaryBaseSounderGroupIsValid => !(HasAncillaryBaseSounder??false) || AncillaryBaseSounderGroup is null || AncillaryBaseSounderGroup >= 0 && AncillaryBaseSounderGroup <= GroupConfigData.NumSounderGroups;
        public bool DaySensitivityIsValid            => sensitivityIsValid(_deviceData.DaySensitivity);
        public bool NightSensitivityIsValid          => sensitivityIsValid(_deviceData.NightSensitivity);
        public bool DayVolumeIsValid                 => volumeIsValid(_deviceData.DayVolume);
        public bool NightVolumeIsValid               => volumeIsValid(_deviceData.NightVolume);
        public bool DayModeIsValid                   => modeIsValid(_deviceData.DayMode);
        public bool NightModeIsValid                 => modeIsValid(_deviceData.NightMode);
        public bool SensitivityIsValid               => DaySensitivityIsValid && NightSensitivityIsValid;
        public bool VolumeIsValid                    => DayVolumeIsValid && NightVolumeIsValid;
        public bool ModeIsValid                      => DayModeIsValid && NightModeIsValid;
        public bool SensVolModeIsValid               => SensitivityIsValid && VolumeIsValid && ModeIsValid;

        public bool IOConfigItemsAreValid
        {
            get
            {
                if (!IsIODevice)
                    return true;
                
                foreach (var io in IOConfigItems)
                    if (!io.Validate())
                        return false;
                
                return true;
            }
        }

        private bool volumeIsValid(int? value)       => !IsVolumeDevice || value is null || value >= DeviceConfigData.MinVolume - 1 && value <= DeviceConfigData.MaxVolume - 1;
        private bool sensitivityIsValid(int? value)  => !IsSensitivityDevice || value is null || (value >= (IsSensitivityHighDevice ? DeviceConfigData.MinSensitivityHigh : DeviceConfigData.MinSensitivity) 
                                                                                               && value <= (IsSensitivityHighDevice ? DeviceConfigData.MaxSensitivityHigh : DeviceConfigData.MaxSensitivity));
        private bool modeIsValid(int? value)         => !IsModeDevice || value is null || DeviceTypes.ModeIsValid(DeviceType, value);


        [JsonIgnore]
        public bool IsZonalDevice           => DeviceTypes.IsZonalDevice(_deviceData.DeviceType, DeviceTypes.CurrentProtocolType);
        [JsonIgnore]
        public bool IsGroupedDevice         => DeviceTypes.IsGroupedDevice(_deviceData.DeviceType, DeviceTypes.CurrentProtocolType);
        [JsonIgnore]
        public bool IsSetDevice             => DeviceTypes.IsSetDevice(_deviceData.DeviceType, DeviceTypes.CurrentProtocolType);
        [JsonIgnore]
        public bool IsIODevice              => DeviceTypes.IsIODevice(_deviceData.DeviceType, DeviceTypes.CurrentProtocolType);
        [JsonIgnore]
        public bool IOOutputIsGrouped       => DeviceTypes.IOOutputIsGrouped(_deviceData.DeviceType, DeviceTypes.CurrentProtocolType);
        public bool IsSensitivityDevice     => DeviceTypes.IsSensitivityDevice(_deviceData.DeviceType, DeviceTypes.CurrentProtocolType);
        public bool IsSensitivityHighDevice => DeviceTypes.IsSensitivityHighDevice(_deviceData.DeviceType, DeviceTypes.CurrentProtocolType);
        public bool IsVolumeDevice          => DeviceTypes.IsVolumeDevice(_deviceData.DeviceType, DeviceTypes.CurrentProtocolType);
        public bool IsModeDevice            => DeviceTypes.IsModeDevice(_deviceData.DeviceType, DeviceTypes.CurrentProtocolType);
        public bool IsSensVolModeDevice     => IsSensitivityDevice || IsSensitivityHighDevice || IsVolumeDevice || IsModeDevice;


        #region IAppViewModel implementation
        public void SetCulture(CultureInfo culture) => RefreshView();
        #endregion`


        #region IPanelToolsViewModel implementation
        public void PopulateView(XfpData data) { }

        public void RefreshView()
        {
            OnPropertyChanged(nameof(SoundersCanHaveRemoteDevices));
            OnPropertyChanged(nameof(DeviceType));
            OnPropertyChanged(nameof(DeviceTypeName));
            //OnPropertyChanged(nameof(ZoneGroup));
            OnPropertyChanged(nameof(ZoneDesc));
            OnPropertyChanged(nameof(GroupDesc));
            OnPropertyChanged(nameof(ZoneIndex));
            OnPropertyChanged(nameof(IsIODevice));
            OnPropertyChanged(nameof(IsGroupedDevice));
            OnPropertyChanged(nameof(IsZonalDevice));
            OnPropertyChanged(nameof(DeviceName));
            OnPropertyChanged(nameof(DeviceNameIndex));
            OnPropertyChanged(nameof(IsValidDeviceType));
            OnPropertyChanged(nameof(DeviceTypeIsValid));
            OnPropertyChanged(nameof(ZoneIsValid));
            OnPropertyChanged(nameof(GroupIsValid));
            OnPropertyChanged(nameof(IOConfigItems));
            OnPropertyChanged(nameof(IOConfigItemsAreValid));
            OnPropertyChanged(nameof(DeviceNameIsValid));
            OnPropertyChanged(nameof(IsSensitivityDevice));
            OnPropertyChanged(nameof(IsSensitivityHighDevice));
            OnPropertyChanged(nameof(IsVolumeDevice));
            OnPropertyChanged(nameof(IsModeDevice));
            OnPropertyChanged(nameof(IsSensVolModeDevice));
            OnPropertyChanged(nameof(SensitivityDesc));
            OnPropertyChanged(nameof(VolumeDesc));
            OnPropertyChanged(nameof(ModeDesc));
            OnPropertyChanged(nameof(SensitivityValue));
            OnPropertyChanged(nameof(VolumeValue));
            OnPropertyChanged(nameof(ModeValue));
            OnPropertyChanged(nameof(DaySensitivity));
            OnPropertyChanged(nameof(NightSensitivity));
            OnPropertyChanged(nameof(DayVolume));
            OnPropertyChanged(nameof(NightVolume));
            OnPropertyChanged(nameof(DaySensitivityIsValid));
            OnPropertyChanged(nameof(NightSensitivityIsValid));
            OnPropertyChanged(nameof(DayVolumeIsValid));
            OnPropertyChanged(nameof(NightVolumeIsValid));
            OnPropertyChanged(nameof(DayModeIsValid));
            OnPropertyChanged(nameof(NightModeIsValid));
            OnPropertyChanged(nameof(SensVolModeIsValid));
            OnPropertyChanged(nameof(RemoteLEDEnabled));
            OnPropertyChanged(nameof(CanHaveAncillaryBaseSounder));
            OnPropertyChanged(nameof(HasAncillaryBaseSounder));
            OnPropertyChanged(nameof(AncillaryBaseSounderGroup));
            OnPropertyChanged(nameof(AncillaryBaseSounderGroupIsValid));
        }
        #endregion
    }
}
