using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Xfp.DataTypes;
using Xfp.ViewModels;
using Xfp.ViewModels.PanelTools;
using CTecControls.UI;
using CTecUtil;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Services.Store;
using Windows.UI.Popups;
using System.Linq;
using System.Security.Policy;
using CTecControls.Util;
using CTecUtil.UI.Util;
using CTecUtil.Config;
using Xfp.Config;
//using Windows.UI.Core;

namespace Xfp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            EventLog.WriteInfo("Starting app");

            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            InitializeComponent();

            DataContext = _context = new MainWindowViewModel(this, hbgMainMenu, PanelControl/*, popPanelManagement*/);
            
            _context.NewLanguageSelectorContext = new((c) => popLanguageSelector.DataContext = c);
            _context.RecentFilesChanged = recentFilesChanged;

            recentFilesChanged();

            //set a delay for setting the _allowSaveWindowState flag.  This permits XfpApplicationConfig.Instance.SaveMainWindowState() to
            //be called while the window is opening and the size/location are being initialised without it messing up the settings
            System.Timers.Timer _appStartingTimer = new() { AutoReset = false, Interval = 1000, Enabled = true };
            _appStartingTimer.Elapsed += new((sender, e) => _allowSaveWindowState = true);
            _appStartingTimer.Start();

            restoreWindowState();
        }


        private bool _allowSaveWindowState = false;

        private MainWindowViewModel _context;


        private void window_Loaded(object sender, RoutedEventArgs e) => _context.CheckCommandLineArgs();


        #region menus
        private void closeMenus() => _context.ClosePopups();
        private void recentFilesChanged() { var ch = mnuRecentFiles.IsChecked; mnuRecentFiles.IsChecked = false; mnuRecentFiles.IsChecked = ch; }
        #endregion


        #region header panel actions
        private void uploadSystem_Click(object sender, RoutedEventArgs e)      => _context.UploadToPanel(true);
        private void downloadSystem_Click(object sender, RoutedEventArgs e)    => _context.DownloadFromPanel(true);
        private void uploadPage_Click(object sender, RoutedEventArgs e)        => _context.UploadToPanel(false);
        private void downloadPage_Click(object sender, RoutedEventArgs e)      => _context.DownloadFromPanel(false);
        private void commsLog_MouseDown(object sender, MouseButtonEventArgs e) => _context.ShowCommsLog(false);
        private void info_MouseDown(object sender, MouseButtonEventArgs e)     => _context.ShowValidationWindow();
        #endregion


        #region Window controls & resizing
        private void btnMinimise_Click(object sender, RoutedEventArgs e) { /*_context.ClosePopups();*/ WindowState = WindowState.Minimized; }
        private void btnMaximise_Click(object sender, RoutedEventArgs e) { /*_context.ClosePopups();*/ WindowState = WindowState.Maximized; XfpApplicationConfig.Settings.MainWindow.IsMaximised = true; /*updateWindowParams(true);*/ }
        private void btnRestore_Click(object sender, RoutedEventArgs e) { /*_context.ClosePopups(); XfpApplicationConfig.Instance.RestoreMainWindowState(this);*/ WindowState = WindowState.Normal; XfpApplicationConfig.Settings.MainWindow.IsMaximised = false; /*updateWindowParams(true);*/ }
        private void window_StateChanged(object sender, EventArgs e) { /*_context.ClosePopups();*/ _context.ChangeWindowState(WindowState); }
        private void btnExit_Click(object sender, RoutedEventArgs e) => exitApp();

        private void window_SizeChanged(object sender, SizeChangedEventArgs e) { _context.ClosePopups(); updateWindowParams(); }
        private void window_LocationChanged(object sender, EventArgs e) { _context.ClosePopups(); updateWindowParams(); }

        private void mouseLeftButtonDown_DragMove(object sender, MouseButtonEventArgs e) { try { _context.ClosePopups(); DragMove(); updateWindowParams(); } catch { } }

        /// <summary>App lost focus</summary>
        private void app_Deactivated(object sender, EventArgs e) => _context.ClosePopups();

        /// <summary>Alt-F4</summary>
        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => exitApp(e);

        private void exitApp(System.ComponentModel.CancelEventArgs e = null)
        {
            _context?.ClosePopups();

            try
            {
                if (!App.Aborting && !checkSaveChanges())
                {
                    if (e is not null)
                        e.Cancel = true;
                    return;
                }
            }
            catch { }

            EventLog.WriteInfo("Exiting app");
            _context?.ExitApp();
        }


        private void updateWindowParams(bool save = false) { if (_allowSaveWindowState) XfpApplicationConfig.Settings.UpdateMainWindowParams(this, _context.LayoutTransform.ScaleX, save); }

        private void restoreWindowState()
        {
            if (App.AnotherInstanceIsRunning)
                XfpApplicationConfig.Settings.OffsetMainWindowPosition();

            _context.ChangeWindowState(this.WindowState = WindowUtil.SetWindowDimensions(this, XfpApplicationConfig.Settings.MainWindow));
            this.BringIntoView();
        }


        #region prevent maximised window from being clipped
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            WindowUtil.PreventClipWhenMaximised(this);
        }
        #endregion


        #endregion


        #region pop-ups
        private void closeLanguage_Click(object sender, EventArgs e) => _context.CloseLanguageSelector();
        private void closeData_Click(object sender, EventArgs e)     => _context.DataPopupIsOpen = false;
        private void closeAbout_Click(object sender, EventArgs e)    => _context.AboutIsOpen = false;
        #endregion


        private void window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            debugKeyPress(e, "window_PreviewKeyDown");

            if (e.Key is Key.Escape)
            {
                if (_context.MainMenuIsOpen)
                {
                    _context.MainMenuIsOpen = false;
                    e.Handled = true;
                }
                else if (_context.ClosePopups())
                    e.Handled = true;
            }
            else
            {
                if (((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt)) != ModifierKeys.None)
                 && TextProcessing.KeyEventArgsIsPossibleMenuOption(e)
                 && _context.CheckHotKeys(e))
                {
                    e.Handled = true;
                }
                else if (_context.MainMenuIsOpen)
                {
                    MainMenu_PreviewKeyDown(sender, e);
                }
            }
        }

        private void window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                closeMenus();

                //Ctrl+mouse wheel zooms the screen
                if (e.Delta > 0)
                    _context.ZoomIn();
                else
                    _context.ZoomOut();
            }
        }

        private void window_PreviewMouseDown(object sender, MouseButtonEventArgs e) => _context.RefreshUI();


        private bool checkSaveChanges()
        {
            if (_context?.DataHasChanged == false)
                return true;

            return CTecMessageBox.ShowYesNoWarn(_context.GetUnsavedDataMessage(), Cultures.Resources.Message_Title_Settings_Not_Saved) == MessageBoxResult.Yes;
        }


        private void navBar_PreviewKeyDown(object sender, KeyEventArgs e) { if (e.Key is Key.Up or Key.Down) { _context.ChangeNavBarSelection(e.Key); e.Handled = true; } }


        private void numericValue_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = TextProcessing.StringIsNumeric(e.Text);

        private void closeZoomControl(object sender, EventArgs e) => _context.CloseZoomControl();
        private void zoomOut_MouseDown(object sender, MouseButtonEventArgs e) => _context.ZoomOut();
        private void zoomIn_MouseDown(object sender, MouseButtonEventArgs e) => _context.ZoomIn();

        private void showComboBoxDropdown(object sender, MouseButtonEventArgs e)      => _context.ShowComboBoxDropdown(sender);
        private void showPortsComboBoxDropdown(object sender, MouseButtonEventArgs e) => _context.ShowPortsComboBoxDropdown(sender);


        private void MainMenu_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            debugKeyPress(e, "MainMenu_PreviewKeyDown");

            if (e.Key is Key.Escape)
            {
                if (mnuSerialPort.IsChecked == true)
                {
                    mnuSerialPort.IsChecked = false;
                    return;
                }

                if (mnuProtocol.IsChecked == true)
                {
                    mnuProtocol.IsChecked = false;
                    return;
                }

                _context.MainMenuIsOpen = false;
                return;
            }

            if (_context.RecentFilesListAvailable && mnuRecentFilesSubmenu.IsOpen)
            {
                if (TextProcessing.KeyEventArgsIsNumeric(e))
                {
                    _context.OpenRecentFile((int.Parse(TextProcessing.KeyToString(e)) + 9) % 10);
                    e.Handled = true;
                }
            }
            //else if (TextProcessing.KeyEventArgsIsPossibleMenuOption(e))
            //{
            //    //if (_context.CheckKey(e, mnuRecentFilesSubmenu)
            //    // || _context.CheckKey(e, MainMenuView)
            //    // || _context.CheckKey(e, MainMenuSettings)
            //    // || _context.CheckKey(e, MainMenuHelp))
            //    //{
            //    //    _context.MainMenuIsOpen = false;
            //    //    e.Handled = true;
            //    //}
            //}
        }


        private void debugKeyPress(KeyEventArgs e, string header)
        {
            ////debug output for keypress
            //CTecUtil.Debug.Write(header + " Key=");
            //if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) CTecUtil.Debug.Write("Ctrl+");
            //if ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) CTecUtil.Debug.Write("Shift+");
            //if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt) CTecUtil.Debug.Write("Alt+");
            //CTecUtil.Debug.Write(TextProcessing.KeyToString(e));
            //CTecUtil.Debug.WriteLine();
        }

        //private void mnuSerialPort_MouseDown(object sender, RoutedEventArgs e)
        //{
        //    _context.ShowSerialPortPopup(((HamburgerMenuOption)sender)?.IsChecked??false);
        //    e.Handled = true;
        //}


        private void recentFile_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
                return;

            if (sender is TextBlock txtFile)
            {
                _context.FileOpen((string)txtFile.Tag);
                e.Handled = true;
            }
        }


        private void PanelManagement_PreviewMouseDown(object sender, MouseButtonEventArgs e) => _context.ShowPanelManagementWindow(this);
        private void FileNew_MouseDown(object sender, MouseButtonEventArgs e) => _context.FileNewCommand.Execute(null);
        private void FileOpen_MouseDown(object sender, MouseButtonEventArgs e) => _context.FileOpenCommand.Execute(null);
        private void FileSave_MouseDown(object sender, MouseButtonEventArgs e) => _context.FileSaveCommand.Execute(null);
        private void FileSaveAs_MouseDown(object sender, MouseButtonEventArgs e) => _context.FileSaveAsCommand.Execute(null);
        private void Print_MouseDown(object sender, MouseButtonEventArgs e) => _context.PrintCommand.Execute(null);
        //private void Print_PreviewMouseDown(object sender, MouseButtonEventArgs e) => _context.PrintCommand.Execute(null);
        //private void PrintReport_Click(object sender, RoutedEventArgs e) => _context.PrintDocument();
        //private void PrintProperties_Click(object sender, RoutedEventArgs e) => _context.PrinterProperties();
        //private void ClosePrint_Click(object sender, EventArgs e) { }// => _context.ClosePrintOption();
        //private void CancelPrint_Click(object sender, RoutedEventArgs e) { }// => _context.ClosePrintOption();
        private void ConnectSerial_MouseDown(object sender, MouseButtonEventArgs e) => _context.ConnectSerialCommand.Execute(null);
        private void DisconnectSerial_MouseDown(object sender, MouseButtonEventArgs e) => _context.DisconnectSerialCommand.Execute(null);
        private void SelectLanguage_PreviewMouseDown(object sender, MouseButtonEventArgs e) => _context.SelectLanguageCommand.Execute(null);
        private void Zoom_PreviewMouseDown(object sender, MouseButtonEventArgs e) => _context.ZoomCommand.Execute(null);
        private void Data_PreviewMouseDown(object sender, MouseButtonEventArgs e) => _context.DataCommand.Execute(null);
        private void CommsLog_PreviewMouseDown(object sender, MouseButtonEventArgs e) => _context.ShowCommsLog(false);
        private void About_PreviewMouseDown(object sender, MouseButtonEventArgs e) => _context.ShowAboutPopup();
        private void History_PreviewMouseDown(object sender, MouseButtonEventArgs e) => _context.ShowRevisionHistoryWindow();
        private void Exit_PreviewMouseDown(object sender, MouseButtonEventArgs e) => exitApp();
        private void LeftBar_PreviewMouseDown(object sender, MouseButtonEventArgs e) => closeMenus();
        private void MainPanel_PreviewMouseDown(object sender, MouseButtonEventArgs e) => closeMenus();

        private void recentFilesMenuItem_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
                return;
            _context.RemoveRecentFilesListItem((string)((MenuItem)sender)?.Tag);
        }

        private void Protocol_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //double-click opens menu + highlights the protocol submenu
            if (e.ClickCount > 1)
                _context.HighlightProtocolsMenu(mnuProtocol, mnuProtocols, mnuExit);
        }

        //private void cboPrinter_PreviewMouseDown(object sender, MouseButtonEventArgs e)   => _context.PrinterListIsOpen = true;
        //private void PrintOptions_PreviewMouseDown(object sender, MouseButtonEventArgs e) => _context.PrinterListIsOpen = false;
        //private void PrinterList_MouseUp(object sender, MouseButtonEventArgs e)           => _context.PrinterListIsOpen = false;

    }
}
