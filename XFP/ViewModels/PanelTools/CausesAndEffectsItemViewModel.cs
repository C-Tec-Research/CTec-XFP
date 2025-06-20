﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Design;
using System.Windows.Input;
using System.Windows.Media;
using CTecUtil.ViewModels;
using CTecControls.UI;
using CTecControls.ViewModels;
using CTecDevices.Protocol;
using Newtonsoft.Json.Linq;
using Windows.Security.Authentication.Web.Core;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;
using Xfp.UI.Interfaces;
using Xfp.UI.Views.PanelTools;
using static Xfp.DataTypes.PanelData.CEConfigData;

namespace Xfp.ViewModels.PanelTools
{
    class CausesAndEffectsItemViewModel : ViewModelBase, IPanelToolsViewModel
    {
        public CausesAndEffectsItemViewModel(CEEvent data, int index)
        {
            _number = index + 1;
            _data = data;
            InitFixedComboLists();
            RefreshView();
            
            //timer to keep the Times menu up-to-date
            _timesTimer.Elapsed += new((s, e) => OnPropertyChanged(nameof(Times)));
        }

        private enum CEValue { ActionType, ActionParam, TriggerType, TriggerParam, TriggerCondition, ResetType, ResetParam, ResetCondition };
        

        protected static System.Timers.Timer _timesTimer = new() { AutoReset = true, Enabled = true, Interval = 2000 };


        private int     _number;
        private CEEvent _data;

        public CEEvent Data { get => _data; set { _data = value; /*initData();*/ RefreshComboLists(); RefreshView(); } }


        public int CENumber => _number;

        public int SelectedActionTypeIndex
        {
            get => Array.IndexOf(Enum.GetValues(_data.ActionType.GetType()), _data.ActionType);
            set
            {
                if (value != SelectedActionTypeIndex)
                {
                    //var prev = _data.ActionType;
                    if (value < 0) value = 0;
                    _data.ActionType = (CEActionTypes)(Enum.GetValues(_data.ActionType.GetType())).GetValue(value);

                    //if (_data.ActionType != prev)
                    clearDependants(CEValue.ActionType);
                    refreshView(CEValue.ActionType);
                }
            }
        }

        public int SelectedActionParamIndex
        {
            get => _data.ActionParam + 1;
            set
            {
                if (value != SelectedActionParamIndex)
                {
                    if (value < 0) value = 0;
                    //var prev = _data.ActionParam;
                    _data.ActionParam = value - 1;

                    //if (_data.ActionParam != prev)
                        clearDependants(CEValue.ActionParam);
                    refreshView(CEValue.ActionParam);
                }
            }
        }

        public int SelectedTriggerTypeIndex
        {
            get => Array.IndexOf(Enum.GetValues(_data.TriggerType.GetType()), _data.TriggerType);
            set
            {
                if (value != SelectedTriggerTypeIndex)
                {
                    if (value < 0) value = 0;
                    //var prev = _data.TriggerType;
                    _data.TriggerType = (CETriggerTypes)(Enum.GetValues(_data.TriggerType.GetType())).GetValue(value);

                    //if (_data.TriggerType != prev)
                        clearDependants(CEValue.TriggerType);
                    refreshView(CEValue.TriggerType);
                }
            }
        }

        public int SelectedTriggerParamIndex
        {
            get => _data.TriggerParam + 1;
            set
            {
                if (value != SelectedTriggerParamIndex)
                {
                    if (value < 0) value = 0;
                    //var prev = _data.TriggerParam;
                    _data.TriggerParam = value - 1;

                    //if (_data.TriggerParam != prev)
                        clearDependants(CEValue.TriggerParam);
                    refreshView(CEValue.TriggerParam);
                }
            }
        }

        public int SelectedTriggerParam2Index
        {
            get => _data.TriggerParam2 + 1;
            set
            {
                if (value != SelectedTriggerParam2Index)
                {
                    if (value < 0) value = 0;
                    //var prev = _data.TriggerParam2;
                    _data.TriggerParam2 = value - 1;

                    //if (_data.TriggerParam2 != prev)
                        clearDependants(CEValue.TriggerParam);
                    refreshView(CEValue.TriggerParam);
                }
            }
        }

