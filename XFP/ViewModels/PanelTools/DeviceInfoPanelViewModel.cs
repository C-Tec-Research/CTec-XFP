using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using CTecControls.UI;
using Xfp.DataTypes.PanelData;
using CTecDevices.Protocol;
using Xfp.UI.Interfaces;
using Xfp.DataTypes;
using CTecDevices.DeviceTypes;
using static Xfp.ViewModels.PanelTools.DeviceItemViewModel;
using CTecDevices;
using CTecDevices.DataTypes;

namespace Xfp.ViewModels.PanelTools
{
    /// <summary>
    /// Viewmodel for Device.xaml
    /// </summary>
    public class DeviceInfoPanelViewModel : PanelToolsPageViewModelBase, IPanelToolsViewModel
    {
        public DeviceInfoPanelViewModel(FrameworkElement parent) : base(parent)
        {
            //add DetectedLoops update to timer
            _chChCheckChanges.Elapsed += new((s, e) => OnPropertyChanged(nameof(DetectedLoops)));
        }


        private Collection<DeviceItemViewModel> _deviceList = new();
        public Collection<DeviceItemViewModel> DeviceList { get => _deviceList; set { _deviceList = value; updateModesList(); RefreshView(); } }


        internal DeviceNameGetter GetDeviceName;
        internal DeviceNameSetter SetDeviceName;


        private bool _displayShowFittedDeviceOnlyOption = true;
        private bool _showFittedDeviceOnly;

        public int MaxNameLength => DeviceNamesConfigData.DeviceNameLength;

        private int? _deviceSelectorDeviceType;

        public bool CurrentProtocolIsXfpCast => DeviceTypes.CurrentProtocolIsXfpCast;

        public int? DeviceSelectorDeviceType { get => _deviceSelectorDeviceType; set { _deviceSelectorDeviceType = value; OnPropertyChanged(); OnPropertyChanged(nameof(DeviceTypeToolTip)); } }
        public string DeviceTypeToolTip { get => DeviceSelectorDeviceType != null ? Cultures.Resources.ToolTip_Click_To_Change_Device_Type : null; }


        public override bool DebugMode { get => base.DebugMode; set { base.DebugMode = value; updateDebugInfo(); } }


        public int? DetectedLoops { get => LoopConfigData.DetectedLoops; set { } }


        public bool NoDeviceDetails
        {
            get
            {
                foreach (var _ in from d in _deviceList
                                  where d is not null
                                  select new { })
                    return false;

                return true;
            }
        }

        public bool NoDeviceType
        {
            get
            {
                if (NoDeviceDetails)
                    return true;

                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (d.DeviceType is not null)
                        return false;
                }
                return true;
            }
        }

        
        public bool DisplayShowFittedDevicesOnlyOption { get => _displayShowFittedDeviceOnlyOption; set { _displayShowFittedDeviceOnlyOption = value; OnPropertyChanged(); } }

        //public bool ShowFittedDevicesOnly { get => _showFittedDeviceOnly; set { _showFittedDeviceOnly = value; OnShowFittedDeviceChange?.Invoke(_showFittedDeviceOnly); OnPropertyChanged(); } }

        public bool SoundersCanHaveRemoteDevices  => DeviceTypes.SoundersCanHaveRemoteDevices(DeviceTypes.CurrentProtocolType);


        #region alarm verification count
        public int ManualCallpoints { get => _data?.CurrentPanel.PanelConfig.MCPDebounce ?? MinManualCallpoints; set { _data.CurrentPanel.PanelConfig.MCPDebounce = value; OnPropertyChanged(); } }
        public int InterfaceUnits   { get => _data?.CurrentPanel.PanelConfig.IODebounce ?? MinInterfaceUnits;    set { _data.CurrentPanel.PanelConfig.IODebounce = value; OnPropertyChanged(); } }
        public int Detectors        { get => _data?.CurrentPanel.PanelConfig.DetectorDebounce ?? MinDetectors;   set { _data.CurrentPanel.PanelConfig.DetectorDebounce = value; OnPropertyChanged(); } }

        public int MinManualCallpoints => LoopConfigData.MinManualCallpoints;
        public int MaxManualCallpoints => LoopConfigData.MaxManualCallpoints;
        public int MinInterfaceUnits => LoopConfigData.MinInterfaceUnits;
        public int MaxInterfaceUnits => LoopConfigData.MaxInterfaceUnits;
        public int MinDetectors => LoopConfigData.MinDetectors;
        public int MaxDetectors => LoopConfigData.MaxDetectors;
        public int MinSensitivity => IsSensitivityHighDevice??false ? DeviceConfigData.MinSensitivityHigh : DeviceConfigData.MinSensitivity;
        public int MaxSensitivity => IsSensitivityHighDevice??false ? DeviceConfigData.MaxSensitivityHigh : DeviceConfigData.MaxSensitivity;
        public int DefaultSensitivity => IsSensitivityHighDevice??false ? DeviceConfigData.DefaultSensitivityHigh : DeviceConfigData.DefaultSensitivity;
        public int MinVolume => DeviceConfigData.MinVolume;
        public int MaxVolume => DeviceConfigData.MaxVolume;
        #endregion


        /// <summary>dummy invalid value for checking base sounder group</summary>
        private const int _noBaseSounderGroup = -9999;


        #region selected devices properties
        private int? _zoneIndex;
        private int? _groupIndex;
        //private int? _dayVolumeIndex;
        //private int? _nightVolumeIndex;
        private ModeSettingOption _dayMode;
        private ModeSettingOption _nightMode;
        private int? _ancillaryBaseSounderGroupIndex;

