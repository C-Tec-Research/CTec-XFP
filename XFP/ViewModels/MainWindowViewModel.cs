using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Newtonsoft.Json;
using CTecUtil;
using CTecUtil.IO;
using static CTecUtil.IO.SerialComms;
using CTecUtil.Pipes;
using static CTecUtil.Pipes.PipeServer;
using CTecUtil.StandardPanelDataTypes;
using CTecUtil.UI;
using CTecControls;
using CTecControls.UI;
using CTecControls.UI.Interfaces;
using CTecControls.Util;
using CTecControls.ViewModels;
using CTecDevices.Protocol;
using Xfp.Cultures;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;
using Xfp.Files;
using Xfp.IO;
using Xfp.UI.Interfaces;
using Xfp.UI.Views;
using Xfp.UI.Views.PanelTools;
using Xfp.ViewModels.PanelTools;
using Xfp.Config;
using Xfp.Printing;
using CTecUtil.UI.Util;

namespace Xfp.ViewModels
{
    public class MainWindowViewModel : PanelToolsPageViewModelBase, IPanelToolsViewModel, IDisposable
    {
        public MainWindowViewModel(Window window, HamburgerMenu hamb, NumberSpinner panelControl) : base(window)
        {
            _mainAppWindow = window;
            _mainMenu = hamb;
            _panelControl = panelControl;

            XfpApplicationConfig.Settings.InitConfigSettings(SupportedApps.XFP, updateFileMenuRecentFiles);

            //ShowRegistrationWindow(true);

            UIState.SetBusyState();

            XfpCommands.Initialise(new(() => PanelNumber));
            PanelComms.InitialisePingCommands();
            PanelComms.PollingMode = SerialComms.PingModes.NoPing;

            DeviceTypes.CurrentProtocolType = CTecDevices.ObjectTypes.XfpCast;

            CTecUtil.Cultures.CultureResources.InitSupportedCultures(Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));

            PanelNumber = XfpData.MinPanelNumber;

            _pages = new()
            {
                (_siteConfigPage = new SiteConfig()),
                (_deviceDetailsPage = new DeviceDetails()),
                (_loop1Page = new DeviceOverview(1)),
                (_loop2Page = new DeviceOverview(2)),
                (_zonesPage = new ZoneConfig()),
                (_groupsPage = new GroupConfig()),
                (_setsPage = new SetsConfig()),
                (_ceConfigPage = new CausesAndEffects()),
                (_networkConfigPage = new NetworkConfig()),
                (_eventLogPage = new EventLogViewer()),
                (_commentsPage = new Comments()),
            };

            foreach (var p in _pages)
                (p.DataContext as PanelToolsPageViewModelBase)?.SetChangesAreAllowedChecker(CheckChangesAreAllowed);

            (_siteConfigPage.DataContext as SiteConfigViewModel).SystemNameChanged = new((name) => SystemName = name);

            ReadApplicationCfg();
            SetDeviceSelectorMenus();
            InitNewDataset();

            NavBarSelectedIndex = 0;

            //init panel comms delegates
            SerialComms.NotifyFirmwareVersion = new((firmwareResponse) => validateFirmwareVersion(firmwareResponse));

            SerialComms.NotifyDeviceProtocol = new((command) =>
            {
                var loopType = LoopConfigData.LoopTypeBundle.Parse(command);
                _panelProtocol = loopType.Protocol;
                LoopConfigData.DetectedLoops = loopType.NumLoops;
            });

            SerialComms.NotifyConnectionStatus = new((status) =>
            {
                PanelIsReadOnly = (CommsStatus = status) == SerialComms.ConnectionStatus.ConnectedReadOnly;
                FirmwareError = CommsStatus == SerialComms.ConnectionStatus.FirmwareNotSupported;
                OnPropertyChanged(nameof(PanelIsConnected));
                OnPropertyChanged(nameof(PanelIsDisconnected));
            });

            _chChCheckChanges.Elapsed += (s, e) => OnPropertyChanged(nameof(DataHasChanged));

            startComPortWatcher();

            //start listener in case another instance of the app sends data
            startPipeServer();
        }


        private Window _mainAppWindow;


