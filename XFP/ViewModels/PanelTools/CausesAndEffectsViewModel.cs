using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using CTecControls.UI;
using CTecControls.ViewModels;
using CTecDevices.Protocol;
using CTecUtil.UI;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;
using Xfp.IO;
using Xfp.UI.Interfaces;
using Xfp.UI.Views;
using Xfp.UI.Views.PanelTools;
using static System.Windows.Forms.Design.AxImporter;
using static Xfp.IO.PanelComms;

namespace Xfp.ViewModels.PanelTools
{
    class CausesAndEffectsViewModel : PanelToolsPageViewModelBase, IPanelToolsViewModel, IConfigToolsPageViewModel
    {
        public CausesAndEffectsViewModel(FrameworkElement parent) : base(parent)
        {
            _timerHeaders = new();
            for (int i = 0; i < CEConfigData.NumEvents; i++)
                _timerHeaders.Add(string.Format(Cultures.Resources.Time_T_x, i + 1));

            //init empty events list with all the menu getters
            for (int i = 0; i < CEConfigData.NumEvents; i++)
            {
                var c = new CausesAndEffectsItemViewModel(new(), i);
                //c.ActionsMenu      = getActions;
                //c.TriggersMenu     = getTriggers;
                //c.InputsMenu       = getInputs;
                //c.Loop1DevicesMenu = getLoop1Devices;
                //c.Loop2DevicesMenu = getLoop2Devices;
                //c.ZonesMenu        = getZones;
                ////c.ZoneNumbersMenu  = getZoneNumbers;
                //c.ZonesPanelsMenu  = getZonesPanels;
                //c.GroupsMenu       = getGroups;
                //c.SetsMenu         = getSets;
                //c.EventsMenu       = getEvents;
                ////c.EventNumbersMenu = getEventNumbers;
                //c.RelaysMenu       = getRelays;
                //c.SetsRelaysMenu   = getSetsRelays;
                //c.TimesMenu        = getTimes;
                //c.TrueOrFalseMenu  = getTrueOrFalse;
                CEConfigItems.Add(c);
            }

            //InitFixedComboLists();
        }


        private List<string> _timerHeaders;

        //public delegate void SystemNameChangedHandler(string siteName);
        //public SystemNameChangedHandler OnSystemNameChanged;


