using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Xps;
using CTecControls.UI;
using CTecControls.ViewModels;
using CTecDevices.Protocol;
using CTecUtil;
using CTecUtil.StandardPanelDataTypes;
using CTecUtil.UI;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;
using Xfp.IO;
using Xfp.UI.Interfaces;
using Xfp.UI.Views;
using static Xfp.IO.PanelComms;

namespace Xfp.ViewModels.PanelTools
{
    class SiteConfigViewModel : PanelToolsPageViewModelBase, IPanelToolsViewModel, IConfigToolsPageViewModel
    {
        public SiteConfigViewModel(FrameworkElement parent) : base(parent)
        {
            updateTime();
        }


        public delegate void SystemNameChangedHandler(string siteName);
        public SystemNameChangedHandler SystemNameChanged;


        private bool _sendMaintToPanel;
        private bool _syncPanelTime = true;


        public string SystemName             { get => _data?.SiteConfig.SystemName;         set { if (_data != null) { _data.SiteConfig.SystemName = value?.TrimEnd(); SystemNameChanged?.Invoke(value); OnPropertyChanged(); } } }
        public string ClientName             { get => _data?.SiteConfig.Client.Name;        set { if (_data != null) { _data.SiteConfig.Client.Name = value; OnPropertyChanged(); } } }
        public List<string> ClientAddress    { get => _data?.SiteConfig.Client.Address;     set { if (_data != null) { _data.SiteConfig.Client.Address = value; OnPropertyChanged(); } } }
        public string ClientPostcode         { get => _data?.SiteConfig.Client.Postcode;    set { if (_data != null) { _data.SiteConfig.Client.Postcode = value; OnPropertyChanged(); } } }
        public string ClientTelephone        { get => _data?.SiteConfig.ClientTel;          set { if (_data != null) { _data.SiteConfig.ClientTel = value; OnPropertyChanged(); } } }
        public DateTime? InstallDate         { get => _data?.SiteConfig.InstallDate;        set { if (_data != null) { _data.SiteConfig.InstallDate = value; OnPropertyChanged(); } } }
        public DateTime? CommissionDate      { get => _data?.SiteConfig.CommissionDate;     set { if (_data != null) { _data.SiteConfig.CommissionDate = value; OnPropertyChanged(); } } }
        public string InstallerName          { get => _data?.SiteConfig.Installer.Name;     set { if (_data != null) { _data.SiteConfig.Installer.Name = value; OnPropertyChanged(); } } }
        public List<string> InstallerAddress { get => _data?.SiteConfig.Installer.Address;  set { if (_data != null) { _data.SiteConfig.Installer.Address = value; OnPropertyChanged(); } } }
        public string InstallerPostcode      { get => _data?.SiteConfig.Installer.Postcode; set { if (_data != null) { _data.SiteConfig.Installer.Postcode = value; OnPropertyChanged(); } } }
        public string EngineerTelephone      { get => _data?.SiteConfig.InstallerTel;       set { if (_data != null) { _data.SiteConfig.InstallerTel = value; OnPropertyChanged(); } } }
        public string EngineerName           { get => _data?.SiteConfig.EngineerName;       set { if (_data != null) { _data.SiteConfig.EngineerName = value; OnPropertyChanged(); } } }
        //public string PanelLocation          { get => _data?.SiteConfig.PanelLocation;      set { if (_data != null) { _data.SiteConfig.PanelLocation = value; OnPropertyChanged(); } } }
        public string FirmwareVersion        { get => _data?.FirmwareVersion;               set { if (_data != null) { _data.FirmwareVersion = value; OnPropertyChanged(); } } }

        public string QuiescentString        { get => _data?.SiteConfig.QuiescentString;    set { if (_data is not null) { _data.SiteConfig.QuiescentString = value; OnPropertyChanged(); OnPropertyChanged(nameof(QuiescentStringIsValid)); } } }
        public string MaintenanceString      { get => _data?.SiteConfig.MaintenanceString;  set { if (_data is not null) { _data.SiteConfig.MaintenanceString = value; OnPropertyChanged(); OnPropertyChanged(nameof(MaintenanceStringIsValid)); } } }
        public DateTime? MaintenanceDate     { get => _data?.SiteConfig.MaintenanceDate;    set { if (_data is not null) { _data.SiteConfig.MaintenanceDate = value; OnPropertyChanged(); } } }
        public bool SendMaintToPanel         { get => _sendMaintToPanel;                    set { _sendMaintToPanel = value; OnPropertyChanged(); } }

        public TimeSpan OccupiedBegins       { get => _data?.SiteConfig.OccupiedBegins ?? new(); set { if (_data is not null) { _data.SiteConfig.OccupiedBegins = value; OnPropertyChanged(); } } }
        public TimeSpan OccupiedEnds         { get => _data?.SiteConfig.OccupiedEnds ?? new();   set { if (_data is not null) { _data.SiteConfig.OccupiedEnds = value; OnPropertyChanged(); } } }

