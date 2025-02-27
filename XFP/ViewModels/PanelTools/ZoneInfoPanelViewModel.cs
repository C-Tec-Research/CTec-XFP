using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CTecControls.UI;
using CTecControls.UI.DeviceSelector;
using CTecControls.ViewModels;
using Xfp.DataTypes.PanelData;
using CTecDevices.Protocol;
using Xfp.UI.Interfaces;
using CTecUtil;
using System.Windows.Controls;
using Xfp.DataTypes;
using Windows.Graphics.Printing;
using Windows.UI.ViewManagement;
using System.Windows.Markup;
using Xfp.UI.Views.PanelTools;
using System.Windows.Threading;

namespace Xfp.ViewModels.PanelTools
{
    public class ZoneInfoPanelViewModel : PanelToolsPageViewModelBase, IPanelToolsViewModel
    {
        public ZoneInfoPanelViewModel(FrameworkElement parent) : base(parent)
        {
        }


        private TimeSpan _inputDelay;

        /// <summary>Input delay as shown in the Zones Page header</summary>
        public TimeSpan InputDelay { private get => _inputDelay; set { _inputDelay = value; OnPropertyChanged(nameof(DelayTotalIsValid)); } }


        private ObservableCollection<ZoneConfigItemViewModel> _zoneList = new();
        public ObservableCollection<ZoneConfigItemViewModel> ZoneList { get => _zoneList; set { _zoneList = value; RefreshView(); } }

        public bool NoItemSelected => _zoneList.Count == 0;


        public int MaxNameLength => ZoneConfigData.MaxNameLength;


        #region selected zones properties
        private int? _dayOptionIndex;
        private int? _nightOptionIndex;


        public bool? IsPanelData
        {
            get
            {
                try
                {
                    if (NoItemSelected)
                        return null;
                    
                    bool? result = null;
                    for (int z = 0; z < _zoneList.Count; z++)
                    {
                        if (z == 0)
                            result = _zoneList[z].IsPanelData;
                        else if (result != _zoneList[z].IsPanelData)
                            return null;
                    }
                    return result;
                }
                finally
                {
                    OnPropertyChanged(nameof(IsPanelDataIsNull));
                }
            }
        }

        public int? ZoneNum { get => _zoneList.Count == 1 ? _zoneList[0].ZoneNum : null; }

        public string ZoneDesc
        {
            get => _zoneList.Count == 1 ? _zoneList[0].ZoneDesc : null;
            set
            {
                //NB: can edit desc when only 1 row is selected
                if (_zoneList.Count == 1)
                {
                    _zoneList[0].SetZoneDesc(value);
                    OnPropertyChanged(nameof(ZoneDescIsValid));
                }
            }
        }

        public TimeSpan? SounderDelay
        {
            get
            {
                try
                {
                    if (NoItemSelected)
                        return null;

                    TimeSpan? result = null;
                    foreach (var z in _zoneList)
                    {
                        if (result is null)
                            result = z.SounderDelay;
                        else if (!result.Equals(z.SounderDelay))
                            return null;
                    }
                    return result;
                }
                finally
                {
                    OnPropertyChanged(nameof(SounderDelayIsNull));
                    //OnPropertyChanged(nameof(ZonesHaveCommonSounderDelay));
                    //OnPropertyChanged(nameof(IndicateMultipleValues));
                }
            }
            set
            {
                if (value is not null)
                {
                    foreach (var z in _zoneList)
                    {
                        z.SounderDelay = (TimeSpan)value;
                        z.RefreshView();
                    }
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(SounderDelayIsNull));
                OnPropertyChanged(nameof(SounderDelayIsValid));
                OnPropertyChanged(nameof(ZonesHaveCommonSounderDelay));
                OnPropertyChanged(nameof(IndicateMultipleValues));
                OnPropertyChanged(nameof(DelayTotal));
                OnPropertyChanged(nameof(DelayTotalIsValid));
                OnPropertyChanged(nameof(DelayTotalHighErrorMessage));
            }
        }

        public TimeSpan? Relay1Delay
        {
            get
            {
                try
                {
                    if (NoItemSelected)
                        return null;

                    TimeSpan? result = null;
                    foreach (var z in _zoneList)
                    {
                        if (result is null)
                            result = z.Relay1Delay;
                        else if (!result.Equals(z.Relay1Delay))
                            return null;
                    }
                    return result;
                }
                finally
                {
                    OnPropertyChanged(nameof(Relay1Delay));
                    OnPropertyChanged(nameof(ZonesHaveCommonRelay1Delay));
                    OnPropertyChanged(nameof(IndicateMultipleValues));
                }
            }
            set
            {
                if (value is not null)
                {
                    foreach (var z in _zoneList)
                    {
                        z.Relay1Delay = (TimeSpan)value;
                        z.RefreshView();
                    }
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(Relay1Delay));
                OnPropertyChanged(nameof(Relay1DelayIsNull));
                OnPropertyChanged(nameof(Relay1DelayIsValid));
                OnPropertyChanged(nameof(ZonesHaveCommonRelay1Delay));
                OnPropertyChanged(nameof(IndicateMultipleValues));
                OnPropertyChanged(nameof(DelayTotal));
                OnPropertyChanged(nameof(DelayTotalIsValid));
                OnPropertyChanged(nameof(DelayTotalHighErrorMessage));
            }
        }

