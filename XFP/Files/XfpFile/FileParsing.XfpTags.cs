using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xfp.Files.XfpFile
{
    internal partial class FileParsingXfp
    {
        internal class XfpTags : Tags
        {
            internal const string FileXfpTools = "File XFPTools";

            internal const string ObjectCETable   = "Object C_E Table";
            internal const string ArrayCEArray    = "Array C_E Array";
            internal const string ArrayTimeEvents = "Array Time Events";

            internal const string UseInSpecialCAndE     = "Use in Special C&E";
            internal const string ArrayZoneList         = "Array Zone List";
            internal const string ZoneName              = "Name";
            internal const string ZoneMCP               = "MCP";
            internal const string ZoneDetector          = "Detector";
            internal const string ZoneRemoteDelay       = "Remote Delay";
            internal const string ZoneSounderDelay      = "Sounder Delay";
            internal const string ZoneRelay1Delay       = "Relay 1 Delay";
            internal const string ZoneRelay2Delay       = "Relay 2 Delay";
            internal const string ZoneDetectorReset     = "Detector Reset";
            internal const string ZoneAlarmReset        = "Alarm Reset";
            internal const string ZoneNonFire           = "NonFire";
            internal const string ZoneEndDelays         = "End Delays";
            internal const string ZoneArrayGroups       = "Array Groups";
            internal const string ZoneArrayZoneSets     = "Array Zone Sets";
            internal const string ZoneArrayDependencies = "Array Dependencies";

            internal const string ObjectRepeaterList = "Object Repeater List";
            internal const string RepeaterSegment    = "Segment";
            internal const string RepeaterPanelName  = "Panel Name";
            internal const string RepeaterItemName   = "Name";
            internal const string RepeaterItemFitted = "Fitted";
            internal const string RepeaterArrayRepeater = "Array Repeater";
            internal const string RepeaterArrayOutput   = "Array Output";

            internal const string ObjectGroupList     = "Object Group List";
            internal const string GroupArrayGroup     = "Array Group";
            internal const string GroupArrayOutputSet = "Array Output Set";

            internal const string ObjectXfpNetwork          = "Object XFP Network";
            internal const string NetworkArrayPanelsData    = "Array Panels Data";
            internal const string NetworkAcceptFaults       = "Accept_Faults";
            internal const string NetworkAcceptAlarms       = "Accept_Alarms";
            internal const string NetworkAcceptControls     = "Accept_Controls";
            internal const string NetworkAcceptDisablements = "Accept_Disablements";
            internal const string NetworkAcceptOccupied     = "Accept_Occupied";

            internal const string ObjectLoop               = "Object Loop";
            internal const string ObjectLoop1              = "Object Loop 1";
            internal const string ObjectLoop2              = "Object Loop 2";
            internal const string LoopDeviceIndex          = "Device Index";
            internal const string LoopGroup                = "Group";
            internal const string LoopAsIOUnit             = "As I/O Unit";
            internal const string LoopArrayDevice          = "Array Device";
            internal const string LoopDeviceType           = "Device Type";
            internal const string LoopDeviceZone           = "Zone";
            internal const string LoopDeviceName           = "Name";
            internal const string LoopDeviceHint           = "Hint";
            internal const string LoopDeviceGroup          = "Group";
            internal const string LoopDeviceNameIndex      = "Name Index";
            internal const string LoopDeviceHasBaseSounder = "Has Base Sounder";
            internal const string LoopDeviceBaseSounder    = "Base Sounder";
            internal const string LoopDeviceTypeChanged    = "Type Changed";
            internal const string LoopDeviceArraySharedData= "Array Shared Data";
            internal const string LoopDeviceArraySubName   = "Array SubName";

            internal const string LoopCount = "Loop Count";
            internal const string SystemType = "System Type";
            internal const string DateEnabled    = "Date Enabled";
            internal const string SoundersPulsed = "SoundersPulsed";
            internal const string CopyTime = "CopyTime";
            internal const string FaultLockout = "FaultLockout";
            internal const string PhasedDelay = "PhasedDelay";
            internal const string InvestigationPeriod = "InvestigationPeriod";
            internal const string InvestigationPeriod1 = "InvestigationPeriod1";
            internal const string PanelLocation = "PanelLocation";
            internal const string EngineerNo = "EngineerNo";
            internal const string MaintenanceString = "MaintenanceString";
            internal const string QuiescentString = "QuiescentString";
            internal const string Engineer = "Engineer";
            internal const string MaintenanceDate = "MaintenanceDate";
            internal const string AL3Code = "AL3Code";
            internal const string AL2Code = "AL2Code";
            internal const string ClientName = "ClientName";
            internal const string Loop2 = "Loop2";
            internal const string FrontPanel = "FrontPanel";
            internal const string Loop1 = "Loop1";
            internal const string MainVersion = "MainVersion";
            internal const string Installer = "Installer";
            internal const string ClientAddress3 = "ClientAddress3";
            internal const string ClientAddress2 = "ClientAddress2";
            internal const string ClientAddress1 = "ClientAddress1";
            internal const string ClientAddress5 = "ClientAddress5";
            internal const string ClientAddress4 = "ClientAddress4";
            internal const string InstallerAddress3 = "InstallerAddress3";
            internal const string InstallerAddress2 = "InstallerAddress2";
            internal const string InstallerAddress1 = "InstallerAddress1";
            internal const string InstallerAddress5 = "InstallerAddress5";
            internal const string InstallerAddress4 = "InstallerAddress4";
            internal const string NightBegin = "NightBegin";
            internal const string NightEnd = "NightEnd";
            internal const string RecalibrationTime = "ReCal";
            internal const string PanelSounder1_Group = "PanelSounder1_Group";
            internal const string PanelSounder2_Group = "PanelSounder2_Group";
            internal const string Day_Enable_Flags = "Day_Enable_Flags";
            internal const string Night_Enable_Flags = "Night_Enable_Flags";
            internal const string Polling_LED = "Polling_LED";
            internal const string MCP_Delay = "MCP_Delay";
            internal const string Detector_Delay = "Detector_Delay";
            internal const string IO_Delay = "IO_Delay";
            internal const string ReSound_Function = "ReSound_Function";
            internal const string BST_Adjustment = "BST_Adjustment";
            internal const string RealTime_Event_Output = "RealTime_Event_Output";
            internal const string PanelNumber = "Panel Number";
            internal const string IntermittentTone = "Intermittent Tone";
            internal const string ContinuousTone = "Continuous Tone";
            internal const string DelayTimer = "Delay Timer";
            internal const string NoNameAllocated = "No name allocated";

        }
    }
}
