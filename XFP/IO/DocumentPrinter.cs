using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CTecUtil.UI;
using CTecControls.UI;
using Xfp.ViewModels.PanelTools;
using Windows.Graphics.Printing;
using CTecUtil.Printing;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;

namespace Xfp.DataTypes.Printing
{
    public class DocumentPrinter
    {
        public DocumentPrinter()
        {
        }


        public static void PrintDocument(XfpData data, PrintingParameters printParams)
        {
            if (printParams.PrintQueue == null)
            {
                MessageBox.Show(Cultures.Resources.Printer_Not_Found);
                return;
            }

            PrintDialog ptr = printParams.PrintHandler;
            ptr.PrintQueue = printParams.PrintQueue;
            ptr.PrintTicket.CopyCount = printParams.Copies;
            ptr.PrintTicket.PageOrientation = printParams.Orientation;

            //ptr.PrintVisual(canvas);

            // Create a FlowDocument
            FlowDocument doc = new FlowDocument(new Paragraph(new Run("Hello, this is a test document for printing.")));

            // Print the document
            IDocumentPaginatorSource idpSource = doc;
            ptr.PrintDocument(idpSource.DocumentPaginator, "Printing FlowDocument");

            UIState.SetBusyState();

            try
            {
                var tempFilePath = PrintUtil.GetTempPrintFileName("test");
                //using (Package package = Package.Open(tempFilePath))
                {
                    var xpsDocument = new XpsDocument(tempFilePath, FileAccess.ReadWrite);
                    XpsDocumentWriter xpsdw = XpsDocument.CreateXpsDocumentWriter(xpsDocument);

                    if (printParams.PrintSiteConfig) printSite();
                    if (printParams.PrintLoopInfo)  printDevices();
                    if (printParams.PrintZones) printZones();
                    if (printParams.PrintGroups) printGroups();
                    if (printParams.PrintSets) printSets();
                    if (printParams.PrintCAndE) printCAndE();
                    if (printParams.PrintNetworkConfig) printNetworkConfig();
                    if (printParams.PrintEventLog) printEventLog();
                    if (printParams.PrintComments) printComments();
                }
            }
            catch (Exception ex)
            {
                CTecMessageBox.ShowException(Cultures.Resources.Error_Printing_Document, Cultures.Resources.Print, ex);
            }
        }


        private static void printSite() { }
        private static void printDevices() { }
        private static void printZones() { }
        private static void printGroups() { }
        private static void printSets() { }
        private static void printCAndE() { }
        private static void printNetworkConfig() { }
        private static void printEventLog() { }
        private static void printComments() { }
    }
}
