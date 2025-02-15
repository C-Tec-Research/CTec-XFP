﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CTecControls.ViewModels;
using CTecUtil.IO;
using CTecUtil.Config;
using CTecControls.UI;
using CTecUtil;
using System.Drawing.Printing;
using System.Printing;
using Xfp.DataTypes;
using Xfp.DataTypes.Printing;
using Xfp.ViewModels.PanelTools;
using System.Windows.Documents;
using System.Reflection;
using System.Security.Permissions;
using Windows.Graphics.Printing;

namespace Xfp.ViewModels
{
    public class PrintDialogWindowViewModel : ViewModelBase
    {
        public PrintDialogWindowViewModel(ApplicationConfig applicationConfig, ObservableCollection<Page> pages, Page currentPage)
        {
            this.applicationConfig = applicationConfig;
            _pages       = pages;
            _currentPage = currentPage;

            ZoomLevel    = this.applicationConfig.PrintParametersWindow.Scale;

            IsPortrait          = true;
            PrintCurrentPage    = true;
            PrintAllLoopDevices = true;
            PrintOrderDevice    = true;

            if (_currentPage.DataContext is DevicesViewModel)
                setCurrentPageToPrint();

            refreshPrinterStatus();
        }


        //public void ShowDialog()
        //{
        //    OnPropertyChanged(nameof(Printers));
        //    OnPropertyChanged(nameof(SelectedPrinter));
        //    OnPropertyChanged(nameof(PrintAllPages));
        //    OnPropertyChanged(nameof(CanPrint));
        //    refreshPrinterStatus();

        //    if (PrintCurrentPage)
        //        setCurrentPageToPrint();
        //}


        public PrintQueue Queue => new PrintServer().GetPrintQueues().FirstOrDefault(p => p.Name == SelectedPrinter);


        //public void Print()
        //{
        //    var printQueue = new PrintServer().GetPrintQueues().FirstOrDefault(p => p.Name == SelectedPrinter);
        //}
        //    if (printQueue == null)  
        //    {  
        //        MessageBox.Show(Cultures.Resources.Printer_Not_Found);
        //        return;  
        //    }

            //    PrintDialog printDialog = new PrintDialog();
            //    printDialog.PrintQueue = printQueue;
            //    printDialog.PrintTicket.CopyCount = NumPrintCopies;
            //    printDialog.PrintTicket.PageOrientation =  PageOrientation.Landscape;

            //    //printDialog.PrintVisual(canvas);

            //    // Create a FlowDocument
            //    FlowDocument doc = new FlowDocument(new Paragraph(new Run("Hello, this is a test document for printing.")));

            //    // Print the document
            //    IDocumentPaginatorSource idpSource = doc;
            //    printDialog.PrintDocument(idpSource.DocumentPaginator, "Printing FlowDocument");
            //}


        private ObservableCollection<Page> _pages = new();
        private Page _currentPage;

        private ApplicationConfig applicationConfig { get; }


        public void UpdateWindowParams(bool save = false) => applicationConfig.UpdatePrintParametersWindowParams(LayoutTransform.ScaleX, save);

        public void Close(Window window) { UpdateWindowParams(true); }


        #region printer
        private System.Drawing.Printing.PrinterSettings _printerSettings = new();
        private bool _printerListIsOpen;
        private bool _printIsOpen;

