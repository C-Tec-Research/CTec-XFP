using System;
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

namespace Xfp.ViewModels
{
    public class PrintDialogWindowViewModel : ViewModelBase
    {
        public PrintDialogWindowViewModel(ApplicationConfig applicationConfig, ObservableCollection<Page> pages, Page currentPage)
        {
            ApplicationConfig = applicationConfig;
            _pages            = pages;
            _currentPage      = currentPage;
            ZoomLevel         = ApplicationConfig.PrintParametersWindow.Scale;
        }


        public void ShowDialog()
        {
            OnPropertyChanged(nameof(Printers));
            OnPropertyChanged(nameof(SelectedPrinter));
            OnPropertyChanged(nameof(PrintAllPages));
            OnPropertyChanged(nameof(CanPrint));
            refreshPrinterStatus();

            if (PrintCurrentPage)
                setCurrentPageToPrint();
        }


        private ObservableCollection<Page> _pages = new();
        private Page _currentPage;

        public ApplicationConfig ApplicationConfig { get; }


        public void UpdateWindowParams(Window window, bool save = false) => ApplicationConfig.UpdatePrintParametersWindowParams(window, LayoutTransform.ScaleX, save);

        public void Close(Window window) => UpdateWindowParams(window, true);



        public void RefreshView()
        {
            //OnPropertyChanged(nameof(CurrentFileName));
        }


        #region printer
        private PrinterSettings _printerSettings = new();
        private bool _printerListIsOpen;
        private bool _printIsOpen;

