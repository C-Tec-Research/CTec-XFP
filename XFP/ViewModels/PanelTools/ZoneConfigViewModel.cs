using CTecControls.UI;
using CTecDevices.Protocol;
using CTecUtil.StandardPanelDataTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;
using Xfp.IO;
using Xfp.UI.Interfaces;
using Xfp.UI.Views.PanelTools;

namespace Xfp.ViewModels.PanelTools
{
    public class ZoneConfigViewModel : PanelToolsPageViewModelBase, IPanelToolsViewModel, IConfigToolsPageViewModel
    {
        public ZoneConfigViewModel(FrameworkElement parent, ZoneInfoPanel infoPanel) : base(parent)
        {
            _infoPanelViewModel = infoPanel.DataContext as ZoneInfoPanelViewModel;

            //start time to periodically validate the total delay time (InputDelay + selected device's output delays)
            var totalCheckTimer = new System.Timers.Timer() { Interval = 1000, AutoReset = true };
            totalCheckTimer.Elapsed += (s,e) => handleTimer();
            totalCheckTimer.Start();
        }


        private void handleTimer()
        {
            try
            {
                OnPropertyChanged(nameof(DelayTotalIsValid));
                foreach (var z in ZoneConfigItems) 
                    if (_data.CurrentPanel is not null)
                        z.InputDelay = _data.CurrentPanel.ZoneConfig.InputDelay;
            }
            catch { } 
        }

        
        protected ZoneInfoPanelViewModel _infoPanelViewModel;


        private ObservableCollection<ZoneConfigItemViewModel> _zoneConfigItems = new();
        private ObservableCollection<ZoneConfigItemViewModel> _selectedItems = new();
        private ZoneConfigItemViewModel _selectedZone;

        public ObservableCollection<ZoneConfigItemViewModel> ZoneConfigItems  { get => _zoneConfigItems;  set { _zoneConfigItems = value; OnPropertyChanged(); } }
        public ObservableCollection<ZoneConfigItemViewModel> SelectedItems    { get => _selectedItems;    set { _selectedItems = value; OnPropertyChanged(); } }
        public ZoneConfigItemViewModel                       SelectedZone     { get => _selectedZone;     set { _selectedZone = value; OnPropertyChanged(); } }

        public TimeSpan InputDelay
        {
            get => _data.CurrentPanel.ZoneConfig.InputDelay;
            set
            {
                _data.CurrentPanel.ZoneConfig.InputDelay = _infoPanelViewModel.InputDelay = value;
                foreach (var z in ZoneConfigItems)
                    z.InputDelay = _data.CurrentPanel.ZoneConfig.InputDelay;
                OnPropertyChanged();
            }
        }

        public TimeSpan InvestigationPeriod { get => _data.CurrentPanel.ZoneConfig.InvestigationPeriod; set { _data.CurrentPanel.ZoneConfig.InvestigationPeriod = value; OnPropertyChanged(); } }

        public bool   DelayTotalIsValid    => _infoPanelViewModel.DelayTotalIsValid;


        private double _zonesLeftHeaderWidth              = 170;
        private double _zonesOutputDelaysHeaderWidth      = 70;
        private double _zonesFunctioningWithHeaderWidth   = 120;
        private double _zonesMiddleHeaderWidth            = 120;
        private double _zonesDayDependenciesHeaderWidth   = 200;
        private double _zonesNightDependenciesHeaderWidth = 200;
        private double _zonesNormalHeaderHeight           = 13;
        private double _zonesMultHeaderHeight             = 52;
        private double _panelsLeftHeaderWidth             = 170;
        private double _panelsOutputDelaysHeaderWidth     = 70;
        public double ZonesLeftHeaderWidth              { get => _zonesLeftHeaderWidth;              set { _zonesLeftHeaderWidth = value; OnPropertyChanged(); } }
        public double ZonesOutputDelaysHeaderWidth      { get => _zonesOutputDelaysHeaderWidth;      set { _zonesOutputDelaysHeaderWidth = value; OnPropertyChanged(); } }
        public double ZonesFunctioningWithHeaderWidth   { get => _zonesFunctioningWithHeaderWidth;   set { _zonesFunctioningWithHeaderWidth = value; OnPropertyChanged(); } }
        public double ZonesMiddleHeaderWidth            { get => _zonesMiddleHeaderWidth;            set { _zonesMiddleHeaderWidth = value; OnPropertyChanged(); } }
        public double ZonesDayDependenciesHeaderWidth   { get => _zonesDayDependenciesHeaderWidth;   set { _zonesDayDependenciesHeaderWidth = value; OnPropertyChanged(); } }
        public double ZonesNightDependenciesHeaderWidth { get => _zonesNightDependenciesHeaderWidth; set { _zonesNightDependenciesHeaderWidth = value; OnPropertyChanged(); } }
        public double ZonesNormalHeaderHeight           { get => _zonesNormalHeaderHeight;           set { _zonesNormalHeaderHeight = value; OnPropertyChanged(); } }
        public double ZonesMultHeaderHeight             { get => _zonesMultHeaderHeight;             set { _zonesMultHeaderHeight = value; OnPropertyChanged(); } }
        public double PanelsLeftHeaderWidth             { get => _panelsLeftHeaderWidth;             set { _panelsLeftHeaderWidth = value; OnPropertyChanged(); } }
        public double PanelsOutputDelaysHeaderWidth     { get => _panelsOutputDelaysHeaderWidth;     set { _panelsOutputDelaysHeaderWidth = value; OnPropertyChanged(); } }


