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
            InitFixedComboLists();
            initData();
            RefreshView();
            
            /// <summary>Add update to Times menu</summary>
            _timesTimer.Elapsed += new((s, e) => OnPropertyChanged(nameof(Times)));
        }

        protected static System.Timers.Timer _timesTimer = new() { AutoReset = true, Enabled = true, Interval = 2000 };


        private int     _number;
        private CEEvent _data;
        //private LoopConfigData _loopConfig;

        public CEEvent Data { get => _data; set { _data = value; initData(); RefreshView(); } }


        private void initData()
        {
            #region stuff
            /*_actionParamLoop1Device = _actionParamLoop2Device =*/ /*_actionParamGroup =*/ /*_actionParamZone =*/ /*_actionParamSet =*/ /*_actionParamRelay =*/ /*_actionParamSetRelay =*/ /*_actionParamEvent = -1;*/
            /*_triggerParamLoop1Device = _triggerParamLoop2Device =*/ /*_triggerParamInput =*/ /*_triggerParamZonePanel =*/ /*_triggerParamEvent =*/ /*_triggerParamTimer =*/ /*_triggerParamEvent =*/ /*_triggerParamEvent2 =*/ /*_triggerParamZone = _triggerParamZone2 =*/ /*-1;*/
            /*_resetParamLoop1Device =*/ /*_resetParamLoop2Device =*/ /*_resetParamInput =*/ /*_resetParamZonePanel =*/ /*_resetParamEvent =*/ /*_resetParamTimer =*/ /*_resetParamEvent =*/ /*_resetParamEvent2 = *//*_resetParamZone =*/ /*_resetParamZone2 =*/ /*-1;*/

            //switch (_data.ActionType)
            //{
            //    //case CEActionTypes.TriggerLoop1Device:
            //    //case CEActionTypes.Loop1DeviceDisable: _actionParamLoop1Device = _data.ActionParam; break;

            //    //case CEActionTypes.TriggerLoop2Device:
            //    //case CEActionTypes.Loop2DeviceDisable: _actionParamLoop2Device = _data.ActionParam; break;

            //    //case CEActionTypes.PanelRelay: _actionParamRelay = _data.ActionParam; break;

            //    //case CEActionTypes.SounderAlert:
            //    //case CEActionTypes.SounderEvac:
            //    //case CEActionTypes.GroupDisable:
            //    //case CEActionTypes.TriggerBeacons: _actionParamGroup = _data.ActionParam; break;

            //    //case CEActionTypes.ZoneDisable:
            //    //case CEActionTypes.PutZoneIntoAlarm: _actionParamZone = _data.ActionParam; break;

            //    //case CEActionTypes.TriggerOutputSet: _actionParamSet = _data.ActionParam; break;

            //    //case CEActionTypes.OutputDisable: _actionParamSetRelay = _data.ActionParam; break;

            //    //case CEActionTypes.TriggerNetworkEvent: _actionParamEvent = _data.ActionParam; break;
            //}

            //switch (_data.TriggerType)
            //{
            //    //case CETriggerTypes.Loop1DeviceTriggered:
            //    //case CETriggerTypes.Loop1DevicePrealarm: _triggerParamLoop1Device = _data.TriggerParam  ; break;

            //    //case CETriggerTypes.Loop2DeviceTriggered:
            //    //case CETriggerTypes.Loop2DevicePrealarm: _triggerParamLoop2Device = _data.TriggerParam  ; break;

            //    //case CETriggerTypes.PanelInput: _triggerParamInput = _data.TriggerParam; break;

            //    //case CETriggerTypes.ZoneOrPanelInFire:
            //    //case CETriggerTypes.ZoneHasDeviceInAlarm: _triggerParamZonePanel = _data.TriggerParam; break;

            //    //case CETriggerTypes.OtherEventTriggered:
            //    //case CETriggerTypes.NetworkEventTriggered: _triggerParamEvent = _data.TriggerParam; break;

            //    //case CETriggerTypes.TimerEventTn: _triggerParamTimer = _data.TriggerParam; break;

            //    //case CETriggerTypes.EventAnd: /*_triggerParamEvent = _data.TriggerParam;*/ _triggerParamEvent2 = _data.TriggerParam2; break;

            //    //case CETriggerTypes.ZoneAnd: _triggerParamZone = _data.TriggerParam; _triggerParamZone2 = _data.TriggerParam2; break;
            //}

            //switch (_data.ResetType)
            //{
            //    //case CETriggerTypes.Loop1DeviceTriggered:
            //    //case CETriggerTypes.Loop1DevicePrealarm: _resetParamLoop1Device = _data.ResetParam; break;

            //    //case CETriggerTypes.Loop2DeviceTriggered:
            //    //case CETriggerTypes.Loop2DevicePrealarm: _resetParamLoop2Device = _data.ResetParam; break;

            //    //case CETriggerTypes.PanelInput: _resetParamInput = _data.ResetParam; break;

            //    //case CETriggerTypes.ZoneOrPanelInFire:
            //    //case CETriggerTypes.ZoneHasDeviceInAlarm: _resetParamZonePanel = _data.ResetParam; break;

            //    //case CETriggerTypes.OtherEventTriggered:
            //    //case CETriggerTypes.NetworkEventTriggered: _resetParamEvent = _data.ResetParam; break;

            //    //case CETriggerTypes.TimerEventTn: _resetParamTimer = _data.ResetParam; break;

            //    //case CETriggerTypes.EventAnd: _resetParamEvent = _data.ResetParam; _resetParamEvent2 = _data.ResetParam2; break;

            //    //case CETriggerTypes.ZoneAnd: _resetParamZone = _data.ResetParam; _resetParamZone2 = _data.ResetParam2; break;
            //}
            #endregion

            _triggerType = _data.TriggerType != CETriggerTypes.None ? (int)_data.TriggerType : -1;
            _resetType   = _data.ResetType   != CETriggerTypes.None ? (int)_data.ResetType   : -1;

            //_triggerCondition = _data.TriggerParam >= 0 ? _data.TriggerCondition == true ? 0 : 1 : -1;
            //_resetCondition   = _data.ResetParam   >= 0 ? _data.ResetCondition   == true ? 0 : 1 : -1;

            //TriggerCondition = _data.TriggerCondition;            
            //ResetCondition   = _data.ResetCondition;            
        }


        public int CENumber => _number;
        //public CEActionTypes ActionType    { get => _data?.ActionType ?? CEActionTypes.None; set { if (_data is not null) _data.ActionType = value; initData(); OnPropertyChanged(); updateSelections(); } }
        //public int ActionParam             { get => _data.ActionParam;                       set { _data.ActionParam = value; OnPropertyChanged(); updateSelections(); } }
        //public CETriggerTypes? TriggerType { get => _data.TriggerType;                       set { _triggerType = (int)value; _data.TriggerType = value??CETriggerTypes.None; initData(); OnPropertyChanged(); updateSelections(); } }
        //public int TriggerParam            { get => _data.TriggerParam;                      set { _data.TriggerParam = value; OnPropertyChanged(); updateSelections(); } }
        //public int TriggerParam2           { get => _data.TriggerParam2;                     set { _data.TriggerParam2 = value; OnPropertyChanged(); updateSelections(); } }
        //public bool TriggerCondition       { get => _data.TriggerCondition;                  set { _data.TriggerCondition = value; OnPropertyChanged(); updateSelections(); } }
        //public CETriggerTypes? ResetType   { get => _data.ResetType;                         set { _resetType = (int)value; _data.ResetType = value??CETriggerTypes.None; initData(); OnPropertyChanged(); updateSelections(); } }
        //public int ResetParam              { get => _data.ResetParam;                        set { _data.ResetParam = value; OnPropertyChanged(); updateSelections(); } }
        //public int ResetParam2             { get => _data.ResetParam2;                       set { _data.ResetParam2 = value; OnPropertyChanged(); updateSelections(); } }
        //public bool ResetCondition         { get => _data.ResetCondition;                    set { _data.ResetCondition = value; OnPropertyChanged(); updateSelections(); } }


        #region stuff
        //private int _actionParamLoop1Device = -1;
        //private int _actionParamLoop2Device = -1;
        //private int _actionParamRelay = -1;
        //private int _actionParamGroup = -1;
        //private int _actionParamZone = -1;
        //private int _actionParamSet = -1;
        //private int _actionParamSetRelay = -1;
        //private int _actionParamEvent = -1;
        private int _triggerType = -1;
        //private int _triggerParamLoop1Device = -1;
        //private int _triggerParamLoop2Device = -1;
        //private int _triggerParamZonePanel = -1;
        //private int _triggerParamInput = -1;
        //private int _triggerParamTimer = -1;
        //private int _triggerParamEvent = -1;
        //private int _triggerParamEvent2 = -1;
        //private int _triggerParamZone = -1;
        //private int _triggerParamZone2 = -1;
        private int _resetType = -1;
        //private int _resetParamLoop1Device = -1;
        //private int _resetParamLoop2Device = -1;
        //private int _resetParamZonePanel = -1;
        //private int _resetParamInput = -1;
        //private int _resetParamTimer = -1;
        //private int _resetParamEvent = -1;
        //private int _resetParamEvent2 = -1;
        //private int _resetParamZone = -1;
        //private int _resetParamZone2 = -1;
        #endregion
        

        private string getActionParamString(int index)
        {
            try
            {
                return _data.ActionType switch
                {
                    CEActionTypes.TriggerLoop1Device or
                    CEActionTypes.Loop1DeviceDisable => getListString(Loop1Devices, index),

                    CEActionTypes.TriggerLoop2Device or
                    CEActionTypes.Loop2DeviceDisable => getListString(Loop2Devices, index),

                    CEActionTypes.PanelRelay => getListString(Relays, index),

                    CEActionTypes.SounderAlert or
                    CEActionTypes.SounderEvac or
                    CEActionTypes.GroupDisable or
                    CEActionTypes.TriggerBeacons => getListString(Groups, index),

                    CEActionTypes.ZoneDisable or
                    CEActionTypes.PutZoneIntoAlarm => getListString(Zones, index),

                    CEActionTypes.TriggerOutputSet => getListString(Sets, index),

                    CEActionTypes.OutputDisable => getListString(SetsRelays, index),

                    CEActionTypes.TriggerNetworkEvent => getListString(Events, index),

                    _ => "",
                };
            }
            catch { return ""; }
        }

        public string getTriggerParamString(int index)
        {
            try
            {
                return _data.TriggerType switch
                {
                    CETriggerTypes.Loop1DevicePrealarm or
                    CETriggerTypes.Loop1DeviceTriggered => getListString(Loop1Devices, index),

                    CETriggerTypes.Loop2DevicePrealarm or
                    CETriggerTypes.Loop2DeviceTriggered => getListString(Loop2Devices, index),

                    CETriggerTypes.ZoneOrPanelInFire or
                    CETriggerTypes.ZoneHasDeviceInAlarm => getListString(ZonesPanels, index),

                    CETriggerTypes.OtherEventTriggered or
                    CETriggerTypes.NetworkEventTriggered => getListString(Events, index),

                    CETriggerTypes.PanelInput => getListString(Inputs, index),

                    CETriggerTypes.TimerEventTn => getListString(Times, index),

                    CETriggerTypes.EventAnd => getListString(EventNumbers, index),

                    CETriggerTypes.ZoneAnd => getListString(ZoneNumbers, index),

                    _ => "",
                };
            }
            catch { return ""; }
        }

        private int getTriggerParamIndex(string item)
        {
            return _data.TriggerType switch
            {
                CETriggerTypes.Loop1DevicePrealarm or
                CETriggerTypes.Loop1DeviceTriggered => _data.TriggerParam = getListIndex(Loop1Devices, item) - 1,

                CETriggerTypes.Loop2DevicePrealarm or
                CETriggerTypes.Loop2DeviceTriggered => _data.TriggerParam = getListIndex(Loop2Devices, item) - 1,

                CETriggerTypes.ZoneOrPanelInFire or
                CETriggerTypes.ZoneHasDeviceInAlarm => _data.TriggerParam = getListIndex(ZonesPanels, item) - 1,

                CETriggerTypes.OtherEventTriggered or
                CETriggerTypes.NetworkEventTriggered => _data.TriggerParam = getListIndex(Events, item) - 1,

                CETriggerTypes.PanelInput => _data.TriggerParam = getListIndex(Inputs, item) - 1,

                CETriggerTypes.TimerEventTn => _data.TriggerParam = getListIndex(Times, item) - 1,

                CETriggerTypes.EventAnd => _data.TriggerParam = getListIndex(EventNumbers, item) - 1,

                CETriggerTypes.ZoneAnd => _data.TriggerParam = getListIndex(ZoneNumbers, item) - 1,

                _ => -1,
            };
        }
        

        public int SelectedActionParamIndex
        {
            get => _data.ActionParam + 1;
            set
            {
                var prev = _data.ActionParam;
                _data.ActionParam = _data.ActionParam = value - 1;

                if (_data.ActionParam != prev)
                    clearDependants(ClearLevel.ActionParam);
                updateSelections();
            }
        }

        public int SelectedTriggerParamIndex
        {
            get => _data.TriggerParam + 1;
            set
            {
                var prev = _data.TriggerParam;
                _data.TriggerParam = _data.TriggerParam = value - 1;

                if (_data.TriggerParam != prev)
                    clearDependants(ClearLevel.TriggerParam);
                updateSelections();
            }
        }


        public string SelectedActionType
        {
            get => Enums.CEActionTypesToString(_data.ActionType);
            set
            {
                var prev = _data.ActionType;
                _data.ActionType = Enums.StringToCEActionTypes(value);
                if (_data.ActionType != prev)
                    clearDependants(ClearLevel.ActionType);
                updateSelections();
            }
        }

        public string SelectedActionParam
        {
            //get
            //{
            //    //try
            //    //{
            //    //    //return ActionType switch
            //    //    //{
            //    //    //    CEActionTypes.TriggerLoop1Device or
            //    //    //    CEActionTypes.Loop1DeviceDisable => getListString(Loop1Devices, _data.ActionParam + 1),

            //    //    //    CEActionTypes.TriggerLoop2Device or
            //    //    //    CEActionTypes.Loop2DeviceDisable => getListString(Loop2Devices, _data.ActionParam + 1),

            //    //    //    CEActionTypes.PanelRelay => getListString(Relays, _data.ActionParam + 1),

            //    //    //    CEActionTypes.SounderAlert or
            //    //    //    CEActionTypes.SounderEvac or
            //    //    //    CEActionTypes.GroupDisable or
            //    //    //    CEActionTypes.TriggerBeacons => getListString(Groups, _data.ActionParam + 1),

            //    //    //    CEActionTypes.ZoneDisable or
            //    //    //    CEActionTypes.PutZoneIntoAlarm => getListString(Zones, _data.ActionParam + 1),

            //    //    //    CEActionTypes.TriggerOutputSet => getListString(Sets, _data.ActionParam + 1),

            //    //    //    CEActionTypes.OutputDisable => getListString(SetsRelays, _data.ActionParam + 1),

            //    //    //    CEActionTypes.TriggerNetworkEvent => getListString(Events, _data.ActionParam + 1),

            //    //    //    _ => "",
            //    //    //};
            //    //}
            //    //catch { return "";  }

            //}
            get => getActionParamString(_data.ActionParam + 1);
            set
            {
                var prev = _data.ActionParam;
                _data.ActionParam = _data.ActionType switch
                {
                    CEActionTypes.TriggerLoop1Device or
                    CEActionTypes.Loop1DeviceDisable => _data.ActionParam = getListIndex(Loop1Devices, value) - 1,

                    CEActionTypes.TriggerLoop2Device or
                    CEActionTypes.Loop2DeviceDisable => _data.ActionParam = getListIndex(Loop2Devices, value) - 1,

                    CEActionTypes.PanelRelay => _data.ActionParam = getListIndex(Relays, value) - 1,

                    CEActionTypes.SounderAlert or
                    CEActionTypes.SounderEvac or
                    CEActionTypes.GroupDisable or
                    CEActionTypes.TriggerBeacons => _data.ActionParam = getListIndex(Groups, value) - 1,

                    CEActionTypes.ZoneDisable or
                    CEActionTypes.PutZoneIntoAlarm => _data.ActionParam = getListIndex(Zones, value) - 1,

                    CEActionTypes.TriggerOutputSet => _data.ActionParam = getListIndex(Sets, value) - 1,

                    CEActionTypes.OutputDisable => _data.ActionParam = getListIndex(SetsRelays, value) - 1,

                    CEActionTypes.TriggerNetworkEvent => _data.ActionParam = getListIndex(Events, value) - 1,

                    _ => -1,
                };

                if (_data.ActionParam != prev)
                    clearDependants(ClearLevel.ActionParam);
                updateSelections();
            }
        }

        public string SelectedTriggerType
        {
            get => Enums.CETriggerTypesToString((CETriggerTypes)_data.TriggerType);
            set
            {
                var prev = _data.TriggerType;
                _data.TriggerType = Enums.StringToCETriggerTypes(value); 
                if (_data.TriggerType != prev)
                    clearDependants(ClearLevel.TriggerType);
                updateSelections();
            }
        }

        public string SelectedTriggerParam
        {
            get => getTriggerParamString(_data.TriggerParam + 1);
            set
            {
                var prev = _data.TriggerParam;
                _data.TriggerParam = getTriggerParamIndex(value);
                //_data.TriggerType switch
                //{
                //    CETriggerTypes.Loop1DevicePrealarm or
                //    CETriggerTypes.Loop1DeviceTriggered => _data.TriggerParam = getListIndex(Loop1Devices, value) - 1,

                //    CETriggerTypes.Loop2DevicePrealarm or
                //    CETriggerTypes.Loop2DeviceTriggered => _data.TriggerParam = getListIndex(Loop2Devices, value) - 1,

                //    CETriggerTypes.ZoneOrPanelInFire or
                //    CETriggerTypes.ZoneHasDeviceInAlarm => _data.TriggerParam = getListIndex(ZonesPanels, value) - 1,

                //    CETriggerTypes.OtherEventTriggered or
                //    CETriggerTypes.NetworkEventTriggered => _data.TriggerParam = getListIndex(Events, value) - 1,

                //    CETriggerTypes.PanelInput => _data.TriggerParam = getListIndex(Inputs, value) - 1,

                //    CETriggerTypes.TimerEventTn => _data.TriggerParam = getListIndex(Times, value) - 1,

                //    CETriggerTypes.EventAnd => _data.TriggerParam = getListIndex(EventNumbers, value) - 1,

                //    CETriggerTypes.ZoneAnd => _data.TriggerParam = getListIndex(ZoneNumbers, value) - 1,

                //    _ => -1,
                //};

                if (_data.TriggerParam != prev)
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
                    return _data.TriggerType switch
                    {
                        CETriggerTypes.EventAnd => getListString(EventNumbers, _data.TriggerParam2 + 1),
                        CETriggerTypes.ZoneAnd  => getListString(ZoneNumbers, _data.TriggerParam2 + 1),
                        _ => "",
                    };
                }
                catch { return ""; }
            }
            set
            {
                var prev = _data.TriggerParam2;
                _data.TriggerParam2 = _data.TriggerType switch
                {
                    CETriggerTypes.EventAnd => _data.TriggerParam2 = getListIndex(EventNumbers, value) - 1,
                    CETriggerTypes.ZoneAnd  => _data.TriggerParam2 = getListIndex(ZoneNumbers, value) - 1,
                    _ => -1,
                };

                if (_data.TriggerParam2 != prev)
                    clearDependants(ClearLevel.TriggerParam);
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
            get => Enums.CETriggerTypesToString((CETriggerTypes)_data.ResetType);
            set
            {
                var prev = _data.ResetType;
                _data.ResetType = Enums.StringToCETriggerTypes(value);
                if (_data.ResetType != prev)
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
                    return _data.ResetType switch
                    {
                        CETriggerTypes.Loop1DevicePrealarm or
                        CETriggerTypes.Loop1DeviceTriggered => getListString(Loop1Devices, _data.ResetParam + 1),

                        CETriggerTypes.Loop2DevicePrealarm or
                        CETriggerTypes.Loop2DeviceTriggered => getListString(Loop2Devices, _data.ResetParam + 1),

                        CETriggerTypes.ZoneOrPanelInFire or
                        CETriggerTypes.ZoneHasDeviceInAlarm => getListString(ZonesPanels, _data.ResetParam + 1),

                        CETriggerTypes.OtherEventTriggered or
                        CETriggerTypes.NetworkEventTriggered => getListString(Events, _data.ResetParam + 1),

                        CETriggerTypes.PanelInput => getListString(Inputs, _data.ResetParam + 1),

                        CETriggerTypes.TimerEventTn => getListString(Times, _data.ResetParam + 1),

                        CETriggerTypes.EventAnd => getListString(EventNumbers, _data.ResetParam + 1),

                        CETriggerTypes.ZoneAnd => getListString(ZoneNumbers, _data.ResetParam + 1),

                        _ => "",
                    };
                }
                catch { return ""; }
            }
            set
            {
                var prev = _data.ResetParam;
                _data.ResetParam = _data.ResetType switch
                {
                    CETriggerTypes.Loop1DevicePrealarm or
                    CETriggerTypes.Loop1DeviceTriggered => _data.ResetParam = getListIndex(Loop1Devices, value) - 1,

                    CETriggerTypes.Loop2DevicePrealarm or
                    CETriggerTypes.Loop2DeviceTriggered => _data.ResetParam = getListIndex(Loop2Devices, value) - 1,

                    CETriggerTypes.ZoneOrPanelInFire or
                    CETriggerTypes.ZoneHasDeviceInAlarm => _data.ResetParam = getListIndex(ZonesPanels, value) - 1,

                    CETriggerTypes.OtherEventTriggered or
                    CETriggerTypes.NetworkEventTriggered => _data.ResetParam = getListIndex(Events, value) - 1,

                    CETriggerTypes.PanelInput => _data.ResetParam = getListIndex(Inputs, value) - 1,

                    CETriggerTypes.TimerEventTn => _data.ResetParam = getListIndex(Times, value) - 1,

                    CETriggerTypes.EventAnd => _data.ResetParam = getListIndex(EventNumbers, value) - 1,

                    CETriggerTypes.ZoneAnd => _data.ResetParam = getListIndex(ZoneNumbers, value) - 1,

                    _ => -1,
                };

                if (_data.ResetParam != prev)
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
                    return _data.ResetType switch
                    {
                        CETriggerTypes.EventAnd => getListString(EventNumbers, _data.ResetParam2 + 1),
                        CETriggerTypes.ZoneAnd => getListString(ZoneNumbers, _data.ResetParam2 + 1),
                        _ => "",
                    };
                }
                catch { return ""; }
            }
            set
            {
                var prev = _data.ResetParam2;
                _data.ResetParam2 = _data.ResetType switch
                {
                    CETriggerTypes.EventAnd => _data.ResetParam2 = getListIndex(EventNumbers, value) - 1,
                    CETriggerTypes.ZoneAnd  => _data.ResetParam2  = getListIndex(ZoneNumbers, value) - 1,
                    _ => -1,
                };

                if (_data.ResetParam2 != prev)
                    clearDependants(ClearLevel.ResetParam);
                updateSelections();
            }
        }

        //public int SelectedResetConditionIndex
        //{
        //    //get => _data.ResetCondition is null ? -1 : _data.ResetCondition == true ? 0 : 1;
        //    get => _data.ResetCondition == true ? 0 : 1;
        //    set
        //    {
        //        //_data.ResetCondition = value switch { 0 => true, 1 => false, _ => null };
        //        _data.ResetCondition = value switch { 0 => true, _ => false };
        //        OnPropertyChanged();
        //        updateSelections();
        //    }
        //}
        
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
            //if (level <= ClearLevel.ActionType)       { ActionParam = -1; SelectedActionParamIndex = -1; SelectedActionParam = ""; }
            //if (level <= ClearLevel.ActionParam)      { TriggerType = (CETriggerTypes)(-1); SelectedTriggerType = ""; }
            //if (level <= ClearLevel.TriggerType)      { TriggerParam = TriggerParam2 = -1; SelectedTriggerParamIndex = SelectedTriggerParam2Index = -1; SelectedTriggerParam = SelectedTriggerParam2 = ""; }
            //if (level <= ClearLevel.TriggerParam)     { TriggerCondition = true; SelectedTriggerConditionIndex = 0; }
            //if (level <= ClearLevel.TriggerCondition) { ResetType = (CETriggerTypes)(-1); SelectedResetType = ""; }
            //if (level <= ClearLevel.ResetType)        { ResetParam = ResetParam2 = -1; SelectedResetParam = SelectedResetParam2 = ""; }
            //if (level <= ClearLevel.ResetParam)       { ResetCondition = false; SelectedResetConditionIndex = 1; }

            //if (level <= ClearLevel.ActionType)       { ActionParam = -1; SelectedActionParam = ""; }
            //if (level <= ClearLevel.ActionParam)      { TriggerType = (CETriggerTypes)(-1); SelectedTriggerType = ""; }
            //if (level <= ClearLevel.TriggerType)      { TriggerParam = TriggerParam2 = -1; SelectedTriggerParam = SelectedTriggerParam2 = ""; }
            //if (level <= ClearLevel.TriggerParam)     { TriggerCondition = true; SelectedTriggerCondition = TrueOrFalse[0]; }
            //if (level <= ClearLevel.TriggerCondition) { ResetType = (CETriggerTypes)(-1); SelectedResetType = ""; }
            //if (level <= ClearLevel.ResetType)        { ResetParam = ResetParam2 = -1; SelectedResetParam = SelectedResetParam2 = ""; }
            //if (level <= ClearLevel.ResetParam)       { ResetCondition = false; SelectedResetCondition = TrueOrFalse[1]; }

            if      (level <= ClearLevel.ActionType)       { SelectedActionParam = ""; }
            else if (level <= ClearLevel.ActionParam)      { SelectedActionParamIndex = -1; /*SelectedTriggerType = "";*/ }
            //else if (level <= ClearLevel.TriggerType)      { SelectedTriggerParam = SelectedTriggerParam2 = ""; }
            else if (level <= ClearLevel.TriggerType)      { SelectedTriggerParamIndex = -1; SelectedTriggerParam2 = ""; }
            else if (level <= ClearLevel.TriggerParam)     { SelectedTriggerCondition = TrueOrFalse[0]; }
            else if (level <= ClearLevel.TriggerCondition) { SelectedResetType = ""; }
            else if (level <= ClearLevel.ResetType)        { SelectedResetParam = SelectedResetParam2 = ""; }
            else if (level <= ClearLevel.ResetParam)       { _data.ResetCondition = false; SelectedResetCondition = TrueOrFalse[1]; }
        }


        #region option visibilities

        #region action
        public bool ShowActionParam => !_data.NoActionParam(_data.ActionType);
        public bool ShowActionLoop1Devices => _data.ActionType switch { CEActionTypes.TriggerLoop1Device or CEActionTypes.Loop1DeviceDisable => true, _ => false };
        public bool ShowActionLoop2Devices => _data.ActionType switch { CEActionTypes.TriggerLoop2Device or CEActionTypes.Loop2DeviceDisable => true, _ => false };
        public bool ShowActionRelays => _data.ActionType switch { CEActionTypes.PanelRelay => true, _ => false };
        public bool ShowActionGroups => _data.ActionType switch { CEActionTypes.SounderAlert or CEActionTypes.SounderEvac or CEActionTypes.GroupDisable or CEActionTypes.TriggerBeacons => true, _ => false };
        public bool ShowActionZones => _data.ActionType switch { CEActionTypes.ZoneDisable or CEActionTypes.PutZoneIntoAlarm => true, _ => false };
        public bool ShowActionSets => _data.ActionType switch { CEActionTypes.TriggerOutputSet => true, _ => false };
        public bool ShowActionSetsAndRelays => _data.ActionType switch { CEActionTypes.OutputDisable => true, _ => false };
        public bool ShowActionEvents => _data.ActionType switch { CEActionTypes.TriggerNetworkEvent => true, _ => false };
        #endregion

        #region trigger
        //public bool ShowTrigger => ActionType != CEActionTypes.None && (!ShowActionParam || SelectedActionParamIndex > -1);
        public bool ShowTrigger => _data.ActionType != CEActionTypes.None && (!ShowActionParam || SelectedActionParam != "");
        public bool ShowTriggerParam => ShowTrigger && !ShowTriggerParam2 && !_data.NoTriggerParam(_data.TriggerType);
        public bool ShowTriggerParam2 => ShowTrigger && (_data.TriggerType == CETriggerTypes.EventAnd || _data.TriggerType == CETriggerTypes.ZoneAnd);
        public bool ShowTriggerLoop1Devices => ShowTriggerParam && _data.TriggerType switch { CETriggerTypes.Loop1DeviceTriggered or CETriggerTypes.Loop1DevicePrealarm => true, _ => false };
        public bool ShowTriggerLoop2Devices => ShowTriggerParam && _data.TriggerType switch { CETriggerTypes.Loop2DeviceTriggered or CETriggerTypes.Loop2DevicePrealarm => true, _ => false };
        public bool ShowTriggerZones => ShowTriggerParam && _data.TriggerType switch { CETriggerTypes.ZoneAnd => true, _ => false };
        public bool ShowTriggerZones2 => ShowTriggerParam2 && _data.TriggerType switch { CETriggerTypes.ZoneAnd => true, _ => false };
        public bool ShowTriggerZonesAndPanels => ShowTriggerParam && _data.TriggerType switch { CETriggerTypes.ZoneOrPanelInFire or CETriggerTypes.ZoneHasDeviceInAlarm => true, _ => false };
        public bool ShowTriggerEvents => ShowTriggerParam && _data.TriggerType switch { CETriggerTypes.OtherEventTriggered or CETriggerTypes.EventAnd => true, _ => false };
        public bool ShowTriggerEvents2 => ShowTriggerParam2 && _data.TriggerType switch { CETriggerTypes.EventAnd => true, _ => false };
        public bool ShowTriggerTimes => ShowTriggerParam && _data.TriggerType switch { CETriggerTypes.TimerEventTn => true, _ => false };
        public bool ShowTriggerInputs => ShowTriggerParam && _data.TriggerType switch { CETriggerTypes.PanelInput => true, _ => false };
        public bool ShowTriggerCondition => (ShowTriggerParam && _data.TriggerParam >= 0) || (ShowTriggerParam2 && SelectedTriggerParam != "" && SelectedTriggerParam2 != "" || _data.NoTriggerParam(_data.TriggerType) && _data.TriggerType != CETriggerTypes.None);
        #endregion

        #region reset
        public bool ShowReset => ShowTriggerCondition /*&& _data.TriggerCondition is not null*/;
        public bool ShowResetParam => ShowReset && !ShowResetParam2 && !_data.NoTriggerParam(_data.ResetType);
        public bool ShowResetParam2 => ShowReset && (_data.ResetType == CETriggerTypes.EventAnd || _data.ResetType == CETriggerTypes.ZoneAnd);
        public bool ShowResetLoop1Devices => ShowResetParam && _data.ResetType switch { CETriggerTypes.Loop1DeviceTriggered or CETriggerTypes.Loop1DevicePrealarm => true, _ => false };
        public bool ShowResetLoop2Devices => ShowResetParam && _data.ResetType switch { CETriggerTypes.Loop2DeviceTriggered or CETriggerTypes.Loop2DevicePrealarm => true, _ => false };
        public bool ShowResetZones => ShowResetParam && _data.ResetType switch { CETriggerTypes.ZoneAnd => true, _ => false };
        public bool ShowResetZones2 => ShowResetParam2 && _data.ResetType switch { CETriggerTypes.ZoneAnd => true, _ => false };
        public bool ShowResetZonesAndPanels => ShowResetParam && _data.ResetType switch { CETriggerTypes.ZoneOrPanelInFire or CETriggerTypes.ZoneHasDeviceInAlarm => true, _ => false };
        public bool ShowResetEvents => ShowResetParam && _data.ResetType switch { CETriggerTypes.OtherEventTriggered or CETriggerTypes.EventAnd => true, _ => false };//ShowResetParam && ResetType switch { CETriggerTypes.OtherEventTriggered or CETriggerTypes.EventAnd or CETriggerTypes.NetworkEventTriggered => true, _ => false };
        public bool ShowResetEvents2 => ShowResetParam2 && _data.ResetType switch { CETriggerTypes.EventAnd => true, _ => false };
        public bool ShowResetTimes => ShowResetParam && _data.ResetType switch { CETriggerTypes.TimerEventTn => true, _ => false };
        public bool ShowResetInputs => ShowResetParam && _data.ResetType switch { CETriggerTypes.PanelInput => true, _ => false };
        public bool ShowResetCondition => ShowResetParam && _data.ResetParam >= 0 || ShowResetParam2 && _data.ResetParam >= 0 && _data.ResetParam2 >= 0 || _data.NoTriggerParam(_data.ResetType) && _data.ResetType != CETriggerTypes.None;
        #endregion

        #endregion

        #region option validation
        //public bool ActionParamIsValid => !ShowActionParam || SelectedActionParam != "";
        public bool ActionParamIsValid => !ShowActionParam || SelectedActionParamIndex > -1 || _data.ActionType == CEActionTypes.None;
        public bool ActionTriggerIsValid => !ShowTrigger || SelectedTriggerType != "";
        //public bool TriggerParam1IsValid => !ShowTriggerParam || SelectedTriggerParam != "" || TriggerType == CETriggerTypes.None;
        public bool TriggerParam1IsValid => !ShowTriggerParam || SelectedTriggerParamIndex > -1 || _data.TriggerType == CETriggerTypes.None;
        public bool IndicateInvalidTriggerParam2 => !(!ShowTriggerParam2 || !(!ShowTriggerParam2 || SelectedTriggerParam == "" && SelectedTriggerParam2 == ""));//!(!ShowTriggerParam2 || !(!ShowTriggerParam2 || SelectedTriggerParam != "" && SelectedTriggerParam2 != ""));
        public bool IndicateInvalidSelectedTriggerParam1 =>  !((!ShowTriggerEvents2 || SelectedTriggerParam != "" || SelectedTriggerParam2 == "") && (!ShowTriggerZones2 || SelectedTriggerParam != "" || SelectedTriggerParam2 == ""));
        public bool IndicateInvalidSelectedTriggerParam2 => !((!ShowTriggerEvents2 || SelectedTriggerParam2 != "" || SelectedTriggerParam == "") && (!ShowTriggerZones2 || SelectedTriggerParam2 != "" || SelectedTriggerParam == ""));
        public bool TriggerConditionIsValid => !ShowTriggerCondition || SelectedTriggerCondition != "";

        public bool ResetTriggerIsValid => !ShowReset || SelectedResetType != "";
        public bool ResetParam1IsValid => !ShowResetParam || SelectedResetParam != "" || _data.ResetType == CETriggerTypes.None;
        public bool IndicateInvalidResetParam2 => !(!ShowResetParam2 || !(!ShowResetParam2 || SelectedResetParam == "" && SelectedResetParam2 == ""));
        public bool IndicateInvalidSelectedResetParam1 => !((!ShowResetEvents2 || SelectedResetParam != "" || SelectedResetParam2 == "") && (!ShowResetZones2 || SelectedResetParam != "" || SelectedResetParam2 == ""));
        public bool IndicateInvalidSelectedResetParam2 => !((!ShowResetEvents2 || SelectedResetParam2 != "" || SelectedResetParam == "") && (!ShowResetZones2 || SelectedResetParam2 != "" || SelectedResetParam == ""));
        public bool ResetConditionIsValid => !ShowResetCondition || SelectedResetCondition != "";
        #endregion


        private void updateSelections()
        {   
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

            OnPropertyChanged(nameof(SelectedActionType));
            OnPropertyChanged(nameof(SelectedActionParam));
            OnPropertyChanged(nameof(SelectedActionParamIndex));
            OnPropertyChanged(nameof(SelectedTriggerType));
            OnPropertyChanged(nameof(SelectedTriggerParam));
            OnPropertyChanged(nameof(SelectedTriggerParam2));
            OnPropertyChanged(nameof(SelectedTriggerParamIndex));
            //OnPropertyChanged(nameof(SelectedTriggerParam2Index));
            OnPropertyChanged(nameof(SelectedResetType));
            OnPropertyChanged(nameof(SelectedResetParam));
            OnPropertyChanged(nameof(SelectedResetParam2));
            //OnPropertyChanged(nameof(SelectedTriggerConditionIndex));
            OnPropertyChanged(nameof(SelectedTriggerCondition));
            //OnPropertyChanged(nameof(SelectedResetConditionIndex));
            OnPropertyChanged(nameof(SelectedResetCondition));
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
        //public MenuList ZoneNumbersMenu;
        public MenuList ZonesPanelsMenu;
        public MenuList GroupsMenu;
        public MenuList SetsMenu;
        public MenuList EventsMenu;
        //public MenuList EventNumbersMenu;
        public MenuList RelaysMenu;
        public MenuList SetsRelaysMenu;
        public MenuList TimesMenu;
        public MenuList TrueOrFalseMenu;

        private List<string> _actions;
        private List<string> _triggers;
        private List<string> _inputs;
        private List<string> _loop1Devices;
        private List<string> _loop2Devices;
        private List<string> _zones;
        private List<string> _zoneNumbers;
        private List<string> _zonesPanels;
        private List<string> _groups;
        private List<string> _sets;
        private List<string> _events;
        private List<string> _eventNumbers;
        private List<string> _relays;
        private List<string> _setsRelays;
        private List<string> _times;
        private List<string> _trueOrFalse;

        public List<string> Actions { get => ActionsMenu?.Invoke(); }
        public List<string> Triggers { get => TriggersMenu?.Invoke(); }
        public List<string> Inputs { get => InputsMenu?.Invoke(); }
        public List<string> Loop1Devices { get => Loop1DevicesMenu?.Invoke(); }
        public List<string> Loop2Devices { get => Loop2DevicesMenu?.Invoke(); }
        public List<string> Zones { get => ZonesMenu?.Invoke(); }
        //public List<string> ZoneNumbers { get => ZoneNumbersMenu?.Invoke(); }
        public List<string> ZoneNumbers { get => _zoneNumbers; }
        public List<string> ZonesPanels { get => ZonesPanelsMenu?.Invoke(); }
        public List<string> Groups { get => GroupsMenu?.Invoke(); }
        public List<string> Sets { get => SetsMenu?.Invoke(); }
        public List<string> Events { get => EventsMenu?.Invoke(); }
        //public List<string> EventNumbers { get => EventNumbersMenu?.Invoke(); }
        public List<string> EventNumbers { get => _eventNumbers; }
        public List<string> Relays { get => RelaysMenu?.Invoke(); }
        public List<string> SetsRelays { get => SetsRelaysMenu?.Invoke(); }
        public List<string> Times { get => TimesMenu?.Invoke(); }
        public List<string> TrueOrFalse { get => TrueOrFalseMenu?.Invoke(); }
        //public List<string> Actions      { get => _actions;      set { _actions = value; OnPropertyChanged(); } }
        //public List<string> Triggers     { get => _triggers;     set { _triggers = value; OnPropertyChanged(); } }
        //public List<string> Inputs       { get => _inputs;       set { _inputs = value; OnPropertyChanged(); } }
        //public List<string> Loop1Devices { get => _loop1Devices; set { _loop1Devices = value; OnPropertyChanged(); } }
        //public List<string> Loop2Devices { get => _loop2Devices; set { _loop2Devices = value; OnPropertyChanged(); } }
        //public List<string> Zones        { get => _zones;        set { _zones = value; OnPropertyChanged(); } }
        //public List<string> ZoneNumbers  { get => _zoneNumbers;  set { _zoneNumbers = value; OnPropertyChanged(); } }
        //public List<string> ZonesPanels  { get => _zonesPanels;  set { _zonesPanels = value; OnPropertyChanged(); } }
        //public List<string> Groups       { get => _groups;       set { _groups = value; OnPropertyChanged(); } }
        //public List<string> Sets         { get => _sets;         set { _sets = value; OnPropertyChanged(); } }
        //public List<string> Events       { get => _events;       set { _events = value; OnPropertyChanged(); } }
        //public List<string> EventNumbers { get => _eventNumbers; set { _eventNumbers = value; OnPropertyChanged(); } }
        //public List<string> Relays       { get => _relays;       set { _relays = value; OnPropertyChanged(); } }
        //public List<string> SetsRelays   { get => _setsRelays;   set { _setsRelays = value; OnPropertyChanged(); } }
        //public List<string> Times        { get => _times;        set { _times = value; OnPropertyChanged(); } }
        //public List<string> TrueOrFalse  { get => _trueOrFalse;  set { _trueOrFalse = value; OnPropertyChanged(); } }


        private int getListIndex(List<string> list, string item)
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i] == item)
                    return i;
            return -1;
        }

        private string getListString(List<string> list, int? index)
        {
            if (list is not null && index >= 0 && index < list.Count)
                return list[(int)index];
            return "";
        }
        private int getListIndex(List<string> list, int? index)
        {
            if (index is not null && index >= 0 && index < list.Count)
                return (int)index;
            return -1;
        }
        

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
        public void SetCulture(CultureInfo culture)
        {
            RefreshView();
        }
        #endregion


        #region IPanelToolsViewModel implementation
        public void PopulateView(XfpData data) { }

        //public void RefreshView(int errorCode = 0)
        public void RefreshView()
        {
            //InitFixedComboLists();

            var ap = SelectedActionParamIndex;
            var tp = SelectedTriggerParamIndex;

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

            OnPropertyChanged(nameof(SelectedActionType));
            OnPropertyChanged(nameof(SelectedActionParam));
            OnPropertyChanged(nameof(SelectedActionParamIndex));
            OnPropertyChanged(nameof(SelectedTriggerType));
            OnPropertyChanged(nameof(SelectedTriggerParam));
            OnPropertyChanged(nameof(SelectedTriggerParam2));
            OnPropertyChanged(nameof(SelectedTriggerParamIndex));
            //OnPropertyChanged(nameof(SelectedTriggerParam2Index));
            //OnPropertyChanged(nameof(SelectedTriggerConditionIndex));
            OnPropertyChanged(nameof(SelectedTriggerCondition));
            OnPropertyChanged(nameof(SelectedResetType));
            OnPropertyChanged(nameof(SelectedResetParam));
            OnPropertyChanged(nameof(SelectedResetParam2));
            //OnPropertyChanged(nameof(SelectedResetConditionIndex));
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

            SelectedTriggerParam = getTriggerParamString(tp);
            SelectedActionParam  = getActionParamString(ap);

            updateSelections();
        }
        #endregion
    }
}
