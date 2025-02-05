using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;

namespace Xfp.Files.XfpFile
{
    internal partial class FileParsingXfp
    {
        private static void parseMiscTags(string currentLine, ref XfpData result)
        {
            switch (ItemName(currentLine))
            {
                case XfpTags.PanelNumber:           result.CurrentPanel.PanelNumber = parseInt(currentLine); break;
                case XfpTags.PanelSounder1_Group:   result.CurrentPanel.GroupConfig.PanelSounder1Group = parseInt(currentLine); break;
                case XfpTags.PanelSounder2_Group:   result.CurrentPanel.GroupConfig.PanelSounder2Group = parseInt(currentLine); break;
                //case XfpTags.Loop1:                 result.CurrentPanel.Loop1Available = ParseString(currentLine); break;
                //case XfpTags.Loop2:                 result.CurrentPanel.Loop2Available = ParseString(currentLine); break;
                case XfpTags.MainVersion:           result.FirmwareVersion = ParseString(currentLine); break;

                case XfpTags.LoopCount:             result.SiteConfig.LoopCount = parseInt(currentLine); break;
                case XfpTags.SystemType:            result.CurrentPanel.Protocol = parseProtocol(currentLine); break;
                case XfpTags.DateEnabled:           result.SiteConfig.DateEnabled = parseBool(currentLine); break;
                case XfpTags.SoundersPulsed:        result.SiteConfig.SoundersPulsed = parseBool(currentLine); break;
                case XfpTags.CopyTime:              result.SiteConfig.CopyTime = parseBool(currentLine); break;
                case XfpTags.FaultLockout:          result.SiteConfig.FaultLockout = parseInt(currentLine); break;
                case XfpTags.PhasedDelay:           result.CurrentPanel.GroupConfig.PhasedDelay = parseIntTime(currentLine, 0xffff); break;     //0xffff denotes zero phased delay
                case XfpTags.InvestigationPeriod:   result.CurrentPanel.ZoneConfig.InputDelay = parseIntTime(currentLine); break;
                case XfpTags.InvestigationPeriod1:  result.CurrentPanel.ZoneConfig.InvestigationPeriod = parseIntTime(currentLine); break;

                case XfpTags.PanelLocation:         result.CurrentPanel.NetworkConfig.RepeaterSettings.Repeaters[result.CurrentPanel.PanelNumber - 1].Location = ParseString(currentLine); break;
                case XfpTags.AL2Code:               result.SiteConfig.AL2Code = ParseString(currentLine); break;
                case XfpTags.AL3Code:               result.SiteConfig.AL3Code = ParseString(currentLine); break;
                case XfpTags.FrontPanel:            result.SiteConfig.FrontPanel = ParseString(currentLine); break;
                case XfpTags.ClientName:            result.SiteConfig.Client.Name = ParseString(currentLine); break;
                case XfpTags.ClientAddress1:        result.SiteConfig.Client.Address[0] = ParseString(currentLine); break;
                case XfpTags.ClientAddress2:        result.SiteConfig.Client.Address[1] = ParseString(currentLine); break;
                case XfpTags.ClientAddress3:        result.SiteConfig.Client.Address[2] = ParseString(currentLine); break;
                case XfpTags.ClientAddress4:        result.SiteConfig.Client.Address[3] = ParseString(currentLine); break;
                case XfpTags.ClientAddress5:        result.SiteConfig.Client.Postcode = ParseString(currentLine); break;
                case XfpTags.Installer:             result.SiteConfig.Installer.Name = ParseString(currentLine); break;
                case XfpTags.InstallerAddress1:     result.SiteConfig.Installer.Address[0] = ParseString(currentLine); break;
                case XfpTags.InstallerAddress2:     result.SiteConfig.Installer.Address[1] = ParseString(currentLine); break;
                case XfpTags.InstallerAddress3:     result.SiteConfig.Installer.Address[2] = ParseString(currentLine); break;
                case XfpTags.InstallerAddress4:     result.SiteConfig.Installer.Address[3] = ParseString(currentLine); break;
                case XfpTags.InstallerAddress5:     result.SiteConfig.Installer.Postcode = ParseString(currentLine); break;
                case XfpTags.Engineer:              result.SiteConfig.EngineerName = ParseString(currentLine); break;
                case XfpTags.EngineerNo:            result.SiteConfig.EngineerNo = ParseString(currentLine); break;
                case XfpTags.NightBegin:            result.SiteConfig.OccupiedEnds = parseTime(currentLine); break;
                case XfpTags.NightEnd:              result.SiteConfig.OccupiedBegins = parseTime(currentLine); break;
                case XfpTags.RecalibrationTime:     result.SiteConfig.RecalibrationTime = parseTime(currentLine); break;
                case XfpTags.Day_Enable_Flags:      parseDayEnableFlags(currentLine, ref result); break;
                case XfpTags.Night_Enable_Flags:    parseNightEnableFlags(currentLine, ref result); break;
                case XfpTags.MaintenanceString:     result.SiteConfig.MaintenanceString = ParseString(currentLine); break;
                case XfpTags.QuiescentString:       result.SiteConfig.QuiescentString = ParseString(currentLine); break;
                case XfpTags.MaintenanceDate:       result.SiteConfig.MaintenanceDate = parseDate(currentLine); break;
                case XfpTags.Polling_LED:           result.SiteConfig.BlinkPollingLED = parseBool(currentLine); break;
                case XfpTags.MCP_Delay:             result.SiteConfig.MCPDebounce = parseInt(currentLine); break;
                case XfpTags.Detector_Delay:        result.SiteConfig.DetectorDebounce = parseInt(currentLine); break;
                case XfpTags.IO_Delay:              result.SiteConfig.IODebounce = parseInt(currentLine); break;
                case XfpTags.ReSound_Function:      result.CurrentPanel.GroupConfig.ReSoundFunction = parseBool(currentLine); break;
                case XfpTags.BST_Adjustment:        result.SiteConfig.AutoAdjustDST = parseBool(currentLine); break;
                case XfpTags.RealTime_Event_Output: result.SiteConfig.RealTimeEventOutput = parseBool(currentLine); break;
                case XfpTags.IntermittentTone:      result.CurrentPanel.GroupConfig.IntermittentTone = parseInt(currentLine); break;
                case XfpTags.ContinuousTone:        result.CurrentPanel.GroupConfig.ContinuousTone = parseInt(currentLine); break;
                case XfpTags.DelayTimer:            result.CurrentPanel.SetConfig.DelayTimer = parseIntTime(currentLine); break;
            }
        }


