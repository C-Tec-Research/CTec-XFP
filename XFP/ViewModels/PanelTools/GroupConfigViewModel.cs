using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using CTecControls.UI;
using CTecDevices.Protocol;
using CTecUtil.StandardPanelDataTypes;
using CTecUtil.Utils;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;
using static Xfp.DataTypes.PanelData.GroupConfigData;
using Xfp.IO;
using Xfp.UI.Interfaces;

namespace Xfp.ViewModels.PanelTools
{
    public class GroupConfigViewModel : PanelToolsPageViewModelBase, IPanelToolsViewModel, IConfigToolsPageViewModel
    {
        public GroupConfigViewModel(FrameworkElement parent, Grid grid) : base(parent)
        {
            _grid = grid;
        }


        private Grid _grid;

        public bool CurrentProtocolIsCast => DeviceTypes.CurrentProtocolIsXfpCast;

        private ObservableCollection<GroupConfigRowViewModel> _groupConfigItems;
        public ObservableCollection<GroupConfigRowViewModel> GroupConfigItems { get => _groupConfigItems; set { _groupConfigItems = value; OnPropertyChanged(); } }


        public string PanelSounder1GroupDesc { get => string.Format(Cultures.Resources.Panel_Sounder_x_Belongs_To_Sounder_Group, 1); }
        public string PanelSounder2GroupDesc { get => string.Format(Cultures.Resources.Panel_Sounder_x_Belongs_To_Sounder_Group, 2); }
        public int PanelSounder1Group { get => _data.CurrentPanel.GroupConfig.PanelSounder1Group; set { _data.CurrentPanel.GroupConfig.PanelSounder1Group = value; refreshValidators(); } }
        public int PanelSounder2Group { get => _data.CurrentPanel.GroupConfig.PanelSounder2Group; set { _data.CurrentPanel.GroupConfig.PanelSounder2Group = value; refreshValidators(); } }
        public int MinSounderGroup { get => 1; }
        public int MaxSounderGroup { get => GroupConfigData.NumSounderGroups; }
        public int EvacTone  { get => _data?.CurrentPanel.GroupConfig.ContinuousTone ?? 0;   set { if (_data is not null) _data.CurrentPanel.GroupConfig.ContinuousTone = value; refreshValidators(); } }
        public int AlertTone { get => _data?.CurrentPanel.GroupConfig.IntermittentTone ?? 0; set { if (_data is not null) _data.CurrentPanel.GroupConfig.IntermittentTone = value; refreshValidators(); } }
        public bool NewFireCausesResound { get => _data.CurrentPanel.GroupConfig.ReSoundFunction; set { _data.CurrentPanel.GroupConfig.ReSoundFunction = value; OnPropertyChanged(); } }
        public TimeSpan PhasedDelay { get => _data.CurrentPanel.GroupConfig.PhasedDelay; set { _data.CurrentPanel.GroupConfig.PhasedDelay = value; refreshValidators(); } }

        public TimeSpan MaxPhasedDelay => GroupConfigData.MaxPhasedDelay;

        public bool PanelSounder1GroupIsValid => GroupConfigData.IsValidPanelSounderGroup(PanelSounder1Group);
        public bool PanelSounder2GroupIsValid => GroupConfigData.IsValidPanelSounderGroup(PanelSounder2Group);
        public bool EvacToneIsValid           => GroupConfigData.IsValidAlarmTone(EvacTone);
        public bool AlertToneIsValid          => GroupConfigData.IsValidAlarmTone(AlertTone);
        public bool PhasedDelayIsValid        => GroupConfigData.IsValidPhasedDelay(PhasedDelay);

        public string EvacToneDesc
        {
            get => string.Format(Cultures.Resources.Tone_Message_Pair_x_Primary, _data.CurrentPanel.GroupConfig.ContinuousTone);
            set { _data.CurrentPanel.GroupConfig.ContinuousTone = (int)TextUtil.ExtractIntFromFormattedText(value, Cultures.Resources.Tone_Message_Pair_x_Primary) - 1; OnPropertyChanged(); }
        }
        public string AlertToneDesc
        {
            get => string.Format(Cultures.Resources.Tone_Message_Pair_x_Secondary, _data.CurrentPanel.GroupConfig.IntermittentTone);
            set { _data.CurrentPanel.GroupConfig.IntermittentTone = (int)TextUtil.ExtractIntFromFormattedText(value, Cultures.Resources.Tone_Message_Pair_x_Secondary) - 1; OnPropertyChanged(); }
        }
        
