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
using CTecUtil.ViewModels;
using CTecUtil.IO;
using CTecUtil.Config;
using CTecControls.UI;
using CTecUtil;
using System.Drawing.Printing;
using System.Printing;
using Xfp.DataTypes;
using Xfp.ViewModels.PanelTools;
using System.Windows.Documents;
using System.Reflection;
using System.Security.Permissions;
using Windows.Graphics.Printing;
using CTecControls.Util;
using System.Globalization;
using System.Diagnostics.Eventing.Reader;
using System.Windows.Input;

namespace Xfp.ViewModels
{
    public class PrintDialogWindowViewModel : ViewModelBase
    {
        public PrintDialogWindowViewModel(ApplicationConfig applicationConfig, ObservableCollection<Page> pages, Page currentPage, int panelCount, Button printButton, Button previewButton)
        {
            _applicationConfig = applicationConfig;
            _pages       = pages;
            _currentPage = currentPage;
            _panelCount  = panelCount;

            ZoomLevel = _applicationConfig.PrintParametersWindow.Scale;
            
            var printer = _applicationConfig.LastPrinter;
            SelectedPrinter = !string.IsNullOrEmpty(printer) ? printer : new LocalPrintServer().DefaultPrintQueue.Name;
            PrintSelectedPanel = true;

            setCurrentPageToPrint();

            refreshPrinterStatus();
        }


        public PrintQueue Queue => new PrintServer().GetPrintQueues().FirstOrDefault(p => p.Name == SelectedPrinter);


        private ObservableCollection<Page> _pages = new();
        private Page _currentPage;
        private int _panelCount;

        private ApplicationConfig _applicationConfig { get; }

        private HotKeyList _hotKeys = new();
        internal HotKeyList HotKeys { get => _hotKeys; set => _hotKeys = value; }


        public void UpdateWindowParams() => _applicationConfig.UpdatePrintParametersWindowParams(LayoutTransform.ScaleX);

        public void Close(Window window) => UpdateWindowParams();


        #region printer
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

        public List<string> _printerDevices;


        public bool   PrinterListIsOpen { get => _printerListIsOpen;          set { _printerListIsOpen = value; OnPropertyChanged(); } }
        public string SelectedPrinter   { get => PrintParams.PrintQueue.Name; set { PrintParams.SetPrinter(value); OnPropertyChanged(); PrinterListIsOpen = false; } }

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
        public static PrintParameters PrintParams { get; set; } = new();

