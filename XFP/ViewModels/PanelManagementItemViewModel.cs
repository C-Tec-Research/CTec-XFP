using System.Collections.Generic;
using System.Globalization;
using CTecUtil.ViewModels;
using Xfp.DataTypes;
using Xfp.UI.Interfaces;

namespace Xfp.ViewModels
{
    public class PanelManagementItemViewModel : ViewModelBase, IPanelToolsViewModel
    {
        public PanelManagementItemViewModel(int number)
        {
            _number = number;
            RefreshView();
        }

        public delegate void      StatusChangeNotifier();
        public delegate List<int> PanelNumberSetGetter();
        public delegate int       TotalFittedGetter();

        public StatusChangeNotifier StatusChanged;
        public PanelNumberSetGetter GetPanelNumberSet;
        public TotalFittedGetter    GetTotalFitted;
        
        private int _number;

        public int       Number         => _number;
        public string    Name           => string.Format(Cultures.Resources.Panel_x, _number);
        public List<int> PanelNumberSet => GetPanelNumberSet?.Invoke()??new List<int>();
        public int       TotalFitted    => GetTotalFitted?.Invoke()??0;
        public bool      IsFitted       => (GetPanelNumberSet?.Invoke()??new List<int>()).Contains(_number);


        #region IAppViewModel implementation
        public void SetCulture(CultureInfo culture) => RefreshView();
        #endregion


        #region IPanelToolsViewModel implementation
        public void PopulateView(XfpData data) { }

        public void RefreshView()
        {
            OnPropertyChanged(nameof(Number));
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(IsFitted));
            OnPropertyChanged(nameof(PanelNumberSet));
            OnPropertyChanged(nameof(TotalFitted));
        }

        public void LeavingPage() { }
        #endregion
    }
}