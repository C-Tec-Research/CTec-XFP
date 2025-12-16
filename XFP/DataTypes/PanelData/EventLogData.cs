using CTecUtil.Printing;
using CTecUtil.Utils;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Xfp.UI.Interfaces;

namespace Xfp.DataTypes.PanelData
{
    public partial class EventLogData : ConfigData, IConfigData
    {
        public string LogText { get; set; }

        /// <summary>
        /// Path of the last-read comms log file, if any.
        /// </summary>
        public string FilePath { get; set; }
    }
}
