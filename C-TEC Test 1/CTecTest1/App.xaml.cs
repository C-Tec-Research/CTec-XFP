using CTecControls.UI;
using CTecUtil;
using CTecUtil.IO;
using CTecUtil.Pipes;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Markup;
using Windows.ApplicationModel.Activation;
using WinRT;
using static CTecUtil.Pipes.PipeServer;

namespace Xfp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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
            //        MessageBox.Show("Notification click #1", "C-TEC Test App", MessageBoxButton.OK, MessageBoxImage.Information);
            //        close();
            //        return;
            //    }
            //};

            //if (ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
            //{
            //    Application.Current.Dispatcher.Invoke(new Action(() => Application.Current.MainWindow.Activate()));

            //    MessageBox.Show("Notification click #2", "C-TEC Test App", MessageBoxButton.OK, MessageBoxImage.Information);
            //    close();
            //    return;
            //}
            #endregion

            CTecUtil.UI.UIState.SetBusyState();

            //AnotherInstanceIsRunning = CTecUtil.SingletonApp.CheckForAnotherInstance();

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
