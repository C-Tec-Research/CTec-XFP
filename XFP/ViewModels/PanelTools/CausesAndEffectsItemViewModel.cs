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

        private object _setDataLock = new();
        private object _refreshLock = new();

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


        public CEEvent Data
        {
            get => _data;
            set
            {
                lock (_setDataLock)
                {
                    _data = value;
                    RefreshView();
                }
            }
        }


        public int CENumber => _number;

        public int SelectedActionTypeIndex
        {
            get => Array.IndexOf(Enum.GetValues(_data.ActionType.GetType()), _data.ActionType);
            set
            {
                if (value != SelectedActionTypeIndex)
                {
                    if (value < 0) value = 0;
                    _data.ActionType = (CEActionTypes)(Enum.GetValues(_data.ActionType.GetType())).GetValue(value);
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
                    _data.ActionParam = value - 1;
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
                    _data.TriggerType = (CETriggerTypes)(Enum.GetValues(_data.TriggerType.GetType())).GetValue(value);
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
                    _data.TriggerParam = value - 1;
                    clearDependants(CEValue.TriggerParam);
                    
                    if (SelectedTriggerConditionIndex == 0)
                        SelectedTriggerConditionIndex = 1;

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
                    _data.TriggerParam2 = value - 1;
                    clearDependants(CEValue.TriggerParam);

                    if (SelectedTriggerConditionIndex == 0)
                        SelectedTriggerConditionIndex = 1;

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
                    _data.TriggerCondition = value == 1;
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
                    _data.ResetType = (CETriggerTypes)(Enum.GetValues(_data.ResetType.GetType())).GetValue(value);
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
                    _data.ResetParam = _data.ResetParam = value - 1;
                    clearDependants(CEValue.ResetParam);

                    if (SelectedResetConditionIndex == 0)
                        SelectedResetConditionIndex = 2;

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
                    _data.ResetParam2 = value - 1;
                    clearDependants(CEValue.ResetParam);

                    if (SelectedResetConditionIndex == 0)
                        SelectedResetConditionIndex = 2;

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
                    _data.ResetCondition = value == 1;
                    refreshView(CEValue.ResetCondition);
                }
            }
        }


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
        }

        public void RestoreIndices(CEValue? level = null)
        {
            if (level != CEValue.ActionType)       SelectedActionTypeIndex       = _savedActionTypeIndex;
            if (level != CEValue.ActionParam)      SelectedActionParamIndex      = _savedActionParamIndex;
            if (level != CEValue.TriggerType)      SelectedTriggerTypeIndex      = _savedTriggerTypeIndex;
            if (level != CEValue.TriggerParam)     SelectedTriggerParamIndex     = _savedTriggerParam1Index;
            if (level != CEValue.TriggerParam)     SelectedTriggerParam2Index    = _savedTriggerParam2Index;
            if (level != CEValue.TriggerCondition) SelectedTriggerConditionIndex = _savedTriggerConditionIndex;
            if (level != CEValue.ResetType)        SelectedResetTypeIndex        = _savedResetTypeIndex;
            if (level != CEValue.ResetParam)       SelectedResetParamIndex       = _savedResetParam1Index;
            if (level != CEValue.ResetParam)       SelectedResetParam2Index      = _savedResetParam2Index;
            if (level != CEValue.ResetCondition)   SelectedResetConditionIndex   = _savedResetConditionIndex;
        }


        private void clearDependants(CEValue level)
        {
            //switch (level)
            //{

            //    case CEValue.ActionType:       SelectedActionParamIndex = 0; break;
            //    case CEValue.ActionParam:      SelectedTriggerTypeIndex = 0; break;
            //    case CEValue.TriggerType:      SelectedTriggerParamIndex = SelectedTriggerParam2Index = 0; break;
            //    case CEValue.TriggerParam:     SelectedTriggerConditionIndex = 0; break;
            //    case CEValue.TriggerCondition: SelectedResetTypeIndex = 0; break;
            //    case CEValue.ResetType:        SelectedResetParamIndex = SelectedResetParam2Index = 0; break;
            //    case CEValue.ResetParam:       SelectedResetConditionIndex = 0; break;
            //}
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
        public bool ShowTriggerCondition      => (ShowTriggerParam && SelectedTriggerParamIndex > 0) 
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
        public bool ShowResetCondition      => ShowResetParam && SelectedResetParamIndex > 0 || ShowResetParam2 && SelectedResetParamIndex > 0 && SelectedResetParam2Index > 0 || _data.NoTriggerParam(_data.ResetType) && _data.ResetType != CETriggerTypes.None;
        #endregion

        #endregion

        #region option validation
        public bool ActionParamIsValid                   => !ShowActionParam || SelectedActionParamIndex > 0 || _data.ActionType == CEActionTypes.None;

        public bool TriggerTypeIsValid                   => !ShowTriggerType || SelectedTriggerTypeIndex > 0;
        public bool TriggerParam1IsValid                 => !ShowTriggerParam || SelectedTriggerParamIndex > 0 || _data.TriggerType == CETriggerTypes.None;
        public bool IndicateInvalidANDTriggerParams      => !(!ShowTriggerParam2 || !(!ShowTriggerParam2 || SelectedTriggerParamIndex < 0 && SelectedTriggerParam2Index <= 0));
        public bool IndicateInvalidSelectedTriggerParam1 => !IndicateInvalidANDTriggerParams && (ShowTriggerEvents2 || ShowTriggerZones2) && SelectedTriggerParamIndex <= 0;
        public bool IndicateInvalidSelectedTriggerParam2 => !IndicateInvalidANDTriggerParams && (!ShowTriggerEvents2 || !ShowTriggerZones2) && SelectedTriggerParam2Index <= 0;
        public bool TriggerConditionIsValid              => !ShowTriggerCondition || SelectedTriggerConditionIndex > 0;

        public bool ResetTypeIsValid                     => !ShowResetType || SelectedResetTypeIndex > 0;
        public bool ResetParam1IsValid                   => !ShowResetParam || SelectedResetParamIndex > 0 || _data.ResetType == CETriggerTypes.None;
        public bool IndicateInvalidANDResetParams        => !(!ShowResetParam2 || !(!ShowResetParam2 || SelectedResetParamIndex < 0 && SelectedResetParam2Index <= 0));
        public bool IndicateInvalidSelectedResetParam1   => !IndicateInvalidANDResetParams && (ShowResetEvents2 || ShowResetZones2) && SelectedResetParamIndex <= 0;
        public bool IndicateInvalidSelectedResetParam2   => !IndicateInvalidANDResetParams && (!ShowResetEvents2 || !ShowResetZones2) && SelectedResetParam2Index <= 0;
        public bool ResetConditionIsValid                => !ShowResetCondition || SelectedResetConditionIndex > 0;
        #endregion


        #region IAppViewModel implementation
        public void SetCulture(CultureInfo culture) { }
        #endregion


        #region IPanelToolsViewModel implementation
        public void PopulateView(XfpData data) { }

        public void RefreshView() => refreshView(null);
        private void refreshView(CEValue? level)
        {
            lock (_refreshLock)
            {
                //SaveIndices();

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

                //RestoreIndices(level);
            }
        }
        #endregion
    }
}
