using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using CTecUtil;
using CTecUtil.Pipes;
using CTecUtil.IO;
using Newtonsoft.Json;
using static CTecUtil.Pipes.PipeServer;
using CTecControls.UI;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.ApplicationModel.Activation;

namespace Xfp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private PipeClient _pipeClient;

        public static string SingletonPipeName = "Xfp.App.Pipe";
        public static bool Aborting = false;


        internal static bool DebugMode { get; set; }
        internal static bool AnotherInstanceIsRunning { get; set; }


        protected override void OnStartup(StartupEventArgs e)
        {
            #region prevent starting up from a click on an app notification
            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                if (ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
                {
                    //app instance was launched by an app notification 
                    close();
                    return;
                }
            };

            if (ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
            {
                close();
                return;
            }
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
