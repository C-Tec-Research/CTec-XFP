using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using CTecUtil.ViewModels;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;
using Xfp.UI.Interfaces;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ExplorerBar;

namespace Xfp.ViewModels.PanelTools
{
    internal class ZoneDependenciesViewModel : ViewModelBase, IPanelToolsViewModel
    {
        public ZoneDependenciesViewModel(System.Collections.IList zoneList)
        {
            if (zoneList is null || (ZoneCount = zoneList.Count) == 0)
                return;
            
            for (int i = 0; i < zoneList.Count; i++)
            {
                if (zoneList[i] is ZoneConfigItemViewModel z)
                {
                    if (i == 0)
                    {
                        _zoneNum            = z.ZoneNum;
                        _zoneName           = z.ZoneDesc;
                        _dayOption          = z.Day.DependencyOption;
                        _dayDetectorReset   = z.Day.DetectorReset;
                        _dayAlarmReset      = z.Day.AlarmReset;
                        _nightOption        = z.Night.DependencyOption;
                        _nightDetectorReset = z.Night.DetectorReset;
                        _nightAlarmReset    = z.Night.AlarmReset;
                    }
                    else
                    {
                        if (z.ZoneNum                != _zoneNum)              _zoneNum            = null; 
                        if (z.ZoneDesc               != _zoneName)             _zoneName           = null;
                        if (z.Day.DependencyOption   != _dayOption)            _dayOption          = null;
                        if (z.Day.DetectorReset      != _dayDetectorReset)   { _dayDetectorReset   = new(0, 0, 0); DayDetectorResetIsNull = true; }
                        if (z.Day.AlarmReset         != _dayAlarmReset)      { _dayAlarmReset      = new(0, 0, 0); DayAlarmResetIsNull    = true; }
                        if (z.Night.DependencyOption != _nightOption)          _nightOption        = null;
                        if (z.Night.DetectorReset    != _nightDetectorReset) { _nightDetectorReset = new(0, 0, 0); NightDetectorResetIsNull = true; }
                        if (z.Night.AlarmReset       != _nightAlarmReset)    { _nightAlarmReset    = new(0, 0, 0); NightAlarmResetIsNull    = true; }
                    }
                }
            }

            RefreshView();

            initComboList();
        }


        internal int?   _zoneNum;
        internal string _zoneName;
        internal int    _zoneCount;
        private ZoneDependencyOptions? _dayOption;
        private ZoneDependencyOptions? _nightOption;
        private TimeSpan? _dayDetectorReset;
        private TimeSpan? _nightDetectorReset;
        private TimeSpan? _dayAlarmReset;
        private TimeSpan? _nightAlarmReset;
        private bool _dayDetectorResetIsNull;
        private bool _nightDetectorResetIsNull;
        private bool _dayAlarmResetIsNull;
        private bool _nightAlarmResetIsNull;


        private string _defaultZoneName { get => ZoneNum is not null ? string.Format(Cultures.Resources.Zone_x, ZoneNum) : null; }
        
        public int?   ZoneNum   {  get => _zoneNum;  set { _zoneNum  = value; OnPropertyChanged(); } }
        public string ZoneName  {  get => _zoneName; set { _zoneName = value; OnPropertyChanged(); } }
        public string ZoneDesc  { get => ZoneName == _defaultZoneName ? ZoneName : string.Format(Cultures.Resources.Zone_x_Name_y, ZoneNum, ZoneName); }
        public int    ZoneCount {  get => _zoneCount; set { _zoneCount = value; OnPropertyChanged(); } }
        public string ZoneCountDesc => string.Format(Cultures.Resources.x_Zones, ZoneCount);

