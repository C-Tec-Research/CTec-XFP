using CTecControls.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xfp.DataTypes.PanelData;
using Xfp.DataTypes;

namespace Xfp.ViewModels.PanelTools
{
    public class NetworkConfigPanelSettingsViewModel : ViewModelBase
    {
        public NetworkConfigPanelSettingsViewModel() { }
        public NetworkConfigPanelSettingsViewModel(XfpData data, int index)
        {
            _data = data;
            _index = index;
            RefreshView();
        }


        private XfpData _data;
        private int _index;


        public bool AcceptFaults   { get =>_index < _data.CurrentPanel.NetworkConfig.PanelSettings.Count ? _data.CurrentPanel.NetworkConfig.PanelSettings[_index].AcceptFaults : false; 
                                     set { _data.CurrentPanel.NetworkConfig.PanelSettings[_index].AcceptFaults = value; OnPropertyChanged(); } }
        public bool AcceptAlarms   { get =>_index < _data.CurrentPanel.NetworkConfig.PanelSettings.Count ? _data.CurrentPanel.NetworkConfig.PanelSettings[_index].AcceptAlarms : false; 
                                     set { _data.CurrentPanel.NetworkConfig.PanelSettings[_index].AcceptAlarms = value; OnPropertyChanged(); } }
        public bool AcceptControls { get =>_index < _data.CurrentPanel.NetworkConfig.PanelSettings.Count ? _data.CurrentPanel.NetworkConfig.PanelSettings[_index].AcceptControls : false; 
                                     set { _data.CurrentPanel.NetworkConfig.PanelSettings[_index].AcceptControls = value; OnPropertyChanged(); } }
        public bool AcceptDisablements   { get =>_index < _data.CurrentPanel.NetworkConfig.PanelSettings.Count ? _data.CurrentPanel.NetworkConfig.PanelSettings[_index].AcceptDisablements : false; 
                                     set { _data.CurrentPanel.NetworkConfig.PanelSettings[_index].AcceptDisablements = value; OnPropertyChanged(); } }
        public bool AcceptOccupied { get =>_index < _data.CurrentPanel.NetworkConfig.PanelSettings.Count ? _data.CurrentPanel.NetworkConfig.PanelSettings[_index].AcceptOccupied : false; 
                                     set { _data.CurrentPanel.NetworkConfig.PanelSettings[_index].AcceptOccupied = value; OnPropertyChanged(); } }

        #region IAppViewModel implementation
        public void SetCulture(CultureInfo culture) { }
        #endregion


        #region IPanelToolsViewModel implementation

        public void RefreshView(int errorCode = 0)
        {
            OnPropertyChanged(nameof(AcceptFaults));
            OnPropertyChanged(nameof(AcceptAlarms));
            OnPropertyChanged(nameof(AcceptControls));
            OnPropertyChanged(nameof(AcceptDisablements));
            OnPropertyChanged(nameof(AcceptOccupied));
        }
        #endregion
    }
}
