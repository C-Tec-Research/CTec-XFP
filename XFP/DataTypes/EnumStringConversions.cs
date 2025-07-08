using CTecDevices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.DataFormats;

namespace Xfp.DataTypes
{
    public class Enums
    {
        public static string EnumToString(object value)
        {
            if (value is IOTypes it) return IOTypesToString(it);
            if (value is AlarmTypes at) return AlarmTypesToString(at);
            if (value is SetTriggerTypes st) return SetTriggerypesToString(st);
            if (value is CEActionTypes cat) return CEActionTypesToString(cat);
            if (value is CETriggerTypes ctt) return CETriggerTypesToString(ctt);
            if (value is ZoneDependencyOptions zd) return ZoneDependencyOptionToString(zd);
            if (value is ValidationCodes vc) return ValidationCodesToString(vc);
            return "";
        }

        public static string IOTypesToString(IOTypes ioType)
            => ioType switch
            {
                IOTypes.Input   => Cultures.Resources.Input,
                IOTypes.Output  => Cultures.Resources.Output,
                _               => Cultures.Resources.Not_Used,
            };
        public static IOTypes StringToIOTypes(string ioType)
        {
            if (ioType == Cultures.Resources.Input)  return IOTypes.Input;
            if (ioType == Cultures.Resources.Output) return IOTypes.Output;
            return IOTypes.NotUsed;
        }

        public static string AlarmTypesToString(AlarmTypes alarmType)
            => alarmType switch
            {
                AlarmTypes.Alert    => Cultures.Resources.AlarmType_Alert,
                AlarmTypes.Evacuate => Cultures.Resources.AlarmType_Evacuate,
                _                   => Cultures.Resources.AlarmType_Off,
            };

        public static AlarmTypes StringToAlarmTypes(string alarmType)
        {
            if (alarmType == Cultures.Resources.AlarmType_Alert)    return AlarmTypes.Alert;
            if (alarmType == Cultures.Resources.AlarmType_Evacuate) return AlarmTypes.Evacuate;
            else return AlarmTypes.Off;
        }

        public static string SetTriggerypesToString(SetTriggerTypes setting)
            => setting switch
            {
                SetTriggerTypes.Pulsed     => Cultures.Resources.TriggerType_Pulsed,
                SetTriggerTypes.Continuous => Cultures.Resources.TriggerType_Continuous,
                SetTriggerTypes.Delayed    => Cultures.Resources.TriggerType_Delayed,
                _                          => Cultures.Resources.TriggerType_Not_Triggered,
            };

        public static SetTriggerTypes StringToSetTriggerTypes(string alarmType)
        {
            if (alarmType == Cultures.Resources.TriggerType_Pulsed)     return SetTriggerTypes.Pulsed;
            if (alarmType == Cultures.Resources.TriggerType_Continuous) return SetTriggerTypes.Continuous;
            if (alarmType == Cultures.Resources.TriggerType_Delayed)    return SetTriggerTypes.Delayed;
            else return SetTriggerTypes.NotTriggered;
        }


