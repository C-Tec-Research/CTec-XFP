using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xfp.ViewModels.PanelTools;

namespace Xfp.UI.Views.PanelTools
{
    public partial class CausesAndEffects : Page
    {
        public CausesAndEffects()
        {
            InitializeComponent();
            DataContext = _context = new CausesAndEffectsViewModel(this);
            _context.CultureChanged = new((c) =>
            {
                tp1.SetCulture(c); tp5.SetCulture(c); tp9.SetCulture(c);  tp13.SetCulture(c); 
                tp2.SetCulture(c); tp6.SetCulture(c); tp10.SetCulture(c); tp14.SetCulture(c);
                tp3.SetCulture(c); tp7.SetCulture(c); tp11.SetCulture(c); tp15.SetCulture(c);
                tp4.SetCulture(c); tp8.SetCulture(c); tp12.SetCulture(c); tp16.SetCulture(c);
            });
        }


        private CausesAndEffectsViewModel _context;

        private void ctrl_PreviewKeyDown(object sender, KeyEventArgs e)            { if (!_context.CheckChangesAreAllowed?.Invoke() ?? true) e.Handled = true; }
        private void ctrl_PreviewMouseDown(object sender, MouseButtonEventArgs e)  { if (!_context.CheckChangesAreAllowed?.Invoke() ?? true) e.Handled = true; }
        private void combo_PreviewMouseDown(object sender, MouseButtonEventArgs e) { if (_context.CheckChangesAreAllowed?.Invoke() ?? true) _context.ShowComboBoxDropdown(sender); }
        private void ctrl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)  { if (_context.IsReadOnly) e.Handled = true; }

        private void grdZoneConfig_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is DataGrid grid)
            {
                _context.DataGridWidth = grid.ActualWidth;

                //find the column widths so that the above-grid headers can be aligned
                _context.ColumnWidths = new();
                for (int i = 0; i < grid.Columns.Count; i++)
                    _context.ColumnWidths.Add(grid.Columns[i].ActualWidth);
                _context.RefreshView();
            }
        }
    }
}