        /// <summary>
        /// Update the SelectedItems list to match the Zone DataGrid SelectedItems
        /// </summary>
        public void ChangeZoneSelection(System.Collections.IList selectedItems)
        {
            _selectedItems = new();
            foreach (var item in selectedItems)
                _selectedItems.Add(item as ZoneConfigItemViewModel);

            _infoPanelViewModel.ZoneList   = _selectedItems;
            _infoPanelViewModel.InputDelay = InputDelay;
        }
        
        /// <summary>
        /// Update the SelectedItems list to match the Panel DataGrid SelectedItems
        /// </summary>
        public void ChangePanelSelection(System.Collections.IList selectedItems)
        {
            _selectedItems = new();
            foreach (var item in selectedItems)
                _selectedItems.Add(item as ZoneConfigItemViewModel);

            _infoPanelViewModel.ZoneList   = _selectedItems;
            _infoPanelViewModel.InputDelay = InputDelay;
        }
        
        public void MovePrev(DataGrid grid)
        {
            if (movePrev() is ZoneConfigItemViewModel z)
                grid.ScrollIntoView(z);
        }

        public void MoveNext(DataGrid grid)
        {
            if (moveNext() is ZoneConfigItemViewModel z)
                grid.ScrollIntoView(z);
        }

        protected ZoneConfigItemViewModel movePrev()
        {
            var items = ZoneConfigItems;

            if (SelectedItems.Count == 0)
                return SelectedZone = items[0];

            var firstIndex = items.Count;
            foreach (var z in from i in SelectedItems 
                              where i.ZoneNum < firstIndex
                              select i)
                firstIndex = (z.IsPanelData ? z.ZoneNum + ZoneConfigData.NumZones : z.ZoneNum) - 1;

            var newSelectedZone = items[firstIndex > 0 ? firstIndex - 1 : ^1];
            _infoPanelViewModel.ZoneList   = new() { newSelectedZone };
            _infoPanelViewModel.InputDelay = InputDelay;
            SelectedZone = newSelectedZone;

            RefreshView();
            return newSelectedZone;
        }
        
        protected ZoneConfigItemViewModel moveNext()
        {
            var items = ZoneConfigItems;

            if (SelectedItems.Count == 0)
                return SelectedZone = items[0];

            var lastIndex = 0;
            foreach (var z in from i in SelectedItems
                                where i.ZoneNum > lastIndex
                                select i)
                lastIndex = (z.IsPanelData ? z.ZoneNum + ZoneConfigData.NumZones : z.ZoneNum) - 1;

            var newSelectedZone = items[lastIndex < items.Count - 1 ? lastIndex + 1 : 0];
            _infoPanelViewModel.ZoneList   = new() { newSelectedZone };
            _infoPanelViewModel.InputDelay = InputDelay;
            SelectedZone = newSelectedZone;

            RefreshView();
            return newSelectedZone;
        }


        public bool CheckEnvisionPrefixes()
        {
            if (DeviceTypes.CurrentProtocolType != CTecDevices.ObjectTypes.XfpApollo)
                return true;

            foreach (var _ in from z in ZoneConfigItems
                              where !z.ZoneDescIsEnvisionCompatible()
                              select new { })
                return false;

            return true;
        }


        public void MakeZoneDescEnvisionCompatible()
        {
            if (DeviceTypes.CurrentProtocolType == CTecDevices.ObjectTypes.XfpApollo)
                foreach (var z in ZoneConfigItems)
                    z.MakeZoneDescEnvisionCompatible();
        }


        #region ConfigToolsPageViewModelBase overrides
        public override void SetChangesAreAllowedChecker(ChangesAreAllowedChecker checker) => _infoPanelViewModel.SetChangesAreAllowedChecker(CheckChangesAreAllowed = checker);


        public override bool IsReadOnly
        {
            get => base.IsReadOnly;
            set => _infoPanelViewModel.IsReadOnly = base.IsReadOnly = value;
        }
        #endregion


