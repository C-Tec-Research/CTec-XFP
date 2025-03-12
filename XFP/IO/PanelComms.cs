using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CTecUtil.IO;
using CTecControls.UI;
using Xfp.DataTypes.PanelData;
using Xfp.DataTypes;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using static Xfp.DataTypes.PanelData.XfpPanelData;
using CTecUtil.StandardPanelDataTypes;
using System.Windows.Input.Manipulations;
using CTecDevices.Protocol;
using System.Security.Policy;
using CTecUtil;

namespace Xfp.IO
{
    internal partial class PanelComms
    {
        static PanelComms()
        {
            SerialComms.ShowErrorMessage  = error;
            SerialComms.ShowErrorMessage2 = (m, m2) => { if (string.IsNullOrEmpty(m2)) error(m); else error2(m, m2); };
            SerialComms.PingTimerPeriod   = 3000;
        }


        internal static void InitialisePingCommands()
        {
            SerialComms.PingCommand            = XfpCommands.Poll;
            SerialComms.LoggingModePingCommand = XfpCommands.LoggingModeKeepPoll;
            SerialComms.CheckFirmwareCommand   = XfpCommands.RequestFirmwareVersion;
            SerialComms.CheckWriteableCommand  = XfpCommands.CheckNvmLink;
            SerialComms.CheckProtocolCommand   = XfpCommands.RequestLoopType;
        }


        /// <summary>
        /// Controls the polling byte sent to the panel
        /// </summary>
        internal static SerialComms.PingModes PollingMode { set => SerialComms.PingMode = value; }


        private static void error(string message) => Application.Current.Dispatcher.Invoke(new Action(() => { CTecMessageBox.ShowOKError(message, Cultures.Resources.Panel_Comms); }), DispatcherPriority.Normal);
        private static void error2(string message, string message2) => Application.Current.Dispatcher.Invoke(new Action(() => { CTecMessageBox.ShowOKError2(message, message2, Cultures.Resources.Panel_Comms); }), DispatcherPriority.Normal);


        /// <summary>
        /// Query the panel's firmware version.
        /// </summary>
        /// <param name="downloading">True if the upcoming operation is a download, false if it's an upload.</param>
        /// <param name="receiver">Delegate that will handle the response - i.e. check the version number is valid.</param>
        //internal static void QueryFirmwareVersion()
        //{
        //    SerialComms.InitCommandQueue(Cultures.Resources.Firmware_Version);
        //    SerialComms.AddNewCommandSubqueue(SerialComms.Direction.Download, Cultures.Resources.Firmware_Version, null);
        //    SerialComms.EnqueueCommand(XfpCommands.RequestFirmwareVersion(), receiveFirmwareVersion);
        //    SendCommandQueueToPanel(null);
        //}


        /// <summary>
        /// Start a new command queue with the given name
        /// </summary>
        /// <param name="operationName">Name of the process to be displayed in the progress bar window.<br/>
        /// E.g. 'Downloading from panel...'</param>
        internal static void InitCommandQueue(CommsDirection direction, string operationName)
            => SerialComms.InitCommandQueue(direction, operationName, DeviceTypes.CurrentProtocolName);


        /// <summary>
        /// Start a new command queue with the given name
        /// </summary>
        /// <param name="name">Name of the queue - to display in the progress bar window while this queue is being processed.</param>
        internal static void InitNewDownloadCommandSubqueue(string name, SerialComms.SubqueueCompletedHandler onCompletion)
            => SerialComms.AddNewCommandSubqueue(CTecUtil.CommsDirection.Download, name, onCompletion);


        /// <summary>
        /// Start a new command queue with the given name
        /// </summary>
        /// <param name="name">Name of the queue - to display in the progress bar window while this queue is being processed.</param>
        internal static void InitNewUploadCommandSubqueue(string name, SerialComms.SubqueueCompletedHandler onCompletion)
            => SerialComms.AddNewCommandSubqueue(CTecUtil.CommsDirection.Upload, name, onCompletion);


        #region add request commands

        #region firmware version
        internal static void AddCommandRequestFirmwareVersion() => SerialComms.EnqueueCommand(XfpCommands.RequestFirmwareVersion(), null, receiveFirmwareVersion);
        #endregion

        # region devices
        internal static void AddCommandRequestDevice(int loopNum, int deviceIndex, string description)           => SerialComms.EnqueueCommand(XfpCommands.RequestDevice(loopNum, deviceIndex), description, deviceIndex, loopNum, receiveDevice);
        internal static void AddCommandRequestBaseSounderGroup(int loopNum, int deviceIndex, string description) => SerialComms.EnqueueCommand(XfpCommands.RequestDevice(loopNum, deviceIndex += DeviceConfigData.NumDevices + 1), description, deviceIndex, loopNum, receiveBaseSounderGroup);
        internal static void AddCommandRequestDeviceName(int nameIndex, string description)                      => SerialComms.EnqueueCommand(XfpCommands.RequestDeviceName(nameIndex), description, nameIndex, receiveDeviceName);
        #endregion

