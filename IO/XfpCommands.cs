using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CTecUtil;
using CTecUtil.IO;
using CTecUtil.StandardPanelDataTypes;
using Windows.ApplicationModel.VoiceCommands;
using Xfp.DataTypes.PanelData;
using static CTecUtil.IO.SerialComms;
using static Xfp.DataTypes.PanelData.XfpPanelData;

namespace Xfp.IO
{
    internal class XfpCommands
    {
        static XfpCommands()
        {
            SerialComms.AckByte = XfpCommandCodes.Ack;
            SerialComms.NakByte = XfpCommandCodes.Nak;
        }


        internal delegate int PanelNumberGetter();
        private static PanelNumberGetter getPanelNumber;


        internal static void Initialise(PanelNumberGetter getPanelNumber) => XfpCommands.getPanelNumber = getPanelNumber;


        internal static byte[] Poll()                => buildPingCommand(XfpCommandCodes.StandardPing);
        internal static byte[] LoggingModeKeepPoll() => buildPingCommand(XfpCommandCodes.LoggingPing);
        internal static byte[] CheckNvmLink()        => buildPingCommand(XfpCommandCodes.PollNvmLink);
        internal static byte[] RequestLoopType()     => buildCommand(XfpCommandCodes.RequestLoopType);


        internal const int FirmwareRevLength = 5;


        #region request panel data
        internal static byte[] RequestDevice(int loop, int index) => buildCommand(XfpCommandCodes.RequestPData, new byte[] { (byte)(index + 1), (byte)loop });
        internal static byte[] RequestDeviceName(int index)       => buildCommand(XfpCommandCodes.RequestPointName, new byte[] { (byte)(index) });

        internal static byte[] RequestZoneName(int index)         => buildCommand(XfpCommandCodes.RequestZoneName, new byte[] { (byte)(index + 1) });
        internal static byte[] RequestZoneTimers(int index)       => buildCommand(XfpCommandCodes.RequestZoneTimers, new byte[] { (byte)(index + 1) });
        internal static byte[] RequestPhasedSettings()            => buildCommand(XfpCommandCodes.RequestPhasedSettings);
        internal static byte[] RequestZoneGroup(int index)        => buildCommand(XfpCommandCodes.RequestZGroup, new byte[] { (byte)(index + 1) });
        internal static byte[] RequestZoneSet(int index)          => buildCommand(XfpCommandCodes.RequestZSet, new byte[] { (byte)(index) });

        internal static byte[] RequestFirmwareVersion()           => buildCommand(XfpCommandCodes.RequestMainVersion);
        internal static byte[] RequestAL2Code()                   => buildCommand(XfpCommandCodes.RequestAL2Code);
        internal static byte[] RequestAL3Code()                   => buildCommand(XfpCommandCodes.RequestAL3Code);
        internal static byte[] RequestQuiescentString()           => buildCommand(XfpCommandCodes.RequestQuiescentName);
        internal static byte[] RequestMaintenanceString()         => buildCommand(XfpCommandCodes.RequestMaintName);
        internal static byte[] RequestMaintenanceDate()           => buildCommand(XfpCommandCodes.RequestMaintDate);
        internal static byte[] RequestDayNight()                  => buildCommand(XfpCommandCodes.RequestDayNight);

        internal static byte[] RequestCEEvent(int index)          => buildCommand(XfpCommandCodes.RequestCEEvent, new byte[] { (byte)(index + 1) });

        internal static byte[] RequestRepeaterName(int index)     => buildCommand(XfpCommandCodes.RequestRepeaterName, new byte[] { (byte)(index + 1) });
        internal static byte[] RequestNetworkPanelData()          => buildCommand(XfpCommandCodes.RequestNetPanelData);

        internal static byte[] RequestEventLog(int index, bool delete)
        {
            //var indexBytes = ByteArrayProcessing.IntToByteArray(index, 2);
            //var jj = indexBytes.Concat(new byte[]{ (byte)1});
            //return buildCommand(XfpCommandCodes.RequestEvent, (byte[]) ByteArrayProcessing.IntToByteArray(index, 2).Concat(new byte[] { (byte)(delete ? 1 : 0) }));
            var indexBytes = ByteArrayProcessing.IntToByteArray(index, 2);
            return buildCommand(XfpCommandCodes.RequestEvent, new byte[] { indexBytes[0], indexBytes[1], (byte)(delete ? 1 : 0) });
        }
        #endregion


