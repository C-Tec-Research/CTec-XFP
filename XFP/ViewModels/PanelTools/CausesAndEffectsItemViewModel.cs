using System;
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
using Windows.Foundation.Metadata;

namespace Xfp.ViewModels.PanelTools
{
    class CausesAndEffectsItemViewModel : ViewModelBase, IPanelToolsViewModel
    {
        public CausesAndEffectsItemViewModel(CEEvent data, int index)
        {
            _number = index + 1;
            Data = data;
        }

        public enum CEValue { ActionType, ActionParam, TriggerType, TriggerParam, TriggerCondition, ResetType, ResetParam, ResetCondition };
        

        private int     _number;
        private CEEvent _data = new();
        private bool?   _triggerCondition;
        private bool?   _resetCondition;
        private object  _dataLock = new();


        public CEEvent Data
        {
            get => _data;
            set
            {
                lock (_dataLock)
                {
                    _data = value;
                    _triggerCondition = _data.TriggerCondition;
                    _resetCondition   = _data.ResetCondition;
                    RefreshView();
                }
            }
        }

        public int CENumber => _number;


        #region menu item counts
        public int ActionTypesMenuCount { get; set; }
        public int TriggerTypesMenuCount { get; set; }
        public int GroupsMenuCount { get; set; }
        public int InputsMenuCount { get; set; }
        public int Loop1DevicesMenuCount { get; set; }
        public int Loop2DevicesMenuCount { get; set; }
        public int ZonesMenuCount { get; set; }
        public int ZonePanelsMenuCount { get; set; }
        public int SetsMenuCount { get; set; }
        public int EventsMenuCount { get; set; }
        public int RelaysMenuCount { get; set; }
        public int SetsRelaysMenuCount { get; set; }
        public int TimesMenuCount { get; set; }
        public int ZoneNumbersMenuCount { get; set; }
        public int EventNumbersMenuCount { get; set; }
        public int ConditionsMenuCount { get; set; }

        private int actionParamMenuCount   => ShowActionEvents ? EventsMenuCount : ShowActionGroups ? GroupsMenuCount : ShowActionLoop1Devices ? Loop1DevicesMenuCount : ShowActionLoop2Devices ? Loop2DevicesMenuCount : ShowActionRelays ? RelaysMenuCount : ShowActionSets ? SetsMenuCount : ShowActionSetsAndRelays ? SetsRelaysMenuCount : ShowActionZones ? ZonesMenuCount : 0;
        private int triggerParamMenuCount  => ShowTriggerParamAND ? ShowTriggerEvents2 ? EventNumbersMenuCount : ZoneNumbersMenuCount : ShowTriggerEvents ? EventsMenuCount : ShowTriggerInputs ? InputsMenuCount : ShowTriggerLoop1Devices ? Loop1DevicesMenuCount : ShowTriggerLoop2Devices ? Loop2DevicesMenuCount : ShowTriggerTimes ? TimesMenuCount : ShowTriggerZones ? ZonesMenuCount : ShowTriggerZonesAndPanels ? ZonePanelsMenuCount : 0;
        private int resetParamMenuCount    => ShowResetParamAND   ? ShowResetEvents2   ? EventNumbersMenuCount : ZoneNumbersMenuCount : ShowResetEvents   ? EventsMenuCount : ShowResetInputs   ? InputsMenuCount : ShowResetLoop1Devices   ? Loop1DevicesMenuCount : ShowResetLoop2Devices   ? Loop2DevicesMenuCount : ShowResetTimes   ? TimesMenuCount : ShowResetZones   ? ZonesMenuCount : ShowResetZonesAndPanels   ? ZonePanelsMenuCount : 0;
        private int triggerParam2MenuCount => ShowTriggerEvents2 ? EventNumbersMenuCount : ShowTriggerZones2 ? ZoneNumbersMenuCount : 0;
        private int resetParam2MenuCount   => ShowResetEvents2   ? EventNumbersMenuCount : ShowResetZones2   ? ZoneNumbersMenuCount : 0;
        #endregion


