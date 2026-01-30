using CTecUtil.Pipes;
using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;

namespace Xfp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        static App()
        {
            // Ensure resource parsing and any culture-sensitive conversions use invariant culture
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            // Ensure WPF's default language for FrameworkElement matches the invariant culture
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.InvariantCulture.IetfLanguageTag)));

            //initialize icon definition resources
            CTecControls.Util.IconUtilities.InitIconResources();
        }

        private PipeClient _pipeClient;

        public static string SingletonPipeName = "Xfp.App.Pipe";
        public static bool Aborting = false;


        internal static bool DebugMode { get; set; }
        internal static bool AnotherInstanceIsRunning { get; set; }


        protected override void OnStartup(StartupEventArgs e)
        {
            #region prevent starting up from a click on an app notification
            //ToastNotificationManagerCompat.OnActivated += toastArgs =>
            //{
            //    Application.Current.Dispatcher.Invoke(new Action(() => Application.Current.MainWindow.Activate()));
                
            //    if (ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
            //    {
            //        close();
            //        return;
            //    }
            //};

            //if (ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
            //{
            //    Application.Current.Dispatcher.Invoke(new Action(() => Application.Current.MainWindow.Activate()));
            //    close();
            //    return;
            //}
            #endregion

            CTecUtil.UI.UIState.SetBusyState();

            AnotherInstanceIsRunning = CTecUtil.SingletonApp.CheckForAnotherInstance();

            ////don't start up if an instance of the app is already running
            //if (CTecUtil.SingletonApp.SwitchIfAlreadyRunning())
            //{
            //    if (e.Args.Length > 0)
            //    {
            //        //if args is present treat it as an XFP file path: send this to the other app instance which can then attempt to open it
            //        List<PipeTransferData> dataToSend = new() { new(PipeTransferData.DataTypes.Path, e.Args[0])};
            //        _pipeClient = new PipeClient();
            //        //data is sent in json format
            //        _pipeClient.Send(JsonConvert.SerializeObject(dataToSend, Formatting.Indented), SingletonPipeName, 3000);
            //        _pipeClient = null;
            //    }

            //    close();
            //    return;
            //}

            base.OnStartup(e);
        }

        private void close()
        {
            Aborting = true;
            App.Current.Shutdown();
            CTecUtil.IO.SerialComms.ClosePort();
            Environment.Exit(0);
        }
    }
}
