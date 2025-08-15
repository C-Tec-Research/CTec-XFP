using CTecControls.UI;
using CTecUtil;
using CTecUtil.UI.Util;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using ToastNotifications.Core;
using ToastNotifications.Lifetime.Clear;
using Xfp.Config;
using Xfp.ViewModels;

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

            DataContext = _context = new MainWindowViewModel(this);

            //restoreWindowState();
        }


        private bool _allowSaveWindowState = false;

        private MainWindowViewModel _context;


        private void window_Loaded(object sender, RoutedEventArgs e) { }
        

        private void OnUnload(object sender, RoutedEventArgs e) => _context.Dispose();


        #region Window controls & resizing
        private void btnMinimise_Click(object sender, RoutedEventArgs e) { /*_context.ClosePopups();*/ WindowState = WindowState.Minimized; }
        private void btnMaximise_Click(object sender, RoutedEventArgs e) { /*_context.ClosePopups();*/ WindowState = WindowState.Maximized; XfpApplicationConfig.Settings.MainWindow.IsMaximised = true; /*updateWindowParams(true);*/ }
        private void btnRestore_Click(object sender, RoutedEventArgs e) { /*_context.ClosePopups(); XfpApplicationConfig.Instance.RestoreMainWindowState(this);*/ WindowState = WindowState.Normal; XfpApplicationConfig.Settings.MainWindow.IsMaximised = false; /*updateWindowParams(true);*/ }
        private void window_StateChanged(object sender, EventArgs e) { /*_context.ClosePopups();*/ _context.ChangeWindowState(WindowState); }
        private void btnExit_Click(object sender, RoutedEventArgs e) => exitApp();

        private void window_SizeChanged(object sender, SizeChangedEventArgs e) { updateWindowParams(); }
        private void window_LocationChanged(object sender, EventArgs e) {updateWindowParams(); }

        private void mouseLeftButtonDown_DragMove(object sender, MouseButtonEventArgs e) { try { DragMove(); updateWindowParams(); } catch { } }

        /// <summary>App lost focus</summary>
        private void app_Deactivated(object sender, EventArgs e) { }

        /// <summary>Alt-F4</summary>
        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => exitApp(e);

        private void exitApp(System.ComponentModel.CancelEventArgs e = null)
        {
            XfpApplicationConfig.Settings.UpdateMainWindowParams(this, true);
            EventLog.WriteInfo("Exiting app");
            _context?.ExitApp();
        }


        private void updateWindowParams() { if (_allowSaveWindowState) XfpApplicationConfig.Settings.UpdateMainWindowParams(this, _context.LayoutTransform.ScaleX); }

        private void restoreWindowState()
        {
            if (App.AnotherInstanceIsRunning)
                XfpApplicationConfig.Settings.OffsetMainWindowPosition();

            if (XfpApplicationConfig.Settings.MainWindow.Location is null)
                this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            else
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


        #region notification.wpf
        private void Info_Click(object sender, RoutedEventArgs e)    => _context.ShowInformation("Information");
        private void Success_Click(object sender, RoutedEventArgs e) => _context.ShowOk("Ok");
        private void Warn_Click(object sender, RoutedEventArgs e)    => _context.ShowWarning("Warning");
        private void Error_Click(object sender, RoutedEventArgs e)   => _context.ShowError("Error");
        private void Clear_Click(object sender, RoutedEventArgs e)   => _context.ClearAll();


        #endregion

    }
}