        #region IAppViewModel implementation
        public void SetCulture(CultureInfo culture)
        {
            PageHeader = Cultures.Resources.Nav_Zone_Configuration;

            foreach (var z in _zoneConfigItems)
                z.SetCulture(culture);

            _infoPanelViewModel.SetCulture(culture);

            CultureChanged?.Invoke(culture);
        }
        #endregion


        #region IPanelToolsViewModel implementation
        public void PopulateView(XfpData data)
        {
            if (data is null)
                return;

            _data = data;

            _zoneConfigItems = new();

            if (data.CurrentPanel.ZoneConfig.Zones != null)
            {
                foreach (var z in data.CurrentPanel.ZoneConfig.Zones)
                    _zoneConfigItems.Add(new(z));
                foreach (var p in data.CurrentPanel.ZonePanelConfig.Panels)
                    _zoneConfigItems.Add(new(p));
            }

            _infoPanelViewModel.PopulateView(data);

            RefreshView();
        }
        
        public void RefreshView()
        {        
            if (_data is null)
                return;

            Validator.IsValid(Parent);

            OnPropertyChanged(nameof(ZoneConfigItems));
            foreach (var z in ZoneConfigItems)
                z.RefreshView();
            OnPropertyChanged(nameof(IsReadOnly));
            OnPropertyChanged(nameof(InputDelay));
            OnPropertyChanged(nameof(InvestigationPeriod));
        }
        #endregion


        #region IConfigToolsPageViewModel implementation
        public void EnqueuePanelDownloadCommands(bool allPages)
        {
            PanelComms.ZoneNameReceived       = zoneNameReceived;
            PanelComms.ZoneTimersReceived     = zoneTimersReceived;
            PanelComms.PhasedSettingsReceived = phasedSettingsReceived;

            PanelComms.InitNewDownloadCommandSubqueue(Cultures.Resources.Comms_Zone_Names, null);
            for (int zone = 0; zone < ZoneConfigData.NumZones + ZonePanelConfigData.NumZonePanels; zone++)
                PanelComms.AddCommandRequestZoneName(zone, String.Format(Cultures.Resources.Zone_Name_x, zone + 1));

            PanelComms.InitNewDownloadCommandSubqueue(Cultures.Resources.Comms_Zones, downloadRequestsCompleted);
            for (int zone = 0; zone < ZoneConfigData.NumZones + ZonePanelConfigData.NumZonePanels; zone++)
                PanelComms.AddCommandRequestZoneTimers(zone, String.Format(Cultures.Resources.Zone_Timer_x, zone + 1));

            PanelComms.AddCommandRequestPhasedSettings(Cultures.Resources.Phased_Settings);
        }

        public void EnqueuePanelUploadCommands(bool allPages)
        {
            PanelComms.InitNewUploadCommandSubqueue(Cultures.Resources.Comms_Zone_Names, null);
            for (int i = 0; i < _zoneConfigItems.Count; i++)
            {
                var name = _zoneConfigItems[i].ZoneDesc;
                PanelComms.AddCommandSetZoneName(new(i + 1, name, ZoneConfigData.MaxNameLength), string.Format(Cultures.Resources.Zone_x_Name_y, i + 1, name));
            }

            PanelComms.InitNewUploadCommandSubqueue(Cultures.Resources.Comms_Zones, uploadRequestsCompleted);
            for (int i = 0; i < ZoneConfigData.NumZones + ZonePanelConfigData.NumZonePanels; i++)
                PanelComms.AddCommandSetZoneTimers(bundleTimers(i, _zoneConfigItems[i]), string.Format(Cultures.Resources.Zone_Timer_x, i + 1));

            PanelComms.AddCommandSetPhasedSettings(new() { PhasedDelay         = _data.CurrentPanel.GroupConfig.PhasedDelay, 
                                                           InputDelay          = _data.CurrentPanel.ZoneConfig.InputDelay, 
                                                           InvestigationPeriod = _data.CurrentPanel.ZoneConfig.InvestigationPeriod },
                                                   Cultures.Resources.Phased_Settings);
        }


        private ZoneConfigData.ZoneTimersBundle bundleTimers(int index, ZoneConfigItemViewModel zone)
        {
            var result = new ZoneConfigData.ZoneTimersBundle();
            
            result.Index        = index;
            result.SounderDelay = zone.SounderDelay;
            result.Relay1Delay  = zone.Relay1Delay;
            result.Relay2Delay  = zone.Relay2Delay;
            result.OutputDelay  = zone.RemoteDelay;
            result.Detectors    = zone.Detectors;
            result.MCPs         = zone.MCPs;
            result.EndDelays    = zone.EndDelays;

            if (!zone.IsPanelData)
            {
                result.DayDependency          = zone.Day.DependencyOption;
                result.NightDependency        = zone.Night.DependencyOption;
                result.DayDetectorResetTime   = zone.Day.DetectorReset;
                result.DayDetectorAlarmTime   = zone.Day.AlarmReset;
                result.NightDetectorResetTime = zone.Night.DetectorReset;
                result.NightDetectorAlarmTime = zone.Night.AlarmReset;
            }

            return result;
        }

        
        private ZoneConfigData.PhasedSettingsBundle bundlePhasedSettings()
            => new ZoneConfigData.PhasedSettingsBundle()
            {
                //PhasedDelay = _data.GroupConfig.PhasedDelay,
                //InputDelay = _data.ZoneConfig.InputDelay,
                //InvestigationPeriod = _data.ZoneConfig.InvestigationPeriod
            };

