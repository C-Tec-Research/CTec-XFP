using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using Xfp.DataTypes.PanelData;
using Xfp.ViewModels;
using CTecDevices.Protocol;
using System.Threading.Channels;
using Xfp.DataTypes;
using CTecDevices;

namespace Xfp.Files.XfpFile
{
    internal partial class FileParsingXfp
    {
        private static void parseDeviceConfig(StreamReader inputStream, ref XfpData result, int loopNum)
        {
            string currentLine;
            while ((currentLine = readNext(inputStream, XfpTags.EndObject)) != null)
            {
                // NB: the "Loop As IO Unit" tag is not used.

                var loop = loopNum > 1 ? result.CurrentPanel.LoopConfig.Loop2 : result.CurrentPanel.LoopConfig.Loop1;
                switch (ItemName(currentLine))
                {
                    //case XfpTags.LoopDeviceIndex: loop.DeviceIndex = parseInt(currentLine); break;
                    case XfpTags.LoopGroup:       loop.Group       = parseInt(currentLine); break;
                    case XfpTags.LoopArrayDevice: parseDevices(inputStream, ref result, loopNum); break;
                }
            }
        }


        internal static int parseLoopNum(string currentLine)
        {
            var split = currentLine.Split(' ');
            int loopNum;
            if (int.TryParse(split[split.Length - 1], out loopNum))
                return loopNum;
            return 0;
        }


        private static void parseDevices(StreamReader inputStream, ref XfpData result, int loopNum)
        {
            string currentLine;
            while ((currentLine = readNext(inputStream, XfpTags.EndArray)) != null)
            {
                if (ItemName(currentLine) == XfpTags.ObjectItem)
                {
                    var index = parseItemIndex(currentLine);
                    while ((currentLine = readNext(inputStream, XfpTags.EndObject)) != null)
                    {
                        if (index >= 0)
                        {
                            // NB: Zones/Groups:  the "Group" tag is not used - zones or groups are stored in the "Zone" field.
                            //
                            // NB: device names:  the "Name Index" tag is redundant, as are the name indices in the "Stored
                            //                    Data".  Instead, we read the actual names from "Name" (which is the main
                            //                    name and IO desc[0]) and the "SubName" array (the other 3 IO descs).
                            //
                            // NB: base sounders: the "Has Base Sounder" tag is redundant because the group number is read
                            //                    from "Shared Data" (provided it's valid for the device type).
                            // 
                            // NB: The "Hint" and "Type Changed" tags are not used.

                            var loop = loopNum > 1 ? result.CurrentPanel.LoopConfig.Loop2 : result.CurrentPanel.LoopConfig.Loop1;

                            if (index < DeviceConfigData.NumDevices)
                            {
                                var device = loop.Devices[index];

                                switch (ItemName(currentLine))
                                {
                                    case XfpTags.LoopDeviceType:            int dType = parseInt(currentLine); device.DeviceType = dType > 0 && dType < 254 ? dType : null; break;
                                    case XfpTags.LoopDeviceZone:            parseZone(currentLine, ref result, ref device); break;
                                    case XfpTags.LoopDeviceName:            device.NameIndex      = device.IOConfig[0].NameIndex = parseDeviceName(ref result, currentLine); break;
                                    case XfpTags.LoopDeviceArraySharedData: parseSharedData(inputStream, ref result, ref device); break;
                                    case XfpTags.LoopDeviceArraySubName:    parseSubNames(inputStream, ref result, ref device); break;
                                }
                            }
                            else if (DeviceTypes.SoundersCanHaveRemoteDevices(result.CurrentPanel.Protocol) && index > DeviceConfigData.NumDevices && index < DeviceConfigData.NumDevices * 2)
                            {
                                // Apollo protocol: only 126/255 devices are used; the upper half of the devices array in the file
                                // is used to store the base sounder group of the device (in the Zone field, would you believe?)
                                // (...and these upper device indices are offset by an additional 1 on top of the 126, ffs)

                                var device = loop.Devices[index - DeviceConfigData.NumDevices - 1];

                                switch (ItemName(currentLine))
                                {
                                    case XfpTags.LoopDeviceZone: device.AncillaryBaseSounderGroup = DeviceTypes.CanHaveAncillaryBaseSounder(device.DeviceType, DeviceTypes.CurrentProtocolType) ? parseInt(currentLine) : null; break;
                                }
                            }
                        }
                    }
                }
            }
        }


