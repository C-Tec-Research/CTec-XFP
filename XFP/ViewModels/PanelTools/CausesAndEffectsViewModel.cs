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
        public CausesAndEffectsItemViewModel CEItem1  { get => _ceConfigItems[0]; set { _ceConfigItems[0] = value; OnPropertyChanged(); } }
        public CausesAndEffectsItemViewModel CEItem2  { get => _ceConfigItems[1]; set { _ceConfigItems[1] = value; OnPropertyChanged(); } }
        public CausesAndEffectsItemViewModel CEItem3  { get => _ceConfigItems[2]; set { _ceConfigItems[2] = value; OnPropertyChanged(); } }
        public CausesAndEffectsItemViewModel CEItem4  { get => _ceConfigItems[3]; set { _ceConfigItems[3] = value; OnPropertyChanged(); } }
        public CausesAndEffectsItemViewModel CEItem5  { get => _ceConfigItems[4]; set { _ceConfigItems[4] = value; OnPropertyChanged(); } }
        public CausesAndEffectsItemViewModel CEItem6  { get => _ceConfigItems[5]; set { _ceConfigItems[5] = value; OnPropertyChanged(); } }
        public CausesAndEffectsItemViewModel CEItem7  { get => _ceConfigItems[6]; set { _ceConfigItems[6] = value; OnPropertyChanged(); } }
        public CausesAndEffectsItemViewModel CEItem8  { get => _ceConfigItems[7]; set { _ceConfigItems[7] = value; OnPropertyChanged(); } }
        public CausesAndEffectsItemViewModel CEItem9  { get => _ceConfigItems[8]; set { _ceConfigItems[8] = value; OnPropertyChanged(); } }
        public CausesAndEffectsItemViewModel CEItem10 { get => _ceConfigItems[9]; set { _ceConfigItems[9] = value; OnPropertyChanged(); } }
        public CausesAndEffectsItemViewModel CEItem11 { get => _ceConfigItems[10]; set { _ceConfigItems[10] = value; OnPropertyChanged(); } }
        public CausesAndEffectsItemViewModel CEItem12 { get => _ceConfigItems[11]; set { _ceConfigItems[11] = value; OnPropertyChanged(); } }
        public CausesAndEffectsItemViewModel CEItem13 { get => _ceConfigItems[12]; set { _ceConfigItems[12] = value; OnPropertyChanged(); } }
        public CausesAndEffectsItemViewModel CEItem14 { get => _ceConfigItems[13]; set { _ceConfigItems[13] = value; OnPropertyChanged(); } }
        public CausesAndEffectsItemViewModel CEItem15 { get => _ceConfigItems[14]; set { _ceConfigItems[14] = value; OnPropertyChanged(); } }
        public CausesAndEffectsItemViewModel CEItem16 { get => _ceConfigItems[15]; set { _ceConfigItems[15] = value; OnPropertyChanged(); } }

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
        private List<string> _zones1 { get; set; }
        private List<string> _zones2 { get; set; }
        private List<string> _zonesPanels { get; set; }
        private List<string> _groups { get; set; }
        private List<string> _sets { get; set; }
        private List<string> _events { get; set; }
        private List<string> _events1 { get; set; }
        private List<string> _events2 { get; set; }
        private List<string> _relays { get; set; }
        private List<string> _setsRelays { get; set; }
        private List<string> _times { get; set; }
        private List<string> _trueOrFalse { get; set; }

        private List<string> getActions() => _actions;
        private List<string> getTriggers() => _triggers;
        private List<string> getInputs() => _inputs;
        private List<string> getLoop1Devices() => _loop1Devices;
        private List<string> getLoop2Devices() => _loop2Devices;
        private List<string> getZones() => _zones;
        private List<string> getZones1() => _zones1;
        private List<string> getZones2() => _zones2;
        private List<string> getZonesPanels() => _zonesPanels;
        private List<string> getGroups() => _groups;
        private List<string> getSets() => _sets;
        private List<string> getEvents() => _events;
        private List<string> getEvents1() => _events1;
        private List<string> getEvents2() => _events2;
        private List<string> getRelays() => _relays;
        private List<string> getSetsRelays() => _setsRelays;
        private List<string> getTimes() => _times;
        private List<string> getTrueOrFalse() => _trueOrFalse;

        private void initComboLists()
        {
            _actions = new();
            foreach (var a in Enum.GetValues(typeof(CEActionTypes)))
                _actions.Add(Enums.CEActionTypesToString((CEActionTypes)a));

            _triggers = new();
            foreach (var t in Enum.GetValues(typeof(CETriggerTypes)))
                if ((CETriggerTypes)t != CETriggerTypes.None)
                    _triggers.Add(Enums.CETriggerTypesToString((CETriggerTypes)t));

            _groups = new();
            for (int i = 0; i <= GroupConfigData.NumSounderGroups; i++)
                _groups.Add(i == 0 ? Cultures.Resources.Action_All_Groups : string.Format(Cultures.Resources.Group_x, i));

            _inputs = new();
            for (int i = 0; i < 2; i++)
                _inputs.Add(string.Format(Cultures.Resources.Input_x, i + 1));

            _loop1Devices = new();
            if (_data is not null)
                for (int d = 0; d < DeviceConfigData.NumDevices && d < _data.CurrentPanel.Loop1Config.Devices.Count; d++)
                    _loop1Devices.Add(string.Format(Cultures.Resources.Device_x_Type_y, d + 1, (DeviceTypes.DeviceTypeName(_data.CurrentPanel.Loop1Config.Devices[d].DeviceType, DeviceTypes.CurrentProtocolType) ?? Cultures.Resources.No_Device)));

            _loop2Devices = new();
            if (_data is not null)
                for (int d = 0; d < DeviceConfigData.NumDevices && d < _data.CurrentPanel.Loop2Config.Devices.Count; d++)
                    _loop2Devices.Add(string.Format(Cultures.Resources.Device_x_Type_y, d + 1, (DeviceTypes.DeviceTypeName(_data.CurrentPanel.Loop2Config.Devices[d].DeviceType, DeviceTypes.CurrentProtocolType) ?? Cultures.Resources.No_Device)));

            _zones = new();
            _zones1 = new();
            _zones2 = new();
            for (int i = 0; i < ZoneConfigData.NumZones; i++)
            {
                _zones.Add(string.Format(Cultures.Resources.Zone_x, i + 1));
                _zones1.Add((i + 1).ToString());
                _zones2.Add((i + 1).ToString());
            }

            _zonesPanels = new();
            for (int i = 0; i < ZoneConfigData.NumZones; i++)
                _zonesPanels.Add(string.Format(Cultures.Resources.Zone_x, i + 1));
            for (int i = 0; i < ZoneConfigData.NumPanels; i++)
                _zonesPanels.Add(string.Format(Cultures.Resources.Panel_x, i + 1));

            _sets = new();
            for (int i = 0; i < XfpPanelData.NumSets; i++)
                _sets.Add(string.Format(Cultures.Resources.Set_x, i + 1));

            _events = new();
            _events1 = new();
            _events2 = new();
            for (int i = 0; i < CEConfigData.NumEvents; i++)
            {
                _events.Add(string.Format(Cultures.Resources.Event_x, i + 1));
                _events1.Add((i + 1).ToString());
                _events2.Add((i + 1).ToString());
            }

            _relays = new();
            for (int i = 0; i < XfpPanelData.NumRelays; i++)
                _relays.Add(string.Format(Cultures.Resources.Relay_x, i + 1));

            _setsRelays = new();
            for (int i = 0; i < XfpPanelData.NumSets; i++)
                _setsRelays.Add(string.Format(Cultures.Resources.Set_x, i + 1));
            for (int i = 0; i < XfpPanelData.NumRelays; i++)
                _setsRelays.Add(string.Format(Cultures.Resources.Relay_x, i + 1));

            //_times = new();
            //for (int i = 0; i < CEConfigData.NumEvents; i++)
            //    _times.Add(string.Format(Cultures.Resources.Time_x, string.Format(Cultures.Resources.Time_T_x, i + 1)));

            if (_trueOrFalse is null)
            {
                _trueOrFalse = new() { Cultures.Resources.True, Cultures.Resources.False };
            }
            else
            {
                _trueOrFalse[0] = Cultures.Resources.True;
                _trueOrFalse[1] = Cultures.Resources.False;
            }

            initTimersList();
        }

        private void initTimersList()
        {
            _times = new();
            int t = 1;
            foreach (var time in TimerEvents)
                _times.Add(string.Format(Cultures.Resources.Time_T_x, t++) + ": " + time.ToString(@"hh\:mm"));
        }

        private void updateTimersList()
        {
            for (int i = 0; i < TimerEvents.Count; i++)
                _times[i] = string.Format(Cultures.Resources.Time_T_x, i + 1) + ": " + TimerEvents[i].ToString(@"hh\:mm");
            
            foreach (var c in _ceConfigItems)
                c.RefreshView();
        }
        #endregion


        #region IAppViewModel implementation
        public void SetCulture(CultureInfo culture)
        {
            PageHeader = Cultures.Resources.Nav_C_And_E_Configuration;
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

            _ceConfigItems = new();
            if (data.CurrentPanel.ZoneConfig.Zones != null)
            {
                for (int i = 0; i < data.CurrentPanel.CEConfig.Events.Count; i++)
                {
                    var c = new CausesAndEffectsItemViewModel(data.CurrentPanel.CEConfig.Events[i], i, data.CurrentPanel.LoopConfig);
                    c.SetCulture(CultureInfo.CurrentCulture);
                    c.ActionsMenu      = getActions;
                    c.TriggersMenu     = getTriggers;
                    c.InputsMenu       = getInputs;
                    c.Loop1DevicesMenu = getLoop1Devices;
                    c.Loop2DevicesMenu = getLoop2Devices;
                    c.ZonesMenu        = getZones;
                    c.Zones1Menu       = getZones1;
                    c.Zones2Menu       = getZones2;
                    c.ZonesPanelsMenu  = getZonesPanels;
                    c.GroupsMenu       = getGroups;
                    c.SetsMenu         = getSets;
                    c.EventsMenu       = getEvents;
                    c.Events1Menu      = getEvents1;
                    c.Events2Menu      = getEvents2;
                    c.RelaysMenu       = getRelays;
                    c.SetsRelaysMenu   = getSetsRelays;
                    c.TimesMenu        = getTimes;
                    c.TrueOrFalseMenu  = getTrueOrFalse;
                    _ceConfigItems.Add(c);
                }
            }
            
            RefreshView();
        }

        public void RefreshView()
        {
            if (_data is null)
                return;

            initComboLists();

            //OnPropertyChanged(nameof(TimerEvents));
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
            //initLists();
            foreach (var c in CEConfigItems)
                c.RefreshView();
            OnPropertyChanged(nameof(CEItem1));
            OnPropertyChanged(nameof(CEItem2));
            OnPropertyChanged(nameof(CEItem3));
            OnPropertyChanged(nameof(CEItem4));
            OnPropertyChanged(nameof(CEItem5));
            OnPropertyChanged(nameof(CEItem6));
            OnPropertyChanged(nameof(CEItem7));
            OnPropertyChanged(nameof(CEItem8));
            OnPropertyChanged(nameof(CEItem9));
            OnPropertyChanged(nameof(CEItem10));
            OnPropertyChanged(nameof(CEItem11));
            OnPropertyChanged(nameof(CEItem12));
            OnPropertyChanged(nameof(CEItem13));
            OnPropertyChanged(nameof(CEItem14));
            OnPropertyChanged(nameof(CEItem15));
            OnPropertyChanged(nameof(CEItem16));
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
                                                        ActionType       = CEConfigItems[ce].ActionType,
                                                        ActionParam      = CEConfigItems[ce].ActionParam,
                                                        TriggerType      = CEConfigItems[ce].TriggerType??CETriggerTypes.None,
                                                        TriggerParam     = CEConfigItems[ce].TriggerParam,
                                                        TriggerParam2    = CEConfigItems[ce].TriggerParam2,
                                                        TriggerCondition = CEConfigItems[ce].TriggerCondition,
                                                        ResetType        = CEConfigItems[ce].ResetType??CETriggerTypes.None,
                                                        ResetParam       = CEConfigItems[ce].ResetParam,
                                                        ResetParam2      = CEConfigItems[ce].ResetParam2,
                                                        ResetCondition   = CEConfigItems[ce].ResetCondition,
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

            CEConfigItems[ce.Index].ActionType = ce.ActionType;
            CEConfigItems[ce.Index].ActionParam = ce.ActionParam;
            CEConfigItems[ce.Index].TriggerType = ce.TriggerType;
            CEConfigItems[ce.Index].TriggerParam = ce.TriggerParam;
            CEConfigItems[ce.Index].TriggerParam2 = ce.TriggerParam2;
            CEConfigItems[ce.Index].TriggerCondition = ce.TriggerCondition;
            CEConfigItems[ce.Index].ResetType = ce.ResetType;
            CEConfigItems[ce.Index].ResetParam = ce.ResetParam;
            CEConfigItems[ce.Index].ResetParam2 = ce.ResetParam2;
            CEConfigItems[ce.Index].ResetCondition = ce.ResetCondition;

            TimerEvents[ce.Index] = ce.TimerEventTime;

            return true;
        }

        private void downloadRequestsCompleted() { }

        private void uploadRequestsCompleted() { }
        #endregion
    }
}
