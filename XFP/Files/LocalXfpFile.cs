using CTecControls.UI;
using CTecDevices.Protocol;
using CTecUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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


        internal static void ReadDefiningSettings(string path, ref List<CTecDevices.ObjectTypes> protocols, ref List<int> panelNumbers, ref List<string> firmwareVersions)
        {
            try
            {
                using (StreamReader strm = new StreamReader(path))
                {
                    XfpFile.FileParsingXfp.ReadDefiningSettings(strm, ref protocols, ref panelNumbers, ref firmwareVersions);
                    return;
                }
            }
            catch (Exception ex)
            {
                CTecUtil.Debug.WriteLine(ex.ToString());
            }

            return;
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
                    //the old file format contained only 1 panel and its settings were part of the global site.
                    //so if an old-format file is read is will have a null PanelConfig but the panel settings
                    //will be in the "legacy" settings in the site config global settings.
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
                for (int i = panelData.Loop1Config.Devices.Count - 1; i >= DeviceConfigData.NumDevices; i--)
                    panelData.Loop1Config.Devices.RemoveAt(i);
                for (int i = panelData.Loop2Config.Devices.Count - 1; i >= DeviceConfigData.NumDevices; i--)
                    panelData.Loop2Config.Devices.RemoveAt(i);

                //ensure IO devices have valid InputOutput value
                foreach (var d in panelData.Loop1Config.Devices)
                    if (d.IsIODevice)
                        foreach (var io in d.IOConfig)
                            if ((int)io.InputOutput < 0 || (int)io.InputOutput > 2)
                                io.InputOutput = DeviceTypes.DefaultIOOutputType(d.DeviceType, io.Index, DeviceTypes.CurrentProtocolType);

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
