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
        public CausesAndEffectsItemViewModel(CEEvent data, int index/*, LoopConfigData loopConfig*/)
        {
            _number = index + 1;
            _data = data;
            //_loopConfig = loopConfig;
            initData();
            RefreshView();
            
            /// <summary>Add update to Times menu</summary>
            _timesTimer.Elapsed += new((s, e) => OnPropertyChanged(nameof(Times)));
        }

        protected static System.Timers.Timer _timesTimer = new() { AutoReset = true, Enabled = true, Interval = 2000 };


        private int     _number;
        private CEEvent _data;
        //private LoopConfigData _loopConfig;

        public CEEvent Data { set { _data = value; initData(); RefreshView(); } }


        private void initData()
        {
            _actionParamLoop1Device = _actionParamLoop2Device = _actionParamRelay = _actionParamGroup = _actionParamZone = _actionParamSet = _actionParamRelay = _actionParamSetRelay = _actionParamEvent = -1;
            _triggerParamLoop1Device = _triggerParamLoop2Device = _triggerParamInput = _triggerParamZonePanel = _triggerParamEvent = _triggerParamTimer = _triggerParamEvent = _triggerParamEvent2 = _triggerParamZone = _triggerParamZone2 = -1;
            _resetParamLoop1Device = _resetParamLoop2Device = _resetParamInput = _resetParamZonePanel = _resetParamEvent = _resetParamTimer = _resetParamEvent =    _resetParamEvent2 = _resetParamZone = _resetParamZone2 = -1;

            switch (_data.ActionType)
            {
                case CEActionTypes.TriggerLoop1Device:
                case CEActionTypes.Loop1DeviceDisable: _actionParamLoop1Device = _data.ActionParam; break;

                case CEActionTypes.TriggerLoop2Device:
                case CEActionTypes.Loop2DeviceDisable: _actionParamLoop2Device = _data.ActionParam; break;

                case CEActionTypes.PanelRelay: _actionParamRelay = _data.ActionParam; break;

                case CEActionTypes.SounderAlert:
                case CEActionTypes.SounderEvac:
                case CEActionTypes.GroupDisable:
                case CEActionTypes.TriggerBeacons: _actionParamGroup = _data.ActionParam; break;

                case CEActionTypes.ZoneDisable:
                case CEActionTypes.PutZoneIntoAlarm: _actionParamZone = _data.ActionParam; break;

                case CEActionTypes.TriggerOutputSet: _actionParamSet = _data.ActionParam; break;

                case CEActionTypes.OutputDisable: _actionParamSetRelay = _data.ActionParam; break;

                case CEActionTypes.TriggerNetworkEvent: _actionParamEvent = _data.ActionParam; break;
            }

            switch (_data.TriggerType)
            {
                case CETriggerTypes.Loop1DeviceTriggered:
                case CETriggerTypes.Loop1DevicePrealarm: _triggerParamLoop1Device = _data.TriggerParam; break;

                case CETriggerTypes.Loop2DeviceTriggered:
                case CETriggerTypes.Loop2DevicePrealarm: _triggerParamLoop2Device = _data.TriggerParam; break;

                case CETriggerTypes.PanelInput: _triggerParamInput = _data.TriggerParam; break;

                case CETriggerTypes.ZoneOrPanelInFire:
                case CETriggerTypes.ZoneHasDeviceInAlarm: _triggerParamZonePanel = _data.TriggerParam; break;

                case CETriggerTypes.OtherEventTriggered:
                case CETriggerTypes.NetworkEventTriggered: _triggerParamEvent = _data.TriggerParam; break;

                case CETriggerTypes.TimerEventTn: _triggerParamTimer = _data.TriggerParam; break;

                case CETriggerTypes.EventAnd: _triggerParamEvent = _data.TriggerParam; _triggerParamEvent2 = _data.TriggerParam2; break;

                case CETriggerTypes.ZoneAnd: _triggerParamZone = _data.TriggerParam; _triggerParamZone2 = _data.TriggerParam2; break;
            }

            switch (_data.ResetType)
            {
                case CETriggerTypes.Loop1DeviceTriggered:
                case CETriggerTypes.Loop1DevicePrealarm: _resetParamLoop1Device = _data.ResetParam; break;

                case CETriggerTypes.Loop2DeviceTriggered:
                case CETriggerTypes.Loop2DevicePrealarm: _resetParamLoop2Device = _data.ResetParam; break;

                case CETriggerTypes.PanelInput: _resetParamInput = _data.ResetParam; break;

                case CETriggerTypes.ZoneOrPanelInFire:
                case CETriggerTypes.ZoneHasDeviceInAlarm: _resetParamZonePanel = _data.ResetParam; break;

                case CETriggerTypes.OtherEventTriggered:
                case CETriggerTypes.NetworkEventTriggered: _resetParamEvent = _data.ResetParam; break;

                case CETriggerTypes.TimerEventTn: _resetParamTimer = _data.ResetParam; break;

                case CETriggerTypes.EventAnd: _resetParamEvent = _data.ResetParam; _resetParamEvent2 = _data.ResetParam2; break;

                case CETriggerTypes.ZoneAnd: _resetParamZone = _data.ResetParam; _resetParamZone2 = _data.ResetParam2; break;
            }

            _triggerType = _data.TriggerType != CETriggerTypes.None ? (int)_data.TriggerType : -1;
            _resetType   = _data.ResetType   != CETriggerTypes.None ? (int)_data.ResetType   : -1;

            //_triggerCondition = _data.TriggerParam >= 0 ? _data.TriggerCondition == true ? 0 : 1 : -1;
            //_resetCondition   = _data.ResetParam   >= 0 ? _data.ResetCondition   == true ? 0 : 1 : -1;

            TriggerCondition = _data.TriggerCondition;            
            ResetCondition   = _data.ResetCondition;            
        }


        public int CENumber => _number;
        public CEActionTypes ActionType    { get => _data?.ActionType ?? CEActionTypes.None; set { if (_data is not null) _data.ActionType = value; initData(); OnPropertyChanged(); updateSelections(); } }
        public int ActionParam             { get => _data.ActionParam;                       set { _data.ActionParam = value; OnPropertyChanged(); updateSelections(); } }
        public CETriggerTypes? TriggerType { get => _data.TriggerType;                       set { _triggerType = (int)value; _data.TriggerType = value??CETriggerTypes.None; initData(); OnPropertyChanged(); updateSelections(); } }
        public int TriggerParam            { get => _data.TriggerParam;                      set { _data.TriggerParam = value; OnPropertyChanged(); updateSelections(); } }
        public int TriggerParam2           { get => _data.TriggerParam2;                     set { _data.TriggerParam2 = value; OnPropertyChanged(); updateSelections(); } }
        public bool TriggerCondition       { get => _data.TriggerCondition;                  set { _data.TriggerCondition = value; OnPropertyChanged(); updateSelections(); } }
        public CETriggerTypes? ResetType   { get => _data.ResetType;                         set { _resetType = (int)value; _data.ResetType = value??CETriggerTypes.None; initData(); OnPropertyChanged(); updateSelections(); } }
        public int ResetParam              { get => _data.ResetParam;                        set { _data.ResetParam = value; OnPropertyChanged(); updateSelections(); } }
        public int ResetParam2             { get => _data.ResetParam2;                       set { _data.ResetParam2 = value; OnPropertyChanged(); updateSelections(); } }
        public bool ResetCondition         { get => _data.ResetCondition;                    set { _data.ResetCondition = value; OnPropertyChanged(); updateSelections(); } }


        private int _actionParamLoop1Device = -1;
        private int _actionParamLoop2Device = -1;
        private int _actionParamRelay = -1;
        private int _actionParamGroup = -1;
        private int _actionParamZone = -1;
        private int _actionParamSet = -1;
        private int _actionParamSetRelay = -1;
        private int _actionParamEvent = -1;
        private int _triggerType = -1;
        private int _triggerParamLoop1Device = -1;
        private int _triggerParamLoop2Device = -1;
        private int _triggerParamZonePanel = -1;
        private int _triggerParamInput = -1;
        private int _triggerParamTimer = -1;
        private int _triggerParamEvent = -1;
        private int _triggerParamEvent2 = -1;
        private int _triggerParamZone = -1;
        private int _triggerParamZone2 = -1;
        //private int _triggerCondition = -1;
        private int _resetType = -1;
        private int _resetParamLoop1Device = -1;
        private int _resetParamLoop2Device = -1;
        private int _resetParamZonePanel = -1;
        private int _resetParamInput = -1;
        private int _resetParamTimer = -1;
        private int _resetParamEvent = -1;
        private int _resetParamEvent2 = -1;
        private int _resetParamZone = -1;
        private int _resetParamZone2 = -1;
        //private int _resetCondition = -1;

        public string SelectedActionType
        {
            get => Enums.CEActionTypesToString(ActionType);
            set
            {
                var prev = ActionType;
                ActionType = Enums.StringToCEActionTypes(value);
                if (ActionType != prev)
                    clearDependants(ClearLevel.ActionType);
                updateSelections();
            }
        }

        //public string SelectedActionParam
        //{
        //    get
        //    {
        //        try
        //        {
        //            return ActionType switch
        //            {
        //                CEActionTypes.TriggerLoop1Device or
        //                CEActionTypes.Loop1DeviceDisable => getListString(Loop1Devices, _actionParamLoop1Device),

        //                CEActionTypes.TriggerLoop2Device or
        //                CEActionTypes.Loop2DeviceDisable => getListString(Loop2Devices, _actionParamLoop2Device),

        //                CEActionTypes.PanelRelay => getListString(Relays, _actionParamRelay),

        //                CEActionTypes.SounderAlert or
        //                CEActionTypes.SounderEvac or
        //                CEActionTypes.GroupDisable or
        //                CEActionTypes.TriggerBeacons => getListString(Groups, _actionParamGroup),

        //                CEActionTypes.ZoneDisable or
        //                CEActionTypes.PutZoneIntoAlarm => getListString(Zones, _actionParamZone),

        //                CEActionTypes.TriggerOutputSet => getListString(Sets, _actionParamSet),

        //                CEActionTypes.OutputDisable => getListString(SetsRelays, _actionParamSetRelay),

        //                CEActionTypes.TriggerNetworkEvent => getListString(Events, _actionParamEvent),

        //                _ => "",
        //            };
        //        }
        //        catch { return ""; }
        //    }
        //    set
        //    {
        //        var prev = ActionParam;
        //        ActionParam = ActionType switch
        //        {
        //            CEActionTypes.TriggerLoop1Device or
        //            CEActionTypes.Loop1DeviceDisable => _actionParamLoop1Device = getListIndex(Loop1Devices, value),

        //            CEActionTypes.TriggerLoop2Device or
        //            CEActionTypes.Loop2DeviceDisable => _actionParamLoop2Device = getListIndex(Loop2Devices, value),

        //            CEActionTypes.PanelRelay => _actionParamRelay = getListIndex(Relays, value),

        //            CEActionTypes.SounderAlert or
        //            CEActionTypes.SounderEvac or
        //            CEActionTypes.GroupDisable or
        //            CEActionTypes.TriggerBeacons => _actionParamGroup = getListIndex(Groups, value),

        //            CEActionTypes.ZoneDisable or
        //            CEActionTypes.PutZoneIntoAlarm => _actionParamZone = getListIndex(Zones, value),

        //            CEActionTypes.TriggerOutputSet => _actionParamSet = getListIndex(Sets, value),

        //            CEActionTypes.OutputDisable => _actionParamSetRelay = getListIndex(SetsRelays, value),

        //            CEActionTypes.TriggerNetworkEvent => _actionParamEvent = getListIndex(Events, value),

        //            _ => -1,
        //        };

        //        if (ActionParam != prev)
        //            clearDependants(ClearLevel.ActionParam);
        //        updateSelections();
        //    }
        //}
        public string SelectedActionParam
        {
            get
            {
                try
                {
                    return ActionType switch
                    {
                        CEActionTypes.TriggerLoop1Device or
                        CEActionTypes.Loop1DeviceDisable => getListString(Loop1Devices, _actionParamLoop1Device),

                        CEActionTypes.TriggerLoop2Device or
                        CEActionTypes.Loop2DeviceDisable => getListString(Loop2Devices, _actionParamLoop2Device),

                        CEActionTypes.PanelRelay => getListString(Relays, _actionParamRelay),

                        CEActionTypes.SounderAlert or
                        CEActionTypes.SounderEvac or
                        CEActionTypes.GroupDisable or
                        CEActionTypes.TriggerBeacons => getListString(Groups, _actionParamGroup),

                        CEActionTypes.ZoneDisable or
                        CEActionTypes.PutZoneIntoAlarm => getListString(Zones, _actionParamZone),

                        CEActionTypes.TriggerOutputSet => getListString(Sets, _actionParamSet),

                        CEActionTypes.OutputDisable => getListString(SetsRelays, _actionParamSetRelay),

                        CEActionTypes.TriggerNetworkEvent => getListString(Events, _actionParamEvent),

                        _ => "",
                    };
                }
                catch { return "";  }
            }
            set
            {
                var prev = ActionParam;
                ActionParam = ActionType switch
                {
                    CEActionTypes.TriggerLoop1Device or
                    CEActionTypes.Loop1DeviceDisable => _actionParamLoop1Device = getListIndex(Loop1Devices, value),

                    CEActionTypes.TriggerLoop2Device or
                    CEActionTypes.Loop2DeviceDisable => _actionParamLoop2Device = getListIndex(Loop2Devices, value),

                    CEActionTypes.PanelRelay => _actionParamRelay = getListIndex(Relays, value),

                    CEActionTypes.SounderAlert or
                    CEActionTypes.SounderEvac or
                    CEActionTypes.GroupDisable or
                    CEActionTypes.TriggerBeacons => _actionParamGroup = getListIndex(Groups, value),

                    CEActionTypes.ZoneDisable or
                    CEActionTypes.PutZoneIntoAlarm => _actionParamZone = getListIndex(Zones, value),

                    CEActionTypes.TriggerOutputSet => _actionParamSet = getListIndex(Sets, value),

                    CEActionTypes.OutputDisable => _actionParamSetRelay = getListIndex(SetsRelays, value),

                    CEActionTypes.TriggerNetworkEvent => _actionParamEvent = getListIndex(Events, value),

                    _ => -1,
                };

                if (ActionParam != prev)
                    clearDependants(ClearLevel.ActionParam);
                updateSelections();
            }
        }

        public int SelectedActionParamIndex
        {
            get
            {
                try
                {
                    return ActionType switch
                    {
                        CEActionTypes.TriggerLoop1Device or
                        CEActionTypes.Loop1DeviceDisable => getListIndex(Loop1Devices, _actionParamLoop1Device),

                        CEActionTypes.TriggerLoop2Device or
                        CEActionTypes.Loop2DeviceDisable => getListIndex(Loop2Devices, _actionParamLoop2Device),

                        CEActionTypes.PanelRelay => getListIndex(Relays, _actionParamRelay),

                        CEActionTypes.SounderAlert or
                        CEActionTypes.SounderEvac or
                        CEActionTypes.GroupDisable or
                        CEActionTypes.TriggerBeacons => getListIndex(Groups, _actionParamGroup),

                        CEActionTypes.ZoneDisable or
                        CEActionTypes.PutZoneIntoAlarm => getListIndex(Zones, _actionParamZone),

                        CEActionTypes.TriggerOutputSet => getListIndex(Sets, _actionParamSet),

                        CEActionTypes.OutputDisable => getListIndex(SetsRelays, _actionParamSetRelay),

                        CEActionTypes.TriggerNetworkEvent => getListIndex(Events, _actionParamEvent),

                        _ => -1,
                    };
                }
                catch { return -1; }
            }
            set
            {
                var prev = ActionParam;
                ActionParam = ActionType switch
                {
                    CEActionTypes.TriggerLoop1Device or
                    CEActionTypes.Loop1DeviceDisable => _actionParamLoop1Device = getListIndex(Loop1Devices, value),

                    CEActionTypes.TriggerLoop2Device or
                    CEActionTypes.Loop2DeviceDisable => _actionParamLoop2Device = getListIndex(Loop2Devices, value),

                    CEActionTypes.PanelRelay => _actionParamRelay = getListIndex(Relays, value),

                    CEActionTypes.SounderAlert or
                    CEActionTypes.SounderEvac or
                    CEActionTypes.GroupDisable or
                    CEActionTypes.TriggerBeacons => _actionParamGroup = getListIndex(Groups, value),

                    CEActionTypes.ZoneDisable or
                    CEActionTypes.PutZoneIntoAlarm => _actionParamZone = getListIndex(Zones, value),

                    CEActionTypes.TriggerOutputSet => _actionParamSet = getListIndex(Sets, value),

                    CEActionTypes.OutputDisable => _actionParamSetRelay = getListIndex(SetsRelays, value),

                    CEActionTypes.TriggerNetworkEvent => _actionParamEvent = getListIndex(Events, value),

                    _ => -1,
                };

                if (ActionParam != prev)
                    clearDependants(ClearLevel.ActionParam);
                updateSelections();
            }
        }

        public string SelectedTriggerType
        {
            get => Enums.CETriggerTypesToString((CETriggerTypes)TriggerType);
            set
            {
                var prev = TriggerType;
                TriggerType = Enums.StringToCETriggerTypes(value); 
                if (TriggerType != prev)
                    clearDependants(ClearLevel.TriggerType);
                updateSelections();
            }
        }

        public string SelectedTriggerParam
        {
            get
            {
                try
                {
                    return TriggerType switch
                    {
                        CETriggerTypes.Loop1DevicePrealarm or
                        CETriggerTypes.Loop1DeviceTriggered => getListString(Loop1Devices, _triggerParamLoop1Device),

                        CETriggerTypes.Loop2DevicePrealarm or
                        CETriggerTypes.Loop2DeviceTriggered => getListString(Loop2Devices, _triggerParamLoop2Device),

                        CETriggerTypes.ZoneOrPanelInFire or
                        CETriggerTypes.ZoneHasDeviceInAlarm => getListString(ZonesPanels, _triggerParamZonePanel),

                        CETriggerTypes.OtherEventTriggered or
                        CETriggerTypes.NetworkEventTriggered => getListString(Events, _triggerParamEvent),

                        CETriggerTypes.PanelInput => getListString(Inputs, _triggerParamInput),

                        CETriggerTypes.TimerEventTn => getListString(Times, _triggerParamTimer),

                        CETriggerTypes.EventAnd => getListString(Events, _triggerParamEvent),

                        CETriggerTypes.ZoneAnd => getListString(Zones, _triggerParamZone),

                        _ => "",
                    };
                }
                catch { return ""; }
            }
            set
            {
                var prev = TriggerParam;
                TriggerParam = TriggerType switch
                {
                    CETriggerTypes.Loop1DevicePrealarm or
                    CETriggerTypes.Loop1DeviceTriggered => _triggerParamLoop1Device = getListIndex(Loop1Devices, value),

                    CETriggerTypes.Loop2DevicePrealarm or
                    CETriggerTypes.Loop2DeviceTriggered => _triggerParamLoop2Device = getListIndex(Loop2Devices, value),

                    CETriggerTypes.ZoneOrPanelInFire or
                    CETriggerTypes.ZoneHasDeviceInAlarm => _triggerParamZonePanel = getListIndex(ZonesPanels, value),

                    CETriggerTypes.OtherEventTriggered or
                    CETriggerTypes.NetworkEventTriggered => _triggerParamEvent = getListIndex(Events, value),

                    CETriggerTypes.PanelInput => _triggerParamInput = getListIndex(Inputs, value),

                    CETriggerTypes.TimerEventTn => _triggerParamTimer = getListIndex(Times, value),

                    CETriggerTypes.EventAnd => _triggerParamEvent = getListIndex(Events, value),

                    CETriggerTypes.ZoneAnd => _triggerParamZone = getListIndex(Zones, value),

                    _ => -1,
                };

                if (TriggerParam != prev)
                    clearDependants(ClearLevel.TriggerParam);
                updateSelections();
            }
        }

        public int SelectedTriggerParamIndex
        {
            get
            {
                try
                {
                    return TriggerType switch
                    {
                        CETriggerTypes.Loop1DevicePrealarm or
                        CETriggerTypes.Loop1DeviceTriggered => getListIndex(Loop1Devices, _triggerParamLoop1Device),

                        CETriggerTypes.Loop2DevicePrealarm or
                        CETriggerTypes.Loop2DeviceTriggered => getListIndex(Loop2Devices, _triggerParamLoop2Device),

                        CETriggerTypes.ZoneOrPanelInFire or
                        CETriggerTypes.ZoneHasDeviceInAlarm => getListIndex(ZonesPanels, _triggerParamZonePanel),

                        CETriggerTypes.OtherEventTriggered or
                        CETriggerTypes.NetworkEventTriggered => getListIndex(Events, _triggerParamEvent),

                        CETriggerTypes.PanelInput => getListIndex(Inputs, _triggerParamInput),

                        CETriggerTypes.TimerEventTn => getListIndex(Times, _triggerParamTimer),

                        CETriggerTypes.EventAnd => getListIndex(EventNumbers, _triggerParamEvent),

                        CETriggerTypes.ZoneAnd => getListIndex(ZoneNumbers, _triggerParamZone),

                        _ => -1,
                    };
                }
                catch { return -1; }
            }
            set
            {
                var prev = TriggerParam;
                TriggerParam = TriggerType switch
                {
                    CETriggerTypes.Loop1DevicePrealarm or
                    CETriggerTypes.Loop1DeviceTriggered => _triggerParamLoop1Device = getListIndex(Loop1Devices, value),

                    CETriggerTypes.Loop2DevicePrealarm or
                    CETriggerTypes.Loop2DeviceTriggered => _triggerParamLoop2Device = getListIndex(Loop2Devices, value),

                    CETriggerTypes.ZoneOrPanelInFire or
                    CETriggerTypes.ZoneHasDeviceInAlarm => _triggerParamZonePanel = getListIndex(ZonesPanels, value),

                    CETriggerTypes.OtherEventTriggered or
                    CETriggerTypes.NetworkEventTriggered => _triggerParamEvent = getListIndex(Events, value),

                    CETriggerTypes.PanelInput => _triggerParamInput = getListIndex(Inputs, value),

                    CETriggerTypes.TimerEventTn => _triggerParamTimer = getListIndex(Times, value),

                    CETriggerTypes.EventAnd => _triggerParamEvent = getListIndex(EventNumbers, value),

                    CETriggerTypes.ZoneAnd => _triggerParamZone = getListIndex(ZoneNumbers, value),

                    _ => -1,
                };

                if (TriggerParam != prev)
                    clearDependants(ClearLevel.TriggerParam);
                updateSelections();
            }
        }

        public string SelectedTriggerParam2
        {
            get
            {
                try
                {   
                    return TriggerType switch
                    {
                        CETriggerTypes.EventAnd => getListString(Events, _triggerParamEvent2),
                        CETriggerTypes.ZoneAnd  => getListString(Zones,  _triggerParamZone2),
                        _ => "",
                    };
                }
                catch { return ""; }
            }
            set
            {
                var prev = TriggerParam2;
                TriggerParam2 = TriggerType switch
                {
                    CETriggerTypes.EventAnd => _triggerParamEvent2 = getListIndex(Events, value),
                    CETriggerTypes.ZoneAnd  => _triggerParamZone2  = getListIndex(Zones, value),
                    _ => -1,
                };

                if (TriggerParam2 != prev)
                    clearDependants(ClearLevel.TriggerParam);
                updateSelections();
            }
        }

        public int SelectedTriggerParam2Index
        {
            get
            {
                try
                {
                    return TriggerType switch
                    {
                        CETriggerTypes.EventAnd => getListIndex(EventNumbers, _triggerParamEvent2),
                        CETriggerTypes.ZoneAnd  => getListIndex(ZoneNumbers,  _triggerParamZone2),
                        _ => -1,
                    };
                }
                catch { return -1; }
            }
            set
            {
                var prev = TriggerParam2;
                TriggerParam2 = TriggerType switch
                {
                    CETriggerTypes.EventAnd => _triggerParamEvent2 = getListIndex(EventNumbers, value),
                    CETriggerTypes.ZoneAnd  => _triggerParamZone2  = getListIndex(ZoneNumbers, value),
                    _ => -1,
                };

                if (TriggerParam2 != prev)
                    clearDependants(ClearLevel.TriggerParam);
                updateSelections();
            }
        }

        public int SelectedTriggerConditionIndex
        {
            //get => _data.TriggerCondition is null ? -1 : _data.TriggerCondition == true ? 0 : 1;
            get => _data.TriggerCondition == true ? 0 : 1;
            set
            {
                //_data.TriggerCondition = value switch { 0 => true, 1 => false, _ => null };
                _data.TriggerCondition = value switch { 0 => true, _ => false };
                updateSelections();
            }
        }

        public string SelectedTriggerCondition
        {
            //get => _data.TriggerCondition switch { true => Cultures.Resources.True, false => Cultures.Resources.False, _ => "" };
            get => _data.TriggerCondition switch { true => Cultures.Resources.True, _ => Cultures.Resources.False };
            set
            {
                //_data.TriggerCondition = getListIndex(TrueOrFalse, value) switch { 0 => true, 1 => false, _ => null };
                _data.TriggerCondition = getListIndex(TrueOrFalse, value) switch { 0 => true, _ => false };
                updateSelections();
            }
        }

        public string SelectedResetType
        { 
            get => Enums.CETriggerTypesToString((CETriggerTypes)ResetType);
            set
            {
                var prev = ResetType;
                ResetType = Enums.StringToCETriggerTypes(value);
                if (ResetType != prev)
                    clearDependants(ClearLevel.ResetType);
                updateSelections();
            }
        }

        public string SelectedResetParam
        {
            get
            {
                try
                {
                    return ResetType switch
                    {
                        CETriggerTypes.Loop1DevicePrealarm or
                        CETriggerTypes.Loop1DeviceTriggered => getListString(Loop1Devices, _resetParamLoop1Device),

                        CETriggerTypes.Loop2DevicePrealarm or
                        CETriggerTypes.Loop2DeviceTriggered => getListString(Loop2Devices, _resetParamLoop2Device),

                        CETriggerTypes.ZoneOrPanelInFire or
                        CETriggerTypes.ZoneHasDeviceInAlarm => getListString(ZonesPanels, _resetParamZonePanel),

                        CETriggerTypes.OtherEventTriggered or
                        CETriggerTypes.NetworkEventTriggered => getListString(Events, _resetParamEvent),

                        CETriggerTypes.PanelInput => getListString(Inputs, _resetParamInput),

                        CETriggerTypes.TimerEventTn => getListString(Times, _resetParamTimer),

                        CETriggerTypes.EventAnd => getListString(EventNumbers, _resetParamEvent),

                        CETriggerTypes.ZoneAnd => getListString(ZoneNumbers, _resetParamZone),

                        _ => "",
                    };
                }
                catch { return ""; }
            }
            set
            {
                var prev = ResetParam;
                ResetParam = ResetType switch
                {
                    CETriggerTypes.Loop1DevicePrealarm or
                    CETriggerTypes.Loop1DeviceTriggered => _resetParamLoop1Device = getListIndex(Loop1Devices, value),

                    CETriggerTypes.Loop2DevicePrealarm or
                    CETriggerTypes.Loop2DeviceTriggered => _resetParamLoop2Device = getListIndex(Loop2Devices, value),

                    CETriggerTypes.ZoneOrPanelInFire or
                    CETriggerTypes.ZoneHasDeviceInAlarm => _resetParamZonePanel = getListIndex(ZonesPanels, value),

                    CETriggerTypes.OtherEventTriggered or
                    CETriggerTypes.NetworkEventTriggered => _resetParamEvent = getListIndex(Events, value),

                    CETriggerTypes.PanelInput => _resetParamInput = getListIndex(Inputs, value),

                    CETriggerTypes.TimerEventTn => _resetParamTimer = getListIndex(Times, value),

                    CETriggerTypes.EventAnd => _resetParamEvent = getListIndex(EventNumbers, value),

                    CETriggerTypes.ZoneAnd => _resetParamZone = getListIndex(ZoneNumbers, value),

                    _ => -1,
                };

                if (ResetParam != prev)
                    clearDependants(ClearLevel.ResetParam);
                updateSelections();
            }
        }

        public string SelectedResetParam2
        {
            get
            {
                try
                {
                    return ResetType switch
                    {
                        CETriggerTypes.EventAnd => getListString(EventNumbers, _resetParamEvent2),
                        CETriggerTypes.ZoneAnd => getListString(ZoneNumbers, _resetParamZone2),
                        _ => "",
                    };
                }
                catch { return ""; }
            }
            set
            {
                var prev = ResetParam2;
                ResetParam2 = ResetType switch
                {
                    CETriggerTypes.EventAnd => _resetParamEvent2 = getListIndex(EventNumbers, value),
                    CETriggerTypes.ZoneAnd  => _resetParamZone2  = getListIndex(ZoneNumbers, value),
                    _ => -1,
                };

                if (ResetParam2 != prev)
                    clearDependants(ClearLevel.ResetParam);
                updateSelections();
            }
        }

        public int SelectedResetConditionIndex
        {
            //get => _data.ResetCondition is null ? -1 : _data.ResetCondition == true ? 0 : 1;
            get => _data.ResetCondition == true ? 0 : 1;
            set
            {
                //_data.ResetCondition = value switch { 0 => true, 1 => false, _ => null };
                _data.ResetCondition = value switch { 0 => true, _ => false };
                OnPropertyChanged();
                updateSelections();
            }
        }
        
        public string SelectedResetCondition
        {
            //get => _data.ResetCondition switch { true => Cultures.Resources.True, false => Cultures.Resources.False, _ => "" };
            get => _data.ResetCondition switch { true => Cultures.Resources.True, _ => Cultures.Resources.False };
            set
            {
                //_data.ResetCondition = getListIndex(TrueOrFalse, value) switch { 0 => true, 1 => false, _ => null };
                _data.ResetCondition = getListIndex(TrueOrFalse, value) switch { 0 => true, _ => false };
                updateSelections();
            }
        }



        private enum ClearLevel { ActionType, ActionParam, TriggerType, TriggerParam, TriggerCondition, ResetType, ResetParam };
        private void clearDependants(ClearLevel level)
        {
            if (level <= ClearLevel.ActionType)       { ActionParam = -1; SelectedActionParamIndex = -1; SelectedActionParam = ""; }
            if (level <= ClearLevel.ActionParam)      { TriggerType = (CETriggerTypes)(-1); SelectedTriggerType = ""; }
            if (level <= ClearLevel.TriggerType)      { TriggerParam = TriggerParam2 = -1; SelectedTriggerParamIndex = SelectedTriggerParam2Index = -1; SelectedTriggerParam = SelectedTriggerParam2 = ""; }
            if (level <= ClearLevel.TriggerParam)     { TriggerCondition = true; SelectedTriggerConditionIndex = 0; }
            if (level <= ClearLevel.TriggerCondition) { ResetType = (CETriggerTypes)(-1); SelectedResetType = ""; }
            if (level <= ClearLevel.ResetType)        { ResetParam = ResetParam2 = -1; SelectedResetParam = SelectedResetParam2 = ""; }
            if (level <= ClearLevel.ResetParam)       { ResetCondition = false; SelectedResetConditionIndex = 1; }
        }


        #region option visibilities

        #region action
        public bool ShowActionParam => !_data.NoActionParam(ActionType);
        public bool ShowActionLoop1Devices => ActionType switch { CEActionTypes.TriggerLoop1Device or CEActionTypes.Loop1DeviceDisable => true, _ => false };
        public bool ShowActionLoop2Devices => ActionType switch { CEActionTypes.TriggerLoop2Device or CEActionTypes.Loop2DeviceDisable => true, _ => false };
        public bool ShowActionRelays => ActionType switch { CEActionTypes.PanelRelay => true, _ => false };
        public bool ShowActionGroups => ActionType switch { CEActionTypes.SounderAlert or CEActionTypes.SounderEvac or CEActionTypes.GroupDisable or CEActionTypes.TriggerBeacons => true, _ => false };
        public bool ShowActionZones => ActionType switch { CEActionTypes.ZoneDisable or CEActionTypes.PutZoneIntoAlarm => true, _ => false };
        public bool ShowActionSets => ActionType switch { CEActionTypes.TriggerOutputSet => true, _ => false };
        public bool ShowActionSetsAndRelays => ActionType switch { CEActionTypes.OutputDisable => true, _ => false };
        public bool ShowActionEvents => ActionType switch { CEActionTypes.TriggerNetworkEvent => true, _ => false };
        #endregion

        #region trigger
        public bool ShowTrigger => ActionType != CEActionTypes.None && (!ShowActionParam || SelectedActionParamIndex > -1);
        public bool ShowTriggerParam => ShowTrigger && !ShowTriggerParam2 && !_data.NoTriggerParam(TriggerType);
        public bool ShowTriggerParam2 => ShowTrigger && (TriggerType == CETriggerTypes.EventAnd || TriggerType == CETriggerTypes.ZoneAnd);
        public bool ShowTriggerLoop1Devices => ShowTriggerParam && TriggerType switch { CETriggerTypes.Loop1DeviceTriggered or CETriggerTypes.Loop1DevicePrealarm => true, _ => false };
        public bool ShowTriggerLoop2Devices => ShowTriggerParam && TriggerType switch { CETriggerTypes.Loop2DeviceTriggered or CETriggerTypes.Loop2DevicePrealarm => true, _ => false };
        public bool ShowTriggerZones => ShowTriggerParam && TriggerType switch { CETriggerTypes.ZoneAnd => true, _ => false };
        public bool ShowTriggerZones2 => ShowTriggerParam2 && TriggerType switch { CETriggerTypes.ZoneAnd => true, _ => false };
        public bool ShowTriggerZonesAndPanels => ShowTriggerParam && TriggerType switch { CETriggerTypes.ZoneOrPanelInFire or CETriggerTypes.ZoneHasDeviceInAlarm => true, _ => false };
        public bool ShowTriggerEvents => ShowTriggerParam && TriggerType switch { CETriggerTypes.OtherEventTriggered or CETriggerTypes.EventAnd => true, _ => false };
        public bool ShowTriggerEvents2 => ShowTriggerParam2 && TriggerType switch { CETriggerTypes.EventAnd => true, _ => false };
        public bool ShowTriggerTimes => ShowTriggerParam && TriggerType switch { CETriggerTypes.TimerEventTn => true, _ => false };
        public bool ShowTriggerInputs => ShowTriggerParam && TriggerType switch { CETriggerTypes.PanelInput => true, _ => false };
        public bool ShowTriggerCondition => ShowTriggerParam && TriggerParam >= 0 || ShowTriggerParam2 && TriggerParam >= 0 && TriggerParam2 >= 0 || _data.NoTriggerParam(TriggerType) && TriggerType != CETriggerTypes.None;
        #endregion

        #region reset
        public bool ShowReset => ShowTriggerCondition /*&& _data.TriggerCondition is not null*/;
        public bool ShowResetParam => ShowReset && !ShowResetParam2 && !_data.NoTriggerParam(ResetType);
        public bool ShowResetParam2 => ShowReset && (ResetType == CETriggerTypes.EventAnd || ResetType == CETriggerTypes.ZoneAnd);
        public bool ShowResetLoop1Devices => ShowResetParam && ResetType switch { CETriggerTypes.Loop1DeviceTriggered or CETriggerTypes.Loop1DevicePrealarm => true, _ => false };
        public bool ShowResetLoop2Devices => ShowResetParam && ResetType switch { CETriggerTypes.Loop2DeviceTriggered or CETriggerTypes.Loop2DevicePrealarm => true, _ => false };
        public bool ShowResetZones => ShowResetParam && ResetType switch { CETriggerTypes.ZoneAnd => true, _ => false };
        public bool ShowResetZones2 => ShowResetParam2 && ResetType switch { CETriggerTypes.ZoneAnd => true, _ => false };
        public bool ShowResetZonesAndPanels => ShowResetParam && ResetType switch { CETriggerTypes.ZoneOrPanelInFire or CETriggerTypes.ZoneHasDeviceInAlarm => true, _ => false };
        public bool ShowResetEvents => ShowResetParam && ResetType switch { CETriggerTypes.OtherEventTriggered or CETriggerTypes.EventAnd => true, _ => false };//ShowResetParam && ResetType switch { CETriggerTypes.OtherEventTriggered or CETriggerTypes.EventAnd or CETriggerTypes.NetworkEventTriggered => true, _ => false };
        public bool ShowResetEvents2 => ShowResetParam2 && ResetType switch { CETriggerTypes.EventAnd => true, _ => false };
        public bool ShowResetTimes => ShowResetParam && ResetType switch { CETriggerTypes.TimerEventTn => true, _ => false };
        public bool ShowResetInputs => ShowResetParam && ResetType switch { CETriggerTypes.PanelInput => true, _ => false };
        public bool ShowResetCondition => ShowResetParam && ResetParam >= 0 || ShowResetParam2 && ResetParam >= 0 && ResetParam2 >= 0 || _data.NoTriggerParam(ResetType) && ResetType != CETriggerTypes.None;
        #endregion

        #endregion

        #region option validation
        public bool ActionParamIsValid => !ShowActionParam || SelectedActionParamIndex >  -1;
        public bool ActionTriggerIsValid => !ShowTrigger || SelectedTriggerType != "";

        public bool TriggerParam1IsValid => !ShowTriggerParam || SelectedTriggerParamIndex > -1 || TriggerType == CETriggerTypes.None;
        public bool IndicateInvalidTriggerParam2 => !(!ShowTriggerParam2 || !(!ShowTriggerParam2 || SelectedTriggerParamIndex < 0 && SelectedTriggerParam2Index < 0));
        public bool IndicateInvalidSelectedTriggerParam1 => !((!ShowTriggerEvents2 || SelectedTriggerParamIndex > -1 || SelectedTriggerParam2Index < 0) && (!ShowTriggerZones2 || SelectedTriggerParamIndex > -1 || SelectedTriggerParam2Index < 0));
        public bool IndicateInvalidSelectedTriggerParam2 => !((!ShowTriggerEvents2 || SelectedTriggerParam2Index > -1 || SelectedTriggerParamIndex < 0) && (!ShowTriggerZones2 || SelectedTriggerParam2Index > -1 || SelectedTriggerParamIndex < 0));
        public bool TriggerConditionIsValid => !ShowTriggerCondition || SelectedTriggerConditionIndex != -1;

        public bool ResetTriggerIsValid => !ShowReset || SelectedResetType != "";
        public bool ResetParam1IsValid => !ShowResetParam || SelectedResetParam != "" || ResetType == CETriggerTypes.None;
        public bool IndicateInvalidResetParam2 => !(!ShowResetParam2 || !(!ShowResetParam2 || SelectedResetParam == "" && SelectedResetParam2 == ""));
        public bool IndicateInvalidSelectedResetParam1 => !((!ShowResetEvents2 || SelectedResetParam != "" || SelectedResetParam2 == "") && (!ShowResetZones2 || SelectedResetParam != "" || SelectedResetParam2 == ""));
        public bool IndicateInvalidSelectedResetParam2 => !((!ShowResetEvents2 || SelectedResetParam2 != "" || SelectedResetParam == "") && (!ShowResetZones2 || SelectedResetParam2 != "" || SelectedResetParam == ""));
        public bool ResetConditionIsValid => !ShowResetCondition || SelectedResetConditionIndex != -1;
        #endregion


        private void updateSelections()
        {   
            OnPropertyChanged(nameof(CENumber));
            OnPropertyChanged(nameof(ActionType));
            OnPropertyChanged(nameof(ActionParam));
            OnPropertyChanged(nameof(TriggerType));
            OnPropertyChanged(nameof(TriggerParam));
            OnPropertyChanged(nameof(TriggerParam2));
            OnPropertyChanged(nameof(TriggerCondition));
            OnPropertyChanged(nameof(ResetType));
            OnPropertyChanged(nameof(ResetParam));
            OnPropertyChanged(nameof(ResetParam2));
            OnPropertyChanged(nameof(ResetCondition));

            OnPropertyChanged(nameof(SelectedActionType));
            //OnPropertyChanged(nameof(SelectedActionParam));
            OnPropertyChanged(nameof(SelectedActionParamIndex));
            OnPropertyChanged(nameof(SelectedTriggerType));
            OnPropertyChanged(nameof(SelectedTriggerParamIndex));
            OnPropertyChanged(nameof(SelectedTriggerParam2Index));
            OnPropertyChanged(nameof(SelectedResetType));
            OnPropertyChanged(nameof(SelectedResetParam));
            OnPropertyChanged(nameof(SelectedResetParam2));
            OnPropertyChanged(nameof(SelectedTriggerConditionIndex));
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
            OnPropertyChanged(nameof(ShowTrigger));
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
            OnPropertyChanged(nameof(ShowReset));
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
            OnPropertyChanged(nameof(ActionTriggerIsValid));
            OnPropertyChanged(nameof(TriggerParam1IsValid));
            OnPropertyChanged(nameof(IndicateInvalidTriggerParam2));
            OnPropertyChanged(nameof(IndicateInvalidSelectedTriggerParam1));
            OnPropertyChanged(nameof(IndicateInvalidSelectedTriggerParam2));
            OnPropertyChanged(nameof(TriggerConditionIsValid));
            OnPropertyChanged(nameof(ResetTriggerIsValid));
            OnPropertyChanged(nameof(ResetParam1IsValid));
            OnPropertyChanged(nameof(IndicateInvalidResetParam2));
            OnPropertyChanged(nameof(IndicateInvalidSelectedResetParam1));
            OnPropertyChanged(nameof(IndicateInvalidSelectedResetParam2));
            OnPropertyChanged(nameof(ResetConditionIsValid));
        }


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
        public MenuList ZoneNumbersMenu;
        public MenuList ZonesPanelsMenu;
        public MenuList GroupsMenu;
        public MenuList SetsMenu;
        public MenuList EventsMenu;
        public MenuList EventNumbersMenu;
        public MenuList RelaysMenu;
        public MenuList SetsRelaysMenu;
        public MenuList TimesMenu;
        public MenuList TrueOrFalseMenu;

        public List<string> Actions      { get => ActionsMenu?.Invoke(); }
        public List<string> Triggers     { get => TriggersMenu?.Invoke(); }
        public List<string> Inputs       { get => InputsMenu?.Invoke(); }
        public List<string> Loop1Devices { get => Loop1DevicesMenu?.Invoke(); }
        public List<string> Loop2Devices { get => Loop2DevicesMenu?.Invoke(); }
        public List<string> Zones        { get => ZonesMenu?.Invoke(); }
        public List<string> ZoneNumbers  { get => ZoneNumbersMenu?.Invoke(); }
        public List<string> ZonesPanels  { get => ZonesPanelsMenu?.Invoke(); }
        public List<string> Groups       { get => GroupsMenu?.Invoke(); }
        public List<string> Sets         { get => SetsMenu?.Invoke(); }
        public List<string> Events       { get => EventsMenu?.Invoke(); }
        public List<string> EventNumbers { get => EventNumbersMenu?.Invoke(); }
        public List<string> Relays       { get => RelaysMenu?.Invoke(); }
        public List<string> SetsRelays   { get => SetsRelaysMenu?.Invoke(); }
        public List<string> Times        { get => TimesMenu?.Invoke(); }
        public List<string> TrueOrFalse  { get => TrueOrFalseMenu?.Invoke(); }


        private int getListIndex(List<string> list, string item)
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i] == item)
                    return i;
            return -1;
        }

        private string getListString(List<string> list, int? index)
        {
            if (index is not null && index >= 0 && index < list.Count)
                return list[(int)index];
            return "";
        }
        private int getListIndex(List<string> list, int? index)
        {
            if (index is not null && index >= 0 && index < list.Count)
                return (int)index;
            return -1;
        }
        #endregion


        #region IAppViewModel implementation
        public void SetCulture(CultureInfo culture)
        {
            RefreshView();
        }
        #endregion


        #region IPanelToolsViewModel implementation
        public void PopulateView(XfpData data)
        {
            OnPropertyChanged(nameof(SelectedActionType));
            OnPropertyChanged(nameof(SelectedActionParamIndex));
            OnPropertyChanged(nameof(SelectedTriggerType));
            OnPropertyChanged(nameof(SelectedTriggerParamIndex));
            OnPropertyChanged(nameof(SelectedTriggerParam2Index));
            OnPropertyChanged(nameof(SelectedTriggerConditionIndex));
            OnPropertyChanged(nameof(SelectedTriggerCondition));
            OnPropertyChanged(nameof(SelectedResetType));
            OnPropertyChanged(nameof(SelectedResetParam));
            OnPropertyChanged(nameof(SelectedResetParam2));
            OnPropertyChanged(nameof(SelectedResetConditionIndex));
            OnPropertyChanged(nameof(SelectedResetCondition));
        }

        //public void RefreshView(int errorCode = 0)
        public void RefreshView()
        {
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

            OnPropertyChanged(nameof(CENumber));
            OnPropertyChanged(nameof(ActionType));
            OnPropertyChanged(nameof(ActionParam));
            OnPropertyChanged(nameof(TriggerType));
            OnPropertyChanged(nameof(TriggerParam));
            OnPropertyChanged(nameof(TriggerParam2));
            OnPropertyChanged(nameof(TriggerCondition));
            OnPropertyChanged(nameof(ResetType));
            OnPropertyChanged(nameof(ResetParam));
            OnPropertyChanged(nameof(ResetParam2));
            OnPropertyChanged(nameof(ResetCondition));

            OnPropertyChanged(nameof(SelectedActionType));
            OnPropertyChanged(nameof(SelectedActionParam));
            OnPropertyChanged(nameof(SelectedActionParamIndex));
            OnPropertyChanged(nameof(SelectedTriggerType));
            OnPropertyChanged(nameof(SelectedTriggerParam));
            OnPropertyChanged(nameof(SelectedTriggerParamIndex));
            OnPropertyChanged(nameof(SelectedTriggerParam2Index));
            OnPropertyChanged(nameof(SelectedTriggerConditionIndex));
            OnPropertyChanged(nameof(SelectedTriggerCondition));
            OnPropertyChanged(nameof(SelectedResetType));
            OnPropertyChanged(nameof(SelectedResetParam));
            OnPropertyChanged(nameof(SelectedResetParam2));
            OnPropertyChanged(nameof(SelectedResetConditionIndex));
            OnPropertyChanged(nameof(SelectedResetCondition));

            OnPropertyChanged(nameof(ActionParamIsValid));
            OnPropertyChanged(nameof(ActionTriggerIsValid));
            OnPropertyChanged(nameof(TriggerParam1IsValid));
            OnPropertyChanged(nameof(IndicateInvalidTriggerParam2));
            OnPropertyChanged(nameof(IndicateInvalidSelectedTriggerParam1));
            OnPropertyChanged(nameof(IndicateInvalidSelectedTriggerParam2));
            OnPropertyChanged(nameof(TriggerConditionIsValid));
            OnPropertyChanged(nameof(ResetTriggerIsValid));
            OnPropertyChanged(nameof(ResetParam1IsValid));
            OnPropertyChanged(nameof(IndicateInvalidResetParam2));
            OnPropertyChanged(nameof(IndicateInvalidSelectedResetParam1));
            OnPropertyChanged(nameof(IndicateInvalidSelectedResetParam2));
            OnPropertyChanged(nameof(ResetConditionIsValid));

            updateSelections();
        }
        #endregion
    }
}
