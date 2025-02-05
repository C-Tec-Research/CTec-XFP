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
        private static void parseNetworkConfig(StreamReader inputStream, ref XfpData result)
        {
            string currentLine;
            while ((currentLine = readNext(inputStream, XfpTags.EndObject)) != null)
            {
                switch (ItemName(currentLine))
                {
                    case XfpTags.NetworkArrayPanelsData: parseNetworkPanels(inputStream, ref result); break;
                }
            }
        }


        private static void parseNetworkPanels(StreamReader inputStream, ref XfpData result)
        {
            string currentLine;
            while ((currentLine = readNext(inputStream, XfpTags.EndArray)) != null)
                if (ItemName(currentLine) == XfpTags.ObjectItem)
                    parseNetworkPanelItem(inputStream, ref result, parseItemIndex(currentLine));
        }


        private static void parseNetworkPanelItem(StreamReader inputStream, ref XfpData result, int index)
        {
            string currentLine;
            while ((currentLine = readNext(inputStream, XfpTags.EndObject)) != null)
            {
                if (index > 0 && index <= result.CurrentPanel.NetworkConfig.PanelSettings.Count)
                {
                    switch (ItemName(currentLine))
                    {
                        case XfpTags.NetworkAcceptFaults:       result.CurrentPanel.NetworkConfig.PanelSettings[index - 1].AcceptFaults       = parseBool(currentLine); break;
                        case XfpTags.NetworkAcceptAlarms:       result.CurrentPanel.NetworkConfig.PanelSettings[index - 1].AcceptAlarms       = parseBool(currentLine); break;
                        case XfpTags.NetworkAcceptControls:     result.CurrentPanel.NetworkConfig.PanelSettings[index - 1].AcceptControls     = parseBool(currentLine); break;
                        case XfpTags.NetworkAcceptDisablements: result.CurrentPanel.NetworkConfig.PanelSettings[index - 1].AcceptDisablements = parseBool(currentLine); break;
                        case XfpTags.NetworkAcceptOccupied:     result.CurrentPanel.NetworkConfig.PanelSettings[index - 1].AcceptOccupied     = parseBool(currentLine); break;
                    }
                }
            }
        }

    }
}
