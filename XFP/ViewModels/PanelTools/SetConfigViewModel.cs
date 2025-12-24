using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using CTecControls.UI;
using CTecUtil.StandardPanelDataTypes;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;
using Xfp.IO;
using Xfp.UI.Interfaces;

namespace Xfp.ViewModels.PanelTools
{
    public class SetConfigViewModel : PanelToolsPageViewModelBase, IPanelToolsViewModel, IConfigToolsPageViewModel
    {
        public SetConfigViewModel(FrameworkElement parent, Grid grid, StackPanel headerRow1, TextBlock headerRow2) : base(parent)
        {
            _grid = grid;
            _headerRow1 = headerRow1;
            _headerRow2 = headerRow2;
        }


        private Grid _grid;
        private StackPanel _headerRow1;
        private TextBlock _headerRow2;



        private ObservableCollection<SetConfigRowViewModel> _setConfigItems;
        public ObservableCollection<SetConfigRowViewModel> SetConfigItems { get => _setConfigItems; set { _setConfigItems = value; OnPropertyChanged(); } }
        public List<bool> OutputSetIsSilenceable  { get => _data.CurrentPanel.ZoneConfig.OutputSetIsSilenceable;  set { _data.CurrentPanel.ZoneConfig.OutputSetIsSilenceable = value; OnPropertyChanged(); } }
        public List<bool> PanelRelayIsSilenceable { get => _data.CurrentPanel.ZoneConfig.PanelRelayIsSilenceable; set { _data.CurrentPanel.ZoneConfig.PanelRelayIsSilenceable = value; OnPropertyChanged(); } }
        
        public TimeSpan DelayTimer { get => _data.CurrentPanel.SetConfig.DelayTimer; set { _data.CurrentPanel.SetConfig.DelayTimer = value; OnPropertyChanged(); OnPropertyChanged(nameof(DelayTimerIsValid)); } }

        public bool DelayTimerIsValid => ZoneConfigData.IsValidSetDelay(_data.CurrentPanel.SetConfig.DelayTimer);


        private double _headerLeftWidth = 30;
        private double _gridHeaderPanelHeight = 70;
        private bool   _pulseAllowed = true;

        public double HeaderLeftWidth       { get => _headerLeftWidth;       set { _headerLeftWidth = value; OnPropertyChanged(); } }
        public double GridHeaderPanelHeight { get => _gridHeaderPanelHeight; set { _gridHeaderPanelHeight = value; OnPropertyChanged(); } }

        public bool PulseAllowed { get => _pulseAllowed; set { _pulseAllowed = value; OnPropertyChanged(); } }


        public SetTriggerTypes TriggerNotTriggered => SetTriggerTypes.NotTriggered;
        public SetTriggerTypes TriggerPulsed       => SetTriggerTypes.Pulsed;
        public SetTriggerTypes TriggerContinuous   => SetTriggerTypes.Continuous;
        public SetTriggerTypes TriggerDelayed      => SetTriggerTypes.Delayed;


        private bool _itemsSelected;
        public bool ItemsSelected { get => _itemsSelected; set { _itemsSelected = value; OnPropertyChanged(); } }

        private List<SetConfigItemViewModel> _selectedTriggers = new();
        private SetConfigItemViewModel _newSelectionStart;


        /// <summary>
        /// Assign the triggers to the buttons in the grid
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
                    try
                    {
                        if (col < SetConfigData.NumOutputSetTriggers)
                            SetConfigItems[row].OutputSetTriggers[col].Button = b;
                        else
                            SetConfigItems[row].PanelRelayTriggers[(col - 1) % SetConfigData.NumOutputSetTriggers].Button = b;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }


        /// <summary>
        /// Adjust the above-grid header widths to match their respective columns
        /// </summary>
        internal void InitGrid()
        {
            var widthL = 0.0;

            var col = 0;
            for ( ; col < 2; col++)
                widthL += _grid.ColumnDefinitions[col].ActualWidth;

            HeaderLeftWidth = widthL;
            GridHeaderPanelHeight = _headerRow1.ActualHeight + _headerRow2.ActualHeight;
        }


        internal void SetAllTo(SetTriggerTypes triggerType)
        {
            foreach (var g in SetConfigItems)
            {
                for (int i = 0; i < g.OutputSetTriggers.Count; i++)
                    g.OutputSetTriggers[i].Trigger = triggerType;
                for (int i = 0; i < g.PanelRelayTriggers.Count; i++)
                    g.PanelRelayTriggers[i].Trigger = triggerType;
            }
        }


