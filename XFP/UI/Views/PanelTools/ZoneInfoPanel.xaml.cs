using CTecUtil.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xfp.ViewModels.PanelTools;

namespace Xfp.UI.Views.PanelTools
{
    /// <summary>
    /// Interaction logic for ZoneInfoPanel.xaml
    /// </summary>
    public partial class ZoneInfoPanel : UserControl
    {
        public ZoneInfoPanel()
        {
            InitializeComponent();
            DataContext = _context = new ZoneInfoPanelViewModel(this);
            //popDependenciesEditor.DataContext = _context;
        }


        private ZoneInfoPanelViewModel _context;


        public delegate void NextPrevHandler(/*bool isPanelGrid*/);

        /// <summary>Event raised when the Next button is clicked</summary>
        public event NextPrevHandler OnMoveNext;

        /// <summary>Event raised when the Prev button is clicked</summary>
        public event NextPrevHandler OnMovePrev;


        private void prevButton_Click(object sender, RoutedEventArgs e) => OnMovePrev?.Invoke(/*_context.IsPanelData ?? false*/);
        private void nextButton_Click(object sender, RoutedEventArgs e) => OnMoveNext?.Invoke(/*_context.IsPanelData ?? false*/);

        private void detailsPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Left or Key.Up)
            {
                OnMovePrev?.Invoke(/*_context.IsPanelData ?? false*/);
                e.Handled = true;
            }
            else if (e.Key is Key.Right or Key.Down)
            {
                OnMoveNext?.Invoke(/*_context.IsPanelData ?? false*/);
                e.Handled = true;
            }
        }


        private void comboBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)   { if (_context.CheckChangesAreAllowed?.Invoke() ?? true) _context.ShowComboBoxDropdown(sender); }
        private void comboBox_PreviewKeyDown(object sender, KeyEventArgs e)             { if (_context.CheckChangesAreAllowed?.Invoke() ?? true) _context.ShowComboBoxDropdown(sender); }
        private void zoneDesc_PreviewKeyDown(object sender, KeyEventArgs e)             { if (_context.CheckChangesAreAllowed?.Invoke() ?? true) e.Handled = true; else _context.ZoneDesc = (sender as TextBox).Text; }
        private void spinner_PreviewMouseDown(object sender, MouseButtonEventArgs e)    { if (!(_context.CheckChangesAreAllowed?.Invoke() ?? false)) e.Handled = true; }
        private void spinner_PreviewKeyDown(object sender, KeyEventArgs e)              { if (!(_context.CheckChangesAreAllowed?.Invoke() ?? false)) e.Handled = true; }
        private void checkBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)   { if (!(_context.CheckChangesAreAllowed?.Invoke() ?? false)) e.Handled = true; }
        private void checkBox_PreviewKeyDown(object sender, KeyEventArgs e)             { if (!(_context.CheckChangesAreAllowed?.Invoke() ?? false)) e.Handled = true; }
        private void textBox_PreviewKeyDown(object sender, KeyEventArgs e)              { if (!TextUtil.KeyIsSafeEditKey(e.Key) && !(_context.CheckChangesAreAllowed?.Invoke() ?? false)) e.Handled = true; }
        private void timePicker_PreviewMouseDown(object sender, MouseButtonEventArgs e) { if (!(_context.CheckChangesAreAllowed?.Invoke() ?? false)) e.Handled = true; }
        private void timePicker_PreviewKeyDown(object sender, KeyEventArgs e)           { if (!(_context.CheckChangesAreAllowed?.Invoke() ?? false)) e.Handled = true; }
        private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)  { if (e.Command == ApplicationCommands.Paste) if (!(_context.CheckChangesAreAllowed?.Invoke() ?? false)) e.Handled = true; }
        
        private void zoneDesc_KeyUp(object sender, KeyEventArgs e) => _context.ZoneDesc = (sender as TextBox).Text;

    }
}