        public int SelectedTriggerConditionIndex
        {
            get => _data.TriggerCondition switch { true => 1, false => 2, };
            set
            {
                if (value != SelectedTriggerConditionIndex)
                {
                    if (value < 0) value = 0;
                    //var prev = _data.TriggerCondition;
                    _data.TriggerCondition = value == 1;

                    //if (_data.TriggerCondition != prev)
                        clearDependants(CEValue.TriggerCondition);
                    refreshView(CEValue.TriggerCondition);
                }
            }
        }

        public int SelectedResetTypeIndex
        {
            get => Array.IndexOf(Enum.GetValues(_data.ResetType.GetType()), _data.ResetType);
            set
            {
                if (value != SelectedResetTypeIndex)
                {
                    if (value < 0) value = 0;
                    //var prev = _data.ResetType;
                    _data.ResetType = (CETriggerTypes)(Enum.GetValues(_data.ResetType.GetType())).GetValue(value);

                    //if (_data.ResetType != prev)
                        clearDependants(CEValue.ResetType);
                    refreshView(CEValue.ResetType);
                }
            }
        }

        public int SelectedResetParamIndex
        {
            get => _data.ResetParam + 1;
            set
            {
                if (value != SelectedResetParamIndex)
                {
                    if (value < 0) value = 0;
                    //var prev = _data.ResetParam;
                    _data.ResetParam = _data.ResetParam = value - 1;

                    //if (_data.ResetParam != prev)
                        clearDependants(CEValue.ResetParam);
                    refreshView(CEValue.ResetParam);
                }
            }
        }

        public int SelectedResetParam2Index
        {
            get => _data.ResetParam2 + 1;
            set
            {
                if (value != SelectedResetParam2Index)
                {
                    if (value < 0) value = 0;
                    //var prev = _data.ResetParam2;
                    _data.ResetParam2 = value - 1;

                    //if (_data.ResetParam2 != prev)
                        clearDependants(CEValue.ResetParam);
                    refreshView(CEValue.ResetParam);
                }
            }
        }

        public int SelectedResetConditionIndex
        {
            get => _data.ResetCondition switch { true => 1, false => 2, };
            set
            {
                if (value != SelectedResetConditionIndex)
                {
                    if (value < 0) value = 0;
                    //var prev = _data.ResetCondition;
                    _data.ResetCondition = value == 1;

                    refreshView(CEValue.ResetCondition);
                }
            }
        }


        private void clearDependants(CEValue level)
        {
            switch (level)
            {

                case CEValue.ActionType:       SelectedActionParamIndex = 0; break;
                case CEValue.ActionParam:      SelectedTriggerTypeIndex = 0; break;
                case CEValue.TriggerType:      SelectedTriggerParamIndex = SelectedTriggerParam2Index = 0; break;
                case CEValue.TriggerParam:     SelectedTriggerConditionIndex = 0; break;
                case CEValue.TriggerCondition: SelectedResetTypeIndex = 0; break;
                case CEValue.ResetType:        SelectedResetParamIndex = SelectedResetParam2Index = 0; break;
                case CEValue.ResetParam:       SelectedResetConditionIndex = 0; break;
            }
        }


        #region option visibilities

        #region action
        public bool ShowActionParam         => !_data.NoActionParam(_data.ActionType);
        public bool ShowActionLoop1Devices  => _data.ActionType switch { CEActionTypes.TriggerLoop1Device or CEActionTypes.Loop1DeviceDisable => true, _ => false };
        public bool ShowActionLoop2Devices  => _data.ActionType switch { CEActionTypes.TriggerLoop2Device or CEActionTypes.Loop2DeviceDisable => true, _ => false };
        public bool ShowActionRelays        => _data.ActionType switch { CEActionTypes.PanelRelay => true, _ => false };
        public bool ShowActionGroups        => _data.ActionType switch { CEActionTypes.SounderAlert or CEActionTypes.SounderEvac or CEActionTypes.GroupDisable or CEActionTypes.TriggerBeacons => true, _ => false };
        public bool ShowActionZones         => _data.ActionType switch { CEActionTypes.ZoneDisable or CEActionTypes.PutZoneIntoAlarm => true, _ => false };
        public bool ShowActionSets          => _data.ActionType switch { CEActionTypes.TriggerOutputSet => true, _ => false };
        public bool ShowActionSetsAndRelays => _data.ActionType switch { CEActionTypes.OutputDisable => true, _ => false };
        public bool ShowActionEvents        => _data.ActionType switch { CEActionTypes.TriggerNetworkEvent => true, _ => false };
        #endregion

