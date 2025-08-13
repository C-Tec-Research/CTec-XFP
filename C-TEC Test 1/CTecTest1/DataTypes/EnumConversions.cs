using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xfp.DataTypes
{
    internal class EnumConversions
    {
        internal static AlarmTypes IntToAlarmType(int value)             => value switch { 1 => AlarmTypes.Alert, 2 => AlarmTypes.Evacuate, _ => AlarmTypes.Off };
        internal static SetTriggerTypes IntToSetTriggerType(int value)   => value switch { 1 => SetTriggerTypes.Pulsed, 2 => SetTriggerTypes.Continuous, 3 => SetTriggerTypes.Delayed, _ => SetTriggerTypes.NotTriggered };
        internal static byte SetTriggerTypeToByte(SetTriggerTypes value) => value switch { SetTriggerTypes.Pulsed => 1, SetTriggerTypes.Continuous => 2, SetTriggerTypes.Delayed => 3, _ => 0 };

        internal static ZoneDependencyOptions ByteToZoneDependencyOption(byte dep)
            => dep switch 
            {
                0x41 => ZoneDependencyOptions.A,
                0x42 => ZoneDependencyOptions.B,
                0x43 => ZoneDependencyOptions.C,
                0x44 => ZoneDependencyOptions.Dwelling,
                0x49 => ZoneDependencyOptions.Investigation,
                0x4e => ZoneDependencyOptions.Normal,
                _    => ZoneDependencyOptions.NotSet,
            };

        internal static byte ZoneDependencyOptionToByte(ZoneDependencyOptions dep)
            => dep switch 
            {
                ZoneDependencyOptions.A             => 0x41,
                ZoneDependencyOptions.B             => 0x42,
                ZoneDependencyOptions.C             => 0x43,
                ZoneDependencyOptions.Dwelling      => 0x44,
                ZoneDependencyOptions.Investigation => 0x49,
                ZoneDependencyOptions.Normal or _   => 0x4e,
            };

        internal static CETriggerTypes ByteToCETriggerType(byte value)
            => value >= (byte)CETriggerTypes.ZoneAnd                  ? CETriggerTypes.ZoneAnd 
             : value >= (byte)CETriggerTypes.AnyDwellingAndCommunal   ? CETriggerTypes.AnyDwellingAndCommunal
             : value >= (byte)CETriggerTypes.NetworkEventTriggered    ? CETriggerTypes.NetworkEventTriggered 
             : value >= (byte)CETriggerTypes.MoreThanOneZoneInAlarm   ? CETriggerTypes.MoreThanOneZoneInAlarm 
             : value >= (byte)CETriggerTypes.MoreThanOneAlarm         ? CETriggerTypes.MoreThanOneAlarm 
             : value >= (byte)CETriggerTypes.AnyDisablement           ? CETriggerTypes.AnyDisablement 
             : value >= (byte)CETriggerTypes.AnyRemotePanelInFire     ? CETriggerTypes.AnyRemotePanelInFire 
             : value >= (byte)CETriggerTypes.AnyDeviceInAlarm         ? CETriggerTypes.AnyDeviceInAlarm 
             : value >= (byte)CETriggerTypes.ZoneHasDeviceInAlarm     ? CETriggerTypes.ZoneHasDeviceInAlarm 
             : value >= (byte)CETriggerTypes.TimerEventTn             ? CETriggerTypes.TimerEventTn 
             : value >= (byte)CETriggerTypes.PanelUnoccupied          ? CETriggerTypes.PanelUnoccupied 
             : value >= (byte)CETriggerTypes.PanelOccupied            ? CETriggerTypes.PanelOccupied 
             : value >= (byte)CETriggerTypes.EventAnd                 ? CETriggerTypes.EventAnd 
             : value >= (byte)CETriggerTypes.OtherEventTriggered      ? CETriggerTypes.OtherEventTriggered 
             : value >= (byte)CETriggerTypes.PanelInput               ? CETriggerTypes.PanelInput 
             : value >= (byte)CETriggerTypes.AnyFault                 ? CETriggerTypes.AnyFault 
             : value >= (byte)CETriggerTypes.Loop2DevicePrealarm      ? CETriggerTypes.Loop2DevicePrealarm 
             : value >= (byte)CETriggerTypes.Loop1DevicePrealarm      ? CETriggerTypes.Loop1DevicePrealarm 
             : value >= (byte)CETriggerTypes.AnyPrealarm              ? CETriggerTypes.AnyPrealarm 
             : value >= (byte)CETriggerTypes.PanelReset               ? CETriggerTypes.PanelReset 
             : value >= (byte)CETriggerTypes.PanelSilenced            ? CETriggerTypes.PanelSilenced 
             : value >= (byte)CETriggerTypes.AnyZoneInFire            ? CETriggerTypes.AnyZoneInFire 
             : value >= (byte)CETriggerTypes.ZoneOrPanelInFire        ? CETriggerTypes.ZoneOrPanelInFire 
             : value >= (byte)CETriggerTypes.Loop2DeviceTriggered     ? CETriggerTypes.Loop2DeviceTriggered 
             : value >= (byte)CETriggerTypes.Loop1DeviceTriggered     ? CETriggerTypes.Loop1DeviceTriggered
             : CETriggerTypes.None;
    }
}
