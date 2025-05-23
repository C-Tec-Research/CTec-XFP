using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Diagnostics;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CTecControls;
using CTecControls.Config;
using CTecControls.UI.DeviceSelector;
using CTecControls.ViewModels;
using Xfp.Cultures;
using Xfp.DataTypes.PanelData;
using CTecDevices.Protocol;
using Xfp.UI.Interfaces;
using Xfp.ViewModels;
using Xfp.ViewModels.PanelTools;

namespace Xfp.UI.Views.PanelTools
{
    public partial class DeviceOverview : Page
    {
        public DeviceOverview(int loopNum)
        {
            InitializeComponent();
            DataContext = _context = new DevicesViewModel(this, ctcInfoPanel, loopNum);
            (ctcDeviceSelector.DataContext as DeviceSelectorMenuViewModel).MenuOrientation = Orientation.Horizontal;
            ctcDeviceSelector.OnDeviceTypeChanged = new(deviceType => { _context.DeviceSelectorDeviceType = deviceType; });
            ctcDeviceSelector.SetDeviceClick      = new ((int? deviceType) => { _context.ChangeDeviceType(deviceType); });
            ctcDeviceSelector.DeleteClick         = new (() => { _context.DeleteDevices(); });
            _context.CultureChanged = ctcDeviceSelector.SetCulture;
            _context.InitMenu = initMenu;
            ctcInfoPanel.OnMovePrev += new(() => _context.MovePrev(lstDeviceList));
            ctcInfoPanel.OnMoveNext += new(() => _context.MoveNext(lstDeviceList));
        }


        private DevicesViewModel _context;


        private void initMenu(DeviceSelectorConfig.DeviceSelectorMenu config) => ctcDeviceSelector.InitMenu(config);


        private void lstDeviceList_SelectionChanged(object sender, SelectionChangedEventArgs e) => _context.ChangeSelection((sender as ListView).SelectedItems);

        private void lstDeviceList_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A && Keyboard.Modifiers == ModifierKeys.Control)
                _context.ChangeSelection((sender as ListView).SelectedItems);
        }

        private void AddLoop2_PreviewMouseDown(object sender, MouseButtonEventArgs e) { if (!(_context.CheckChangesAreAllowed?.Invoke() ?? false)) e.Handled = true; else _context.AddLoop2(); }
    }
}