        #region trigger
        public bool ShowTriggerType           => _data.ActionType != CEActionTypes.None && (!ShowActionParam || SelectedActionParamIndex > -1);
        public bool ShowTriggerParam          => ShowTriggerType && !ShowTriggerParam2 && !_data.NoTriggerParam(_data.TriggerType);
        public bool ShowTriggerParam2         => ShowTriggerType && (_data.TriggerType == CETriggerTypes.EventAnd || _data.TriggerType == CETriggerTypes.ZoneAnd);
        public bool ShowTriggerLoop1Devices   => ShowTriggerParam && _data.TriggerType switch { CETriggerTypes.Loop1DeviceTriggered or CETriggerTypes.Loop1DevicePrealarm => true, _ => false };
        public bool ShowTriggerLoop2Devices   => ShowTriggerParam && _data.TriggerType switch { CETriggerTypes.Loop2DeviceTriggered or CETriggerTypes.Loop2DevicePrealarm => true, _ => false };
        public bool ShowTriggerZones          => ShowTriggerParam && _data.TriggerType switch { CETriggerTypes.ZoneAnd => true, _ => false };
        public bool ShowTriggerZones2         => ShowTriggerParam2 && _data.TriggerType switch { CETriggerTypes.ZoneAnd => true, _ => false };
        public bool ShowTriggerZonesAndPanels => ShowTriggerParam && _data.TriggerType switch { CETriggerTypes.ZoneOrPanelInFire or CETriggerTypes.ZoneHasDeviceInAlarm => true, _ => false };
        public bool ShowTriggerEvents         => ShowTriggerParam && _data.TriggerType switch { CETriggerTypes.OtherEventTriggered or CETriggerTypes.EventAnd => true, _ => false };
        public bool ShowTriggerEvents2        => ShowTriggerParam2 && _data.TriggerType switch { CETriggerTypes.EventAnd => true, _ => false };
        public bool ShowTriggerTimes          => ShowTriggerParam && _data.TriggerType switch { CETriggerTypes.TimerEventTn => true, _ => false };
        public bool ShowTriggerInputs         => ShowTriggerParam && _data.TriggerType switch { CETriggerTypes.PanelInput => true, _ => false };
        public bool ShowTriggerCondition      => (ShowTriggerParam && _data.TriggerParam > 0) 
                                                || (ShowTriggerParam2 && SelectedTriggerParamIndex > 0 && SelectedTriggerParam2Index > 0 ||
                                                    _data.NoTriggerParam(_data.TriggerType) && _data.TriggerType != CETriggerTypes.None);
        #endregion

        #region reset
        public bool ShowResetType           => ShowTriggerCondition;
        public bool ShowResetParam          => ShowResetType && !ShowResetParam2 && !_data.NoTriggerParam(_data.ResetType);
        public bool ShowResetParam2         => ShowResetType && (_data.ResetType == CETriggerTypes.EventAnd || _data.ResetType == CETriggerTypes.ZoneAnd);
        public bool ShowResetLoop1Devices   => ShowResetParam && _data.ResetType switch { CETriggerTypes.Loop1DeviceTriggered or CETriggerTypes.Loop1DevicePrealarm => true, _ => false };
        public bool ShowResetLoop2Devices   => ShowResetParam && _data.ResetType switch { CETriggerTypes.Loop2DeviceTriggered or CETriggerTypes.Loop2DevicePrealarm => true, _ => false };
        public bool ShowResetZones          => ShowResetParam && _data.ResetType switch { CETriggerTypes.ZoneAnd => true, _ => false };
        public bool ShowResetZones2         => ShowResetParam2 && _data.ResetType switch { CETriggerTypes.ZoneAnd => true, _ => false };
        public bool ShowResetZonesAndPanels => ShowResetParam && _data.ResetType switch { CETriggerTypes.ZoneOrPanelInFire or CETriggerTypes.ZoneHasDeviceInAlarm => true, _ => false };
        public bool ShowResetEvents         => ShowResetParam && _data.ResetType switch { CETriggerTypes.OtherEventTriggered or CETriggerTypes.EventAnd => true, _ => false };
        public bool ShowResetEvents2        => ShowResetParam2 && _data.ResetType switch { CETriggerTypes.EventAnd => true, _ => false };
        public bool ShowResetTimes          => ShowResetParam && _data.ResetType switch { CETriggerTypes.TimerEventTn => true, _ => false };
        public bool ShowResetInputs         => ShowResetParam && _data.ResetType switch { CETriggerTypes.PanelInput => true, _ => false };
        public bool ShowResetCondition      => ShowResetParam && _data.ResetParam > 0 || ShowResetParam2 && _data.ResetParam > 0 && _data.ResetParam2 > 0 || _data.NoTriggerParam(_data.ResetType) && _data.ResetType != CETriggerTypes.None;
        #endregion

