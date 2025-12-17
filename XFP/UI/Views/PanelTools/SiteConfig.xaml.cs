using CTecUtil.Utils;
using System.Windows.Controls;
using System.Windows.Input;
using Xfp.DataTypes;
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

        private void ctrl_PreviewKeyDown(object sender, KeyEventArgs e)                { if (!TextUtil.KeyIsSafeEditKey(e.Key) && !(_context.CheckChangesAreAllowed?.Invoke() ?? true)) e.Handled = true; }
        private void ctrl_PreviewMouseDown(object sender, MouseButtonEventArgs e)      { if (!_context.CheckChangesAreAllowed?.Invoke() ?? true) e.Handled = true; }
        private void ctrl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)      { if (_context.IsReadOnly) e.Handled = true; }
        private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e) { if (e.Command == ApplicationCommands.Paste) if (!(_context.CheckChangesAreAllowed?.Invoke() ?? true)) e.Handled = true; }
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