        public static string CEActionTypesToString(CEActionTypes actionType)
            => actionType switch
            {
                CEActionTypes.None                => "",//Cultures.Resources.Action_No_Events,
                CEActionTypes.TriggerLoop1Device  => string.Format(Cultures.Resources.Action_Trigger_Loop_x_Device, 1),
                CEActionTypes.TriggerLoop2Device  => string.Format(Cultures.Resources.Action_Trigger_Loop_x_Device, 2),
                CEActionTypes.PanelRelay          => Cultures.Resources.Action_Panel_Relay,
                CEActionTypes.SounderAlert        => Cultures.Resources.Action_Sounder_Alert,
                CEActionTypes.SounderEvac         => Cultures.Resources.Action_Sounder_Evac,
                CEActionTypes.SilencePanel        => Cultures.Resources.Action_Silence_Panel,
                CEActionTypes.ResetPanel          => Cultures.Resources.Action_Reset_Panel,
                CEActionTypes.ZoneDisable         => Cultures.Resources.Action_Zone_Disable,
                CEActionTypes.OutputDisable       => Cultures.Resources.Action_Output_Disable,
                CEActionTypes.TriggerOutputSet    => Cultures.Resources.Action_Trigger_Output_Set,
                CEActionTypes.SetToOccupied       => Cultures.Resources.Action_Set_To_Occupied,
                CEActionTypes.AbstractEvent       => Cultures.Resources.Action_Abstract_Event,
                CEActionTypes.MutePanel           => Cultures.Resources.Action_Mute_The_Panel,
                CEActionTypes.SetToUnoccupied     => Cultures.Resources.Action_Set_To_Unoccupied,
                CEActionTypes.OutputDelaysDisable => Cultures.Resources.Action_Output_Delays_Disable,
                CEActionTypes.GroupDisable        => Cultures.Resources.Action_Group_Disable,
                CEActionTypes.Loop1DeviceDisable  => string.Format(Cultures.Resources.Action_Loop_x_Device_Disabled, 1),
                CEActionTypes.Loop2DeviceDisable  => string.Format(Cultures.Resources.Action_Loop_x_Device_Disabled, 2),
                CEActionTypes.EndPhasedEvacuation => Cultures.Resources.Action_End_Phased_Evac,
                CEActionTypes.EndZoneDelays       => Cultures.Resources.Action_End_Zone_Delays,
                CEActionTypes.PutZoneIntoAlarm    => Cultures.Resources.Action_Put_Zone_Into_Alarm,
                CEActionTypes.TriggerNetworkEvent => Cultures.Resources.Action_Trigger_Network_Event,
                CEActionTypes.TriggerBeacons      => Cultures.Resources.Action_Trigger_Beacons,
                _ => ""
            };

        public static CEActionTypes StringToCEActionTypes(string actionType)
        {
            if (actionType == string.Format(Cultures.Resources.Action_Trigger_Loop_x_Device, 1)) return CEActionTypes.TriggerLoop1Device;
            if (actionType == string.Format(Cultures.Resources.Action_Trigger_Loop_x_Device, 2)) return CEActionTypes.TriggerLoop2Device;
            if (actionType == Cultures.Resources.Action_Panel_Relay)           return CEActionTypes.PanelRelay;
            if (actionType == Cultures.Resources.Action_Sounder_Alert)         return CEActionTypes.SounderAlert;
            if (actionType == Cultures.Resources.Action_Sounder_Evac)          return CEActionTypes.SounderEvac;
            if (actionType == Cultures.Resources.Action_Silence_Panel)         return CEActionTypes.SilencePanel;
            if (actionType == Cultures.Resources.Action_Reset_Panel)           return CEActionTypes.ResetPanel;
            if (actionType == Cultures.Resources.Action_Zone_Disable)          return CEActionTypes.ZoneDisable;
            if (actionType == Cultures.Resources.Action_Output_Disable)        return CEActionTypes.OutputDisable;
            if (actionType == Cultures.Resources.Action_Trigger_Output_Set)    return CEActionTypes.TriggerOutputSet;
            if (actionType == Cultures.Resources.Action_Set_To_Occupied)       return CEActionTypes.SetToOccupied;
            if (actionType == Cultures.Resources.Action_Abstract_Event)        return CEActionTypes.AbstractEvent;
            if (actionType == Cultures.Resources.Action_Mute_The_Panel)        return CEActionTypes.MutePanel;
            if (actionType == Cultures.Resources.Action_Set_To_Occupied)       return CEActionTypes.SetToUnoccupied;
            if (actionType == Cultures.Resources.Action_Output_Delays_Disable) return CEActionTypes.OutputDelaysDisable;
            if (actionType == Cultures.Resources.Action_Group_Disable)         return CEActionTypes.GroupDisable;
            if (actionType == string.Format(Cultures.Resources.Action_Loop_x_Device_Disabled, 1)) return CEActionTypes.Loop1DeviceDisable;
            if (actionType == string.Format(Cultures.Resources.Action_Loop_x_Device_Disabled, 2)) return CEActionTypes.Loop2DeviceDisable;
            if (actionType == Cultures.Resources.Action_End_Phased_Evac)       return CEActionTypes.EndPhasedEvacuation;
            if (actionType == Cultures.Resources.Action_End_Zone_Delays)       return CEActionTypes.EndZoneDelays;
            if (actionType == Cultures.Resources.Action_Put_Zone_Into_Alarm)   return CEActionTypes.PutZoneIntoAlarm;
            if (actionType == Cultures.Resources.Action_Trigger_Network_Event) return CEActionTypes.TriggerNetworkEvent;
            if (actionType == Cultures.Resources.Action_Trigger_Beacons)       return CEActionTypes.TriggerBeacons;
            else return CEActionTypes.None;
        }


