using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CTecControls.UI;
using CTecUtil;
using Windows.Storage.Streams;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;

namespace Xfp.Files
{
    internal class LocalXfpFile
    {
        //internal delegate bool PanelNumberConfirmer(int panelNumber);
        //internal static PanelNumberConfirmer ConfirmPanelNumber;


        internal static bool CheckForLegacyXfpFile(string path)
        {
            try
            {
                using (StreamReader strm = new StreamReader(path))
                {
                    //old-style file is identifiable by the first line
                    return XfpFile.FileParsingXfp.ItemName(strm.ReadLine()) == XfpFile.FileParsingXfp.XfpTags.FileXfpTools;
                }
            }
            catch (Exception ex)
            {
                CTecUtil.Debug.WriteLine(ex.ToString());
                return false;
            }
        }


        //internal static XfpData ReadFile(string path)
        //{
        //    try
        //    {
        //        using (StreamReader firstPass = new StreamReader(path))
        //        {
        //            //check first line and if it is an old-style XFP file attempt to parse it
        //            if (XfpFile.FileParsingXfp.ItemName(firstPass.ReadLine()) == XfpFile.FileParsingXfp.XfpTags.FileXfpTools)
        //            {
        //                //first read the protocol type (so we know how many devices to initialise) and panel number
        //                CTecDevices.ObjectTypes protocol = CTecDevices.ObjectTypes.XfpCast;
        //                int panelNumber = 1;
        //                if (XfpFile.FileParsingXfp.ReadProtocolAndPanelNumber(firstPass, ref protocol, ref panelNumber))
        //                {
        //                    if (ConfirmPanelNumber?.Invoke(panelNumber) ?? true)
        //                    {
        //                        //now reread from the start to populate the data
        //                        using (StreamReader secondPass = new StreamReader(path))
        //                        {
        //                            return normaliseData(XfpFile.FileParsingXfp.ParseXfp(secondPass, protocol, panelNumber));
        //                        }
        //                    }
        //                }
        //            }

        //            //not an XFP file - assume it's json
        //            CTecUtil.Debug.WriteLine("Reading json file");
        //            return normaliseData(XfpTextFile.DeserializeJson(File.ReadAllText(path)));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CTecUtil.Debug.WriteLine(ex.ToString());
        //        return null;
        //    }
        //}


        internal static bool ReadDefiningSettings(string path, out CTecDevices.ObjectTypes protocol, out int panelNumber, out string firmwareVersion)
        {
            protocol = CTecDevices.ObjectTypes.XfpCast;
            panelNumber = XfpData.MinPanelNumber;
            firmwareVersion = CTecControls.Cultures.Resources.Unknown;

            try
            {
                using (StreamReader strm = new StreamReader(path))
                {
                    return XfpFile.FileParsingXfp.ReadDefiningSettings(strm, ref protocol, ref panelNumber, ref firmwareVersion);
                }
            }
            catch (Exception ex)
            {
                CTecUtil.Debug.WriteLine(ex.ToString());
            }

            return false;
        }


        internal static XfpData ReadLegacyXfpFile(string path, CTecDevices.ObjectTypes protocol, int panelNumber)
        {
            using (StreamReader strm = new StreamReader(path))
            {
                return normaliseData(XfpFile.FileParsingXfp.ParseXfp(strm, protocol, panelNumber));
            }
        }

        internal static XfpData ReadJsonXfpFile(string path) => normaliseData(XfpTextFile.DeserializeJson(File.ReadAllText(path)));

        internal static XfpData ReadOlderJsonXfpFile(string path) => normaliseData(new XfpData(XfpTextFile.DeserializeVersion1Json(File.ReadAllText(path))));
        

        private static XfpData normaliseData(XfpData data)
        {
            if (data is not null)
            {
                foreach (var p in data.Panels.Values)
                {
                    //PanelConfig is null in the old file format that contained
                    //only 1 panel, so initialise with its global settings
                    if (p.PanelConfig is null)
                        p.PanelConfig = new(data);

                    normaliseData(p);
                }
            }
            return data;
        }


        private static XfpPanelData normaliseData(XfpPanelData panelData)
        {
            if (panelData is not null)
            {
                //reading from a json file will create the default number
                //of devices, so we remove any above the protocol's limit
                if (panelData.Loop1Config.Devices.Count > DeviceConfigData.NumDevices)
                    panelData.Loop1Config.Devices.RemoveRange(DeviceConfigData.NumDevices, panelData.Loop1Config.Devices.Count - DeviceConfigData.NumDevices);
                if (panelData.Loop2Config.Devices.Count > DeviceConfigData.NumDevices)
                    panelData.Loop2Config.Devices.RemoveRange(DeviceConfigData.NumDevices, panelData.Loop2Config.Devices.Count - DeviceConfigData.NumDevices);

                //ensure the DeviceNames table contains *something*
                if (panelData.DeviceNamesConfig.DeviceNames.Count == 0)
                    panelData.DeviceNamesConfig = DeviceNamesConfigData.InitialisedNew();

                //clear (i.e. set to -1 or null) any values 'below' an unset action/trigger/reset type
                foreach (var e in panelData.CEConfig.Events)
                {
                    if (e.ActionType == CEActionTypes.None)
                    {
                        e.ActionParam = -1;
                        e.TriggerType = CETriggerTypes.None;
                    }
                    if (e.ActionType == CEActionTypes.None || e.TriggerType == CETriggerTypes.None)
                    {
                        e.TriggerParam = e.TriggerParam2 = -1;
                        e.TriggerType = CETriggerTypes.None;
                        e.TriggerCondition = false;
                        e.ResetType = CETriggerTypes.None;
                    }
                    if (e.ActionType == CEActionTypes.None || e.TriggerType == CETriggerTypes.None || e.ResetType == CETriggerTypes.None)
                    {
                        e.ResetParam = e.ResetParam2 = -1;
                        e.ResetCondition = false;
                    }
                }

                foreach (var s in panelData.SetConfig.Sets)
                {
                    for (int t = 0; t < s.OutputSetTriggers.Count; t++)
                        if (s.OutputSetTriggers[t] < 0 || (int)s.OutputSetTriggers[t] >= Enum.GetNames(typeof(SetTriggerTypes)).Length)
                            s.OutputSetTriggers[t] = SetTriggerTypes.NotTriggered;
                    for (int t = 0; t < s.PanelRelayTriggers.Count; t++)
                        if (s.PanelRelayTriggers[t] < 0 || (int)s.PanelRelayTriggers[t] >= Enum.GetNames(typeof(SetTriggerTypes)).Length)
                            s.PanelRelayTriggers[t] = SetTriggerTypes.NotTriggered;
                }

                //copy panel names from the zone data to the network config's repeater names
                foreach (var zp in panelData.ZonePanelConfig.Panels)
                {
                    if (zp.Index < panelData.NetworkConfig.RepeaterSettings.Repeaters.Count)
                    {
                        panelData.NetworkConfig.RepeaterSettings.Repeaters[zp.Index].Name = zp.Name;
                        panelData.NetworkConfig.RepeaterSettings.Repeaters[zp.Index].Index = zp.Index;
                    }
                    else
                    {
                        panelData.NetworkConfig.RepeaterSettings.Repeaters.Add(new() { Index = zp.Index, Name = zp.Name });
                    }
                }

                ////set the network config Fitted flags
                //panelData.NetworkConfig.RepeaterSettings.Repeaters[panelData.PanelNumber - 1].Fitted = true;
            }

            return panelData;
        }


        private static string fileReadError(string path) => string.Format("Could not read the file: '{0}", path);
    }
}
