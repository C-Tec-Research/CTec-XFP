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
using System.ComponentModel.Composition.Primitives;
using Xfp.DataTypes.PanelData;

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

                // Create a FlowDocument
                var systemName = Cultures.Resources.System_Name + ": " + (string.IsNullOrWhiteSpace(data.SiteConfig.SystemName) ? Cultures.Resources.Not_Set : data.SiteConfig.SystemName);

                var printArea  = new Size(printParams.PrintHandler.PrintableAreaWidth, printParams.PrintHandler.PrintableAreaHeight);
                var pageMargin = new Thickness(10);
                var pageNumber = 1;

                //FlowDocument doc = new FlowDocument(PrintUtil.DocumentHeader(Cultures.Resources.XFP_Config_Print_Description, systemName));
                //doc.Name        = _printFilePrefix;
                //doc.PageHeight  = printParams.PrintHandler.PrintableAreaHeight;
                //doc.PageWidth   = printParams.PrintHandler.PrintableAreaWidth;
                //doc.PagePadding = new Thickness(15);
                //doc.ColumnGap   = 0;
                //doc.ColumnWidth = printParams.PrintHandler.PrintableAreaWidth;

 //               List<List<Grid>> report = new();

                //if (printParams.PrintSiteConfig)
                //    data.SiteConfig.Print(doc);

                //foreach (var p in data.Panels.Values)
                //{
                //    //if (printParams.PrintSiteConfig) p.PanelConfig.Print(doc, p);
                //    if (printParams.PrintLoopInfo)          
                //    {
                //        //p.Loop1Config.Print(doc, data, p.PanelNumber, printParams.PrintAllLoopDevices, printParams.LoopPrintOrder);
                //        //p.Loop2Config.Print(doc, data, p.PanelNumber, printParams.PrintAllLoopDevices, printParams.LoopPrintOrder);
                //        report.Add(p.Loop1Config.Print2(data, p.PanelNumber, printParams.PrintAllLoopDevices, printParams.LoopPrintOrder));
                //    }
                //    //if (printParams.PrintZones)         p.ZoneConfig.Print(doc, p);
                //    //if (printParams.PrintGroups)        p.GroupConfig.Print(doc, p);
                //    //if (printParams.PrintSets)          p.SetConfig.Print(doc, p);
                //    //if (printParams.PrintCAndE)         p.CEConfig.Print(doc, p.PanelNumber, data);
                //    //if (printParams.PrintNetworkConfig) p.NetworkConfig.Print(doc, data, data.CurrentPanel.PanelNumber);
                //    //if (printParams.PrintEventLog)      printEventLog(doc);
                //    //if (printParams.PrintComments)      printComments(doc);
                //}

                //print or preview the document

                switch (printAction)
                {
                    case CTecUtil.PrintActions.Print:
                        //IDocumentPaginatorSource idpSource = doc;
                        //printParams.PrintHandler.PrintDocument(idpSource.DocumentPaginator, Cultures.Resources.XFP_Config_Print_Description);

                        ////DocumentPaginator paginator = ((IDocumentPaginatorSource)doc).DocumentPaginator;
                        ////paginator = new DocumentPaginatorWrapper(paginator, new Size(doc.PageHeight, doc.PageWidth), new Size(0, 20), Cultures.Resources.System_Name + ": " + systemName);
                        ////printParams.PrintHandler.PrintDocument(paginator, Cultures.Resources.XFP_Config_Print_Description);

                        for (int loop = 0; loop < LoopConfigData.MaxLoops; loop++)
                        {
                            var reportTitle = string.Format(Cultures.Resources.Loop_x_Devices, loop + 1);
                            var dataGrid = loop == 0 
                                            ? new Xfp.Printing.DeviceDetails(data, data.CurrentPanel.PanelNumber).Loop1DetailsDataGrid 
                                            : new Xfp.Printing.DeviceDetails(data, data.CurrentPanel.PanelNumber).Loop2DetailsDataGrid;

                            var report = new CustomDataGridDocumentPaginator(dataGrid, systemName, reportTitle, printArea, pageMargin, pageNumber);
                            printParams.PrintHandler.PrintDocument(report, Cultures.Resources.XFP_Config_Print_Description);
                break;
                        }

                        //foreach (var gg in report)
                        //{
                        //    var pag = new CustomDataGridDocumentPaginator(gg, "", new(210, 297), new(10));
                        //    printParams.PrintHandler.PrintDocument(pag, Cultures.Resources.XFP_Config_Print_Description);
                        //}
                        break;

                    case CTecUtil.PrintActions.Preview: 
                        //new FlowDocumentViewer(doc, Cultures.Resources.XFP_Config_Print_Description, XfpApplicationConfig.Settings, true).ShowDialog();
                        break;
                }

            }
            catch (Exception ex)
            {
                CTecMessageBox.ShowException(Cultures.Resources.Error_Printing_Document, CTecControls.Cultures.Resources.Print, ex);
            }
        }


        private static void printNetworkConfig(FlowDocument doc) { }        
        private static void printEventLog(FlowDocument doc) { }
        private static void printComments(FlowDocument doc) { }
    }
}