        # region zones, groups, sets
        internal static void AddCommandRequestZoneName(int index, string description)   => SerialComms.EnqueueCommand(XfpCommands.RequestZoneName(index), description,  index, receiveZoneName);
        internal static void AddCommandRequestZoneTimers(int index, string description) => SerialComms.EnqueueCommand(XfpCommands.RequestZoneTimers(index), description, index, receiveZoneTimers);
        internal static void AddCommandRequestPhasedSettings(string description)        => SerialComms.EnqueueCommand(XfpCommands.RequestPhasedSettings(), description, receivePhasedSettings);
        internal static void AddCommandRequestZoneGroup(int index, string description)  => SerialComms.EnqueueCommand(XfpCommands.RequestZoneGroup(index), description, index, receiveZoneGroup);
        internal static void AddCommandRequestZoneSet(int index, string description)    => SerialComms.EnqueueCommand(XfpCommands.RequestZoneSet(index), description, index, receiveZoneSet);
        #endregion

        #region site config
        internal static void AddCommandRequestQuiescentString(string description)         => SerialComms.EnqueueCommand(XfpCommands.RequestQuiescentString(), description, receiveQuiescentString);
        internal static void AddCommandRequestMaintenanceString(string description)       => SerialComms.EnqueueCommand(XfpCommands.RequestMaintenanceString(), description, receiveMaintenanceString);
        internal static void AddCommandRequestMaintenanceDate(string description)         => SerialComms.EnqueueCommand(XfpCommands.RequestMaintenanceDate(), description, receiveMaintenanceDate);
        internal static void AddCommandRequestAL2Code(string description)                 => SerialComms.EnqueueCommand(XfpCommands.RequestAL2Code(), description, receiveAL2Code);
        internal static void AddCommandRequestAL3Code(string description)                 => SerialComms.EnqueueCommand(XfpCommands.RequestAL3Code(), description, receiveAL3Code);
        internal static void AddCommandRequestDayNight(string description)                => SerialComms.EnqueueCommand(XfpCommands.RequestDayNight(), description, receiveDayNight);
        #endregion

        #region c&e config
        internal static void AddCommandRequestCEEvent(int index, string description)      => SerialComms.EnqueueCommand(XfpCommands.RequestCEEvent(index), description, index, receiveCEEvent);
        #endregion

        #region network config
        internal static void AddCommandRequestRepeaterName(int index, string description) => SerialComms.EnqueueCommand(XfpCommands.RequestRepeaterName(index), description, index, receiveRepeaterName);
        internal static void AddCommandRequestNetworkPanelData(string description)        => SerialComms.EnqueueCommand(XfpCommands.RequestNetworkPanelData(), description, receiveNetworkPanelData);
        #endregion

        #region event log viewer
        internal static void AddCommandRequestEventLog(int index, bool clearRecords)      => SerialComms.EnqueueCommand(XfpCommands.RequestEventLog(index, clearRecords), null, receiveEventLogData);
        #endregion

        #endregion


        #region add send commands

        #region devices
        internal static void AddCommandSetDevice(DeviceData device, string description)           => SerialComms.EnqueueCommand(XfpCommands.SetDevice(device), description);
        internal static void AddCommandSetBaseSounderGroup(DeviceData device, string description) => SerialComms.EnqueueCommand(XfpCommands.SetDevice(device), description);
        internal static void AddCommandSetDeviceName(int key, string name, string description)    => SerialComms.EnqueueCommand(XfpCommands.SetDeviceName(key, name), description);
        internal static void AddCommandEndDeviceNameUpload(string description)                    => SerialComms.EnqueueCommand(XfpCommands.SetDeviceNameEnd(), description);
        #endregion

        # region zones, groups, sets
        internal static void AddCommandSetZoneName(IndexedText zone, string description)                               => SerialComms.EnqueueCommand(XfpCommands.SetZoneName(zone), description);
        internal static void AddCommandSetZoneTimers(ZoneConfigData.ZoneTimersBundle zone, string description)         => SerialComms.EnqueueCommand(XfpCommands.SetZoneTimers(zone), description);
        internal static void AddCommandSetPhasedSettings(ZoneConfigData.PhasedSettingsBundle zone, string description) => SerialComms.EnqueueCommand(XfpCommands.SetZonePhasedSettings(zone), description);
        internal static void AddCommandSetZoneGroup(GroupConfigData.GroupBundle group, string description)             => SerialComms.EnqueueCommand(XfpCommands.SetZoneGroup(group), description);
        internal static void AddCommandSetZoneSet(SetConfigData.SetBundle set, string description)                     => SerialComms.EnqueueCommand(XfpCommands.SetZoneSet(set), description);
        #endregion
            
