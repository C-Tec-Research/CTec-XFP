﻿using System;
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


        public static void PrintDocument(XfpData data, PrintParameters printParams)
        {
            if (printParams.PrintQueue == null)
            {
                MessageBox.Show(Cultures.Resources.Printer_Not_Found);
                return;
            }

            //use PrintDialog (not displayed) to get printer properties
            var ptr = printParams.PrintHandler;
            ptr.PrintQueue = printParams.PrintQueue;
            ptr.PrintTicket.CopyCount = printParams.Copies;
            ptr.PrintTicket.PageOrientation = printParams.Orientation;

            //ptr.PrintVisual(canvas);

            // Create a FlowDocument
            FlowDocument doc = new FlowDocument(new Paragraph(new Run("Hello, this is a test document for printing.")));
            
            doc.PageHeight = ptr.PrintableAreaHeight;
            doc.PageWidth = ptr.PrintableAreaWidth;
            doc.PagePadding = new Thickness(50);
            doc.ColumnGap = 0;
            doc.ColumnWidth = ptr.PrintableAreaWidth;


            UIState.SetBusyState();

            try
            {
                var tempFilePath = PrintUtil.GetTempPrintFileName("test");

                var xpsDocument = new XpsDocument(tempFilePath, FileAccess.ReadWrite);
                XpsDocumentWriter xpsdw = XpsDocument.CreateXpsDocumentWriter(xpsDocument);

                if (printParams.PrintSiteConfig)    printSite(xpsdw);
                if (printParams.PrintLoopInfo)      printDevices(xpsdw);
                if (printParams.PrintZones)         printZones(xpsdw);
                if (printParams.PrintGroups)        printGroups(xpsdw);
                if (printParams.PrintSets)          printSets(xpsdw);
                if (printParams.PrintCAndE)         printCAndE(xpsdw);
                if (printParams.PrintNetworkConfig) printNetworkConfig(xpsdw);
                if (printParams.PrintEventLog)      printEventLog(xpsdw);
                if (printParams.PrintComments)      printComments(xpsdw);

                // Print the document
                IDocumentPaginatorSource idpSource = doc;
                //ptr.PrintDocument(idpSource.DocumentPaginator, "Printing FlowDocument");
            }
            catch (Exception ex)
            {
                CTecMessageBox.ShowException(Cultures.Resources.Error_Printing_Document, Cultures.Resources.Print, ex);
            }
        }


        private static void printSite(XpsDocumentWriter writer) { }
        private static void printDevices(XpsDocumentWriter writer) { }
        private static void printZones(XpsDocumentWriter writer) { }
        private static void printGroups(XpsDocumentWriter writer) { }
        private static void printSets(XpsDocumentWriter writer) { }
        private static void printCAndE(XpsDocumentWriter writer) { }
        private static void printNetworkConfig(XpsDocumentWriter writer) { }
        private static void printEventLog(XpsDocumentWriter writer) { }
        private static void printComments(XpsDocumentWriter writer) { }
    }
}
