using CTecUtil.IO;
using System;

namespace Xfp.Files
{
    internal class TextFileBase : TextFile
    {
        protected static string CurrentFolder { get; set; }
        

        internal static new bool OpenFile() => throw new NotImplementedException("OpenFile()");

        internal static bool OpenFile(string filter)
        {
            FilePath = CurrentFolder;
            Filter = filter;
            return TextFile.OpenFile();
        }
    }
}
