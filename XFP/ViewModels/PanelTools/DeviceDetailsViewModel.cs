using System;
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CTecControls;
using CTecControls.ViewModels;
using Xfp.DataTypes.PanelData;
using Xfp.IO;
using CTecDevices.Protocol;
using Xfp.UI.Interfaces;
using Xfp.UI.Views;
using Xfp.UI.Views.PanelTools;
using CTecUtil.UI;

namespace Xfp.ViewModels.PanelTools
{
    /// <summary>
    /// Viewmodel for Devices.xaml
    /// </summary>
    public class DeviceDetailsViewModel : DevicesViewModel
    {
        public DeviceDetailsViewModel(FrameworkElement parent, DeviceInfoPanel infoPanel, DataGrid loop1Grid, DataGrid loop2Grid) : base(parent, infoPanel)
        {
            _infoPanelViewModel.DisplayShowFittedDevicesOnlyOption = true;
            _infoPanelViewModel.OnShowFittedDeviceChange = new((show) => { ShowOnlyFittedDevices = show; });
            LoopChanged += changeLoop;
        }


        private void changeLoop(int loop)
        {
            _infoPanelViewModel.PopulateView(_data);
        }


        private bool _showOnlyFittedDevices;
        public bool ShowOnlyFittedDevices
        {
            get => _showOnlyFittedDevices;
            set
            {
                _showOnlyFittedDevices = value;
                updateShowOnlyIfFittedItems();
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentLoop));
            }
        }


        private void updateShowOnlyIfFittedItems()
        {
            if (Loop1 != null)
                foreach (var d in Loop1)
                    d.ShowOnlyIfFitted = _showOnlyFittedDevices;
            if (Loop2 != null)
                foreach (var d in Loop2)
                    d.ShowOnlyIfFitted = _showOnlyFittedDevices;
        }


        public void MovePrev(DataGrid DataGrid1, DataGrid dataGrid2)
        {
            if (movePrev() is DeviceItemViewModel d)
            {
                if (IsLoop1)
                    DataGrid1.ScrollIntoView(d);
                else
                    dataGrid2.ScrollIntoView(d);
            }
        }


        public void MoveNext(DataGrid dataGrid1, DataGrid dataGrid2)
        {
            if (moveNext() is DeviceItemViewModel d)
            {
                if (IsLoop1)
                    dataGrid1.ScrollIntoView(d);
                else
                    dataGrid2.ScrollIntoView(d);
            }
        }
        
        
        public override int LoopNum
        {
            get => _loopNum;
            set
            {
                if (value != _loopNum)
                {
                    UIState.SetBusyState();
                    _loopNum = value;
                    LoopChanged?.Invoke(_loopNum);
                    _infoPanelViewModel.DeviceList = value == 2 ? _loop2SelectedItems : _loop1SelectedItems;
                    RefreshView();
                }
            }
        }


        public Visibility RemoteLEDColumnVisibility   => DeviceTypes.CurrentProtocolIsXfpApollo ? Visibility.Visible : Visibility.Collapsed;
        public Visibility BaseSounderColumnVisibility => DeviceTypes.CurrentProtocolIsXfpApollo ? Visibility.Visible : Visibility.Collapsed;



        #region ConfigToolsPageViewModelBase overrides
        public override void SetChangesAreAllowedChecker(ChangesAreAllowedChecker checker) => _infoPanelViewModel.SetChangesAreAllowedChecker(checker);
        #endregion


        #region IAppViewModel implementation
        public override void SetCulture(CultureInfo culture) { base.SetCulture(culture); PageHeader = Cultures.Resources.Nav_Device_Details; }
        #endregion


        #region IPanelToolsViewModel implementation
        public override void RefreshView()
        {
            if (_data is null)
                return;

            base.RefreshView();

            //InitGrid();

            //OnPropertyChanged(nameof(LoopNum));
            //OnPropertyChanged(nameof(IsLoop1));
            //OnPropertyChanged(nameof(CurrentLoop));
            //OnPropertyChanged(nameof(IsReadOnly));
            OnPropertyChanged(nameof(ShowOnlyFittedDevices));
            OnPropertyChanged(nameof(RemoteLEDColumnVisibility));
            OnPropertyChanged(nameof(BaseSounderColumnVisibility));

            updateShowOnlyIfFittedItems();
        }
        #endregion
    }
}
