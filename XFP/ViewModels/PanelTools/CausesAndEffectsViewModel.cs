using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;
using Xfp.IO;
using Xfp.UI.Interfaces;

namespace Xfp.ViewModels.PanelTools
{
    class CausesAndEffectsViewModel : PanelToolsPageViewModelBase, IPanelToolsViewModel, IConfigToolsPageViewModel
    {
        public CausesAndEffectsViewModel(FrameworkElement parent) : base(parent)
        {
            _timerHeaders = new();
            for (int i = 0; i < CEConfigData.NumEvents; i++)
                _timerHeaders.Add(string.Format(Cultures.Resources.Time_T_x, i + 1));

            initFixedComboLists();

            for (int i = 0; i < CEConfigData.NumEvents; i++)
                CEConfigItems.Add(new CausesAndEffectsItemViewModel(new(), i));
        }


        private List<string> _timerHeaders;


        public List<string>   TimerHeaders { get => _timerHeaders; }
        private List<TimeSpan> TimerEvents  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes??new(); }
        public TimeSpan TimerEvent1  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[0]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[0] = value; OnPropertyChanged(); updateTimersList(0); } }
        public TimeSpan TimerEvent2  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[1]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[1] = value; OnPropertyChanged(); updateTimersList(1); } }
        public TimeSpan TimerEvent3  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[2]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[2] = value; OnPropertyChanged(); updateTimersList(2); } }
        public TimeSpan TimerEvent4  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[3]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[3] = value; OnPropertyChanged(); updateTimersList(3); } }
        public TimeSpan TimerEvent5  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[4]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[4] = value; OnPropertyChanged(); updateTimersList(4); } }
        public TimeSpan TimerEvent6  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[5]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[5] = value; OnPropertyChanged(); updateTimersList(5); } }
        public TimeSpan TimerEvent7  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[6]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[6] = value; OnPropertyChanged(); updateTimersList(6); } }
        public TimeSpan TimerEvent8  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[7]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[7] = value; OnPropertyChanged(); updateTimersList(7); } }
        public TimeSpan TimerEvent9  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[8]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[8] = value; OnPropertyChanged(); updateTimersList(8); } }
        public TimeSpan TimerEvent10  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[9]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[9] = value; OnPropertyChanged(); updateTimersList(9); } }
        public TimeSpan TimerEvent11  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[10]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[10] = value; OnPropertyChanged(); updateTimersList(10); } }
        public TimeSpan TimerEvent12  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[11]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[11] = value; OnPropertyChanged(); updateTimersList(11); } }
        public TimeSpan TimerEvent13  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[12]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[12] = value; OnPropertyChanged(); updateTimersList(12); } }
        public TimeSpan TimerEvent14  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[13]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[13] = value; OnPropertyChanged(); updateTimersList(13); } }
        public TimeSpan TimerEvent15  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[14]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[14] = value; OnPropertyChanged(); updateTimersList(14); } }
        public TimeSpan TimerEvent16  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[15]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[15] = value; OnPropertyChanged(); updateTimersList(15); } }

        private ObservableCollection<CausesAndEffectsItemViewModel> _ceConfigItems = new();
        public ObservableCollection<CausesAndEffectsItemViewModel> CEConfigItems { get => _ceConfigItems; set { _ceConfigItems = value; OnPropertyChanged(); } }
        
        private double       _dataGridWidth = 400.0;
        private List<double> _columnWidths = new();
        public double       DataGridWidth { get => _dataGridWidth; set { _dataGridWidth = value; OnPropertyChanged(); } }
        public List<double> ColumnWidths  { get => _columnWidths;  set { _columnWidths = value; OnPropertyChanged(); } }


        #region input validation
        #endregion


        #region comboboxes
        public List<string> Actions { get; set; }
        public List<string> Triggers { get; set; }
        public List<string> Inputs { get; set; }
        public List<string> Loop1Devices { get; set; }
        public List<string> Loop2Devices { get; set; }
        public List<string> Zones { get; set; }
        public List<string> ZonesPanels { get; set; }
        public List<string> Groups { get; set; }
        public List<string> Sets { get; set; }
        public List<string> Events { get; set; }
        public List<string> Relays { get; set; }
        public List<string> SetsRelays { get; set; }
        public List<string> Times { get; set; }
        public List<string> TrueOrFalse { get; set; }
        public List<string> ZoneNumbers { get; set; }
        public List<string> EventNumbers { get; set; }

        private void getActionsList()
        {
            if (Actions is null)
            {
                Actions = _data.GetCEActionsList();
            }
            else
            {
                int i = 0;
                foreach (var a in _data.GetCEActionsList())
                    Actions[i++] = a;
            }
        }

        private void getTriggersList()
        //{ List<string> tt = new() { "" }; if (_data is not null) tt.AddRange(_data.GetCETriggersList()); return tt; }
        {
            if (Triggers is null)
            {
                Triggers = ["", .. _data.GetCETriggersList()];
            }
            else
            {
                int i = 1;
                foreach (var t in _data.GetCETriggersList())
                    Triggers[i++] = t;
            }
        }

        private void getGroupsList()
        //{ List<string> gg = new() { "" }; if (_data is not null) gg.AddRange(_data.GetGroupsList());     return gg; }
        {
            if (Groups is null)
            {
                Groups = ["", .. _data.GetGroupsList()];
            }
            else
            {
                int i = 1;
                foreach (var g in _data.GetGroupsList())
                    Groups[i++] = g;
            }
        }

        private void getInputsList()
        //{ List<string> ii = new() { "" }; if (_data is not null) ii.AddRange(_data.GetInputsList());     return ii; }
        {
            if (Inputs is null)
            {
                Inputs = ["", .. _data.GetInputsList()];
            }
            else
            {
                int i = 1;
                foreach (var n in _data.GetInputsList())
                    Inputs[i++] = n;
            }
        }
        
        private void getLoop1DevicesList()
        //{ List<string> dd = new() { "" }; if (_data is not null) dd.AddRange(_data.GetLoop1DeviceList(_data.CurrentPanel.PanelNumber)); return dd; }
        {
            if (Loop1Devices is null)
            {
                Loop1Devices = ["", .. _data.GetLoop1DeviceList(_data.CurrentPanel.PanelNumber)];
            }
            else
            {
                int i = 1;
                foreach (var d in _data.GetLoop1DeviceList(_data.CurrentPanel.PanelNumber))
                    Loop1Devices[i++] = d;
            }
            OnPropertyChanged(nameof(Loop1Devices));
        }

        private void getLoop2DevicesList()
        //{ List<string> dd = new() { "" }; if (_data is not null) dd.AddRange(_data.GetLoop2DeviceList(_data.CurrentPanel.PanelNumber)); return dd; }
        {
            if (Loop2Devices is null)
            {
                Loop2Devices = ["", .. _data.GetLoop2DeviceList(_data.CurrentPanel.PanelNumber)];
            }
            else
            {
                int i = 1;
                foreach (var d in _data.GetLoop2DeviceList(_data.CurrentPanel.PanelNumber))
                    Loop2Devices[i++] = d;
            }
        }

        private void getZonesList()        
        //{ List<string> zz = new() { "" }; if (_data is not null) zz.AddRange(_data.GetZonesList());      return zz; }
        {
            if (Zones is null)
            {
                Zones = ["", .. _data.GetZonesList()];
            }
            else
            {
                int i = 1;
                foreach (var z in _data.GetZonesList())
                    Zones[i++] = z;
            }
        }

        private void getZonePanelsList()
        //{ List<string> zp = new() { "" }; if (_data is not null) zp.AddRange(_data.GetZonePanelsList()); return zp; }
        {
            if (ZonesPanels is null)
            {
                ZonesPanels = ["", .. _data.GetZonePanelsList()];
            }
            else
            {
                int i = 1;
                foreach (var z in _data.GetZonePanelsList())
                    ZonesPanels[i++] = z;
            }
        }

        private void getSetsList()
        {
//{ List<string> ss = new() { "" }; if (_data is not null) ss.AddRange(_data.GetSetsList());       return ss; }
            if (Sets is null)
            {
                Sets = ["", .. _data.GetSetsList()];
            }
            else
            {
                int i = 1;
                foreach (var s in _data.GetSetsList())
                    Sets[i++] = s;
            }
        }

        private void getEventsList()
        //{ List<string> ee = new() { "" }; if (_data is not null) ee.AddRange(_data.GetEventsList());     return ee; }
        {
            if (Events is null)
            {
                Events = ["", .. _data.GetEventsList()];
            }
            else
            {
                int i = 1;
                foreach (var e in _data.GetEventsList())
                    Events[i++] = e;
            }
        }

        private void getRelaysList()
        //{ List<string> rr = new() { "" }; if (_data is not null) rr.AddRange(_data.GetRelaysList());     return rr; }
        {
            if (Relays is null)
            {
                Relays = ["", .. _data.GetRelaysList()];
            }
            else
            {
                int i = 1;
                foreach (var s in _data.GetRelaysList())
                    Relays[i++] = s;
            }
        }

        private void getSetsRelaysList()   
        //{ List<string> sr = new() { "" }; if (_data is not null) sr.AddRange(_data.GetSetsRelaysList()); return sr; }
        {
            if (SetsRelays is null)
            {
                SetsRelays = ["", .. _data.GetSetsRelaysList()];
            }
            else
            {
                int i = 1;
                foreach (var s in _data.GetSetsRelaysList())
                    SetsRelays[i++] = s;
            }
        }

        private void getTimesList()
        //{ List<string> tt = new() { "" }; if (_data is not null) tt.AddRange(_data.GetCETimerTList());   return tt; }
        {
            if (Times is null)
            {
                Times = ["", .. _data.GetCETimerTList()];
            }
            else
            {
                int i = 1;
                foreach (var t in _data.GetCETimerTList())
                    Times[i++] = t;
            }
        }

        
        /// <summary>
        /// Initialise the Zone and Group number lists (i.e. numerals-only list)
        /// </summary>
        private void initFixedComboLists()
        {
            ZoneNumbers = new() { "" };
            for (int i = 0; i < ZoneConfigData.NumZones; i++)
                ZoneNumbers.Add((i + 1).ToString());

            EventNumbers = new() { "" };
            for (int i = 0; i < CEConfigData.NumEvents; i++)
                EventNumbers.Add((i + 1).ToString());

            OnPropertyChanged(nameof(ZoneNumbers));
            OnPropertyChanged(nameof(EventNumbers));
        }

        internal void InitComboLists()
        {
            foreach (var ce in CEConfigItems)
                ce.SaveIndices();

            if (_data is not null)
            {
                getActionsList();
                getTriggersList();
                getGroupsList();
                getInputsList();
                getLoop1DevicesList();
                getLoop2DevicesList();
                getZonesList();
                getZonePanelsList();
                getSetsList();
                getEventsList();
                getRelaysList();
                getSetsRelaysList();
                getTimesList();
            }


            if (TrueOrFalse is null)
            {
                TrueOrFalse = new() { "", Cultures.Resources.True, Cultures.Resources.False };
            }
            else
            {
                TrueOrFalse[1] = Cultures.Resources.True;
                TrueOrFalse[2] = Cultures.Resources.False;
            }


            //Times = new() { "" };
            //int t = 1;
            //foreach (var time in TimerEvents)
            //    Times.Add(string.Format(Cultures.Resources.Time_T_x, t++) + ": " + time.ToString(@"hh\:mm"));

            foreach (var ce in CEConfigItems)
                ce.RestoreIndices();
        }


        private void updateTimersList(int index)
        {
            //for (int i = 0; i < TimerEvents.Count; i++)
            //    Times[i] = string.Format(Cultures.Resources.Time_T_x, i + 1) + ": " + TimerEvents[i].ToString(@"hh\:mm");
            Times[index] = string.Format(Cultures.Resources.Time_T_x, index + 1) + ": " + TimerEvents[index].ToString(@"hh\:mm");
        }
        #endregion


        #region IAppViewModel implementation
        public void SetCulture(CultureInfo culture)
        {
            PageHeader = Cultures.Resources.Nav_C_And_E_Configuration;
            InitComboLists();
            
            foreach (var ce in _ceConfigItems)
                ce.SetCulture(culture);

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
            
            //refreshView(true);
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

            OnPropertyChanged(nameof(Actions));
            OnPropertyChanged(nameof(Triggers));
            OnPropertyChanged(nameof(Inputs));
            OnPropertyChanged(nameof(Loop1Devices));
            OnPropertyChanged(nameof(Loop2Devices));
            OnPropertyChanged(nameof(Zones));
            OnPropertyChanged(nameof(ZonesPanels));
            OnPropertyChanged(nameof(Groups));
            OnPropertyChanged(nameof(Sets));
            OnPropertyChanged(nameof(Events));
            OnPropertyChanged(nameof(Relays));
            OnPropertyChanged(nameof(SetsRelays));
            OnPropertyChanged(nameof(Times));
            OnPropertyChanged(nameof(TrueOrFalse));
            
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

            CEConfigItems[ce.Index].SaveIndices();

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

            CEConfigItems[ce.Index].RefreshView();

            TimerEvents[ce.Index] = ce.TimerEventTime;

            CEConfigItems[ce.Index].RestoreIndices();

            return true;
        }

        private void downloadRequestsCompleted() { }

        private void uploadRequestsCompleted() { }
        #endregion
    }
}
