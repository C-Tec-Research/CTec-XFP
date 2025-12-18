using CTecUtil.Printing;
using CTecUtil.Utils;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Xfp.UI.Interfaces;

namespace Xfp.DataTypes.PanelData
{
    public partial class EventLogData
    {
        public static string LogText { get; set; }

        /// <summary>
        /// Path of the last-read comms log file, if any.
        /// </summary>
        public static string FilePath { get; set; }


        public static void Clear() => FilePath = LogText = null;


        public delegate void ViewerClearer();
        public static ViewerClearer ClearEventLogViewer;
    }
}
