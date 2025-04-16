using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CTecControls;
using CTecControls.UI;
using CTecControls.Util;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;
using Xfp.UI.Interfaces;
using Xfp.UI.ViewHelpers;
using Xfp.ViewModels;
using Xfp.ViewModels.PanelTools;

namespace Xfp.UI.Views.PanelTools
{
    public partial class GroupConfig : Page
    {
        public GroupConfig()
        {
            InitializeComponent();
            DataContext = _context = new GroupConfigViewModel(this, grdGroupConfig);
            _context.CultureChanged = new((c) => tpPhasedDelay.SetCulture(c));
        }


        private GroupConfigViewModel _context;


        private void AlarmOffButton_Click(object sender, RoutedEventArgs e)        { if (_context.CheckChangesAreAllowed()) _context.SetSelectionTo(AlarmTypes.Off); else e.Handled = true; }
        private void AlarmAlertButton_Click(object sender, RoutedEventArgs e)      { if (_context.CheckChangesAreAllowed()) _context.SetSelectionTo(AlarmTypes.Alert); else e.Handled = true; }
        private void AlarmEvacButton_Click(object sender, RoutedEventArgs e)       { if (_context.CheckChangesAreAllowed()) _context.SetSelectionTo(AlarmTypes.Evacuate); else e.Handled = true; }

        private void SetAllOff_Click(object sender, RoutedEventArgs e)             { if (_context.CheckChangesAreAllowed()) _context.SetAllTo(AlarmTypes.Off); else e.Handled = true; }
        private void SetAllAlert_Click(object sender, RoutedEventArgs e)           { if (_context.CheckChangesAreAllowed()) _context.SetAllTo(AlarmTypes.Alert); else e.Handled = true; }
        private void SetAllEvac_Click(object sender, RoutedEventArgs e)            { if (_context.CheckChangesAreAllowed()) _context.SetAllTo(AlarmTypes.Evacuate); else e.Handled = true; }

        private void ctrl_PreviewEvent(object sender, RoutedEventArgs e)           { if (!_context.CheckChangesAreAllowed()) e.Handled = true; }
        private void ctrl_PreviewKeyDown(object sender, KeyEventArgs e)            { if (_context.IsReadOnly) e.Handled = true; }
        private void ctrl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)  { if (_context.IsReadOnly) e.Handled = true; }
        private void combo_PreviewMouseDown(object sender, MouseButtonEventArgs e) { if (_context.CheckChangesAreAllowed?.Invoke() ?? true) _context.ShowComboBoxDropdown(sender); }

        private void ToggleButton_PreviewMouseDown(object sender, MouseButtonEventArgs e) => _context.alarmMouseDown(sender, e);
        private void ToggleButton_MouseEnter(object sender, MouseEventArgs e)             => _context.alarmMouseEnter(sender, e);
        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)              => _context.InitGrid();
    }
}
