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
using Xfp.DataTypes;
using System.ComponentModel.Composition.Primitives;
using Xfp.DataTypes.PanelData;
using CTecUtil.ViewModels;
using Xfp.UI.ViewHelpers;

namespace Xfp.Printing
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
                //var systemName = Cultures.Resources.XFP_Config_Print_Description + " - " + Cultures.Resources.System_Name + ": " + (string.IsNullOrWhiteSpace(data.SiteConfig.SystemName) ? Cultures.Resources.Not_Set : data.SiteConfig.SystemName);

                var printArea  = new Size(printParams.PrintHandler.PrintableAreaWidth, printParams.PrintHandler.PrintableAreaHeight);
                var pageMargin = new Thickness(20);
                var pageNumber = 1;

                var noSysName = string.IsNullOrWhiteSpace(data.SiteConfig.SystemName);
                var sysName = noSysName ? Cultures.Resources.Print_Error_System_Name_Not_Set : data.SiteConfig.SystemName;
                FlowDocument doc = new FlowDocument(PrintUtil.DocumentHeader(Cultures.Resources.XFP_Config_Print_Description, sysName, noSysName));
                doc.Name = _printFilePrefix;
                doc.PageHeight = printParams.PrintHandler.PrintableAreaHeight;
                doc.PageWidth = printParams.PrintHandler.PrintableAreaWidth;
                doc.PagePadding = new Thickness(15);
                doc.ColumnGap = 0;
                doc.ColumnWidth = printParams.PrintHandler.PrintableAreaWidth;

                List<List<Grid>> report = new();

                List<int> panelList = printParams.PrintAllPanels ? [..(from k in data.Panels.Keys select k)] : new() { data.CurrentPanel.PanelNumber };

                if (printParams.PrintSiteConfig)
                {
                    data.SiteConfig.Print(doc);

                    foreach (var i in panelList)
                    {
                        var p = data.Panels[i];
                        p.PanelConfig.Print(doc, p, ref pageNumber);
                    }
                }

                foreach (var i in panelList)
                {
                    var p = data.Panels[i];
            
                    if (printParams.PrintLoopInfo)
                    {
                        p.Loop1Config.Print(doc, data, p.PanelNumber, printParams.PrintAllLoopDevices, printParams.LoopPrintOrder, ref pageNumber);
                        p.Loop2Config.Print(doc, data, p.PanelNumber, printParams.PrintAllLoopDevices, printParams.LoopPrintOrder, ref pageNumber);
                    }
                    if (printParams.PrintZones)         p.ZoneConfig.Print(doc, p, ref pageNumber);
                    if (printParams.PrintGroups)        p.GroupConfig.Print(doc, p, ref pageNumber);
                    if (printParams.PrintSets)          p.SetConfig.Print(doc, p, ref pageNumber);
                    if (printParams.PrintCAndE)         p.CEConfig.Print(doc, p.PanelNumber, data, ref pageNumber);
                    if (printParams.PrintNetworkConfig) p.NetworkConfig.Print(doc, data, data.CurrentPanel.PanelNumber, ref pageNumber);
                }

                if (printParams.PrintEventLog) printEventLog(doc, ref pageNumber);
                if (printParams.PrintComments) printComments(doc, ref pageNumber);

                //print or preview the document
                switch (printAction)
                {
                    case CTecUtil.PrintActions.Print:
                        IDocumentPaginatorSource idpSource = doc;
                        printParams.PrintHandler.PrintDocument(idpSource.DocumentPaginator, Cultures.Resources.XFP_Config_Print_Description);
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


        private static void printNetworkConfig(FlowDocument doc, ref int pageNumber)
        {
            if (pageNumber++ > 1)
                PrintUtil.InsertPageBreak(doc);
        }
        
        private static void printEventLog(FlowDocument doc, ref int pageNumber)
        {
            if (pageNumber++ > 1)
                PrintUtil.InsertPageBreak(doc);
        }

        private static void printComments(FlowDocument doc, ref int pageNumber)
        {
            if (pageNumber++ > 1)
                PrintUtil.InsertPageBreak(doc);
        }
    }
}
