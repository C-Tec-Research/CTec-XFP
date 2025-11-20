using CTecDevices.Protocol;
using CTecUtil.StandardPanelDataTypes;
using CTecUtil.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;

namespace Xfp.ViewModels.PanelTools
{
    public class ZoneConfigItemViewModel : ViewModelBase
    {
        public ZoneConfigItemViewModel() { }
        public ZoneConfigItemViewModel(ZoneBase data)
        {
            _data = data;
            RefreshView();
        }


        private ZoneBase _data;

        private ObservableCollection<TimeSpan> _delays;

        public bool     IsPanelData               => _data.IsPanelData;
        public int      ZoneNum                   => _data?.Number??1;
        public string   ZoneDesc            { get => _data.Name;        set { SetZoneDesc(value); } }
        public string   ZoneDescFull              => string.IsNullOrWhiteSpace(ZoneDesc) ? _data.IsPanelData ? string.Format(Cultures.Resources.Panel_x, _data.Number) : string.Format(Cultures.Resources.Zone_x, _data.Number) : ZoneDesc;

        /// <summary>
        /// Check for Envision compatibility: for Apollo, the DeviceName must be 
        /// prefixed with the Envision device type code; returns true if protocol is CAST.
        /// </summary>
        public bool ZoneDescIsEnvisionCompatible() => ZoneDesc.StartsWith((IsPanelData ? ZoneConfigData.NumZones + ZoneNum : ZoneNum).ToString("00 "));

        /// <summary>
        /// Apply the Envision compatibility prefix: for Apollo, ensure the ZoneDesc is prefixed with the ZoneNum.
        /// </summary>
        public void MakeZoneDescEnvisionCompatible()
        {
            if (ZoneDesc is null || !ZoneDescIsEnvisionCompatible())
            {
                ZoneDesc = string.Format("{0:00} {1}", (IsPanelData ? ZoneConfigData.NumZones + ZoneNum : ZoneNum), ZoneDesc ?? "");

                if (ZoneDesc.Length > ZoneConfigData.MaxNameLength)
                    ZoneDesc = ZoneDesc.Remove(ZoneConfigData.MaxNameLength);
            }
        }


        /// <summary>Input delay as shown in the Zones Page header</summary>
        public TimeSpan InputDelay          { private get => _data.InputDelay; set { _data.InputDelay = value; OnPropertyChanged(nameof(DelayTotalIsValid)); } }
        
