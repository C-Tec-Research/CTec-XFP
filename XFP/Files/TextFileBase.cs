using System;
using CTecUtil.Config;
using CTecUtil.IO;
using Xfp.Config;

namespace Xfp.Files
{
    internal class TextFileBase : TextFile
    {
        protected static string CurrentFolder { get; set; }
        

        internal static bool OpenFile() => throw new NotImplementedException("OpenFile()");

        
        /// <summary>
        /// Open a file using an OpenFileDialog and set FilePath to the selected file.
        /// </summary>
        /// <param name="appFolderInfo">The last folder info from the parent app's ApplicationConfig instance; used to retrieve the folder last used for opening or saving a file of this type</param>
        /// <returns>True if a file was selected, False if the dialog was cancelled.</returns>
        internal static bool OpenFile(string filter, AppLastFolderInfo lastFolder)
        {
            FilePath = CurrentFolder;
            Filter = filter;
            return TextFile.OpenFile(lastFolder);
        }
    }
}