        private static void parseAddress(StreamReader inputStream, NameAndAddressData result)
        {
            var addr = parseMemoText(inputStream);
            for (int i = 0; i < addr.Count - 1 && i < result.Address.Count; i++)
                result.Address[i] = addr[i];
            result.Postcode = addr[^1];
        }


        private static void parseDayEnableFlags(string currentLine, ref XfpData result)
        {
            var flags = parseInt(currentLine);
            result.SiteConfig.DayStart.Add((flags & 0x01) > 0);
            result.SiteConfig.DayStart.Add((flags & 0x02) > 0);
            result.SiteConfig.DayStart.Add((flags & 0x04) > 0);
            result.SiteConfig.DayStart.Add((flags & 0x08) > 0);
            result.SiteConfig.DayStart.Add((flags & 0x10) > 0);
            result.SiteConfig.DayStart.Add((flags & 0x20) > 0);
            result.SiteConfig.DayStart.Add((flags & 0x40) > 0);
        }


        private static void parseNightEnableFlags(string currentLine, ref XfpData result)
        {
            var flags = parseInt(currentLine);
            result.SiteConfig.NightStart.Add((flags & 0x01) > 0);
            result.SiteConfig.NightStart.Add((flags & 0x02) > 0);
            result.SiteConfig.NightStart.Add((flags & 0x04) > 0);
            result.SiteConfig.NightStart.Add((flags & 0x08) > 0);
            result.SiteConfig.NightStart.Add((flags & 0x10) > 0);
            result.SiteConfig.NightStart.Add((flags & 0x20) > 0);
            result.SiteConfig.NightStart.Add((flags & 0x40) > 0);
        }
    }
}