        #region selected indices
        public int SelectedActionTypeIndex
        {
            get => Array.IndexOf(Enum.GetValues(_data.ActionType.GetType()), _data.ActionType);
            set
            {
                if (value != SelectedActionTypeIndex)
                {
                    if (value < 0) value = 0;
                    _data.ActionType = (CEActionTypes)(Enum.GetValues(_data.ActionType.GetType())).GetValue(value);
                    RefreshView();
                }
            }
        }

        public int SelectedActionParamIndex
        {
            get => _data.ActionParam + 1 < actionParamMenuCount ? _data.ActionParam + 1 : 0;
            set
            {
                if (value != SelectedActionParamIndex)
                {
                    if (value < 0 || value >= actionParamMenuCount) value = 0;
                    _data.ActionParam = value - 1;
                    RefreshView();
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
                    if (value < 0 || value >= TriggerTypesMenuCount) value = 0;
                    _data.TriggerType = (CETriggerTypes)(Enum.GetValues(_data.TriggerType.GetType())).GetValue(value);
                    RefreshView();
                }
            }
        }

        public int SelectedTriggerParamIndex
        {
            get => _data.TriggerParam + 1 < triggerParamMenuCount ? _data.TriggerParam + 1 : 0;
            set
            {
                if (value != SelectedTriggerParamIndex)
                {
                    if (value < 0 || value >= triggerParamMenuCount) value = 0;
                    _data.TriggerParam = value - 1;
                    RefreshView();
                }
            }
        }

        public int SelectedTriggerParam2Index
        {
            get => _data.TriggerParam2 + 1 < triggerParam2MenuCount ? _data.TriggerParam2 + 1 : 0;
            set
            {
                if (value != SelectedTriggerParam2Index)
                {
                    if (value < 0 || value >= triggerParam2MenuCount) value = 0;
                    _data.TriggerParam2 = value - 1;
                    RefreshView();
                }
            }
        }

        public int SelectedTriggerConditionIndex
        {
            //get => _data.TriggerCondition switch { true => 1, false => 2, };
            //set
            //{
            //    if (value != SelectedTriggerConditionIndex)
            //    {
            //        if (value < 0 || value >= ConditionsMenuCount) value = 0;
            //        _data.TriggerCondition = value == 1;
            //        refreshView(CEValue.TriggerCondition);
            //    }
            //}
            get => _triggerCondition switch { true => 1, false => 2, _ => 0 };
            set
            {
                if (value != SelectedTriggerConditionIndex)
                {
                    if (value < 0 || value >= ConditionsMenuCount) value = 0;
                    _triggerCondition = value switch { 1 => true, 2 => false, _ => null };
                    _data.TriggerCondition = value == 1;
                    RefreshView();
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
                    if (value < 0 || value >= TriggerTypesMenuCount) value = 0;
                    _data.ResetType = (CETriggerTypes)(Enum.GetValues(_data.ResetType.GetType())).GetValue(value);
                    RefreshView();
                }
            }
        }

        public int SelectedResetParamIndex
        {
            get => _data.ResetParam + 1 < resetParamMenuCount ? _data.ResetParam + 1 : 0;
            set
            {
                if (value != SelectedResetParamIndex)
                {
                    if (value < 0 || value >= resetParamMenuCount) value = 0;
                    _data.ResetParam = _data.ResetParam = value - 1;
                    RefreshView();
                }
            }
        }

        public int SelectedResetParam2Index
        {
            get => _data.ResetParam2 + 1 < resetParam2MenuCount ? _data.ResetParam2 + 1 : 0;
            set
            {
                if (value != SelectedResetParam2Index)
                {
                    if (value < 0 || value >= resetParam2MenuCount) value = 0;
                    _data.ResetParam2 = value - 1;
                    RefreshView();
                }
            }
        }