        public int? DeviceType
        {
            get
            {
                if (NoDeviceDetails)
                    return null;

                if (_deviceList.Count == 1)
                    return _deviceList[0].DeviceType;

                int? result = null;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (result is null)
                        result = d.DeviceType;
                    else if (result != d.DeviceType)
                        return null;
                }
                return result;
            }
            set
            {
                if (value is not null)
                    foreach (var d in _deviceList)
                        d.DeviceType = value;

                RefreshView();
            }
        }

        public string DeviceTypeName
        {
            get
            {
                if (NoDeviceDetails)
                    return "";

                if (_deviceList.Count == 1)
                    return _deviceList[0].DeviceTypeName;

                string result = "";
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (result == "")
                        result = d.DeviceTypeName;
                    else if (result != d.DeviceTypeName)
                        return "";
                }

                return result;
            }
        }

        public int? DeviceNum
        {
            get
            {
                if (_deviceList.Count == 1)
                    return _deviceList[0]?.DeviceNum;

                int? result = null;
                foreach (var d in _deviceList)
                {
                    if (d is not null)
                    {
                        if (result is null)
                            result = d.DeviceNum;
                        else if (result != d.DeviceNum)
                            return null;
                    }
                }
                return result;
            }
        }

        public string ZoneGroupDesc
        {
            get
            {
                if (NoDeviceDetails)
                    return "";

                bool? isGroup = null;
                bool? isZone = null;
                bool? isIO = null;

                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (isGroup is null)
                        isGroup = d.IsGroupedDevice;
                    else if (isGroup != d.IsGroupedDevice)
                        return "";

                    if (isZone is null)
                        isZone = d.IsZonalDevice;
                    else if (isZone != d.IsZonalDevice)
                        return "";
                }

                return isGroup??false ? Cultures.Resources.Group : isZone??false ? Cultures.Resources.Zone : isIO??false ? Cultures.Resources.IO_Config : Cultures.Resources.Zone_Group;
            }
        }

        public string Zone
        {
            get
            {
                _zoneIndex = null;

                if (NoDeviceDetails)
                    return null;

                if (IsZonedDevice is null)
                    return null;

                if (_deviceList.Count == 1)
                {
                    _zoneIndex = _deviceList[0].ZoneIndex;
                    if (!ZoneIsValid)
                    {
                        _zoneIndex = -1;
                        return null;
                    }
                    return Zones[_zoneIndex??0];
                }

                _zoneIndex = null;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (_zoneIndex is null)
                        _zoneIndex = d.ZoneIndex;
                    else if (_zoneIndex != d.ZoneIndex)
                    {
                        _zoneIndex = null;
                        return null;
                    }
                }

                return _zoneIndex is not null && _zoneIndex > -1 && _zoneIndex < ZoneConfigData.NumZones ?  Zones[(int)_zoneIndex] : null;
            }
            set
            {
                if (value is not null & !NoDeviceDetails)
                    foreach (var d in _deviceList)
                        d.ZoneIndex = (_zoneIndex = findIndexInCombo(Zones, value))??0;

                OnPropertyChanged();
                OnPropertyChanged(nameof(ZoneIsValid));
                OnPropertyChanged(nameof(IOZoneSet1IsValid));
                OnPropertyChanged(nameof(IOZoneSet2IsValid));
                OnPropertyChanged(nameof(IOZoneSet3IsValid));
                OnPropertyChanged(nameof(IOZoneSet4IsValid));
                OnPropertyChanged(nameof(DevicesHaveCommonZone));
                OnPropertyChanged(nameof(DevicesHaveCommonZGSType));
                OnPropertyChanged(nameof(IndicateMultipleValues));
            }
        }

        public string Group
        {
            get
            {
                _groupIndex = null;

                if (NoDeviceDetails)
                    return null;

                if (IsGroupedDevice is null)
                    return null;

                if (_deviceList.Count == 1)
                {
                    _groupIndex = _deviceList[0].GroupIndex;
                    if (!GroupIsValid)
                    {
                        _groupIndex = -1;
                        return null;
                    }
                    return Groups[_groupIndex??0];
                }

                _groupIndex = null;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (_groupIndex is null)
                        _groupIndex = d.GroupIndex;
                    else if (_groupIndex != d.GroupIndex)
                    {
                        _groupIndex = null;
                        return null;
                    }
                }

                return _groupIndex is not null && _groupIndex > -1 && _groupIndex < GroupConfigData.NumSounderGroups ? Groups[(int)_groupIndex] : null;
            }
            set
            {
                if (value is not null & !NoDeviceDetails)
                    foreach (var d in _deviceList)
                        d.GroupIndex = (_groupIndex = findIndexInCombo(Groups, value))??0;

                OnPropertyChanged();
                OnPropertyChanged(nameof(GroupIsValid));
                OnPropertyChanged(nameof(IOZoneSet1IsValid));
                OnPropertyChanged(nameof(IOZoneSet2IsValid));
                OnPropertyChanged(nameof(IOZoneSet3IsValid));
                OnPropertyChanged(nameof(IOZoneSet4IsValid));
                OnPropertyChanged(nameof(DevicesHaveCommonZone));
                OnPropertyChanged(nameof(DevicesHaveCommonZGSType));
                OnPropertyChanged(nameof(IndicateMultipleValues));
            }
        }

        public string AncillaryBaseSounderGroup
        {
            get
            {
                _ancillaryBaseSounderGroupIndex = null;

                if (NoDeviceDetails)
                    return null;

                if (!DevicesHaveCommonHasAncillaryBaseSounder)
                    return null;

                if (_deviceList.Count == 1)
                    return AncillaryBaseSounderGroups[_deviceList[0].AncillaryBaseSounderGroup??0];

                _ancillaryBaseSounderGroupIndex = null;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (_ancillaryBaseSounderGroupIndex is null)
                        _ancillaryBaseSounderGroupIndex = d.AncillaryBaseSounderGroup??0;
                    else if (_ancillaryBaseSounderGroupIndex != d.AncillaryBaseSounderGroup)
                    {
                        _ancillaryBaseSounderGroupIndex = null;
                        return null;
                    }
                }

                return _ancillaryBaseSounderGroupIndex is not null && _ancillaryBaseSounderGroupIndex > -1 ? AncillaryBaseSounderGroups[(int)_ancillaryBaseSounderGroupIndex] : null;
            }

            set
            {
                if (value is not null)
                    foreach (var d in _deviceList)
                        if (!NoDeviceDetails)
                            d.AncillaryBaseSounderGroup = findIndexInCombo(AncillaryBaseSounderGroups, value) ?? 0;

                OnPropertyChanged();
                OnPropertyChanged(nameof(DevicesHaveCommonHasAncillaryBaseSounder));
                OnPropertyChanged(nameof(DevicesHaveCommonAncillaryBaseSounderGroup));
                OnPropertyChanged(nameof(IndicateMultipleValues));
            }
        }


        public int? DaySensitivity
        {
            get
            {
                if (_deviceList.Count == 1)
                    if (_deviceList[0]?.DaySensitivityIsValid == true)
                        return _deviceList[0]?.DaySensitivity;

                int? result = null;
                bool? canHave = null;
                foreach (var d in _deviceList)
                {
                    if (d?.DeviceType is null)
                        continue;

                    if (result is null)
                        if (d.DaySensitivityIsValid)
                            result = d.DaySensitivity;
                    
                    if (canHave is null)
                        canHave = d.IsSensitivityDevice;
                    else if (canHave != d.IsSensitivityDevice)
                        return null;
                    else if (result != d.DaySensitivity)
                        return null;
                }
                return result??0;
            }
            set
            {
                if (value is not null)
                    foreach (var d in _deviceList)
                        if (d is not null)
                            d.DaySensitivity = NoDeviceDetails ? null : value.Value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(DaySensitivity));
                OnPropertyChanged(nameof(DaySensitivityIsValid));
                OnPropertyChanged(nameof(DevicesHaveCommonDaySensitivity));
                OnPropertyChanged(nameof(IndicateMultipleValues));
            }
        }

        public int? NightSensitivity
        {
            get
            {
                if (_deviceList.Count == 1)
                    if (_deviceList[0]?.NightSensitivityIsValid == true)
                        return _deviceList[0]?.NightSensitivity;

                int? result = null;
                bool? canHave = null;
                foreach (var d in _deviceList)
                {
                    if (d?.DeviceType is null)
                        continue;

                    if (result is null)
                        if (d.NightSensitivityIsValid)
                            result = d.NightSensitivity;
                    
                    if (canHave is null)
                        canHave = d.IsSensitivityDevice;
                    else if (canHave != d.IsSensitivityDevice)
                        return null;
                    else if (result != d.NightSensitivity)
                        return null;
                }
                return result??0;
            }
            set
            {
                if (value is not null)
                    foreach (var d in _deviceList)
                        if (d is not null)
                            d.NightSensitivity = NoDeviceDetails ? null : value.Value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(NightSensitivity));
                OnPropertyChanged(nameof(NightSensitivityIsValid));
                OnPropertyChanged(nameof(DevicesHaveCommonNightSensitivity));
                OnPropertyChanged(nameof(IndicateMultipleValues));
            }
        }

        public string DayVolume
        {
            get
            {
                if (NoDeviceDetails || DeviceType is null)
                    return null;
                
                if (!IsVolumeDevice??false || !DevicesHaveCommonDayVolume)
                    return null;

                if (_deviceList.Count == 1)
                    return _deviceList[0].DayVolumeIsValid ? Volumes[_deviceList[0].DayVolume??0] : null;

                int? dayVolIdx = null;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (dayVolIdx is null)
                    {
                        if (d.DayVolumeIsValid)
                            dayVolIdx = d.DayVolume;
                    }
                    else if (dayVolIdx != d.DayVolume)
                        return null;
                }

                return dayVolIdx is not null && dayVolIdx > -1 ? Volumes[(int)dayVolIdx] : null;
            }

            set
            {
                if (value is not null)
                    foreach (var d in _deviceList)
                        if (!NoDeviceDetails)
                            d.DayVolume = findIndexInCombo(Volumes, value) ?? 0;

                OnPropertyChanged();
                OnPropertyChanged(nameof(DayVolumeIsValid));
                OnPropertyChanged(nameof(DevicesHaveCommonDayVolume));
                OnPropertyChanged(nameof(IndicateMultipleValues));
            }
        }

        public string NightVolume
        {
            get
            {
                if (NoDeviceDetails || DeviceType is null)
                    return null;

                if (!IsVolumeDevice??false || !DevicesHaveCommonNightVolume)
                    return null;

                if (_deviceList.Count == 1)
                    return _deviceList[0].NightVolumeIsValid ? Volumes[_deviceList[0].NightVolume??0] : null;

                int? nightVolIdx = null;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (nightVolIdx is null)
                    {
                        if (d.NightVolumeIsValid)
                            nightVolIdx = d.NightVolume;
                    }
                    else if (nightVolIdx != d.NightVolume)
                        return null;
                }

                return nightVolIdx is not null && nightVolIdx > -1 ? Volumes[(int)nightVolIdx] : null;
            }

            set
            {
                if (value is not null)
                    foreach (var d in _deviceList)
                        if (!NoDeviceDetails)
                            d.NightVolume = findIndexInCombo(Volumes, value) ?? 0;

                OnPropertyChanged();
                OnPropertyChanged(nameof(NightVolumeIsValid));
                OnPropertyChanged(nameof(DevicesHaveCommonNightVolume));
                OnPropertyChanged(nameof(IndicateMultipleValues));
            }
        }

        private ModeSettingOption findModeInList(int? modeNum)
        {
            if (modeNum is null) return null;
            foreach (var m in from m in Modes where m.Number == modeNum select m) return m;
            return null;
        }

        public ModeSettingOption DayMode
        {
            get
            {
                _dayMode = null;

                if (Modes.Count == 0)
                    return null;

                if (NoDeviceDetails)
                    return null;

                if (!IsModeDevice ?? false || !DevicesHaveCommonDayMode)
                    return null;

                if (_deviceList.Count == 1)
                    return _dayMode =  findModeInList(_deviceList[0].DayMode);

                int? mode = null;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (mode is null)
                    {
                        if (d.DayModeIsValid)
                            mode = d.DayMode??-1;
                        else
                            mode = -1;
                    }
                    else if (mode != d.DayMode)
                        return _dayMode = null;
                }

                return _dayMode = findModeInList(mode);
            }
            set
            {
                if (value is not null && !NoDeviceDetails)
                {
                    if (value.IsEnabled)
                        foreach (var d in _deviceList)
                            if (d is not null)
                                d.DayMode = value?.Number;

                    OnPropertyChanged();
                    OnPropertyChanged(nameof(DayModeIsInvalid));
                    OnPropertyChanged(nameof(DevicesHaveCommonDayMode));
                    OnPropertyChanged(nameof(IndicateMultipleValues));
                }
            }
        }

        public ModeSettingOption NightMode
        {
            get
            {
                _nightMode = null;

                if (Modes.Count == 0)
                    return null;

                if (NoDeviceDetails)
                    return null;

                if (!IsModeDevice ?? false || !DevicesHaveCommonNightMode)
                    return null;

                if (_deviceList.Count == 1)
                    return _nightMode =  findModeInList(_deviceList[0].NightMode);

                int? mode = null;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (mode is null)
                    {
                        if (d.NightModeIsValid)
                            mode = d.NightMode??-1;
                        else
                            mode = -1;
                    }
                    else if (mode != d.NightMode)
                        return _nightMode = null;
                }

                return _nightMode = findModeInList(mode);
            }
            set
            {
                if (value is not null && !NoDeviceDetails)
                    foreach (var d in _deviceList)
                        if (d is not null)
                            d.NightMode = value?.Number;

                OnPropertyChanged();
                OnPropertyChanged(nameof(NightModeIsInvalid));
                OnPropertyChanged(nameof(DevicesHaveCommonNightMode));
                OnPropertyChanged(nameof(IndicateMultipleValues));
            }
        }

        public bool SensitivitiesHaveSameRange
        {
            get
            {
                if (_deviceList.Count == 1)
                    return true;

                bool? result = null;
                bool? canHave = null;
                foreach (var d in _deviceList)
                {
                    if (d?.DeviceType is null)
                        continue;

                    if (result is null)
                        result = d.IsSensitivityHighDevice;
                    
                    if (canHave is null)
                        canHave = d.IsSensitivityDevice;
                    else if (canHave != d.IsSensitivityDevice)
                        return false;
                    else if (result != d.IsSensitivityHighDevice)
                        return false;
                }
                return result is not null;
            }
        }

        public string DeviceName
        {
            get
            {
                if (NoDeviceDetails)
                    return "";

                if (_deviceList.Count == 1)
                    return _deviceList[0].DeviceName;

                string result = "";
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (result == "")
                        result = d.DeviceName;
                    else if (result != d.DeviceName)
                        return "";
                }
                return result;
            }
            set
            {
                if (checkNameMemoryLimit(value))
                {
                    foreach (var d in _deviceList)
                    {
                        if (d.DeviceType is null)
                            continue;

                        d.DeviceName = value;
                        d.RefreshView();

                        if (validIOIndex(0, d))
                            d.IOConfigItems[0].NameIndex = d.DeviceData.NameIndex;
                    }
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(DeviceNameIsValid));
                OnPropertyChanged(nameof(DevicesHaveCommonDeviceName));
                OnPropertyChanged(nameof(IndicateMultipleValues));
                updateDebugInfo();
            }
        }


        private bool setDeviceName(string value)
        {
            if (!checkNameMemoryLimit(value))
                return false;

            foreach (var d in _deviceList)
            {
                if (d.DeviceType is null)
                    continue;

                d.DeviceName = value;
                d.RefreshView();

                if (validIOIndex(0, d))
                    //setIODescription(0, value);
                    d.IOConfigItems[0].NameIndex = d.DeviceData.NameIndex;
            }
        
            OnPropertyChanged();
            OnPropertyChanged(nameof(DeviceNameIsValid));
            OnPropertyChanged(nameof(DevicesHaveCommonDeviceName));
            OnPropertyChanged(nameof(IndicateMultipleValues));
            updateDebugInfo();
            return true;
        }


        /// <summary>
        /// Refreshes the debug info (show via Ctrl-Alt-Shift-U)
        /// </summary>
        private void updateDebugInfo()
        {
            OnPropertyChanged(nameof(DetectedLoops));
            OnPropertyChanged(nameof(DebugNamesUsed));
            OnPropertyChanged(nameof(DebugNameBytesUsed));
            OnPropertyChanged(nameof(DebugNameBytesRemaining));
        }


        public bool? RemoteLEDEnabled
        {
            get
            {
                if (NoDeviceDetails)
                    return null;

                if (_deviceList.Count == 1)
                    return _deviceList[0].RemoteLEDEnabled;

                bool? rle = null;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (!d.IsSensitivityDevice)
                        rle = false;

                    if (rle is null)
                        rle = d.RemoteLEDEnabled;
                    else if (rle != d.RemoteLEDEnabled)
                        return null;
                }
                return rle;
            }
            set
            {
                try
                {
                    foreach (var d in _deviceList)
                        d.RemoteLEDEnabled = value;
                }
                finally
                {
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(DevicesHaveCommonRemoteLEDEnabled));
                    OnPropertyChanged(nameof(IndicateMultipleValues));
                }
            }
        }


        #region I/O settings
        public int    IOInputOutput1     { get => (int)getInputOutput(0); set => setInputOutput(0, value); }
        public int    IOInputOutput2     { get => (int)getInputOutput(1); set => setInputOutput(1, value); }
        public int    IOInputOutput3     { get => (int)getInputOutput(2); set => setInputOutput(2, value); }
        public int    IOInputOutput4     { get => (int)getInputOutput(3); set => setInputOutput(3, value); }
        public string IOInputOutputDesc1 { get { var io = (int)getInputOutput(0); return io > -1 && io >= 0 ? InputOutputs[(int)io] : ""; } }
        public string IOInputOutputDesc2 { get { var io = (int)getInputOutput(1); return io > -1 && io >= 0 ? InputOutputs[(int)io] : ""; } }
        public string IOInputOutputDesc3 { get { var io = (int)getInputOutput(2); return io > -1 && io >= 0 ? InputOutputs[(int)io] : ""; } }
        public string IOInputOutputDesc4 { get { var io = (int)getInputOutput(3); return io > -1 && io >= 0 ? InputOutputs[(int)io] : ""; } }
        public int    IOInputChannel1    { get => getIOChannel(0);         set => setChannel(0, value); }
        public int    IOInputChannel2    { get => getIOChannel(1);         set => setChannel(1, value); }
        public int    IOInputChannel3    { get => getIOChannel(2);         set => setChannel(2, value); }
        public int    IOInputChannel4    { get => getIOChannel(3);         set => setChannel(3, value); }
        public int?   IOOutputChannel1   { get => getIOChannel(0);         set => setChannel(0, value); }
        public int?   IOOutputChannel2   { get => getIOChannel(1);         set => setChannel(1, value); }
        public int?   IOOutputChannel3   { get => getIOChannel(2);         set => setChannel(2, value); }
        public int?   IOOutputChannel4   { get => getIOChannel(3);         set => setChannel(3, value); }
        public string IOZoneSet1         { get => getIOZoneSet(0);         set => setIOZoneSet(0, value); }
        public string IOZoneSet2         { get => getIOZoneSet(1);         set => setIOZoneSet(1, value); }
        public string IOZoneSet3         { get => getIOZoneSet(2);         set => setIOZoneSet(2, value); }
        public string IOZoneSet4         { get => getIOZoneSet(3);         set => setIOZoneSet(3, value); }
        public string IODescription1     { get => getIODescription(0);     set => DeviceName = value; }
        public string IODescription2     { get => getIODescription(1);     set { setIODescription(1, value); updateDebugInfo(); } }
        public string IODescription3     { get => getIODescription(2);     set { setIODescription(2, value); updateDebugInfo(); } }
        public string IODescription4     { get => getIODescription(3);     set { setIODescription(3, value); updateDebugInfo(); } }
      
        public bool IOInUse1   => getInputOutput(0) != IOTypes.NotUsed;
        public bool IOInUse2   => getInputOutput(1) != IOTypes.NotUsed;
        public bool IOInUse3   => getInputOutput(2) != IOTypes.NotUsed;
        public bool IOInUse4   => getInputOutput(3) != IOTypes.NotUsed;
        public bool IOIsInput1 => getInputOutput(0) == IOTypes.Input;
        public bool IOIsInput2 => getInputOutput(1) == IOTypes.Input;
        public bool IOIsInput3 => getInputOutput(2) == IOTypes.Input;
        public bool IOIsInput4 => getInputOutput(3) == IOTypes.Input;

        private IOTypes getInputOutput(int index)
        {
            if (NoDeviceDetails)
                return (IOTypes)(-1);

            IOTypes? io = null;

            foreach (var d in _deviceList)
            {
                if (d.DeviceType is null)
                    return (IOTypes)(-1);

                if (d.IsIODevice && validIOIndex(index, d))
                {
                    if (io is null)
                        io = d.IOConfigItems[index].InputOutput;
                    else if (io != d.IOConfigItems[index].InputOutput)
                        return (IOTypes)(-1);
                }
            }

            return io??(IOTypes)(-1);
        }

        private void setInputOutput(int index, int? value)
        {
            if (value is not null)
                foreach (var d in _deviceList)
                    if (validIOIndex(index, d))
                        d.IOConfigItems[index].InputOutput = (IOTypes)value;
            
            OnPropertyChanged(nameof(IOInputOutput1));
            OnPropertyChanged(nameof(IOInputOutput2));
            OnPropertyChanged(nameof(IOInputOutput3));
            OnPropertyChanged(nameof(IOInputOutput4));
            OnPropertyChanged(nameof(IOInUse1));
            OnPropertyChanged(nameof(IOInUse2));
            OnPropertyChanged(nameof(IOInUse3));
            OnPropertyChanged(nameof(IOInUse4));
            OnPropertyChanged(nameof(IOIsInput1));
            OnPropertyChanged(nameof(IOIsInput2));
            OnPropertyChanged(nameof(IOIsInput3));
            OnPropertyChanged(nameof(IOIsInput4));
            OnPropertyChanged(nameof(IOZoneSet1));
            OnPropertyChanged(nameof(IOZoneSet2));
            OnPropertyChanged(nameof(IOZoneSet3));
            OnPropertyChanged(nameof(IOZoneSet4));
            OnPropertyChanged(nameof(IOInputOutput1IsValid));
            OnPropertyChanged(nameof(IOInputOutput2IsValid));
            OnPropertyChanged(nameof(IOInputOutput3IsValid));
            OnPropertyChanged(nameof(IOInputOutput4IsValid));
            OnPropertyChanged(nameof(IOChannel1IsValid));
            OnPropertyChanged(nameof(IOChannel2IsValid));
            OnPropertyChanged(nameof(IOChannel3IsValid));
            OnPropertyChanged(nameof(IOChannel4IsValid));
            OnPropertyChanged(nameof(IOZoneSet1IsValid));
            OnPropertyChanged(nameof(IOZoneSet2IsValid));
            OnPropertyChanged(nameof(IOZoneSet3IsValid));
            OnPropertyChanged(nameof(IOZoneSet4IsValid));
            OnPropertyChanged(nameof(IODescription1IsValid));
            OnPropertyChanged(nameof(IODescription2IsValid));
            OnPropertyChanged(nameof(IODescription3IsValid));
            OnPropertyChanged(nameof(IODescription4IsValid));
            OnPropertyChanged(nameof(DevicesHaveCommonIOInputOutput1));
            OnPropertyChanged(nameof(DevicesHaveCommonIOInputOutput2));
            OnPropertyChanged(nameof(DevicesHaveCommonIOInputOutput3));
            OnPropertyChanged(nameof(DevicesHaveCommonIOInputOutput4));
            OnPropertyChanged(nameof(DevicesHaveCommonIOChannel1));
            OnPropertyChanged(nameof(DevicesHaveCommonIOChannel2));
            OnPropertyChanged(nameof(DevicesHaveCommonIOChannel3));
            OnPropertyChanged(nameof(DevicesHaveCommonIOChannel4));
            OnPropertyChanged(nameof(DevicesHaveCommonIOZoneSet1));
            OnPropertyChanged(nameof(DevicesHaveCommonIOZoneSet2));
            OnPropertyChanged(nameof(DevicesHaveCommonIOZoneSet3));
            OnPropertyChanged(nameof(DevicesHaveCommonIOZoneSet4));
            OnPropertyChanged(nameof(DevicesHaveCommonIODescription1));
            OnPropertyChanged(nameof(DevicesHaveCommonIODescription2));
            OnPropertyChanged(nameof(DevicesHaveCommonIODescription3));
            OnPropertyChanged(nameof(DevicesHaveCommonIODescription4));
        }

        private int getIOChannel(int index)
        {
            if (NoDeviceDetails)
                return -1;

            int? channel = null;

            foreach (var d in _deviceList)
            {
                if (d.DeviceType is null)
                    return -1;

                if (d.IsIODevice && validIOIndex(index, d))
                {
                    if (channel is null)
                        channel = d.IOConfigItems[index].Channel;
                    else if (channel != d.IOConfigItems[index].Channel)
                        return -1;
                }
            }

            return channel??-1;
        }

        private void setChannel(int index, int? value)
        {
            if (value is not null)
                foreach (var d in _deviceList)
                    if (validIOIndex(index, d))
                        d.IOConfigItems[index].Channel = value;

            OnPropertyChanged(nameof(IOInputChannel1));
            OnPropertyChanged(nameof(IOInputChannel2));
            OnPropertyChanged(nameof(IOInputChannel3));
            OnPropertyChanged(nameof(IOInputChannel4));
            OnPropertyChanged(nameof(IOOutputChannel1));
            OnPropertyChanged(nameof(IOOutputChannel2));
            OnPropertyChanged(nameof(IOOutputChannel3));
            OnPropertyChanged(nameof(IOOutputChannel4));
            OnPropertyChanged(nameof(IOChannel1IsValid));
            OnPropertyChanged(nameof(IOChannel2IsValid));
            OnPropertyChanged(nameof(IOChannel3IsValid));
            OnPropertyChanged(nameof(IOChannel4IsValid));
            OnPropertyChanged(nameof(DevicesHaveCommonIOChannel1));
            OnPropertyChanged(nameof(DevicesHaveCommonIOChannel2));
            OnPropertyChanged(nameof(DevicesHaveCommonIOChannel3));
            OnPropertyChanged(nameof(DevicesHaveCommonIOChannel4));
        }

        private int? getIOZoneSetIndex(int index)
        {
            if (NoDeviceDetails)
                return null;

            int? zoneGroup = null;

            foreach (var d in _deviceList)
            {
                if (d.DeviceType is null)
                    return null;

                if (d.IsIODevice && validIOIndex(index, d))
                {
                    if (zoneGroup is null)
                        zoneGroup = d.IOConfigItems[index].ZoneGroupSet;
                    else if (zoneGroup != d.IOConfigItems[index].ZoneGroupSet)
                        return null;
                }
            }

            return zoneGroup;
        }

        private string getIOZoneSet(int index)
        {
            if (NoDeviceDetails)
                return null;

            string zoneGroup = null;

            foreach (var d in _deviceList)
            {
                if (d.DeviceType is null)
                    return null;

                if (d.IsIODevice && validIOIndex(index, d))
                {
                    var zg = d.IOConfigItems[index].ZoneGroupSet is null ? ""
                                                                         : d.IOConfigItems[index].ZoneGroupSet == 0 ? Cultures.Resources.Use_In_Special_C_And_E
                                                                         : d.IOConfigItems[index].InputOutput == IOTypes.Input ? _zones[(int)d.IOConfigItems[index].ZoneGroupSet]
                                                                         : d.IOConfigItems[index].InputOutput == IOTypes.Output ? 
                                                                           d.IsGroupedDevice ? _groups[(int)d.IOConfigItems[index].ZoneGroupSet] 
                                                                                             : _sets[(int)d.IOConfigItems[index].ZoneGroupSet] : null;
                    if (zoneGroup is null)
                        zoneGroup = zg;
                    else if (zoneGroup != zg)
                        return null;
                }
            }

            return zoneGroup;
        }

        private void setIOZoneSet(int index, string value)
        {
            if (value is not null)
                foreach (var d in _deviceList)
                    if (validIOIndex(index, d))
                        d.IOConfigItems[index].ZoneGroupSet = string.IsNullOrEmpty(value) ? null
                                                         : value == Cultures.Resources.Use_In_Special_C_And_E ? 0
                                                         : d.IOConfigItems[index].InputOutput == IOTypes.Input ? findIndexInCombo(_zones, value)
                                                         : d.IOConfigItems[index].InputOutput == IOTypes.Output ? findIndexInCombo(_sets, value) : null;
            OnPropertyChanged(nameof(IOZoneSet1));
            OnPropertyChanged(nameof(IOZoneSet2));
            OnPropertyChanged(nameof(IOZoneSet3));
            OnPropertyChanged(nameof(IOZoneSet4));
            OnPropertyChanged(nameof(IOZoneSet1IsValid));
            OnPropertyChanged(nameof(IOZoneSet2IsValid));
            OnPropertyChanged(nameof(IOZoneSet3IsValid));
            OnPropertyChanged(nameof(IOZoneSet4IsValid));
            OnPropertyChanged(nameof(DevicesHaveCommonIOZoneSet1));
            OnPropertyChanged(nameof(DevicesHaveCommonIOZoneSet2));
            OnPropertyChanged(nameof(DevicesHaveCommonIOZoneSet3));
            OnPropertyChanged(nameof(DevicesHaveCommonIOZoneSet4));
        }

        private string getIODescription(int index)
        {
            if (NoDeviceDetails)
                return "";

            string result = "";
            foreach (var d in _deviceList)
            {
                if (d.DeviceType is null)
                    continue;

                if (d.IsIODevice && validIOIndex(index, d))
                {
                    var name = index == 0 ? d.DeviceName : GetDeviceName?.Invoke(d.IOConfigItems[index].NameIndex);

                    if (result == "")
                        result = name;
                    else if (result != name)
                        return "";
                }
            }
            return result;
        }

        private bool setIODescription(int ioIndex, string value)
        {
            //if (value is not null)
            {
                if (!checkIONameMemoryLimit(ioIndex, value))
                    return false;

                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    d.IOConfigItems[ioIndex].NameIndex = _data.CurrentPanel.DeviceNamesConfig.Update(d.IOConfigItems[ioIndex].NameIndex, value);
                    d.RefreshView();
                }

                OnPropertyChanged(nameof(DeviceName));
                OnPropertyChanged(nameof(DeviceNameIsValid));
                OnPropertyChanged(nameof(IODescription1));
                OnPropertyChanged(nameof(IODescription2));
                OnPropertyChanged(nameof(IODescription3));
                OnPropertyChanged(nameof(IODescription4));
                OnPropertyChanged(nameof(IODescription1IsValid));
                OnPropertyChanged(nameof(IODescription2IsValid));
                OnPropertyChanged(nameof(IODescription3IsValid));
                OnPropertyChanged(nameof(IODescription4IsValid));
                OnPropertyChanged(nameof(DevicesHaveCommonIODescription1));
                OnPropertyChanged(nameof(DevicesHaveCommonIODescription2));
                OnPropertyChanged(nameof(DevicesHaveCommonIODescription3));
                OnPropertyChanged(nameof(DevicesHaveCommonIODescription4));
            }

            return true;
        }


        private void setIODescription(System.Windows.Controls.TextBox textBox)
        {
        }

        private bool validIOIndex(int index, DeviceItemViewModel device) => index >= 0 && index < (device.IOConfigItems?.Count??0);
        #endregion


        private bool checkNameMemoryLimit(string value)
        {
            if (string.IsNullOrEmpty(value))
                return true;

            int bytesNeeded = 0;

            foreach (var d in _deviceList)
            {
                var oldName = GetDeviceName?.Invoke(d.DeviceNameIndex);
                bytesNeeded += value.Length - oldName.Length;
                if (string.IsNullOrEmpty(oldName))
                    bytesNeeded++;
            }

            OnPropertyChanged(nameof(DebugNameBytesUsed));
            if (DebugNameBytesRemaining - bytesNeeded > 0)
                return true;

            CTecMessageBox.ShowOKError(Cultures.Resources.Error_Name_Limit_Exceeded, Cultures.Resources.Device_Name);
            return false;
        }

        private bool checkIONameMemoryLimit(int ioIndex, string value)
        {
            if (string.IsNullOrEmpty(value))
                return true;

            int bytesNeeded = 0;

            foreach (var d in _deviceList)
            {
                var oldName = GetDeviceName?.Invoke(d.IOConfigItems[ioIndex].NameIndex);
                bytesNeeded += value.Length - oldName.Length;
                if (string.IsNullOrEmpty(oldName))
                    bytesNeeded++;
            }

            if (DebugNameBytesRemaining - bytesNeeded > 0)
                return true;

            CTecMessageBox.ShowOKError(Cultures.Resources.Error_Name_Limit_Exceeded, Cultures.Resources.Device_Name);
            return false;
        }


        public bool? IsGroupedDevice
        {
            get
            {
                if (NoDeviceDetails)
                    return null;

                bool? result = null;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (result is null)
                        result = d.IsGroupedDevice;
                    else if (d.IsGroupedDevice != result)
                        return null;
                }
                return result;
            }
        }

        public bool? IsZonedDevice
        {
            get
            {
                if (NoDeviceDetails)
                    return null;

                bool? result = null;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (result is null)
                        result = d.IsZonalDevice;
                    else if (d.IsZonalDevice != result)
                        return null;
                }
                return result;
            }
        }

        public bool? IsZoneAndGroupDevice => IsZonedDevice == true && IsGroupedDevice == true;

        public bool? IsSetDevice
        {
            get
            {
                if (NoDeviceDetails)
                    return null;

                bool? result = null;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (result is null)
                        result = d.IsSetDevice;
                    else if (d.IsSetDevice != result)
                        return null;
                }
                return result;
            }
        }

        public bool? IsIODevice
        {
            get
            {
                if (NoDeviceDetails)
                    return null;

                bool? result = null;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (!d.IsIODevice)
                        return false;

                    if (result is null)
                        result = d.IsIODevice;
                    else if (d.IsIODevice != result)
                        return null;
                }

                if (result == true && _deviceList.Count > 1)
                    return devicesAreNotMixedWithHush;
                return result;
            }
        }


        public bool? IOOutputIsGrouped
        {
            get
            {
                if (NoDeviceDetails)
                    return null;

                bool? result = null;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (result is null)
                        result = d.IOOutputIsGrouped;
                    else if (d.IOOutputIsGrouped != result)
                        return null;
                }
                return result;
            }
        }

        public bool? IsSensitivityDevice
        {
            get
            {
                if (NoDeviceDetails)
                    return null;

                bool? result = null;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (!d.IsSensitivityDevice)
                        return false;

                    if (result is null)
                        result = d.IsSensitivityDevice;
                    else if (d.IsSensitivityDevice != result)
                        return null;
                }

                if (result == true && _deviceList.Count > 1)
                    return devicesAreNotMixedWithHush;
                return result;
            }
        }

        public bool? IsSensitivityHighDevice
        {
            get
            {
                if (NoDeviceDetails)
                    return null;

                bool? result = null;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (!d.IsSensitivityHighDevice)
                        return false;

                    if (result is null)
                        result = d.IsSensitivityHighDevice;
                    else if (d.IsSensitivityHighDevice != result)
                        return null;
                }

                if (result == true && _deviceList.Count > 1)
                    return devicesAreNotMixedWithHush;
                return result;
            }
        }

        public bool? IsVolumeDevice
        {
            get
            {
                if (NoDeviceDetails)
                    return null;

                bool? result = null;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (!d.IsVolumeDevice)
                        return false;

                    if (result is null)
                        result = d.IsVolumeDevice;
                    else if (d.IsVolumeDevice != result)
                        return null;
                }

                if (result == true && _deviceList.Count > 1)
                    return devicesAreNotMixedWithHush;
                return result;
            }
        }

        public bool? IsModeDevice
        {
            get
            {
                if (NoDeviceDetails)
                    return null;

                bool? result = null;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (!d.IsModeDevice)
                        return false;

                    if (result is null)
                        result = d.IsModeDevice;
                    else if (d.IsModeDevice != result)
                        return null;
                }
                return result;
            }
        }




        #region commonalities
        public bool DevicesHaveCommonDeviceType
        {
            get
            {
                if (_deviceList.Count < 2)
                    return true;

                if (DeviceTypesAreAllNull)
                    return true;

                int? t = null;
                int dCount = 0;
                foreach (var d in _deviceList)
                {
                    if (t is not null && t != d.DeviceType)
                        return false;
                    t = d.DeviceType;
                    dCount++;
                }
                return dCount < 2 || t >= 0;
            }
        }

        public bool DevicesHaveDiffentTypesOrContainNulls => !DevicesHaveCommonDeviceType || DeviceList.Count > 1 && DeviceListContainsNullDevices;

        public bool DevicesHaveCommonZGSType
        {
            get
            {
                if (_deviceList.Count == 1)
                    return DeviceType is not null;

                if (IsGroupedDevice == true ^ IsZonedDevice == true)
                    return true;
                return false;
            }
        }

        public bool DevicesHaveCommonZone
        {
            get
            {
                if (_deviceList.Count < 2)
                    return true;

                int dCount = 0;

                if (IsZonedDevice == true)
                {
                    int? z = null;
                    foreach (var d in _deviceList)
                    {
                        if (d.DeviceType is null)
                            continue;

                        if (z is not null && z != d.Zone)
                            return false;
                        z = d.Zone;
                        dCount++;
                    }
                    return dCount < 2 || z >= 0;
                }

                return true;
            }
        }

        public bool DevicesHaveCommonGroup
        {
            get
            {
                if (_deviceList.Count < 2)
                    return true;

                int dCount = 0;

                if (IsGroupedDevice == true)
                {
                    int? g = null;
                    foreach (var d in _deviceList)
                    {
                        if (d.DeviceType is null)
                            continue;

                        if (g is not null && g != d.Group)
                            return false;
                        g = d.Group;
                        dCount++;
                    }
                    return dCount < 2 || g >= 0;
                }

                return true;
            }
        }

        public bool DevicesHaveCommonDeviceName
        {
            get
            {
                if (NoDeviceDetails)
                    return true;

                if (_deviceList.Count < 2)
                    return true;

                string name = null;
                int dCount = 0;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (name is not null && name != d.DeviceName)
                        return false;
                    name = d.DeviceName;
                    dCount++;
                }
                return dCount < 2 || name is not null;
            }
        }

        public bool DevicesHaveCommonAncillaryBaseSounderGroup
        {
            get
            {
                if (NoDeviceDetails)
                    return true;

                if (_deviceList.Count < 2)
                    return true;

                if (IsSensitivityDevice != true)
                    return true;

                int? absg = _noBaseSounderGroup;
                bool? canHave = null;
                int dCount = 0;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (canHave is not null && canHave == false && canHave != d.CanHaveAncillaryBaseSounder || absg != _noBaseSounderGroup && absg != d.AncillaryBaseSounderGroup)
                        return false;
                    if ((canHave = d.CanHaveAncillaryBaseSounder) == true)
                        absg = d.AncillaryBaseSounderGroup;
                    dCount++;
                }
                return dCount < 2 || canHave is not null && absg != _noBaseSounderGroup;
            }
        }

        public bool DevicesHaveCommonHasAncillaryBaseSounder
        {
            get
            {
                if (NoDeviceDetails)
                    return true;

                if (_deviceList.Count < 2)
                    return true;

                bool? canHave = null;
                int dCount = 0;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;
                    
                    if (canHave is not null && canHave != d.CanHaveAncillaryBaseSounder)
                        return false;
                    canHave = d.CanHaveAncillaryBaseSounder;
                    dCount++;
                }
                return dCount < 2 || canHave is not null;
            }
        }

        public bool DevicesHaveCommonDaySensitivity
        {
            get
            {
                if (NoDeviceDetails)
                    return true;

                if (_deviceList.Count < 2)
                    return true;

                if (IsSensitivityDevice == false)
                    return true;
                
                if (IsSensitivityHighDevice == null)
                    return true;
                
                int? sens = null;
                int dCount = 0;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (sens is not null && sens != d.DaySensitivity)
                        return false;
                    sens = d.DaySensitivity;
                    dCount++;
                }
                return dCount < 2 || sens is not null;
            }
        }

        public bool DevicesHaveCommonNightSensitivity
        {
            get
            {
                if (NoDeviceDetails)
                    return true;

                if (_deviceList.Count < 2)
                    return true;

                if (IsSensitivityDevice == false)
                    return true;
                
                if (IsSensitivityHighDevice == null)
                    return true;
                
                int? sens = null;
                int dCount = 0;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (sens is not null && sens != d.NightSensitivity)
                        return false;
                    sens = d.NightSensitivity;
                    dCount++;
                }
                return dCount < 2 || sens is not null;
            }
        }

        public bool DevicesHaveCommonDayMode
        {
            get
            {
                if (NoDeviceDetails)
                    return true;

                if (_deviceList.Count < 2)
                    return true;

                if (IsModeDevice == false)
                    return true;
                
                int? mode = null;
                int dCount = 0;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (mode is not null && mode != d.DayMode)
                        return false;
                    mode = d.DayMode;
                    dCount++;
                }
                return dCount < 2 || mode is not null;
            }
        }

        public bool DevicesHaveCommonNightMode
        {
            get
            {
                if (NoDeviceDetails)
                    return true;

                if (_deviceList.Count < 2)
                    return true;

                if (IsModeDevice == false)
                    return true;
                
                int? mode = null;
                int dCount = 0;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (mode is not null && mode != d.NightMode)
                        return false;
                    mode = d.NightMode;
                    dCount++;
                }
                return dCount < 2 || mode is not null;
            }
        }

        public bool DevicesHaveCommonRemoteLEDEnabled
        {
            get
            {
                if (NoDeviceDetails)
                    return true;

                if (_deviceList.Count < 2)
                    return true;

                if (IsSensitivityDevice == false)
                    return true;

                bool? led = null;
                int dCount = 0;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (led is not null && led != d.RemoteLEDEnabled)
                        return false;
                    led = d.RemoteLEDEnabled;
                    dCount++;
                }
                return dCount < 2 || led is not null;
            }
        }


        public bool DevicesHaveCommonDayVolume
        {
            get
            {
                if (NoDeviceDetails)
                    return true;

                if (_deviceList.Count < 2)
                    return true;

                if (IsVolumeDevice == false)
                    return true;
                
                int? vol = null;
                int dCount = 0;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (vol is not null && vol != d.DayVolume)
                        return false;
                    vol = d.DayVolume;
                    dCount++;
                }
                return dCount < 2 || vol is not null;
            }
        }

        public bool DevicesHaveCommonNightVolume
        {
            get
            {
                if (NoDeviceDetails)
                    return true;

                if (_deviceList.Count < 2)
                    return true;

                if (IsVolumeDevice == false)
                    return true;
                
                int? vol = null;
                int dCount = 0;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    if (vol is not null && vol != d.NightVolume)
                        return false;
                    vol = d.NightVolume;
                    dCount++;
                }
                return dCount < 2 || vol is not null;
            }
        }

        public bool DevicesHaveCommonIOInputOutput1 => devicesHaveCommonIOInputOutput(0);
        public bool DevicesHaveCommonIOInputOutput2 => devicesHaveCommonIOInputOutput(1);
        public bool DevicesHaveCommonIOInputOutput3 => devicesHaveCommonIOInputOutput(2);
        public bool DevicesHaveCommonIOInputOutput4 => devicesHaveCommonIOInputOutput(3);
        public bool DevicesHaveCommonIOChannel1 => devicesHaveCommonIOChannel(0);
        public bool DevicesHaveCommonIOChannel2 => devicesHaveCommonIOChannel(1);
        public bool DevicesHaveCommonIOChannel3 => devicesHaveCommonIOChannel(2);
        public bool DevicesHaveCommonIOChannel4 => devicesHaveCommonIOChannel(3);
        public bool DevicesHaveCommonIOZoneSet1 => devicesHaveCommonIOZoneSet(0);
        public bool DevicesHaveCommonIOZoneSet2 => devicesHaveCommonIOZoneSet(1);
        public bool DevicesHaveCommonIOZoneSet3 => devicesHaveCommonIOZoneSet(2);
        public bool DevicesHaveCommonIOZoneSet4 => devicesHaveCommonIOZoneSet(3);
        public bool DevicesHaveCommonIODescription1 => devicesHaveCommonIODescription(0);
        public bool DevicesHaveCommonIODescription2 => devicesHaveCommonIODescription(1);
        public bool DevicesHaveCommonIODescription3 => devicesHaveCommonIODescription(2);
        public bool DevicesHaveCommonIODescription4 => devicesHaveCommonIODescription(3);

        private bool devicesHaveCommonIOInputOutputs => devicesHaveCommonIOInputOutput(0) && devicesHaveCommonIOInputOutput(1) && devicesHaveCommonIOInputOutput(2) && devicesHaveCommonIOInputOutput(3);
        private bool devicesHaveCommonIOChannels     => devicesHaveCommonIOChannel(0) && devicesHaveCommonIOChannel(1) && devicesHaveCommonIOChannel(2) && devicesHaveCommonIOChannel(3);
        private bool devicesHaveCommonIOZoneSets     => devicesHaveCommonIOZoneSet(0) && devicesHaveCommonIOZoneSet(1) && devicesHaveCommonIOZoneSet(2) && devicesHaveCommonIOZoneSet(3);
        private bool devicesHaveCommonIODescriptions => devicesHaveCommonIODescription(0) && devicesHaveCommonIODescription(1) && devicesHaveCommonIODescription(2) && devicesHaveCommonIODescription(3);

        private bool devicesHaveCommonIOInputOutput(int index)
        {
            if (NoDeviceDetails)
                return true;

            if (_deviceList.Count < 2)
                return true;

            if (IsIODevice != true)
                return true;

            IOTypes? io = null;
            int dCount = 0;
            foreach (var d in _deviceList)
            {
                if (!d.IsIODevice)
                    return true;

                if (d.DeviceType is null)
                    continue;

                if (io is not null && io != d.IOConfigItems[index].InputOutput)
                    return false;
                    
                io = d.IOConfigItems[index].InputOutput;
                dCount++;
            }
            return dCount < 2 || io != null;
        }

        private bool devicesHaveCommonIOChannel(int index)
        {
            if (NoDeviceDetails)
                return true;

            if (_deviceList.Count < 2)
                return true;

            if (IsIODevice != true)
                return true;

            int? channel = null;
            int dCount = 0;
            foreach (var d in _deviceList)
            {
                if (!d.IsIODevice)
                    return true;

                if (d.DeviceType is null)
                    continue;

                if (channel is not null && channel != d.IOConfigItems[index].Channel)
                    return false;
                    
                channel = d.IOConfigItems[index].Channel;
                dCount++;
            }
            return dCount < 2 || channel != null;
        }

        private bool devicesHaveCommonIOZoneSet(int index)
        {
            if (NoDeviceDetails)
                return true;

            if (_deviceList.Count < 2)
                return true;

            if (IsIODevice != true)
                return true;

            int? zoneSet = null;
            int dCount = 0;
            foreach (var d in _deviceList)
            {
                if (!d.IsIODevice)
                    return true;

                if (d.DeviceType is null)
                    continue;

                if (zoneSet is not null && zoneSet != d.IOConfigItems[index].ZoneGroupSet)
                    return false;
                    
                zoneSet = d.IOConfigItems[index].ZoneGroupSet;
                dCount++;
            }
            return dCount < 2 || zoneSet != null;
        }

        private bool devicesHaveCommonIODescription(int index)
        {
            if (NoDeviceDetails)
                return true;

            if (_deviceList.Count < 2)
                return true;

            if (IsIODevice != true)
                return true;

            int? desc = null;
            int dCount = 0;

            foreach (var d in _deviceList)
            {
                if (!d.IsIODevice)
                    return true;

                if (d.DeviceType is null)
                    continue;

                if (desc is not null && desc != d.IOConfigItems[index].NameIndex)
                    return false;

                desc = d.IOConfigItems[index].NameIndex;
                dCount++;
            }
            return dCount < 2 || desc != null;
        }

        public bool IndicateMultipleValues  => !DevicesHaveCommonZone 
                                            || !DevicesHaveCommonGroup 
                                            || !DevicesHaveCommonDeviceName 
                                            || !DevicesHaveCommonDayVolume
                                            || !DevicesHaveCommonNightVolume
                                            || !DevicesHaveCommonDaySensitivity
                                            || !DevicesHaveCommonNightSensitivity
                                            || !DevicesHaveCommonDayMode
                                            || !DevicesHaveCommonNightMode
                                            || !DevicesHaveCommonRemoteLEDEnabled 
                                            || !DevicesHaveCommonAncillaryBaseSounderGroup 
                                            || !devicesHaveCommonIOInputOutputs 
                                            || !devicesHaveCommonIOChannels 
                                            || !devicesHaveCommonIOZoneSets 
                                            || !devicesHaveCommonIODescriptions;


        /// <summary>
        /// Subaddresses on HS2 have defined meanings (Sounder, Fire2, etc) rather than just numbers, so don't share common I/O config properties.
        /// </summary>
        private bool devicesAreNotMixedWithHush
        {
            get
            {
                bool? isHS2 = null;
                foreach (var d in _deviceList)
                {
                    if (d.DeviceType is null)
                        continue;

                    bool tmp = deviceIsHush2(d.DeviceType);

                    if (isHS2 is not null && isHS2 != tmp)
                        return false;
                    isHS2 = tmp;
                }
                return isHS2 is not null;
            }
        }
        #endregion


        private bool deviceIsHush2(int? deviceType) => DeviceTypes.CurrentProtocolIsXfpCast && (deviceType??0) == (int)XfpCastDeviceTypeIds.HS2;


        public List<string> SubaddressNames => (IsIODevice??false) && deviceIsHush2(DeviceType) ? new() { Cultures.Resources.Subaddress_Hush_0, 
                                                                                              Cultures.Resources.Subaddress_Hush_1, 
                                                                                              Cultures.Resources.Subaddress_Hush_2, 
                                                                                              Cultures.Resources.Subaddress_Hush_3 }
                                                                                    : new() { "0", "1", "2", "3" };


        public bool DeviceListContainsNullDevices
        {
            get
            {
                foreach (var _ in from d in _deviceList
                                  where d.DeviceType is null
                                  select new { })
                    return true;

                return false;
            }
        }

        public bool DeviceTypesAreAllNull
        {
            get
            {
                foreach (var _ in from d in _deviceList
                                  where d is not null && d.DeviceType is not null
                                  select new { })
                    return false;

                return true;
            }
        }

        public bool DeviceTypeIsValid       => DeviceType is null || DeviceTypes.IsValidDeviceType(DeviceType, DeviceTypes.CurrentProtocolType);
        public bool DeviceTypeIsNonSpecific => NoDeviceType || !DevicesHaveCommonDeviceType || DeviceListContainsNullDevices;
        public bool ZoneIsValid             => (!IsZonedDevice??false)   || _zoneIndex >= 0 && _zoneIndex <= ZoneConfigData.NumZones;
        public bool GroupIsValid            => (!IsGroupedDevice??false) || _groupIndex >= 0 && _groupIndex <= GroupConfigData.NumSounderGroups;
        public bool DeviceNameIsValid       => NoDeviceType || DeviceName is null || DeviceName.Length <= MaxNameLength;
        public bool AncillaryBaseSounderGroupIsValid => _ancillaryBaseSounderGroupIndex is null || (_ancillaryBaseSounderGroupIndex >= 0 && _ancillaryBaseSounderGroupIndex < GroupConfigData.NumSounderGroups);
        public bool DaySensitivityIsValid   => sensitivityIsValid(DaySensitivity);
        public bool NightSensitivityIsValid => sensitivityIsValid(NightSensitivity);
        public bool DayVolumeIsValid        => volumeIsValid(findIndexInCombo(Volumes, DayVolume));
        public bool NightVolumeIsValid      => volumeIsValid(findIndexInCombo(Volumes, NightVolume));
        public bool DayModeIsInvalid        => selectedDayModesInvalid(DayMode);
        public bool NightModeIsInvalid      => selectedNightModesInvalid(NightMode);
        public bool IOInputOutput1IsValid   => ioInputOutputIsValid(0);
        public bool IOInputOutput2IsValid   => ioInputOutputIsValid(1);
        public bool IOInputOutput3IsValid   => ioInputOutputIsValid(2);
        public bool IOInputOutput4IsValid   => ioInputOutputIsValid(3);
        public bool IOChannel1IsValid       => ioChannelIsValid(0);
        public bool IOChannel2IsValid       => ioChannelIsValid(1);
        public bool IOChannel3IsValid       => ioChannelIsValid(2);
        public bool IOChannel4IsValid       => ioChannelIsValid(3);
        public bool IOZoneSet1IsValid       => ioZoneSetIsValid(0);
        public bool IOZoneSet2IsValid       => ioZoneSetIsValid(1);
        public bool IOZoneSet3IsValid       => ioZoneSetIsValid(2);
        public bool IOZoneSet4IsValid       => ioZoneSetIsValid(3);
        public bool IODescription1IsValid   => ioDescriptionIsValid(0);
        public bool IODescription2IsValid   => ioDescriptionIsValid(1);
        public bool IODescription3IsValid   => ioDescriptionIsValid(2);
        public bool IODescription4IsValid   => ioDescriptionIsValid(3);


        private bool volumeIsValid(int? value)            => IsVolumeDevice == false || value >= DeviceConfigData.MinVolume - 1 && value <= DeviceConfigData.MaxVolume - 1;
        private bool sensitivityIsValid(int? value)       => IsSensitivityDevice == false || value >= MinSensitivity && value <= MaxSensitivity;

        private bool selectedDayModesInvalid(ModeSettingOption value)
        {
            if (IsModeDevice == false)
                return false;
            
            if (DeviceList.Count == 1)
                return !DeviceTypes.ModeIsValid(DeviceList[0].DeviceType, DeviceList[0].DayMode);

            int valid = (from d in DeviceList
                         where DeviceTypes.ModeIsValid(d.DeviceType, d.DayMode)
                         select d).Count();

            return valid == 0;
        }

        private bool selectedNightModesInvalid(ModeSettingOption value)
        {
            if (IsModeDevice == false)
                return true;
            
            if (DeviceList.Count == 1)
                return !DeviceTypes.ModeIsValid(DeviceList[0].DeviceType, DeviceList[0].NightMode);

            int valid = (from d in DeviceList
                         where DeviceTypes.ModeIsValid(d.DeviceType, d.NightMode)
                         select d).Count();

            return valid == 0;
        }
        
        private bool ioInputOutputIsValid(int index)
        {
            if (getInputOutput(index) is IOTypes io)
                return  (int)io >= (DeviceList.Count > 0 ? -1 : 0) && (int)io < Enum.GetNames(typeof(IOTypes)).Length;
            return true;
        }

        private bool ioChannelIsValid(int index)
        {
            if (getIOChannel(index) is int channel)
                return (int)channel >= (DeviceList.Count > 0 ? -1 : 0) && channel <= (getInputOutput(index) == IOTypes.Input ? InputChannels.Count : OutputChannels.Count);
            return true;
        }

        private bool ioZoneSetIsValid(int index)
        {
            if (getIOZoneSetIndex(index) is int zs)
                return (int)zs >= (DeviceList.Count > 0 ? -1 : 0) && zs < (getInputOutput(index) == IOTypes.Input ? Zones.Count : Sets.Count);
            return true;
        }

        private bool ioDescriptionIsValid(int index)
        {
            var desc = getIODescription(index);
            return desc is null || desc.Length <= MaxNameLength;
        }
        #endregion


        #region device editing
        public delegate void ShowFittedDevicesChanged(bool show);
        public ShowFittedDevicesChanged OnShowFittedDeviceChange;


        public void ChangeDeviceType() => ChangeDeviceType(DeviceSelectorDeviceType);

        /// <summary>
        /// Set, change or delete device type
        /// </summary>
        public void ChangeDeviceType(int? deviceType)
        {
            if (deviceType is null)
                return;

            if (countDeviceTypeChanges(deviceType, false) > 0
             && !CheckChangesAreAllowed.Invoke())
                return;

            changeDevices(deviceType);
        }


        /// <summary>
        /// Count the number of devices that would have their type 
        /// changed according to the current DeviceSelector value.<br/>
        /// Network Controller is not counted, since it cannot be changed.
        /// </summary>
        /// <returns></returns>
        private int countDeviceTypeChanges(int? deviceType, bool ignoreNulls)
        {
            var typeChangeCount = 0;
            foreach (var d in _deviceList)
                if (((!ignoreNulls && d.DeviceType is null) || d.DeviceType == 0 || DeviceTypes.IsValidDeviceType(d.DeviceType, DeviceTypes.CurrentProtocolType))
                 && d.DeviceType != DeviceSelectorDeviceType)
                    typeChangeCount++;
            return typeChangeCount;
        }


        /// <summary>
        /// Change device
        /// </summary>
        private void changeDevices(int? deviceType)
        {
            if (deviceType == DeviceType)
                return;

            //count the number of devices that would have their type changed
            var numToChange = countDeviceTypeChanges(deviceType, true);
            if (numToChange > 0)
            {
                string msg;
                if (numToChange != 1)
                    msg = string.Format(Cultures.Resources.Query_Change_Device_Types, numToChange, DeviceTypes.DeviceTypeName(deviceType, DeviceTypes.CurrentProtocolType));
                else if (DeviceList.Count > 1)
                    msg = string.Format("Change 1 device to {0}", DeviceTypes.DeviceTypeName(deviceType, DeviceTypes.CurrentProtocolType));
                else
                    msg = string.Format(Cultures.Resources.Query_Change_Device_Type, DeviceTypes.DeviceTypeName(deviceType, DeviceTypes.CurrentProtocolType));

                if (CTecMessageBox.ShowYesNoWarn(msg, Cultures.Resources.Nav_Device_Details) != MessageBoxResult.Yes)
                    return;
            }

            foreach (var d in _deviceList)
            {
                if (d.DeviceType != DeviceSelectorDeviceType.Value)
                {
                    bool resetZGA = d.IsGroupedDevice != DeviceTypes.IsGroupedDevice(deviceType, DeviceTypes.CurrentProtocolType)
                                 || d.IsZonalDevice   != DeviceTypes.IsZonalDevice(deviceType, DeviceTypes.CurrentProtocolType);
                    d.DeviceType = deviceType.Value;
                    if (resetZGA)
                        d.ZoneIndex = 1;

                    if (!d.DayModeIsValid)
                        d.DayMode = DeviceTypes.MinValidMode(d.DeviceType);
                    if (!d.NightModeIsValid)
                        d.NightMode = DeviceTypes.MinValidMode(d.DeviceType);

                    if (d.IsSensitivityHighDevice)
                        d.DaySensitivity = d.NightSensitivity = DeviceConfigData.DefaultSensitivityHigh;
                    else if (d.IsSensitivityDevice)
                        d.DaySensitivity = d.NightSensitivity = DeviceConfigData.DefaultSensitivity;
                    else if (d.IsVolumeDevice)
                        d.DayVolume = d.NightVolume = DeviceConfigData.DefaultVolume;

                    d.RefreshView();
                }
            }

            RefreshView();
            updateModesList();
        }


        /// <summary>
        /// Delete devices
        /// </summary>
        public void DeleteDevices()
        {
            if (_deviceList.Count == 0)
                return;

            var numToDelete = 0;

            //check whether selected devices are non-blank devices
            foreach (var d in _deviceList)
                if (d.DeviceType is not null)
                    numToDelete++;

            if (numToDelete == 0)
                return;

            if (!CheckChangesAreAllowed.Invoke())
                return;

            string msg;

            if (numToDelete > 1)
                msg = string.Format(Cultures.Resources.Query_Delete_Devices, numToDelete);
            else if (_deviceList.Count > 1)
                msg = string.Format(Cultures.Resources.Query_Delete_1_Device);
            else
                msg = string.Format(Cultures.Resources.Query_Delete_Device);

            if (CTecMessageBox.ShowYesNoWarn(msg, Cultures.Resources.Nav_Device_Details) == MessageBoxResult.Yes)
            {
                foreach (var d in _deviceList)
                {
                    //delete devicename entries first
                    if (d.DeviceNameIndex >= 0)
                        d.DeviceName = null;
                    foreach (var io in d.IOConfigItems)
                        if (io.NameIndex >= 0)
                            setIODescription(io.Index, null);

                    d.DeviceData.DeviceType = null;
                    d.DeviceData.Zone = 0;
                    d.DeviceData.Group = 0;
                    d.DeviceData.NameIndex = 0;
                    d.DeviceData.DaySensitivity = 0;
                    d.DeviceData.DayVolume = 0;
                    d.DeviceData.NightSensitivity = 0;
                    d.DeviceData.NightVolume = 0;
                    d.DeviceData.IOConfig = new();
                    d.DeviceData.RemoteLEDEnabled = false;
                    d.DeviceData.AncillaryBaseSounderGroup = 0;
                    d.RefreshView();
                }

                RefreshView();
            }
        }
        #endregion


        #region comboboxes
        private ObservableCollection<string> _zones;
        //private ObservableCollection<string> _zoneNames;
        private ObservableCollection<string> _groups;
        private ObservableCollection<string> _sets;
        private ObservableCollection<string> _baseSounderGroups;    
        private ObservableCollection<string> _volumes;    
        private ObservableCollection<ModeSettingOption> _modeSettings = new();    
        //private ObservableCollection<string> _modes;    
        private ObservableCollection<string> _inputOutputs;
        private ObservableCollection<string> _inputChannels;
        private ObservableCollection<string> _outputChannels;

        public ObservableCollection<string> Zones             { get => _zones;             set { _zones = value; OnPropertyChanged(); } }
        public ObservableCollection<string> Groups            { get => _groups;            set { _groups = value; OnPropertyChanged(); } }
        public ObservableCollection<string> Sets              { get => _sets;              set { _sets = value; OnPropertyChanged(); } }
        public ObservableCollection<string> AncillaryBaseSounderGroups { get => _baseSounderGroups; set { _baseSounderGroups = value; OnPropertyChanged(); } }
        public ObservableCollection<string> Volumes           { get { if (_volumes is null || _volumes.Count < 2) initVolumesList(); return _volumes; }   set { _volumes = value; OnPropertyChanged(); } }
        public ObservableCollection<ModeSettingOption> Modes  { get => _modeSettings;      set { _modeSettings = value; OnPropertyChanged(); } }
        public ObservableCollection<string> InputOutputs      { get => _inputOutputs;      set { _inputOutputs = value; OnPropertyChanged(); } }
        public ObservableCollection<string> InputChannels     { get => _inputChannels;     set { _inputChannels = value; OnPropertyChanged(); } }
        public ObservableCollection<string> OutputChannels    { get => _outputChannels;    set { _outputChannels = value; OnPropertyChanged(); } }

        public bool                         EnableIOEdit     => !DeviceTypes.CurrentProtocolIsXfpCast;

        private void initLists()
        {
            if (_data is null)
                return;

            initZGSLists();
            initBaseSounderList();
            initIOLists();
            initIOChannelLists();
            initVolumesList();
            initModesList();
        }


        private void initZGSLists()
        {
            var zIdx = _zoneIndex;
            var gIdx = _groupIndex;
            
            if (_zones is null)
            {
                _zones  = new() { Cultures.Resources.Use_In_Special_C_And_E };
                for (int z = 0; z < _data.CurrentPanel.ZoneConfig.Zones.Count; z++)
                    _zones.Add(_data.CurrentPanel.ZoneConfig.Zones[z].DisplayName);
            }
            else
            {
                _zones[0] = Cultures.Resources.Use_In_Special_C_And_E;
                for (int z = 0; z < _data.CurrentPanel.ZoneConfig.Zones.Count; z++)
                    _zones[z + 1] = _data.CurrentPanel.ZoneConfig.Zones[z].DisplayName;
            }
            
            if (_groups is null)
            {
                _groups = new() { Cultures.Resources.Use_In_Special_C_And_E };
                for (int i = 1; i <= GroupConfigData.NumSounderGroups; i++)
                    _groups.Add(string.Format(Cultures.Resources.Group_x, i));
            }
            else
            {
                _groups[0] = Cultures.Resources.Use_In_Special_C_And_E;
                for (int i = 1; i <= GroupConfigData.NumSounderGroups; i++)
                    _groups[i] = string.Format(Cultures.Resources.Group_x, i);
            }

            if (_sets is null)
            {
                _sets = new() { Cultures.Resources.Use_In_Special_C_And_E };
                for (int s = 1; s <= DeviceData.NumIOSets; s++)
                    _sets.Add(string.Format(Cultures.Resources.Set_x, s));
            }
            else
            {
                _sets[0] = Cultures.Resources.Use_In_Special_C_And_E;
                for (int s = 1; s <= DeviceData.NumIOSets; s++)
                    _sets[s] = string.Format(Cultures.Resources.Set_x, s);
            }

            //ZoneGroup = "";

            OnPropertyChanged(nameof(Zones));
            OnPropertyChanged(nameof(Groups));
            OnPropertyChanged(nameof(Sets));
            _zoneIndex = zIdx;
            _groupIndex = gIdx;
            OnPropertyChanged(nameof(Zone));
        }
        
        private void initBaseSounderList()
        {
            var bsg    = AncillaryBaseSounderGroup;
            var bsgIdx = _ancillaryBaseSounderGroupIndex;

            if (_baseSounderGroups is null)
            {
                _baseSounderGroups = new() { CTecControls.Cultures.Resources.None };
                for (int g = 1; g <= GroupConfigData.NumSounderGroups; g++)
                    _baseSounderGroups.Add(string.Format(Cultures.Resources.Group_x, g));
            }
            else
            {
                _baseSounderGroups[0] = CTecControls.Cultures.Resources.None;
                for (int g = 1; g <= GroupConfigData.NumSounderGroups; g++)
                    _baseSounderGroups[g] = string.Format(Cultures.Resources.Group_x, g);
            }
            
            OnPropertyChanged(nameof(AncillaryBaseSounderGroups));
            OnPropertyChanged(nameof(AncillaryBaseSounderGroup));
            //OnPropertyChanged(nameof(AncillaryBaseSounderGroupIndex));
            AncillaryBaseSounderGroup      = bsg;
            _ancillaryBaseSounderGroupIndex = bsgIdx;
        }
        
        private void initIOLists()
        {
            var io1Idx = IOInputOutput1;
            var io2Idx = IOInputOutput2;
            var io3Idx = IOInputOutput3;
            var io4Idx = IOInputOutput4;

            if (_inputOutputs is null)
            {
                _inputOutputs = new()
                {
                    Cultures.Resources.N_A,
                    Cultures.Resources.Input,
                    Cultures.Resources.Output,
                };
            }
            else
            {
                _inputOutputs[0] = Cultures.Resources.N_A;
                _inputOutputs[1] = Cultures.Resources.Input;
                _inputOutputs[2] = Cultures.Resources.Output;
            }

            OnPropertyChanged(nameof(InputOutputs));

            IOInputOutput1 = io1Idx;
            IOInputOutput2 = io2Idx;
            IOInputOutput3 = io3Idx;
            IOInputOutput4 = io4Idx;
        }

        private void initIOChannelLists()
        {
            var input1Idx = IOInputChannel1;
            var input2Idx = IOInputChannel2;
            var input3Idx = IOInputChannel3;
            var input4Idx = IOInputChannel4;
            var output1Idx = IOOutputChannel1;
            var output2Idx = IOOutputChannel2;
            var output3Idx = IOOutputChannel3;
            var output4Idx = IOOutputChannel4;

            _inputChannels = new();
            _outputChannels = new();
            for (int c = 0; c < IOSettingData.NumInputChannels; c++)
                _inputChannels.Add((c + 1).ToString());
            for (int c = 0; c < IOSettingData.NumOutputChannels; c++)
                _outputChannels.Add((c + 1).ToString());

            OnPropertyChanged(nameof(InputChannels));
            OnPropertyChanged(nameof(OutputChannels));

            IOInputChannel1 = input1Idx;
            IOInputChannel2 = input2Idx;
            IOInputChannel3 = input3Idx;
            IOInputChannel4 = input4Idx;
            IOOutputChannel1 = output1Idx;
            IOOutputChannel2 = output2Idx;
            IOOutputChannel3 = output3Idx;
            IOOutputChannel4 = output4Idx;
        }

        private void initVolumesList()
        {
            //var dayVolIdx = _dayVolumeIndex;
            //var nightVolIdx = _nightVolumeIndex;

            if (_volumes is null)
            {
                _volumes = new();
                foreach (var s in DeviceTypes.VolumeSettings)
                    _volumes.Add(s);
            }
            else
            {
                while (_volumes.Count < DeviceTypes.VolumeSettings.Count)
                    _volumes.Add("");

                for (int s = 0; s < DeviceTypes.VolumeSettings.Count; s++)
                    _volumes[s] = DeviceTypes.VolumeSettings[s];

                for (int s = _volumes.Count; s > DeviceTypes.VolumeSettings.Count; s--)
                    _volumes.RemoveAt(s - 1);
            }

            OnPropertyChanged(nameof(Volumes));

            //_dayVolumeIndex = dayVolIdx;
            //_nightVolumeIndex = nightVolIdx;
        }

        private void initModesList()
        {
            var dNum = DayMode?.Number;
            var nNum = NightMode?.Number;

            Modes = new();
            foreach (var m in DeviceTypes.ModeSettings())
                Modes.Add(new(m));

            SomeModesNotAvailableForSelectedDevices = false;
            NoModesAvailableForSelectedDevices = false;

            DayMode = findModeInList(dNum);
            NightMode = findModeInList(nNum);

            updateModesList();
        }
        
        private void updateModesList()
        {
            var comp = new List<ModeSetting>(DeviceTypes.ModeSettings((from d in DeviceList select d?.DeviceType).ToList()));
 
            var excluded = 0;
            foreach (var m in Modes)
                if (!(m.IsEnabled = findIndexInCombo(comp, m.Number) > -1))
                    excluded++;

            if (NoModesAvailableForSelectedDevices = excluded >= _modeSettings.Count)
                SomeModesNotAvailableForSelectedDevices = false;
            else
                SomeModesNotAvailableForSelectedDevices = excluded > 0;

            OnPropertyChanged(nameof(Modes));
            OnPropertyChanged(nameof(DayMode));
            OnPropertyChanged(nameof(NightMode));
            OnPropertyChanged(nameof(SomeModesNotAvailableForSelectedDevices));
            OnPropertyChanged(nameof(NoModesAvailableForSelectedDevices));

            foreach (var m in Modes)
                m.RefreshView();
        }



        public bool SomeModesNotAvailableForSelectedDevices { get; set; }
        public bool NoModesAvailableForSelectedDevices { get; set; }


        private int? findIndexInCombo(ObservableCollection<string> list, string text)
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i] == text)
                    return i;
            return null;
        }

        private int findIndexInCombo(IList<ModeSetting> list, int mode)
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i].Number == mode)
                    return list[i].Index;
            return -1;
        }

        private int findIndexInCombo(ObservableCollection<ModeSettingOption> list, int mode)
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i].Number == mode)
                    return list[i].Index;
            return -1;
        }
        #endregion


        #region I/O config pop-up
        private string _errorMessage;
        public string ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(); } }

        public string PopupHeader { get => string.Format(Cultures.Resources.IO_Configuration, DeviceNum); }

        public bool SaveAvailable { get => string.IsNullOrEmpty(_errorMessage); }

        #endregion


        #region debug info
        public string DebugNamesUsed          => _data.CurrentPanel?.DeviceNamesConfig?.TotalNamesUsedText;
        public int    DebugNameBytesUsed      => _data.CurrentPanel?.DeviceNamesConfig?.BytesUsed??0;
        public int    DebugNameBytesRemaining => _data.CurrentPanel?.DeviceNamesConfig?.BytesRemaining??0;
        #endregion


        #region ConfigToolsPageViewModelBase overrides
        public override bool IsReadOnly
        {
            get => base.IsReadOnly;
            set { base.IsReadOnly = value; RefreshView(); }
        }
        #endregion


        #region IAppViewModel implementation
        public void SetCulture(CultureInfo culture) 
        {
            initLists();
            RefreshView();
        }
        #endregion


        #region IPanelToolsViewModel implementation
        public void PopulateView(XfpData data)
        {
            _data = data;
            initLists();
            RefreshView();
        }

        public void RefreshView()
        {
            if (_data is null)
                return;

             //_dayVolumeIndex   = findIndexInCombo(Volumes, DayVolume) ?? -1;
             //_nightVolumeIndex = findIndexInCombo(Volumes, NightVolume) ?? -1;

            OnPropertyChanged(nameof(CurrentProtocolIsXfpCast));
            OnPropertyChanged(nameof(SoundersCanHaveRemoteDevices));
            OnPropertyChanged(nameof(ManualCallpoints));
            OnPropertyChanged(nameof(InterfaceUnits));
            OnPropertyChanged(nameof(Detectors));
            OnPropertyChanged(nameof(DeviceList));
            OnPropertyChanged(nameof(DeviceNum));
            OnPropertyChanged(nameof(DeviceType));
            OnPropertyChanged(nameof(DeviceTypeName));
            OnPropertyChanged(nameof(NoDeviceType));
            OnPropertyChanged(nameof(NoDeviceDetails));
            OnPropertyChanged(nameof(IsGroupedDevice));
            OnPropertyChanged(nameof(IsZonedDevice));
            OnPropertyChanged(nameof(IsZoneAndGroupDevice));
            OnPropertyChanged(nameof(IsSetDevice));
            OnPropertyChanged(nameof(IsIODevice));
            OnPropertyChanged(nameof(IsSensitivityDevice));
            OnPropertyChanged(nameof(IsSensitivityHighDevice));
            OnPropertyChanged(nameof(IsVolumeDevice));
            OnPropertyChanged(nameof(IsModeDevice));
            OnPropertyChanged(nameof(DaySensitivity));
            OnPropertyChanged(nameof(NightSensitivity));
            OnPropertyChanged(nameof(DayMode));
            OnPropertyChanged(nameof(NightMode));
            OnPropertyChanged(nameof(SensitivitiesHaveSameRange));
            OnPropertyChanged(nameof(DaySensitivityIsValid));
            OnPropertyChanged(nameof(NightSensitivityIsValid));
            OnPropertyChanged(nameof(DayModeIsInvalid));
            OnPropertyChanged(nameof(NightModeIsInvalid));
            OnPropertyChanged(nameof(DayVolume));
            OnPropertyChanged(nameof(NightVolume));
            OnPropertyChanged(nameof(DayVolumeIsValid));
            OnPropertyChanged(nameof(NightVolumeIsValid));
            OnPropertyChanged(nameof(MinSensitivity));
            OnPropertyChanged(nameof(MaxSensitivity));
            OnPropertyChanged(nameof(MinVolume));
            OnPropertyChanged(nameof(MaxVolume));
            OnPropertyChanged(nameof(SubaddressNames));
            OnPropertyChanged(nameof(ZoneGroupDesc));
            OnPropertyChanged(nameof(Zone));
            OnPropertyChanged(nameof(Group));
            OnPropertyChanged(nameof(DeviceName));
            OnPropertyChanged(nameof(RemoteLEDEnabled));
            OnPropertyChanged(nameof(AncillaryBaseSounderGroup));
            OnPropertyChanged(nameof(DevicesHaveCommonDeviceType));
            OnPropertyChanged(nameof(DevicesHaveCommonZGSType));
            OnPropertyChanged(nameof(DevicesHaveCommonZone));
            OnPropertyChanged(nameof(DevicesHaveCommonGroup));
            OnPropertyChanged(nameof(DevicesHaveCommonDeviceName));
            OnPropertyChanged(nameof(DevicesHaveCommonRemoteLEDEnabled));
            OnPropertyChanged(nameof(DevicesHaveCommonHasAncillaryBaseSounder));
            OnPropertyChanged(nameof(DevicesHaveCommonAncillaryBaseSounderGroup));
            OnPropertyChanged(nameof(DevicesHaveCommonDaySensitivity));
            OnPropertyChanged(nameof(DevicesHaveCommonNightSensitivity));
            OnPropertyChanged(nameof(DevicesHaveCommonDayVolume));
            OnPropertyChanged(nameof(DevicesHaveCommonNightVolume));
            OnPropertyChanged(nameof(DevicesHaveCommonDayMode));
            OnPropertyChanged(nameof(DevicesHaveCommonNightMode));
            OnPropertyChanged(nameof(DeviceListContainsNullDevices));
            OnPropertyChanged(nameof(DeviceTypesAreAllNull));
            OnPropertyChanged(nameof(DevicesHaveDiffentTypesOrContainNulls));
            OnPropertyChanged(nameof(DeviceTypeIsValid));
            OnPropertyChanged(nameof(ZoneIsValid));
            OnPropertyChanged(nameof(GroupIsValid));
            OnPropertyChanged(nameof(AncillaryBaseSounderGroupIsValid));
            OnPropertyChanged(nameof(DeviceTypeIsNonSpecific));
            OnPropertyChanged(nameof(IndicateMultipleValues));
            OnPropertyChanged(nameof(NoDeviceType));
            OnPropertyChanged(nameof(NoDeviceDetails));
            OnPropertyChanged(nameof(IsReadOnly));
            OnPropertyChanged(nameof(Zones));
            OnPropertyChanged(nameof(Groups));
            OnPropertyChanged(nameof(AncillaryBaseSounderGroups));
            OnPropertyChanged(nameof(EnableIOEdit));
            OnPropertyChanged(nameof(IOInputOutput1));
            OnPropertyChanged(nameof(IOInputOutput2));
            OnPropertyChanged(nameof(IOInputOutput3));
            OnPropertyChanged(nameof(IOInputOutput4));
            OnPropertyChanged(nameof(IOInputOutputDesc1));
            OnPropertyChanged(nameof(IOInputOutputDesc2));
            OnPropertyChanged(nameof(IOInputOutputDesc3));
            OnPropertyChanged(nameof(IOInputOutputDesc4));
            OnPropertyChanged(nameof(IOInUse1));
            OnPropertyChanged(nameof(IOInUse2));
            OnPropertyChanged(nameof(IOInUse3));
            OnPropertyChanged(nameof(IOInUse4));
            OnPropertyChanged(nameof(IOIsInput1));
            OnPropertyChanged(nameof(IOIsInput2));
            OnPropertyChanged(nameof(IOIsInput3));
            OnPropertyChanged(nameof(IOIsInput4));
            OnPropertyChanged(nameof(IOInputChannel1));
            OnPropertyChanged(nameof(IOInputChannel2));
            OnPropertyChanged(nameof(IOInputChannel3));
            OnPropertyChanged(nameof(IOInputChannel4));
            OnPropertyChanged(nameof(IOOutputChannel1));
            OnPropertyChanged(nameof(IOOutputChannel2));
            OnPropertyChanged(nameof(IOOutputChannel3));
            OnPropertyChanged(nameof(IOOutputChannel4));
            OnPropertyChanged(nameof(IOOutputIsGrouped));
            OnPropertyChanged(nameof(IOZoneSet1));
            OnPropertyChanged(nameof(IOZoneSet2));
            OnPropertyChanged(nameof(IOZoneSet3));
            OnPropertyChanged(nameof(IOZoneSet4));
            OnPropertyChanged(nameof(IODescription1));
            OnPropertyChanged(nameof(IODescription2));
            OnPropertyChanged(nameof(IODescription3));
            OnPropertyChanged(nameof(IODescription4));
            OnPropertyChanged(nameof(IOInputOutput1IsValid));
            OnPropertyChanged(nameof(IOInputOutput2IsValid));
            OnPropertyChanged(nameof(IOInputOutput3IsValid));
            OnPropertyChanged(nameof(IOInputOutput4IsValid));
            OnPropertyChanged(nameof(IOChannel1IsValid));
            OnPropertyChanged(nameof(IOChannel2IsValid));
            OnPropertyChanged(nameof(IOChannel3IsValid));
            OnPropertyChanged(nameof(IOChannel4IsValid));
            OnPropertyChanged(nameof(IOZoneSet1IsValid));
            OnPropertyChanged(nameof(IOZoneSet2IsValid));
            OnPropertyChanged(nameof(IOZoneSet3IsValid));
            OnPropertyChanged(nameof(IOZoneSet4IsValid));
            OnPropertyChanged(nameof(IOZoneSet1IsValid));
            OnPropertyChanged(nameof(IOZoneSet2IsValid));
            OnPropertyChanged(nameof(IOZoneSet3IsValid));
            OnPropertyChanged(nameof(IOZoneSet4IsValid));
            OnPropertyChanged(nameof(IODescription1IsValid));
            OnPropertyChanged(nameof(IODescription2IsValid));
            OnPropertyChanged(nameof(IODescription3IsValid));
            OnPropertyChanged(nameof(IODescription4IsValid));
            OnPropertyChanged(nameof(DevicesHaveCommonIOInputOutput1));
            OnPropertyChanged(nameof(DevicesHaveCommonIOInputOutput2));
            OnPropertyChanged(nameof(DevicesHaveCommonIOInputOutput3));
            OnPropertyChanged(nameof(DevicesHaveCommonIOInputOutput4));
            OnPropertyChanged(nameof(DevicesHaveCommonIOChannel1));
            OnPropertyChanged(nameof(DevicesHaveCommonIOChannel2));
            OnPropertyChanged(nameof(DevicesHaveCommonIOChannel3));
            OnPropertyChanged(nameof(DevicesHaveCommonIOChannel4));
            OnPropertyChanged(nameof(DevicesHaveCommonIOZoneSet1));
            OnPropertyChanged(nameof(DevicesHaveCommonIOZoneSet2));
            OnPropertyChanged(nameof(DevicesHaveCommonIOZoneSet3));
            OnPropertyChanged(nameof(DevicesHaveCommonIOZoneSet4));
            OnPropertyChanged(nameof(DevicesHaveCommonIODescription1));
            OnPropertyChanged(nameof(DevicesHaveCommonIODescription2));
            OnPropertyChanged(nameof(DevicesHaveCommonIODescription3));
            OnPropertyChanged(nameof(DevicesHaveCommonIODescription4));

            OnPropertyChanged(nameof(PopupHeader));

            updateDebugInfo();

            Validator.IsValid(Parent);
        }
        #endregion
    }
}