        internal void SetSelectionTo(SetTriggerTypes triggerType)
        {
            foreach (var g in _selectedTriggers)
                //if (triggerType != SetTriggerTypes.Pulsed || g.IsPanelRelayTrigger)
                g.Trigger = triggerType;
        }


        internal void triggerMouseDown(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleButton;
            var alarm = findTrigger(btn);

            switch (Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift))
            {
                case ModifierKeys.None:
                    
                    var wasChecked = btn.IsChecked == true;

                    //uncheck all buttons & clear selected list
                    foreach (var c in _grid.Children)
                        if (c is ToggleButton t)
                            t.IsChecked = false;

                    _selectedTriggers.Clear();

                    if (wasChecked)
                    {
                        _newSelectionStart = null;
                    }
                    else
                    {
                        //start selection anew with this one
                        _newSelectionStart = alarm;
                        _selectedTriggers.Add(alarm);
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
                        if (_selectedTriggers.Contains(alarm))
                            _selectedTriggers.Remove(alarm);
                    }
                    else
                    {
                        btn.IsChecked = true;
                        if (!_selectedTriggers.Contains(alarm))
                            _selectedTriggers.Add(alarm);
                    }
                    e.Handled = true;
                    break;

                case ModifierKeys.Shift:

                    //uncheck all buttons & clear selected list
                    foreach (var c in _grid.Children)
                        if (c is ToggleButton t)
                            t.IsChecked = false;

                    _selectedTriggers.Clear();
                    alarm.IsChecked = true;
                    _selectedTriggers.Add(alarm);

                    selectRange(alarm);

                    e.Handled = true;
                    break;
            }
        }


        internal void alarmSelectAll(DataGrid grid)
        {
            foreach (var s in SetConfigItems)
            {
                foreach (var a in s.OutputSetTriggers)
                    a.Button.IsChecked = true;
                foreach (var a in s.PanelRelayTriggers)
                    a.Button.IsChecked = true;
            }
        }



        internal void triggerMouseEnter(object sender, RoutedEventArgs e)
        {
            if (_newSelectionStart is null)
                return;

            if (e is MouseEventArgs me && me.LeftButton != MouseButtonState.Pressed)
                return;
            
            var btn = sender as ToggleButton;
            var alarm = findTrigger(btn);

            if (alarm != _newSelectionStart)
                selectRange(alarm);
        }


        private SetConfigItemViewModel findTrigger(ToggleButton button)
        {
            var x = Grid.GetColumn(button);
            var y = Grid.GetRow(button);
            var col = x - ((x - 2) / 6) - 2;
            var row = y - 1;
            return col < SetConfigData.NumOutputSetTriggers ? SetConfigItems[row].OutputSetTriggers[col] 
                                                            : SetConfigItems[row].PanelRelayTriggers[col - SetConfigData.NumOutputSetTriggers - 1];
        }

        private void selectRange(SetConfigItemViewModel trigger)
        {
            var triggerIndex = getTriggerIndex(trigger);
            var startRow = Math.Min(trigger.ZoneIndex, _newSelectionStart.ZoneIndex);
            var startCol = Math.Min(triggerIndex,      _newSelectionStart.TriggerIndex + (_newSelectionStart.IsPanelRelayTrigger ? SetConfigData.NumOutputSetTriggers : 0));
            var endRow   = Math.Max(trigger.ZoneIndex, _newSelectionStart.ZoneIndex);
            var endCol   = Math.Max(triggerIndex,      _newSelectionStart.TriggerIndex + (_newSelectionStart.IsPanelRelayTrigger ? SetConfigData.NumOutputSetTriggers : 0));

            try
            {
                var itemsToRemove = new List<SetConfigItemViewModel>();

                foreach (var a in _selectedTriggers)
                {
                    var aTriggerIndex = getTriggerIndex(a);
                    if (aTriggerIndex < startCol || aTriggerIndex > endCol || a.ZoneIndex < startRow || a.ZoneIndex > endRow)
                    {
                        a.Button.IsChecked = false;
                        itemsToRemove.Add(a);
                    }
                }

                foreach (var r in itemsToRemove)
                    _selectedTriggers.Remove(r);

                foreach (var r in _setConfigItems)
                {
                    foreach (var t in r.OutputSetTriggers)
                    {
                        var tTriggerIndex = getTriggerIndex(t);
                        if (t.ZoneIndex >= startRow && t.ZoneIndex <= endRow
                         && tTriggerIndex >= startCol && tTriggerIndex <= endCol
                         && !_selectedTriggers.Contains(t))
                            _selectedTriggers.Add(t);
                    }

                    foreach (var t in r.PanelRelayTriggers)
                    {
                        var tTriggerIndex = getTriggerIndex(t);
                        if (t.ZoneIndex >= startRow && t.ZoneIndex <= endRow
                         && tTriggerIndex >= startCol && tTriggerIndex <= endCol
                         && !_selectedTriggers.Contains(t))
                            _selectedTriggers.Add(t);
                    }
                }

                foreach (var s in _selectedTriggers)
                    s.Button.IsChecked = true;
            }
            catch (InvalidOperationException)
            {
            }
        }