        #region site config
        internal static void AddCommandSetQuiescentString(Text text, string description)                         => SerialComms.EnqueueCommand(XfpCommands.SetQuiescentString(text), description);
        internal static void AddCommandSetMaintenanceString(Text text, string description)                       => SerialComms.EnqueueCommand(XfpCommands.SetMaintenanceString(text), description);
        internal static void AddCommandSetMaintenanceDate(Date date, string description)                         => SerialComms.EnqueueCommand(XfpCommands.SetMaintenanceDate(date), description);
        internal static void AddCommandSetAL2Code(Text al2Code, string description)                              => SerialComms.EnqueueCommand(XfpCommands.SetAL2Code(al2Code), description);
        internal static void AddCommandSetAL3Code(PanelConfigData.AL3CodeBundle al3Code, string description) => SerialComms.EnqueueCommand(XfpCommands.SetAL3Code(al3Code), description);
        internal static void AddCommandSetDayNight(SiteConfigData.DayNightBundle dayNight, string description)   => SerialComms.EnqueueCommand(XfpCommands.SetDayNight(dayNight), description);
        internal static void AddCommandSyncPanelTime(string description)                                         => SerialComms.EnqueueCommand(XfpCommands.SetPanelTime(), description);
        #endregion

        #region c&e config
        internal static void AddCommandSetCEEvent(CEConfigData.CEBundle data, string description)              => SerialComms.EnqueueCommand(XfpCommands.SetCEEvent(data), description);
        #endregion

        #region network config
        internal static void AddCommandSetRepeaterName(IndexedText repeater, string description)                     => SerialComms.EnqueueCommand(XfpCommands.SetRepeaterName(repeater), description, repeater.Index);
        internal static void AddCommandSetNetworkPanelData(NetworkConfigData.NetworkBundle data, string description) => SerialComms.EnqueueCommand(XfpCommands.SetNetworkPanelData(data), description);
        #endregion

        #endregion


        internal static void SendCommandQueueToPanel(Window owner, Action onStart = null, SerialComms.OnFinishedHandler onEnd = null)
        {
            if (string.IsNullOrWhiteSpace(SerialComms.Settings.PortName))
            {
                error(Cultures.Resources.Error_Serial_Port_Settings);
                return;
            }

            if (owner != null)
                SerialComms.OwnerWindow = owner;
            SerialComms.StartSendingCommandQueue(onStart, onEnd);
        }


        internal static void TerminateSubqueue() => SerialComms.CancelCurrentQueue();


        #region delegates for returning data to calling process

        internal delegate bool PanelDataHandler(object d);

        internal static PanelDataHandler AckReceived;

        internal static PanelDataHandler FirmwareVersionReceived;

        internal static PanelDataHandler DeviceReceived;
        internal static PanelDataHandler BaseSounderGroupReceived;
        internal static PanelDataHandler DeviceNameReceived;

        internal static PanelDataHandler ZoneNameReceived;
        internal static PanelDataHandler ZoneTimersReceived;
        internal static PanelDataHandler PhasedSettingsReceived;
        internal static PanelDataHandler ZoneGroupReceived;
        internal static PanelDataHandler ZoneSetReceived;

        internal static PanelDataHandler QuiescentStringReceived;
        internal static PanelDataHandler MaintenanceStringReceived;
        internal static PanelDataHandler MaintenanceDateReceived;
        internal static PanelDataHandler AL2CodeReceived;
        internal static PanelDataHandler AL3CodeReceived;
        internal static PanelDataHandler DayNightReceived;
        
        internal static PanelDataHandler CEEventReceived;

        internal static PanelDataHandler RepeaterNameReceived;
        internal static PanelDataHandler NetworkPanelDataReceived;

        internal static PanelDataHandler EventLogDataReceived;

        #endregion


        #region handlers for received data
        private static SerialComms.MultiIndexResponseDataHandler receiveDevice           = new((data, index1, index2) => (bool)DeviceReceived?.Invoke  (DeviceData.Parse                           (data, XfpCommands.ResponseIsDeviceRequest, index1, index2)));

        private static SerialComms.MultiIndexResponseDataHandler receiveBaseSounderGroup = new((data, index1, index2) => (bool)BaseSounderGroupReceived?.Invoke(DeviceData.Parse                   (data, XfpCommands.ResponseIsDeviceRequest, index1, index2)));