        #region start-up
        /// <summary>
        /// Read commandline args and treat as an XFP file path if present.<br/>
        /// NB: only to be called on start-up.
        /// </summary>
        internal void CheckCommandLineArgs()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
                FileOpen(args[1]);
        }


        private PipeServer _pipeServer;

        /// <summary>
        /// Start listener in case another instance of the app sends data
        /// </summary>
        private void startPipeServer()
        {
            if (_pipeServer?.ReceivePipeMessage is not null)
                _pipeServer.ReceivePipeMessage -= ReceivePipeMessage;
            _pipeServer = new PipeServer();
            _pipeServer.ReceivePipeMessage += new PipeMessageHandler(ReceivePipeMessage);
            _pipeServer.Listen(App.SingletonPipeName);
        }


        /// <summary>
        /// Handle incoming message from another instance of the app.<br/>
        /// This will occur if the user has double-clicked an XFP data file: the other instance will have sent the file path here then shut itself down.
        /// </summary>
        private void ReceivePipeMessage(string transferData)
        {
            //data is send in a json-format string
            if (JsonConvert.DeserializeObject<List<PipeTransferData>>(transferData) is List<PipeTransferData> data)
                foreach (var d in data)
                    if (d.DataType == PipeTransferData.DataTypes.Path)
                        Application.Current.Dispatcher.Invoke(new Action(() => FileOpen(d.Data)));
        }


        public void ReadApplicationCfg()
        {
            SerialComms.Settings = XfpApplicationConfig.Settings.SerialPortSettings;
            ZoomLevel = XfpApplicationConfig.Settings.MainWindow.Scale;
            var cultureName = XfpApplicationConfig.Settings.Culture;
            DeviceTypes.CurrentProtocolType = CTecDevices.Enums.StringToObjectType(XfpApplicationConfig.Settings.Protocol);
            SetCulture(cultureName is null ? CultureInfo.CurrentCulture : CultureInfo.GetCultureInfo(cultureName));
            updateFileMenuRecentFiles();
        }
        #endregion


        #region data set
        /// <summary>
        /// Initialise an empty dataset with confirmation on changes to current data.
        /// </summary>
        public void InitNewDataset() => InitNewDataset(CurrentProtocol, true);

        /// <summary>
        /// Initialise an empty dataset with optional confirmation on changes to current data.
        /// </summary>
        /// <param name="checkForChanges">Set to True to check the current data for changes and ask for confirmation</param>
        public void InitNewDataset(CTecDevices.ObjectTypes protocol, bool checkForChanges, bool repopulateView = true)
        {
            if (checkForChanges && DataHasChanged && !askClearCurrentData((_currentPage.DataContext as PanelToolsPageViewModelBase)?.PageHeader))
                return;

            CloseValidationWindow();

            CurrentProtocol = protocol;

            if (repopulateView)
                PopulateView(XfpData.InitialisedNew(CurrentProtocol, PanelNumber = PanelNumber, true));

            OnPropertyChanged(nameof(CurrentFileName));
            OnPropertyChanged(nameof(CurrentFilePath));
        }


        /// <summary>
        /// Returns <see langword="true"/> if any of the panel config settings have been changed
        /// </summary>
        public override bool DataHasChanged
        {
            get
            {
                if (_dataFromFile is not null)   return !_data.Equals((ConfigData)_dataFromFile);
                if (_dataFromDevice is not null) return !_data.Equals((ConfigData)_dataFromDevice);
                if (_savedData is not null)      return !_data.Equals((ConfigData)_savedData);
                return false;
            }
        }

        internal bool FirmwareVersionHasChanged => _dataFromFile is not null && _data.CurrentPanel.PanelConfig.FirmwareVersion == ((XfpData)_dataFromFile).CurrentPanel.PanelConfig.FirmwareVersion;
        internal bool FirmwareVersionisOlder => _dataFromFile is not null && FirmwareVersionHasChanged && _data.CurrentPanel.PanelConfig.FirmwareVersion == ((XfpData)_dataFromFile).CurrentPanel.PanelConfig.FirmwareVersion;

        internal bool ToolsVersionHasChanged => _dataFromFile is not null && _data.ToolsVersion == ((XfpData)_dataFromFile).ToolsVersion;


        /// <summary>
        /// Returns <see langword="true"/> if the specified panel is newly added or removed, or if its settings have been changed
        /// </summary>
        public bool PanelDataHasChanged(int panelNumber)
            => _savedData is XfpData sd && sd.Panels.TryGetValue(panelNumber, out XfpPanelData value) && _data.Panels.ContainsKey(panelNumber)
                ? value.Equals(value)
                : true;


        internal string GetUnsavedDataMessage()
        {
            var p = DataHasChanged;

            if (p)
                return Cultures.Resources.Message_Panel_Settings_Not_Saved;

            //there are changes in configurator settings, so get a list of active pages
            var pageList = new StringBuilder();

            if (p)
                pageList.Append("\t" + Cultures.Resources.Panel_Configuration + Environment.NewLine);

            if (pageList.Length > 0)
                return string.Format(Cultures.Resources.Message_Settings_x_Not_Saved, pageList.ToString().TrimEnd());

            return null;
        }

        public override void SaveCopyOfCurrentData() => _savedData = new XfpData(_data);
        public override void SaveCopyOfDataFromFile() { _dataFromFile = new XfpData(_data); _dataFromDevice = null; }
        public override void SaveCopyOfDataFromDevice() { _dataFromDevice = new XfpData(_data); }
        #endregion


        #region pages
        private ObservableCollection<Page> _pages = new();
        private Page _deviceDetailsPage;
        private Page _loop1Page;
        private Page _loop2Page;
        private Page _zonesPage;
        private Page _groupsPage;
        private Page _setsPage;
        private Page _siteConfigPage;
        private Page _ceConfigPage;
        private Page _networkConfigPage;
        private Page _eventLogPage;
        private Page _commentsPage;

        private Page _currentPage;


        /// <summary>Is the current page one of the Device view pages?</summary>
        public bool CurrentPageIsDevices => _currentPage == _deviceDetailsPage || _currentPage == _loop1Page || _currentPage == _loop2Page;
        public bool CurrentPageIsZones => _currentPage == _zonesPage;


        /// <summary>List of pages to download - comprises the current page plus any others required</summary>
        private List<Page> currentPageDownloadList
        {
            get => CurrentPageIsDevices ? new() { _deviceDetailsPage, /*_zonesPage, _siteConfigPage*/ }
                 : CurrentPageIsZones ? new() { _zonesPage, _networkConfigPage, }
                                        : new() { _currentPage };
        }
        #endregion


        #region navigaton menu
        public void SetDeviceSelectorMenus()
        {
            foreach (var p in _pages)
                (p.DataContext as IXfpDevicesViewModel)?.InitDeviceSelector();
        }


        private ObservableCollection<NavBarMenuItemViewModel> _navBarItems;
        public ObservableCollection<NavBarMenuItemViewModel> NavBarItems
        {
            get
            {
                if (_navBarItems is null)
                {
                    _navBarItems = new()
                    {
                        new NavBarMenuItemViewModel { Page = _deviceDetailsPage,  MenuIcon = UI.Bitmaps.GetBitmap("nav-device-details") },
                        new NavBarMenuItemViewModel { Page = _loop1Page,          MenuIcon = UI.Bitmaps.GetBitmap("nav-device-summary") },
                        new NavBarMenuItemViewModel { Page = _loop2Page,          MenuIcon = UI.Bitmaps.GetBitmap("nav-device-summary"), },
                        new NavBarMenuItemViewModel { Page = _zonesPage,          MenuIcon = UI.Bitmaps.GetBitmap("nav-zone-config") },
                        new NavBarMenuItemViewModel { Page = _groupsPage,         MenuIcon = UI.Bitmaps.GetBitmap("nav-zone-config") },
                        new NavBarMenuItemViewModel { Page = _setsPage,           MenuIcon = UI.Bitmaps.GetBitmap("nav-zone-config") },
                        new NavBarMenuItemViewModel { Page = _siteConfigPage,     MenuIcon = UI.Bitmaps.GetBitmap("nav-site-config") },
                        new NavBarMenuItemViewModel { Page = _ceConfigPage,       MenuIcon = UI.Bitmaps.GetBitmap("nav-paging-config") },
                        new NavBarMenuItemViewModel { Page = _networkConfigPage,  MenuIcon = UI.Bitmaps.GetBitmap("nav-paging-config") },
                        new NavBarMenuItemViewModel { Page = _eventLogPage,       MenuIcon = UI.Bitmaps.GetBitmap("nav-event-log") },
                        new NavBarMenuItemViewModel { Page = _commentsPage,       MenuIcon = UI.Bitmaps.GetBitmap("nav-comments") },
                    };
                }
                return _navBarItems;
            }
        }


        private void setNavBarText()
        {
            var sel = NavBarSelectedIndex;

            for (int i = 0; i < NavBarItems.Count; i++)
            {
                NavBarItems[i].MenuText = (NavBarItems[i].Page.DataContext as PageViewModelBase).PageHeader;
                NavBarItems[i].RefreshView();
            }

            NavBarSelectedIndex = sel;
            OnPropertyChanged(nameof(NavBarItems));
            OnPropertyChanged(nameof(CurrentPage));
        }


        public void ChangeNavBarSelection(Key key)
        {
            if (key == Key.Up)
                NavBarSelectedIndex = NavBarSelectedIndex > 0 ? NavBarSelectedIndex - 1 : _navBarItems.Count - 1;
            else if (key == Key.Down)
                NavBarSelectedIndex = NavBarSelectedIndex < _navBarItems.Count - 1 ? NavBarSelectedIndex + 1 : 0;
        }
        #endregion


        /// <summary>refresh error info, if shown</summary>
        public void RefreshUI() => _validationWindow?.RefreshView();


        #region ViewModel properties
        public static string VersionString { get => string.Format(Cultures.Resources.Version_x, BuildInfo.Details.Version); }

        public BitmapImage CurrentLanguageIcon { get => CTecControls.Cultures.CultureTools.GetFlagIcon(Cultures.Resources.Culture?.Name); }

        public string CultureName { get => CultureInfo.CurrentCulture.Name; }

        public string SystemName { get => _data.SiteConfig.SystemName ?? ""; set { _data.SiteConfig.SystemName = value; OnPropertyChanged(); } }


        private SerialComms.ConnectionStatus _commsStatus = SerialComms.ConnectionStatus.Unknown;
        public SerialComms.ConnectionStatus CommsStatus
        {
            get => _commsStatus;
            set
            {
                (_eventLogPage as EventLogViewer)._context.CommsStatus = _commsStatus = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PanelIsConnected));
                OnPropertyChanged(nameof(PanelIsDisconnected));
                OnPropertyChanged(nameof(PanelIsReadOnly));
            }
        }

        public bool PanelIsConnected { get => SerialComms.IsConnected; }
        public bool PanelIsDisconnected { get => SerialComms.IsDisconnected; }

        public string PanelDisconnectedIconSvgPathData => IconUtilities.IconSvgPathData(IconTypes.DisconnectedSerial);


        private bool _panelIsReadOnly = true;
        public bool PanelIsReadOnly { get => _panelIsReadOnly; set { _panelIsReadOnly = value; OnPropertyChanged(); } }

        private int _navBarSelectedIndex;
        public int NavBarSelectedIndex
        {
            get => _navBarSelectedIndex;
            set
            {
                if (_pages.Count > 0)
                {
                    UIState.SetBusyState();
                    _navBarSelectedIndex = value >= 0 && value < NavBarItems.Count ? value : 0;
                    CurrentPage = NavBarItems[_navBarSelectedIndex].Page;
                    CurrentFilePath = (CurrentPage.DataContext as PageViewModelBase)?.CurrentFilePath;
                    HeaderPanelEnabled = CurrentPage != _eventLogPage && CurrentPage != _commentsPage;
                    OnPropertyChanged();
                }
            }
        }


        public Page CurrentPage
        {
            get => _currentPage;
            set
            {
                //refresh previous page first to make sure the bindings are up-to-date when we refresh the new page
                //if (value == _ceConfigPage)
                //    ((_currentPage = value).DataContext as IPanelToolsViewModel)?.PopulateView(_data);
                //else
                    ((_currentPage = value)?.DataContext as IPanelToolsViewModel)?.RefreshView();

                //if (_currentPage == _eventLogPage)
                //    PanelComms.PollingMode = PingModes.Listening;
                //else
                //    PanelComms.PollingMode = PingModes.Polling;
                PanelComms.PollingMode = PingModes.Polling;

                (_eventLogPage.DataContext as EventLogViewerViewModel).IsActivePage = _currentPage == _eventLogPage;

                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentPageIsDevices));

                updateFileMenuRecentFiles();
            }
        }


        public new int PanelNumber
        {
            get => XfpData.CurrentPanelNumber;
            set
            {
                if (value != XfpData.CurrentPanelNumber)
                {
                    var readOnlyState = IsReadOnly;

                    XfpData.CurrentPanelNumber = value;
                    PopulateView(_data);
                    foreach (var p in _pages)
                        if (p.DataContext is PanelToolsPageViewModelBase vm)
                            vm.PanelNumber = value;
                    OnPropertyChanged();

                    IsReadOnly = readOnlyState;
                }
            }
        }


        public List<int> PanelNumberSet => [.. _data.Panels.Keys];
        public int PanelCount => _data.Panels.Count;
        public string NumberOfPanels => string.Format(Cultures.Resources.Panel_Count_x, PanelCount);
        public int PanelNumberMin => XfpData.MinPanelNumber;
        public int PanelNumberMax => XfpData.MaxPanelNumber;

        //Exposes the NumberSpinner's tooltip for use by the panel selector's parent framework element
        public string PanelNumberToolTip => _panelControl.ToolTip?.ToString();


        private bool _mainWindowEnabled  = true;
        private bool _headerPanelEnabled = true;
        public bool MainWindowEnabled { get => _mainWindowEnabled; set { _mainWindowEnabled = value; OnPropertyChanged(); } }
        public bool HeaderPanelEnabled { get => _headerPanelEnabled; set { _headerPanelEnabled = value; OnPropertyChanged(); OnPropertyChanged(nameof(PanelIsReadOnly)); } }


        private ScaleTransform _layoutTransform;
        public ScaleTransform LayoutTransform
        {
            get => _layoutTransform; 
            set
            {
                _layoutTransform = CTecUtil.Config.UI.LayoutTransform
                                 = CTecControls.Config.UI.LayoutTransform
                                 = Xfp.Config.UI.LayoutTransform
                                 = value; 
                OnPropertyChanged();
            }
        }
        #endregion


        #region protocol
        public string ApolloProtocol => CTecDevices.Cultures.Resources.ProtocolName_Apollo;
        public string CastProtocol => CTecDevices.Cultures.Resources.ProtocolName_Cast;


        /// <summary>
        /// The protocol that the Tools is set to
        /// </summary>
        public CTecDevices.ObjectTypes CurrentProtocol
        {
            get => DeviceTypes.CurrentProtocolType;
            set
            {
                if (DeviceTypes.CurrentProtocolType != value)
                {
                    CTecUtil.UI.UIState.SetBusyState();

                    XfpApplicationConfig.Settings.Protocol = CTecDevices.Enums.ObjectTypeName(_data.CurrentPanel.Protocol = DeviceTypes.CurrentProtocolType = value);

                    SetDeviceSelectorMenus();
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ProtocolName));
                    OnPropertyChanged(nameof(CurrentProtocolIsApollo));
                    OnPropertyChanged(nameof(CurrentProtocolIsCast));
                }
            }
        }

        /// <summary>
        /// The panel protocol
        /// </summary>
        private CTecDevices.ObjectTypes _panelProtocol = CTecDevices.ObjectTypes.NotSet;


        private void setProtocol(CTecDevices.ObjectTypes protocol)
        {
            if (!DataHasChanged || askOverwriteChanges(string.Format(Cultures.Resources.Change_Protocol_To_x, CTecDevices.Enums.ObjectTypeName(protocol))))
            {
                CurrentProtocol = protocol;
            }
        }

        public bool CurrentProtocolIsApollo { get => CurrentProtocol == CTecDevices.ObjectTypes.XfpApollo; set { if (value) { changeProtocol(CTecDevices.ObjectTypes.XfpApollo); OnPropertyChanged(); } } }
        public bool CurrentProtocolIsCast { get => CurrentProtocol == CTecDevices.ObjectTypes.XfpCast; set { if (value) { changeProtocol(CTecDevices.ObjectTypes.XfpCast); OnPropertyChanged(); } } }

        private bool changeProtocol(CTecDevices.ObjectTypes newProtocol, bool askConfirm = true)
        {
            if (newProtocol != CurrentProtocol)
            {
                if (askConfirm && !askClearCurrentData(string.Format(Cultures.Resources.Change_Protocol_To_x, newProtocol)))
                    return false;

                //CurrentProtocol = newProtocol;
                ClosePopups();
                //if (askConfirm)
                    InitNewDataset(newProtocol, false, askConfirm);
            }

            return true;
        }

        public string ProtocolName
        {
            get => DeviceTypes.CurrentProtocolName;
            set
            {
                DeviceTypes.CurrentProtocolType = value.ToLower() switch
                {
                    "apollo" => CTecDevices.ObjectTypes.XfpApollo,
                    _ => CTecDevices.ObjectTypes.XfpCast
                };
                OnPropertyChanged();
            }
        }
        #endregion


        #region menu options and hot keys
        private HamburgerMenu _mainMenu;
        private HotKeyList _hotKeys;

        private HotKeyList HotKeys { get => _hotKeys; }


        private void SetHambMenuText() => _mainMenu.SetCulture(CultureInfo.CurrentCulture);


        /// <summary>
        /// Handle 'translation' of hotkeys according to the menu text.<br/>
        /// NB: _hotkeys must be initialised before calling as this is a recursive function.
        /// </summary>
        /// <param name="ctrl"></param>
        private void assignHotKeys(Control ctrl, HotKeyList hotKeys)
        {
            if (ctrl is null) return;

            if (ctrl is HamburgerMenu menu)
            {
                var children = UITools.GetLogicalChildCollection<Control>(menu.Content);

                foreach (var child in children)
                {
                    if (child is HamburgerMenu submenu)
                    {
                        assignHotKeys(submenu, hotKeys);

                        var grandchildren = UITools.GetLogicalChildCollection<Control>(submenu.Content);
                        foreach (var grandchild in grandchildren)
                            assignHotKeys(grandchild, hotKeys);
                    }
                    else if (child.DataContext is HamburgerMenuItemViewModel dc)
                    {
                        dc.InputGestureText = getInputGestureKey(dc.RawText, hotKeys);
                    }
                }
            }
        }


        private string getInputGestureKey(string text, HotKeyList hotKeys)
        {
            if (text is null)
                return "";

            if (text.Trim().TrimEnd('_').Contains('_') || text == CTecControls.Cultures.Resources.Menu_Exit)
            {
                // is it one of the options that have hotkeys?
                if (text == CTecControls.Cultures.Resources.Menu_File
                 || text == CTecControls.Cultures.Resources.Menu_New
                 || text == CTecControls.Cultures.Resources.Menu_Open
                 || text == CTecControls.Cultures.Resources.Menu_Save
                 || text == CTecControls.Cultures.Resources.Menu_Print
                 || text == CTecControls.Cultures.Resources.Menu_Settings
                 || text == CTecControls.Cultures.Resources.Menu_Protocol
                 || text == CTecControls.Cultures.Resources.Menu_View
                 || text == CTecControls.Cultures.Resources.Menu_Zoom
                 || text == CTecControls.Cultures.Resources.Menu_Help
                 || text == CTecControls.Cultures.Resources.Menu_Exit
                 || text == CTecControls.Cultures.Resources.Menu_Hotkey_Debug_Mode)
                {
                    string key;

                    // modifier keys - default is to use Ctrl; Serial Monitor uses Shift+Alt; DebugMode uses Ctrl+Shift+Alt
                    bool ctrl  = false;
                    bool shift = false;
                    bool alt   = false;

                    var isTopLevel = text == CTecControls.Cultures.Resources.Menu_File
                                  || text == CTecControls.Cultures.Resources.Menu_Settings
                                  || text == CTecControls.Cultures.Resources.Menu_View
                                  || text == CTecControls.Cultures.Resources.Menu_Help;

                    if (text == CTecControls.Cultures.Resources.Menu_Exit)
                    {
                        alt = true;
                        key = "F4";
                    }
                    else
                    {
                        // find the underscore in the menu text (already guaranteed it's there and isn't last char)
                        var ul = text.IndexOf("_");

                        // underscore precedes the hotkey
                        key = text.ToUpper(CultureInfo.CurrentCulture)[ul + 1].ToString();

                        if (text == CTecControls.Cultures.Resources.Menu_Hotkey_Debug_Mode)
                            ctrl = alt = shift = true;
                        else if (text == CTecControls.Cultures.Resources.Menu_Hotkey_Serial_Monitor)
                            shift = alt = true;
                        else if (isTopLevel)
                            alt = true;
                        else
                            ctrl = true;
                    }

                    // add the key and its related action to the hotkeys
                    var properties = new HotKeyList.KeyProperties(key, ctrl, shift, alt);
                    Action command = null;

                    if (isTopLevel)
                        command = new(() => openMainMenu(text));
                    else
                        command = getCommandForMenuText(text);

                    if (hotKeys.Add(properties, command))
                    {
                        // set menu item's hotkey modifiers
                        var prefix = new StringBuilder();
                        if (ctrl)  prefix.Append(CTecControls.Cultures.Resources.Key_Name_Ctrl + "+");
                        if (shift) prefix.Append(CTecControls.Cultures.Resources.Key_Name_Shift + "+");
                        if (alt)   prefix.Append(CTecControls.Cultures.Resources.Key_Name_Alt + "+");

                        return prefix + key;
                    }
                }
            }

            return "";
        }


        private Action getCommandForMenuText(string menuText)
        {
            if (menuText == CTecControls.Cultures.Resources.Menu_New) return () => InitNewDataset();
            if (menuText == CTecControls.Cultures.Resources.Menu_Open) return () => FileOpen(null);
            if (menuText == CTecControls.Cultures.Resources.Menu_Save) return FileSave;
            if (menuText == CTecControls.Cultures.Resources.Menu_Save_As) return FileSaveAs;
            if (menuText == CTecControls.Cultures.Resources.Menu_Print) return showPrintOptions;
            if (menuText == CTecControls.Cultures.Resources.Menu_Zoom) return showZoomControl;
            if (menuText == CTecControls.Cultures.Resources.Menu_Hotkey_Debug_Mode) return ToggleDebugMode;
            return null;
        }


        public bool CheckHotKeys(KeyEventArgs e)
        {
            var ctrl  = (e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            var shift = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift)   == ModifierKeys.Shift;
            var alt   = (e.KeyboardDevice.Modifiers & ModifierKeys.Alt)     == ModifierKeys.Alt;

            var keyStr = TextProcessing.KeyToString(e);

            HotKeyList.KeyProperties hotKey = new(keyStr, ctrl, shift, alt);

            if (HotKeys.TryGetValue(hotKey, out Action command))
            {
                command?.Invoke();
                return true;
            }

            return false;
        }


        /// <summary>
        /// Check whether the key pressed is an option on the specified menu
        /// </summary>
        public bool CheckKey(KeyEventArgs e, MenuItem menu)
        {
            var keyStr = TextProcessing.KeyToString(e);
            if (string.IsNullOrEmpty(keyStr))
                return false;

            if (menu.HasItems)
            {
                if (!menu.IsSubmenuOpen)
                {
                    if (menu.Header is string topMenuText)
                    {
                        var txt = topMenuText.Trim().TrimEnd('_').Trim();
                        if (txt.Contains('_'))
                        {
                            if (keyStr[0] == topMenuText.ToUpper(CultureInfo.CurrentCulture)[topMenuText.IndexOf("_") + 1])
                            {
                                menu.IsSubmenuOpen = true;
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    //scan the menu items for one with the required underlined letter
                    foreach (var i in menu.Items)
                    {
                        if (i is MenuItem menuItem && menuItem.Header is string menuText)
                        {
                            var txt = menuText.Trim().TrimEnd('_').Trim();
                            if (txt.Contains('_'))
                            {
                                var shortcutKey = menuText.ToUpper(CultureInfo.CurrentCulture)[menuText.IndexOf("_") + 1];

                                if (keyStr[0] == shortcutKey)
                                {
                                    MainMenuIsOpen = false;

                                    //get the menu item's Command, if set
                                    var cmd = ((MenuItem)i).Command;
                                    if (cmd is not null)
                                    {
                                        cmd.Execute(null);
                                        return true;
                                    }

                                    //no Command, check the text
                                    var action = getCommandForMenuText(menuText);
                                    if (action is not null)
                                    {
                                        action.Invoke();
                                        return true;
                                    }

                                    //recent files' numeric shortcuts
                                    if (TextProcessing.CharIsNumeric(shortcutKey) && menuItem.Tag is string path)
                                    {
                                        FileOpen(path);
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }
        #endregion


        #region recent files list
        private bool _recentFilesListAvailable;
        //private bool _recentFilesListIsOpen;
        public bool RecentFilesListAvailable { get => _recentFilesListAvailable; set { _recentFilesListAvailable = value; OnPropertyChanged(); } }
        //public bool RecentFilesListIsOpen    { get => _recentFilesListIsOpen;    set { if (_recentFilesListAvailable) { _recentFilesListIsOpen = value; OnPropertyChanged(); }  } }

        private ObservableCollection<TextBlock> _recentFilesList;
        public ObservableCollection<TextBlock> RecentFilesList { get => _recentFilesList; private set { _recentFilesList = value; OnPropertyChanged(); } }


        public delegate void RecentFilesChangeHandler();
        public RecentFilesChangeHandler RecentFilesChanged;


        private void updateFileMenuRecentFiles()
        {
            //clear the menu to remove any out-of-date recent files
            _recentFilesList = new();

            //add any recent files
            var recentFiles = XfpApplicationConfig.Settings.RecentPanelFiles.Items;

            if (RecentFilesListAvailable = recentFiles.Count > 0)
                for (int i = 0; i < recentFiles.Count; i++)
                    _recentFilesList.Add(new() { Text = (i + 1) % 10 + ": " + System.IO.Path.GetFileName(recentFiles[i]), Tag = recentFiles[i], ToolTip = recentFiles[i] });

            OnPropertyChanged(nameof(RecentFilesList));
            RecentFilesListAvailable = _recentFilesList.Count > 0;

            RecentFilesChanged?.Invoke();
        }


        internal void OpenRecentFile(int index)
        {
            if (index >= 0 && index < _recentFilesList.Count)
                openDataFile(_recentFilesList[index].Tag as string);
        }

        internal void RemoveRecentFilesListItem(string filePath)
        {
            XfpApplicationConfig.Settings.RecentPanelFiles.Remove(filePath);
            updateFileMenuRecentFiles();
        }
        #endregion


        #region main menu button
        private bool _mainMenuIsOpen;
        public bool MainMenuIsOpen
        {
            get => _mainMenuIsOpen;
            set
            {
                if (_mainMenuIsOpen = value)
                {
                    initPortsComboList();
                    OnPropertyChanged(nameof(NoPortsAvailable));
                }
                else
                    ProtocolsHighlightVisibility = Visibility.Collapsed;

                OnPropertyChanged();
            }
        }

        public void ShowMainMenu()
        {
            MainMenuIsOpen = true;
        }

        public bool CloseMainMenu()
        {
            var closedIt = MainMenuIsOpen;
            MainMenuIsOpen = false;
            return closedIt;
        }


        //public delegate void MainMenuOpenerInitialiser();
        //public MainMenuOpenerInitialiser InitMainMenuOpeners;

        //public delegate void SubMenuOpener(string menuText);
        //public SubMenuOpener OpenSubMenu;

        private void openMainMenu(string menuText) => MainMenuIsOpen = true;

        internal void HighlightProtocolsMenu(HamburgerMenuOption protocolMenuHeader, HamburgerMenu protocolMenu, HamburgerMenuSubheader optionBelow)
        {
            //show the menu highlighting
            ProtocolsHighlightVisibility = Visibility.Visible;

            protocolMenuHeader.IsChecked = true;
            protocolMenu.IsOpen = true;
            MainMenuIsOpen = true;

            //ensure menu scrolls by showing the item below
            optionBelow.BringIntoView();
        }

        private Visibility _protocolsHighlightVisibility = Visibility.Collapsed;
        public Visibility ProtocolsHighlightVisibility { get => _protocolsHighlightVisibility; set { _protocolsHighlightVisibility = value; OnPropertyChanged(); } }

        #endregion#


        #region menu & button commands
        public void ShowPanelManagementWindow(Window parent) { ClosePopups(); CloseValidationWindow(); ShowPanelManagementPopup(parent); }
        public ICommand FileNewCommand { get => new DelegateCommand(() => { ClosePopups(); InitNewDataset(); }); }
        public ICommand FileOpenCommand { get => new DelegateCommand(() => { ClosePopups(); FileOpen(); }); }
        public ICommand FileSaveCommand { get => new DelegateCommand(() => { ClosePopups(); FileSave(); }); }
        public ICommand FileSaveAsCommand { get => new DelegateCommand(() => { ClosePopups(); FileSaveAs(); }); }
        public ICommand PrintCommand { get => new DelegateCommand(() => { ClosePopups(); showPrintOptions(); }); }
        public ICommand ConnectSerialCommand { get => new DelegateCommand(() => { ClosePopups(); ConnectSerialPort(); }); }
        public ICommand DisconnectSerialCommand { get => new DelegateCommand(() => { ClosePopups(); DisconnectSerialPort(); }); }
        public ICommand SelectLanguageCommand { get => new DelegateCommand(() => { ClosePopups(); ShowLanguageSelector(); }); }
        public ICommand ApolloProtocolCommand { get => new DelegateCommand(() => { ClosePopups(); setProtocol(CTecDevices.ObjectTypes.XfpApollo); }); }
        public ICommand CastProtocolCommand { get => new DelegateCommand(() => { ClosePopups(); setProtocol(CTecDevices.ObjectTypes.XfpCast); }); }
        public ICommand ZoomCommand { get => new DelegateCommand(() => { ClosePopups(); showZoomControl(); }); }
        public ICommand DataCommand { get => new DelegateCommand(() => { ClosePopups(); showDataPopup(); }); }
        public ICommand CommsLogCommand { get => new DelegateCommand(() => { ClosePopups(); ShowCommsLog(false); }); }
        public ICommand AboutCommand { get => new DelegateCommand(() => { ClosePopups(); ShowAboutPopup(); }); }
        public ICommand SwitchDebugModeCommand { get => new DelegateCommand(() => { ClosePopups(); ToggleDebugMode(); }); }
        #endregion


        #region main menu options
        public bool FileOpen(string path = null)
        {
            try
            {
                CloseMainMenu();
                return openDataFile(path);
            }
            finally
            {
                updateFileMenuRecentFiles();
            }
        }

        public void FileSave()
        {
            if (preSaveToFileValidation())
            {
                //save all data
                var savedPath = TextFile.FilePath = CurrentFilePath;
                CurrentFilePath = XfpTextFile.SaveFile(_data, PanelNumber) ?? savedPath;
                SaveCopyOfDataFromFile();
                SaveCopyOfCurrentData();
                XfpApplicationConfig.Settings.RecentPanelFiles.Add(TextFile.FilePath);
            }
        }

        public void FileSaveAs()
        {
            if (preSaveToFileValidation())
            {
                var savedPath = TextFile.FilePath = CurrentFilePath;
                CurrentFilePath = XfpTextFile.SaveFileAs(_data, PanelNumber) ?? savedPath;
                XfpApplicationConfig.Settings.RecentPanelFiles.Add(TextFile.FilePath);
                updateFileMenuRecentFiles();
            }
        }

        private bool openDataFile(string path = null)
        {
            var savedData = new XfpData(_data);
            XfpData newData = null;

            try
            {
                CloseValidationWindow();

                if (path is not null)
                {
                    if (!new FileInfo(path).Exists)
                    {
                        if (errorFileDoesNotExist(path))
                            RemoveRecentFilesListItem(path);
                        return false;
                    }
                    TextFile.FilePath = path;
                }
                else
                {
                    if (!XfpTextFile.OpenFile())
                        return false;

                    path = XfpTextFile.FilePath;
                }

                if (!DataHasChanged || askOverwriteChanges(string.Format(Cultures.Resources.Open_File_x, Path.GetFileName(path))))
                {                    
                    if (LocalXfpFile.CheckForLegacyXfpFile(TextFile.FilePath))
                    {
                        newData = openLegacyXfpFile(path);
                    }
                    else
                    {
                        newData = openXfp2File(path);
                    }
                }

                if (newData is null)
                    return false;

                CurrentFilePath = TextFile.FilePath;
                foreach (var p in _pages)
                    (p.DataContext as PageViewModelBase).CurrentFilePath = CurrentFilePath;

                XfpApplicationConfig.Settings.RecentPanelFiles.Add(TextFile.FilePath);

                PopulateView(newData);
                SaveCopyOfDataFromFile();

                if (!newData.Validate())
                    ShowValidationWindow();

                OnPropertyChanged(nameof(SystemName));
                return true;
            }
            catch (Exception ex)
            {
                CTecUtil.Debug.WriteLine(EventLog.WriteError(ex.ToString()));
                errorOpeningFile(Cultures.Resources.Product_Full_Name, TextFile.FilePath, ex);
                PopulateView(savedData);
            }
            return false;
        }


        /// <summary>
        /// Open a .XFP2 file
        /// </summary>
        private XfpData openXfp2File(string path)
        {
            XfpData result;
            
            List<CTecDevices.ObjectTypes> protocols = new();
            List<int> panelNumbers = new();
            List<string> firmwareVersions = new();
            LocalXfpFile.ReadDefiningSettings(TextFile.FilePath, ref protocols, ref panelNumbers, ref firmwareVersions);
            

            if (protocols.Count == 0)
            {
                CTecMessageBox.ShowOKError(Cultures.Resources.Error_Invalid_Xfp_Data, Cultures.Resources.Open_File);
                return null;
            }

            var msgVersionCheck = new StringBuilder();

            if (_data.CurrentPanel.PanelConfig.FirmwareVersion != CTecControls.Cultures.Resources.Not_Available)
            {
                if (panelNumbers?.IndexOf(PanelNumber - 1) is int idx)
                {
                    if (idx < firmwareVersions.Count)
                    {
                        if (_data.CurrentPanel.PanelConfig.FirmwareVersionEquals(firmwareVersions[idx]) == false)
                        {
                            msgVersionCheck.Append(firmwareVersions[idx] == CTecControls.Cultures.Resources.Not_Available
                                                    ? Cultures.Resources.File_Data_Is_Unknown_Firmware
                                                    : _data.CurrentPanel.PanelConfig.FirmwareVersionCompare(firmwareVersions[idx]) switch
                                                    {
                                                        -1 => string.Format(Cultures.Resources.File_Data_Is_Older_Firmware_x, firmwareVersions[idx]),
                                                        1 => string.Format(Cultures.Resources.File_Data_Is_Newer_Firmware_x, firmwareVersions[idx]),
                                                        _ => Cultures.Resources.File_Data_Is_Unknown_Firmware,
                                                    });
                        }

                        if (msgVersionCheck.Length > 0)
                            if (!askConfirm(msgVersionCheck.ToString(), "Version Check"))
                                return null;
                    }
                }
            }

            
            var loop = (_deviceDetailsPage.DataContext as DevicesViewModel).LoopNum - 1;

            if (loop < protocols.Count && protocols[loop] != CurrentProtocol)
            {
                if (!askProtocolChange(Path.GetFileName(path), DeviceTypes.ProtocolName(protocols[loop])))
                    return null;

                changeProtocol(protocols[loop], false);
            }

            //assume it's a json multi-panel dataset
            result = LocalXfpFile.ReadJsonXfpFile(TextFile.FilePath);

            //no panels may indicate older single-panel xfp2 file
            if (result.Panels.Count == 0)
                result = openOlderXfp2File(path);

            if (result is null
                || result.CurrentPanel.LoopConfig?.Loop1 is null
                || result.CurrentPanel.LoopConfig?.Loop2 is null
                || result.CurrentPanel.ZoneConfig is null
                || result.CurrentPanel.GroupConfig is null
                || result.SiteConfig is null)
            {
                errorOpeningFile(Cultures.Resources.Product_Full_Name, TextFile.FilePath, Cultures.Resources.Error_File_Data_Is_Not_Panel_Data);
                return null;
            }

            if (!result.Panels.ContainsKey(PanelNumber))
                PanelNumber = result.Panels.First().Value.PanelNumber;

            if (result.CurrentPanel.Protocol == CTecDevices.ObjectTypes.NotSet)
            {
                errorOpeningFile(Cultures.Resources.Product_Full_Name, TextFile.FilePath, Cultures.Resources.Error_File_Data_Unsupported_Protocol);
                return null;
            }

            return result;
        }


        class OlderXfp2Data
        {

        }

        /// <summary>
        /// Open an .XFP2 file
        /// </summary>
        private XfpData openOlderXfp2File(string path)
        {
            XfpData result;

            result = LocalXfpFile.ReadOlderJsonXfpFile(TextFile.FilePath);

            if (!result.Panels.ContainsKey(PanelNumber))
                PanelNumber = result.Panels.First().Value.PanelNumber;

            //if (result is null
            //    || result.CurrentPanel.LoopConfig?.Loop1 is null
            //    || result.CurrentPanel.LoopConfig?.Loop2 is null
            //    || result.CurrentPanel.ZoneConfig is null
            //    || result.CurrentPanel.GroupConfig is null
            //    || result.SiteConfig is null)
            //{
            //    errorOpeningFile(Cultures.Resources.Product_Full_Name, TextFile.FilePath, Cultures.Resources.Error_File_Data_Is_Not_Panel_Data);
            //    return null;
            //}

            //if (result.CurrentPanel.Protocol == CTecDevices.ObjectTypes.NotSet)
            //{
            //    errorOpeningFile(Cultures.Resources.Product_Full_Name, TextFile.FilePath, Cultures.Resources.Error_File_Data_Unsupported_Protocol);
            //    return null;
            //}

            return result;
        }


        /// <summary>
        /// Open a legacy .XFP file as created by the old Tools
        /// </summary>
        private XfpData openLegacyXfpFile(string path)
        {
            XfpData result;

            //read the protocol type(so we know how many devices to initialise)
            //and panel number(so we know where the data is going)

            List<CTecDevices.ObjectTypes> protocols = new();
            List<int> panelNumbers = new();
            List<string> firmwareVersions = new();

            LocalXfpFile.ReadDefiningSettings(TextFile.FilePath, ref protocols, ref panelNumbers, ref firmwareVersions);

            if (protocols.Count == 0)
            {
                CTecMessageBox.ShowOKError(Cultures.Resources.Error_Invalid_Xfp_Data, Cultures.Resources.Open_File);
                return null;
            }

            //NB: legacy file has only global protocol/panelnumber/firmware, so retrieve them (or defaults)
            var protocol        = protocols.Count > 0        ? protocols[0]        : CTecDevices.ObjectTypes.XfpCast;
            var panelNumber     = panelNumbers.Count > 0     ? panelNumbers[0]     : XfpData.MinPanelNumber;
            var firmwareVersion = firmwareVersions.Count > 0 ? firmwareVersions[0] : CTecControls.Cultures.Resources.Unknown;

            var protocolChanged = protocol != CurrentProtocol;
            var keepOtherPanels = true;

            if (DataHasChanged || protocolChanged)
            {
                if (protocolChanged)
                {
                    if (!askProtocolChange(Path.GetFileName(path), DeviceTypes.ProtocolName(protocol)))
                        return null;

                    changeProtocol(protocol, false);
                }
                else if (_data.Panels.ContainsKey(panelNumber) && PanelDataHasChanged(panelNumber))
                {
                    if (!askOverwritePanel(panelNumber, string.Format(Cultures.Resources.Open_File_x, Path.GetFileName(path))))
                        return null;
                }
                else
                {
                    if (_data.Panels.Count > 1 || !_data.Panels.ContainsKey(panelNumber))
                    {
                        switch (askKeepExistingPanelData(panelNumber))
                        {
                            case MessageBoxResult.No:
                                keepOtherPanels = false;
                                break;
                            case MessageBoxResult.Cancel:
                                return null;
                        }
                    }
                }
            }

            if (protocolChanged || !keepOtherPanels)
                result = XfpData.InitialisedNew(protocol, panelNumber, true);
            else
                result = new XfpData(_data);

            var panelData = LocalXfpFile.ReadLegacyXfpFile(TextFile.FilePath, protocol, panelNumber);

            if (result.Panels.Keys.Contains(panelNumber))
                result.Panels[panelNumber] = panelData.Panels[panelNumber];
            else
                result.Panels.Add(PanelNumber = panelNumber, panelData.Panels[panelNumber]);

            result.CurrentPanel.PanelConfig.FirmwareVersion = panelData.CurrentPanel.PanelConfig.FirmwareVersion;
            result.ToolsVersion = panelData.ToolsVersion;
            result.SiteConfig = new(panelData.SiteConfig);
            result.Comments = panelData.Comments;

            return result;
        }


        private MessageBoxResult askKeepExistingPanelData(int panelNumber)
        {
            if (PanelNumberSet.Count > 1 || panelNumber != PanelNumber)
                return CTecMessageBox.ShowYesNoCancelQuery(PanelNumberSet.Count == 1 ? string.Format(Cultures.Resources.Query_Keep_Other_Panel_Settings_Single_x, panelNumber, PanelNumber)
                                                                                     : string.Format(Cultures.Resources.Query_Keep_Other_Panel_Settings, panelNumber),
                                                           Cultures.Resources.Other_Panel_Settings,
                                                           new() { Cultures.Resources.Option_Keep, Cultures.Resources.Option_Remove });
            return MessageBoxResult.Yes;
        }


        private bool preSaveToFileValidation()
        {
            if (!_data.Validate())
            {
                var siteWarnings = _data.SiteConfig.HasWarnings();
                var otherErrors  = _data.SiteConfig.HasErrors();

                //check other pages
                if (!otherErrors)
                    foreach (var p in _data.Panels)
                        if (p.Value.HasErrorsOrWarnings())
                            otherErrors = true;

                //if there are errors, show info window and ask whether to save
                if (siteWarnings || otherErrors)
                {
                    ShowValidationWindow();

                    return CTecMessageBox.ShowYesNoWarn(siteWarnings ? Cultures.Resources.Query_Site_Config_Warnings_Save_Anyway 
                                                                     : Cultures.Resources.Query_Data_Errors_Save_Anyway,
                                                        Cultures.Resources.Pre_Save_Check) == MessageBoxResult.Yes;
                }
            }

            return true;
        }


        public bool LanguageSelectionAvailable => CTecUtil.Cultures.CultureResources.SupportedCultures.Count > 0;

        public void ShowPortsComboBoxDropdown(object sender)
        {
            initPortsComboList();
            base.ShowComboBoxDropdown(sender);
        }
        #endregion


        #region serial port

        SerialPortWatcher _comPortWatcher;
        private bool _noPortsAvailable;

        private void startComPortWatcher()
        {
            var portNames = SerialComms.GetPortNames();
            PortNamesList = [.. portNames];
            _comPortWatcher = new();
            _comPortWatcher.PortsChanged = new((ports) => initPortsComboList(ports));
        }


        public bool SerialPortIsIdle => SerialComms.IsIdle;
        public string ToolTip_SelectCOMPort => CTecControls.Cultures.Resources.ToolTip_Select_COM_Port;


        public void ConnectSerialPort()
        {
            if (!SerialComms.IsConnected)
            {
                CTecUtil.UI.UIState.SetBusyState();
                //Dispatcher.CurrentDispatcher.Invoke(new Action(() => { Mouse.OverrideCursor = Cursors.Wait; Thread.Sleep(2000); Mouse.OverrideCursor = null; }), DispatcherPriority.Normal);
                //Dispatcher.CurrentDispatcher.Invoke(new Action(() => SerialComms.Connect()), DispatcherPriority.Normal);
                SerialComms.Connect();
                Thread.Sleep(2000);
                OnPropertyChanged(nameof(PanelIsConnected));
                OnPropertyChanged(nameof(PanelIsDisconnected));
            }
        }

        public void DisconnectSerialPort()
        {
            if (SerialPortIsIdle)
            {
                CTecUtil.UI.UIState.SetBusyState();
                SerialComms.Disconnect();
                Thread.Sleep(2000);
                OnPropertyChanged(nameof(PanelIsConnected));
                OnPropertyChanged(nameof(PanelIsDisconnected));
            }
        }

        public ObservableCollection<string> PortNamesList { get; set; }

        public bool NoPortsAvailable { get => _noPortsAvailable; set { _noPortsAvailable = value; OnPropertyChanged(); } }

        //public string SerialPortNameIconPathData => IconUtilities.IconSvgPathData(IconTypes.PortName);


        public int PortNameIndex
        {
            get => PortNamesList.IndexOf(PortName);
            set { try { PortName = PortNamesList[value]; } catch { PortName = ""; } }
        }

        public string PortName
        {
            get => XfpApplicationConfig.Settings.SerialPortSettings.PortName;
            set
            {
                if (!string.IsNullOrEmpty(value) && value != SerialComms.PortName)
                    XfpApplicationConfig.SerialPortName = SerialComms.PortName = value;
                OnPropertyChanged();
            }
        }

        private void initPortsComboList()
        {
            try
            {
                var port = SerialComms.Settings.PortName;

                NoPortsAvailable = PortNamesList.Count == 0;
                PortNameIndex = PortNamesList.IndexOf(port);

                OnPropertyChanged(nameof(PortNamesList));
                OnPropertyChanged(nameof(PortNameIndex));
                OnPropertyChanged(nameof(PortName));
                OnPropertyChanged(nameof(NoPortsAvailable));
            }
            catch (InvalidOperationException) { }
        }

        private void initPortsComboList(List<string> ports)
        {
            try
            {
                PortNamesList = new(ports);
                initPortsComboList();
            }
            catch (InvalidOperationException) { }
        }
        #endregion


        #region Print dialog
        private void showPrintOptions()
        {
            UIState.SetBusyState();

            var printDialog = new PrintDialogWindow(_data, XfpApplicationConfig.Settings, _pages, _currentPage) { Owner = _mainAppWindow };
            bool? result;

            try
            {
                MainWindowEnabled = false;
                result = printDialog.ShowDialog();
            }
            finally
            {
                MainWindowEnabled = true;
            }

            if (result == true)
            {
                var p = printDialog.PrintQueue;
                XfpPrinting.PrintConfig(_data, printDialog.PrintParams, PrintActions.Print);
            }
        }

        internal void PrintPreview() { }
        #endregion


        #region pop-ups


        #region panel management
        private NumberSpinner _panelControl;

        public void ShowPanelManagementPopup(Window mainWindow)
        {
            UIState.SetBusyState();
            MainWindowEnabled = false;
            var originalPanelNumber = PanelNumber;

            new PanelManagementWindow(_data, CurrentProtocol, PanelNumberSet).ShowDialog(mainWindow);

            if (!PanelNumberSet.Contains(originalPanelNumber))
            {
                var found = false;
                for (int i = PanelNumberSet.Count - 1; i >= 0; i--)
                {
                    if (PanelNumberSet[i] < originalPanelNumber)
                    {
                        PanelNumber = PanelNumberSet[i];
                        found = true;
                        break;
                    }
                }

                if (!found)
                    PanelNumber = PanelNumberSet[0];
            }

            MainWindowEnabled = true;

            OnPropertyChanged(nameof(PanelNumber));
            OnPropertyChanged(nameof(PanelNumberSet));
        }
        #endregion


        #region language selector pop-up
        public PopLanguageSelectorViewModel PopLanguageSelectorContext;

        public delegate void NewLanguageSelectorContextNotifier(PopLanguageSelectorViewModel context);
        
        /// <summary>
        /// Notify when language pop-up's data context has changed
        /// </summary>
        public NewLanguageSelectorContextNotifier NewLanguageSelectorContext;

        public void ShowLanguageSelector()
        {
            NewLanguageSelectorContext?.Invoke(PopLanguageSelectorContext = new PopLanguageSelectorViewModel()
            {
                OnCultureChanged = cultureChanged,
                LanguageSelectorIsOpen = true
            });
        }

        public bool CloseLanguageSelector()
        {
            bool closedIt = false;

            if (PopLanguageSelectorContext is not null)
            {
                closedIt = PopLanguageSelectorContext.LanguageSelectorIsOpen;
                PopLanguageSelectorContext.LanguageSelectorIsOpen = false;
            }

            return closedIt;
        }

        private void cultureChanged(CultureInfo culture)
        {
            PopLanguageSelectorContext.LanguageSelectorIsOpen = false;
            SetCulture(culture);
        }
        #endregion


        #region screen zoom pop-up
        public double ZoomLevel
        {
            get => XfpApplicationConfig.Settings.MainWindow.Scale;
            set
            {
                XfpApplicationConfig.Settings.MainWindow.Scale = value;
                LayoutTransform = new ScaleTransform(value, value);
                OnPropertyChanged();
            }
        }

        private bool _zoomControlIsOpen;
        private bool _dataPopupIsOpen;
        public bool ZoomControlIsOpen { get => _zoomControlIsOpen; set { _zoomControlIsOpen = value; OnPropertyChanged(); } }
        public bool DataPopupIsOpen   { get => _dataPopupIsOpen;   set { if (_dataPopupIsOpen = value) refreshDataInfo(); OnPropertyChanged(); } }

        public double MinZoom => XfpApplicationConfig.MinZoom;
        public double MaxZoom => XfpApplicationConfig.MaxZoom;
        public double ZoomStep => XfpApplicationConfig.ZoomStep;
        public double LargeZoomStep => ZoomStep * 2;
        public double TickFrequency => (MaxZoom - MinZoom) / 4;

        public void ZoomIn() { ClosePopups(true); ZoomLevel = Math.Min(ZoomLevel + ZoomStep, MaxZoom); }
        public void ZoomOut() { ClosePopups(true); ZoomLevel = Math.Max(ZoomLevel - ZoomStep, MinZoom); }

        private void showZoomControl() => ZoomControlIsOpen = true;

        public bool CloseZoomControl()
        {
            var closedIt = ZoomControlIsOpen;
            ZoomControlIsOpen = false;
            return closedIt;
        }

        public ZoomControl ZoomControl = new();
        #endregion


        private void showDataPopup() => DataPopupIsOpen = true;


        #region download log window
        private CTecControls.UI.CommsLogWindow _logWindow;
        
        
        public void ShowCommsLog(bool scrollToEnd)
        {
            UIState.SetBusyState();
            CloseCommsLog();
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                                                     {
                                                         (_logWindow = new CTecControls.UI.CommsLogWindow(XfpApplicationConfig.Settings, DebugMode)).Show();
                                                         if (scrollToEnd) _logWindow.ScrollToEnd();
                                                     }), DispatcherPriority.Background);
        }

        public void CloseCommsLog()
        {
            if (_logWindow is not null)
                _logWindow.Close();
            _logWindow = null;
        }
        #endregion


        #region data info
        public int?   DetectedLoops           => LoopConfigData.DetectedLoops;
        public string DebugNamesUsed          => _data.CurrentPanel.DeviceNamesConfig.TotalNamesUsedText;
        public int    DebugNameBytesUsed      => _data.CurrentPanel.DeviceNamesConfig.BytesUsed;
        public int    DebugNameBytesRemaining => _data.CurrentPanel.DeviceNamesConfig.BytesRemaining;
        
        private void refreshDataInfo()
        {
            OnPropertyChanged(nameof(DetectedLoops));
            OnPropertyChanged(nameof(DebugNamesUsed));
            OnPropertyChanged(nameof(DebugNameBytesUsed));
            OnPropertyChanged(nameof(DebugNameBytesRemaining));
        }
        #endregion


        #region About pop-up
        public void ShowAboutPopup() => AboutIsOpen = true;

        public bool CloseAboutPopup()
        {
            bool closedIt = AboutIsOpen;
            AboutIsOpen = false;
            return closedIt;
        }


        private bool _aboutIsOpen;
        public bool AboutIsOpen { get => _aboutIsOpen; set { _aboutIsOpen = value; OnPropertyChanged(); OnPropertyChanged(nameof(AboutHeaderWidth)); } }

        public TextBlock AboutHeaderTextBlock { get; set; }
        public string AboutHeader      => Cultures.Resources.Product_Full_Name;
        public double AboutHeaderWidth => FontUtil.MeasureText(AboutHeader, AboutHeaderTextBlock.FontFamily, AboutHeaderTextBlock.FontSize, AboutHeaderTextBlock.FontStyle, AboutHeaderTextBlock.FontWeight, AboutHeaderTextBlock.FontStretch).Width;


        public string   ExeVersion       { get => string.Format(Cultures.Resources.Version_x_Abbr, BuildInfo.Details.Version); }
        public DateTime ExeDate          { get => BuildInfo.Details.BuildDate.Value; }
        public string   ControlsVersion  { get => string.Format(Cultures.Resources.Version_x_Abbr, CTecControls.BuildInfo.Details.Version); }
        public DateTime ControlsDate     { get => CTecControls.BuildInfo.Details.BuildDate.Value; }
        public string   DevicesVersion   { get => string.Format(Cultures.Resources.Version_x_Abbr, CTecDevices.BuildInfo.Details.Version); }
        public DateTime DevicesDate      { get => CTecDevices.BuildInfo.Details.BuildDate.Value; }
        public string   UtilVersion      { get => string.Format(Cultures.Resources.Version_x_Abbr, CTecUtil.BuildInfo.Details.Version); }
        public DateTime UtilDate         { get => CTecUtil.BuildInfo.Details.BuildDate.Value; }
        public string   CopyrightDetails { get => string.Format(Cultures.Resources.Copyright_Details, BuildInfo.Details.BuildYear); }
        public string   Who              { get => Credits.Components[0].Notes; }
        #endregion


        #region Revision History & Registration
        public void ShowRevisionHistoryWindow()
        {
            UIState.SetBusyState();
            try
            {
                MainWindowEnabled = false;

                var document = DocxToFlowDocumentConverter.ReadDocxResource("XFP Revision History");
                new FlowDocumentViewer(document, Cultures.Resources.XFP_Revision_History, XfpApplicationConfig.Settings, false).ShowDialog();
            }
            catch (Exception ex) { }
            finally { MainWindowEnabled = true; }
        }

        public void ShowRegistrationWindow(bool conditionalCheck = false)
        {
            if (!conditionalCheck || XfpApplicationConfig.Settings.RegistrationDetails.LastPrompted is null
                                  || string.IsNullOrEmpty(XfpApplicationConfig.Settings.RegistrationDetails.Email) &&
                                     XfpApplicationConfig.Settings.RegistrationDetails.LastPrompted?.Date.Add(new TimeSpan(7, 0, 0, 0)).Date < DateTime.Now.Date)
            {
                UIState.SetBusyState();
                try
                {
                    MainWindowEnabled = false;
                    new RegistrationWindow(XfpApplicationConfig.Settings).ShowDialog();
                }
                catch (Exception ex) { }
                finally { MainWindowEnabled = true; }
            }
        }
        #endregion


        public bool ClosePopups() => ClosePopups(false);

        public bool ClosePopups(bool excludingZoomPopup)
        {
            UIState.SetBusyState();

            //var closedPrinters   = ClosePrinterList();
            //var closedPrint      = ClosePrintOption();
            var closedLang       = CloseLanguageSelector();
            var closedMain       = CloseMainMenu();
            var closedAbout      = CloseAboutPopup();
            var closedZoom       = excludingZoomPopup || CloseZoomControl();

            return /*closedPrinters || closedPrint ||*/ closedLang || closedMain || closedAbout || closedZoom;
        }

        #endregion


        #region messages
        private bool askOverwriteChanges(string msgBoxTitle) => CTecMessageBox.ShowYesNoQuery(Cultures.Resources.Warn_Overwrite_Changes + "\n\n" +
                                                                                              Cultures.Resources.Unsaved_Changes_Will_Be_Lost + "\n\n" +
                                                                                              Cultures.Resources.Warn_Confirm_Continue,
                                                                                              msgBoxTitle) == MessageBoxResult.Yes;
        private bool askOverwritePanel(int panelNumber, string msgBoxTitle) => CTecMessageBox.ShowYesNoQuery(string.Format(Cultures.Resources.Warn_Overwrite_Panel_x, PanelNumber) + "\n\n" +
                                                                                                             Cultures.Resources.Warn_Confirm_Continue,
                                                                                                             msgBoxTitle) == MessageBoxResult.Yes;
        private bool askProtocolChange(string msgBoxTitle, string protocol) => CTecMessageBox.ShowYesNoWarn(string.Format(Cultures.Resources.Warn_File_Protocol_x, protocol) + "\n\n" +
                                                                                                            Cultures.Resources.Warn_Clear_Data + "\n" +
                                                                                                            Cultures.Resources.Unsaved_Changes_Will_Be_Lost + "\n\n" +
                                                                                                            Cultures.Resources.Warn_Confirm_Continue,
                                                                                                            msgBoxTitle) == MessageBoxResult.Yes;
        private bool askClearCurrentData(string msgBoxTitle) => CTecMessageBox.ShowYesNoQuery(Cultures.Resources.Warn_Clear_Data + "\n" +
                                                                                              Cultures.Resources.Unsaved_Changes_Will_Be_Lost + "\n\n" +
                                                                                              Cultures.Resources.Warn_Confirm_Continue, msgBoxTitle) == MessageBoxResult.Yes;
        private bool askConfirm(string message, string msgBoxTitle) => CTecMessageBox.ShowYesNoWarn(message + "\n\n" + Cultures.Resources.Warn_Confirm_Continue, msgBoxTitle) == MessageBoxResult.Yes;
        private bool errorFileDoesNotExist(string path) => CTecMessageBox.ShowYesNoError(string.Format(Cultures.Resources.Error_File_x_Does_Not_Exist, Path.GetFileName(path)), Cultures.Resources.Open_File) == MessageBoxResult.Yes;
        private void errorUnknownFirmware(string version) => CTecMessageBox.ShowOKError(string.Format(Cultures.Resources.Error_Comms_Firmware_Unknown, version), Cultures.Resources.Panel_Comms);
        private void errorFirmwareVersionNotSupported(string version) => CTecMessageBox.ShowOKError(string.Format(Cultures.Resources.Error_Comms_Firmware_x_Not_Supported, version), Cultures.Resources.Panel_Comms);
        private void errorOpeningFile(string component, string filename, string reason = null) => CTecMessageBox.ShowOKError(Cultures.Resources.Error_Opening_File + "\n\n" + (!string.IsNullOrEmpty(reason) ? reason + "\n\n" : "") + filename, component);
        private void errorOpeningFile(string component, string filename, Exception exception) => CTecMessageBox.ShowException(Cultures.Resources.Error_Opening_File + "\n\n" + filename, component, exception);
        private void errorInvalidData(string message, string type) => CTecMessageBox.ShowOKError(Cultures.Resources.Error_Opening_File + "\n\n" + message, Cultures.Resources.Product_Full_Name);
        private void errorCommsConnection(string message) => CTecMessageBox.ShowOKError(message, Cultures.Resources.Panel_Comms);
        #endregion


        #region Minimise/Maximise/Restore/Exit
        private bool _windowIsMaximised;
        public bool WindowIsMaximised { get => _windowIsMaximised; set { _windowIsMaximised = value; OnPropertyChanged(); } }
        public void ChangeWindowState(WindowState windowState) => WindowIsMaximised = windowState == WindowState.Maximized;

        public void ExitApp()
        {
            try
            {
                UIState.SetBusyState();
                
                //kill if we get a freeze on closing ports
                System.Timers.Timer killer = new(3000) { AutoReset = false, Enabled = true };
                killer.Elapsed += (s, e) => Environment.Exit(0);

                CTecUtil.IO.SerialComms.ClosePort();
                Thread closePort = new Thread(new ThreadStart(CloseSerialPort));
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                CTecUtil.Debug.WriteLine(EventLog.WriteError("Error on exiting app\n\n" + ex.ToString()));
            }
        }

        private void CloseSerialPort() { try { SerialComms.ClosePort(); } catch { } }
        #endregion


        #region Panel comms upload/download
        public void DownloadFromPanel(bool allPages)
        {
            if (_currentPage == _eventLogPage || _currentPage == _commentsPage)
                return;

            if (!checkPanelStatus(CommsDirection.Download, allPages))
                return;

            //check whether data on the relevant pages has been changed and would be overwritten
            bool dataHasBeenEdited = false;

            if (allPages)
            {
                dataHasBeenEdited = DataHasChanged;
            }
            else if (_currentPage.DataContext is IConfigToolsPageViewModel)
            {
                if (_savedData is XfpData sd && sd.Panels.Keys != _data.Panels.Keys)
                    dataHasBeenEdited = true;
                else
                {
                    foreach (var p in currentPageDownloadList)
                    {
                        if (!(p.DataContext as IConfigToolsPageViewModel).DataEquals((XfpData)_savedData))
                        {
                            dataHasBeenEdited = true;
                            break;
                        }
                    }
                }
            }

            if (!dataHasBeenEdited || askOverwriteChanges(Cultures.Resources.Download_System))
            {
                CloseValidationWindow();
                CloseCommsLog();
                ClosePopups();

                if (checkProtocol(CommsDirection.Download, allPages))
                {
                    if (allPages)
                    {
                        PanelComms.InitCommandQueue(CommsDirection.Download, Cultures.Resources.Comms_Downloading_System);
                        foreach (var p in _pages)
                            if (p != _loop1Page && p != _loop2Page)
                                enqueueDownloadCommandsForPage(p, allPages);
                    }
                    else
                    {
                        PanelComms.InitCommandQueue(CommsDirection.Download, Cultures.Resources.Comms_Downloading_Page);
                        foreach (var p in currentPageDownloadList)
                            enqueueDownloadCommandsForPage(p, allPages);
                    }

                    PanelComms.SendCommandQueueToPanel(_mainAppWindow, commsStarting, allPages ? downloadEndedAllPages : downloadEndedThisPage);
                }
            }
        }

        public void UploadToPanel(bool allPages)
        {
            if (_currentPage == _eventLogPage
             || _currentPage == _commentsPage
             || !checkPanelStatus(CommsDirection.Upload, allPages)
             || !checkProtocol(CommsDirection.Upload, allPages))
                return;

            List<Page> pagesToCheck = allPages ? new(_pages) : CurrentPageIsDevices ? new() { _deviceDetailsPage, _zonesPage/*, _groupsPage*/ } : new() { _currentPage };

            _data.Validate();
            foreach (var _ in from p in pagesToCheck
                              where p is IConfigToolsPageViewModel
                              where ((IConfigToolsPageViewModel)p.DataContext)?.HasErrors() ?? false
                              select new { })
            {
                ShowValidationWindow(NavBarItems[NavBarSelectedIndex].MenuText);
                CTecMessageBox.ShowOKError(Cultures.Resources.Error_Preupload_Data_Validation, Cultures.Resources.Upload_Page);
                return;
            }

            CloseValidationWindow();
            CloseCommsLog();
            ClosePopups();

            PanelComms.InitCommandQueue(CommsDirection.Upload, allPages ? Cultures.Resources.Comms_Uploading_System : Cultures.Resources.Comms_Uploading_Page);

            if (allPages)
            {
                foreach (var p in _pages)
                    if (p != _loop1Page && p != _loop2Page)
                        enqueueUploadCommandsForPage(p, true);
            }
            else
            {
                enqueueUploadCommandsForPage(_currentPage, false);
            }

            PanelComms.SendCommandQueueToPanel(_mainAppWindow, commsStarting, allPages ? uploadEndedAllPages : uploadEndedThisPage);
        }


        private bool checkPanelStatus(CommsDirection direction, bool allPages)
        {
            if (SerialComms.TransferInProgress())
                return false;

            if (SerialComms.IsDisconnected)
            {
                UIState.SetBusyState();
                SerialComms.Connect();
                Thread.Sleep(2000);
                return true;
            }

            string msg;

            switch (CommsStatus)
            {
                case SerialComms.ConnectionStatus.ConnectedWriteable:
                    return true;

                case SerialComms.ConnectionStatus.Disconnected:
                    msg = string.Format(Cultures.Resources.Error_Comms_Panel_x_Not_Connected, PanelNumber);
                    break;

                case SerialComms.ConnectionStatus.Listening:
                    msg = Cultures.Resources.Error_Comms_Logging_Mode;
                    break;

                case SerialComms.ConnectionStatus.ConnectedReadOnly:
                    if (direction == CommsDirection.Download)
                        return true;
                    msg = Cultures.Resources.Error_Comms_Nvm_Link;
                    break;

                case SerialComms.ConnectionStatus.FirmwareNotSupported:
                    msg = Cultures.Resources.Error_Comms_Firmware_Not_Supported;
                    break;

                default:
                    if (_panelProtocol == CTecDevices.ObjectTypes.NotSet)
                        msg = Cultures.Resources.Error_Comms_Panel_Protocol_Not_Identified;
                    else
                        msg = Cultures.Resources.Error_Comms_Connection_Status_Unknown;
                    break;
            }
            
            CTecMessageBox.ShowOKStop(msg, direction == CommsDirection.Upload ? allPages ? Cultures.Resources.Upload_System : Cultures.Resources.Upload_Page
                                                                              : allPages ? Cultures.Resources.Download_System : Cultures.Resources.Download_Page);
            return false;
        }


        private bool checkProtocol(CommsDirection direction, bool allPages)
        {
            if (_panelProtocol == CurrentProtocol)
                return true;

            if (direction == CommsDirection.Download)
            {
                var yes = CTecMessageBox.ShowYesNoWarn(string.Format(Cultures.Resources.Error_Comms_Panel_Protocol_x_Does_Not_Match_Tools_y,
                                                                     CTecDevices.Enums.ObjectTypeName(_panelProtocol),
                                                                     CTecDevices.Enums.ObjectTypeName(CurrentProtocol)) + "\n\n" + Cultures.Resources.Warn_Confirm_Continue,
                                                       allPages? Cultures.Resources.Comms_Downloading_System : Cultures.Resources.Comms_Downloading_Page) == MessageBoxResult.Yes;
                if (yes)
                    CurrentProtocol = _panelProtocol;
                return yes;
            }

            CTecMessageBox.ShowOKError(string.Format(Cultures.Resources.Error_Comms_Panel_Protocol_x_Does_Not_Match_Tools_y,
                                                     CTecDevices.Enums.ObjectTypeName(_panelProtocol),
                                                     CTecDevices.Enums.ObjectTypeName(CurrentProtocol)), 
                                       allPages? Cultures.Resources.Comms_Uploading_System : Cultures.Resources.Comms_Uploading_Page);
            return false;
        }
        #endregion


        #region Panel comms send/receive handlers
        private string _panelFirmwareVersion;
        private const int _minFirmwareVersion = 3;

        private void commsStarting() => MainWindowEnabled = false;


        private void enqueueDownloadCommandsForPage(Page page, bool allPages) => (page.DataContext as IConfigToolsPageViewModel)?.EnqueuePanelDownloadCommands(allPages);
        private void enqueueUploadCommandsForPage(Page page, bool allPages)   => (page.DataContext as IConfigToolsPageViewModel)?.EnqueuePanelUploadCommands(allPages);


        private bool validateFirmwareVersion(byte[] data)
            => validateFirmwareVersion(Text.Parse(data, XfpCommands.ResponseIsFirmwareVersionRequest, XfpCommands.FirmwareRevLength));


        private bool validateFirmwareVersion(object data)
        {
            try
            {
                var version = (data as Text)?.Value;

                if ((version?.Length ?? 0) > 0)
                {
                    //check version is high enough for this tool to support
                    var isValid = int.TryParse(version.Substring(0, 2), out var majorVersion) && majorVersion >= _minFirmwareVersion;

                    if (!isValid && version != _panelFirmwareVersion)
                        Application.Current.Dispatcher.Invoke(new Action(() => { errorFirmwareVersionNotSupported(version); }), DispatcherPriority.ContextIdle);

                    ////update the firmware version on the Site Config page
                    //Application.Current.Dispatcher.Invoke(new Action(() => { (_siteConfigPage.DataContext as SiteConfigViewModel).FirmwareVersion = version; }), DispatcherPriority.ContextIdle);

                    _panelFirmwareVersion = version;

                    return isValid;
                }
            }
            catch (Exception ex)
            {
                CTecUtil.Debug.WriteLine("Error validationg firmware version:\n" + ex.ToString());
            }

            return false;
        }


        private bool _firmwareError;
        public bool FirmwareError { get => _firmwareError; set { _firmwareError = value; OnPropertyChanged(); } }


        private void downloadEndedAllPages(CommsResult wasCompleted) => downloadEnded(wasCompleted, true);
        private void downloadEndedThisPage(CommsResult wasCompleted) => downloadEnded(wasCompleted, false);
        private void uploadEndedAllPages(CommsResult wasCompleted)   => uploadEnded(wasCompleted, true);
        private void uploadEndedThisPage(CommsResult wasCompleted)   => uploadEnded(wasCompleted, false);

        private void downloadEnded(CommsResult result, bool allPages)
        {
            try
            {
                var download = ((CurrentPage == _loop1Page || CurrentPage == _loop2Page ? _deviceDetailsPage.DataContext : CurrentPage.DataContext) as PageViewModelBase).PageHeader;
                var title = allPages ? Cultures.Resources.Download_System : string.Format(Cultures.Resources.Download_x, download);
                showNotification(CommsDirection.Download, result, title);

                commsEnded();

                //if (wasCompleted)
                {
                    if (allPages
                     || _currentPage == _deviceDetailsPage
                     || _currentPage == _loop1Page
                     || _currentPage == _loop2Page)
                    {
                        (_deviceDetailsPage.DataContext as IPanelToolsViewModel).PopulateView(_data);
                        (_loop1Page.DataContext as IPanelToolsViewModel)?.PopulateView(_data);
                        (_loop2Page.DataContext as IPanelToolsViewModel)?.PopulateView(_data);
                    }
                    else if (_currentPage == _setsPage)
                    {
                        (_setsPage.DataContext as IPanelToolsViewModel)?.PopulateView(_data);
                    }
                    else if (_currentPage == _siteConfigPage)
                    {
                        if (_currentPage.DataContext is SiteConfigViewModel siteConfig)
                            siteConfig.FirmwareVersion = _panelFirmwareVersion;
                    }
                    else if (_currentPage == _ceConfigPage)
                    {
                        (_ceConfigPage.DataContext as CausesAndEffectsViewModel).RefreshView();
                    }

                    OnPropertyChanged(nameof(SystemName));
                    OnPropertyChanged(nameof(PanelNumber));
                    OnPropertyChanged(nameof(IsReadOnly));
                    OnPropertyChanged(nameof(PanelNumberSet));
                    OnPropertyChanged(nameof(NumberOfPanels));
                    OnPropertyChanged(nameof(PanelNumberToolTip));
                    OnPropertyChanged(nameof(PanelCount));
                }

                SaveCopyOfCurrentData();
                SaveCopyOfDataFromFile();
            }
            catch (Exception ex)
            {
                CTecUtil.CommsLog.AddException(Cultures.Resources.Error_After_Download_End, ex, true);
            }

            if (DebugMode)
                if (CTecUtil.CommsLog.Failed)
                    ShowCommsLog(true);

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (!_data.Validate())
                    ShowValidationWindow(_currentPage.Title);
            }));
        }


        private void uploadEnded(CommsResult result, bool allPages)
        {
            try
            {
                var upload = ((CurrentPage == _loop1Page || CurrentPage == _loop2Page ? _deviceDetailsPage.DataContext : CurrentPage.DataContext) as PageViewModelBase)?.PageHeader;
                if (upload is not null)
                {
                    var desc = allPages ? Cultures.Resources.Upload_System : string.Format(Cultures.Resources.Upload_x, upload);
                    //AppNotification.Show(desc, CTecUtil.Enums.CommsResultUploadToString(result));
                    showNotification(CommsDirection.Upload, result, desc);
                }
            }
            finally
            {
                commsEnded();
            }
        }


        private void commsEnded()
        {
            try
            {
                CTecUtil.Debug.WriteLine("commsEnded()");
                normaliseData();
            }
            finally
            {
                //HeaderPanelEnabled = true;
                MainWindowEnabled = true;
                IsReadOnly = true;
                //(_currentPage.DataContext as IPanelToolsViewModel)?.RefreshView();
            }
        }


        private void showNotification(CommsDirection direction, CommsResult result, string title)
        {
            var message = direction switch
            {
                CommsDirection.Upload   => CTecUtil.Enums.CommsResultUploadToString(result),
                CommsDirection.Download => CTecUtil.Enums.CommsResultDownloadToString(result),
                _ => ""
            };

            switch (result)
            {
                case CommsResult.Ok:        Notifications.ShowSuccess(message, title); break;
                case CommsResult.Failed:    Notifications.ShowError(message, title); break;
                case CommsResult.Cancelled: Notifications.ShowWarning(message, title); break;
                default:                    Notifications.Show(message, title); break;
            }
        }

        private void normaliseData()
        {      
            if (_data.CurrentPanel.DeviceNamesConfig.DeviceNames.Count == 0)
                _data.CurrentPanel.DeviceNamesConfig = DeviceNamesConfigData.InitialisedNew();
        }
        #endregion


        #region validation
        private ValidationWindow _validationWindow;

        /// <summary>
        /// Display validation results pop-up.
        /// </summary>
        /// <param name="spotlightedPages">The currently-open app page, plus any pages associated with it, e.g. if downloaded together.<br/>
        ///                                If this is not null the error tree will expand those pages if there are any errors or warnings within.<br/>
        ///                                Set to null [default] to expand all branches.</param>
        public async void ShowValidationWindow(string spotlightPage = null)
        {
            CancellationTokenSource s_cts = new CancellationTokenSource();

            try
            {
                if (spotlightPage is null)
                    spotlightPage = NavBarItems[NavBarSelectedIndex].MenuText;

                CloseValidationWindow();
                var loopNum = CurrentPage == _deviceDetailsPage ? ((DeviceDetailsViewModel)_deviceDetailsPage.DataContext).LoopNum : 0;
                _validationWindow = new(_data, PanelNumber, loopNum, spotlightPage);

                s_cts.CancelAfter(2500);
                await Task.Run(new Action(() => { Application.Current.Dispatcher.Invoke(new Action(() => { try { _validationWindow?.Show(); } catch { } }), DispatcherPriority.Normal); }));
            }
            catch (OperationCanceledException)
            {
                CTecMessageBox.ShowOKError(Cultures.Resources.Error_Validation_Window_Timeout, Cultures.Resources.Data_Validation);
            }
            catch (Exception ex)
            {
                CTecMessageBox.ShowOKError(Cultures.Resources.Error_Validation_Window, Cultures.Resources.Data_Validation);
            }
            finally
            {
                s_cts.Dispose();
            }
        }

        public void CloseValidationWindow() => Application.Current.Dispatcher.Invoke(new Action(() => { _validationWindow?.Close(); _validationWindow = null; }));
        #endregion


        #region PanelToolsPageViewModelBase overrides
        public new bool CheckChangesAreAllowed()
        {
            if (!IsReadOnly)
                return true;
            if (CTecMessageBox.ShowYesNoQuery(Cultures.Resources.Query_Enable_Changes, Cultures.Resources.Settings_Are_Readonly) == MessageBoxResult.Yes)
                IsReadOnly = false;
            return !IsReadOnly;
        }


        public override bool IsReadOnly
        {
            get => base.IsReadOnly;
            set
            {
                base.IsReadOnly = value;

                foreach (var p in _pages)
                    if (p.DataContext is PanelToolsPageViewModelBase)
                        (p.DataContext as PanelToolsPageViewModelBase).IsReadOnly = IsReadOnly;

                OnPropertyChanged();
            }
        }
        #endregion


        #region IAppViewModel implementation
        public void SetCulture() => SetCulture(Cultures.Resources.Culture);

        public void SetCulture(CultureInfo culture)
        {
            if (culture is null)
                return;

            UIState.SetBusyState();

            MainMenuIsOpen = false;
            CloseValidationWindow();

            XfpApplicationConfig.Settings.Culture = culture.Name;
            CultureResources.ChangeCulture(culture);

            foreach (var p in _pages)
                (p?.DataContext as IAppViewModel)?.SetCulture(culture);

            _hotKeys = new();
            assignHotKeys(_mainMenu, _hotKeys);

            setNavBarText();
            SetHambMenuText();

            OnPropertyChanged(nameof(CurrentLanguageIcon));
            OnPropertyChanged(nameof(CultureName));
            OnPropertyChanged(nameof(VersionString));
            OnPropertyChanged(nameof(ToolTip_SelectCOMPort));
        }
        #endregion


        #region IPanelToolsViewModel implementation
        public void PopulateView(XfpData data)
        {
            UIState.SetBusyState();

            //if (CurrentProtocol != _data?.CurrentPanel?.Protocol)
            //    changeProtocol(data.CurrentPanel.Protocol, false);
            
            _data = data;

            IsReadOnly = true;

            foreach (var p in _pages)
            {
                (p.DataContext as PanelToolsPageViewModelBase).PanelNumber = data.CurrentPanel.PanelNumber;

                if (p == _deviceDetailsPage)
                    (p.DataContext as DeviceDetailsViewModel)?.PopulateView(_data);
                else if (p == _zonesPage)
                    (p.DataContext as ZoneConfigViewModel)?.PopulateView(_data);
                else
                    (p?.DataContext as IPanelToolsViewModel)?.PopulateView(_data);
            }

            OnPropertyChanged(nameof(SystemName));
            OnPropertyChanged(nameof(PanelNumber));
            OnPropertyChanged(nameof(IsReadOnly));
            OnPropertyChanged(nameof(PanelNumberSet));
            OnPropertyChanged(nameof(NumberOfPanels));
            OnPropertyChanged(nameof(PanelNumberToolTip));
            OnPropertyChanged(nameof(PanelCount));

            SaveCopyOfCurrentData();
        }

        public void RefreshView() => (_currentPage?.DataContext as IPanelToolsViewModel)?.RefreshView();
        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            _comPortWatcher.Dispose();
        }

        #endregion


        public void ToggleDebugMode()
        {
            DebugMode = App.DebugMode = !DebugMode;

            foreach (var p in _pages)
                (p.DataContext as PageViewModelBase).DebugMode = DebugMode;
        }
    }
}
