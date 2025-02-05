using System;
using System.Collections.Generic;
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
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;
using Xfp.UI.Interfaces;
using Xfp.ViewModels;
using Xfp.ViewModels.PanelTools;

namespace Xfp.UI.Views.PanelTools
{
    public partial class SiteConfig : Page
    {
        public SiteConfig()
        {
            InitializeComponent();
            DataContext = _context = new SiteConfigViewModel(this);
            _context.CultureChanged = new((C) => { tpOccupied.SetCulture(C); tpUnoccupied.SetCulture(C); });
        }


        private SiteConfigViewModel _context;

        private void ctrl_PreviewKeyDown(object sender, KeyEventArgs e)                { if (!_context.CheckChangesAreAllowed?.Invoke() ?? true) e.Handled = true; }
        private void ctrl_PreviewMouseDown(object sender, MouseButtonEventArgs e)      { if (!_context.CheckChangesAreAllowed?.Invoke() ?? true) e.Handled = true; }
        private void ctrl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)      { if (_context.IsReadOnly) e.Handled = true; }
        private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e) { if (e.Command == ApplicationCommands.Paste) if (!(_context.CheckChangesAreAllowed?.Invoke() ?? false)) e.Handled = true; }
        private void al2Code_PreviewKeyDown(object sender, KeyEventArgs e)             { if (!_context.CheckChangesAreAllowed?.Invoke() ?? true) e.Handled = true; else _context.AccessCode_PreviewKeyDown(sender as TextBox, e, AccessLevels.User); }
        private void al3Code_PreviewKeyDown(object sender, KeyEventArgs e)             { if (!_context.CheckChangesAreAllowed?.Invoke() ?? true) e.Handled = true; else _context.AccessCode_PreviewKeyDown(sender as TextBox, e, AccessLevels.Engineer); }

        private void al2Code_PreviewTextInput(object sender, TextCompositionEventArgs e) { _context.AccessCode_PreviewTextInput(sender as TextBox, e, AccessLevels.User); }
        private void al3Code_PreviewTextInput(object sender, TextCompositionEventArgs e) { _context.AccessCode_PreviewTextInput(sender as TextBox, e, AccessLevels.Engineer); }

        private void output1_PreviewMouseDown(object sender, MouseButtonEventArgs e) { if (!_context.IsReadOnly) _context.ShowComboBoxDropdown(sender); else _context.CheckChangesAreAllowed?.Invoke(); }
        private void output2_PreviewMouseDown(object sender, MouseButtonEventArgs e) { if (!_context.IsReadOnly) _context.ShowComboBoxDropdown(sender); else _context.CheckChangesAreAllowed?.Invoke(); }
        private void output3_PreviewMouseDown(object sender, MouseButtonEventArgs e) { if (!_context.IsReadOnly) _context.ShowComboBoxDropdown(sender); else _context.CheckChangesAreAllowed?.Invoke(); }
        private void output4_PreviewMouseDown(object sender, MouseButtonEventArgs e) { if (!_context.IsReadOnly) _context.ShowComboBoxDropdown(sender); else _context.CheckChangesAreAllowed?.Invoke(); }
        private void output5_PreviewMouseDown(object sender, MouseButtonEventArgs e) { if (!_context.IsReadOnly) _context.ShowComboBoxDropdown(sender); else _context.CheckChangesAreAllowed?.Invoke(); }
        private void printLogOptions_Click(object sender, MouseButtonEventArgs e)    { if (!_context.IsReadOnly) _context.ShowComboBoxDropdown(sender); else _context.CheckChangesAreAllowed?.Invoke(); }
    }
}