        private static SerialComms.ReceivedResponseDataHandler receiveDeviceName         = new((data, index) => (bool)DeviceNameReceived?.Invoke       (DeviceData.ParseDeviceName                 (data, XfpCommands.ResponseIsDeviceNameRequest, index)));

        private static SerialComms.ReceivedResponseDataHandler receiveZoneName           = new((data, index) => (bool)ZoneNameReceived?.Invoke         (IndexedText.Parse                          (data, XfpCommands.ResponseIsZoneNameRequest, index, ZoneConfigData.MaxNameLength)));
        private static SerialComms.ReceivedResponseDataHandler receiveZoneTimers         = new((data, index) => (bool)ZoneTimersReceived?.Invoke       (ZoneConfigData.ZoneTimersBundle.Parse      (data, XfpCommands.ResponseIsZoneTimersRequest, index)));
        private static SerialComms.ReceivedResponseDataHandler receivePhasedSettings     = new((data, index) => (bool)PhasedSettingsReceived?.Invoke   (ZoneConfigData.PhasedSettingsBundle.Parse  (data, XfpCommands.ResponseIsPhasedSettingsRequest)));
        private static SerialComms.ReceivedResponseDataHandler receiveZoneGroup          = new((data, index) => (bool)ZoneGroupReceived?.Invoke        (GroupConfigData.GroupBundle.Parse          (data, XfpCommands.ResponseIsZoneGroupRequest, index)));
        private static SerialComms.ReceivedResponseDataHandler receiveZoneSet            = new((data, index) => (bool)ZoneSetReceived?.Invoke          (SetConfigData.SetBundle.Parse              (data, XfpCommands.ResponseIsZoneSetRequest, index)));

        private static SerialComms.ReceivedResponseDataHandler receiveFirmwareVersion    = new((data, index) => (bool)FirmwareVersionReceived?.Invoke  (Text.Parse                                 (data, XfpCommands.ResponseIsFirmwareVersionRequest,   XfpPanelData.FirmwareVersionLength, true)));
        private static SerialComms.ReceivedResponseDataHandler receiveQuiescentString    = new((data, index) => (bool)QuiescentStringReceived?.Invoke  (Text.Parse                                 (data, XfpCommands.ResponseIsQuiescentStringRequest,   PanelConfigData.MaxQuiescentStringLength)));
        private static SerialComms.ReceivedResponseDataHandler receiveMaintenanceString  = new((data, index) => (bool)MaintenanceStringReceived?.Invoke(Text.Parse                                 (data, XfpCommands.ResponseIsMaintenanceStringRequest, PanelConfigData.MaxMaintenanceStringLength)));
        private static SerialComms.ReceivedResponseDataHandler receiveMaintenanceDate    = new((data, index) => (bool)MaintenanceDateReceived?.Invoke  (Date.Parse                                 (data, XfpCommands.ResponseIsMaintenanceDateRequest)));
        private static SerialComms.ReceivedResponseDataHandler receiveAL2Code            = new((data, index) => (bool)AL2CodeReceived?.Invoke          (PanelConfigData.ParseAL2Code           (data, XfpCommands.ResponseIsAL2CodeRequest)));
        private static SerialComms.ReceivedResponseDataHandler receiveAL3Code            = new((data, index) => (bool)AL3CodeReceived?.Invoke          (PanelConfigData.AL3CodeBundle.Parse    (data, XfpCommands.ResponseIsAL3CodeRequest)));
        private static SerialComms.ReceivedResponseDataHandler receiveDayNight           = new((data, index) => (bool)DayNightReceived?.Invoke         (SiteConfigData.DayNightBundle.Parse        (data, XfpCommands.ResponseIsDayNightRequest)));

        private static SerialComms.ReceivedResponseDataHandler receiveCEEvent            = new((data, index) => (bool)CEEventReceived?.Invoke          (CEConfigData.CEBundle.Parse                (data, XfpCommands.ResponseIsCEEventRequest, index)));

        private static SerialComms.ReceivedResponseDataHandler receiveRepeaterName       = new((data, index) => (bool)RepeaterNameReceived?.Invoke     (IndexedText.Parse                          (data, XfpCommands.ResponseIsRepeaterNameRequest, index, ZoneConfigData.MaxNameLength)));
        private static SerialComms.ReceivedResponseDataHandler receiveNetworkPanelData   = new((data, index) => (bool)NetworkPanelDataReceived?.Invoke (NetworkConfigData.NetworkBundle.Parse      (data, XfpCommands.ResponseIsNetworkPanelDataRequest)));

        private static SerialComms.ReceivedResponseDataHandler receiveEventLogData       = new((data, index) => (bool)EventLogDataReceived?.Invoke     (Text.Parse                                 (data, XfpCommands.ResponseIsEventLogDataRequest, XfpData.EventLogDataLength)));
        #endregion
    }
}