        public int SelectedResetConditionIndex
        {
            //get => _data.ResetCondition switch { true => 1, false => 2, };
            //set
            //{
            //    if (value != SelectedResetConditionIndex)
            //    {
            //        if (value < 0 || value >= ConditionsMenuCount) value = 0;
            //        _data.ResetCondition = value == 1;
            //        RefreshView();
            //    }
            //}
            get => _resetCondition switch { true => 1, false => 2, _ => 0 };
            set
            {
                if (value != SelectedResetConditionIndex)
                {
                    if (value < 0 || value >= ConditionsMenuCount) value = 0;
                    _resetCondition = value switch { 1 => true, 2 => false, _ => null };
                    _data.ResetCondition = value == 1;
                    RefreshView();
                }
            }

        }
        #endregion


        #region save/restore indices
        private int _savedActionTypeIndex;
        private int _savedActionParamIndex;
        private int _savedTriggerTypeIndex;
        private int _savedTriggerParam1Index;
        private int _savedTriggerParam2Index;
        private int _savedTriggerConditionIndex;
        private int _savedResetTypeIndex;
        private int _savedResetParam1Index;
        private int _savedResetParam2Index;
        private int _savedResetConditionIndex;

        public void SaveIndices()
        {
            _savedActionTypeIndex       = SelectedActionTypeIndex;
            _savedActionParamIndex      = SelectedActionParamIndex;
            _savedTriggerTypeIndex      = SelectedTriggerTypeIndex;
            _savedTriggerParam1Index    = SelectedTriggerParamIndex;
            _savedTriggerParam2Index    = SelectedTriggerParam2Index;
            _savedTriggerConditionIndex = SelectedTriggerConditionIndex;
            _savedResetTypeIndex        = SelectedResetTypeIndex;
            _savedResetParam1Index      = SelectedResetParamIndex;
            _savedResetParam2Index      = SelectedResetParam2Index;
            _savedResetConditionIndex   = SelectedResetConditionIndex;

            //set all to zero so that the screen is forced to update when RestoreIndices() is called
            SelectedActionTypeIndex = SelectedActionParamIndex   
                                    = SelectedTriggerTypeIndex = SelectedTriggerParamIndex = SelectedTriggerParam2Index = SelectedTriggerConditionIndex 
                                    = SelectedResetTypeIndex   = SelectedResetParamIndex   = SelectedResetParam2Index   = SelectedResetConditionIndex = 0;
        }