        public int? DayOption               { get => (int?)_dayOption;       set { _dayOption   = (ZoneDependencyOptions)value; OnPropertyChanged(); OnPropertyChanged(nameof(ShowDayDetectorReset)); OnPropertyChanged(nameof(ShowDayAlarmReset)); } }
        public int? NightOption             { get => (int?)_nightOption;     set { _nightOption = (ZoneDependencyOptions)value; OnPropertyChanged(); OnPropertyChanged(nameof(ShowNightDetectorReset)); OnPropertyChanged(nameof(ShowNightAlarmReset)); } }
        public TimeSpan? DayDetectorReset   { get => _dayDetectorReset;      set { _dayDetectorReset   = value; OnPropertyChanged(); DayDetectorResetIsNull = value is null; } }
        public TimeSpan? NightDetectorReset { get => _nightDetectorReset;    set { _nightDetectorReset = value; OnPropertyChanged(); NightDetectorResetIsNull = value is null; } }
        public TimeSpan? DayAlarmReset      { get => _dayAlarmReset;         set { _dayAlarmReset      = value; OnPropertyChanged(); DayAlarmResetIsNull = value is null; } }
        public TimeSpan? NightAlarmReset    { get => _nightAlarmReset;       set { _nightAlarmReset    = value; OnPropertyChanged(); NightAlarmResetIsNull = value is null; } }
        
        public bool ShowDayDetectorReset    => _dayOption   == ZoneDependencyOptions.A;
        public bool ShowNightDetectorReset  => _nightOption == ZoneDependencyOptions.A;
        public bool ShowDayAlarmReset       => _dayOption   switch { ZoneDependencyOptions.A or ZoneDependencyOptions.B => true, _ => false };
        public bool ShowNightAlarmReset     => _nightOption switch { ZoneDependencyOptions.A or ZoneDependencyOptions.B => true, _ => false };
        
        public bool DayDetectorResetIsNull   { get => _dayDetectorResetIsNull;   set { _dayDetectorResetIsNull   = value; OnPropertyChanged(); } }
        public bool NightDetectorResetIsNull { get => _nightDetectorResetIsNull; set { _nightDetectorResetIsNull = value; OnPropertyChanged(); } }
        public bool DayAlarmResetIsNull      { get => _dayAlarmResetIsNull;      set { _dayAlarmResetIsNull      = value; OnPropertyChanged(); } }
        public bool NightAlarmResetIsNull    { get => _nightAlarmResetIsNull;    set { _nightAlarmResetIsNull    = value; OnPropertyChanged(); } }

        
        private bool _isOpen;
        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                _isOpen = value;
                OnPropertyChanged();
            }
        }


        #region comboboxes
        private ObservableCollection<string> _dependencyOptions;
        public ObservableCollection<string> DependencyOptions { get => _dependencyOptions; set { _dependencyOptions = value; OnPropertyChanged(); } }


        private void initComboList()
        {
            _dependencyOptions = new()
            {
                Cultures.Resources.Zone_Dependency_A,
                Cultures.Resources.Zone_Dependency_B,
                Cultures.Resources.Zone_Dependency_C,
                Cultures.Resources.Zone_Dependency_Normal,
                Cultures.Resources.Zone_Dependency_Investigation,
                Cultures.Resources.Zone_Dependency_Dwelling
            };
            OnPropertyChanged(nameof(DependencyOptions));
        }
        #endregion


        #region IAppViewModel implementation
        public void SetCulture(CultureInfo culture) { }
        #endregion


        #region IPanelToolsViewModel implementation
        public void PopulateView(XfpData data) { }

        public void RefreshView()
        {
            OnPropertyChanged(nameof(ZoneNum));
            OnPropertyChanged(nameof(ZoneName));
            OnPropertyChanged(nameof(ZoneDesc));
            OnPropertyChanged(nameof(ZoneCount));
            OnPropertyChanged(nameof(ZoneCountDesc));
            OnPropertyChanged(nameof(DayOption));
            OnPropertyChanged(nameof(NightOption));
            OnPropertyChanged(nameof(DayDetectorReset));
            OnPropertyChanged(nameof(NightDetectorReset));
            OnPropertyChanged(nameof(DayAlarmReset));
            OnPropertyChanged(nameof(NightAlarmReset));
            OnPropertyChanged(nameof(ShowDayDetectorReset));
            OnPropertyChanged(nameof(ShowNightDetectorReset));
            OnPropertyChanged(nameof(ShowDayAlarmReset));
            OnPropertyChanged(nameof(ShowNightAlarmReset));
        }
        #endregion
    }

}