        public List<string>   TimerHeaders { get => _timerHeaders; }
        private List<TimeSpan> TimerEvents  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes??new(); }
        public TimeSpan TimerEvent1  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[0]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[0] = value; OnPropertyChanged(); updateTimersList(); } }
        public TimeSpan TimerEvent2  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[1]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[1] = value; OnPropertyChanged(); updateTimersList(); } }
        public TimeSpan TimerEvent3  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[2]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[2] = value; OnPropertyChanged(); updateTimersList(); } }
        public TimeSpan TimerEvent4  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[3]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[3] = value; OnPropertyChanged(); updateTimersList(); } }
        public TimeSpan TimerEvent5  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[4]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[4] = value; OnPropertyChanged(); updateTimersList(); } }
        public TimeSpan TimerEvent6  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[5]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[5] = value; OnPropertyChanged(); updateTimersList(); } }
        public TimeSpan TimerEvent7  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[6]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[6] = value; OnPropertyChanged(); updateTimersList(); } }
        public TimeSpan TimerEvent8  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[7]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[7] = value; OnPropertyChanged(); updateTimersList(); } }
        public TimeSpan TimerEvent9  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[8]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[8] = value; OnPropertyChanged(); updateTimersList(); } }
        public TimeSpan TimerEvent10  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[9]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[9] = value; OnPropertyChanged(); updateTimersList(); } }
        public TimeSpan TimerEvent11  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[10]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[10] = value; OnPropertyChanged(); updateTimersList(); } }
        public TimeSpan TimerEvent12  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[11]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[11] = value; OnPropertyChanged(); updateTimersList(); } }
        public TimeSpan TimerEvent13  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[12]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[12] = value; OnPropertyChanged(); updateTimersList(); } }
        public TimeSpan TimerEvent14  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[13]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[13] = value; OnPropertyChanged(); updateTimersList(); } }
        public TimeSpan TimerEvent15  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[14]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[14] = value; OnPropertyChanged(); updateTimersList(); } }
        public TimeSpan TimerEvent16  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[15]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[15] = value; OnPropertyChanged(); updateTimersList(); } }

        private ObservableCollection<CausesAndEffectsItemViewModel> _ceConfigItems = new();
        public ObservableCollection<CausesAndEffectsItemViewModel> CEConfigItems { get => _ceConfigItems; set { _ceConfigItems = value; OnPropertyChanged(); } }
        //public CausesAndEffectsItemViewModel CEItem1  { get => _ceConfigItems[0]; set { _ceConfigItems[0] = value; OnPropertyChanged(); } }
        //public CausesAndEffectsItemViewModel CEItem2  { get => _ceConfigItems[1]; set { _ceConfigItems[1] = value; OnPropertyChanged(); } }
        //public CausesAndEffectsItemViewModel CEItem3  { get => _ceConfigItems[2]; set { _ceConfigItems[2] = value; OnPropertyChanged(); } }
        //public CausesAndEffectsItemViewModel CEItem4  { get => _ceConfigItems[3]; set { _ceConfigItems[3] = value; OnPropertyChanged(); } }
        //public CausesAndEffectsItemViewModel CEItem5  { get => _ceConfigItems[4]; set { _ceConfigItems[4] = value; OnPropertyChanged(); } }
        //public CausesAndEffectsItemViewModel CEItem6  { get => _ceConfigItems[5]; set { _ceConfigItems[5] = value; OnPropertyChanged(); } }
        //public CausesAndEffectsItemViewModel CEItem7  { get => _ceConfigItems[6]; set { _ceConfigItems[6] = value; OnPropertyChanged(); } }
        //public CausesAndEffectsItemViewModel CEItem8  { get => _ceConfigItems[7]; set { _ceConfigItems[7] = value; OnPropertyChanged(); } }
        //public CausesAndEffectsItemViewModel CEItem9  { get => _ceConfigItems[8]; set { _ceConfigItems[8] = value; OnPropertyChanged(); } }
        //public CausesAndEffectsItemViewModel CEItem10 { get => _ceConfigItems[9]; set { _ceConfigItems[9] = value; OnPropertyChanged(); } }
        //public CausesAndEffectsItemViewModel CEItem11 { get => _ceConfigItems[10]; set { _ceConfigItems[10] = value; OnPropertyChanged(); } }
        //public CausesAndEffectsItemViewModel CEItem12 { get => _ceConfigItems[11]; set { _ceConfigItems[11] = value; OnPropertyChanged(); } }
        //public CausesAndEffectsItemViewModel CEItem13 { get => _ceConfigItems[12]; set { _ceConfigItems[12] = value; OnPropertyChanged(); } }
        //public CausesAndEffectsItemViewModel CEItem14 { get => _ceConfigItems[13]; set { _ceConfigItems[13] = value; OnPropertyChanged(); } }
        //public CausesAndEffectsItemViewModel CEItem15 { get => _ceConfigItems[14]; set { _ceConfigItems[14] = value; OnPropertyChanged(); } }
        //public CausesAndEffectsItemViewModel CEItem16 { get => _ceConfigItems[15]; set { _ceConfigItems[15] = value; OnPropertyChanged(); } }
        
        private double       _dataGridWidth = 400.0;
        private List<double> _columnWidths = new();
        public double       DataGridWidth { get => _dataGridWidth; set { _dataGridWidth = value; OnPropertyChanged(); } }
        public List<double> ColumnWidths  { get => _columnWidths;  set { _columnWidths = value; OnPropertyChanged(); } }


        #region input validation
        #endregion


        #region comboboxes
        private List<string> _actions { get; set; }
        private List<string> _triggers { get; set; }
        private List<string> _inputs { get; set; }
        private List<string> _loop1Devices { get; set; }
        private List<string> _loop2Devices { get; set; }
        private List<string> _zones { get; set; }
        private List<string> _zonesPanels { get; set; }
        private List<string> _groups { get; set; }
        private List<string> _sets { get; set; }
        private List<string> _events { get; set; }
        private List<string> _relays { get; set; }
        private List<string> _setsRelays { get; set; }
        private List<string> _times { get; set; }
        private List<string> _trueOrFalse { get; set; }

        private List<string> getActions() => _actions;
        private List<string> getTriggers() => _triggers;
        private List<string> getInputs() => _inputs;
        private List<string> getLoop1Devices() => getLoop1DevicesList();
        private List<string> getLoop2Devices() => getLoop2DevicesList();
        private List<string> getZones() => _zones;
        private List<string> getZonesPanels() => _zonesPanels;
        private List<string> getGroups() => _groups;
        private List<string> getSets() => _sets;
        private List<string> getEvents() => _events;
        private List<string> getRelays() => _relays;
        private List<string> getSetsRelays() => _setsRelays;
        private List<string> getTimes() => _times;
        private List<string> getTrueOrFalse() => _trueOrFalse;

        private List<string> getActionsList()      => _data?.GetCEActionsList();
        private List<string> getTriggersList()     { List<string> tt = new() { "" }; if (_data is not null) tt.AddRange(_data.GetCETriggersList()); return tt; }
        private List<string> getGroupsList()       { List<string> gg = new() { "" }; if (_data is not null) gg.AddRange(_data.GetGroupsList());     return gg; }
        private List<string> getInputsList()       { List<string> ii = new() { "" }; if (_data is not null) ii.AddRange(_data.GetInputsList());     return ii; }
        private List<string> getLoop1DevicesList() { List<string> dd = new() { "" }; if (_data is not null) dd.AddRange(_data.GetLoop1DeviceList(_data.CurrentPanel.PanelNumber)); return dd; }
        private List<string> getLoop2DevicesList() { List<string> dd = new() { "" }; if (_data is not null) dd.AddRange(_data.GetLoop2DeviceList(_data.CurrentPanel.PanelNumber)); return dd; }
        private List<string> getZonesList()        { List<string> zz = new() { "" }; if (_data is not null) zz.AddRange(_data.GetZonesList());      return zz; }
        private List<string> getZonePanelsList()   { List<string> zp = new() { "" }; if (_data is not null) zp.AddRange(_data.GetZonePanelsList()); return zp; }
        private List<string> getSetsList()         { List<string> ss = new() { "" }; if (_data is not null) ss.AddRange(_data.GetSetsList());       return ss; }
        private List<string> getEventsList()       { List<string> ee = new() { "" }; if (_data is not null) ee.AddRange(_data.GetEventsList());     return ee; }
        private List<string> getRelaysList()       { List<string> rr = new() { "" }; if (_data is not null) rr.AddRange(_data.GetRelaysList());     return rr; }
        private List<string> getSetsRelaysList()   { List<string> sr = new() { "" }; if (_data is not null) sr.AddRange(_data.GetSetsRelaysList()); return sr; }
        private List<string> getTimersList()       { List<string> tt = new() { "" }; if (_data is not null) tt.AddRange(_data.GetCETimerTList());   return tt; }

        internal void InitComboLists()
        {
            if (_data is not null)
            {
                _actions      = getActionsList();
                _triggers     = getTriggersList();
                _groups       = getGroupsList();
                _inputs       = getInputsList();
                _loop1Devices = getLoop1DevicesList();
                _loop2Devices = getLoop2DevicesList();
                _zones        = getZonesList();
                _zonesPanels  = getZonePanelsList();
                _sets         = getSetsList();
                _events       = getEventsList();
                _relays       = getRelaysList();
                _setsRelays   = getSetsRelaysList();
                _times        = getTimersList();
            }


            if (_trueOrFalse is null)
            {
                _trueOrFalse = new() { "", Cultures.Resources.True, Cultures.Resources.False };
            }
            else
            {
                _trueOrFalse[1] = Cultures.Resources.True;
                _trueOrFalse[2] = Cultures.Resources.False;
            }


            _times = new() { "" };
            int t = 1;
            foreach (var time in TimerEvents)
                _times.Add(string.Format(Cultures.Resources.Time_T_x, t++) + ": " + time.ToString(@"hh\:mm"));

            foreach (var ce in _ceConfigItems)
            {
                //ce.RefreshComboLists();
                ce.Actions      = _actions;
                ce.Triggers     = _triggers;
                ce.Groups       = _groups;
                ce.Inputs       = _inputs;
                ce.Loop1Devices = _loop1Devices;
                ce.Loop2Devices = _loop2Devices;
                ce.Zones        = _zones;
                ce.ZonesPanels  = _zonesPanels;
                ce.Sets         = _sets;
                ce.Events       = _events;
                ce.Relays       = _relays;
                ce.SetsRelays   = _setsRelays;
                ce.Times        = _times;
                ce.TrueOrFalse  = _trueOrFalse;
            }
        }


        private void updateTimersList()
        {
            for (int i = 0; i < TimerEvents.Count; i++)
                _times[i] = string.Format(Cultures.Resources.Time_T_x, i + 1) + ": " + TimerEvents[i].ToString(@"hh\:mm");
            
            foreach (var c in _ceConfigItems)
                c.RefreshTimesList();
        }
        #endregion


        #region IAppViewModel implementation
        public void SetCulture(CultureInfo culture)
        {
            PageHeader = Cultures.Resources.Nav_C_And_E_Configuration;
            InitComboLists();
            RefreshView();

            CultureChanged?.Invoke(culture);
        }
        #endregion


        #region IPanelToolsViewModel implementation
        public void PopulateView(XfpData data)
        {
            if (data is null)
                return;

            _data = data;

            populateView();
        }

        private void populateView()
        { 
            InitComboLists();

            if (_data.CurrentPanel.CEConfig.Events is not null)
                for (int i = 0; i < _data.CurrentPanel.CEConfig.Events.Count && i < CEConfigItems.Count; i++)
                    CEConfigItems[i].Data = _data.CurrentPanel.CEConfig.Events[i];
            
            refreshView(false);
        }


        public void RefreshView() => refreshView(true);

        private void refreshView(bool initComboLists)
        {
            if (_data is null)
                return;

            if (initComboLists)
                InitComboLists();

            OnPropertyChanged(nameof(TimerEvent1));
            OnPropertyChanged(nameof(TimerEvent2));
            OnPropertyChanged(nameof(TimerEvent3));
            OnPropertyChanged(nameof(TimerEvent4));
            OnPropertyChanged(nameof(TimerEvent5));
            OnPropertyChanged(nameof(TimerEvent6));
            OnPropertyChanged(nameof(TimerEvent7));
            OnPropertyChanged(nameof(TimerEvent8));
            OnPropertyChanged(nameof(TimerEvent9));
            OnPropertyChanged(nameof(TimerEvent10));
            OnPropertyChanged(nameof(TimerEvent11));
            OnPropertyChanged(nameof(TimerEvent12));
            OnPropertyChanged(nameof(TimerEvent13));
            OnPropertyChanged(nameof(TimerEvent14));
            OnPropertyChanged(nameof(TimerEvent15));
            OnPropertyChanged(nameof(TimerEvent16));
            OnPropertyChanged(nameof(ColumnWidths));

            foreach (var c in CEConfigItems)
                c.RefreshView();
        }
        #endregion


        #region IConfigToolsPageViewModel implementation
        public void EnqueuePanelDownloadCommands(bool allPages)
        {
            PanelComms.CEEventReceived = ceEventReceived;
            PanelComms.InitNewDownloadCommandSubqueue(Cultures.Resources.Comms_C_And_E, downloadRequestsCompleted);
            for (int ce = 0; ce < CEConfigData.NumEvents; ce++)
            {
                PanelComms.AddCommandRequestCEEvent(ce, String.Format(Cultures.Resources.C_And_E_Event_x, ce + 1));    //bodge: request each twice so the screen updates correctly -
                PanelComms.AddCommandRequestCEEvent(ce, String.Format(Cultures.Resources.C_And_E_Event_x, ce + 1));    //       especially error boxes which otherwise get out of sync
            }
        }

        public void EnqueuePanelUploadCommands(bool allPages)
        {
            PanelComms.InitNewUploadCommandSubqueue(Cultures.Resources.Comms_C_And_E, uploadRequestsCompleted);
            for (int ce = 0; ce < CEConfigData.NumEvents; ce++)
                PanelComms.AddCommandSetCEEvent(new() { Index = ce,
                                                        ActionType       = CEConfigItems[ce].Data.ActionType,
                                                        ActionParam      = CEConfigItems[ce].Data.ActionParam,
                                                        TriggerType      = CEConfigItems[ce].Data.TriggerType,
                                                        TriggerParam     = CEConfigItems[ce].Data.TriggerParam,
                                                        TriggerParam2    = CEConfigItems[ce].Data.TriggerParam2,
                                                        TriggerCondition = CEConfigItems[ce].Data.TriggerCondition,
                                                        ResetType        = CEConfigItems[ce].Data.ResetType,
                                                        ResetParam       = CEConfigItems[ce].Data.ResetParam,
                                                        ResetParam2      = CEConfigItems[ce].Data.ResetParam2,
                                                        ResetCondition   = CEConfigItems[ce].Data.ResetCondition,
                                                        TimerEventTime   = TimerEvents[ce]                    
                                                    },
                                                string.Format(Cultures.Resources.C_And_E_Event_x, ce + 1));
        }
        

        public bool DataEquals(XfpData otherData) => _data.SiteConfig.Equals(otherData.SiteConfig);


        public bool HasErrorsOrWarnings() => _data.SiteConfig.HasErrorsOrWarnings();
        public bool HasErrors()           => _data.SiteConfig.HasErrors();
        public bool HasWarnings()         => _data.SiteConfig.HasWarnings();
        #endregion


        #region Panel comms receive-data handlers
        private bool ceEventReceived(object data)
        {
            if (data is not CEConfigData.CEBundle ce
             || ce.Index < 0 
             || ce.Index >= CEConfigData.NumEvents)
                return false;

            CTecUtil.CommsLog.AddReceivedData(string.Format(Cultures.Resources.C_And_E_Event_x, ce.Index + 1));

            CEConfigItems[ce.Index].Data.ActionType = ce.ActionType;
            CEConfigItems[ce.Index].Data.ActionParam = ce.ActionParam;
            CEConfigItems[ce.Index].Data.TriggerType = ce.TriggerType;
            CEConfigItems[ce.Index].Data.TriggerParam = ce.TriggerParam;
            CEConfigItems[ce.Index].Data.TriggerParam2 = ce.TriggerParam2;
            CEConfigItems[ce.Index].Data.TriggerCondition = ce.TriggerCondition;
            CEConfigItems[ce.Index].Data.ResetType = ce.ResetType;
            CEConfigItems[ce.Index].Data.ResetParam = ce.ResetParam;
            CEConfigItems[ce.Index].Data.ResetParam2 = ce.ResetParam2;
            CEConfigItems[ce.Index].Data.ResetCondition = ce.ResetCondition;

            TimerEvents[ce.Index] = ce.TimerEventTime;

            return true;
        }

        private void downloadRequestsCompleted() { }

        private void uploadRequestsCompleted() { }
        #endregion
    }
}
