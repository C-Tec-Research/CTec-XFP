using CTecControls.UI;
using CTecUtil.Printing;
using CTecUtil.UI;
using CTecUtil.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Xfp.Config;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;
using Xfp.UI.Views.PanelTools;

namespace Xfp.Printing
{
    public class XfpPrinting
    {
        static XfpPrinting()
        {
            PrintUtil.OnPrintPreview = PrintPreview;
        }

        public XfpPrinting()
        {
        }

        private static readonly string _printFilePrefix = "XfpConfig";

        public static void PrintConfig(XfpData data, XfpPrintParameters printParams, CTecUtil.PrintActions printAction)
        {
            if (printParams.PrintQueue == null)
            {
                CTecMessageBox.ShowOKError(Cultures.Resources.Printer_Not_Found, CTecControls.Cultures.Resources.Print);
                return;
            }

            try
            {
                UIState.SetBusyState();

                var printQueue = new PrintServer().GetPrintQueues().FirstOrDefault(p => p.Name == printParams.PrintQueue.Name);

                if (printQueue == null)
                {
                    CTecMessageBox.ShowOKError(Cultures.Resources.Printer_Not_Found, CTecControls.Cultures.Resources.Print);
                    return;
                }

                PrintUtil.PrintHandler.PrintQueue                  = printParams.Settings.PrintQueue;
                PrintUtil.PrintHandler.PrintTicket                 = printParams.Settings.PrintTicket;
                PrintUtil.PrintHandler.PrintTicket.PageOrientation = printParams.Settings.Orientation;
                PrintUtil.PrintHandler.PrintTicket.CopyCount       = printParams.Settings.Copies;

                var printArea  = new Size(printParams.PrintHandler.PrintableAreaWidth, printParams.PrintHandler.PrintableAreaHeight);
                var pageMargin = new Thickness(20);
                var pageNumber = 1;

                var noSysName = string.IsNullOrWhiteSpace(data.SiteConfig.SystemName);
                var sysName = noSysName ? Cultures.Resources.Print_Error_System_Name_Not_Set : data.SiteConfig.SystemName;

                // Create a FlowDocument
                FlowDocument doc = new FlowDocument(PrintUtil.DocumentHeader(Cultures.Resources.XFP_Config_Print_Description, sysName, noSysName));
                doc.Name = _printFilePrefix;
                doc.PageHeight = printParams.PrintHandler.PrintableAreaHeight;
                doc.PageWidth = printParams.PrintHandler.PrintableAreaWidth;
                doc.PagePadding = new Thickness(25);
                doc.ColumnGap = 0;
                doc.ColumnWidth = printParams.PrintHandler.PrintableAreaWidth;

                List<int> panelList = printParams.PrintAllPanels ? [..from k in data.Panels.Keys select k] : new() { data.CurrentPanel.PanelNumber };

                if (printParams.PrintSiteConfig)
                    data.SiteConfig.GetReport(doc);

                foreach (var i in panelList)
                {
                    var p = data.Panels[i];
                    
                    if (printParams.PrintSiteConfig)    p.PanelConfig.GetReport(doc, p);
                    if (printParams.PrintLoopInfo)      p.LoopConfig.GetReport(doc, p.PanelNumber, printParams.PrintLoop1, printParams.PrintLoop2, printParams.PrintAllLoopDevices, printParams.LoopPrintOrder);
                    if (printParams.PrintZones)         p.ZoneConfig.GetReport(doc, p);
                    if (printParams.PrintGroups)        p.GroupConfig.GetReport(doc, p);
                    if (printParams.PrintSets)          p.SetConfig.GetReport(doc, p);
                    if (printParams.PrintCAndE)         p.CEConfig.GetReport(doc, p.PanelNumber, data);
                    if (printParams.PrintNetworkConfig) p.NetworkConfig.GetReport(doc, data, data.CurrentPanel.PanelNumber);
                }

                if (printParams.PrintEventLog) EventLogData.GetReport(doc);
                if (printParams.PrintComments) CommentsData.GetReport(doc, data.Comments);

                PrintUtil.Print(doc, Cultures.Resources.XFP_Config_Print_Description, printParams.Settings, printAction);
            }
            catch (Exception ex)
            {
                CTecMessageBox.ShowException(Cultures.Resources.Error_Printing_Document, CTecControls.Cultures.Resources.Print, ex);
            }
        }


        private static void PrintPreview(FlowDocument document, string description, PrintParameters parameters)
            => new FlowDocumentViewer(document, description, XfpApplicationConfig.Settings, true, parameters).Show();
        

        //private static void printComments(FlowDocument doc, string comments)
        //{
        //    GridUtil.ResetDefaults();            
        //    PrintUtil.PageHeader(doc, Cultures.Resources.Nav_Comments);
        //    var commentsPage = new Section();
        //    var grid = new Grid() { MaxWidth = 400, HorizontalAlignment = HorizontalAlignment.Left };
        //    grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            
        //    if (!string.IsNullOrWhiteSpace(comments))
        //    {
        //        GridUtil.AddCellToGrid(grid, comments, 0, 0, false, false);
        //    }
        //    else
        //    {
        //        GridUtil.SetFontStyle(FontStyles.Italic);
        //        GridUtil.AddCellToGrid(grid, !string.IsNullOrWhiteSpace(comments) ? comments : Cultures.Resources.Comments_Is_Blank, 0, 0, false, false);
        //    }

        //    commentsPage.Blocks.Add(new BlockUIContainer(grid));
        //    doc.Blocks.Add(commentsPage);
        //    GridUtil.ResetDefaults();
        //}
    }
}