        public List<bool> DayStart           { get => _data?.SiteConfig.DayStart;           set { if (_data is not null) { _data.SiteConfig.DayStart  = value; OnPropertyChanged(); } } }
        public List<bool> NightStart         { get => _data?.SiteConfig.NightStart;         set { if (_data is not null) { _data.SiteConfig.NightStart = value; OnPropertyChanged(); } } }

        public string AL2Code { get => _data.SiteConfig.AL2Code; set { _data.SiteConfig.AL2Code = value; OnPropertyChanged(); OnPropertyChanged(nameof(AL2CodeIsValid)); } }
        public string AL3Code { get => _data.SiteConfig.AL3Code; set { _data.SiteConfig.AL3Code = value; OnPropertyChanged(); OnPropertyChanged(nameof(AL3CodeIsValid)); } }

        public bool ShowRecalibrationTime => !DeviceTypes.CurrentProtocolIsXfpCast;
        public TimeSpan RecalibrationTime { get => _data?.SiteConfig.RecalibrationTime ?? new();   set { if (_data is not null) { _data.SiteConfig.RecalibrationTime = value; OnPropertyChanged(); } } }
        public bool RealTimeEventOutput   { get => _data?.SiteConfig.RealTimeEventOutput ?? new(); set { if (_data is not null) { _data.SiteConfig.RealTimeEventOutput = value; OnPropertyChanged(); } } }
        public bool BlinkPollingLED       { get => _data?.SiteConfig.BlinkPollingLED ?? new();     set { if (_data is not null) { _data.SiteConfig.BlinkPollingLED = value; OnPropertyChanged(); } } }

        public bool SyncPanelTime { get => _syncPanelTime;                           set { _syncPanelTime = value; OnPropertyChanged(); } }
        public bool AutoAdjustDST { get => _data?.SiteConfig.AutoAdjustDST ?? false; set { if (_data is not null) { _data.SiteConfig.AutoAdjustDST = value; OnPropertyChanged(); } } }


        private string _currentTime;
        public string CurrentTime { get => _currentTime; set { _currentTime = value; OnPropertyChanged(nameof(CurrentTime), false); } }

        private async void updateTime()
        {
            CurrentTime = DateTime.Now.ToString("G");
            await Task.Delay(1000);
            updateTime();
        }


        public bool AL2CodeIsValid           => validateAccessCode(AL2Code) && AL2Code.Length == SiteConfigData.AccessCodeLength;
        public bool AL3CodeIsValid           => validateAccessCode(AL3Code) && AL3Code.Length == SiteConfigData.AccessCodeLength;
        public bool QuiescentStringIsValid   => QuiescentString?.Length   < SiteConfigData.MaxQuiescentStringLength;
        public bool MaintenanceStringIsValid => MaintenanceString?.Length < SiteConfigData.MaxMaintenanceStringLength;