        private static void parseZone(string currentLine, ref XfpData result, ref DeviceData device)
        {
            if (device.IsIODevice)
                return;

            if (DeviceTypes.IsZonalDevice(device.DeviceType, result.CurrentPanel.Protocol))
                device.Zone = parseInt(currentLine);
            else if (DeviceTypes.IsGroupedDevice(device.DeviceType, result.CurrentPanel.Protocol))
                device.Group = parseInt(currentLine);
        }


        private static void parseSharedData(StreamReader inputStream, ref XfpData result, ref DeviceData device)
        {
            string currentLine;
            while ((currentLine = readNext(inputStream, XfpTags.EndArray)) != null)
            {
                if (ItemName(currentLine) == XfpTags.Item)
                {
                    var index = parseItemIndex(currentLine);
                    var value = parseInt(currentLine);

                    if (index >= 0 && index < DeviceData.NumSharedData)
                    {
                        if (device.IsSensitivityDevice)
                        {
                            switch (index)
                            {
                                case 2: device.DaySensitivity   = value; break;
                                case 3: device.NightSensitivity = value; break;
                                case 7: device.RemoteLEDEnabled = value > 0; break;
                            }
                        }
                        else if (device.IsVolumeDevice)
                        {
                            switch (index)
                            {
                                case 2: device.DayVolume   = value - 1; break;
                                case 3: device.NightVolume = value - 1; break;
                            }
                        }
                        else if (device.IsIODevice)
                        {
                            if (index % 2 == 0)
                            {
                                var i = index / 2;

                                while (device.IOConfig.Count <= i)
                                    device.IOConfig.Add(new(i, device.DeviceType));

                                if (value == 0xff)
                                {
                                    device.IOConfig[i].InputOutput  = IOTypes.NotUsed;
                                    device.IOConfig[i].Channel      = null;
                                    device.IOConfig[i].ZoneGroupSet = null;

                                    if (i > 0)
                                        device.IOConfig[i].NameIndex = 0;
                                }
                                else
                                {
                                    var isInput = (value & 0x80) == 0;
                                    device.IOConfig[i].InputOutput  = isInput ? IOTypes.Input       : IOTypes.Output;
                                    device.IOConfig[i].Channel      = isInput ? (value & 0x40) >> 6 : Math.Max(0,((value & 0x60) >> 5) - 1);
                                    device.IOConfig[i].ZoneGroupSet = isInput ? value & 0x3f        : value & 0x1f;
                                }                            
                            }
                        }
                    }
                }
            }
        }


        private static int parseDeviceName(ref XfpData result, string currentLine)
        {
            var name = parseName(currentLine);

            if (name == "")
                return 0;

            //is the name already in the list?
            var idx = result.CurrentPanel.DeviceNamesConfig.GetNameIndex(name);
            if (idx > 0)
                return idx;

            //add to the device names table
            return result.CurrentPanel.DeviceNamesConfig.Add(name);
        }


        private static string parseName(string currentLine)
        {
            var name = ParseString(currentLine);
            return name == XfpTags.NoNameAllocated ? "" : name;
        }


        private static void parseSubNames(StreamReader inputStream, ref XfpData result, ref DeviceData device)
        {
            string currentLine;
            while ((currentLine = readNext(inputStream, XfpTags.EndArray)) != null)
            {
                if (ItemName(currentLine) == XfpTags.Item)
                {
                    var index = parseItemIndex(currentLine);
                    if (index >= 0 && index < DeviceData.NumSubNames)
                        device.IOConfig[index + 1].NameIndex = parseDeviceName(ref result, currentLine);
                }
            }
        }

    }
}