        private int getTriggerIndex(SetConfigItemViewModel trigger) => trigger.IsPanelRelayTrigger ? trigger.TriggerIndex + SetConfigData.NumOutputSetTriggers : trigger.TriggerIndex;


        /// <summary>
        /// Update the data corresponding to the decoupled row/column value.
        /// </summary>
        private void updateValue(int row, int col, bool isPanelRelay, SetTriggerTypes value)
        {
            if (row >= _data.CurrentPanel.ZoneConfig.Zones.Count)
            {
                row -= _data.CurrentPanel.ZoneConfig.Zones.Count;

                if (isPanelRelay)
                    _data.CurrentPanel.SetConfig.Sets[row].PanelRelayTriggers[col] = value;
                else
                    _data.CurrentPanel.SetConfig.Sets[row].OutputSetTriggers[col] = value;
            }
            else
            {
                if (isPanelRelay)
                    _data.CurrentPanel.SetConfig.Sets[row].PanelRelayTriggers[col] = value;
                else
                    _data.CurrentPanel.SetConfig.Sets[row].OutputSetTriggers[col] = value;
            }
        }


        #region IAppViewModel implementation
        public void SetCulture(CultureInfo culture)
        {
            PageHeader = Cultures.Resources.Nav_Set_Configuration;

            if (_setConfigItems is not null)
                foreach (var s in _setConfigItems)
                    s.SetCulture(culture);

            InitGrid();

            CultureChanged?.Invoke(culture);
        }
        #endregion


        #region IPanelToolsViewModel implementation
        public void PopulateView(XfpData data)
        {
            if (data is null)
                return;

            _data = data;

            _setConfigItems = new();

            for (int i = 0; i < ZoneConfigData.NumZones; i++)
                _setConfigItems.Add(new(_data.CurrentPanel.ZoneConfig.Zones[i]) { SetData = _data.CurrentPanel.SetConfig.Sets[i], ValueChanged = updateValue });
            for (int i = 0; i < ZoneConfigData.NumPanels; i++)
                _setConfigItems.Add(new(_data.CurrentPanel.ZonePanelConfig.Panels[i]) { SetData = _data.CurrentPanel.SetConfig.Sets[i + ZoneConfigData.NumZones], ValueChanged = updateValue });

            RefreshView();
        }
        
        public void RefreshView()
        {
            if (_data is null)
                return;

            initButtonGrid();

            Validator.IsValid(Parent);

            OnPropertyChanged(nameof(IsReadOnly));
            OnPropertyChanged(nameof(DelayTimer));
            OnPropertyChanged(nameof(DelayTimerIsValid));
            OnPropertyChanged(nameof(SetConfigItems));
            OnPropertyChanged(nameof(HeaderLeftWidth));
            OnPropertyChanged(nameof(OutputSetIsSilenceable));
            OnPropertyChanged(nameof(PanelRelayIsSilenceable));
        }
        #endregion


        #region IConfigToolsPageViewModel implementation
        public void EnqueuePanelDownloadCommands(bool allPages)
        {
            PanelComms.ZoneSetReceived = zoneSetReceived;
            PanelComms.InitNewDownloadCommandSubqueue(Cultures.Resources.Comms_Sets, downloadRequestsCompleted);
            for (int zone = 0; zone <= ZoneConfigData.NumZones + ZonePanelConfigData.NumZonePanels; zone++)
                PanelComms.AddCommandRequestZoneSet(zone, String.Format(Cultures.Resources.Zone_Set_x, zone + 1));
            if (!allPages)
            {
                PanelComms.ZoneNameReceived = zoneNameReceived;
                for (int zone = 0; zone < ZoneConfigData.NumZones + ZonePanelConfigData.NumZonePanels; zone++)
                    PanelComms.AddCommandRequestZoneName(zone, String.Format(Cultures.Resources.Zone_Name_x, zone + 1));
            }
        }

