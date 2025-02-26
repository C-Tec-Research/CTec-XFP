using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CTecControls.UI;
using CTecControls.ViewModels;
using CTecUtil;
using CTecUtil.IO;
using CTecUtil.StandardPanelDataTypes;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;
using Xfp.IO;
using Xfp.UI.Interfaces;


namespace Xfp.ViewModels.PanelTools
{
    public class EventLogViewerViewModel : PanelToolsPageViewModelBase, IPanelToolsViewModel
    {
        public EventLogViewerViewModel(FrameworkElement parent) : base(parent)
        {
        }


        public delegate void DataReceivedNotifier(string logText);
        public delegate void TextClearer();
        public DataReceivedNotifier DataReceived;
        public TextClearer ClearText;

        private SerialComms.ConnectionStatus _commStatus;
        private bool _isActivePage;
        private bool _eraseAfter;
        private bool _isLogging;
        private bool _isPaused;
        private bool isLogging          { get => _isLogging;  set { _isLogging = value; isPaused = false; RefreshView(); } }
        private bool isPaused           { get => _isPaused;   set { if (!(_isPaused = value) && isLogging) enqueueEventLogRequest(); RefreshView(); } }
        private bool eraseAfter         { get => _eraseAfter; set { _eraseAfter = value; OnPropertyChanged(); } }
        private bool isLoggingOrPaused => isLogging || isPaused;
        private bool isPausedOrStopped => isPaused || !isLogging;
        private bool isConnected       => CommsStatus == SerialComms.ConnectionStatus.ConnectedReadOnly || CommsStatus == SerialComms.ConnectionStatus.ConnectedWriteable;

        public SerialComms.ConnectionStatus CommsStatus
        {
            get => _commStatus; 
            set
            {
                _commStatus = value;

                if (_commStatus == SerialComms.ConnectionStatus.Disconnected)
                    if (isLogging)
                        isPaused = true;

                //if (_commStatus == SerialComms.ConnectionStatus.ConnectedWriteable 
                // || _commStatus == SerialComms.ConnectionStatus.ConnectedReadOnly)
                //{
                //  *** don't auto-restart because that interrupts pinging and we end up with the ConnectedReadonly icon showing
                //    //if (isLogging)
                //    //    isPaused = false;
                //}
                RefreshView(); 
            }
        }


        public bool IsActivePage
        {
            get => _isActivePage;
            set 
            {
                _isActivePage = value;

                if (isLogging)
                {
                    if (_isActivePage)
                        enqueueEventLogRequest();
                    else
                        _isPaused = true;
                }
                RefreshView();
            }
        }

        public bool IsStartVisible    => !IsStopAvailable;
        public bool IsStartAvailable  => !isLoggingOrPaused;
        public bool IsPauseAvailable  => !isPausedOrStopped;
        public bool IsResumeAvailable => isPaused;
        public bool IsStopAvailable   => isLoggingOrPaused;
        public bool IsResetAvailable  => !isLoggingOrPaused && _eventLogIndex > 0;
        public bool IsClearAvailable  => (LogText?.Length ?? 0) > 0;
        public bool IsSaveAvailable   => isPausedOrStopped && IsClearAvailable;
        public bool IsOpenAvailable   => !isLogging;




        private string _logText;
        public string LogText
        {
            get => _logText;
            set
            {
                _logText = value;
                Application.Current.Dispatcher.Invoke(new Action(() => ClearText?.Invoke()));
                Application.Current.Dispatcher.Invoke(new Action(() => DataReceived?.Invoke(_logText)));
                OnPropertyChanged();
            }
        }

        private string CurrentFolder { get; set; }


        public void Start()
        {
            eraseAfter = CTecMessageBox.ShowYesNoQuery(Cultures.Resources.Erase_Event_Log_After_Reading, Cultures.Resources.Nav_Event_Log) == MessageBoxResult.Yes;
            _eventLogIndex = 0; 
            isLogging = true;
            enqueueEventLogRequest();
        }

    
        public void Pause()  => isPaused = true;
        public void Resume() => isPaused = false;
        public void Stop()   => isLogging = false;
        //public void ResetIndex() { _eventLogIndex = 0; RefreshView(); }
        public void Clear() { ClearText?.Invoke(); _logText = ""; RefreshView(); }

        public void OpenFromFile()
        {
            TextFile.FilePath = CurrentFolder;
            TextFile.Filter = Cultures.Resources.XFP_Log_Files + " (*" + CTecUtil.IO.TextFile.LogFileExt + ")|*" + CTecUtil.IO.TextFile.LogFileExt;
            if (TextFile.OpenFile())
            {
                LogText = File.ReadAllText(TextFile.FilePath);
                _eventLogIndex = 0;
                RefreshView();
            }
        }

        public void SaveToFile()
        {
            TextFile.FilePath = CurrentFolder;
            TextFile.Filter = Cultures.Resources.XFP_Log_Files + " (*" + CTecUtil.IO.TextFile.LogFileExt + ")|*" + CTecUtil.IO.TextFile.LogFileExt;
            TextFile.SaveFileAs(LogText);
            CurrentFolder = System.IO.Path.GetDirectoryName(TextFile.FilePath);
        }


        private int _eventLogIndex = 0;

        private void enqueueEventLogRequest()
        {
            //CTecUtil.UI.UIState.SetBusyState();
            PanelComms.EventLogDataReceived = eventLogDataReceived;
            PanelComms.InitCommandQueue(CommsDirection.Download, Cultures.Resources.Nav_Event_Log);
            PanelComms.InitNewDownloadCommandSubqueue(Cultures.Resources.Nav_Event_Log, null);
            PanelComms.AddCommandRequestEventLog(_eventLogIndex, eraseAfter);
        }


        #region IAppViewModel implementation
        public void SetCulture(CultureInfo culture) => PageHeader = Cultures.Resources.Nav_Event_Log;
        #endregion


        #region IPanelToolsViewModel implementation
        public void PopulateView(XfpData data) { }

        public void RefreshView() 
        {
            OnPropertyChanged(nameof(isLogging));
            OnPropertyChanged(nameof(isPaused));
            OnPropertyChanged(nameof(IsActivePage));
            OnPropertyChanged(nameof(isLoggingOrPaused));
            OnPropertyChanged(nameof(isPausedOrStopped));
            OnPropertyChanged(nameof(IsStartVisible)); 
            OnPropertyChanged(nameof(IsStartAvailable)); 
            OnPropertyChanged(nameof(IsPauseAvailable)); 
            OnPropertyChanged(nameof(IsResumeAvailable)); 
            OnPropertyChanged(nameof(IsStopAvailable)); 
            OnPropertyChanged(nameof(IsResetAvailable)); 
            OnPropertyChanged(nameof(IsClearAvailable)); 
            OnPropertyChanged(nameof(IsSaveAvailable)); 
            OnPropertyChanged(nameof(IsOpenAvailable)); 
            OnPropertyChanged(nameof(LogText));
        }
        #endregion


        private bool eventLogDataReceived(object data)
        { 
            if (data is not Text eventText) return false;

            if (!string.IsNullOrWhiteSpace(eventText.Value))
            {
                _logText += eventText.Value + Environment.NewLine;
                Application.Current.Dispatcher.Invoke(new Action(() => DataReceived?.Invoke(eventText.Value + "\r")));

                if (IsActivePage && !isPausedOrStopped)
                {
                    _eventLogIndex++;
                    enqueueEventLogRequest();
                }
            }
            //else
            //{
            //    _eventLogIndex = 0;
            //}

            return true;
        }
    }
}
