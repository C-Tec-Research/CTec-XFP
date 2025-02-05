using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;
using Xfp.UI.Interfaces;

namespace Xfp.Files.XfpFile
{
    internal partial class FileParsingXfp
    {
        private static void parseZoneList(StreamReader inputStream, ref XfpData result)
        {
            string currentLine;
            while ((currentLine = readNext(inputStream, XfpTags.EndArray)) != null)
            {
                if (ItemName(currentLine) == XfpTags.ObjectItem)
                {
                    int index = parseItemIndex(currentLine);

                    if (index == 0)
                        parseSilenceableSets(inputStream, ref result);
                    else if (index > 0 && index <= ZoneConfigData.NumZones)
                        parseZoneArray(inputStream, ref result, index);
                    else if (index <= ZoneConfigData.NumZones + ZonePanelConfigData.NumZonePanels)
                        parseZonePanelArray(inputStream, ref result, index);
                }
            }
        }


        private static void parseSilenceableSets(StreamReader inputStream, ref XfpData result)
        {
            string currentLine;
            while ((currentLine = readNext(inputStream, XfpTags.EndObject)) != null)
            {
                switch (ItemName(currentLine))
                {
                    case XfpTags.ZoneArrayZoneSets:
                        while ((currentLine = readNext(inputStream, XfpTags.EndArray)) != null)
                        {
                            if (ItemName(currentLine) == XfpTags.Item)
                            {
                                var index = parseItemIndex(currentLine);
                                if (index > 0)
                                {
                                    if (index <= SetConfigData.NumOutputSetTriggers)
                                        result.CurrentPanel.ZoneConfig.OutputSetIsSilenceable[index - 1] = parseInt(currentLine) > 0;
                                    else if (index <= SetConfigData.NumOutputSetTriggers + SetConfigData.NumPanelRelayTriggers)
                                        result.CurrentPanel.ZoneConfig.PanelRelayIsSilenceable[index - SetConfigData.NumOutputSetTriggers - 1] = parseInt(currentLine) > 0;
                                }
                            }
                        }
                        break;
                }
            }
        }


        private static void parseZoneArray(StreamReader inputStream, ref XfpData result, int zoneNum)
        {
            string currentLine;
            while ((currentLine = readNext(inputStream, XfpTags.EndObject)) != null)
            {
                if (zoneNum > 0)
                {
                    int index = zoneNum - 1;
                    switch (ItemName(currentLine))
                    {
                        //case XfpTags.ZoneName:              var name = ParseString(currentLine); if (name != result.ZoneConfig.Zones[index].DefaultName) result.ZoneConfig.Zones[index].Name = name; break;
                        case XfpTags.ZoneMCP:               result.CurrentPanel.ZoneConfig.Zones[index].MCPs = parseBool(currentLine); break;
                        case XfpTags.ZoneDetector:          result.CurrentPanel.ZoneConfig.Zones[index].Detectors = parseBool(currentLine); break;
                        case XfpTags.ZoneRemoteDelay:       result.CurrentPanel.ZoneConfig.Zones[index].RemoteDelay = parseIntTime(currentLine); break;
                        case XfpTags.ZoneSounderDelay:      result.CurrentPanel.ZoneConfig.Zones[index].SounderDelay = parseIntTime(currentLine); break;
                        case XfpTags.ZoneRelay1Delay:       result.CurrentPanel.ZoneConfig.Zones[index].Relay1Delay = parseIntTime(currentLine); break;
                        case XfpTags.ZoneRelay2Delay:       result.CurrentPanel.ZoneConfig.Zones[index].Relay2Delay = parseIntTime(currentLine); break;
                        case XfpTags.ZoneDetectorReset:     result.CurrentPanel.ZoneConfig.Zones[index].DetectorReset = parseInt(currentLine); break;
                        case XfpTags.ZoneAlarmReset:        result.CurrentPanel.ZoneConfig.Zones[index].AlarmReset = parseInt(currentLine); break;
                        case XfpTags.ZoneNonFire:           /*result.CurrentPanel.ZoneConfig.Zones[index].NonFire = parseBool(currentLine);*/ break;
                        case XfpTags.ZoneEndDelays:         result.CurrentPanel.ZoneConfig.Zones[index].EndDelays = parseBool(currentLine); break;
                        case XfpTags.ZoneArrayGroups:       parseGroups(inputStream, ref result, index, false); break;
                        case XfpTags.ZoneArrayZoneSets:     parseSets(inputStream, ref result, index, false); break;
                        case XfpTags.ZoneArrayDependencies: parseDependencies(inputStream, ref result, index, false); break;
                    }
                }
            }
        }


