using CTecControls.UI;
using CTecUtil;
using CTecUtil.Config;
using CTecUtil.Printing;
using CTecUtil.Utils;
using CTecUtil.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management;
using System.Printing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xfp.DataTypes;
using Xfp.UI.Views.PanelTools;
using Xfp.ViewModels.PanelTools;

namespace Xfp.ViewModels
{
    public class PrintDialogWindowViewModel : ViewModelBase
    {
        public PrintDialogWindowViewModel(ApplicationConfig applicationConfig, ObservableCollection<Page> pages, Page currentPage, XfpData data, Grid outerGrid)
        {
            _applicationConfig = applicationConfig;
            _pages       = pages;
            _currentPage = currentPage;
            _outerGrid   = outerGrid;
            _panelCount  = data.Panels.Count;

            if (currentPage.DataContext is PanelToolsPageViewModelBase panelVm)
                _currentPanelLoopCount = data.Panels[panelVm.PanelNumber].LoopConfig.NumLoops;
            _panelNums.AddRange(from p in data.Panels select p.Value.PanelNumber);
            _panelLoops.AddRange(from p in data.Panels select p.Value.LoopConfig.NumLoops);

            var printer = _applicationConfig.LastPrinter;
            SelectedPrinter = !string.IsNullOrEmpty(printer) ? printer : new LocalPrintServer().DefaultPrintQueue.Name;
            PrintSelectedPanel = true;

            setCurrentPageToPrint();
            refreshPrinterStatus();
            RefreshView();
        }


        public PrintQueue Queue => new PrintServer().GetPrintQueues().FirstOrDefault(p => p.Name == SelectedPrinter);


        private ObservableCollection<Page> _pages = new();
        private Page _currentPage;
        private int  _currentPanelLoopCount;
        private int  _panelCount;
        private List<int> _panelNums = new List<int>();
        private List<int> _panelLoops = new List<int>();

        private Grid _outerGrid;

        private ApplicationConfig _applicationConfig { get; }

        private HotKeyList _hotKeys = new();
        internal HotKeyList HotKeys { get => _hotKeys; set => _hotKeys = value; }


        //public void UpdateWindowParams() => _applicationConfig.UpdatePrintParametersWindowParams(LayoutTransform.ScaleX);

        //public void Close(Window window) => UpdateWindowParams();


        #region printer
        private bool _printerListIsOpen;
        private bool _printIsOpen;

        private object _printerListLock = new();