        #region input validation
        public void AccessCode_PreviewKeyDown(TextBox textBox, KeyEventArgs e, AccessLevels level)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;       // <-- the Space char isn't isn't passed into PreviewTextInput, so we prevent it here
            }
            else if (new Key[] { Key.Delete, Key.Back }.Contains(e.Key))
            {
                int newSelectionStart;
                switch (level)
                {
                    case AccessLevels.User:     AL2Code = previewKeyDownHandler(textBox, e, out newSelectionStart);    break;
                    case AccessLevels.Engineer: AL3Code = previewKeyDownHandler(textBox, e, out newSelectionStart);    break;
                    default: newSelectionStart = textBox.Text.Length; break;
                }
                textBox.SelectionStart = newSelectionStart;
            }
        }


        private InputTextValidator validateAccessCode = new((text) => SiteConfigData.ValidateAccessCodeChars(text));

        public void AccessCode_PreviewTextInput(TextBox textBox, TextCompositionEventArgs e, AccessLevels level)
        {
            int newSelectionStart;
            switch (level)
            {
                case AccessLevels.User:     AL2Code = previewTextInputHandlerWithValidation(textBox, e, validateAccessCode, out newSelectionStart);    break;
                case AccessLevels.Engineer: AL3Code = previewTextInputHandlerWithValidation(textBox, e, validateAccessCode, out newSelectionStart);    break;
                default: newSelectionStart = textBox.Text.Length; break;
            }
            textBox.SelectionStart = newSelectionStart;
        }
        #endregion


        #region comboboxes
        //private ObservableCollection<string> _outputStatesOptionsList;
        //private ObservableCollection<string> _printLogOptionsList;
        //public ObservableCollection<string> OutputStateOptionsList { get => _outputStatesOptionsList; }
        //public ObservableCollection<string> PrintLogOptionsList    { get => _printLogOptionsList; }
        
        private void initComboLists()
        {
        }
        #endregion

        
        #region IAppViewModel implementation
        public void SetCulture(CultureInfo culture)
        {
            PageHeader = Cultures.Resources.Nav_Site_Configuration;

            initComboLists();

            //force update of calendar controls' text (month names & watermark)
            var saveI = InstallDate;
            var saveC = CommissionDate;
            var saveR = RecalibrationTime;
            var saveB = OccupiedBegins;
            var saveE = OccupiedEnds;
            InstallDate = CommissionDate = DateTime.Today;
            RecalibrationTime = OccupiedBegins = OccupiedEnds = TimeSpan.Zero;
            InstallDate = saveI;
            CommissionDate = saveC;
            RecalibrationTime = saveR;
            OccupiedBegins = saveB;
            OccupiedEnds = saveE;

            RefreshView();

            CultureChanged?.Invoke(culture);
        }
        #endregion


        #region IPanelToolsViewModel implementation
        public void PopulateView(XfpData data)
        {
            if (data is null)
                return;

            _data = data;
            RefreshView();
        }

        public void RefreshView()
        {
            if (_data is null)
                return;

            OnPropertyChanged(nameof(ShowRecalibrationTime));
            OnPropertyChanged(nameof(SystemName));
            OnPropertyChanged(nameof(ClientName));
            OnPropertyChanged(nameof(ClientAddress));
            OnPropertyChanged(nameof(ClientPostcode));
            OnPropertyChanged(nameof(ClientTelephone));
            OnPropertyChanged(nameof(InstallerName));
            OnPropertyChanged(nameof(InstallerAddress));
            OnPropertyChanged(nameof(InstallerPostcode));
            OnPropertyChanged(nameof(EngineerTelephone));
            OnPropertyChanged(nameof(EngineerName));
            //OnPropertyChanged(nameof(PanelLocation));
            OnPropertyChanged(nameof(QuiescentString));
            OnPropertyChanged(nameof(MaintenanceString));
            OnPropertyChanged(nameof(MaintenanceDate));
            OnPropertyChanged(nameof(AL2Code));
            OnPropertyChanged(nameof(AL3Code));
            OnPropertyChanged(nameof(OccupiedBegins));
            OnPropertyChanged(nameof(OccupiedEnds));
            OnPropertyChanged(nameof(DayStart));
            OnPropertyChanged(nameof(NightStart));
            OnPropertyChanged(nameof(RecalibrationTime));
            OnPropertyChanged(nameof(RealTimeEventOutput));
            OnPropertyChanged(nameof(BlinkPollingLED));
            OnPropertyChanged(nameof(SyncPanelTime));
            OnPropertyChanged(nameof(AutoAdjustDST));
            OnPropertyChanged(nameof(IsReadOnly));
            OnPropertyChanged(nameof(AL2CodeIsValid));
            OnPropertyChanged(nameof(AL3CodeIsValid));
            OnPropertyChanged(nameof(QuiescentStringIsValid));
            OnPropertyChanged(nameof(MaintenanceStringIsValid));
        }
        #endregion


        #region IConfigToolsPageViewModel implementation
        public void EnqueuePanelDownloadCommands(bool allPages)
        {
            PanelComms.QuiescentStringReceived   = quiescentStringReceived;
            PanelComms.MaintenanceStringReceived = maintenanceStringReceived;
            PanelComms.MaintenanceDateReceived   = maintenanceDateReceived;
            PanelComms.AL2CodeReceived  = al2CodeReceived;
            PanelComms.AL3CodeReceived  = al3CodeReceived;
            PanelComms.DayNightReceived = dayNightReceived;

            PanelComms.InitNewDownloadCommandSubqueue(Cultures.Resources.Comms_Site_Information, downloadRequestsCompleted);
            PanelComms.AddCommandRequestQuiescentString(Cultures.Resources.Normal_String);
            PanelComms.AddCommandRequestMaintenanceString(Cultures.Resources.Maintenance_String);
            PanelComms.AddCommandRequestMaintenanceDate(Cultures.Resources.Maintenance_Date);
            PanelComms.AddCommandRequestAL2Code(Cultures.Resources.AL2_Code);
            PanelComms.AddCommandRequestAL3Code(Cultures.Resources.AL3_Code);
            PanelComms.AddCommandRequestDayNight(Cultures.Resources.Day_And_Night_Settings);
        }

        public void EnqueuePanelUploadCommands(bool allPages)
        {
            PanelComms.InitNewUploadCommandSubqueue(Cultures.Resources.Comms_Site_Information, uploadRequestsCompleted);
            PanelComms.AddCommandSetQuiescentString(new(QuiescentString, SiteConfigData.MaxQuiescentStringLength), Cultures.Resources.Normal_String);
            PanelComms.AddCommandSetMaintenanceString(new(MaintenanceString, SiteConfigData.MaxMaintenanceStringLength), Cultures.Resources.Maintenance_String);
            if (SendMaintToPanel && MaintenanceDate is not null)
                PanelComms.AddCommandSetMaintenanceDate(new((DateTime)MaintenanceDate), Cultures.Resources.Maintenance_Date);
            PanelComms.AddCommandSetAL2Code(new(AL2Code, SiteConfigData.AccessCodeLength), Cultures.Resources.AL2_Code);
            PanelComms.AddCommandSetAL3Code(new() { AL3Code = AL3Code, 
                                                    BlinkPollingLED = BlinkPollingLED, 
                                                    DetectorDebounce = _data.SiteConfig.DetectorDebounce, 
                                                    IODebounce = _data.SiteConfig.IODebounce, 
                                                    MCPDebounce = _data.SiteConfig.MCPDebounce }, 
                                            Cultures.Resources.AL3_Code);
            PanelComms.AddCommandSetDayNight(new() { DayModeStart = OccupiedBegins, 
                                                     NightModeStart = OccupiedEnds, 
                                                     DayFlags = DayStart, 
                                                     NightFlags = NightStart, 
                                                     AutoAdjustDST = AutoAdjustDST, 
                                                     RealTimeEventOutput = RealTimeEventOutput, 
                                                     RecalibrateTime = RecalibrationTime },
                                             Cultures.Resources.Day_And_Night_Settings);
            if (SyncPanelTime)
                PanelComms.AddCommandSyncPanelTime(Cultures.Resources.Send_Time_To_Panel);
        }
        

        public bool DataEquals(XfpData otherData) => _data.SiteConfig.Equals(otherData.SiteConfig);


        public bool HasErrorsOrWarnings() => _data.SiteConfig.HasErrorsOrWarnings();
        public bool HasErrors()           => _data.SiteConfig.HasErrors();
        public bool HasWarnings()         => _data.SiteConfig.HasWarnings();
        #endregion


        #region Panel comms receive-data handlers
        private bool quiescentStringReceived(object data)
        {
            if (data is not Text code) return false;
            CTecUtil.CommsLog.AddReceivedData(Cultures.Resources.Normal_String);
            QuiescentString = code.Value;
            return true;
        }

        private bool maintenanceStringReceived(object data)
        {
            if (data is not Text code) return false;
            CTecUtil.CommsLog.AddReceivedData(Cultures.Resources.Maintenance_String);
            MaintenanceString = code.Value;
            return true;
        }

        private bool maintenanceDateReceived(object data)
        {
            if (data is not Date code) return false;
            CTecUtil.CommsLog.AddReceivedData(Cultures.Resources.Maintenance_Date);
            MaintenanceDate = code.Value;
            return true;
        }

        private bool al2CodeReceived(object data)
        {
            if (data is not string code) return false;
            CTecUtil.CommsLog.AddReceivedData(Cultures.Resources.AL2_Code);
            AL2Code = code;
            return true;
        }

        private bool al3CodeReceived(object data)
        {
            if (data is not SiteConfigData.AL3CodeBundle al3Bundle)
                return false;
            CTecUtil.CommsLog.AddReceivedData(Cultures.Resources.AL3_Code);
            AL3Code         = al3Bundle.AL3Code;
            BlinkPollingLED = al3Bundle.BlinkPollingLED;
            _data.SiteConfig.MCPDebounce      = al3Bundle.MCPDebounce;
            _data.SiteConfig.IODebounce       = al3Bundle.IODebounce;
            _data.SiteConfig.DetectorDebounce = al3Bundle.DetectorDebounce;
            return true;
        }

        private bool dayNightReceived(object data)
        {
            if (data is not SiteConfigData.DayNightBundle dnBundle)
                return false;
            CTecUtil.CommsLog.AddReceivedData(Cultures.Resources.Day_And_Night_Settings);
            OccupiedBegins      = dnBundle.DayModeStart;
            OccupiedEnds        = dnBundle.NightModeStart;
            RecalibrationTime   = dnBundle.RecalibrateTime;
            DayStart            = dnBundle.DayFlags;
            NightStart          = dnBundle.NightFlags;
            AutoAdjustDST       = dnBundle.AutoAdjustDST;
            RealTimeEventOutput = dnBundle.RealTimeEventOutput;
            return true;
        }

        private void downloadRequestsCompleted() { }
        private void uploadRequestsCompleted() { }
        #endregion


        #region printing
        public override bool PrintPage(XpsDocumentWriter documentWriter)
        {
            return true;
        }
        #endregion
    }
}