        private ObservableCollection<string> _alertTones;
        private ObservableCollection<string> _evacTones;
        public ObservableCollection<string> AlertTones {  get => _alertTones; set { _alertTones = value; OnPropertyChanged(); } }
        public ObservableCollection<string> EvacTones  {  get => _evacTones;  set { _evacTones = value; OnPropertyChanged(); } }

        private double _zoneNameWidth = 40;
        public double ZoneNameWidth { get => _zoneNameWidth;       set { _zoneNameWidth = value; OnPropertyChanged(); } }

        public AlarmTypes AlarmOff   => AlarmTypes.Off;
        public AlarmTypes AlarmAlert => AlarmTypes.Alert;
        public AlarmTypes AlarmEvac  => AlarmTypes.Evacuate;


        //private bool _itemsSelected;
        //public bool ItemsSelected { get => _itemsSelected; set { _itemsSelected = value; OnPropertyChanged(); } }

        private List<GroupConfigItemViewModel> _selectedAlarms = new();
        private GroupConfigItemViewModel _newSelectionStart;


        /// <summary>
        /// Assign the alarms to the buttons in the grid
        /// </summary>
        private void initButtonGrid()
        {
            CTecUtil.UI.UIState.SetBusyState();

            foreach (var c in _grid.Children)
            {
                if (c is ToggleButton b)
                {
                    var x = Grid.GetColumn(b);
                    var y = Grid.GetRow(b);
                    var col = x - ((x - 2) / 6) - 2;
                    var row = y - 1;
                    GroupConfigItems[row].Alarms[col].Button = b;
                }
            }
        }


        internal void InitGrid()
        {
            //adjust the above-grid header widths to match their respective columns
            var widthL = 0.0;

            var col = 0;
            for ( ; col < 2; col++)
                widthL += _grid.ColumnDefinitions[col].ActualWidth;

            ZoneNameWidth = widthL;
        }


        private void initComboLists()
        {
            int aTone = AlertTone;
            int eTone = EvacTone;

            if (AlertTones is null)
            {
                AlertTones = new();
                EvacTones = new();
            }

            if (DeviceTypes.CurrentProtocolIsXfpApollo)
            {
                for (int t = 0; t < GroupConfigData.NumToneMessagePairs; t++)
                {
                    if (t >= AlertTones.Count)
                    {
                        AlertTones.Add(string.Format(Cultures.Resources.Tone_Message_Pair_x_Secondary, t + 1));
                        EvacTones.Add(string.Format(Cultures.Resources.Tone_Message_Pair_x_Primary, t + 1));
                    }
                    else
                    {
                        AlertTones[t] = string.Format(Cultures.Resources.Tone_Message_Pair_x_Secondary, t + 1);
                        EvacTones[t]  = string.Format(Cultures.Resources.Tone_Message_Pair_x_Primary, t + 1);
                    }
                }
            }
            else
            {
                for (int t = 0; t < GroupConfigData.NumToneMessagePairs; t++)
                {
                    var option = string.Format(Cultures.Resources.Tone_Message_M_x, t + 1);

                    if (t >= AlertTones.Count)
                    {
                        AlertTones.Add(option);
                        EvacTones.Add(option);
                    }
                    else
                    {
                        AlertTones[t] = EvacTones[t] = option;
                    }
                }
            }

            for (int t = AlertTones.Count - 1; t >= GroupConfigData.NumToneMessagePairs; t--)
            {
                AlertTones.RemoveAt(t);
                EvacTones.RemoveAt(t);
            }

            OnPropertyChanged(nameof(AlertTones));
            OnPropertyChanged(nameof(EvacTones));

            AlertTone = aTone;
            EvacTone = eTone;
        }


        internal void SetAllTo(AlarmTypes alarmType)
        {
            foreach (var g in GroupConfigItems)
                for (int i = 0; i < g.Alarms.Count; i++)
                    g.Alarms[i].Alarm = alarmType;
        }


        internal void SetSelectionTo(AlarmTypes alarmType)
        {
            foreach (var g in _selectedAlarms)
                g.Alarm = alarmType;
        }