        public bool DataEquals(XfpData otherData) => _data.CurrentPanel.ZoneConfig.Equals(otherData.CurrentPanel.ZoneConfig);

        
        public bool HasErrorsOrWarnings() => _data.CurrentPanel.ZoneConfig.HasErrorsOrWarnings();
        public bool HasErrors()           => _data.CurrentPanel.ZoneConfig.HasErrors();
        public bool HasWarnings()         => _data.CurrentPanel.ZoneConfig.HasWarnings();
        #endregion


        #region Panel comms receive-data handlers
        private bool zoneNameReceived(object data)
        {
            if (data is not IndexedText zone
             || zone.Index < 0 
             || zone.Index >= ZoneConfigData.NumZones + ZonePanelConfigData.NumZonePanels)
                return false;

            CTecUtil.CommsLog.AddReceivedData(string.Format(Cultures.Resources.Zone_x_Name_y, zone.Index + 1, zone.Value));

            _zoneConfigItems[zone.Index].ZoneDesc = zone.Value;
            return true;
        }
        
        private bool zoneTimersReceived(object data)
        {
            if (data is not ZoneConfigData.ZoneTimersBundle zone
             || zone.Index < 0
             || zone.Index > ZoneConfigData.NumZones + ZonePanelConfigData.NumZonePanels)
                return false;

            var type = _zoneConfigItems[zone.Index].IsPanelData ? Cultures.Resources.Panel : Cultures.Resources.Zone;
            CTecUtil.CommsLog.AddReceivedData(string.Format(Cultures.Resources.Zone_Timers_Type_x_Index_y, type, zone.Index + 1));

            _zoneConfigItems[zone.Index].SounderDelay = zone.SounderDelay;
            _zoneConfigItems[zone.Index].Relay1Delay  = zone.Relay1Delay;
            _zoneConfigItems[zone.Index].Relay2Delay  = zone.Relay2Delay;
            _zoneConfigItems[zone.Index].RemoteDelay  = zone.OutputDelay;
            _zoneConfigItems[zone.Index].Detectors = zone.Detectors;
            _zoneConfigItems[zone.Index].MCPs      = zone.MCPs;
            _zoneConfigItems[zone.Index].EndDelays = zone.EndDelays;

            if (!_zoneConfigItems[zone.Index].IsPanelData)
            {
                _zoneConfigItems[zone.Index].Day.DependencyOption   = zone.DayDependency;
                _zoneConfigItems[zone.Index].Night.DependencyOption = zone.NightDependency;
                _zoneConfigItems[zone.Index].Day.DetectorReset      = zone.DayDetectorResetTime;
                _zoneConfigItems[zone.Index].Day.AlarmReset         = zone.DayDetectorAlarmTime;
                _zoneConfigItems[zone.Index].Night.DetectorReset    = zone.NightDetectorResetTime;
                _zoneConfigItems[zone.Index].Night.AlarmReset       = zone.NightDetectorAlarmTime;
            }
            
            ZoneConfigItems[zone.Index].RefreshView();

            return true;
        }

        private bool phasedSettingsReceived(object data)
        {
            if (data is not ZoneConfigData.PhasedSettingsBundle zone)
                return false;

            CTecUtil.CommsLog.AddReceivedData(Cultures.Resources.Zone_Phased_Settings);

            _data.CurrentPanel.GroupConfig.PhasedDelay = zone.PhasedDelay;
            InputDelay          = zone.InputDelay;
            InvestigationPeriod = zone.InvestigationPeriod;
            return true;
        }

        private void downloadRequestsCompleted() { }

        private void uploadRequestsCompleted() { }
        #endregion
        
        public List<string> Printers
        {
            get
            {
                var result = new List<string>();
                foreach (var p in PrinterSettings.InstalledPrinters)
                    result.Add(p.ToString());
                return result;
            }
        }

        private string _selectedPrinter;
        public string SelectedPrinter
        {
            get
            {
                _selectedPrinter ??= Printers.FirstOrDefault();
                return _selectedPrinter;
            }
            set
            {
                _selectedPrinter = value;
                OnPropertyChanged();
            }
        }
    }
}
