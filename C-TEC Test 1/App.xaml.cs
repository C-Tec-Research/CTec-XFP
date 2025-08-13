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

            //    Aborting = true;
            //    App.Current.Shutdown();
            //    CTecUtil.IO.SerialComms.ClosePort();
            //    Environment.Exit(0);
            //    return;
            //}

            base.OnStartup(e);
        }
    }
}