        ///// <summary>
        ///// Update the SelectedItems list to match the DataGrid or ListView's SelectedItems
        ///// </summary>
        //public void ChangeSelection(System.Collections.IList selectedItems)
        //{
        //    //_groupConfigItems = new();
        //    //foreach (var item in selectedItems)
        //    //    _groupConfigItems.Add(item as GroupConfigItemRowViewModel);
        //    //OnPropertyChanged(nameof(GroupConfigItems));
        //}

        
        internal void alarmMouseDown(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleButton;
            var alarm = findAlarm(btn);

            switch (Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift))
            {
                case ModifierKeys.None:
                    
                    var wasChecked = btn.IsChecked == true;

                    //uncheck all buttons & clear selected list
                    foreach (var c in _grid.Children)
                        if (c is ToggleButton t)
                            t.IsChecked = false;

                    _selectedAlarms.Clear();

                    if (wasChecked)
                    {
                        _newSelectionStart = null;
                    }
                    else
                    {
                        //start selection anew with this one
                        _newSelectionStart = alarm;
                        _selectedAlarms.Add(alarm);
                        btn.IsChecked = true;

                    }
                    e.Handled = true;
                    break;

                case ModifierKeys.Control:

                    //add this alarm to the selected items
                    _newSelectionStart = alarm;

                    if (btn.IsChecked == true)
                    {
                        //uncheck this item and remove from list
                        btn.IsChecked = false;
                        if (_selectedAlarms.Contains(alarm))
                            _selectedAlarms.Remove(alarm);
                    }
                    else
                    {
                        btn.IsChecked = true;
                        if (!_selectedAlarms.Contains(alarm))
                            _selectedAlarms.Add(alarm);
                    }
                    e.Handled = true;
                    break;

                case ModifierKeys.Shift:

                    //uncheck all buttons & clear selected list
                    foreach (var c in _grid.Children)
                        if (c is ToggleButton t)
                            t.IsChecked = false;

                    _selectedAlarms.Clear();
                    alarm.IsChecked = true;
                    _selectedAlarms.Add(alarm);

                    selectRange(alarm);

                    e.Handled = true;
                    break;
            }
        }

        internal void alarmMouseEnter(object sender, RoutedEventArgs e)
        {
            if (_newSelectionStart is null)
                return;

            if (e is MouseEventArgs me && me.LeftButton != MouseButtonState.Pressed)
                return;
            
            var btn = sender as ToggleButton;
            var alarm = findAlarm(btn);

            if (alarm != _newSelectionStart)
                selectRange(alarm);
        }


        private GroupConfigItemViewModel findAlarm(ToggleButton button)
        {
            var x = Grid.GetColumn(button);
            var y = Grid.GetRow(button);
            var col = x - ((x - 2) / 6) - 2;
            var row = y - 1;
            return GroupConfigItems[row].Alarms[col];
        }


        private void selectRange(GroupConfigItemViewModel alarm)
        {
            var startRow = Math.Min(alarm.ZoneIndex,  _newSelectionStart.ZoneIndex);
            var startCol = Math.Min(alarm.AlarmIndex, _newSelectionStart.AlarmIndex);
            var endRow   = Math.Max(alarm.ZoneIndex,  _newSelectionStart.ZoneIndex);
            var endCol   = Math.Max(alarm.AlarmIndex, _newSelectionStart.AlarmIndex);

            try
            {
                var itemsToRemove = new List<GroupConfigItemViewModel>();

                foreach (var a in _selectedAlarms)
                {
                    if (a.AlarmIndex < startCol || a.AlarmIndex > endCol || a.ZoneIndex < startRow || a.ZoneIndex > endRow)
                    {
                        a.Button.IsChecked = false;
                        itemsToRemove.Add(a);
                    }
                }

                foreach (var r in itemsToRemove)
                    _selectedAlarms.Remove(r);

                foreach (var r in _groupConfigItems)
                    foreach (var c in r.Alarms)
                        if (c.ZoneIndex >= startRow && c.ZoneIndex <= endRow
                            && c.AlarmIndex >= startCol && c.AlarmIndex <= endCol
                            && !_selectedAlarms.Contains(c))
                            _selectedAlarms.Add(c);

                foreach (var s in _selectedAlarms)
                    s.Button.IsChecked = true;
            }
            catch (InvalidOperationException)
            {
            }
        }