        public int  PanelCount                => _panelCount;
        public bool PrintAllPanels      { get => PrintParams.PrintAllPages;       set { PrintParams.PrintAllPages = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }
        public bool PrintSelectedPanel  { get => !PrintParams.PrintAllPanels;     set { PrintParams.PrintAllPanels = !value; OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }
        public bool PrintAllPages       { get => PrintParams.PrintAllPages;       set { if (PrintParams.PrintAllPages = value) PrintParams.SetAllPagesToPrint(true); OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }
        public bool PrintCurrentPage    { get => PrintParams.PrintAllPages;       set { if (!(PrintParams.PrintAllPanels = !value)) setCurrentPageToPrint(); OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }
        public bool SelectPagesToPrint  { get => PrintParams.SelectPagesToPrint;  set { PrintParams.SelectPagesToPrint = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }
        public bool PrintSiteConfig     { get => PrintParams.PrintSiteConfig;     set { PrintParams.PrintSiteConfig = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }
        public bool PrintLoopInfo       { get => PrintParams.PrintLoopInfo;       set { PrintParams.PrintLoopInfo = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }
        public bool PrintZones          { get => PrintParams.PrintZones;          set { PrintParams.PrintZones = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }
        public bool PrintGroups         { get => PrintParams.PrintGroups;         set { PrintParams.PrintGroups = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }
        public bool PrintSets           { get => PrintParams.PrintSets;           set { PrintParams.PrintSets = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }
        public bool PrintNetworkConfig  { get => PrintParams.PrintNetworkConfig;  set { PrintParams.PrintNetworkConfig = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }
        public bool PrintCAndE          { get => PrintParams.PrintCAndE;          set { PrintParams.PrintCAndE = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }
        public bool PrintComments       { get => PrintParams.PrintComments;       set { PrintParams.PrintComments = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }
        public bool PrintEventLog       { get => PrintParams.PrintEventLog;       set { PrintParams.PrintEventLog = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanPrint)); } }

        public bool PrintAllLoopDevices { get => PrintParams.PrintAllLoopDevices; set { PrintParams.PrintAllLoopDevices = value; OnPropertyChanged(); } }
        public bool PrintOrderDeviceNumber { get => PrintParams.LoopPrintOrder == SortOrder.Number; set { PrintParams.LoopPrintOrder = SortOrder.Number; OnPropertyChanged(); OnPropertyChanged(nameof(PrintOrderDeviceType)); OnPropertyChanged(nameof(PrintOrderGroupZone)); } }
        public bool PrintOrderDeviceType   { get => PrintParams.LoopPrintOrder == SortOrder.Type;  set { PrintParams.LoopPrintOrder = SortOrder.Type; OnPropertyChanged(); OnPropertyChanged(nameof(PrintOrderDeviceNumber)); OnPropertyChanged(nameof(PrintOrderGroupZone)); } }
        public bool PrintOrderGroupZone    { get => PrintParams.LoopPrintOrder == SortOrder.ZoneGroupSet;   set { PrintParams.LoopPrintOrder = SortOrder.ZoneGroupSet; OnPropertyChanged(); OnPropertyChanged(nameof(PrintOrderDeviceNumber)); OnPropertyChanged(nameof(PrintOrderDeviceType)); } }


        public bool CanPrint =>  SelectedPrinter is not null //&& (PrintAllPanels/* || CTecUtil.TextProcessing.NumberListToString PrintPanelRange.Length > 0 ||*/)
                                                             && (PrintAllPages || PrintSiteConfig    || PrintLoopInfo || PrintZones    || PrintGroups ||
                                                                 PrintSets     || PrintNetworkConfig || PrintCAndE    || PrintComments || PrintEventLog);

        //private void setAllPagesToPrint(bool value) => PrintSiteConfig = PrintLoopInfo      = PrintZones = PrintGroups   = PrintSets 
        //                                             = PrintSiteConfig = PrintNetworkConfig = PrintCAndE = PrintComments = PrintEventLog = value;

        private void setCurrentPageToPrint()
        {
            //PrintParams.SetAllPagesToPrint(false);
            PrintSiteConfig = PrintLoopInfo = PrintZones = PrintGroups = PrintSets = PrintNetworkConfig = PrintCAndE = PrintComments = PrintEventLog = false;

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


        //internal delegate void OptionSelectedAction(CTecUtil.PrintActions action);
        //internal OptionSelectedAction CloseAction;


        public bool CheckHotKey(KeyEventArgs e)
        {
            var alt = (e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;

            var keyStr = TextProcessing.KeyToString(e);

            HotKeyList.KeyProperties hotKey = new(keyStr, false, false, alt);

            if (_hotKeys.TryGetValue(hotKey, out Action command))
            {
                command?.Invoke();
                return true;
            }

            return false;
        }


        #region zoom
        private ScaleTransform _layoutTransform;
        public ScaleTransform LayoutTransform { get => _layoutTransform; set { _layoutTransform = value; OnPropertyChanged(); } }

        public double ZoomLevel
        {
            get => _applicationConfig.PrintParametersWindow.Scale;
            set
            {
                _applicationConfig.PrintParametersWindow.Scale = value;
                LayoutTransform = new ScaleTransform(value, value);
                OnPropertyChanged();
            }
        }

        public void ZoomIn()  { ZoomLevel = (float)Math.Min(LayoutTransform.ScaleX + ApplicationConfig.ZoomStep, ApplicationConfig.MaxZoom); }
        public void ZoomOut() { ZoomLevel = (float)Math.Max(LayoutTransform.ScaleX - ApplicationConfig.ZoomStep, ApplicationConfig.MinZoom); }
        #endregion
    }
}
