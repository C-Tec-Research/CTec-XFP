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
    public partial class SetsConfig : Page
    {
        public SetsConfig()
        {
            InitializeComponent();
            DataContext = _context = new SetConfigViewModel(this, grdSetsConfig, txtHeaderText);
            SetTriggerTypeToColourConverter.FindResource = new((r) => FindResource(r)); 
            _context.CultureChanged = new((c) => tpDelay.SetCulture(c));
        }


        private SetConfigViewModel _context;


        ///// <summary>
        ///// Get the viewmodel for the group to which the active datagrid row belongs
        ///// </summary>
        ///// <param name="originalSource">e.OriginalSource property of the EventArgs parameter in the caller</param>
        ///// <param name="column">The active DataGrid column</param>
        ///// <returns></returns>
        //private SetConfigItemViewModel findGroup(DependencyObject originalSource, int column) => UITools.FindDataGridRow(originalSource, column)?.DataContext as SetConfigItemViewModel;



        private void NotTriggeredButton_Click(object sender, RoutedEventArgs e) => _context.SetSelectionTo(SetTriggerTypes.NotTriggered);
        private void PulsedButton_Click(object sender, RoutedEventArgs e)       => _context.SetSelectionTo(SetTriggerTypes.Pulsed);
        private void ContinuousButton_Click(object sender, RoutedEventArgs e)   => _context.SetSelectionTo(SetTriggerTypes.Continuous);
        private void DelayedButton_Click(object sender, RoutedEventArgs e)      => _context.SetSelectionTo(SetTriggerTypes.Delayed);
        private void ClearAll_Click(object sender, RoutedEventArgs e)           => _context.SetAllTo(SetTriggerTypes.NotTriggered);

        private void grdHeaders_SizeChanged(object sender, SizeChangedEventArgs e) => _context.InitGrid();

        private void ctrl_PreviewKeyDown(object sender, KeyEventArgs e)           { if (!_context.CheckChangesAreAllowed?.Invoke() ?? true) e.Handled = true; }
        private void ctrl_PreviewMouseDown(object sender, MouseButtonEventArgs e) { if (!_context.CheckChangesAreAllowed?.Invoke() ?? true) e.Handled = true; }
        private void ctrl_PreviewMouseWheel(object sender, MouseWheelEventArgs e) { if (_context.IsReadOnly) e.Handled = true; }
        
        private void ToggleButton_PreviewMouseDown(object sender, MouseButtonEventArgs e) => _context.triggerMouseDown(sender, e);
        private void ToggleButton_MouseEnter(object sender, MouseEventArgs e)             => _context.triggerMouseEnter(sender, e);
    }
}