        /// <summary>
        /// Update the data corresponding to the decoupled row/column value.
        /// </summary>
        private void updateValue(int row, int col, AlarmTypes value)
        {
            if (row >= _data.CurrentPanel.ZoneConfig.Zones.Count)
            {
                row -= _data.CurrentPanel.ZoneConfig.Zones.Count;
                _data.CurrentPanel.ZonePanelConfig.Panels[row].SounderGroups[col] = value;
            }
            else
            {
                _data.CurrentPanel.ZoneConfig.Zones[row].SounderGroups[col] = value;
            }
        }


        #region IAppViewModel implementation
        public void SetCulture(CultureInfo culture)
        {
            PageHeader = Cultures.Resources.Nav_Group_Configuration;

            initComboLists();

            CultureChanged?.Invoke(culture);
        }
        #endregion


        #region IPanelToolsViewModel implementation
        public void PopulateView(XfpData data)
        {
            if (data is null)
                return;

            _data = data;
            _groupConfigItems = new();
            
            foreach (var z in _data.CurrentPanel.ZoneConfig.Zones)
                _groupConfigItems.Add(new(z) { ValueChanged = updateValue });

            foreach (var p in _data.CurrentPanel.ZonePanelConfig.Panels)
                _groupConfigItems.Add(new(p) { ValueChanged = updateValue });

            initButtonGrid();
            
            RefreshView();
        }
        
        public void RefreshView()
        {
            if (_data is null)
                return;

            //Validator.IsValid(Parent);

            initComboLists();

            OnPropertyChanged(nameof(CurrentProtocolIsCast));
            OnPropertyChanged(nameof(GroupConfigItems));
            OnPropertyChanged(nameof(IsReadOnly));
            OnPropertyChanged(nameof(PanelSounder1GroupDesc));
            OnPropertyChanged(nameof(PanelSounder2GroupDesc));
            OnPropertyChanged(nameof(PanelSounder1Group));
            OnPropertyChanged(nameof(PanelSounder2Group));
            OnPropertyChanged(nameof(MinSounderGroup));
            OnPropertyChanged(nameof(MaxSounderGroup));
            OnPropertyChanged(nameof(EvacTones));
            OnPropertyChanged(nameof(AlertTones));
            OnPropertyChanged(nameof(EvacTone));
            OnPropertyChanged(nameof(AlertTone));
            OnPropertyChanged(nameof(EvacToneDesc));
            OnPropertyChanged(nameof(AlertToneDesc));
            OnPropertyChanged(nameof(NewFireCausesResound));
            OnPropertyChanged(nameof(PhasedDelay));
            //OnPropertyChanged(nameof(SelectedAlarm));
            //OnPropertyChanged(nameof(AlarmOffSelected));
            //OnPropertyChanged(nameof(AlarmAlertSelected));
            //OnPropertyChanged(nameof(AlarmEvacSelected));

            refreshValidators();

            foreach (var g in GroupConfigItems)
                g.RefreshView();
        }

        private void refreshValidators()
        {
            OnPropertyChanged(nameof(PanelSounder1GroupIsValid));
            OnPropertyChanged(nameof(PanelSounder2GroupIsValid));
            OnPropertyChanged(nameof(EvacToneIsValid));
            OnPropertyChanged(nameof(AlertToneIsValid));
            OnPropertyChanged(nameof(PhasedDelayIsValid));
        }
        #endregion


        #region IConfigToolsPageViewModel implementation
        public void EnqueuePanelDownloadCommands(bool allPages)
        {
            PanelComms.ZoneGroupReceived = zoneGroupReceived;
            PanelComms.PhasedSettingsReceived = phasedSettingsReceived;
            PanelComms.InitNewDownloadCommandSubqueue(Cultures.Resources.Comms_Groups, downloadRequestsCompleted);
            for (int zone = 0; zone < ZoneConfigData.NumZones + ZonePanelConfigData.NumZonePanels; zone++)
                PanelComms.AddCommandRequestZoneGroup(zone, String.Format(Cultures.Resources.Zone_Group_Settings_x, zone + 1));
            if (!allPages)
            {
                PanelComms.ZoneNameReceived = zoneNameReceived;
                for (int zone = 0; zone < ZoneConfigData.NumZones + ZonePanelConfigData.NumZonePanels; zone++)
                    PanelComms.AddCommandRequestZoneName(zone, String.Format(Cultures.Resources.Zone_Name_x, zone + 1));
                PanelComms.AddCommandRequestPhasedSettings(Cultures.Resources.Phased_Settings);
            }
        }

