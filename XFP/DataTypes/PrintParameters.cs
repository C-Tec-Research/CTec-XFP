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
using CTecUtil;

namespace Xfp.DataTypes
{
    public class PrintParameters
    {
        public PrintParameters()
        {
            PrintHandler = new PrintDialog() { CurrentPageEnabled = false, SelectedPagesEnabled = false, UserPageRangeEnabled = false };
        }


        public PrintDialog PrintHandler;
        
        public PageOrientation Orientation { get => PrintHandler.PrintTicket.PageOrientation??PageOrientation.Portrait; set => PrintHandler.PrintTicket.PageOrientation = value; }
        public int             Copies      { get => PrintHandler.PrintTicket.CopyCount??1;                              set => PrintHandler.PrintTicket.CopyCount = value; }
        public PrintQueue      PrintQueue        => PrintHandler.PrintQueue;
        public PrintTicket     PrintTicket       => PrintHandler.PrintTicket;


        public void SetPrinter(string name) => PrintHandler.PrintQueue = new LocalPrintServer().GetPrintQueue(name);


        public bool PrintAllPanels     { get; set; }
        public bool PrintSelectedPanels{ get; set; }
        public string PrintPanelRange  { get; set; }
        public bool PrintAllPages      { get; set; }
        public bool PrintCurrentPage   { get; set; } = true;
        public bool PrintSiteConfig    { get; set; }
        public bool PrintLoopInfo      { get; set; }
        public bool PrintZones         { get; set; }
        public bool PrintGroups        { get; set; }
        public bool PrintSets          { get; set; }
        public bool PrintNetworkConfig { get; set; }
        public bool PrintCAndE         { get; set; }
        public bool PrintComments      { get; set; }
        public bool PrintEventLog      { get; set; }
        public bool SelectPagesToPrint { get; set; }
        public bool PrintOrderDevice   { get; set; }
        public bool PrintOrderGroup    { get; set; }
        public bool PrintOrderZone     { get; set; }

        public bool PrintAllLoopDevices { get; set; } = false;
        public SortOrder LoopPrintOrder { get; set; } = SortOrder.Number;


        public void SetAllPagesToPrint(bool print) => PrintSiteConfig = PrintLoopInfo = PrintZones = PrintGroups = PrintSets = PrintNetworkConfig = PrintCAndE = PrintComments = PrintEventLog = print;
    }
}