        public void RestoreIndices()
        {
            SelectedActionTypeIndex       = _savedActionTypeIndex;
            SelectedActionParamIndex      = _savedActionParamIndex;
            SelectedTriggerTypeIndex      = _savedTriggerTypeIndex;
            SelectedTriggerParamIndex     = _savedTriggerParam1Index;
            SelectedTriggerParam2Index    = _savedTriggerParam2Index;
            SelectedTriggerConditionIndex = _savedTriggerConditionIndex;
            SelectedResetTypeIndex        = _savedResetTypeIndex;
            SelectedResetParamIndex       = _savedResetParam1Index;
            SelectedResetParam2Index      = _savedResetParam2Index;
            SelectedResetConditionIndex   = _savedResetConditionIndex;
        }
        #endregion


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
        public bool ShowTriggerParam          => ShowTriggerType   && !ShowTriggerParamAND && !_data.NoTriggerParam(_data.TriggerType);
        public bool ShowTriggerParamAND       => ShowTriggerType   && (_data.TriggerType == CETriggerTypes.EventAnd || _data.TriggerType == CETriggerTypes.ZoneAnd);
        public bool ShowTriggerLoop1Devices   => ShowTriggerParam  && _data.TriggerType switch { CETriggerTypes.Loop1DeviceTriggered or CETriggerTypes.Loop1DevicePrealarm => true, _ => false };
        public bool ShowTriggerLoop2Devices   => ShowTriggerParam  && _data.TriggerType switch { CETriggerTypes.Loop2DeviceTriggered or CETriggerTypes.Loop2DevicePrealarm => true, _ => false };
        public bool ShowTriggerZones          => ShowTriggerParam  && _data.TriggerType switch { CETriggerTypes.ZoneAnd => true, _ => false };
        public bool ShowTriggerZones2         => ShowTriggerParamAND && _data.TriggerType switch { CETriggerTypes.ZoneAnd => true, _ => false };
        public bool ShowTriggerZonesAndPanels => ShowTriggerParam  && _data.TriggerType switch { CETriggerTypes.ZoneOrPanelInFire or CETriggerTypes.ZoneHasDeviceInAlarm => true, _ => false };
        public bool ShowTriggerEvents         => ShowTriggerParam  && _data.TriggerType switch { CETriggerTypes.OtherEventTriggered or CETriggerTypes.NetworkEventTriggered or CETriggerTypes.EventAnd => true, _ => false };
        public bool ShowTriggerEvents2        => ShowTriggerParamAND && _data.TriggerType switch { CETriggerTypes.EventAnd => true, _ => false };
        public bool ShowTriggerTimes          => ShowTriggerParam  && _data.TriggerType switch { CETriggerTypes.TimerEventTn => true, _ => false };
        public bool ShowTriggerInputs         => ShowTriggerParam  && _data.TriggerType switch { CETriggerTypes.PanelInput => true, _ => false };
        public bool ShowTriggerCondition      => ShowTriggerType && (!ShowTriggerParam || ShowTriggerParam && SelectedTriggerParamIndex > 0 || ShowTriggerParamAND && SelectedTriggerParam2Index > 0);
        #endregion

        #region reset
        public bool ShowResetType           => ShowTriggerCondition;
        public bool ShowResetParam          => ShowResetType   && !ShowResetParamAND && !_data.NoTriggerParam(_data.ResetType);
        public bool ShowResetParamAND       => ShowResetType   && (_data.ResetType == CETriggerTypes.EventAnd || _data.ResetType == CETriggerTypes.ZoneAnd);
        public bool ShowResetLoop1Devices   => ShowResetParam  && _data.ResetType switch { CETriggerTypes.Loop1DeviceTriggered or CETriggerTypes.Loop1DevicePrealarm => true, _ => false };
        public bool ShowResetLoop2Devices   => ShowResetParam  && _data.ResetType switch { CETriggerTypes.Loop2DeviceTriggered or CETriggerTypes.Loop2DevicePrealarm => true, _ => false };
        public bool ShowResetZones          => ShowResetParam  && _data.ResetType switch { CETriggerTypes.ZoneAnd => true, _ => false };
        public bool ShowResetZones2         => ShowResetParamAND && _data.ResetType switch { CETriggerTypes.ZoneAnd => true, _ => false };
        public bool ShowResetZonesAndPanels => ShowResetParam  && _data.ResetType switch { CETriggerTypes.ZoneOrPanelInFire or CETriggerTypes.ZoneHasDeviceInAlarm => true, _ => false };
        public bool ShowResetEvents         => ShowResetParam  && _data.ResetType switch { CETriggerTypes.OtherEventTriggered or CETriggerTypes.NetworkEventTriggered or CETriggerTypes.EventAnd => true, _ => false };
        public bool ShowResetEvents2        => ShowResetParamAND && _data.ResetType switch { CETriggerTypes.EventAnd => true, _ => false };
        public bool ShowResetTimes          => ShowResetParam  && _data.ResetType switch { CETriggerTypes.TimerEventTn => true, _ => false };
        public bool ShowResetInputs         => ShowResetParam  && _data.ResetType switch { CETriggerTypes.PanelInput => true, _ => false };
        public bool ShowResetCondition      => ShowResetType && (!ShowResetParam || ShowResetParam && SelectedResetParamIndex > 0 || ShowResetParamAND && SelectedResetParam2Index > 0);
        #endregion

