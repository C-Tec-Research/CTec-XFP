using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;
using CTecUtil;
using CTecUtil.IO;
using Xfp.DataTypes;
using CTecControls.UI;

namespace Xfp.Files
{
    internal class XfpTextFile : TextFileBase
    {
        internal const string LegacyFileExt = ".xfp";
        internal const string XfpFileExt = ".xfp2";
        internal static readonly List<string> OpenFileExts = new() { XfpFileExt, LegacyFileExt };


        internal static new bool OpenFile() => OpenFile(GetFilterString(Cultures.Resources.XFP_Panel_Config_Files, OpenFileExts));


        internal static string SaveFile(XfpData data, int panelNumber)
        {
            if (data is null)
                return null;

            if (string.IsNullOrEmpty(FilePath))
                return SaveFileAs(data, panelNumber);
            
            var origPath = FilePath;
            FilePath = SetFileNameSuffix(FilePath, XfpFileExt);

            if (FilePath != origPath && CTecMessageBox.ShowYesNoWarn(string.Format(Cultures.Resources.File_x_Already_Exists, FilePath), Cultures.Resources.Save_File_Header) != MessageBoxResult.Yes)
                return origPath;

            CTecUtil.IO.TextFile.SaveFile(JsonConvert.SerializeObject(data, Formatting.Indented));
            CurrentFolder = Path.GetDirectoryName(FilePath);
            return FilePath;
        }


        internal static string SaveFileAs(XfpData data, int panelNumber)
        {
            if (data is null)
                return null;

            FilePath = SetFileNameSuffix(string.IsNullOrWhiteSpace(FilePath) ? Path.Combine(CurrentFolder ?? "", data.SiteConfig.SystemName.Trim()) : FilePath, XfpFileExt);
            Filter = GetFilterString(Cultures.Resources.XFP_Panel_Config_Files, XfpFileExt);
            return FilePath = CTecUtil.IO.TextFile.SaveFileAs(JsonConvert.SerializeObject(data, Formatting.Indented));
        }


        /// <summary>
        /// Deserialise json format text into XfpPanelData.
        /// </summary>
        internal static XfpData DeserializeJson(string input) => JsonConvert.DeserializeObject<XfpData>(input);


        /// <summary>
        /// Deserialise json format text into XfpPanelData.
        /// </summary>
        internal static XfpData_OldVersion_1 DeserializeVersion1Json(string input) => JsonConvert.DeserializeObject<XfpData_OldVersion_1>(input);

    }
}
