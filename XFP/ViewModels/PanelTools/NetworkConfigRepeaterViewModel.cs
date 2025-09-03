﻿using System.Globalization;
using CTecUtil.ViewModels;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;

namespace Xfp.ViewModels.PanelTools
{
    public class NetworkConfigRepeaterViewModel : ViewModelBase
    {
        public NetworkConfigRepeaterViewModel() { }
        public NetworkConfigRepeaterViewModel(XfpData data, int index)
        {
            _data = data;
            _index = index;
            RefreshView();
        }


        private XfpData _data;
        private int _index;


        public int    Number         => _index < _data.CurrentPanel.NetworkConfig.RepeaterSettings.Repeaters.Count ? _data.CurrentPanel.NetworkConfig.RepeaterSettings.Repeaters[_index].Number : 0;
        public string Name     { get => _data.CurrentPanel.ZonePanelConfig.Panels[_index].Name;
                                 set { _data.CurrentPanel.ZonePanelConfig.Panels[_index].Name = value; OnPropertyChanged(); OnPropertyChanged(nameof(NameIsValid)); } }
        public bool   IsFitted { get => _index < _data.CurrentPanel.NetworkConfig.RepeaterSettings.Repeaters.Count ? _data.CurrentPanel.NetworkConfig.RepeaterSettings.Repeaters[_index].Fitted : false; 
                                 set { _data.CurrentPanel.NetworkConfig.RepeaterSettings.Repeaters[_index].Fitted = value; OnPropertyChanged(); } }
        public string Location { get => _index < _data.CurrentPanel.NetworkConfig.RepeaterSettings.Repeaters.Count ? _data.CurrentPanel.NetworkConfig.RepeaterSettings.Repeaters[_index].Location : ""; 
                                 set { _data.CurrentPanel.NetworkConfig.RepeaterSettings.Repeaters[_index].Location = value; OnPropertyChanged(); } }
        public int    MaxLocationLength => SiteConfigData.MaxLocationLength;

        public string DisplayName   => _data.CurrentPanel.NetworkConfig.RepeaterSettings.Repeaters[_index].DisplayName;
        public bool   NameIsValid   => string.IsNullOrEmpty(Name) || Name.Length <= MaxNameLength;
        public int    MaxNameLength => ZoneConfigData.MaxNameLength;


        #region IAppViewModel implementation
        public void SetCulture(CultureInfo culture)
        {
        }
        #endregion


        #region IPanelToolsViewModel implementation

        public void RefreshView(int errorCode = 0)
        {
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(IsFitted));
            OnPropertyChanged(nameof(Location));
            OnPropertyChanged(nameof(DisplayName));
        }
        #endregion
    }
}