        public TimeSpan? Relay2Delay
        {
            get
            {
                try
                {
                    if (NoItemSelected)
                        return null;

                    TimeSpan? result = null;
                    foreach (var z in _zoneList)
                    {
                        if (result is null)
                            result = z.Relay2Delay;
                        else if (!result.Equals(z.Relay2Delay))
                            return null;
                    }
                    return result;
                }
                finally
                {
                    OnPropertyChanged(nameof(Relay2Delay));
                    OnPropertyChanged(nameof(ZonesHaveCommonRelay2Delay));
                    OnPropertyChanged(nameof(IndicateMultipleValues));
                }
            }
            set
            {
                if (value is not null)
                {
                    foreach (var z in _zoneList)
                    {
                        z.Relay2Delay = (TimeSpan)value;
                        z.RefreshView();
                    }
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(Relay2Delay));
                OnPropertyChanged(nameof(Relay2DelayIsNull));
                OnPropertyChanged(nameof(Relay2DelayIsValid));
                OnPropertyChanged(nameof(ZonesHaveCommonRelay2Delay));
                OnPropertyChanged(nameof(IndicateMultipleValues));
                OnPropertyChanged(nameof(DelayTotal));
                OnPropertyChanged(nameof(DelayTotalIsValid));
                OnPropertyChanged(nameof(DelayTotalHighErrorMessage));
            }
        }

        public TimeSpan? RemoteDelay
        {
            get
            {
                try
                {
                    if (NoItemSelected)
                        return null;

                    TimeSpan? result = null;
                    foreach (var z in _zoneList)
                    {
                        if (result is null)
                            result = z.RemoteDelay;
                        else if (!result.Equals(z.RemoteDelay))
                            return null;
                    }
                    return result;
                }
                finally
                {
                    OnPropertyChanged(nameof(RemoteDelayIsNull));
                    OnPropertyChanged(nameof(ZonesHaveCommonRemoteDelay));
                    OnPropertyChanged(nameof(IndicateMultipleValues));
                }
            }
            set
            {
                if (value is not null)
                {
                    foreach (var z in _zoneList)
                    {
                        z.RemoteDelay = (TimeSpan)value;
                        z.RefreshView();
                    }
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(RemoteDelayIsNull));
                OnPropertyChanged(nameof(RemoteDelayIsValid));
                OnPropertyChanged(nameof(ZonesHaveCommonRemoteDelay));
                OnPropertyChanged(nameof(IndicateMultipleValues));
                OnPropertyChanged(nameof(DelayTotal));
                OnPropertyChanged(nameof(DelayTotalIsValid));
                OnPropertyChanged(nameof(DelayTotalHighErrorMessage));
            }
        }

        public TimeSpan DelayTotal
        {
            get
            {
                var sd = SounderDelay??new TimeSpan(0,0,0);
                var r1 = Relay1Delay??new TimeSpan(0,0,0);
                var r2 = Relay2Delay??new TimeSpan(0,0,0);
                var rd = RemoteDelay??new TimeSpan(0,0,0);
                return sd.Add(r1).Add(r2).Add(rd).Add(InputDelay);
            }
        }

        public bool? Detectors
        {
            get
            {
                try
                {
                    if (NoItemSelected)
                        return null;

                    bool? result = null;
                    foreach (var z in _zoneList)
                    {
                        if (result is null)
                            result = z.Detectors;
                        else if (result != z.Detectors)
                            return null;
                    }
                    return result;
                }
                finally
                {
                    OnPropertyChanged(nameof(DetectorsIsNull));
                    OnPropertyChanged(nameof(ZonesHaveCommonDetectors));
                    OnPropertyChanged(nameof(IndicateMultipleValues));
                }
            }
            set
            {
                if (value is not null)
                {
                    foreach (var z in _zoneList)
                    {
                        z.Detectors = (bool)value;
                        z.RefreshView();
                    }
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(DetectorsIsNull));
                OnPropertyChanged(nameof(ZonesHaveCommonDetectors));
                OnPropertyChanged(nameof(IndicateMultipleValues));
            }
        }

        public bool? MCPs
        {
            get
            {
                try
                {
                    if (NoItemSelected)
                        return null;

                    bool? result = null;
                    foreach (var z in _zoneList)
                    {
                        if (result is null)
                            result = z.MCPs;
                        else if (result != z.MCPs)
                            return null;
                    }
                    return result;
                }
                finally
                {
                    OnPropertyChanged(nameof(MCPsIsNull));
                    OnPropertyChanged(nameof(ZonesHaveCommonMCPs));
                    OnPropertyChanged(nameof(IndicateMultipleValues));
                }
            }
            set
            {
                if (value is not null)
                {
                    foreach (var z in _zoneList)
                    {
                        z.MCPs = (bool)value;
                        z.RefreshView();
                    }
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(MCPsIsNull));
                OnPropertyChanged(nameof(ZonesHaveCommonMCPs));
                OnPropertyChanged(nameof(IndicateMultipleValues));
            }
        }
        
