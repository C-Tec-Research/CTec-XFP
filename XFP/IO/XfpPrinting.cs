using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;
using CTecUtil.Printing;
using CTecUtil.UI;
using CTecControls.UI;
using System.Windows.Forms;
using Xfp.Config;

namespace Xfp.DataTypes.Printing
{
    public class XfpPrinting
    {
        public XfpPrinting()
        {
        }


        private static readonly string _printFilePrefix = "XfpConfig";

        public static void PrintConfig(XfpData data, PrintParameters printParams, CTecUtil.PrintActions printAction)
        {
            if (printParams.PrintQueue == null)
            {
                CTecMessageBox.ShowOKError(Cultures.Resources.Printer_Not_Found, Cultures.Resources.Option_Print);
                return;
            }
            
            try
            {
                UIState.SetBusyState();
            
                var printQueue = new PrintServer().GetPrintQueues().FirstOrDefault(p => p.Name == printParams.PrintQueue.Name);

                if (printQueue == null)
                {
                    CTecMessageBox.ShowOKError(Cultures.Resources.Printer_Not_Found, Cultures.Resources.Option_Print);
                    return;
                }

                // Create a FlowDocument
                FlowDocument doc = new FlowDocument(PrintUtil.PageHeader(Cultures.Resources.XFP_Config_Print_Description));
                doc.Name        = _printFilePrefix;
                doc.PageHeight  = printParams.PrintHandler.PrintableAreaHeight;
                doc.PageWidth   = printParams.PrintHandler.PrintableAreaWidth;
                doc.PagePadding = new Thickness(50);
                doc.ColumnGap   = 0;
                doc.ColumnWidth = printParams.PrintHandler.PrintableAreaWidth;

                //if (printParams.PrintSiteConfig)
                //    data.SiteConfig.Print(doc);

                foreach (var p in data.Panels.Values)
                {
                    doc.Blocks.Add(PrintUtil.DocumentHeader(string.Format(Cultures.Resources.Panel_x, p.PanelNumber)));

                    if (printParams.PrintLoopInfo)          
                    {
                        p.Loop1Config.Print(doc, p.PanelNumber, printParams.PrintAllLoopDevices);
                        p.Loop2Config.Print(doc, p.PanelNumber, printParams.PrintAllLoopDevices);
                    }
                    if (printParams.PrintZones)         p.ZoneConfig.Print(doc, p);
                    if (printParams.PrintGroups)        p.GroupConfig.Print(doc, p);
                    if (printParams.PrintSets)          printSets(doc);
                    if (printParams.PrintCAndE)         printCAndE(doc);
                    if (printParams.PrintNetworkConfig) printNetworkConfig(doc);
                    if (printParams.PrintEventLog)      printEventLog(doc);
                    if (printParams.PrintComments)      printComments(doc);
                }

                //print or preview the document

                switch (printAction)
                {
                    case CTecUtil.PrintActions.Print: 
                        IDocumentPaginatorSource idpSource = doc;
                        printParams.PrintHandler.PrintDocument(idpSource.DocumentPaginator, Cultures.Resources.XFP_Config_Print_Description);
                        break;

                    case CTecUtil.PrintActions.Preview: 
                        new FlowDocumentViewer(doc, Cultures.Resources.XFP_Config_Print_Description, XfpApplicationConfig.Settings, true).ShowDialog();
                        break;
                }

            }
            catch (Exception ex)
            {
                CTecMessageBox.ShowException(Cultures.Resources.Error_Printing_Document, Cultures.Resources.Option_Print, ex);
            }
        }


        //private static void printSite(FlowDocument doc)
        //{
        //    var sitePage = new Section();
        //    sitePage.Blocks.Add(PageHeader(Cultures.Resources.Nav_Site_Configuration));

        //    doc.Blocks.Add(sitePage);
        //}
        
        private static void printGroups(FlowDocument doc)
        {
            var groupsPage = new Section();
            groupsPage.Blocks.Add(PrintUtil.PageHeader(Cultures.Resources.Nav_Group_Configuration));

            doc.Blocks.Add(groupsPage);
        }
        
        private static void printSets(FlowDocument doc)
        {
            var setsPage = new Section();
            setsPage.Blocks.Add(PrintUtil.PageHeader(Cultures.Resources.Nav_C_And_E_Configuration));

            doc.Blocks.Add(setsPage);
        }
        
        private static void printCAndE(FlowDocument doc)
        {
            var cePage = new Section();
            cePage.Blocks.Add(PrintUtil.PageHeader(Cultures.Resources.Nav_C_And_E_Configuration));

            doc.Blocks.Add(cePage);
        }
        
        private static void printNetworkConfig(FlowDocument doc)
        {
            var networkPage = new Section();
            networkPage.Blocks.Add(PrintUtil.PageHeader(Cultures.Resources.Nav_Network_Configuration));

            doc.Blocks.Add(networkPage);
        }
        
        private static void printEventLog(FlowDocument doc)
        {
            var eventLogPage = new Section();
            eventLogPage.Blocks.Add(PrintUtil.PageHeader(Cultures.Resources.Nav_Event_Log));

            doc.Blocks.Add(eventLogPage);
        }
        
        private static void printComments(FlowDocument doc)
        {
            var commentsPage = new Section();
            commentsPage.Blocks.Add(PrintUtil.PageHeader(Cultures.Resources.Nav_Comments));

            doc.Blocks.Add(commentsPage);
        }


        //internal static Paragraph PageHeader(string header)
        //{
        //    var para = new Paragraph();
        //    var bold = new Bold();
        //    bold.Inlines.Add(new Run(header));
        //    para.Inlines.Add(bold);
        //    return para;
        //}
    }
}