        public static string CETriggerTypesToString(CETriggerTypes triggerType)
            => triggerType switch
            {
                CETriggerTypes.None                     => "",
                CETriggerTypes.Loop1DeviceTriggered     => string.Format(Cultures.Resources.Trigger_Loop_x_Device_Triggered, 1),
                CETriggerTypes.Loop2DeviceTriggered     => string.Format(Cultures.Resources.Trigger_Loop_x_Device_Triggered, 2),
                CETriggerTypes.ZoneOrPanelInFire        => Cultures.Resources.Trigger_Zone_Or_Panel_In_Fire,
                CETriggerTypes.AnyZoneInFire            => Cultures.Resources.Trigger_Any_Zone_In_Fire,
                CETriggerTypes.PanelSilenced            => Cultures.Resources.Trigger_Panel_Silenced,
                CETriggerTypes.PanelReset               => Cultures.Resources.Trigger_Panel_Reset,
                CETriggerTypes.AnyPrealarm              => Cultures.Resources.Trigger_Any_Pre_Alarm,
                CETriggerTypes.Loop1DevicePrealarm      => string.Format(Cultures.Resources.Trigger_Loop_x_Device_Pre_Alarm, 1),
                CETriggerTypes.Loop2DevicePrealarm      => string.Format(Cultures.Resources.Trigger_Loop_x_Device_Pre_Alarm, 2),
                CETriggerTypes.AnyFault                 => Cultures.Resources.Trigger_Any_Fault,
                CETriggerTypes.PanelInput               => Cultures.Resources.Trigger_Panel_Input,
                CETriggerTypes.OtherEventTriggered      => Cultures.Resources.Trigger_Other_Event_Triggered,
                CETriggerTypes.EventAnd                 => Cultures.Resources.Trigger_Event_AND,
                CETriggerTypes.PanelOccupied            => Cultures.Resources.Trigger_Panel_Occupied,
                CETriggerTypes.PanelUnoccupied          => Cultures.Resources.Trigger_Panel_Unoccupied,
                CETriggerTypes.TimerEventTn             => Cultures.Resources.Trigger_Timer_Event,
                CETriggerTypes.ZoneHasDeviceInAlarm     => Cultures.Resources.Trigger_Zone_Has_Device_In_Alarm,
                CETriggerTypes.AnyDeviceInAlarm         => Cultures.Resources.Trigger_Any_Device_In_Alarm,
                CETriggerTypes.AnyRemotePanelInFire     => Cultures.Resources.Trigger_Any_Remote_Panel_In_Fire,
                CETriggerTypes.AnyDisablement           => Cultures.Resources.Trigger_Any_Disablement,
                CETriggerTypes.MoreThanOneAlarm         => Cultures.Resources.Trigger_More_Than_One_Alarm,
                CETriggerTypes.MoreThanOneZoneInAlarm   => Cultures.Resources.Trigger_More_Than_One_Zone_In_Alarm,
                CETriggerTypes.NetworkEventTriggered    => Cultures.Resources.Trigger_Network_Event_Triggered,
                CETriggerTypes.AnyDwellingAndCommunal   => Cultures.Resources.Trigger_Any_Dwelling_And_Communal,
                CETriggerTypes.ZoneAnd                  => Cultures.Resources.Trigger_Zone_AND,
                _ => ""
            };