        public bool? EndDelays
        {
            get
            {
                try
                {
                    if (NoItemSelected)
                        return null;

                    bool? result = null;
                    foreach (var z in _zoneList)
                    {
                        if (result is null)
                            result = z.EndDelays;
                        else if (result != z.EndDelays)
                            return null;
                    }
                    return result;
                }
                finally
                {
                    OnPropertyChanged(nameof(EndDelaysIsNull));
                    OnPropertyChanged(nameof(ZonesHaveCommonEndDelays));
                    OnPropertyChanged(nameof(IndicateMultipleValues));
                }
            }
            set
            {
                if (value is not null)
                {
                    foreach (var z in _zoneList)
                    {
                        z.EndDelays = (bool)value;
                        z.RefreshView();
                    }
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(EndDelaysIsNull));
                OnPropertyChanged(nameof(ZonesHaveCommonEndDelays));
                OnPropertyChanged(nameof(IndicateMultipleValues));
            }
        }

        public string DayOption
        {
            get
            {
                _dayOptionIndex = null;

                if (NoItemSelected || !ZonesHaveCommonDayOption)
                    return null;

                if (_zoneList.Count == 1)
                {
                    if (_zoneList[0].IsPanelData)
                        return null;
                    _dayOptionIndex = _zoneList[0].Day?.DependencyOption switch { ZoneDependencyOptions.NotSet => 0, _ => (int)_zoneList[0].Day?.DependencyOption };
                    return DependencyOptions[(int)_dayOptionIndex];
                }

                _dayOptionIndex = null;
                foreach (var z in _zoneList)
                {
                    if (z.IsPanelData)
                        continue;

                    if (_dayOptionIndex is null)
                        _dayOptionIndex = z.Day.DependencyOption switch { ZoneDependencyOptions.NotSet => 0, _ => (int)_zoneList[0].Day.DependencyOption };
                    else if (_dayOptionIndex != (int)z.Day.DependencyOption)
                    {
                        _dayOptionIndex = null;
                        return null;
                    }
                }

                return _dayOptionIndex is not null && _dayOptionIndex > -1 ? DependencyOptions[(int)_dayOptionIndex] : null;
            }

            set
            {
                if (value is not null && !NoItemSelected)
                    foreach (var d in _zoneList)
                    {
                        if (d.Day is not null)
                            d.Day.DependencyOption = (ZoneDependencyOptions)(_dayOptionIndex = findIndexInCombo(DependencyOptions, value) ?? 0);
                        d.RefreshView();
                    }

                OnPropertyChanged();
                OnPropertyChanged(nameof(DayOptionIsValid));
                OnPropertyChanged(nameof(ShowDayDetectorReset));
                OnPropertyChanged(nameof(ShowDayAlarmReset));
                OnPropertyChanged(nameof(ZonesHaveCommonDayOption));
                OnPropertyChanged(nameof(IndicateMultipleValues));
            }
        }

        public string NightOption
        {
            get
            {
                _nightOptionIndex = null;

                if (NoItemSelected || !ZonesHaveCommonNightOption)
                    return null;

                if (_zoneList.Count == 1)
                {
                    if (_zoneList[0].IsPanelData)
                        return null;
                    _nightOptionIndex = _zoneList[0].Night.DependencyOption switch { ZoneDependencyOptions.NotSet => 0, _ => (int)_zoneList[0].Night.DependencyOption };
                    return DependencyOptions[(int)_nightOptionIndex];
                }

                _nightOptionIndex = null;
                foreach (var z in _zoneList)
                {
                    if (z.IsPanelData)
                        continue;

                    if (_nightOptionIndex is null)
                        _nightOptionIndex = z.Night.DependencyOption switch { ZoneDependencyOptions.NotSet => 0, _ => (int)_zoneList[0].Night.DependencyOption };
                    else if (_nightOptionIndex != (int)z.Night.DependencyOption)
                    {
                        _nightOptionIndex = null;
                        return null;
                    }
                }

                return _nightOptionIndex is not null && _nightOptionIndex > -1 ? DependencyOptions[(int)_nightOptionIndex] : null;
            }

            set
            {
                if (value is not null && !NoItemSelected)
                    foreach (var d in _zoneList)
                    {
                        if (d.Night is not null)
                            d.Night.DependencyOption = (ZoneDependencyOptions)(_nightOptionIndex = findIndexInCombo(DependencyOptions, value) ?? 0);
                        d.RefreshView();
                    }

                OnPropertyChanged();
                OnPropertyChanged(nameof(NightOptionIsValid));
                OnPropertyChanged(nameof(ShowNightDetectorReset));
                OnPropertyChanged(nameof(ShowNightAlarmReset));
                OnPropertyChanged(nameof(ZonesHaveCommonNightOption));
                OnPropertyChanged(nameof(IndicateMultipleValues));
            }
        }