        #region set panel data
        internal static byte[] SetDevice(DeviceData device)             => buildCommand(XfpCommandCodes.SetPData, device.ToByteArray());
        internal static byte[] SetDeviceName(int key, string name)      => buildCommand(XfpCommandCodes.SetPointName, DeviceData.DeviceNameToByteArray(key, name));
        internal static byte[] SetDeviceNameEnd()                       => buildCommand(XfpCommandCodes.SetPointName, DeviceData.DeviceNameameDataEndToByteArray());

        internal static byte[] SetZoneName(IndexedText zone)                                       => buildCommand(XfpCommandCodes.SetZoneName, zone.ToByteArray());
        internal static byte[] SetZoneTimers(ZoneConfigData.ZoneTimersBundle timers)               => buildCommand(XfpCommandCodes.SetZoneTimers, timers.ToByteArray());
        internal static byte[] SetZonePhasedSettings(ZoneConfigData.PhasedSettingsBundle settings) => buildCommand(XfpCommandCodes.SetPhasedSettings, settings.ToByteArray());
        internal static byte[] SetZoneGroup(GroupConfigData.GroupBundle group)                     => buildCommand(XfpCommandCodes.SetZGroup, group.ToByteArray());
        internal static byte[] SetZoneSet(SetConfigData.SetBundle set)                             => buildCommand(XfpCommandCodes.SetZSet, set.ToByteArray());

        internal static byte[] SetAL2Code(Text al2Code)                                  => buildCommand(XfpCommandCodes.SetAL2Code, ByteArrayProcessing.IntStrToByteArray(al2Code.Value, 2));
        internal static byte[] SetAL3Code(SiteConfigData.AL3CodeBundle al3Code)          => buildCommand(XfpCommandCodes.SetAL3Code, al3Code.ToByteArray());
        internal static byte[] SetQuiescentString(Text text)                             => buildCommand(XfpCommandCodes.SetQuiescentName, text.ToByteArray());
        internal static byte[] SetMaintenanceString(Text text)                           => buildCommand(XfpCommandCodes.SetMaintName, text.ToByteArray());
        internal static byte[] SetMaintenanceDate(Date date)                             => buildCommand(XfpCommandCodes.SetMaintDate, date.ToByteArray("yyMMddHHmmss"));
        internal static byte[] SetDayNight(SiteConfigData.DayNightBundle dayNight)       => buildCommand(XfpCommandCodes.SetDayNight, dayNight.ToByteArray());
        internal static byte[] SetPanelTime()                                            => buildCommand(XfpCommandCodes.SetDateTime, SiteConfigData.PcTime.ToByteArray());
        internal static byte[] SetCEEvent(CEConfigData.CEBundle data)                    => buildCommand(XfpCommandCodes.SetCEEvent, data.ToByteArray());
        internal static byte[] SetRepeaterName(IndexedText repeater)                     => buildCommand(XfpCommandCodes.SetRepeaterName, repeater.ToByteArray());
        internal static byte[] SetNetworkPanelData(NetworkConfigData.NetworkBundle data) => buildCommand(XfpCommandCodes.SetNetPanelData, data.ToByteArray());
        #endregion


        internal static bool ResponseIsLooptypeRequest(byte[] data)          => data?[0] == XfpCommandCodes.RequestLoopType;

        internal static bool ResponseIsDeviceRequest(byte[] data)            => data?[0] == XfpCommandCodes.RequestPData;
        internal static bool ResponseIsDeviceNameRequest(byte[] data)        => data?[0] == XfpCommandCodes.RequestPointName;
        
        internal static bool ResponseIsZoneNameRequest(byte[] data)          => data?[0] == XfpCommandCodes.RequestZoneName;
        internal static bool ResponseIsZoneTimersRequest(byte[] data)        => data?[0] == XfpCommandCodes.RequestZoneTimers;
        internal static bool ResponseIsPhasedSettingsRequest(byte[] data)    => data?[0] == XfpCommandCodes.RequestPhasedSettings;
        internal static bool ResponseIsZoneGroupRequest(byte[] data)         => data?[0] == XfpCommandCodes.RequestZGroup;
        internal static bool ResponseIsZoneSetRequest(byte[] data)           => data?[0] == XfpCommandCodes.RequestZSet;