        #endregion

        #region option validation
        public bool ActionParamIsValid                   => !ShowActionParam || SelectedActionParamIndex > 0 || _data.ActionType == CEActionTypes.None;

        public bool TriggerTypeIsValid                   => !ShowTriggerType || SelectedTriggerTypeIndex > 0;
        public bool TriggerParam1IsValid                 => !ShowTriggerParam || SelectedTriggerParamIndex > 0 || _data.TriggerType == CETriggerTypes.None;
        public bool IndicateInvalidANDTriggerParams      => !(!ShowTriggerParam2 || !(!ShowTriggerParam2 || SelectedTriggerParamIndex < 0 && SelectedTriggerParam2Index < 0));
        public bool IndicateInvalidSelectedTriggerParam1 => !IndicateInvalidANDTriggerParams && (ShowTriggerEvents2 || ShowTriggerZones2) && SelectedTriggerParamIndex < 0;
        public bool IndicateInvalidSelectedTriggerParam2 => !IndicateInvalidANDTriggerParams && (!ShowTriggerEvents2 || !ShowTriggerZones2) && SelectedTriggerParam2Index < 0;
        public bool TriggerConditionIsValid              => !ShowTriggerCondition || SelectedTriggerConditionIndex > 0;

        public bool ResetTypeIsValid                     => !ShowResetType || SelectedResetTypeIndex > 0;
        public bool ResetParam1IsValid                   => !ShowResetParam || SelectedResetParamIndex > 0 || _data.ResetType == CETriggerTypes.None;
        public bool IndicateInvalidANDResetParams        => !(!ShowResetParam2 || !(!ShowResetParam2 || SelectedResetParamIndex < 0 && SelectedResetParam2Index < 0));
        public bool IndicateInvalidSelectedResetParam1   => !IndicateInvalidANDResetParams && (ShowResetEvents2 || ShowResetZones2) && SelectedResetParamIndex < 0;
        public bool IndicateInvalidSelectedResetParam2   => !IndicateInvalidANDResetParams && (!ShowResetEvents2 || !ShowResetZones2) && SelectedResetParam2Index < 0;
        public bool ResetConditionIsValid                => !ShowResetCondition || SelectedResetConditionIndex > 0;
        #endregion


        #region comboboxes
        /// <summary>
        /// Delegate to get combo string list.  This makes loading quicker as we can have a single instance 
        /// of each list in the page viewmodel instead of each line maintaining their own copies.
        /// </summary>
        public delegate List<string> MenuList();

        public MenuList ActionsMenu;
        public MenuList TriggersMenu;
        public MenuList InputsMenu;
        public MenuList Loop1DevicesMenu;
        public MenuList Loop2DevicesMenu;
        public MenuList ZonesMenu;
        public MenuList ZonesPanelsMenu;
        public MenuList GroupsMenu;
        public MenuList SetsMenu;
        public MenuList EventsMenu;
        public MenuList RelaysMenu;
        public MenuList SetsRelaysMenu;
        public MenuList TimesMenu;
        public MenuList TrueOrFalseMenu;

        private List<string> _zoneNumbers;
        private List<string> _eventNumbers;

        public List<string> Actions      => ActionsMenu?.Invoke();
        public List<string> Triggers     => TriggersMenu?.Invoke();
        public List<string> Inputs       => InputsMenu?.Invoke();
        public List<string> Loop1Devices => Loop1DevicesMenu?.Invoke();
        public List<string> Loop2Devices => Loop2DevicesMenu?.Invoke();
        public List<string> Zones        => ZonesMenu?.Invoke();
        public List<string> ZoneNumbers  => _zoneNumbers;
        public List<string> ZonesPanels  => ZonesPanelsMenu?.Invoke();
        public List<string> Groups       => GroupsMenu?.Invoke();
        public List<string> Sets         => SetsMenu?.Invoke();
        public List<string> Events       => EventsMenu?.Invoke();
        public List<string> EventNumbers => _eventNumbers;
        public List<string> Relays       => RelaysMenu?.Invoke();
        public List<string> SetsRelays   => SetsRelaysMenu?.Invoke();
        public List<string> Times        => TimesMenu?.Invoke();
        public List<string> TrueOrFalse  => TrueOrFalseMenu?.Invoke();
        