        public List<string> Printers
        {
            get
            {
                var result = new List<string>();
                foreach (var p in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                    result.Add(p.ToString());
                return result;
            }
        }

        public bool   PrinterListIsOpen { get => _printerListIsOpen; set { _printerListIsOpen = value; OnPropertyChanged(); } }
        public string SelectedPrinter   { get => _printerSettings.PrinterName; set { _printerSettings.PrinterName = value; OnPropertyChanged(); PrinterListIsOpen = false; } }

        public PrintQueueStatus PrinterStatus => new LocalPrintServer().GetPrintQueue(SelectedPrinter).QueueStatus;

        private async void refreshPrinterStatus()
        {
            OnPropertyChanged(nameof(PrinterStatus));
            await Task.Delay(2000);
            refreshPrinterStatus();
        }
        #endregion


        #region printer settings
        //public CTecUtil.PrinterSettings PrinterSettings { get; set; } = new();
        public bool IsPortrait
        {
            get => PrintParams.Orientation == PageOrientation.Portrait; 
            set => PrintParams.Orientation = value ? PageOrientation.Portrait : PageOrientation.Landscape; 
        }

        public int NumCopies { get => PrintParams.Copies; set { PrintParams.Copies = value; OnPropertyChanged(); } }
        #endregion


        #region print parameters
        public PrintParameters PrintParams { get; set; } = new();

        public bool PrintAllPages       { get => PrintParams.PrintAllPages;      set { if (PrintParams.PrintAllPages = value) PrintParams.SetAllPagesToPrint(true); OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }
        public bool PrintCurrentPage    { get => PrintParams.PrintCurrentPage;   set { if (PrintParams.PrintCurrentPage = value) setCurrentPageToPrint(); OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }
        public bool SelectPagesToPrint  { get => PrintParams.SelectPagesToPrint; set { PrintParams.SelectPagesToPrint = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }
        public bool PrintSiteConfig     { get => PrintParams.PrintSiteConfig;    set { PrintParams.PrintSiteConfig = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }
        public bool PrintLoopInfo       { get => PrintParams.PrintLoopInfo;      set { PrintParams.PrintLoopInfo = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }
        public bool PrintZones          { get => PrintParams.PrintZones;         set { PrintParams.PrintZones = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }
        public bool PrintGroups         { get => PrintParams.PrintGroups;        set { PrintParams.PrintGroups = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }
        public bool PrintSets           { get => PrintParams.PrintSets;          set { PrintParams.PrintSets = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }
        public bool PrintNetworkConfig  { get => PrintParams.PrintNetworkConfig; set { PrintParams.PrintNetworkConfig = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }
        public bool PrintCAndE          { get => PrintParams.PrintCAndE;         set { PrintParams.PrintCAndE = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }
        public bool PrintComments       { get => PrintParams.PrintComments;      set { PrintParams.PrintComments = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }
        public bool PrintEventLog       { get => PrintParams.PrintEventLog;      set { PrintParams.PrintEventLog = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }

        public bool PrintAllLoopDevices { get => PrintParams.PrintAllLoopDevices; set { PrintParams.PrintAllLoopDevices = value; OnPropertyChanged(); } }
        public bool PrintOrderDevice    { get => PrintParams.LoopPrintOrder == LoopPrintOrder.ByDevice; set { PrintParams.LoopPrintOrder = LoopPrintOrder.ByDevice; OnPropertyChanged(); OnPropertyChanged(nameof(PrintOrderGroup)); OnPropertyChanged(nameof(PrintOrderZone)); } }
        public bool PrintOrderGroup     { get => PrintParams.LoopPrintOrder == LoopPrintOrder.ByGroup;  set { PrintParams.LoopPrintOrder = LoopPrintOrder.ByGroup; OnPropertyChanged(); OnPropertyChanged(nameof(PrintOrderDevice)); OnPropertyChanged(nameof(PrintOrderZone)); } }
        public bool PrintOrderZone      { get => PrintParams.LoopPrintOrder == LoopPrintOrder.ByZone;   set { PrintParams.LoopPrintOrder = LoopPrintOrder.ByZone; OnPropertyChanged(); OnPropertyChanged(nameof(PrintOrderDevice)); OnPropertyChanged(nameof(PrintOrderGroup)); } }


        public bool CanPrint =>  SelectedPrinter is not null && (PrintAllPages || PrintSiteConfig    || PrintLoopInfo || PrintZones    || PrintGroups ||
                                                                 PrintSets     || PrintNetworkConfig || PrintCAndE    || PrintComments || PrintEventLog);

        //private void setAllPagesToPrint(bool value) => PrintSiteConfig = PrintLoopInfo      = PrintZones = PrintGroups   = PrintSets 
        //                                             = PrintSiteConfig = PrintNetworkConfig = PrintCAndE = PrintComments = PrintEventLog = value;

        private void setCurrentPageToPrint()
        {
            PrintParams.SetAllPagesToPrint(false);

            if      (_currentPage.DataContext is DevicesViewModel)          PrintLoopInfo = true;
            else if (_currentPage.DataContext is ZoneConfigViewModel)       PrintZones = true;
            else if (_currentPage.DataContext is GroupConfigViewModel)      PrintGroups = true;
            else if (_currentPage.DataContext is SetConfigViewModel)        PrintSets = true;
            else if (_currentPage.DataContext is SiteConfigViewModel)       PrintSiteConfig = true;
            else if (_currentPage.DataContext is CausesAndEffectsViewModel) PrintCAndE = true;
            else if (_currentPage.DataContext is NetworkConfigViewModel)    PrintNetworkConfig = true;
            else if (_currentPage.DataContext is EventLogViewerViewModel)   PrintEventLog = true;
            else if (_currentPage.DataContext is CommentsViewModel)         PrintComments = true;
        }
        #endregion


        //#region Minimise, Maximise/Restore buttons
        //private bool _windowIsMaximised;
        //public bool WindowIsMaximised { get => _windowIsMaximised; set { _windowIsMaximised = value; OnPropertyChanged(); } }
        //#endregion


        #region zoom
        private ScaleTransform _layoutTransform;
        public ScaleTransform LayoutTransform { get => _layoutTransform; set { _layoutTransform = value; OnPropertyChanged(); } }

        public double ZoomLevel
        {
            get => applicationConfig.PrintParametersWindow.Scale;
            set
            {
                applicationConfig.PrintParametersWindow.Scale = value;
                LayoutTransform = new ScaleTransform(value, value);
                OnPropertyChanged();
            }
        }

        public void ZoomIn()  { ZoomLevel = (float)Math.Min(LayoutTransform.ScaleX + ApplicationConfig.ZoomStep, ApplicationConfig.MaxZoom); }
        public void ZoomOut() { ZoomLevel = (float)Math.Max(LayoutTransform.ScaleX - ApplicationConfig.ZoomStep, ApplicationConfig.MinZoom); }
        #endregion
    }
}
