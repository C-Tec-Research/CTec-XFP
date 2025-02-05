using System;
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
        private static void parseGroupList(StreamReader inputStream, ref XfpData result)
        {
            string currentLine;
            while ((currentLine = readNext(inputStream, XfpTags.EndObject)) != null)
            {
                switch (currentLine)
                {
                    case XfpTags.GroupArrayGroup:     parseGroupNames(inputStream, ref result); break;
                    case XfpTags.GroupArrayOutputSet: parseSets(inputStream, ref result); break;
                }
            }
        }


        private static void parseGroupNames(StreamReader inputStream, ref XfpData result)
        {
            string currentLine;
            while ((currentLine = readNext(inputStream, XfpTags.EndArray)) != null)
            {
                //if (ItemName(currentLine) == XfpTags.Item)
                //{
                //    var index = parseItemIndex(currentLine);
                //    if (index > 0 && index < GroupConfigData.NumSounderGroups)
                //    {
                //        //var val = ParseString(currentLine);
                //        //if (val == FileParsingXfp.XfpTags.UseInSpecialCAndE)
                //        //    result.GroupConfig.GroupNames[index] = Cultures.Resources.Use_In_Special_C_And_E;
                //        //else
                //            result.GroupConfig.GroupNames[index] = ParseString(currentLine);
                //    }
                //}
            }
        }


        private static void parseSets(StreamReader inputStream, ref XfpData result)
        {
            string currentLine;
            while ((currentLine = readNext(inputStream, XfpTags.EndArray)) != null)
            {
                //if (ItemName(currentLine) == XfpTags.Item)
                //{
                //    var index = parseItemIndex(currentLine);
                //    if (index >= 0 && index < GroupConfigData.NumSets)
                //    {
                //        var val = ParseString(currentLine);
                //        if (val == FileParsingXfp.XfpTags.UseInSpecialCAndE)
                //            result.GroupConfig.SetNames[index] = Cultures.Resources.Use_In_Special_C_And_E;
                //        else
                //            result.GroupConfig.SetNames[index] = ParseString(currentLine);
                //    }
                //}
            }
        }

    }
}