        /// <summary>
        /// Initialise the Zone and Group number lists (i.e. numerals-only list)
        /// </summary>
        internal void InitFixedComboLists()
        {
            _zoneNumbers = new() { "" };
            for (int i = 0; i < ZoneConfigData.NumZones; i++)
                _zoneNumbers.Add((i + 1).ToString());

            _eventNumbers = new() { "" };
            for (int i = 0; i < CEConfigData.NumEvents; i++)
                _eventNumbers.Add((i + 1).ToString());

            OnPropertyChanged(nameof(ZoneNumbers));
            OnPropertyChanged(nameof(EventNumbers));
        }
        #endregion


        #region IAppViewModel implementation
        public void SetCulture(CultureInfo culture) => RefreshView();
        #endregion


        #region IPanelToolsViewModel implementation
        public void PopulateView(XfpData data) { }

        public void RefreshView() => refreshView(null);
        private void refreshView(CEValue? level)
        {
            var at = SelectedActionTypeIndex;
            var ap = SelectedActionParamIndex;
            var tt = SelectedTriggerTypeIndex;
            var t1 = SelectedTriggerParamIndex;
            var t2 = SelectedTriggerParam2Index;
            var tc = SelectedTriggerConditionIndex;
            var rt = SelectedResetTypeIndex;
            var r1 = SelectedResetParamIndex;
            var r2 = SelectedResetParam2Index;
            var rc = SelectedResetConditionIndex;
            
            OnPropertyChanged(nameof(CENumber));
            //OnPropertyChanged(nameof(ActionType));
            //OnPropertyChanged(nameof(ActionParam));
            //OnPropertyChanged(nameof(TriggerType));
            //OnPropertyChanged(nameof(TriggerParam));
            //OnPropertyChanged(nameof(TriggerParam2));
            //OnPropertyChanged(nameof(TriggerCondition));
            //OnPropertyChanged(nameof(ResetType));
            //OnPropertyChanged(nameof(ResetParam));
            //OnPropertyChanged(nameof(ResetParam2));
            //OnPropertyChanged(nameof(ResetCondition));

            OnPropertyChanged(nameof(SelectedActionTypeIndex));
            OnPropertyChanged(nameof(SelectedActionParamIndex));
            OnPropertyChanged(nameof(SelectedTriggerTypeIndex));
            OnPropertyChanged(nameof(SelectedTriggerParamIndex));
            OnPropertyChanged(nameof(SelectedTriggerParam2Index));
            OnPropertyChanged(nameof(SelectedTriggerConditionIndex));
            OnPropertyChanged(nameof(SelectedResetTypeIndex));
            OnPropertyChanged(nameof(SelectedResetParamIndex));
            OnPropertyChanged(nameof(SelectedResetParam2Index));
            OnPropertyChanged(nameof(SelectedResetConditionIndex));

            OnPropertyChanged(nameof(ShowActionParam));
            OnPropertyChanged(nameof(ShowActionLoop1Devices));
            OnPropertyChanged(nameof(ShowActionLoop2Devices));
            OnPropertyChanged(nameof(ShowActionRelays));
            OnPropertyChanged(nameof(ShowActionGroups));
            OnPropertyChanged(nameof(ShowActionZones));
            OnPropertyChanged(nameof(ShowActionSets));
            OnPropertyChanged(nameof(ShowActionSetsAndRelays));
            OnPropertyChanged(nameof(ShowActionEvents));

            OnPropertyChanged(nameof(ShowTriggerType));
            OnPropertyChanged(nameof(ShowTriggerParam));
            OnPropertyChanged(nameof(ShowTriggerParam2));
            OnPropertyChanged(nameof(ShowTriggerLoop1Devices));
            OnPropertyChanged(nameof(ShowTriggerLoop2Devices));
            OnPropertyChanged(nameof(ShowTriggerZones));
            OnPropertyChanged(nameof(ShowTriggerZones2));
            OnPropertyChanged(nameof(ShowTriggerZonesAndPanels));
            OnPropertyChanged(nameof(ShowTriggerEvents));
            OnPropertyChanged(nameof(ShowTriggerEvents2));
            OnPropertyChanged(nameof(ShowTriggerTimes));
            OnPropertyChanged(nameof(ShowTriggerInputs));
            OnPropertyChanged(nameof(ShowTriggerCondition));

            OnPropertyChanged(nameof(ShowResetType));
            OnPropertyChanged(nameof(ShowResetParam));
            OnPropertyChanged(nameof(ShowResetParam2));
            OnPropertyChanged(nameof(ShowResetLoop1Devices));
            OnPropertyChanged(nameof(ShowResetLoop2Devices));
            OnPropertyChanged(nameof(ShowResetZones));
            OnPropertyChanged(nameof(ShowResetZones2));
            OnPropertyChanged(nameof(ShowResetZonesAndPanels));
            OnPropertyChanged(nameof(ShowResetEvents));
            OnPropertyChanged(nameof(ShowResetEvents2));
            OnPropertyChanged(nameof(ShowResetTimes));
            OnPropertyChanged(nameof(ShowResetInputs));
            OnPropertyChanged(nameof(ShowResetCondition));

            OnPropertyChanged(nameof(ActionParamIsValid));
            OnPropertyChanged(nameof(TriggerTypeIsValid));
            OnPropertyChanged(nameof(TriggerParam1IsValid));
            OnPropertyChanged(nameof(IndicateInvalidANDTriggerParams));
            OnPropertyChanged(nameof(IndicateInvalidSelectedTriggerParam1));
            OnPropertyChanged(nameof(IndicateInvalidSelectedTriggerParam2));
            OnPropertyChanged(nameof(TriggerConditionIsValid));
            OnPropertyChanged(nameof(ResetTypeIsValid));
            OnPropertyChanged(nameof(ResetParam1IsValid));
            OnPropertyChanged(nameof(IndicateInvalidANDResetParams));
            OnPropertyChanged(nameof(IndicateInvalidSelectedResetParam1));
            OnPropertyChanged(nameof(IndicateInvalidSelectedResetParam2));
            OnPropertyChanged(nameof(ResetConditionIsValid));

            if (level != CEValue.ActionType)       SelectedActionTypeIndex = at;
            if (level != CEValue.ActionParam)      SelectedActionParamIndex = ap;
            if (level != CEValue.TriggerType)      SelectedTriggerTypeIndex = tt;
            if (level != CEValue.TriggerParam)     SelectedTriggerParamIndex = t1;
            if (level != CEValue.TriggerParam)     SelectedTriggerParam2Index = t2;
            if (level != CEValue.TriggerCondition) SelectedTriggerConditionIndex = tc;
            if (level != CEValue.ResetType)        SelectedResetTypeIndex = rt;
            if (level != CEValue.ResetParam)       SelectedResetParamIndex = r1;
            if (level != CEValue.ResetParam)       SelectedResetParam2Index = r2;
            if (level != CEValue.ResetCondition)   SelectedResetConditionIndex = rc;
        }