        public TimeSpan? DayDetectorReset
        {
            get
            {
                try
                {
                    if (NoItemSelected)
                        return null;

                    if (IsPanelData != false)
                        return null;

                    TimeSpan? result = null;
                    foreach (var z in _zoneList)
                    {
                        if (_dayOptionIndex is not null)
                        {
                            if (result is null)
                                result = z.Day.DetectorReset;
                            else if (result != z.Day.DetectorReset)
                                return null;
                        }
                    }
                    return result;
                }
                finally
                {
                    OnPropertyChanged(nameof(DayDetectorResetIsNull));
                }
            }
            set
            {
                if (value is not null)
                {
                    foreach (var z in _zoneList)
                    {
                        z.Day.DetectorReset = (TimeSpan)value;
                        z.RefreshView();
                    }
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(DayDetectorResetIsNull));
                OnPropertyChanged(nameof(DayDetectorResetIsValid));
                OnPropertyChanged(nameof(ZonesHaveCommonDayDetectorReset));
                OnPropertyChanged(nameof(IndicateMultipleValues));
            }
        }

        public TimeSpan? NightDetectorReset
        {
            get
            {
                try
                {
                    if (NoItemSelected)
                        return null;

                    if (IsPanelData != false)
                        return null;

                    TimeSpan? result = null;
                    foreach (var z in _zoneList)
                    {
                        if (result is null)
                            result = z.Night.DetectorReset;
                        else if (result != z.Night.DetectorReset)
                            return null;
                    }
                    return result;
                }
                finally
                {
                    OnPropertyChanged(nameof(NightDetectorResetIsNull));
                }
            }
            set
            {
                if (value is not null)
                {
                    foreach (var z in _zoneList)
                    {
                        z.Night.DetectorReset = (TimeSpan)value;
                        z.RefreshView();
                    }
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(NightDetectorResetIsNull));
                OnPropertyChanged(nameof(NightDetectorResetIsValid));
                OnPropertyChanged(nameof(ZonesHaveCommonNightDetectorReset));
                OnPropertyChanged(nameof(IndicateMultipleValues));
            }
        }

        public TimeSpan? DayAlarmReset
        {
            get
            {
                try
                {
                    if (NoItemSelected)
                        return null;

                    if (IsPanelData != false)
                        return null;

                    TimeSpan? result = null;
                    foreach (var z in _zoneList)
                    {
                        if (_nightOptionIndex is not null)
                        {
                            if (result is null)
                                result = z.Day.AlarmReset;
                            else if (result != z.Day.AlarmReset)
                                return null;
                        }
                    }
                    return result;
                }
                finally
                {
                    OnPropertyChanged(nameof(DayAlarmResetIsNull));
                }
            }
            set
            {
                if (value is not null)
                {
                    foreach (var z in _zoneList)
                    {
                        z.Day.AlarmReset = (TimeSpan)value;
                        z.RefreshView();
                    }
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(DayAlarmResetIsNull));
                OnPropertyChanged(nameof(DayAlarmResetIsValid));
                OnPropertyChanged(nameof(ZonesHaveCommonDayAlarmReset));
                OnPropertyChanged(nameof(IndicateMultipleValues));
            }
        }

        public TimeSpan? NightAlarmReset
        {
            get
            {
                try
                {
                    if (NoItemSelected)
                        return null;

                    if (IsPanelData != false)
                        return null;

                    TimeSpan? result = null;
                    foreach (var z in _zoneList)
                    {
                        if (result is null)
                            result = z.Night.AlarmReset;
                        else if (result != z.Night.AlarmReset)
                            return null;
                    }
                    return result;
                }
                finally
                {
                    OnPropertyChanged(nameof(NightAlarmResetIsNull));
                }
            }
            set
            {
                if (value is not null)
                {
                    foreach (var z in _zoneList)
                    {
                        z.Night.AlarmReset = (TimeSpan)value;
                        z.RefreshView();
                    }
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(NightAlarmResetIsNull));
                OnPropertyChanged(nameof(NightAlarmResetIsValid));
                OnPropertyChanged(nameof(ZonesHaveCommonNightAlarmReset));
                OnPropertyChanged(nameof(IndicateMultipleValues));
            }
        }


        
        public bool ShowDayDetectorReset   => _dayOptionIndex   is null || (ZoneDependencyOptions?)_dayOptionIndex   == ZoneDependencyOptions.A;
        public bool ShowNightDetectorReset => _nightOptionIndex is null || (ZoneDependencyOptions?)_nightOptionIndex == ZoneDependencyOptions.A;
        public bool ShowDayAlarmReset      => _dayOptionIndex   is null || (ZoneDependencyOptions?)_dayOptionIndex   switch { ZoneDependencyOptions.A or ZoneDependencyOptions.B => true, _ => false };
        public bool ShowNightAlarmReset    => _nightOptionIndex is null || (ZoneDependencyOptions?)_nightOptionIndex switch { ZoneDependencyOptions.A or ZoneDependencyOptions.B => true, _ => false };
        
        

