using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using CTecUtil.ViewModels;
using CTecControls.UI;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;
using Xfp.UI.Interfaces;

namespace Xfp.ViewModels.PanelTools
{
    public class SetConfigItemViewModel : ViewModelBase, IPanelToolsViewModel
    {
        public SetConfigItemViewModel(bool isPanelRelayTrigger) => IsPanelRelayTrigger = isPanelRelayTrigger;


        private bool _isHeaderBackground;
        private bool _isHeader;
        private bool _isName;
        private bool _isSpacer;
        private string _text;
        private SetTriggerTypes _trigger;
        private int _zoneIndex = 0;
        private int _triggerIndex = 0;
        private bool _isChecked;
        private ToggleButton _button;


        public bool IsHeaderBackground { get => _isHeaderBackground; set { _isHeaderBackground = value; OnPropertyChanged(); } }
        public bool IsHeader           { get => _isHeader;           set { _isHeader = value; OnPropertyChanged(); } }
        public bool IsName             { get => _isName;             set { _isName = value; OnPropertyChanged(); } }
        public bool IsSpacer           { get => _isSpacer;           set { _isSpacer = value; OnPropertyChanged(); } }
        public string Text             { get => _text;               set { _text = value; OnPropertyChanged(); } }
        public SetTriggerTypes Trigger { get => _trigger;            set { if (value != SetTriggerTypes.Pulsed || IsPanelRelayTrigger) { _trigger = value; ValueChanged?.Invoke(ZoneIndex, TriggerIndex, IsPanelRelayTrigger, value); OnPropertyChanged(); } } }
        public int ZoneIndex           { get => _zoneIndex;          set { _zoneIndex = value; OnPropertyChanged(); } }
        public int TriggerIndex        { get => _triggerIndex;       set { _triggerIndex = value; OnPropertyChanged(); } }
        public bool IsChecked          { get => _isChecked;          set { _isChecked = value; OnPropertyChanged(); } }
        public ToggleButton Button     { get => _button;             set { _button = value; OnPropertyChanged(); } }
        public bool IsPanelRelayTrigger { get; set; }

        
        /// <summary>
        /// Delegate for callback to update the source data, needed because SetConfigItemViewModel's properties are decoupled from the source data.
        /// </summary>
        public delegate void ValueChangeNotifier(int row, int col, bool isPanelRelay, SetTriggerTypes value);

        /// <summary>
        /// Callback needed because SetConfigItemViewModel's properties are decoupled from the source data.
        /// </summary>
        public ValueChangeNotifier ValueChanged;


        #region IAppViewModel implementation
        public void SetCulture(CultureInfo culture) { }
        #endregion


        #region IPanelToolsViewModel implementation
        public void PopulateView(XfpData data) { }

        public void RefreshView()
        {
            OnPropertyChanged(nameof(IsHeaderBackground));
            OnPropertyChanged(nameof(IsHeader));
            OnPropertyChanged(nameof(IsName));
            OnPropertyChanged(nameof(Trigger));
            OnPropertyChanged(nameof(Text));
            OnPropertyChanged(nameof(ZoneIndex));
            OnPropertyChanged(nameof(TriggerIndex));
        }
        #endregion
    }
}
