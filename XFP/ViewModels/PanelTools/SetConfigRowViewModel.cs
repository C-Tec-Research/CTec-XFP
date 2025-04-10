using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CTecUtil.ViewModels;
using CTecControls.UI;
using CTecControls.ViewModels;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;
using Xfp.UI.Interfaces;
using Xfp.UI.Views.PanelTools;
using static Xfp.ViewModels.PanelTools.SetConfigItemViewModel;

namespace Xfp.ViewModels.PanelTools
{
    public class SetConfigRowViewModel : ViewModelBase, IPanelToolsViewModel
    {
        public SetConfigRowViewModel(ZoneBase zoneData)
        {
            _zoneData = zoneData;
        }


        private ZoneBase _zoneData = null;


        private SetData _setData = new();
        public SetData SetData
        {
            get => _setData;
            set
            {
                _setData = value;
                _outputSetTriggers = new();
                _panelRelayTriggers = new();
                for (int i = 0; i < SetConfigData.NumOutputSetTriggers; i++)
                    _outputSetTriggers.Add(new(false) { ZoneIndex = IsPanel ? Index + ZoneConfigData.NumZones : Index, TriggerIndex = i, Trigger = _setData.OutputSetTriggers[i], ValueChanged = updateValue });
                for (int i = 0; i < SetConfigData.NumPanelRelayTriggers; i++)
                    _panelRelayTriggers.Add(new(true) { ZoneIndex = IsPanel ? Index + ZoneConfigData.NumZones : Index, TriggerIndex = i, Trigger = _setData.PanelRelayTriggers[i], ValueChanged = updateValue });
                RefreshView();
            }
        }


        public bool   IsPanel => _zoneData.IsPanelData;
        public int    Index   => IsPanel ? _setData.Index + ZoneConfigData.NumZones : _setData.Index;
        public int    Number  => _setData.Number;
        public string Name   => _zoneData.Name;

        
        private ObservableCollection<SetConfigItemViewModel> _outputSetTriggers  = new();
        private ObservableCollection<SetConfigItemViewModel> _panelRelayTriggers = new();
        public ObservableCollection<SetConfigItemViewModel> OutputSetTriggers  { get => _outputSetTriggers;  set { _outputSetTriggers = value; OnPropertyChanged(); } }
        public ObservableCollection<SetConfigItemViewModel> PanelRelayTriggers { get => _panelRelayTriggers; set { _panelRelayTriggers = value; OnPropertyChanged(); } }


        /// <summary>
        /// Delegate for callback to update the source data, needed because SetConfigItemViewModel's properties are decoupled from the source data.
        /// </summary>
        public ValueChangeNotifier ValueChanged;
        
        /// <summary>
        /// Callback needed because SetConfigItemViewModel's properties are decoupled from the source data.
        /// </summary>
        private void updateValue(int row, int col, bool isPanelRelay, SetTriggerTypes value) => ValueChanged?.Invoke(row, col, isPanelRelay, value);


        #region IAppViewModel implementation
        public void SetCulture(CultureInfo culture) => RefreshView();
        #endregion


        #region IPanelToolsViewModel implementation
        public void PopulateView(XfpData data) { }

        public void RefreshView()
        {
            OnPropertyChanged(nameof(IsPanel));
            OnPropertyChanged(nameof(Index));
            OnPropertyChanged(nameof(Name));
            foreach (var a in OutputSetTriggers)
                a.RefreshView();
        }
        #endregion
    }
}