        public bool IsPanelDataIsNull  => IsPanelData is null;
        public bool SounderDelayIsNull => SounderDelay is null;
        public bool Relay1DelayIsNull  => Relay1Delay is null;
        public bool Relay2DelayIsNull  => Relay2Delay is null;
        public bool RemoteDelayIsNull  => RemoteDelay is null;
        public bool DetectorsIsNull    => Detectors is null;
        public bool MCPsIsNull         => MCPs is null;
        public bool EndDelaysIsNull    => EndDelays is null;
        public bool DayOptionIsNull    => _dayOptionIndex is null;
        public bool NightOptionIsNull  => _nightOptionIndex is null;
        public bool DayDetectorResetIsNull   => DayDetectorReset is null;
        public bool NightDetectorResetIsNull => NightDetectorReset is null;
        public bool DayAlarmResetIsNull      => DayAlarmReset is null;
        public bool NightAlarmResetIsNull    => NightAlarmReset is null;

        public bool EnableDependencies       => IsPanelData == false;


        //public string MaxSounderDelay = ZoneConfigData.MaxSounderDelay.ToString("mm':'ss");
        //public string MaxRelay1Delay  = ZoneConfigData.MaxRelay1Delay.ToString("mm':'ss");
        //public string MaxRelay2Delay  = ZoneConfigData.MaxRelay2Delay.ToString("mm':'ss");
        //public string MaxRemoteDelay  = ZoneConfigData.MaxRemoteDelay.ToString("mm':'ss");


        public bool   ZoneDescIsValid            => string.IsNullOrWhiteSpace(ZoneDesc) || ZoneDesc.Length <= ZoneConfigData.MaxNameLength;
        public bool   SounderDelayIsValid        => SounderDelay is null || SounderDelay?.CompareTo(ZoneConfigData.MaxSounderDelay) <= 0;
        public bool   Relay1DelayIsValid         => Relay1Delay is null || Relay1Delay?.CompareTo(ZoneConfigData.MaxRelay1Delay) <= 0;
        public bool   Relay2DelayIsValid         => Relay2Delay is null || Relay2Delay?.CompareTo(ZoneConfigData.MaxRelay2Delay) <= 0;
        public bool   RemoteDelayIsValid         => RemoteDelay is null || RemoteDelay?.CompareTo(ZoneConfigData.MaxRemoteDelay) <= 0;
        public bool   DelayTotalIsValid           { get { var result = DelayTotal.CompareTo(ZoneConfigData.MaxTotalDelay) <= 0; return result; } }
        public bool   DayOptionIsValid           => _dayOptionIndex is null || _dayOptionIndex >= 0 && _dayOptionIndex < DependencyOptions.Count;
        public bool   NightOptionIsValid         => _nightOptionIndex is null || _nightOptionIndex >= 0 && _nightOptionIndex < DependencyOptions.Count;
        public bool   DayDetectorResetIsValid    => !ShowDayDetectorReset || DayDetectorReset is null || DayDetectorReset?.CompareTo(ZoneConfigData.MaxDetectorReset) <= 0;
        public bool   DayAlarmResetIsValid       => !ShowDayAlarmReset || DayAlarmReset is null || DayAlarmReset?.CompareTo(ZoneConfigData.MaxAlarmReset) <= 0;
        public bool   NightDetectorResetIsValid  => !ShowNightDetectorReset || NightDetectorReset is null || NightDetectorReset?.CompareTo(ZoneConfigData.MaxDetectorReset) <= 0;
        public bool   NightAlarmResetIsValid     => !ShowNightAlarmReset || NightAlarmReset is null || NightAlarmReset?.CompareTo(ZoneConfigData.MaxAlarmReset) <= 0;

        public string DelayTotalHighErrorMessage => string.Format(Cultures.Resources.Error_Total_Delay_x_Cannot_Be_More_Than_y, timeToString(DelayTotal), timeToString(ZoneConfigData.MaxTotalDelay));

        
        private string timeToString(TimeSpan time) => time.ToString(time.CompareTo(new(1, 0, 0)) >= 0 ? "hh':'mm':'ss" : "mm':'ss");
        

        public bool ZonesHaveCommonSounderDelay
        {
            get
            {
                if (_zoneList.Count < 2)
                    return true;

                TimeSpan? t = null;
                foreach (var z in _zoneList)
                {
                    if (t is not null && t != z.SounderDelay)
                        return false;
                    t = z.SounderDelay;
                }
                return true;
            }
        }

        public bool ZonesHaveCommonRelay1Delay
        {
            get
            {
                if (_zoneList.Count < 2)
                    return true;

                TimeSpan? t = null;
                foreach (var z in _zoneList)
                {
                    if (t is not null && t != z.Relay1Delay)
                        return false;
                    t = z.Relay1Delay;
                }
                return true;
            }
        }