        public void EnqueuePanelUploadCommands(bool allPages)
        {
            PanelComms.InitNewUploadCommandSubqueue(Cultures.Resources.Comms_Groups, uploadRequestsCompleted);
            for (int i = 0; i < ZoneConfigData.NumZones + ZonePanelConfigData.NumZonePanels; i++)
            {
                var alarms = new List<AlarmTypes>();
                foreach (var a in GroupConfigItems[i].Alarms)
                    alarms.Add(a.Alarm);
                PanelComms.AddCommandSetZoneGroup(new() { Index = i,
                                                          Alarms = alarms,
                                                          PanelSounderGroup1 = PanelSounder1Group,
                                                          PanelSounderGroup2 = PanelSounder2Group,
                                                          AlertTone = AlertTone,
                                                          EvacTone = EvacTone,
                                                          NewFireCausesResound = NewFireCausesResound },
                                                  string.Format(Cultures.Resources.Zone_Group_x, i + 1));
            }

            if (!allPages)
                PanelComms.AddCommandSetPhasedSettings(new() { PhasedDelay = _data.CurrentPanel.GroupConfig.PhasedDelay, 
                                                               InputDelay = _data.CurrentPanel.ZoneConfig.InputDelay, 
                                                               InvestigationPeriod = _data.CurrentPanel.ZoneConfig.InvestigationPeriod },
                                                       Cultures.Resources.Phased_Settings);
        }


        public bool DataEquals(XfpData otherData) => _data.CurrentPanel.GroupConfig.Equals(otherData.CurrentPanel.GroupConfig);


        public bool HasErrorsOrWarnings() => _data.CurrentPanel.GroupConfig.HasErrorsOrWarnings();
        public bool HasErrors()           => _data.CurrentPanel.GroupConfig.HasErrors();
        public bool HasWarnings()         => _data.CurrentPanel.GroupConfig.HasWarnings();
        #endregion


        #region Panel comms receive-data handlers
        private bool zoneNameReceived(object data)
        {
            if (data is not IndexedText zone
             || zone.Index < 0 
             || zone.Index > ZoneConfigData.NumZones + ZonePanelConfigData.NumZonePanels)
                return false;

            CTecUtil.CommsLog.AddReceivedData(string.Format(Cultures.Resources.Zone_x_Name_y, zone.Index + 1, zone.Value));

            if (zone.Index < ZoneConfigData.NumZones)
                _data.CurrentPanel.ZoneConfig.Zones[zone.Index].Name = zone.Value;
            else if (zone.Index < ZoneConfigData.NumZones + ZonePanelConfigData.NumZonePanels)
                _data.CurrentPanel.ZonePanelConfig.Panels[zone.Index - ZoneConfigData.NumZones].Name = zone.Value;

            return true;
        }
        
        private bool zoneGroupReceived(object data)
        {
            if (data is not GroupBundle zone)
                return false;

            CTecUtil.CommsLog.AddReceivedData(string.Format(Cultures.Resources.Zone_Group_Settings_x, zone.Index + 1));

            for (int i = 0; i < NumSounderGroups; i++)
                GroupConfigItems[zone.Index].Alarms[i].Alarm = zone.Alarms[i];

            PanelSounder1Group   = zone.PanelSounderGroup1;
            PanelSounder2Group   = zone.PanelSounderGroup2;
            AlertTone            = zone.AlertTone;
            EvacTone             = zone.EvacTone;
            NewFireCausesResound = zone.NewFireCausesResound;

            return true;
        }

        private bool phasedSettingsReceived(object data)
        {
            if (data is not ZoneConfigData.PhasedSettingsBundle zone)
                return false;

            CTecUtil.CommsLog.AddReceivedData(Cultures.Resources.Group_Phased_Settings);

            //Note: 0xffff denotes zero phased delay
            PhasedDelay        = zone.PhasedDelay.TotalSeconds == 0xffff ? TimeOfDay.Midnight : zone.PhasedDelay;
            _data.CurrentPanel.ZoneConfig.InputDelay          = zone.InputDelay;
            _data.CurrentPanel.ZoneConfig.InvestigationPeriod = zone.InvestigationPeriod;
            return true;
        }

        private void downloadRequestsCompleted() { }

        private void uploadRequestsCompleted() { }
        #endregion
    }
}
