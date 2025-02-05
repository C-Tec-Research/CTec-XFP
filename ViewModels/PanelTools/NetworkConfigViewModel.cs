using CTecUtil.StandardPanelDataTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;
using Xfp.IO;
using Xfp.UI.Interfaces;
using Xfp.UI.Views.PanelTools;
using Xfp.ViewModels.PanelTools;

namespace Xfp.ViewModels.PanelTools
{
    public class NetworkConfigViewModel : PanelToolsPageViewModelBase, IPanelToolsViewModel, IConfigToolsPageViewModel
    {
        public NetworkConfigViewModel(FrameworkElement parent) : base(parent) { }


        private ObservableCollection<NetworkConfigRepeaterViewModel>     _repeaters;
        public ObservableCollection<NetworkConfigPanelSettingsViewModel> _panelSettings;


        public ObservableCollection<NetworkConfigRepeaterViewModel>      Repeaters     => _repeaters;
        public ObservableCollection<NetworkConfigPanelSettingsViewModel> PanelSettings => _panelSettings;


        #region IAppViewModel implementation
        public void SetCulture(CultureInfo culture)
        {
            PageHeader = Cultures.Resources.Nav_Network_Configuration;
        }
        #endregion


        #region IPanelToolsViewModel implementation
        public void PopulateView(XfpData data)
        {
            if (data is null)
                return;

            _data = data;

            _repeaters = new();
            _panelSettings = new();

            for (int i = 0; i < ZonePanelConfigData.NumZonePanels; i++)
            {
                NetworkConfigRepeaterViewModel repeater = new(_data, i);
                NetworkConfigPanelSettingsViewModel panel = new(_data, i);
                _repeaters.Add(repeater);
                _panelSettings.Add(panel);

                //_panelSettings.Add(new(_data.CurrentPanel.NetworkConfig.PanelSettings[i]));
            }

            RefreshView();
        }

        public void RefreshView()
        {
            if (_data is null)
                return;

            //Validator.IsValid(Parent);

            OnPropertyChanged(nameof(Repeaters));
            OnPropertyChanged(nameof(PanelSettings));
        }
        #endregion


        #region IConfigToolsPageViewModel implementation
        public void EnqueuePanelDownloadCommands(bool allPages)
        {
            PanelComms.RepeaterNameReceived     = repeaterNameReceived;
            PanelComms.NetworkPanelDataReceived = networkPanelDataReceived;
            PanelComms.InitNewDownloadCommandSubqueue(Cultures.Resources.Nav_Network_Configuration, downloadRequestsCompleted);
            for (int i = 0; i < ZoneConfigData.NumPanels; i++)
                PanelComms.AddCommandRequestRepeaterName(i, String.Format(Cultures.Resources.Panel_Name_x, i + 1));
            PanelComms.AddCommandRequestNetworkPanelData(Cultures.Resources.Network_Panel_Data);
        }

        public void EnqueuePanelUploadCommands(bool allPages)
        {
            PanelComms.InitNewUploadCommandSubqueue(Cultures.Resources.Nav_Network_Configuration, uploadRequestsCompleted);
            for (int i = 0; i < ZoneConfigData.NumPanels; i++)
                PanelComms.AddCommandSetRepeaterName(new(i + 1, Repeaters[i].Name, ZoneConfigData.MaxNameLength), string.Format(Cultures.Resources.Panel_Name_x_Value_y, i, Repeaters[i].Name));
            var reps = new List<bool>();
            foreach (var r in Repeaters)
                reps.Add(r.IsFitted);
            var pans = new List<NetworkPanelSettingsData>();
            foreach (var p in PanelSettings)
                pans.Add(new() { AcceptAlarms = p.AcceptAlarms, AcceptFaults = p.AcceptFaults, AcceptControls = p.AcceptControls, AcceptDisablements = p.AcceptDisablements, AcceptOccupied = p.AcceptOccupied });
            PanelComms.AddCommandSetNetworkPanelData(new() { PanelSettings = pans, RepeatersFitted = reps }, Cultures.Resources.Network_Panel_Data);
        }


        public bool DataEquals(XfpData otherData) => _data.CurrentPanel.NetworkConfig.Equals(otherData.CurrentPanel.NetworkConfig);


        public bool HasErrorsOrWarnings() => _data.CurrentPanel.GroupConfig.HasErrorsOrWarnings();
        public bool HasErrors() => _data.CurrentPanel.GroupConfig.HasErrors();
        public bool HasWarnings() => _data.CurrentPanel.GroupConfig.HasWarnings();
        #endregion
        

        #region Panel comms receive-data handlers
        private bool repeaterNameReceived(object data)
        {
            if (data is not IndexedText code) return false;
            CTecUtil.CommsLog.AddReceivedData(string.Format(Cultures.Resources.Panel_Name_x_Value_y, code.Index + 1, code.Value));
            Repeaters[code.Index].Name = code.Value;
            return true;
        }
        
        private bool networkPanelDataReceived(object data)
        {
            if (data is not NetworkConfigData.NetworkBundle settings) return false;
            CTecUtil.CommsLog.AddReceivedData(Cultures.Resources.Network_Panel_Data);
            for (int i = 0; i < settings.RepeatersFitted.Count; i++)
            {
                Repeaters[i].IsFitted               = settings.RepeatersFitted[i];
                PanelSettings[i].AcceptFaults       = settings.PanelSettings[i].AcceptFaults;
                PanelSettings[i].AcceptAlarms       = settings.PanelSettings[i].AcceptAlarms;
                PanelSettings[i].AcceptControls     = settings.PanelSettings[i].AcceptControls;
                PanelSettings[i].AcceptDisablements = settings.PanelSettings[i].AcceptDisablements;
                PanelSettings[i].AcceptOccupied     = settings.PanelSettings[i].AcceptOccupied;
            }
            return true;
        }

        private void downloadRequestsCompleted() { }
        private void uploadRequestsCompleted() { }
        #endregion
    }
}