        internal static bool ResponseIsFirmwareVersionRequest(byte[] data)   => data?[0] == XfpCommandCodes.RequestMainVersion;
        internal static bool ResponseIsAL2CodeRequest(byte[] data)           => data?[0] == XfpCommandCodes.RequestAL2Code;
        internal static bool ResponseIsAL3CodeRequest(byte[] data)           => data?[0] == XfpCommandCodes.RequestAL3Code;
        internal static bool ResponseIsQuiescentStringRequest(byte[] data)   => data?[0] == XfpCommandCodes.RequestQuiescentName;
        internal static bool ResponseIsMaintenanceStringRequest(byte[] data) => data?[0] == XfpCommandCodes.RequestMaintName;
        internal static bool ResponseIsMaintenanceDateRequest(byte[] data)   => data?[0] == XfpCommandCodes.RequestMaintDate;
        internal static bool ResponseIsDayNightRequest(byte[] data)          => data?[0] == XfpCommandCodes.RequestDayNight;

        internal static bool ResponseIsCEEventRequest(byte[] data)           => data?[0] == XfpCommandCodes.RequestCEEvent;

        internal static bool ResponseIsRepeaterNameRequest(byte[] data)      => data?[0] == XfpCommandCodes.RequestRepeaterName;
        internal static bool ResponseIsNetworkPanelDataRequest(byte[] data)  => data?[0] == XfpCommandCodes.RequestNetPanelData;

        internal static bool ResponseIsEventLogDataRequest(byte[] data)      => data?[0] == XfpCommandCodes.RequestEvent;


        /// <summary>
        /// Build one of the ping commands (poll, NVM link check, etc.)</code>
        /// </summary>
        /// <param name="command">The command identifier byte, denoting the type of data to send or receive.</param>
        private static byte[] buildPingCommand(byte command)
        {
            if (getPanelNumber is null)
                throw new NotImplementedException("XfpCommands.GetPanelNumber has not been initialised");
            byte[] data = new byte[] { XfpCommandCodes.CommandStartByte, (byte)getPanelNumber?.Invoke(), command, 0 };
            return ByteArrayProcessing.CombineByteArrays(data, new byte[] { SerialComms.CalcChecksum(data, true) });
        }

        
        /// <summary>
        /// Build full command data bytes in the form:<code>[upload/download indicator byte][data type byte][payload length byte][payload......][checksum byte]</code>
        /// </summary>
        /// <param name="command">The command identifier byte, denoting the type of data to send or receive.</param>
        /// <param name="index">Index of data requested (if any)</param>
        private static byte[] buildCommand(byte command, short index) => buildCommand(command, new byte[] { (byte)((index & 0xff00) >> 8), (byte)(index & 0xff) });

        /// <summary>
        /// Build full command data bytes in the form:<code>[upload/download indicator byte][data type byte][payload length byte][payload......][checksum byte]</code>
        /// </summary>
        /// <param name="command">The command identifier byte, denoting the type of data to send or receive.</param>
        /// <param name="payload">Data to send to the panel, or index of data requested (if any)</param>
        private static byte[] buildCommand(byte command, byte[] payload = null)
        {
            if (getPanelNumber is null)
                throw new NotImplementedException("XfpCommands.GetPanelNumber has not been initialised");
            var payloadLength = payload?.Length ?? 0;
            byte[] prefix = new byte[4];
            prefix[0] = XfpCommandCodes.CommandStartByte;
            prefix[1] = (byte)getPanelNumber?.Invoke();
            prefix[2] = command;
            prefix[3] = (byte)payloadLength;

            var data = payloadLength > 0 ? ByteArrayProcessing.CombineByteArrays(prefix, payload) : prefix;
            var checksum = SerialComms.CalcChecksum(data, true);
            return ByteArrayProcessing.CombineByteArrays(data, new byte[] { checksum });
        }

    }
}