        public static CETriggerTypes StringToCETriggerTypes(string triggerType)
        {
                if (triggerType == string.Format(Cultures.Resources.Trigger_Loop_x_Device_Triggered, 1)) return CETriggerTypes.Loop1DeviceTriggered;
                if (triggerType == string.Format(Cultures.Resources.Trigger_Loop_x_Device_Triggered, 2)) return CETriggerTypes.Loop2DeviceTriggered;
                if (triggerType == Cultures.Resources.Trigger_Zone_Or_Panel_In_Fire)       return CETriggerTypes.ZoneOrPanelInFire;
                if (triggerType == Cultures.Resources.Trigger_Any_Zone_In_Fire)            return CETriggerTypes.AnyZoneInFire;
                if (triggerType == Cultures.Resources.Trigger_Panel_Silenced)              return CETriggerTypes.PanelSilenced;
                if (triggerType == Cultures.Resources.Trigger_Panel_Reset)                 return CETriggerTypes.PanelReset;
                if (triggerType == Cultures.Resources.Trigger_Any_Pre_Alarm)               return CETriggerTypes.AnyPrealarm;
                if (triggerType == string.Format(Cultures.Resources.Trigger_Loop_x_Device_Pre_Alarm, 1)) return CETriggerTypes.Loop1DevicePrealarm;
                if (triggerType == string.Format(Cultures.Resources.Trigger_Loop_x_Device_Pre_Alarm, 2)) return CETriggerTypes.Loop2DevicePrealarm;
                if (triggerType == Cultures.Resources.Trigger_Any_Fault)                   return CETriggerTypes.AnyFault;
                if (triggerType == Cultures.Resources.Trigger_Panel_Input)                 return CETriggerTypes.PanelInput;
                if (triggerType == Cultures.Resources.Trigger_Other_Event_Triggered)       return CETriggerTypes.OtherEventTriggered;
                if (triggerType == Cultures.Resources.Trigger_Event_AND)                   return CETriggerTypes.EventAnd;
                if (triggerType == Cultures.Resources.Trigger_Panel_Occupied)              return CETriggerTypes.PanelOccupied;
                if (triggerType == Cultures.Resources.Trigger_Panel_Unoccupied)            return CETriggerTypes.PanelUnoccupied;
                if (triggerType == Cultures.Resources.Trigger_Timer_Event)                 return CETriggerTypes.TimerEventTn;
                if (triggerType == Cultures.Resources.Trigger_Zone_Has_Device_In_Alarm)    return CETriggerTypes.ZoneHasDeviceInAlarm;
                if (triggerType == Cultures.Resources.Trigger_Any_Device_In_Alarm)         return CETriggerTypes.AnyDeviceInAlarm;
                if (triggerType == Cultures.Resources.Trigger_Any_Remote_Panel_In_Fire)    return CETriggerTypes.AnyRemotePanelInFire;
                if (triggerType == Cultures.Resources.Trigger_Any_Disablement)             return CETriggerTypes.AnyDisablement;
                if (triggerType == Cultures.Resources.Trigger_More_Than_One_Alarm)         return CETriggerTypes.MoreThanOneAlarm;
                if (triggerType == Cultures.Resources.Trigger_More_Than_One_Zone_In_Alarm) return CETriggerTypes.MoreThanOneZoneInAlarm;
                if (triggerType == Cultures.Resources.Trigger_Network_Event_Triggered)     return CETriggerTypes.NetworkEventTriggered;
                if (triggerType == Cultures.Resources.Trigger_Any_Dwelling_And_Communal)   return CETriggerTypes.AnyDwellingAndCommunal;
                if (triggerType == Cultures.Resources.Trigger_Zone_AND)                    return CETriggerTypes.ZoneAnd;
                else return CETriggerTypes.None;
        }

        public static string ZoneDependencyOptionToString(ZoneDependencyOptions option)
            => option switch
            {
                ZoneDependencyOptions.A             => Cultures.Resources.Zone_Dependency_A,
                ZoneDependencyOptions.B             => Cultures.Resources.Zone_Dependency_B,
                ZoneDependencyOptions.C             => Cultures.Resources.Zone_Dependency_C,
                ZoneDependencyOptions.Normal        => Cultures.Resources.Zone_Dependency_Normal,
                ZoneDependencyOptions.Investigation => Cultures.Resources.Zone_Dependency_Investigation,
                ZoneDependencyOptions.Dwelling      => Cultures.Resources.Zone_Dependency_Dwelling,
                _ => Cultures.Resources.Not_Set,
            };

        public static ZoneDependencyOptions StringToZoneDependencyOption(string option)
        {
            if (option == Cultures.Resources.Zone_Dependency_A)             return ZoneDependencyOptions.A;
            if (option == Cultures.Resources.Zone_Dependency_B)             return ZoneDependencyOptions.B;
            if (option == Cultures.Resources.Zone_Dependency_C)             return ZoneDependencyOptions.C;
            if (option == Cultures.Resources.Zone_Dependency_Normal)        return ZoneDependencyOptions.Normal;
            if (option == Cultures.Resources.Zone_Dependency_Investigation) return ZoneDependencyOptions.Investigation;
            if (option == Cultures.Resources.Zone_Dependency_Dwelling)      return ZoneDependencyOptions.Dwelling;
            return ZoneDependencyOptions.NotSet;
        }