        public TimeSpan SounderDelay        { get => _data.SounderDelay;set { _data.SounderDelay = value; updateDelayBindings(); } }
        public TimeSpan Relay1Delay         { get => _data.Relay1Delay; set { _data.Relay1Delay = value; updateDelayBindings(); } }
        public TimeSpan Relay2Delay         { get => _data.Relay2Delay; set { _data.Relay2Delay = value; updateDelayBindings(); } }
        public TimeSpan RemoteDelay         { get => _data.RemoteDelay; set { _data.RemoteDelay = value; updateDelayBindings(); } }
        public bool     Detectors           { get => !_data.IsPanelData ? ((ZoneData)_data).Detectors : false;   set { if (!_data.IsPanelData) ((ZoneData)_data).Detectors = value; OnPropertyChanged(); } }
        public bool     MCPs                { get => !_data.IsPanelData ? ((ZoneData)_data).MCPs : false;        set { if (!_data.IsPanelData) ((ZoneData)_data).MCPs = value; OnPropertyChanged(); } }
        public bool     EndDelays           { get => !_data.IsPanelData ? ((ZoneData)_data).EndDelays : false;   set { if (!_data.IsPanelData) ((ZoneData)_data).EndDelays = value; OnPropertyChanged(); } }
        public ZoneDependency Day                 => !_data.IsPanelData ? ((ZoneData)_data).Day : null;
        public ZoneDependency Night               => !_data.IsPanelData ? ((ZoneData)_data).Night : null;
        public string   DayDependency       { get => !_data.IsPanelData ? Enums.ZoneDependencyOptionToString(((ZoneData)_data).Day.DependencyOption) : "";   set { if (!_data.IsPanelData) ((ZoneData)_data).Day.DependencyOption = Enums.StringToZoneDependencyOption(value); updateDependencyBindings(); } }
        public string   NightDependency     { get => !_data.IsPanelData ? Enums.ZoneDependencyOptionToString(((ZoneData)_data).Night.DependencyOption) : ""; set { if (!_data.IsPanelData) ((ZoneData)_data).Night.DependencyOption = Enums.StringToZoneDependencyOption(value); updateDependencyBindings(); } }
        public TimeSpan DayDetectorReset          => !_data.IsPanelData ? ((ZoneData)_data).Day.DetectorReset : new();
        public TimeSpan DayAlarmReset             => !_data.IsPanelData ? ((ZoneData)_data).Day.AlarmReset : new();
        public TimeSpan NightDetectorReset        => !_data.IsPanelData ? ((ZoneData)_data).Night.DetectorReset : new();
        public TimeSpan NightAlarmReset           => !_data.IsPanelData ? ((ZoneData)_data).Night.AlarmReset : new();
        public string   SounderDelayTime          => _data.SounderDelay.ToString("m':'ss");
        public string   Relay1DelayTime           => _data.Relay1Delay.ToString("m':'ss");
        public string   Relay2DelayTime           => _data.Relay2Delay.ToString("m':'ss");
        public string   RemoteDelayTime           => _data.RemoteDelay.ToString("m':'ss");
        public string   DayDetectorResetTime      => !_data.IsPanelData ? ((ZoneData)_data).Day.DetectorReset.ToString("m':'ss") : "";
        public string   DayAlarmResetTime         => !_data.IsPanelData ? ((ZoneData)_data).Day.AlarmReset.ToString("m':'ss") : "";
        public string   NightDetectorResetTime    => !_data.IsPanelData ? ((ZoneData)_data).Night.DetectorReset.ToString("m':'ss") : "";
        public string   NightAlarmResetTime       => !_data.IsPanelData ? ((ZoneData)_data).Night.AlarmReset.ToString("m':'ss") : "";
        public TimeSpan DelayTotal                => SounderDelay.Add(Relay1Delay).Add(Relay2Delay).Add(RemoteDelay).Add(InputDelay);
        public bool     ShowDayDetectorReset      => _data.IsPanelData ? false : ZoneConfigData.HasDetectorReset(((ZoneData)_data).Day.DependencyOption);
        public bool     ShowNightDetectorReset    => _data.IsPanelData ? false : ZoneConfigData.HasDetectorReset(((ZoneData)_data).Night.DependencyOption);
        public bool     ShowDayAlarmReset         => _data.IsPanelData ? false : ZoneConfigData.HasAlarmReset(((ZoneData)_data).Day.DependencyOption);
        public bool     ShowNightAlarmReset       => _data.IsPanelData ? false : ZoneConfigData.HasAlarmReset(((ZoneData)_data).Night.DependencyOption);
        public bool     SounderDelayIsValid       => SounderDelay.CompareTo(ZoneConfigData.MaxSounderDelay) <= 0;
        public bool     Relay1DelayIsValid        => Relay1Delay.CompareTo(ZoneConfigData.MaxRelay1Delay) <= 0;
        public bool     Relay2DelayIsValid        => Relay2Delay.CompareTo(ZoneConfigData.MaxRelay2Delay) <= 0;
        public bool     RemoteDelayIsValid        => RemoteDelay.CompareTo(ZoneConfigData.MaxRemoteDelay) <= 0;
        public bool     DelayTotalIsValid         => true;//RemoteDelay.CompareTo(ZoneConfigData.MaxTotalDelay) <= 0;
        public bool     ZoneDescIsValid           => string.IsNullOrEmpty(ZoneDesc) || ZoneDesc.Length <= ZoneConfigData.MaxNameLength;
        public bool     DayDependencyIsValid      => !_data.IsPanelData ? (int)Day.DependencyOption   >= 0 && (int)Day.DependencyOption   < Enum.GetNames(typeof(ZoneDependencyOptions)).Length : true;
        public bool     NightDependencyIsValid    => !_data.IsPanelData ? (int)Night.DependencyOption >= 0 && (int)Night.DependencyOption < Enum.GetNames(typeof(ZoneDependencyOptions)).Length : true;
        public bool     DayDetectorResetIsValid   => !ShowDayDetectorReset   || DayDetectorReset.CompareTo(ZoneConfigData.MaxDetectorReset) <= 0;
        public bool     DayAlarmResetIsValid      => !ShowDayAlarmReset      || DayAlarmReset.CompareTo(ZoneConfigData.MaxAlarmReset) <= 0;
        public bool     NightDetectorResetIsValid => !ShowNightDetectorReset || NightDetectorReset.CompareTo(ZoneConfigData.MaxDetectorReset) <= 0;
        public bool     NightAlarmResetIsValid    => !ShowNightAlarmReset    || NightAlarmReset.CompareTo(ZoneConfigData.MaxAlarmReset) <= 0;


        public void SetZoneDesc(string name)
        {
            _data.SetDesc(name);
            OnPropertyChanged(nameof(ZoneDesc));
            OnPropertyChanged(nameof(ZoneDescFull));
            OnPropertyChanged(nameof(ZoneDescIsValid));
        }


