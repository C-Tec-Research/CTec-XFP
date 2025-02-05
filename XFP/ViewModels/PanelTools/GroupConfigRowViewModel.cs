using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CTecControls.UI;
using CTecControls.ViewModels;
using Newtonsoft.Json.Linq;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;
using Xfp.UI.Interfaces;
using Xfp.UI.Views.PanelTools;

namespace Xfp.ViewModels.PanelTools
{
    public class GroupConfigRowViewModel : ViewModelBase, IPanelToolsViewModel
    {
        public GroupConfigRowViewModel(ZoneBase zoneData)
        {
            //_alarms = new();
            //for (int i = 0; i < GroupConfigData.NumSounderGroups; i++)
            //    _alarms.Add(new() { AlarmType = AlarmTypes.Off });
            
            _zoneData = zoneData;
            _alarms = new();
            for (int i = 0; i < _zoneData.SounderGroups.Count; i++)
                _alarms.Add(new() { IsAlarm = true, ZoneIndex = IsPanel ? Index + ZoneConfigData.NumZones : Index, AlarmIndex = i, Alarm = _zoneData.SounderGroups[i], ValueChanged = updateValue });
        }


        private ZoneBase _zoneData = null;
        //private ZoneBase ZoneData
        //{
        //    get => _zoneData;
        //    set
        //    {
        //        _zoneData = value;
        //        _alarms = new();
        //        for (int i = 0; i < _zoneData.SounderGroups.Count; i++)
        //            _alarms.Add(new() { IsAlarm = true, ZoneIndex = IsPanel ? Index + ZoneConfigData.NumZones : Index, AlarmIndex = i, Alarm = _zoneData.SounderGroups[i], ValueChanged = updateValue });
        //        RefreshView();
        //    }
        //}


        public bool   IsPanel { get => _zoneData.IsPanelData; }
        public int    Index   { get => _zoneData.Index; set { _zoneData.Index = value; OnPropertyChanged(); } }
        public int    Number  { get => _zoneData.Number; }
        //public string Name   => string.IsNullOrEmpty(_zoneData.Name) ? IsPanel ? string.Format(Cultures.Resources.Panel_x, Number) : string.Format(Cultures.Resources.Zone_x, Number) : _zoneData.Name;
        public string Name   => _zoneData.Name;

        
        private ObservableCollection<GroupConfigItemViewModel> _alarms = new();
        public ObservableCollection<GroupConfigItemViewModel> Alarms { get => _alarms; set { _alarms = value; OnPropertyChanged(); } }

        
        /// <summary>
        /// Delegate for callback to update the source data, needed because GroupConfigItemViewModel's properties are decoupled from the source data.
        /// </summary>
        public GroupConfigItemViewModel.ValueChangeNotifier ValueChanged;        
        
        /// <summary>
        /// Callback needed because GroupConfigItemViewModel's properties are decoupled from the source data.
        /// </summary>
        private void updateValue(int row, int col, AlarmTypes value) => ValueChanged?.Invoke(row, col, value);



        #region IAppViewModel implementation
        public void SetCulture(CultureInfo culture) { }
        #endregion


        #region IPanelToolsViewModel implementation
        public void PopulateView(XfpData data) { }

        public void RefreshView()
        {
            OnPropertyChanged(nameof(IsPanel));
            OnPropertyChanged(nameof(Index));
            OnPropertyChanged(nameof(Name));
            foreach (var a in Alarms)
                a.RefreshView();
        }
        #endregion
    }
}