        public void RefreshComboLists()
        {
            var at = _data.ActionType;
            var ap = _data.ActionParam;
            var tt = _data.TriggerType;
            var t1 = _data.TriggerParam;
            var t2 = _data.TriggerParam2;
            var tc = _data.TriggerCondition;
            var rt = _data.ResetType;
            var r1 = _data.ResetParam;
            var r2 = _data.ResetParam2;
            var rc = _data.ResetCondition;

            OnPropertyChanged(nameof(Actions));
            OnPropertyChanged(nameof(Triggers));
            OnPropertyChanged(nameof(Groups));
            OnPropertyChanged(nameof(Inputs));
            OnPropertyChanged(nameof(Loop1Devices));
            OnPropertyChanged(nameof(Loop2Devices));
            OnPropertyChanged(nameof(Zones));
            OnPropertyChanged(nameof(ZoneNumbers));
            OnPropertyChanged(nameof(ZonesPanels));
            OnPropertyChanged(nameof(Sets));
            OnPropertyChanged(nameof(Events));
            OnPropertyChanged(nameof(EventNumbers));
            OnPropertyChanged(nameof(Relays));
            OnPropertyChanged(nameof(SetsRelays));
            OnPropertyChanged(nameof(Times));
            OnPropertyChanged(nameof(TrueOrFalse));

            _data.ActionType = at;
            _data.ActionParam = ap;
            _data.TriggerType = tt;
            _data.TriggerParam = t1;
            _data.TriggerParam2 = t2;
            _data.TriggerCondition = tc;
            _data.ResetType = rt;
            _data.ResetParam = r1;
            _data.ResetParam2 = r2;
            _data.ResetCondition = rc;
        }
        #endregion
    }
}