        public void EnqueuePanelUploadCommands(bool allPages)
        {
            PanelComms.InitNewUploadCommandSubqueue(Cultures.Resources.Comms_Sets, uploadRequestsCompleted);

            for (int zone = 0; zone <= ZoneConfigData.NumZones + NetworkConfigData.NumPanelSettings; zone++)
            {
                var setTriggers   = new List<SetTriggerTypes>();
                var relayTriggers = new List<SetTriggerTypes> ();

                if (zone == 0)
                {
                    //Silenceable flags are in set 0
                    for (int i = 0; i < SetConfigData.NumOutputSetTriggers; i++)
                        setTriggers.Add(_data.CurrentPanel.ZoneConfig.OutputSetIsSilenceable[i] ? SetTriggerTypes.Pulsed : SetTriggerTypes.NotTriggered);
                    for (int i = 0; i < SetConfigData.NumPanelRelayTriggers; i++)
                        relayTriggers.Add(_data.CurrentPanel.ZoneConfig.PanelRelayIsSilenceable[i] ? SetTriggerTypes.Pulsed : SetTriggerTypes.NotTriggered);
                }
                else
                {
                    for (int i = 0; i < SetConfigData.NumOutputSetTriggers; i++)
                        setTriggers.Add(SetConfigItems[zone - 1].OutputSetTriggers[i].Trigger);
                    for (int i = 0; i < SetConfigData.NumPanelRelayTriggers; i++)
                        relayTriggers.Add(SetConfigItems[zone - 1].PanelRelayTriggers[i].Trigger);
                }

                PanelComms.AddCommandSetZoneSet(new() { Index = zone,
                                                        OutputSetTriggers = setTriggers,
                                                        PanelRelayTriggers = relayTriggers,
                                                        DelayTimer = _data.CurrentPanel.SetConfig.DelayTimer },
                                                string.Format(Cultures.Resources.Set_x, zone + 1));
            }
        }
        

        public bool DataEquals(XfpData otherData) => _data.CurrentPanel.SetConfig.Equals(otherData.CurrentPanel.SetConfig);


        public bool HasErrorsOrWarnings() => false;//_data.SetConfig.HasErrorsOrWarnings();
        public bool HasErrors()           => false;//_data.SetConfig.HasErrors();
        public bool HasWarnings()         => false;//_data.SetConfig.HasWarnings();
        #endregion


        #region Panel comms receive-data handlers
        private bool zoneNameReceived(object data)
        {
            if (data is not IndexedText zone
             || zone.Index < 0 
             || zone.Index >= ZoneConfigData.NumZones + ZonePanelConfigData.NumZonePanels)
                return false;

            CTecUtil.CommsLog.AddReceivedData(string.Format(Cultures.Resources.Zone_Name_x, zone.Value));

            if (zone.Index < ZoneConfigData.NumZones)
                _data.CurrentPanel.ZoneConfig.Zones[zone.Index].Name = zone.Value;
            else if (zone.Index < ZoneConfigData.NumZones + ZonePanelConfigData.NumZonePanels)
                _data.CurrentPanel.ZonePanelConfig.Panels[zone.Index - ZoneConfigData.NumZones].Name = zone.Value;

            return true;
        }
        
        private bool zoneSetReceived(object data)
        {
            if (data is not SetConfigData.SetBundle set)
                return false;

            CTecUtil.CommsLog.AddReceivedData(string.Format(Cultures.Resources.Set_x, set.Index + 1));

            //data in set 0 is the Silenceable flags
            if (set.Index == 0)
            {
                for (int i = 0; i < SetConfigData.NumOutputSetTriggers; i++)
                    OutputSetIsSilenceable[i] = set.OutputSetTriggers[i] != SetTriggerTypes.NotTriggered;

                for (int i = 0; i < SetConfigData.NumPanelRelayTriggers; i++)
                    PanelRelayIsSilenceable[i] = set.PanelRelayTriggers[i] != SetTriggerTypes.NotTriggered;

                OnPropertyChanged(nameof(OutputSetIsSilenceable));
                OnPropertyChanged(nameof(PanelRelayIsSilenceable));
            }
            else
            {
                var offset = set.Index - 1;

                for (int i = 0; i < SetConfigData.NumOutputSetTriggers; i++)
                    _data.CurrentPanel.SetConfig.Sets[offset].OutputSetTriggers[i] = set.OutputSetTriggers[i];

                for (int i = 0; i < SetConfigData.NumPanelRelayTriggers; i++)
                    _data.CurrentPanel.SetConfig.Sets[offset].PanelRelayTriggers[i] = set.PanelRelayTriggers[i];
            }

            DelayTimer = set.DelayTimer;
            return true;
        }

        private void downloadRequestsCompleted() { }

        private void uploadRequestsCompleted() { }
        #endregion
    }
}
