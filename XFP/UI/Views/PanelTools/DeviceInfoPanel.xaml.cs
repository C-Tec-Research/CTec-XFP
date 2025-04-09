using CTecControls.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
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
using WinRT;
using Xfp.DataTypes.PanelData;
using Xfp.UI.Interfaces;
using Xfp.ViewModels;
using Xfp.ViewModels.PanelTools;

namespace Xfp.UI.Views.PanelTools
{
    /// <summary>
    /// Interaction logic for DeviceInfoPanel.xaml
    /// </summary>
    public partial class DeviceInfoPanel : UserControl
    {
        public DeviceInfoPanel()
        {
            InitializeComponent();
            DataContext = _context = new DeviceInfoPanelViewModel(this);
        }


        private DeviceInfoPanelViewModel _context;


        public delegate void NextPrevHandler();

        /// <summary>Event raised when the Next button is clicked</summary>
        public event NextPrevHandler OnMoveNext;

        /// <summary>Event raised when the Prev button is clicked</summary>
        public event NextPrevHandler OnMovePrev;


        private void prevButton_Click(object sender, RoutedEventArgs e) => OnMovePrev?.Invoke();
        private void nextButton_Click(object sender, RoutedEventArgs e) => OnMoveNext?.Invoke();

        private void detailsPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Left or Key.Up)
            {
                OnMovePrev?.Invoke();
                e.Handled = true;
            }
            else if (e.Key is Key.Right or Key.Down)
            {
                OnMoveNext?.Invoke();
                e.Handled = true;
            }
        }


        private void deviceType_MouseDown(object sender, MouseButtonEventArgs e)       { if (_context.CheckChangesAreAllowed?.Invoke() ?? true) _context.ChangeDeviceType(); }
        private void comboBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)  { if (_context.CheckChangesAreAllowed?.Invoke() ?? true) _context.ShowComboBoxDropdown(sender); }
        private void comboBox_PreviewKeyDown(object sender, KeyEventArgs e)            { if (_context.CheckChangesAreAllowed?.Invoke() ?? true) _context.ShowComboBoxDropdown(sender); }
        private void showComboBoxDropdown(object sender, EventArgs e)                  { if (_context.CheckChangesAreAllowed?.Invoke() ?? true) _context.ShowComboBoxDropdown(sender); }
        private void spinner_PreviewMouseDown(object sender, MouseButtonEventArgs e)   { if (!(_context.CheckChangesAreAllowed?.Invoke() ?? false)) e.Handled = true; }
        private void spinner_PreviewKeyDown(object sender, KeyEventArgs e)             { if (!(_context.CheckChangesAreAllowed?.Invoke() ?? false)) e.Handled = true; }
        private void checkBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)  { if (!(_context.CheckChangesAreAllowed?.Invoke() ?? false)) e.Handled = true; }
        private void checkBox_PreviewKeyDown(object sender, KeyEventArgs e)            { if (!(_context.CheckChangesAreAllowed?.Invoke() ?? false)) e.Handled = true; }
        private void textBox_PreviewKeyDown(object sender, KeyEventArgs e)             { if (CTecUtil.TextProcessing.KeyEventArgsIsAlphaNumeric(e)) if (!(_context.CheckChangesAreAllowed?.Invoke() ?? false)) e.Handled = true; }
        private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e) { if (e.Command == ApplicationCommands.Paste) if (!(_context.CheckChangesAreAllowed?.Invoke() ?? false)) e.Handled = true; }
        
        private void infoPanel_KeyDown(object sender, KeyEventArgs e) { }

        private void deviceName_KeyUp(object sender, KeyEventArgs e) => _context.DeviceName = (sender as TextBox).Text;
        private void ioDescription1_KeyUp(object sender, KeyEventArgs e) => deviceName_KeyUp(sender, e);
        private void ioDescription2_KeyUp(object sender, KeyEventArgs e) => _context.IODescription2 = (sender as TextBox).Text;
        private void ioDescription3_KeyUp(object sender, KeyEventArgs e) => _context.IODescription3 = (sender as TextBox).Text;
        private void ioDescription4_KeyUp(object sender, KeyEventArgs e) => _context.IODescription4 = (sender as TextBox).Text;
    }
}
