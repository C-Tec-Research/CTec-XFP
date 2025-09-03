using System.Globalization;
using System.Windows.Controls.Primitives;
using CTecUtil.ViewModels;
using Xfp.DataTypes;
using Xfp.UI.Interfaces;

namespace Xfp.ViewModels.PanelTools
{
    public class GroupConfigItemViewModel : ViewModelBase, IPanelToolsViewModel
    {
        public GroupConfigItemViewModel()
        {
        }


        private bool _isHeaderBackground;
        private bool _isHeader;
        private bool _isName;
        private bool _isAlarm;
        private bool _isSpacer;
        private string _text;
        private AlarmTypes _alarm;
        private int _zoneIndex = 0;
        private int _alarmIndex = 0;
        private bool _isChecked;
        private ToggleButton _button;

        public bool IsHeaderBackground { get => _isHeaderBackground; set { _isHeaderBackground = value; OnPropertyChanged(); } }
        public bool IsHeader           { get => _isHeader;           set { _isHeader = value; OnPropertyChanged(); } }
        public bool IsName             { get => _isName;             set { _isName = value; OnPropertyChanged(); } }
        public bool IsAlarm            { get => _isAlarm;            set { _isAlarm = value; OnPropertyChanged(); } }
        public bool IsSpacer           { get => _isSpacer;           set { _isSpacer = value; OnPropertyChanged(); } }
        public string Text             { get => _text;               set { _text = value; OnPropertyChanged(); } }
        public AlarmTypes Alarm        { get => _alarm;              set { _alarm = value; ValueChanged?.Invoke(ZoneIndex, AlarmIndex, value); OnPropertyChanged(); } }
        public int ZoneIndex           { get => _zoneIndex;          set { _zoneIndex = value; OnPropertyChanged(); } }
        public int AlarmIndex          { get => _alarmIndex;         set { _alarmIndex = value; OnPropertyChanged(); } }
        public bool IsChecked          { get => _isChecked;          set { _isChecked = value; OnPropertyChanged(); } }
        public ToggleButton Button     { get => _button;             set { _button = value; OnPropertyChanged(); } }

        
        /// <summary>
        /// Delegate for callback to update the source data, needed because GroupConfigItemViewModel's properties are decoupled from the source data.
        /// </summary>
        public delegate void ValueChangeNotifier(int row, int col, AlarmTypes value);

        /// <summary>
        /// Callback needed because GroupConfigItemViewModel's properties are decoupled from the source data.
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
            OnPropertyChanged(nameof(IsAlarm));
            OnPropertyChanged(nameof(Alarm));
            OnPropertyChanged(nameof(Text));
            OnPropertyChanged(nameof(ZoneIndex));
            OnPropertyChanged(nameof(AlarmIndex));
        }
        #endregion
    }
}
