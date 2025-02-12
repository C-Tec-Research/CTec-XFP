using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using CTecUtil.UI;
using CTecControls.UI;
using CTecControls.ViewModels;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;
using Xfp.ViewModels.PanelTools;
using Windows.Graphics.Printing;

namespace Xfp.DataTypes
{
    public class PrintParameters
    {
        public PrintParameters()
        {
            PrintHandler = new PrintDialog() { CurrentPageEnabled = false, SelectedPagesEnabled = false, UserPageRangeEnabled = false };
        }


        public PrintDialog PrintHandler;
        //public CTecUtil.PrinterSettings PrinterSettings { get; set; } = new();
        
        public PageOrientation Orientation { get => PrintHandler.PrintTicket.PageOrientation??PageOrientation.Portrait; set => PrintHandler.PrintTicket.PageOrientation = value; }
        public int             Copies      { get => PrintHandler.PrintTicket.CopyCount??1; set => PrintHandler.PrintTicket.CopyCount = value; }
        public PrintQueue      PrintQueue  { get => PrintHandler.PrintQueue; }
        public PrintTicket     PrintTicket { get => PrintHandler.PrintTicket; }


        public bool PrintSiteConfig    { get; set; }
        public bool PrintLoopInfo      { get; set; }
        public bool PrintZones         { get; set; }
        public bool PrintGroups        { get; set; }
        public bool PrintSets          { get; set; }
        public bool PrintNetworkConfig { get; set; }
        public bool PrintCAndE         { get; set; }
        public bool PrintComments      { get; set; }
        public bool PrintEventLog      { get; set; }
        public bool PrintAllPages      { get; set; }
        public bool PrintCurrentPage   { get; set; }
        public bool SelectPagesToPrint { get; set; }
        public bool PrintOrderDevice   { get; set; }
        public bool PrintOrderGroup    { get; set; }
        public bool PrintOrderZone     { get; set; }

        public bool           PrintAllLoopDevices { get; set; }
        public LoopPrintOrder LoopPrintOrder { get; set; }


        public void SetAllPagesToPrint(bool print)
        {
            PrintSiteConfig = PrintLoopInfo = PrintZones = PrintGroups = PrintSets = PrintNetworkConfig = PrintCAndE = PrintComments = PrintEventLog = print;
        }
    }
}