        public List<string> Printers
        {
            get
            {
                var result = new List<string>();
                foreach (var p in PrinterSettings.InstalledPrinters)
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
        
        
        #region print parameters
        public PrintDialogSettings PrintSettings { get; private set; } = new();

        private bool _printAllPages = true;
        private bool _printCurrentPage;
        private bool _selectPagesToPrint;
        private bool _printOrderDevice = true;
        private bool _printOrderGroup;
        private bool _printOrderZone;
        private bool _canPrint;

        public bool PrintSiteConfig     { get => PrintSettings.PrintSiteConfig;    set { PrintSettings.PrintSiteConfig = value; OnPropertyChanged(); } }
        public bool PrintLoopInfo       { get => PrintSettings.PrintLoopInfo;      set { PrintSettings.PrintLoopInfo = value; OnPropertyChanged(); } }
        public bool PrintZones          { get => PrintSettings.PrintZones;         set { PrintSettings.PrintZones = value; OnPropertyChanged(); } }
        public bool PrintGroup          { get => PrintSettings.PrintGroups;        set { PrintSettings.PrintGroups = value; OnPropertyChanged(); } }
        public bool PrintSets           { get => PrintSettings.PrintSets;          set { PrintSettings.PrintSets = value; OnPropertyChanged(); } }
        public bool PrintNetworkConfig  { get => PrintSettings.PrintNetworkConfig; set { PrintSettings.PrintNetworkConfig = value; OnPropertyChanged(); } }
        public bool PrintCAndE          { get => PrintSettings.PrintCAndE;         set { PrintSettings.PrintCAndE = value; OnPropertyChanged(); } }
        public bool PrintComments       { get => PrintSettings.PrintComments;      set { PrintSettings.PrintComments = value; OnPropertyChanged(); } }
        public bool PrintEventLog       { get => PrintSettings.PrintEventLog;      set { PrintSettings.PrintEventLog = value; OnPropertyChanged(); } }

        public bool PrintAllPages       { get => _printAllPages;                    set { if (_printAllPages = value) setAllPagesToPrint(true); OnPropertyChanged(); } }
        public bool PrintCurrentPage    { get => _printCurrentPage;                 set { if (_printCurrentPage = value) setCurrentPageToPrint(); OnPropertyChanged(); } }
        public bool SelectPagesToPrint  { get => _selectPagesToPrint;               set { _selectPagesToPrint = value; OnPropertyChanged(); } }
        public int  NumPrintCopies      { get => PrintSettings.NumCopies;           set { PrintSettings.NumCopies = value; OnPropertyChanged(); } }
        public bool PrintAllLoopDevices { get => PrintSettings.PrintAllLoopDevices; set { PrintSettings.PrintAllLoopDevices = value; OnPropertyChanged(); } }
        public LoopPrintOrder LoopPrintOrder { get => PrintSettings.LoopPrintOrder; set { PrintSettings.LoopPrintOrder = value; OnPropertyChanged(); OnPropertyChanged(nameof(PrintOrderDevice)); OnPropertyChanged(nameof(PrintOrderGroup)); OnPropertyChanged(nameof(PrintOrderZone)); } }
        public bool PrintOrderDevice    { get => LoopPrintOrder == LoopPrintOrder.ByDevice; set { LoopPrintOrder = LoopPrintOrder.ByDevice; OnPropertyChanged(); OnPropertyChanged(nameof(PrintOrderGroup)); OnPropertyChanged(nameof(PrintOrderZone)); } }
        public bool PrintOrderGroup     { get => LoopPrintOrder == LoopPrintOrder.ByGroup;  set { LoopPrintOrder = LoopPrintOrder.ByGroup; OnPropertyChanged(); OnPropertyChanged(nameof(PrintOrderDevice)); OnPropertyChanged(nameof(PrintOrderZone)); } }
        public bool PrintOrderZone      { get => LoopPrintOrder == LoopPrintOrder.ByZone;   set { LoopPrintOrder = LoopPrintOrder.ByZone; OnPropertyChanged(); OnPropertyChanged(nameof(PrintOrderDevice)); OnPropertyChanged(nameof(PrintOrderGroup)); } }


        public bool CanPrint => SelectedPrinter is not null && (PrintAllPages || PrintSiteConfig || PrintLoopInfo || PrintZones || PrintGroup || PrintSets || PrintNetworkConfig || PrintCAndE || PrintComments || PrintEventLog);

        private void setAllPagesToPrint(bool print)
        {
            PrintSiteConfig = PrintLoopInfo = PrintZones = PrintGroup = PrintSets = PrintNetworkConfig = PrintCAndE = PrintComments = PrintEventLog = print;
            OnPropertyChanged(nameof(CanPrint));
        }

        private void setCurrentPageToPrint()
        {
            setAllPagesToPrint(false);

            if (_currentPage.DataContext is DevicesViewModel)
                PrintSettings.PrintLoopInfo = true;
            else if (_currentPage.DataContext is ZoneConfigViewModel)
                PrintSettings.PrintZones = true;
            else if (_currentPage.DataContext is GroupConfigViewModel)
                PrintSettings.PrintGroups = true;
            else if (_currentPage.DataContext is SetConfigViewModel)
                PrintSettings.PrintSets = true;
            else if (_currentPage.DataContext is SiteConfigViewModel)
                PrintSettings.PrintSiteConfig = true;
            else if (_currentPage.DataContext is CausesAndEffectsViewModel)
                PrintSettings.PrintCAndE = true;
            else if (_currentPage.DataContext is NetworkConfigViewModel)
                PrintSettings.PrintNetworkConfig = true;
            else if (_currentPage.DataContext is EventLogViewerViewModel)
                PrintSettings.PrintEventLog = true;
            else if (_currentPage.DataContext is CommentsViewModel)
                PrintSettings.PrintComments = true;

            OnPropertyChanged(nameof(PrintSiteConfig));
            OnPropertyChanged(nameof(PrintLoopInfo));
            OnPropertyChanged(nameof(PrintZones));
            OnPropertyChanged(nameof(PrintGroup));
            OnPropertyChanged(nameof(PrintSets));
            OnPropertyChanged(nameof(PrintNetworkConfig));
            OnPropertyChanged(nameof(PrintCAndE));
            OnPropertyChanged(nameof(PrintComments));
            OnPropertyChanged(nameof(PrintEventLog));
            OnPropertyChanged(nameof(CanPrint));
        }
        #endregion


        #region Minimise, Maximise/Restore buttons
        private bool _windowIsMaximised;
        public bool WindowIsMaximised { get => _windowIsMaximised; set { _windowIsMaximised = value; OnPropertyChanged(); } }
        #endregion


        #region zoom
        private ScaleTransform _layoutTransform;
        public ScaleTransform LayoutTransform { get => _layoutTransform; set { _layoutTransform = value; OnPropertyChanged(); } }

        public double ZoomLevel
        {
            get => ApplicationConfig.PrintParametersWindow.Scale;
            set
            {
                ApplicationConfig.PrintParametersWindow.Scale = value;
                LayoutTransform = new ScaleTransform(value, value);
                OnPropertyChanged();
            }
        }

        public void ZoomIn() { ZoomLevel = (float)Math.Min(LayoutTransform.ScaleX + ApplicationConfig.ZoomStep, CTecUtil.Config.ApplicationConfig.MaxZoom); }
        public void ZoomOut() { ZoomLevel = (float)Math.Max(LayoutTransform.ScaleX - ApplicationConfig.ZoomStep, CTecUtil.Config.ApplicationConfig.MinZoom); }
        #endregion
    }
}
