using CTecControls.UI;
using CTecUtil.Printing;
using CTecUtil.UI;
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

                // Create a FlowDocument
                //var systemName = Cultures.Resources.XFP_Config_Print_Description + " - " + Cultures.Resources.System_Name + ": " + (string.IsNullOrWhiteSpace(data.SiteConfig.SystemName) ? Cultures.Resources.Not_Set : data.SiteConfig.SystemName);

                var printArea  = new Size(printParams.PrintHandler.PrintableAreaWidth, printParams.PrintHandler.PrintableAreaHeight);
                var pageMargin = new Thickness(20);
                var pageNumber = 1;
                double reportHeaderHeight;

                var noSysName = string.IsNullOrWhiteSpace(data.SiteConfig.SystemName);
                var sysName = noSysName ? Cultures.Resources.Print_Error_System_Name_Not_Set : data.SiteConfig.SystemName;
                FlowDocument doc = new FlowDocument(PrintUtil.DocumentHeader(Cultures.Resources.XFP_Config_Print_Description, sysName, noSysName, out reportHeaderHeight));
                doc.Name = _printFilePrefix;
                doc.PageHeight = printParams.PrintHandler.PrintableAreaHeight;
                doc.PageWidth = printParams.PrintHandler.PrintableAreaWidth;
                doc.PagePadding = new Thickness(15);
                doc.ColumnGap = 0;
                doc.ColumnWidth = printParams.PrintHandler.PrintableAreaWidth;

                //var pdfDoc = new PdfSharp.Pdf.PdfDocument();
                //pdfDoc.Info.Title   = Cultures.Resources.XFP_Config_Print_Description;
                //pdfDoc.Info.Subject = sysName;


                //List<List<Grid>> report = new();

                List<int> panelList = printParams.PrintAllPanels ? [..(from k in data.Panels.Keys select k)] : new() { data.CurrentPanel.PanelNumber };

                //if (printParams.PrintSiteConfig)
                //{
                //    data.SiteConfig.GetReport(doc);

                //    foreach (var i in panelList)
                //    {
                //        var p = data.Panels[i];
                //        p.PanelConfig.GetReport(doc, p, ref pageNumber);
                //    }
                //}

                foreach (var i in panelList)
                {
                    var p = data.Panels[i];
            
                    if (printParams.PrintLoopInfo)
                    {
                        //p.Loop1Config.PrintPdf(pdfDoc, data, p.PanelNumber, printParams.PrintAllLoopDevices, printParams.LoopPrintOrder, ref pageNumber);
                        p.Loop2Config.GetReport(doc, data, p.PanelNumber, printParams.PrintAllLoopDevices, printParams.LoopPrintOrder, printAction);
                    }
                    //if (printParams.PrintZones)         p.ZoneConfig.GetReport(doc, p, ref pageNumber);
                    //if (printParams.PrintGroups)        p.GroupConfig.GetReport(doc, p, ref pageNumber);
                    //if (printParams.PrintSets)          p.SetConfig.GetReport(doc, p, ref pageNumber);
                    //if (printParams.PrintCAndE)         p.CEConfig.GetReport(doc, p.PanelNumber, data, ref pageNumber);
                    //if (printParams.PrintNetworkConfig) p.NetworkConfig.GetReport(doc, data, data.CurrentPanel.PanelNumber, ref pageNumber);
                }

                if (printParams.PrintComments) printComments(doc, ref pageNumber);

                PrintUtil.Print(doc, Cultures.Resources.XFP_Config_Print_Description, printParams.Settings, printAction, reportHeaderHeight);
            }
            catch (Exception ex)
            {
                CTecMessageBox.ShowException(Cultures.Resources.Error_Printing_Document, CTecControls.Cultures.Resources.Print, ex);
            }
        }


        private static void PrintPreview(FlowDocument document, string description, PrintParameters parameters)
            => new FlowDocumentViewer(document, description, XfpApplicationConfig.Settings, true, parameters).Show();
        

        private static void printComments(FlowDocument doc, ref int pageNumber)
        {
            //if (pageNumber++ > 1)
            //    PrintUtil.InsertPageBreak(doc);
        }
    }
}