        public ObservableCollection<TimeSpan> Delays { get => _delays; set { _delays = value; OnPropertyChanged(); } }
        

        private bool _editingDependencies;
        public bool EditingDependencies { get => _editingDependencies; set { _editingDependencies = value; OnPropertyChanged(); } }


        #region IAppViewModel implementation
        public void SetCulture(CultureInfo culture)
        {
            OnPropertyChanged(nameof(DayDependency));
            OnPropertyChanged(nameof(NightDependency));
        }
        #endregion


        #region IPanelToolsViewModel implementation
        public void PopulateView(XfpData data) { }
        public void PopulateView(ZoneData data)
        {
            _data = data;
            RefreshView();
            var sd = data.SounderDelay;
            var r1 = data.Relay1Delay;
            var r2 = data.Relay2Delay;
            var rd = data.RemoteDelay;
            SounderDelay = TimeOfDay.Midnight;
            Relay1Delay = TimeOfDay.Midnight;
            Relay2Delay = TimeOfDay.Midnight;
            RemoteDelay = TimeOfDay.Midnight;
            SounderDelay = sd;
            Relay1Delay = r1;
            Relay2Delay = r2;
            RemoteDelay = rd;
            Delays = new();
            Delays.Add(data.SounderDelay);
            Delays.Add(data.Relay1Delay);
            Delays.Add(data.Relay2Delay);
            Delays.Add(data.RemoteDelay);
        }

        public void RefreshView(int errorCode = 0)
        {
            OnPropertyChanged(nameof(IsPanelData));
            OnPropertyChanged(nameof(ZoneNum));
            OnPropertyChanged(nameof(ZoneDesc));
            OnPropertyChanged(nameof(ZoneDescFull));
            OnPropertyChanged(nameof(SounderDelay));
            OnPropertyChanged(nameof(Detectors));
            OnPropertyChanged(nameof(MCPs));
            OnPropertyChanged(nameof(EndDelays));

            updateDelayBindings();
            updateDependencyBindings();
        }

        private void updateDelayBindings()
        {
            OnPropertyChanged(nameof(Relay1Delay));
            OnPropertyChanged(nameof(Relay2Delay));
            OnPropertyChanged(nameof(RemoteDelay));
            OnPropertyChanged(nameof(DelayTotal));
            OnPropertyChanged(nameof(SounderDelayTime));
            OnPropertyChanged(nameof(Relay1DelayTime));
            OnPropertyChanged(nameof(Relay2DelayTime));
            OnPropertyChanged(nameof(RemoteDelayTime));
            OnPropertyChanged(nameof(ZoneDescIsValid));
            OnPropertyChanged(nameof(SounderDelayIsValid));
            OnPropertyChanged(nameof(Relay1DelayIsValid));
            OnPropertyChanged(nameof(Relay2DelayIsValid));
            OnPropertyChanged(nameof(RemoteDelayIsValid));
            OnPropertyChanged(nameof(DelayTotalIsValid));
            OnPropertyChanged(nameof(DayDetectorResetIsValid));
            OnPropertyChanged(nameof(DayAlarmResetIsValid));
            OnPropertyChanged(nameof(NightDetectorResetIsValid));
            OnPropertyChanged(nameof(NightAlarmResetIsValid));
        }

        private void updateDependencyBindings()
        {
            OnPropertyChanged(nameof(Day));
            OnPropertyChanged(nameof(DayDependency));
            OnPropertyChanged(nameof(DayDetectorReset));
            OnPropertyChanged(nameof(DayDetectorResetTime));
            OnPropertyChanged(nameof(DayAlarmReset));
            OnPropertyChanged(nameof(DayAlarmResetTime));
            OnPropertyChanged(nameof(Night));
            OnPropertyChanged(nameof(NightDependency));
            OnPropertyChanged(nameof(NightDetectorReset));
            OnPropertyChanged(nameof(NightDetectorResetTime));
            OnPropertyChanged(nameof(NightAlarmReset));
            OnPropertyChanged(nameof(NightAlarmResetTime));
            OnPropertyChanged(nameof(ShowDayDetectorReset));
            OnPropertyChanged(nameof(ShowDayAlarmReset));
            OnPropertyChanged(nameof(ShowNightDetectorReset));
            OnPropertyChanged(nameof(ShowNightAlarmReset));
            OnPropertyChanged(nameof(DayDependencyIsValid));
            OnPropertyChanged(nameof(DayDetectorResetIsValid));
            OnPropertyChanged(nameof(DayAlarmResetIsValid));
            OnPropertyChanged(nameof(NightDependencyIsValid));
            OnPropertyChanged(nameof(NightDetectorResetIsValid));
            OnPropertyChanged(nameof(NightAlarmResetIsValid));
        }
        #endregion
    }
}