        public bool ZonesHaveCommonRelay2Delay
        {
            get
            {
                if (_zoneList.Count < 2)
                    return true;

                TimeSpan? t = null;
                foreach (var z in _zoneList)
                {
                    if (t is not null && t != z.Relay2Delay)
                        return false;
                    t = z.Relay2Delay;
                }
                return true;
            }
        }

        public bool ZonesHaveCommonRemoteDelay
        {
            get
            {
                if (_zoneList.Count < 2)
                    return true;

                TimeSpan? t = null;
                foreach (var z in _zoneList)
                {
                    if (t is not null && t != z.RemoteDelay)
                        return false;
                    t = z.RemoteDelay;
                }
                return true;
            }
        }

        public bool ZonesHaveCommonDetectors
        {
            get
            {
                if (_zoneList.Count < 2)
                    return true;

                if (IsPanelData != false)
                    return true;

                bool? d = null;
                foreach (var z in _zoneList)
                {
                    if (d is not null && d != z.Detectors)
                        return false;
                    d = z.Detectors;
                }
                return true;
            }
        }

        public bool ZonesHaveCommonMCPs
        {
            get
            {
                if (_zoneList.Count < 2)
                    return true;

                if (IsPanelData != false)
                    return true;

                bool? m = null;
                foreach (var z in _zoneList)
                {
                    if (m is not null && m != z.MCPs)
                        return false;
                    m = z.MCPs;
                }
                return true;
            }
        }

        public bool ZonesHaveCommonEndDelays
        {
            get
            {
                if (_zoneList.Count < 2)
                    return true;

                if (IsPanelData != false)
                    return true;

                bool? e = null;
                foreach (var z in _zoneList)
                {
                    if (e is not null && e != z.EndDelays)
                        return false;
                    e = z.EndDelays;
                }
                return true;
            }
        }

        public bool ZonesHaveCommonDayOption
        {
            get
            {
                if (_zoneList.Count < 2)
                    return true;

                if (IsPanelData != false)
                    return true;

                ZoneDependencyOptions? e = null;
                foreach (var z in _zoneList)
                {
                    if (e is not null && e != z.Day.DependencyOption)
                        return false;
                    e = z.Day.DependencyOption;
                }
                return true;
            }
        }

        public bool ZonesHaveCommonNightOption
        {
            get
            {
                if (_zoneList.Count < 2)
                    return true;

                if (IsPanelData != false)
                    return true;

                ZoneDependencyOptions? e = null;
                foreach (var z in _zoneList)
                {
                    if (e is not null && e != z.Night.DependencyOption)
                        return false;
                    e = z.Night.DependencyOption;
                }
                return true;
            }
        }

        public bool ZonesHaveCommonDayDetectorReset
        {
            get
            {
                if (_zoneList.Count < 2)
                    return true;

                if (IsPanelData != false)
                    return true;

                if (DayOptionIsNull)
                    return false;

                TimeSpan? r = null;
                foreach (var z in _zoneList)
                {
                    if (r is not null && r != z.Day.DetectorReset)
                        return false;
                    r = z.Day.DetectorReset;
                }
                return true;
            }
        }

        public bool ZonesHaveCommonNightDetectorReset
        {
            get
            {
                if (_zoneList.Count < 2)
                    return true;

                if (IsPanelData != false)
                    return true;

                if (NightOptionIsNull)
                    return false;

                TimeSpan? r = null;
                foreach (var z in _zoneList)
                {
                    //if (DayOption is not null)
                    {
                        if (r is not null && r != z.Night.DetectorReset)
                            return false;
                        r = z.Night.DetectorReset;
                    }
                }
                return true;
            }
        }

        public bool ZonesHaveCommonDayAlarmReset
        {
            get
            {
                if (_zoneList.Count < 2)
                    return true;

                if (IsPanelData != false)
                    return true;

                if (DayOptionIsNull)
                    return false;

                TimeSpan? r = null;
                foreach (var z in _zoneList)
                {
                    if (r is not null && r != z.Day.AlarmReset)
                        return false;
                    r = z.Day.AlarmReset;
                }
                return true;
            }
        }

        public bool ZonesHaveCommonNightAlarmReset
        {
            get
            {
                if (_zoneList.Count < 2)
                    return true;

                if (IsPanelData != false)
                    return true;

                if (NightOptionIsNull)
                    return false;

                TimeSpan? r = null;
                foreach (var z in _zoneList)
                {
                    if (r is not null && r != z.Night.AlarmReset)
                        return false;
                    r = z.Night.AlarmReset;
                }
                return true;
            }
        }

        public bool IndicateMultipleValues => !ZonesHaveCommonSounderDelay || !ZonesHaveCommonRelay1Delay || !ZonesHaveCommonRelay2Delay || !ZonesHaveCommonRemoteDelay 
                                           || !ZonesHaveCommonDetectors || !ZonesHaveCommonMCPs || !ZonesHaveCommonEndDelays
                                           || !ZonesHaveCommonDayOption || !ZonesHaveCommonNightOption || !ZonesHaveCommonDayDetectorReset || !ZonesHaveCommonNightDetectorReset || !ZonesHaveCommonDayAlarmReset || !ZonesHaveCommonNightAlarmReset;
        


