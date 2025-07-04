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
        private List<TimeSpan> _timerEvents => _data?.CurrentPanel.CEConfig.TimerEventTimes??new();
        public List<TimeSpan> TimerEvents => _data?.CurrentPanel.CEConfig.TimerEventTimes??new();
        public TimeSpan TimerEvent1   { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[0]??new(0, 0, 0);  set { _data.CurrentPanel.CEConfig.TimerEventTimes[0] = value; OnPropertyChanged(); } }
        public TimeSpan TimerEvent2   { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[1]??new(0, 0, 0);  set { _data.CurrentPanel.CEConfig.TimerEventTimes[1] = value; OnPropertyChanged(); } }
        public TimeSpan TimerEvent3   { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[2]??new(0, 0, 0);  set { _data.CurrentPanel.CEConfig.TimerEventTimes[2] = value; OnPropertyChanged(); } }
        public TimeSpan TimerEvent4   { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[3]??new(0, 0, 0);  set { _data.CurrentPanel.CEConfig.TimerEventTimes[3] = value; OnPropertyChanged(); } }
        public TimeSpan TimerEvent5   { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[4]??new(0, 0, 0);  set { _data.CurrentPanel.CEConfig.TimerEventTimes[4] = value; OnPropertyChanged(); } }
        public TimeSpan TimerEvent6   { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[5]??new(0, 0, 0);  set { _data.CurrentPanel.CEConfig.TimerEventTimes[5] = value; OnPropertyChanged(); } }
        public TimeSpan TimerEvent7   { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[6]??new(0, 0, 0);  set { _data.CurrentPanel.CEConfig.TimerEventTimes[6] = value; OnPropertyChanged(); } }
        public TimeSpan TimerEvent8   { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[7]??new(0, 0, 0);  set { _data.CurrentPanel.CEConfig.TimerEventTimes[7] = value; OnPropertyChanged(); } }
        public TimeSpan TimerEvent9   { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[8]??new(0, 0, 0);  set { _data.CurrentPanel.CEConfig.TimerEventTimes[8] = value; OnPropertyChanged(); } }
        public TimeSpan TimerEvent10  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[9]??new(0, 0, 0);  set { _data.CurrentPanel.CEConfig.TimerEventTimes[9] = value; OnPropertyChanged(); } }
        public TimeSpan TimerEvent11  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[10]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[10] = value; OnPropertyChanged(); } }
        public TimeSpan TimerEvent12  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[11]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[11] = value; OnPropertyChanged(); } }
        public TimeSpan TimerEvent13  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[12]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[12] = value; OnPropertyChanged(); } }
        public TimeSpan TimerEvent14  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[13]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[13] = value; OnPropertyChanged(); } }
        public TimeSpan TimerEvent15  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[14]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[14] = value; OnPropertyChanged(); } }
        public TimeSpan TimerEvent16  { get => _data?.CurrentPanel.CEConfig.TimerEventTimes[15]??new(0, 0, 0); set { _data.CurrentPanel.CEConfig.TimerEventTimes[15] = value; OnPropertyChanged(); } }

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

            foreach (var c in CEConfigItems)
                c.ActionTypesMenuCount = Actions.Count;
        }

        private void getTriggersList()
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

            foreach (var c in CEConfigItems)
                c.TriggerTypesMenuCount = Triggers.Count;
        }

        private void getGroupsList()
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

            foreach (var c in CEConfigItems)
                c.GroupsMenuCount = Groups.Count;
        }

        private void getInputsList()
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

            foreach (var c in CEConfigItems)
                c.InputsMenuCount = Inputs.Count;
        }

        private void getLoop1DevicesList()
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

            foreach (var c in CEConfigItems)
                c.Loop1DevicesMenuCount = Loop1Devices.Count;
        }

        private void getLoop2DevicesList()
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

            foreach (var c in CEConfigItems)
                c.Loop2DevicesMenuCount = Loop2Devices.Count;
        }

        private void getZonesList()        
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

            foreach (var c in CEConfigItems)
                c.ZonesMenuCount = Zones.Count;
        }

        private void getZonePanelsList()
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

            foreach (var c in CEConfigItems)
                c.ZonePanelsMenuCount = ZonesPanels.Count;
        }

        private void getSetsList()
        {
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

            foreach (var c in CEConfigItems)
                c.SetsMenuCount = Sets.Count;
        }

        private void getEventsList()
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

            foreach (var c in CEConfigItems)
                c.EventsMenuCount = Events.Count;
        }

        private void getRelaysList()
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

            foreach (var c in CEConfigItems)
                c.RelaysMenuCount = Relays.Count;
        }

        private void getSetsRelaysList()   
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

            foreach (var c in CEConfigItems)
                c.SetsRelaysMenuCount = SetsRelays.Count;
        }

        private void getTimesList()
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

            foreach (var c in CEConfigItems)
                c.TimesMenuCount = Times.Count;
        }

        private void getTrueFalseList()
        {
            //if (TrueOrFalse is null)
            //{
                TrueOrFalse = new() { "", Cultures.Resources.True, Cultures.Resources.False };
            //}
            //else
            //{
            //    TrueOrFalse[1] = Cultures.Resources.True;
            //    TrueOrFalse[2] = Cultures.Resources.False;
            //    OnPropertyChanged(nameof(TrueOrFalse));
            //}

            foreach (var c in CEConfigItems)
                c.ConditionsMenuCount = TrueOrFalse.Count;
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
                getTrueFalseList();
            }

            foreach (var c in CEConfigItems)
            {
                c.ZoneNumbersMenuCount  = ZoneNumbers.Count;
                c.EventNumbersMenuCount = EventNumbers.Count;
            }

            foreach (var ce in CEConfigItems)
                ce.RestoreIndices();
        }
        #endregion


        #region save/restore indices
        private List<int> _savedActionTypeIndex;
        private List<int> _savedActionParamIndex;
        private List<int> _savedTriggerTypeIndex;
        private List<int> _savedTriggerParam1Index;
        private List<int> _savedTriggerParam2Index;
        private List<int> _savedTriggerConditionIndex;
        private List<int> _savedResetTypeIndex;
        private List<int> _savedResetParam1Index;
        private List<int> _savedResetParam2Index;
        private List<int> _savedResetConditionIndex;

        public void SaveItemIndices()
        {
            _savedActionTypeIndex       = new();
            _savedActionParamIndex      = new();
            _savedTriggerTypeIndex      = new();
            _savedTriggerParam1Index    = new();
            _savedTriggerParam2Index    = new();
            _savedTriggerConditionIndex = new();
            _savedResetTypeIndex        = new();
            _savedResetParam1Index      = new();
            _savedResetParam2Index      = new();
            _savedResetConditionIndex   = new();

            foreach (var c in CEConfigItems)
            {
                _savedActionTypeIndex.Add(c.SelectedActionTypeIndex);
                _savedActionParamIndex.Add(c.SelectedActionParamIndex);
                _savedTriggerTypeIndex.Add(c.SelectedTriggerTypeIndex);
                _savedTriggerParam1Index.Add(c.SelectedTriggerParamIndex);
                _savedTriggerParam2Index.Add(c.SelectedTriggerParam2Index);
                _savedTriggerConditionIndex.Add(c.SelectedTriggerConditionIndex);
                _savedResetTypeIndex.Add(c.SelectedResetTypeIndex);
                _savedResetParam1Index.Add(c.SelectedResetParamIndex);
                _savedResetParam2Index.Add(c.SelectedResetParam2Index);
                _savedResetConditionIndex.Add(c.SelectedResetConditionIndex);
            }
        }

        public void RestoreItemIndices()
        {
            for (int i = 0; i < CEConfigItems.Count; i++)
            {
                CEConfigItems[i].SelectedActionTypeIndex       = _savedActionTypeIndex[i];
                CEConfigItems[i].SelectedActionParamIndex      = _savedActionParamIndex[i];
                CEConfigItems[i].SelectedTriggerTypeIndex      = _savedTriggerTypeIndex[i];
                CEConfigItems[i].SelectedTriggerParamIndex     = _savedTriggerParam1Index[i];
                CEConfigItems[i].SelectedTriggerParam2Index    = _savedTriggerParam2Index[i];
                CEConfigItems[i].SelectedTriggerConditionIndex = _savedTriggerConditionIndex[i];
                CEConfigItems[i].SelectedResetTypeIndex        = _savedResetTypeIndex[i];
                CEConfigItems[i].SelectedResetParamIndex       = _savedResetParam1Index[i];
                CEConfigItems[i].SelectedResetParam2Index      = _savedResetParam2Index[i];
                CEConfigItems[i].SelectedResetConditionIndex   = _savedResetConditionIndex[i];
            }
        }
        #endregion


        #region IAppViewModel implementation
        public void SetCulture(CultureInfo culture)
        {
            PageHeader = Cultures.Resources.Nav_C_And_E_Configuration;
            
            //fudge to save
            SaveItemIndices();

            InitComboLists();
            
            foreach (var ce in _ceConfigItems)
                ce.SetCulture(culture);

            RefreshView();

            CultureChanged?.Invoke(culture);

            RestoreItemIndices();
        }
        #endregion


        #region IPanelToolsViewModel implementation
        public void PopulateView(XfpData data)
        {
            if (data is null)
                return;

            _data = data;

            InitComboLists();

            if (_data.CurrentPanel.CEConfig.Events is not null)
                for (int i = 0; i < _data.CurrentPanel.CEConfig.Events.Count && i < CEConfigItems.Count; i++)
                    CEConfigItems[i].Data = _data.CurrentPanel.CEConfig.Events[i];
            
            RefreshView();
        }


        public void RefreshView()
        {
            if (_data is null)
                return;

            InitComboLists();

            refreshTimes();
            
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
            
            //foreach (var c in CEConfigItems)
            //    c.RefreshView();
        }

        private void refreshTimes()
        {
            OnPropertyChanged(nameof(TimerEvents));
            
            //OnPropertyChanged(nameof(TimerEvent1));
            //OnPropertyChanged(nameof(TimerEvent2));
            //OnPropertyChanged(nameof(TimerEvent3));
            //OnPropertyChanged(nameof(TimerEvent4));
            //OnPropertyChanged(nameof(TimerEvent5));
            //OnPropertyChanged(nameof(TimerEvent6));
            //OnPropertyChanged(nameof(TimerEvent7));
            //OnPropertyChanged(nameof(TimerEvent8));
            //OnPropertyChanged(nameof(TimerEvent9));
            //OnPropertyChanged(nameof(TimerEvent10));
            //OnPropertyChanged(nameof(TimerEvent11));
            //OnPropertyChanged(nameof(TimerEvent12));
            //OnPropertyChanged(nameof(TimerEvent13));
            //OnPropertyChanged(nameof(TimerEvent14));
            //OnPropertyChanged(nameof(TimerEvent15));
            //OnPropertyChanged(nameof(TimerEvent16));
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
                                                        TimerEventTime   = _timerEvents[ce]                    
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

            CEConfigItems[ce.Index].RefreshView();

            _timerEvents[ce.Index] = ce.TimerEventTime;

            return true;
        }

        private void downloadRequestsCompleted() { }

        private void uploadRequestsCompleted() { }
        #endregion
    }
}