        public static string ValidationCodesToString(ValidationCodes code)
            => code switch
            {
                //devices
                ValidationCodes.DeviceConfigDataInvalidDeviceType => Cultures.Resources.Error_Invalid_Device_Type,
                ValidationCodes.DeviceConfigDataInvalidGroup => Cultures.Resources.Error_Invalid_Group_Num,
                ValidationCodes.DeviceConfigDataInvalidZone => Cultures.Resources.Error_Invalid_Zone_Num,
                ValidationCodes.DeviceConfigDataDeviceNameTooLong => Cultures.Resources.Error_Device_Name_Too_Long,
                ValidationCodes.DeviceConfigDataInvalidSounderGroup => Cultures.Resources.Error_Invalid_Sounder_Group,
                ValidationCodes.DeviceConfigDataInvalidIOInputOutput => Cultures.Resources.Error_Invalid_IO_Input_Output,
                ValidationCodes.DeviceConfigDataInvalidIOChannel => Cultures.Resources.Error_Invalid_IO_Channel,
                ValidationCodes.DeviceConfigDataInvalidIOZone => Cultures.Resources.Error_Invalid_IO_Zone,
                ValidationCodes.DeviceConfigDataInvalidIOSet => Cultures.Resources.Error_Invalid_IO_Set,
                ValidationCodes.DeviceConfigDataIODescriptionTooLong => Cultures.Resources.Error_Invalid_IO_Description_Too_Long,
                ValidationCodes.DeviceConfigDataInvalidDaySensitivity => Cultures.Resources.Error_Invalid_Day_Sensitivity,
                ValidationCodes.DeviceConfigDataInvalidNightSensitivity => Cultures.Resources.Error_Invalid_Night_Sensitivity,
                ValidationCodes.DeviceConfigDataInvalidDayVolume => Cultures.Resources.Error_Invalid_Day_Volume,
                ValidationCodes.DeviceConfigDataInvalidNightVolume => Cultures.Resources.Error_Invalid_Night_Volume,
                ValidationCodes.DeviceConfigDataInvalidDayMode => Cultures.Resources.Error_Invalid_Day_Mode,
                ValidationCodes.DeviceConfigDataInvalidNightMode => Cultures.Resources.Error_Invalid_Night_Mode,
                
                //device names
                ValidationCodes.DeviceNamesTooManyBytes => Cultures.Resources.Error_Device_Name_Bytes_Exceeded,
                ValidationCodes.DeviceNamesDataTooLong => Cultures.Resources.Error_Device_Name_Too_Long,
                ValidationCodes.DeviceNamesDataBlankEntry => Cultures.Resources.Error_Device_Name_Blank,

                //zones
                ValidationCodes.ZoneConfigDataInvalidZoneNum => Cultures.Resources.Error_Invalid_Zone_Num,
                ValidationCodes.ZoneConfigDataZoneNameTooLong => Cultures.Resources.Error_Zone_Name_Too_Long,
                ValidationCodes.ZoneConfigDataSounderDelayTooLong => Cultures.Resources.Error_Sounder_Delay_Too_Long,
                ValidationCodes.ZoneConfigDataRelay1DelayTooLong => Cultures.Resources.Error_Relay_1_Delay_Too_Long,
                ValidationCodes.ZoneConfigDataRelay2DelayTooLong => Cultures.Resources.Error_Relay_2_Delay_Too_Long,
                ValidationCodes.ZoneConfigDataOutputDelayTooLong => Cultures.Resources.Error_Output_Delay_Too_Long,
                ValidationCodes.ZoneConfigDataTotalDelayTooLong => Cultures.Resources.Error_Total_Delay_Is_Too_Long,
                ValidationCodes.ZoneConfigDataDayOptionInvalid => Cultures.Resources.Error_Day_Dependency_Option_Invalid,
                ValidationCodes.ZoneConfigDataDayDetectorResetTooLong => Cultures.Resources.Error_Day_Detector_Reset_Too_Long,
                ValidationCodes.ZoneConfigDataDayAlarmResetTooLong => Cultures.Resources.Error_Day_Alarm_Reset_Too_Long,
                ValidationCodes.ZoneConfigDataNightOptionInvalid => Cultures.Resources.Error_Night_Dependency_Option_Invalid,
                ValidationCodes.ZoneConfigDataNightDetectorResetTooLong => Cultures.Resources.Error_Night_Detector_Reset_Too_Long,
                ValidationCodes.ZoneConfigDataNightAlarmResetTooLong => Cultures.Resources.Error_Night_Alarm_Reset_Too_Long,

                //zone panels
                ValidationCodes.ZonePanelConfigDataInvalidPanelNum => Cultures.Resources.Error_Invalid_Panel_Num,
                ValidationCodes.ZonePanelConfigDataPanelNameTooLong => Cultures.Resources.Error_Panel_Name_Too_Long,
                ValidationCodes.ZonePanelConfigDataSounderDelayTooLong => Cultures.Resources.Error_Sounder_Delay_Too_Long,
                ValidationCodes.ZonePanelConfigDataRelay1DelayTooLong => Cultures.Resources.Error_Relay_1_Delay_Too_Long,
                ValidationCodes.ZonePanelConfigDataRelay2DelayTooLong => Cultures.Resources.Error_Relay_2_Delay_Too_Long,
                ValidationCodes.ZonePanelConfigDataOutputDelayTooLong => Cultures.Resources.Error_Output_Delay_Too_Long,
                ValidationCodes.ZonePanelConfigDataTotalDelayTooLong => Cultures.Resources.Error_Total_Delay_Is_Too_Long,

                //groups

                //sets
                ValidationCodes.SetConfigDelayTimerTooLong => Cultures.Resources.Error_Set_Delay_Timer_Too_Long,

                //site config
                ValidationCodes.SiteConfigNoSystemNameWarning => Cultures.Resources.Error_No_System_Name,
                ValidationCodes.SiteConfigNoClientName => Cultures.Resources.Error_No_Client_Name,
                ValidationCodes.SiteConfigNoClientAddress => Cultures.Resources.Error_Missing_Client_Address,
                ValidationCodes.SiteConfigNoClientTel => Cultures.Resources.Error_Missing_Client_Tel,
                ValidationCodes.SiteConfigNoInstallerName => Cultures.Resources.Error_No_Installer_Name,
                ValidationCodes.SiteConfigNoInstallerAddress => Cultures.Resources.Error_Missing_Installer_Address,
                ValidationCodes.SiteConfigNoInstallerTel => Cultures.Resources.Error_Missing_Installer_Tel,
                ValidationCodes.SiteConfigNoEngineerName => Cultures.Resources.Error_Missing_Engineer_Name,
                ValidationCodes.SiteConfigNoInstallDate => Cultures.Resources.Error_Missing_Installation_Date,
                ValidationCodes.SiteConfigNoCommissionDate => Cultures.Resources.Error_Missing_Commissioning_Date,
                ValidationCodes.SiteConfigQuiescentStringBlank => Cultures.Resources.Error_Quiescent_String_Blank,
                ValidationCodes.SiteConfigQuiescentStringTooLong => Cultures.Resources.Error_Quiescent_String_Too_long,
                ValidationCodes.SiteConfigMaintenanceStringBlank => Cultures.Resources.Error_Maintenance_String_Blank,
                ValidationCodes.SiteConfigMaintenanceStringTooLong => Cultures.Resources.Error_Maintenance_String_Too_long,
                ValidationCodes.SiteConfigAL2CodeError => Cultures.Resources.Error_Invalid_AL2_Code,
                ValidationCodes.SiteConfigAL3CodeError => Cultures.Resources.Error_Invalid_AL3_Code,
                
                //C&E config
                ValidationCodes.CEActionLoop1DeviceNotSet => Cultures.Resources.Error_CE_Action_Loop_1_Device_Not_Set,
                ValidationCodes.CEActionLoop2DeviceNotSet => Cultures.Resources.Error_CE_Action_Loop_2_Device_Not_Set,
                ValidationCodes.CEActionDeviceUnassigned => Cultures.Resources.Error_CE_Action_Device_Unassigned,
                ValidationCodes.CEActionPanelRelayNotSet => Cultures.Resources.Error_CE_Action_Panel_Relay_Not_Set,
                ValidationCodes.CEActionGroupNotSet => Cultures.Resources.Error_CE_Action_Group_Not_Set,
                ValidationCodes.CEActionSounderGroupNotSet => Cultures.Resources.Error_CE_Action_Sounder_Group_Not_Set,
                ValidationCodes.CEActionBeaconGroupNotSet => Cultures.Resources.Error_CE_Action_Beacon_Group_Not_Set,
                ValidationCodes.CEActionZoneNotSet => Cultures.Resources.Error_CE_Action_Zone_Not_Set,
                ValidationCodes.CEActionOutputSetNotSet => Cultures.Resources.Error_CE_Action_Output_Not_Set,
                ValidationCodes.CEActionNetworkEventNotSet => Cultures.Resources.Error_CE_Action_Network_Event_Not_Set,
                ValidationCodes.CETriggerTypeNotSet => Cultures.Resources.Error_CE_Trigger_Type_Not_Set,
                ValidationCodes.CETriggerLoop1DeviceNotSet => Cultures.Resources.Error_CE_Trigger_Loop_1_Device_Not_Set,
                ValidationCodes.CETriggerLoop2DeviceNotSet => Cultures.Resources.Error_CE_Trigger_Loop_2_Device_Not_Set,
                ValidationCodes.CETriggerDeviceUnassigned => Cultures.Resources.Error_CE_Trigger_Device_Unassigned,
                ValidationCodes.CETriggerZoneOrPanelNotSet => Cultures.Resources.Error_CE_Trigger_Zone_Or_Panel_Not_Set,
                ValidationCodes.CETriggerPanelInputNotSet => Cultures.Resources.Error_CE_Trigger_Panel_Input_Not_Set,
                ValidationCodes.CETriggerEventNotSet => Cultures.Resources.Error_CE_Trigger_Event_Not_Set,
                ValidationCodes.CETriggerEvent2NotSet => Cultures.Resources.Error_CE_Trigger_Event_2_Not_Set,
                ValidationCodes.CETriggerTimerNotSet => Cultures.Resources.Error_CE_Trigger_Timer_Not_Set,
                ValidationCodes.CETriggerNetworkEventNotSet => Cultures.Resources.Error_CE_Trigger_Network_Event_Not_Set,
                ValidationCodes.CETriggerZoneNotSet => Cultures.Resources.Error_CE_Trigger_Zone_Not_Set,
                ValidationCodes.CETriggerZone2NotSet => Cultures.Resources.Error_CE_Trigger_Zone_2_Not_Set,
                ValidationCodes.CETriggerConditionNotSet => Cultures.Resources.Error_CE_Trigger_Condition_Not_Set,
                ValidationCodes.CEResetTypeNotSet => Cultures.Resources.Error_CE_Reset_Type_Not_Set,
                ValidationCodes.CEResetLoop1DeviceNotSet => Cultures.Resources.Error_CE_Reset_Loop_1_Device_Not_Set,
                ValidationCodes.CEResetLoop2DeviceNotSet => Cultures.Resources.Error_CE_Reset_Loop_2_Device_Not_Set,
                ValidationCodes.CEResetDeviceUnassigned => Cultures.Resources.Error_CE_Reset_Device_Unassigned,
                ValidationCodes.CEResetZoneOrPanelNotSet => Cultures.Resources.Error_CE_Reset_Zone_Or_Panel_Not_Set,
                ValidationCodes.CEResetPanelInputNotSet => Cultures.Resources.Error_CE_Reset_Panel_Input_Not_Set,
                ValidationCodes.CEResetEventNotSet => Cultures.Resources.Error_CE_Reset_Event_Not_Set,
                ValidationCodes.CEResetEvent2NotSet => Cultures.Resources.Error_CE_Reset_Event_2_Not_Set,
                ValidationCodes.CEResetTimerNotSet => Cultures.Resources.Error_CE_Reset_Timer_Not_Set,
                ValidationCodes.CEResetNetworkEventNotSet => Cultures.Resources.Error_CE_Reset_Network_Event_Not_Set,
                ValidationCodes.CEResetZoneNotSet => Cultures.Resources.Error_CE_Reset_Zone_Not_Set,
                ValidationCodes.CEResetZone2NotSet => Cultures.Resources.Error_CE_Reset_Zone_2_Not_Set,
                ValidationCodes.CEResetConditionNotSet => Cultures.Resources.Error_CE_Reset_Condition_Not_Set,

                //network config
                ValidationCodes.NetworkConfigInvalidPanelName => Cultures.Resources.Error_Panel_Name_Too_Long,
                ValidationCodes.NetworkConfigNoPanelLocation => Cultures.Resources.Error_Missing_Panel_Location,

                _ => ""
            };
    }
}