        private int? findIndexInCombo(ObservableCollection<string> list, string text)
        {
            if (list is not null)
                for (int i = 0; i < list.Count; i++)
                    if (list[i] == text)
                        return i;
            return null;
        }
        #endregion


        #region Dependencies pop-up
        //private ZoneDependenciesViewModel _popDependenciesSettings;
        //private Popup _dependenciesPopup;

        //internal void ShowDependenciesEditor(Popup popDependenciesEditor)
        //{
        //    //init only if empty or zones have changed: i.e. clear down values when Save/Cancel/Close clicked, else leave them; popup StaysOpen is false
        //    (_dependenciesPopup = popDependenciesEditor).DataContext = _popDependenciesSettings = new(_zoneList);
        //    _dependenciesPopup.IsOpen = true;
        //}

        //internal void CloseDependenciesEditor()
        //{
        //    _dependenciesPopup.IsOpen = false;
        //}

        //internal void SaveDependencies()
        //{
        //    foreach (var z in _zoneList)
        //    {
        //        if (_popDependenciesSettings.DayOption          is not null) _zoneList[z.ZoneNum - 1].Day.DependencyOption   = (ZoneDependencyOptions)_popDependenciesSettings.DayOption;
        //        if (_popDependenciesSettings.DayDetectorReset   is not null) _zoneList[z.ZoneNum - 1].Day.DetectorReset      = (TimeSpan)_popDependenciesSettings.DayDetectorReset;
        //        if (_popDependenciesSettings.DayAlarmReset      is not null) _zoneList[z.ZoneNum - 1].Day.AlarmReset         = (TimeSpan)_popDependenciesSettings.DayAlarmReset;
        //        if (_popDependenciesSettings.NightOption        is not null) _zoneList[z.ZoneNum - 1].Night.DependencyOption = (ZoneDependencyOptions)_popDependenciesSettings.NightOption;
        //        if (_popDependenciesSettings.NightDetectorReset is not null) _zoneList[z.ZoneNum - 1].Night.DetectorReset    = (TimeSpan)_popDependenciesSettings.NightDetectorReset;
        //        if (_popDependenciesSettings.NightAlarmReset    is not null) _zoneList[z.ZoneNum - 1].Night.AlarmReset       = (TimeSpan)_popDependenciesSettings.NightAlarmReset;
        //    }

        //    _dependenciesPopup.IsOpen = false;
        //}
        #endregion


        #region comboboxes
        private ObservableCollection<string> _dependencyOptions;
        public ObservableCollection<string> DependencyOptions { get => _dependencyOptions; set { _dependencyOptions = value; OnPropertyChanged(); } }


        private void initComboList()
        {
            var d = findIndexInCombo(DependencyOptions, DayOption);
            var n = findIndexInCombo(DependencyOptions, NightOption);

            if (_dependencyOptions is null)
            {
                _dependencyOptions = new()
                {
                    Cultures.Resources.Not_Set,
                    Cultures.Resources.Zone_Dependency_A,
                    Cultures.Resources.Zone_Dependency_B,
                    Cultures.Resources.Zone_Dependency_C,
                    Cultures.Resources.Zone_Dependency_Normal,
                    Cultures.Resources.Zone_Dependency_Investigation,
                    Cultures.Resources.Zone_Dependency_Dwelling
                };
            }
            else
            {
                _dependencyOptions[0] = Cultures.Resources.Not_Set;
                _dependencyOptions[1] = Cultures.Resources.Zone_Dependency_A;
                _dependencyOptions[2] = Cultures.Resources.Zone_Dependency_B;
                _dependencyOptions[3] = Cultures.Resources.Zone_Dependency_C;
                _dependencyOptions[4] = Cultures.Resources.Zone_Dependency_Normal;
                _dependencyOptions[5] = Cultures.Resources.Zone_Dependency_Investigation;
                _dependencyOptions[6] = Cultures.Resources.Zone_Dependency_Dwelling;
            }

            OnPropertyChanged(nameof(DependencyOptions));

            //_dayOptionIndex   = findIndexInCombo(DependencyOptions, DayOption);
            //_nightOptionIndex = findIndexInCombo(DependencyOptions, NightOption);
            DayOption = DependencyOptions[d??0];
            NightOption = DependencyOptions[n??0];
            //OnPropertyChanged(nameof(DayOption));
            //OnPropertyChanged(nameof(NightOption));
        }
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
            initComboList();
            RefreshView();
        }
        #endregion


        #region IPanelToolsViewModel implementation
        public void PopulateView(XfpData data)
        {
            if (data is null)
                return;

            _data = data;
            initComboList();
            RefreshView();
        }

