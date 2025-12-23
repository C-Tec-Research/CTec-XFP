using System.Windows.Controls;
using System.Windows.Input;
using CTecControls.Config;
using CTecControls.ViewModels;
using Xfp.ViewModels.PanelTools;

namespace Xfp.UI.Views.PanelTools
{
    /// <summary>
    /// Interaction logic for DeviceDetails.xaml
    /// </summary>
    public partial class DeviceDetails : Page
    {
        public DeviceDetails()
        {
            InitializeComponent();
            //Loop2Context = new(this, ctcInfoPanel);
            //Loop1Context.LoopChanged = loopChanged;
            //Loop2Context.LoopChanged = loopChanged;
            DataContext = _context = new DeviceDetailsViewModel(this, ctcInfoPanel);
            (ctcDeviceSelector.DataContext as DeviceSelectorMenuViewModel).MenuOrientation = Orientation.Horizontal;
            ctcDeviceSelector.OnDeviceTypeChanged = new (deviceType => { _context.DeviceSelectorDeviceType = deviceType; });
            ctcDeviceSelector.SetDeviceClick      = new ((int? deviceType) => { _context.ChangeDeviceType(deviceType); });
            ctcDeviceSelector.DeleteClick         = new (() => { _context.DeleteDevices(); });
            _context.CultureChanged = ctcDeviceSelector.SetCulture;
            _context.InitMenu = initMenu;
            _context.LoopChanged = loopChanged;
            ctcInfoPanel.OnMovePrev += new(() => _context.MovePrev(grdDeviceSummaryLoop1, grdDeviceSummaryLoop2));
            ctcInfoPanel.OnMoveNext += new(() => _context.MoveNext(grdDeviceSummaryLoop1, grdDeviceSummaryLoop2));
        }


        //private DeviceDetailsViewModel context
        //{
        //    get => DataContext as DeviceDetailsViewModel;
        //    set
        //    {
        //        if (DataContext is not null)
        //        {
        //            ctcInfoPanel.OnMovePrev -= movePrev;
        //            ctcInfoPanel.OnMoveNext -= moveNext;
        //        }

        //        DataContext = value;

        //        ctcInfoPanel.OnMovePrev += movePrev;
        //        ctcInfoPanel.OnMoveNext += moveNext; 
        //    }
        //}

        private DeviceDetailsViewModel _context;

        //private void movePrev() => grdDeviceSummaryLoop1.ScrollIntoView(_context.MovePrev());
        //private void moveNext() => grdDeviceSummaryLoop1.ScrollIntoView(_context.MoveNext());


        //internal DeviceDetailsViewModel Loop1Context;
        //internal DeviceDetailsViewModel Loop2Context;


        private void loopChanged() {  }// => _context = loop == 2 ? Loop2Context : Loop1Context;


        private void initMenu(DeviceSelectorConfig.DeviceSelectorMenu config) => ctcDeviceSelector.InitMenu(config);


        private void grdDeviceDetails_SelectionChanged(object sender, SelectionChangedEventArgs e) => _context.ChangeSelection((sender as DataGrid).SelectedItems);

        private void grdDeviceDetails_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A && Keyboard.Modifiers == ModifierKeys.Control)
                _context.ChangeSelection((sender as DataGrid).SelectedItems);
            else if (e.Key == Key.Delete)
            {
                if (_context.CheckChangesAreAllowed())
                    _context.DeleteDevices();
            }
        }

        private void AddLoop2_PreviewMouseDown(object sender, MouseButtonEventArgs e) { if (!(_context.CheckChangesAreAllowed?.Invoke() ?? false)) e.Handled = true; else _context.AddLoop2(); }
    }
}
