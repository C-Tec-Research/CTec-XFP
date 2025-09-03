using System;
using System.Collections.Generic;
using System.IO;
using CTecControls.UI;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;

namespace Xfp.Files.XfpFile
{
    /// <summary>
    /// Methods for parsing XFP text files
    /// </summary>
    internal partial class FileParsingXfp : FileParsingBase
    {
        /// <summary>Array offset of first device in the XFP file</summary>
        internal const int DeviceListBase = 1;

        ///Array offset of first zone in the XFP file</summary>
        internal const int ZoneListBase = 1;

        /// <summary>Array offset of first group in the XFP file</summary>
        internal const int GroupListBase = 1;

        /// <summary>Array offset of first equation in the XFP file</summary>
        internal const int EquationListBase = 1;

        /// <summary>Array offset of first CE Event in the XFP file</summary>
        internal const int CEEventListBase = 1;

        /// <summary>Array offset of first CE event item in the XFP file</summary>
        internal const int CEEventItemListBase = 1;


        /// <summary>
        /// Finds the Protocol within an XFP file
        /// </summary>
        internal static void ReadDefiningSettings(StreamReader inputStream, ref List<CTecDevices.ObjectTypes> protocols,
                                                                            ref List<int> panelNumbers, 
                                                                            ref List<string> firmwareVersions)
        {
            protocols        = new();
            panelNumbers     = new();
            firmwareVersions = new();

            try
            {
                bool gotProt = false, gotPan = false, gotFw = false;
                string currentLine;
                while ((currentLine = readNext(inputStream, Tags.EndFile)) != null)
                {
                    var item = ItemName(currentLine);

                    if (item == XfpTags.SystemType)                         // <-- legacy file
                    {
                        var p = parseProtocol(currentLine);
                        if (gotProt = p != CTecDevices.ObjectTypes.NotSet)
                            protocols.Add(p);
                    }
                    else if (item == XfpTags.MainVersion)                   // <-- legacy file
                    {
                        var f = ParseString(currentLine);
                        if (gotFw = !string.IsNullOrWhiteSpace(f))
                            firmwareVersions.Add(f);
                    }
                    else if (item == nameof(XfpPanelData.Protocol))         // <-- json file
                        protocols.Add(parseProtocol(currentLine));
                    else if (item == nameof(XfpPanelData.PanelNumber))      // <-- json file
                        panelNumbers.Add(parseInt(currentLine));
                    else if (item == nameof(XfpData.FirmwareVersion))       // <-- json file
                    {
                        var fw = ParseString(currentLine);
                        if (!string.IsNullOrWhiteSpace(fw))
                            firmwareVersions.Add(fw);
                    }

                    if (gotProt && gotPan && gotFw)
                        return;
                }
            }
            catch { }
        }


        /// <summary>
        /// Parse the text file stream as XFP data.
        /// </summary>
        /// <returns>XfpPanelData object populated from the parsed data.</returns>
        internal static XfpData ParseXfp(StreamReader inputStream, CTecDevices.ObjectTypes protocol, int panelNumber)
        {
            LineNumber = 1;
            var result = XfpData.InitialisedNew(protocol, panelNumber, true);

            string currentLine = string.Empty;
            CTecUtil.Debug.WriteLine("Reading XFP file");
            try
            {
                while ((currentLine = readNext(inputStream, Tags.EndFile)) != null)
                {
                    var item = ItemName(currentLine);

                    if (currentLine.StartsWith(XfpTags.ObjectLoop))
                    {
                        parseDeviceConfig(inputStream, ref result, parseLoopNum(currentLine));
                    }
                    else switch (ItemName(currentLine))
                    {
                        case XfpTags.ObjectCETable:      parseCETable(inputStream,       ref result);    break;
                        case XfpTags.ArrayZoneList:      parseZoneList(inputStream,      ref result);    break;
                        case XfpTags.ObjectRepeaterList: parseRepeaterList(inputStream,  ref result);    break;
                        case XfpTags.ObjectGroupList:    parseGroupList(inputStream,     ref result);    break;
                        case XfpTags.ObjectXfpNetwork:   parseNetworkConfig(inputStream, ref result);    break;
                        case Tags.MemoComments:          parseCommentsPage(inputStream,  ref result);    break;
                        default:                         parseMiscTags(currentLine,      ref result);    break;
                    }
                }
            }
            catch (Exception ex)
            {
                CTecUtil.Debug.WriteLine("Conversion Error Line " + " " + currentLine + "\n\n" + ex.Message);
                CTecMessageBox.Show(Cultures.Resources.Error_Could_Not_Parse_File + "\n\n" + ex.Message, Cultures.Resources.Open_File);
            }

            return result;
        }


        private static void parseCommentsPage(StreamReader inputStream, ref XfpData result)
        {
            result.Comments = parseComments(inputStream);
        }
    }
}
