using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;

namespace Xfp.Files.XfpFile
{
    internal partial class FileParsingXfp
    {
        private static void parseRepeaterList(StreamReader inputStream, ref XfpData result)
        {
            string currentLine;
            while ((currentLine = readNext(inputStream, XfpTags.EndObject)) != null)
            {
                switch (ItemName(currentLine))
                {
                    case XfpTags.RepeaterSegment:       result.CurrentPanel.NetworkConfig.RepeaterSettings.Segment = parseInt(currentLine); break;
                    case XfpTags.RepeaterPanelName:     /*result.NetworkConfig.RepeaterSettings.PanelName = ParseString(currentLine);*/ break;
                    case XfpTags.RepeaterArrayRepeater: parseRepeaters(inputStream, ref result); break;
                    case XfpTags.RepeaterArrayOutput:   parseOutputs(inputStream, ref result); break;

                }
            }
        }


        private static void parseRepeaters(StreamReader inputStream, ref XfpData result)
        {
            string currentLine;
            while ((currentLine = readNext(inputStream, XfpTags.EndArray)) != null)
            {
                if (ItemName(currentLine) == XfpTags.ObjectItem)
                {
                    int index = parseItemIndex(currentLine);
                    if (index > 0 && index <= result.CurrentPanel.NetworkConfig.RepeaterSettings.Repeaters.Count)
                            parseRepeaterItem(inputStream, result.CurrentPanel.NetworkConfig.RepeaterSettings.Repeaters[index - 1]);
                }
            }
        }


        private static void parseOutputs(StreamReader inputStream, ref XfpData result)
        {
            //string currentLine;
            //while ((currentLine = readNext(inputStream, XfpTags.EndArray)) != null)
            //{
            //    if (ItemName(currentLine) == XfpTags.ObjectItem)
            //    {
            //        int index = parseItemIndex(currentLine);
            //        if (index > 0 && index <= result.CurrentPanel.NetworkConfig.PanelSettings.Count)
            //            parseNetworkPanelItem(inputStream, result.CurrentPanel.NetworkConfig.PanelSettings, index);
            //    }
            //}
        }


        private static void parseRepeaterItem(StreamReader inputStream, NetworkRepeaterItemData repeaterItem)
        {
            string currentLine;
            while ((currentLine = readNext(inputStream, XfpTags.EndObject)) != null)
            {
                switch (ItemName(currentLine))
                {
                    case XfpTags.RepeaterItemName:   repeaterItem.Name   = ParseString(currentLine); break;
                    case XfpTags.RepeaterItemFitted: repeaterItem.Fitted = parseBool(currentLine); break;
                }
            }
        }
    }
}