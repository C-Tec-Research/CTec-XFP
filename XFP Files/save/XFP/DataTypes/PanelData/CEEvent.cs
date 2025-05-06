﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Activation;
using Xfp.DataTypes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Xfp.DataTypes.PanelData
{
    public class CEEvent : ConfigData
    {
        public CEEvent()
        {
            ActionType = CEActionTypes.None;
            ActionParam = -1;
            TriggerType = CETriggerTypes.None;
            TriggerParam = -1;
            TriggerParam2 = -1;
            TriggerCondition = true;
            ResetType = CETriggerTypes.None;
            ResetParam = -1;
            ResetParam2 = -1;
            ResetCondition = false;
        }

        public CEEvent(CEEvent original)
        {
            Index = original.Index;
            ActionType = original.ActionType;
            ActionParam = original.ActionParam;
            TriggerType = original.TriggerType;
            TriggerParam = original.TriggerParam;
            TriggerParam2 = original.TriggerParam2;
            TriggerCondition = original.TriggerCondition;
            ResetType = original.ResetType;
            ResetParam = original.ResetParam;
            ResetParam2 = original.ResetParam2;
            ResetCondition = original.ResetCondition;
        }

        public int Index { get; set; }
        [JsonIgnore] public int Number => Index + 1;
        public CEActionTypes ActionType { get; set; }
        public int ActionParam { get; set; }
        public CETriggerTypes TriggerType { get; set; }
        public int TriggerParam { get; set; }
        public int TriggerParam2 { get; set; }
        public bool TriggerCondition { get; set; }
        public CETriggerTypes ResetType { get; set; }
        public int ResetParam { get; set; }
        public int ResetParam2 { get; set; }
        public bool ResetCondition { get; set; }


        /// <summary>
        /// Returns an initialised GroupData object.
        /// </summary>
        internal static new CEEvent InitialisedNew()
        {
            return new();
        }


        public override bool Equals(ConfigData otherData)
        {
            return otherData is CEEvent od
                && Index == od.Index
                && ActionType == od.ActionType
                && ActionParam == od.ActionParam
                && TriggerType == od.TriggerType
                && TriggerParam == od.TriggerParam
                && TriggerParam2 == od.TriggerParam2
                && TriggerCondition == od.TriggerCondition
                && ResetType == od.ResetType
                && ResetParam == od.ResetParam
                && ResetParam2 == od.ResetParam2
                && ResetCondition == od.ResetCondition;
        }


        internal bool NoActionParam(CEActionTypes actionType)
            => ActionType switch
            {
                CEActionTypes.None            or CEActionTypes.SilencePanel        or CEActionTypes.ResetPanel or 
                CEActionTypes.SetToOccupied   or CEActionTypes.AbstractEvent       or CEActionTypes.MutePanel or 
                CEActionTypes.SetToUnoccupied or CEActionTypes.OutputDelaysDisable or CEActionTypes.EndPhasedEvacuation or 
                CEActionTypes.EndZoneDelays
                  => true, 
                _ => false
            };

        internal bool NoTriggerParam(CETriggerTypes? triggerType) 
            => triggerType switch
            {
                CETriggerTypes.AnyDeviceInAlarm      or CETriggerTypes.AnyDisablement   or CETriggerTypes.AnyDwellingAndCommunal or 
                CETriggerTypes.AnyFault              or CETriggerTypes.AnyPrealarm      or CETriggerTypes.AnyRemotePanelInFire or 
                CETriggerTypes.AnyZoneInFire         or CETriggerTypes.MoreThanOneAlarm or CETriggerTypes.MoreThanOneZoneInAlarm or 
                CETriggerTypes.NetworkEventTriggered or CETriggerTypes.None             or
                CETriggerTypes.PanelSilenced         or CETriggerTypes.PanelOccupied    or CETriggerTypes.PanelReset or
                CETriggerTypes.PanelUnoccupied  
                  => true, 
                _ => false
            };


        public override bool Validate()
        {
            _errorItems = new(Index, string.Format(Cultures.Resources.Event_x, Number));
            if (ActionType != CEActionTypes.None)
            {
                bool continueChecking = true;

                if (!NoActionParam(ActionType))
                {
                    ValidationCodes error = ValidationCodes.Ok;
                    switch (ActionType)
                    {
                        case CEActionTypes.TriggerLoop1Device:
                        case CEActionTypes.Loop1DeviceDisable: if (!isValidDevice(ActionParam))       error = ValidationCodes.CEActionLoop1DeviceNotSet; break;
                        case CEActionTypes.TriggerLoop2Device:
                        case CEActionTypes.Loop2DeviceDisable: if (!isValidDevice(ActionParam))       error = ValidationCodes.CEActionLoop2DeviceNotSet; break;
                        case CEActionTypes.PanelRelay:         if (!isValidRelay(ActionParam))        error = ValidationCodes.CEActionPanelRelayNotSet; break;
                        case CEActionTypes.SounderAlert:       
                        case CEActionTypes.SounderEvac:        if (!isValidGroup(ActionParam))        error = ValidationCodes.CEActionSounderGroupNotSet; break;
                        case CEActionTypes.ZoneDisable:  
                        case CEActionTypes.PutZoneIntoAlarm:   if (!isValidZone(ActionParam))         error = ValidationCodes.CEActionZoneNotSet; break;
                        case CEActionTypes.OutputDisable:      
                        case CEActionTypes.TriggerOutputSet:   if (!isValidSet(ActionParam))          error = ValidationCodes.CEActionOutputSetNotSet; break;
                        case CEActionTypes.GroupDisable:       if (!isValidGroup(ActionParam))        error = ValidationCodes.CEActionGroupNotSet; break;
                        case CEActionTypes.TriggerNetworkEvent:if (!isValidEvent(ActionParam)) error = ValidationCodes.CEActionNetworkEventNotSet; break;
                        case CEActionTypes.TriggerBeacons:     if (!isValidGroup(ActionParam))        error = ValidationCodes.CEActionBeaconGroupNotSet; break;
                    }

                    if (error != ValidationCodes.Ok)
                    {
                        _errorItems.ValidationCodes.Add(error);
                        continueChecking = false;
                    }
                }

                if (continueChecking)
                {
                    ValidationCodes error = ValidationCodes.Ok;
                    ValidationCodes error2 = ValidationCodes.Ok;
                    if (TriggerType == CETriggerTypes.None || (int)TriggerType < -1)
                    {
                        error = ValidationCodes.CETriggerTypeNotSet;
                    }
                    else if (!NoTriggerParam(TriggerType))
                    {
                        switch (TriggerType)
                        {
                            case CETriggerTypes.Loop1DeviceTriggered:
                            case CETriggerTypes.Loop1DevicePrealarm:    if (!isValidDevice(TriggerParam))       error = ValidationCodes.CETriggerLoop1DeviceNotSet; break;
                            case CETriggerTypes.Loop2DeviceTriggered:
                            case CETriggerTypes.Loop2DevicePrealarm:    if (!isValidDevice(TriggerParam))       error = ValidationCodes.CETriggerLoop2DeviceNotSet; break;
                            case CETriggerTypes.ZoneOrPanelInFire:
                            case CETriggerTypes.ZoneHasDeviceInAlarm:   if (!isValidZoneOrPanel(TriggerParam))  error = ValidationCodes.CETriggerZoneOrPanelNotSet; break;
                            case CETriggerTypes.PanelInput:             if (!isValidInput(TriggerParam))        error = ValidationCodes.CETriggerPanelInputNotSet; break;
                            case CETriggerTypes.OtherEventTriggered:    if (!isValidEvent(TriggerParam))        error = ValidationCodes.CETriggerEventNotSet; break;;
                            case CETriggerTypes.TimerEventTn:           if (!isValidTimer(TriggerParam))        error = ValidationCodes.CETriggerTimerNotSet; break;
                            case CETriggerTypes.NetworkEventTriggered:  if (!isValidEvent(TriggerParam))        error = ValidationCodes.CETriggerNetworkEventNotSet; break;
                            
                            case CETriggerTypes.EventAnd:
                            case CETriggerTypes.ZoneAnd:                if (!isValidZone(TriggerParam2))        error  = ValidationCodes.CEResetZone2NotSet;
                                                                        if (!isValidZone(TriggerParam))         error2 = ValidationCodes.CEResetZoneNotSet;
                                                                        break;
                        }
                    }

                    switch (TriggerType)
                    {
                        case CETriggerTypes.EventAnd: if (!isValidEvent(TriggerParam2)) error2 = ValidationCodes.CETriggerEvent2NotSet; break;
                        case CETriggerTypes.ZoneAnd:  if (!isValidZone(TriggerParam2))  error2 = ValidationCodes.CETriggerZone2NotSet; break;
                    }

                    if (error != ValidationCodes.Ok)
                    {
                        _errorItems.ValidationCodes.Add(error);
                        continueChecking = false;
                    }

                    if (error2 != ValidationCodes.Ok)
                    {
                        _errorItems.ValidationCodes.Add(error2);
                        continueChecking = false;
                    }
                }


                if (continueChecking)
                {
                    ValidationCodes error = ValidationCodes.Ok;
                    ValidationCodes error2 = ValidationCodes.Ok;
                    if (ResetType == CETriggerTypes.None)
                    {
                        error = ValidationCodes.CEResetTypeNotSet;
                    }
                    else if (!NoTriggerParam(ResetType))
                    {
                        switch (ResetType)
                        {
                            case CETriggerTypes.Loop1DeviceTriggered:
                            case CETriggerTypes.Loop1DevicePrealarm:    if (!isValidDevice(ResetParam))       error = ValidationCodes.CEResetLoop1DeviceNotSet; break;
                            case CETriggerTypes.Loop2DeviceTriggered:
                            case CETriggerTypes.Loop2DevicePrealarm:    if (!isValidDevice(ResetParam))       error = ValidationCodes.CEResetLoop2DeviceNotSet; break;
                            case CETriggerTypes.ZoneOrPanelInFire:
                            case CETriggerTypes.ZoneHasDeviceInAlarm:   if (!isValidZoneOrPanel(ResetParam))  error = ValidationCodes.CEResetZoneOrPanelNotSet; break;
                            case CETriggerTypes.PanelInput:             if (!isValidInput(ResetParam))        error = ValidationCodes.CEResetPanelInputNotSet; break;
                            case CETriggerTypes.OtherEventTriggered:    if (!isValidEvent(ResetParam))        error = ValidationCodes.CEResetEventNotSet; break;;
                            case CETriggerTypes.TimerEventTn:           if (!isValidTimer(ResetParam))        error = ValidationCodes.CEResetTimerNotSet; break;
                            case CETriggerTypes.NetworkEventTriggered:  if (!isValidEvent(ResetParam))        error = ValidationCodes.CEResetNetworkEventNotSet; break;
                            
                            case CETriggerTypes.EventAnd:
                            case CETriggerTypes.ZoneAnd:                if (!isValidZone(ResetParam2))        error  = ValidationCodes.CEResetZone2NotSet;
                                                                        if (!isValidZone(ResetParam))         error2 = ValidationCodes.CEResetZoneNotSet;
                                                                        break;
                        }
                    }

                    if (error2 != ValidationCodes.Ok)
                    {
                        _errorItems.ValidationCodes.Add(error2);
                        continueChecking = false;
                    }

                    if (error != ValidationCodes.Ok)
                    {
                        _errorItems.ValidationCodes.Add(error);
                        continueChecking = false;
                    }
                }

            }
            return _errorItems.ValidationCodes.Count == 0;
        }

        private bool isValidDevice(int device)     => device >= 0 && device < DeviceConfigData.NumDevices;
        private bool isValidGroup(int group)       => group >= 0 && group < GroupConfigData.NumSounderGroups;
        private bool isValidInput(int input)       => input >= 0 && input < XfpPanelData.NumPanelInputs;
        private bool isValidEvent(int evnt)        => evnt >= 0 && evnt < CEConfigData.NumEvents;
        private bool isValidPanel(int panel)       => panel >= 0 && ActionParam < NetworkConfigData.NumPanelSettings;
        private bool isValidRelay(int relay)       => relay >= 0 && relay < XfpPanelData.NumRelays;
        private bool isValidSet(int set)           => set >= 0 && set < SetConfigData.NumOutputSetTriggers;
        private bool isValidTimer(int timer)       => timer >= 0 && timer < CEConfigData. NumEvents;
        private bool isValidZone(int zone)         => zone >= 0 && zone < ZoneConfigData.NumZones;
        private bool isValidZoneOrPanel(int zopa)  => zopa >= 0 && zopa < ZoneConfigData.NumZones + ZonePanelConfigData.NumZonePanels;


        public byte[] ToByteArray()
        {
            var result = new byte[1];                
            return result;
        }
    }
}
