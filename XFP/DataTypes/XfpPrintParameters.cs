using System.Printing;
using System.Windows.Controls;
using CTecUtil;
using CTecUtil.Printing;
using Xfp.Config;

namespace Xfp.DataTypes
{
    public class XfpPrintParameters : PrintParameters
    {
        public XfpPrintParameters()
        {
        }


        public override void SetPrinter(string name) => base.SetPrinter(XfpApplicationConfig.Settings.LastPrinter = name);


        public bool PrintAllPanels     { get; set; }
        //public bool PrintSelectedPanel { get; set; }
        //public string PrintPanelRange  { get; set; }
        public bool PrintAllPages      { get; set; }
        //public bool PrintCurrentPage   { get; set; } = true;
        public bool PrintSiteConfig    { get; set; }
        public bool PrintLoopInfo      { get; set; }
        public bool PrintLoop1         { get; set; }
        public bool PrintLoop2         { get; set; }
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


        //public void SetAllPagesToPrint(bool print) => PrintSiteConfig = PrintLoopInfo = PrintZones = PrintGroups = PrintSets = PrintNetworkConfig = PrintCAndE = PrintComments = PrintEventLog = print;
    }
}