        private static void parseZonePanelArray(StreamReader inputStream, ref XfpData result, int index)
        {
            string currentLine;
            while ((currentLine = readNext(inputStream, XfpTags.EndObject)) != null)
            {
                if (index > 0)
                {
                    var panelIndex = index - ZoneConfigData.NumZones - 1;
                    switch (ItemName(currentLine))
                    {
                        case XfpTags.ZoneName:              result.CurrentPanel.NetworkConfig.RepeaterSettings.Repeaters[panelIndex].Name = ParseString(currentLine); break;
                        case XfpTags.ZoneSounderDelay:      result.CurrentPanel.ZonePanelConfig.Panels[panelIndex].SounderDelay = parseIntTime(currentLine); break;
                        case XfpTags.ZoneRelay1Delay:       result.CurrentPanel.ZonePanelConfig.Panels[panelIndex].Relay1Delay = parseIntTime(currentLine); break;
                        case XfpTags.ZoneRelay2Delay:       result.CurrentPanel.ZonePanelConfig.Panels[panelIndex].Relay2Delay = parseIntTime(currentLine); break;
                        case XfpTags.ZoneRemoteDelay:       result.CurrentPanel.ZonePanelConfig.Panels[panelIndex].RemoteDelay = parseIntTime(currentLine); break;
                        case XfpTags.ZoneArrayGroups:       parseGroups(inputStream, ref result, panelIndex, true); break;
                        case XfpTags.ZoneArrayZoneSets:     parseSets(inputStream, ref result, panelIndex, true); break;
                        //case XfpTags.ZoneArrayDependencies: parseDependencies(inputStream, ref result, panelIndex, true); break;
                    }
                }
            }
        }


        private static void parseGroups(StreamReader inputStream, ref XfpData result, int index, bool isPanel)
        {
            ZoneBase zone = isPanel ? result.CurrentPanel.ZonePanelConfig.Panels[index] : result.CurrentPanel.ZoneConfig.Zones[index];

            string currentLine;
            while ((currentLine = readNext(inputStream, XfpTags.EndArray)) != null)
            {
                if (ItemName(currentLine) == XfpTags.Item)
                {
                    var idx = parseItemIndex(currentLine);
                    if (idx > 0 && idx <= zone.SounderGroups.Count)
                    {
                        zone.SounderGroups[idx - 1] = intToAlarmType(parseInt(currentLine));
                    }
                }
            }
        }


        private static void parseSets(StreamReader inputStream, ref XfpData result, int zoneNum, bool isPanel)
        {
            var set = isPanel ? result.CurrentPanel.SetConfig.Sets[zoneNum + ZoneConfigData.NumZones] : result.CurrentPanel.SetConfig.Sets[zoneNum];

            string currentLine;
            while ((currentLine = readNext(inputStream, XfpTags.EndArray)) != null)
            {
                if (ItemName(currentLine) == XfpTags.Item)
                {
                    var idx = parseItemIndex(currentLine);
                    if (idx > 0)
                    {
                        if (idx <= set.OutputSetTriggers.Count)
                            set.OutputSetTriggers[idx - 1] = intToSetTriggerType(parseInt(currentLine));
                        else if (idx <= set.OutputSetTriggers.Count + set.PanelRelayTriggers.Count)
                            set.PanelRelayTriggers[idx - set.OutputSetTriggers.Count - 1] = intToSetTriggerType(parseInt(currentLine));
                    }
                }
            }
        }


        private static AlarmTypes      intToAlarmType(int t)      => t switch { 1 => AlarmTypes.Alert, 2 => AlarmTypes.Evacuate, _ => AlarmTypes.Off };
        private static SetTriggerTypes intToSetTriggerType(int t) => t switch { 1 => SetTriggerTypes.Pulsed, 2 => SetTriggerTypes.Continuous, 3 => SetTriggerTypes.Delayed, _ => SetTriggerTypes.NotTriggered };


        private static void parseDependencies(StreamReader inputStream, ref XfpData result, int zoneNum, bool isPanel)
        {
            if (!isPanel)
            {
                var zone = result.CurrentPanel.ZoneConfig.Zones[zoneNum];

                string currentLine;
                while ((currentLine = readNext(inputStream, XfpTags.EndArray)) != null)
                {
                    if (ItemName(currentLine) == XfpTags.Item)
                    {
                        var idx = parseItemIndex(currentLine);
                        switch (idx)
                        {
                            case 0: break;
                            case 1: zone.Day.DependencyOption   = parseDependencyOption(currentLine);  break;
                            case 2: zone.Day.DetectorReset      = new(0, 0, parseInt(currentLine)); break;
                            case 3: zone.Day.AlarmReset         = new(0, 0, parseInt(currentLine)); break;
                            case 4: zone.Night.DependencyOption = parseDependencyOption(currentLine);  break;
                            case 5: zone.Night.DetectorReset    = new(0, 0, parseInt(currentLine)); break;
                            case 6: zone.Night.AlarmReset       = new(0, 0, parseInt(currentLine)); break;
                        }
                    }
                }
            }
        }


        private static ZoneDependencyOptions parseDependencyOption(string currentLine)
            => parseInt(currentLine) switch
            {
                1 => ZoneDependencyOptions.A,
                2 => ZoneDependencyOptions.B,
                3 => ZoneDependencyOptions.C,
                4 => ZoneDependencyOptions.Normal,
                5 => ZoneDependencyOptions.Investigation,
                6 => ZoneDependencyOptions.Dwelling,
                _ => ZoneDependencyOptions.NotSet,
            };

    }
}