        public void RefreshView()
        {
            if (_data is null)
                return;

            //initComboList();

            OnPropertyChanged(nameof(IsReadOnly));
            OnPropertyChanged(nameof(ZoneList));
            OnPropertyChanged(nameof(IsPanelData));
            OnPropertyChanged(nameof(ZoneNum));
            OnPropertyChanged(nameof(ZoneDesc));
            OnPropertyChanged(nameof(InputDelay));
            OnPropertyChanged(nameof(SounderDelay));
            OnPropertyChanged(nameof(Relay1Delay));
            OnPropertyChanged(nameof(Relay2Delay));
            OnPropertyChanged(nameof(RemoteDelay));
            OnPropertyChanged(nameof(DelayTotal));
            OnPropertyChanged(nameof(Detectors));
            OnPropertyChanged(nameof(MCPs));
            OnPropertyChanged(nameof(EndDelays));
            OnPropertyChanged(nameof(DayOption));
            OnPropertyChanged(nameof(NightOption));
            OnPropertyChanged(nameof(DayDetectorReset));
            OnPropertyChanged(nameof(NightDetectorReset));
            OnPropertyChanged(nameof(DayAlarmReset));
            OnPropertyChanged(nameof(NightAlarmReset));
            OnPropertyChanged(nameof(IsPanelDataIsNull));
            OnPropertyChanged(nameof(SounderDelayIsNull));
            OnPropertyChanged(nameof(Relay1DelayIsNull));
            OnPropertyChanged(nameof(Relay2DelayIsNull));
            OnPropertyChanged(nameof(RemoteDelayIsNull));
            OnPropertyChanged(nameof(DetectorsIsNull));
            OnPropertyChanged(nameof(MCPsIsNull));
            OnPropertyChanged(nameof(EndDelaysIsNull));
            OnPropertyChanged(nameof(DayOptionIsNull));
            OnPropertyChanged(nameof(NightOptionIsNull));
            OnPropertyChanged(nameof(DayDetectorResetIsNull));
            OnPropertyChanged(nameof(NightDetectorResetIsNull));
            OnPropertyChanged(nameof(DayAlarmResetIsNull));
            OnPropertyChanged(nameof(NightAlarmResetIsNull));
            OnPropertyChanged(nameof(EnableDependencies));
            OnPropertyChanged(nameof(ZonesHaveCommonSounderDelay));
            OnPropertyChanged(nameof(ZonesHaveCommonRelay1Delay));
            OnPropertyChanged(nameof(ZonesHaveCommonRelay2Delay));
            OnPropertyChanged(nameof(ZonesHaveCommonRemoteDelay));
            OnPropertyChanged(nameof(ZonesHaveCommonDetectors));
            OnPropertyChanged(nameof(ZonesHaveCommonMCPs));
            OnPropertyChanged(nameof(ZonesHaveCommonDayOption));
            OnPropertyChanged(nameof(ZonesHaveCommonNightOption));
            OnPropertyChanged(nameof(ZonesHaveCommonEndDelays));
            OnPropertyChanged(nameof(ZonesHaveCommonDayOption));
            OnPropertyChanged(nameof(ZonesHaveCommonNightOption));
            OnPropertyChanged(nameof(ZonesHaveCommonDayDetectorReset));
            OnPropertyChanged(nameof(ZonesHaveCommonNightDetectorReset));
            OnPropertyChanged(nameof(ZonesHaveCommonDayAlarmReset));
            OnPropertyChanged(nameof(ZonesHaveCommonNightAlarmReset));
            OnPropertyChanged(nameof(ShowDayDetectorReset));
            OnPropertyChanged(nameof(ShowNightDetectorReset));
            OnPropertyChanged(nameof(ShowDayAlarmReset));
            OnPropertyChanged(nameof(ShowNightAlarmReset));
            OnPropertyChanged(nameof(NoItemSelected));
            OnPropertyChanged(nameof(MaxNameLength));
            OnPropertyChanged(nameof(ZoneDescIsValid));
            OnPropertyChanged(nameof(SounderDelayIsValid));
            OnPropertyChanged(nameof(Relay1DelayIsValid));
            OnPropertyChanged(nameof(Relay2DelayIsValid));
            OnPropertyChanged(nameof(RemoteDelayIsValid));
            OnPropertyChanged(nameof(DelayTotalIsValid));
            OnPropertyChanged(nameof(DayOptionIsValid));
            OnPropertyChanged(nameof(NightOptionIsValid));
            OnPropertyChanged(nameof(DayDetectorResetIsValid));
            OnPropertyChanged(nameof(DayAlarmResetIsValid));
            OnPropertyChanged(nameof(NightDetectorResetIsValid));
            OnPropertyChanged(nameof(NightAlarmResetIsValid));
            OnPropertyChanged(nameof(DelayTotalHighErrorMessage));

            if (string.IsNullOrWhiteSpace(ZoneDesc))
                ZoneDesc = string.Format(IsPanelData??false ? Cultures.Resources.Panel_x : Cultures.Resources.Zone_x, ZoneNum);

            Validator.IsValid(Parent);
        }
        #endregion
    }
}
