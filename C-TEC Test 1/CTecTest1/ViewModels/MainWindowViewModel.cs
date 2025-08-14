using CTecControls.UI;
using CTecDevices.Protocol;
using CTecUtil;
using CTecUtil.IO;
using CTecUtil.UI;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Lifetime.Clear;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using Xfp.Config;
using Xfp.UI.Interfaces;
using Xfp.ViewModels.PanelTools;

namespace Xfp.DataTypes.PanelData { }
namespace Xfp.UI.Interfaces { }
namespace Xfp.IO { }
namespace Xfp.Printing { }
namespace Xfp.UI.Views { }


namespace Xfp.ViewModels
{
    public class MainWindowViewModel : PanelToolsPageViewModelBase, IDisposable
    {
        public MainWindowViewModel(Window window) : base(window)
        {
            XfpApplicationConfig.Settings.InitConfigSettings(SupportedApps.Test, updateFileMenuRecentFiles);

            UIState.SetBusyState();

            CTecUtil.Cultures.CultureResources.InitSupportedCultures(Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));

            ReadApplicationCfg();


            _notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.BottomRight,
                    offsetX: 5,
                    offsetY: 5);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(5),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(6));

                cfg.Dispatcher = Application.Current.Dispatcher;

                cfg.DisplayOptions.TopMost = false;
                cfg.DisplayOptions.Width = 250;
            });

            _notifier.ClearMessages(new ClearAll());
        }

        private readonly Notifier _notifier;


        #region start-up
        public void ReadApplicationCfg()
        {
            SerialComms.Settings = XfpApplicationConfig.Settings.SerialPortSettings;
            var cultureName = XfpApplicationConfig.Settings.Culture;
            DeviceTypes.CurrentProtocolType = CTecDevices.Enums.StringToObjectType(XfpApplicationConfig.Settings.Protocol);
            SetCulture(cultureName is null ? CultureInfo.CurrentCulture : CultureInfo.GetCultureInfo(cultureName));
        }
        #endregion


        private void updateFileMenuRecentFiles() { }


        #region ViewModel properties
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


        #region nofications
        public void ShowInformation(string message) => Notifications.ShowInformation(message);
        public void ShowOk(string message)          => Notifications.ShowSuccess(message);
        public void ShowWarning(string message)     => Notifications.ShowWarning(message);
        public void ShowError(string message)       => Notifications.ShowError(message);
        public void ClearAll()                      => Notifications.ClearAll();
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


        public override bool IsReadOnly => false;
        public void SetCulture() => SetCulture(Cultures.Resources.Culture);
        public void SetCulture(CultureInfo culture) { }
        public void RefreshView() { }
        public void Dispose() => CTecControls.UI.Notifications.Dispose();
    }
}