        public List<string> Printers
        {
            get
            {
                List<string> result = new ();

                lock (_printerListLock)
                {
                    string defaultPrinter = "";

                    try
                    {
                        //get default printer name and put it first in the list
                        result.Add(defaultPrinter = PrintUtil.DefaultPrinterName);
                    }
                    catch { }

                    foreach (var p in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                        if (p is string printer && printer.ToLower() != defaultPrinter.ToLower())
                            result.Add(printer);
                }

                return result;
            }
        }


        //private static string getDefaultPrinterName()
        //{
        //    var query = new ObjectQuery("SELECT * FROM Win32_Printer");
        //    var searcher = new ManagementObjectSearcher(query);

        //    foreach (ManagementObject mo in searcher.Get())
        //    {
        //        if (((bool?) mo["Default"]) ?? false)
        //        {
        //            return mo["Name"] as string;
        //        }
        //    }

        //    return null;
        //}


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
        public static XfpPrintParameters PrintParams { get; set; } = new();

        public int  PanelCount                => _panelCount;
        public bool PrintAllPanels      { get => PrintParams.PrintAllPanels;      set { PrintParams.PrintAllPanels = value; RefreshView(); } }
        public bool PrintSelectedPanel  { get => !PrintParams.PrintAllPanels;     set { PrintParams.PrintAllPanels = !value; RefreshView(); } }
        public bool PrintAllPages       { get => PrintParams.PrintAllPages;       set { if (PrintParams.PrintAllPages = value) SetAllPagesToPrint(true); } }
        public bool PrintCurrentPage    { get => !PrintParams.PrintAllPages && !PrintParams.SelectPagesToPrint; set { if (value) { PrintParams.PrintAllPages = PrintParams.SelectPagesToPrint = false; setCurrentPageToPrint(); } } }
        public bool SelectPagesToPrint  { get => PrintParams.SelectPagesToPrint;  set { if (PrintParams.SelectPagesToPrint = value) PrintAllPages = false; RefreshView(); } }
        public bool PrintSiteConfig     { get => PrintParams.PrintSiteConfig;     set { PrintParams.PrintSiteConfig = value; RefreshView(); } }
        public bool PrintLoopInfo       { get => PrintParams.PrintLoopInfo && (PrintLoop1 || PrintLoop2); set { PrintParams.PrintLoopInfo = PrintLoop1 = value; if (!value) PrintLoop2 = false; RefreshView(); } }
        public bool PrintLoop1          { get => PrintParams.PrintLoop1;          set { PrintParams.PrintLoop1 = value; RefreshView(); } }
        public bool PrintLoop2          { get => PrintParams.PrintLoop2;          set { PrintParams.PrintLoop2 = value; RefreshView(); } }
        public bool PrintZones          { get => PrintParams.PrintZones;          set { PrintParams.PrintZones = value; RefreshView(); } }
        public bool PrintGroups         { get => PrintParams.PrintGroups;         set { PrintParams.PrintGroups = value; RefreshView(); } }
        public bool PrintSets           { get => PrintParams.PrintSets;           set { PrintParams.PrintSets = value; RefreshView(); } }
        public bool PrintNetworkConfig  { get => PrintParams.PrintNetworkConfig;  set { PrintParams.PrintNetworkConfig = value; RefreshView(); } }
        public bool PrintCAndE          { get => PrintParams.PrintCAndE;          set { PrintParams.PrintCAndE = value; RefreshView(); } }
        public bool PrintComments       { get => PrintParams.PrintComments;       set { PrintParams.PrintComments = value; RefreshView(); } }
        public bool PrintEventLog       { get => PrintParams.PrintEventLog;       set { PrintParams.PrintEventLog = value; RefreshView(); } }

        public bool PrintAllLoopDevices    { get => PrintParams.PrintAllLoopDevices; set { PrintParams.PrintAllLoopDevices = value; RefreshView(); } }
        public bool PrintOrderDeviceNumber { get => PrintParams.LoopPrintOrder == SortOrder.Number;       set { PrintParams.LoopPrintOrder = SortOrder.Number; RefreshView(); } }
        public bool PrintOrderDeviceType   { get => PrintParams.LoopPrintOrder == SortOrder.Type;         set { PrintParams.LoopPrintOrder = SortOrder.Type; RefreshView(); } }
        public bool PrintOrderGroupZone    { get => PrintParams.LoopPrintOrder == SortOrder.ZoneGroupSet; set { PrintParams.LoopPrintOrder = SortOrder.ZoneGroupSet; RefreshView(); } }

        public bool LoopSelectionAvailable => PrintParams.SelectPagesToPrint && PrintParams.PrintLoopInfo && (PrintLoop1 || PrintLoop2);
        public bool Loop1Available         => PrintParams.PrintLoopInfo;
        public bool Loop2Available         => PrintParams.PrintLoopInfo && (PrintAllPanels || _currentPanelLoopCount > 1);
        public int MaxLoops
        {
            get
            {
                int result = 1;
                foreach (var loopCount in from loopCount in _panelLoops where loopCount > result select loopCount) { result = loopCount; }
                return result;
            }
        }
        

        public bool CanPrint =>  SelectedPrinter is not null //&& (PrintAllPanels/* || CTecUtil.TextProcessing.NumberListToString PrintPanelRange.Length > 0 ||*/)
                                                             && (PrintAllPages      || PrintSiteConfig 
                                                              || PrintLoopInfo && (PrintLoop1 || PrintLoop2) 
                                                              || PrintZones         || PrintGroups     || PrintSets
                                                              || PrintNetworkConfig || PrintCAndE      || PrintComments || PrintEventLog);

        //private void setAllPagesToPrint(bool value) => PrintSiteConfig = PrintLoopInfo      = PrintZones = PrintGroups   = PrintSets 
        //                                             = PrintSiteConfig = PrintNetworkConfig = PrintCAndE = PrintComments = PrintEventLog = value;

        private void SetAllPagesToPrint(bool print)
        {
            PrintSiteConfig = PrintLoopInfo = PrintZones = PrintGroups = PrintSets = PrintNetworkConfig = PrintCAndE = PrintComments = PrintEventLog = print;
            PrintLoop1 = print;
            PrintLoop2 = print && MaxLoops > 1;
        }

        private void setCurrentPageToPrint()
        {
            //PrintParams.SetAllPagesToPrint(false);
            PrintSiteConfig = PrintLoopInfo = PrintZones = PrintGroups = PrintSets = PrintNetworkConfig = PrintCAndE = PrintComments = PrintEventLog = false;

            if (_currentPage.DataContext is DevicesViewModel)
            {
                PrintLoopInfo = true;

                if (_currentPage.DataContext is DevicesViewModel vm)
                {
                    PrintLoop1 = vm.IsLoop1;
                    PrintLoop2 = !vm.IsLoop1;
                }
            }
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

        public void RefreshView()
        {
            OnPropertyChanged(nameof(CanPrint));
            OnPropertyChanged(nameof(PrintAllPanels));
            OnPropertyChanged(nameof(PrintSelectedPanel));
            OnPropertyChanged(nameof(PrintAllPages));
            OnPropertyChanged(nameof(PrintCurrentPage));
            OnPropertyChanged(nameof(SelectPagesToPrint));
            OnPropertyChanged(nameof(PrintSiteConfig));
            OnPropertyChanged(nameof(PrintLoopInfo));
            OnPropertyChanged(nameof(PrintLoop1));
            OnPropertyChanged(nameof(PrintLoop2));
            OnPropertyChanged(nameof(PrintZones));
            OnPropertyChanged(nameof(PrintGroups));
            OnPropertyChanged(nameof(PrintSets));
            OnPropertyChanged(nameof(PrintNetworkConfig));
            OnPropertyChanged(nameof(PrintCAndE));
            OnPropertyChanged(nameof(PrintComments));
            OnPropertyChanged(nameof(PrintEventLog));
            OnPropertyChanged(nameof(PrintAllLoopDevices));
            OnPropertyChanged(nameof(PrintOrderDeviceNumber));
            OnPropertyChanged(nameof(PrintOrderDeviceType));
            OnPropertyChanged(nameof(PrintOrderGroupZone));
            OnPropertyChanged(nameof(LoopSelectionAvailable));
            OnPropertyChanged(nameof(Loop1Available));
            OnPropertyChanged(nameof(Loop2Available));
            OnPropertyChanged(nameof(MaxLoops));
        }


        //internal delegate void OptionSelectedAction(CTecUtil.PrintActions action);
        //internal OptionSelectedAction CloseAction;


        public bool CheckHotKey(KeyEventArgs e)
        {
            var alt = (e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;

            var keyStr = TextUtil.KeyToString(e);

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

        //public double WindowWidth => _outerGrid.ActualWidth * ZoomLevel;

        //public double ZoomLevel
        //{
        //    get => _applicationConfig.PrintParametersWindow.Scale;
        //    set
        //    {
        //        //_applicationConfig.PrintParametersWindow.Scale = value;
        //        //LayoutTransform = new ScaleTransform(value, value);
        //        //OnPropertyChanged();
        //        //OnPropertyChanged(nameof(WindowWidth));
        //    }
        //}

        //public void ZoomIn()  { /*ZoomLevel = (float)Math.Min(LayoutTransform.ScaleX + ApplicationConfig.ZoomStep, ApplicationConfig.MaxZoom);*/ }
        //public void ZoomOut() { /*ZoomLevel = (float)Math.Max(LayoutTransform.ScaleX - ApplicationConfig.ZoomStep, ApplicationConfig.MinZoom);*/ }
        #endregion
    }
}