        #endregion


        #region option validation
        public bool ActionParamIsValid                   => !ShowActionParam || (SelectedActionParamIndex > 0 && SelectedActionParamIndex < actionParamMenuCount) || _data.ActionType == CEActionTypes.None;

        public bool TriggerTypeIsValid                   => !ShowTriggerType || (SelectedTriggerTypeIndex > 0 && SelectedTriggerTypeIndex < TriggerTypesMenuCount);
        public bool TriggerParam1IsValid                 => !ShowTriggerParam || (SelectedTriggerParamIndex > 0 && SelectedTriggerParamIndex < triggerParamMenuCount) || _data.TriggerType == CETriggerTypes.None;
        public bool IndicateInvalidANDTriggerParams      => ShowTriggerParamAND && (ShowTriggerEvents2 || ShowTriggerZones2) && (SelectedTriggerParamIndex  <= 0 || SelectedTriggerParamIndex  >= triggerParamMenuCount)
                                                                                                                             && (SelectedTriggerParam2Index <= 0 || SelectedTriggerParam2Index >= triggerParam2MenuCount);
        public bool IndicateInvalidSelectedTriggerParam1 => !IndicateInvalidANDTriggerParams && (ShowTriggerEvents2 || ShowTriggerZones2) && SelectedTriggerParamIndex <= 0 && SelectedTriggerParamIndex < triggerParamMenuCount;
        public bool IndicateInvalidSelectedTriggerParam2 => !IndicateInvalidANDTriggerParams && (!ShowTriggerEvents2 || !ShowTriggerZones2) && SelectedTriggerParam2Index <= 0 && SelectedTriggerParam2Index < triggerParam2MenuCount;
        public bool TriggerConditionIsValid              => !ShowTriggerCondition || (SelectedTriggerConditionIndex > 0 && SelectedTriggerConditionIndex < ConditionsMenuCount);

        public bool ResetTypeIsValid                     => !ShowResetType || (SelectedResetTypeIndex > 0 && SelectedResetTypeIndex < TriggerTypesMenuCount);
        public bool ResetParam1IsValid                   => !ShowResetParam || (SelectedResetParamIndex > 0 && SelectedResetParamIndex < resetParamMenuCount) || _data.ResetType == CETriggerTypes.None;
        public bool IndicateInvalidANDResetParams        => ShowResetParamAND && (ShowResetEvents2 || ShowResetZones2) && (SelectedResetParamIndex  <= 0 || SelectedResetParamIndex  >= resetParamMenuCount)
                                                                                                                       && (SelectedResetParam2Index <= 0 || SelectedResetParam2Index >= resetParam2MenuCount);
        public bool IndicateInvalidSelectedResetParam1   => !IndicateInvalidANDResetParams && (ShowResetEvents2 || ShowResetZones2) && SelectedResetParamIndex <= 0 && SelectedResetParamIndex < resetParamMenuCount;
        public bool IndicateInvalidSelectedResetParam2   => !IndicateInvalidANDResetParams && (!ShowResetEvents2 || !ShowResetZones2) && SelectedResetParam2Index <= 0 && SelectedResetParam2Index < resetParam2MenuCount;
        public bool ResetConditionIsValid                => !ShowResetCondition || (SelectedResetConditionIndex > 0 && SelectedResetConditionIndex < ConditionsMenuCount);
        #endregion


        #region IAppViewModel implementation
        public void SetCulture(CultureInfo culture) { }
        #endregion


        #region IPanelToolsViewModel implementation
        public void PopulateView(XfpData data) { }

        public void RefreshView()
        {
            OnPropertyChanged(nameof(CENumber));

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
            OnPropertyChanged(nameof(ShowTriggerParamAND));
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
            OnPropertyChanged(nameof(ShowResetParamAND));
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
        }
        #endregion
    }
}
