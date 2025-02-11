//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Drawing.Printing;
//using System.Linq;
//using System.Printing;
//using System.Text;
//using System.Threading.Tasks;
//using System.Timers;
//using System.Windows;
//using System.Windows.Controls;
//using CTecUtil.UI;
//using CTecControls.UI;
//using CTecControls.ViewModels;
//using Xfp.DataTypes;
//using Xfp.DataTypes.PanelData;
//using Xfp.ViewModels.PanelTools;

//namespace Xfp.DataTypes.Printing
//{
//    public class PrintDialogSettings
//    {
//        public PrintDialogSettings()
//        {
//            _printDialog = new PrintDialog() { CurrentPageEnabled = false, SelectedPagesEnabled = false, UserPageRangeEnabled = false };
//        }


//        PrintDialog _printDialog;
//        public PrinterSettings PrinterSettings { get; set; } = new();

//        public bool PrintSiteConfig { get; set; }
//        public bool PrintLoopInfo { get; set; }
//        public bool PrintZones { get; set; }
//        public bool PrintGroups { get; set; }
//        public bool PrintSets { get; set; }
//        public bool PrintNetworkConfig { get; set; }
//        public bool PrintCAndE { get; set; }
//        public bool PrintComments { get; set; }
//        public bool PrintEventLog { get; set; }

//        public bool PrintAllLoopDevices { get; set; }
//        public LoopPrintOrder LoopPrintOrder { get; set; }

//        public int NumCopies { get; set; } = 1;

//        public PrintQueue PrintQueue => _printDialog.PrintQueue;
//        public PrintTicket PrintTicket => _printDialog.PrintTicket;


//        public void SetAllPagesToPrint(bool print)
//        {
//            PrintSiteConfig = PrintLoopInfo = PrintZones = PrintGroups = PrintSets = PrintNetworkConfig = PrintCAndE = PrintComments = PrintEventLog = print;
//        }
//    }
//}
