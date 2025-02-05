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
using Xfp.DataTypes.PanelData;
using Xfp.UI.Interfaces;
using Xfp.ViewModels;
using Xfp.ViewModels.PanelTools;

namespace Xfp.UI.Views.PanelTools
{
    public partial class ZoneConfig : Page
    {
        public ZoneConfig()
        {
            InitializeComponent();
            DataContext = _context = new ZoneConfigViewModel(this, ctcInfoPanel);
            ctcInfoPanel.OnMovePrev += new(() => _context.MovePrev(grdZoneConfig));
            ctcInfoPanel.OnMoveNext += new(() => _context.MoveNext(grdZoneConfig));
            _context.CultureChanged = new((c) => { tpInputDelay.SetCulture(c); tpInvestigationPeriod.SetCulture(c); } );
        }


        private ZoneConfigViewModel _context;


        private void grdZoneConfig_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is DataGrid grid)
            {
                //adjust the above-grid header widths to match their respective columns
                var widthLeft  = 0.0;
                var widthDelay = 0.0;
                var widthFunc  = 0.0;
                var widthMult  = 0.0;
                var widthDay   = 0.0;
                var widthNight = 0.0;
                double  heightNormal = 0.0;
                double? heightMult;

                var col = 0;
                for (; col < 2; col++)
                {
                    widthLeft += grid.Columns[col].ActualWidth;
                    heightNormal = Math.Max(heightNormal, (grid.Columns[col].Header as TextBlock).ActualHeight);
                }

                for (; col < 6; col++)
                    widthDelay += grid.Columns[col].ActualWidth;
                for (; col < 8; col++)
                    widthFunc += grid.Columns[col].ActualWidth;

                widthMult = grid.Columns[col].ActualWidth;
                heightMult = (grid.Columns[col++].Header as TextBlock)?.ActualHeight;

                for (; col < 12; col++)
                    widthDay += grid.Columns[col].ActualWidth;
                for (; col < grid.Columns.Count; col++)
                    widthNight += grid.Columns[col].ActualWidth;

                _context.ZonesLeftHeaderWidth = widthLeft;
                _context.ZonesOutputDelaysHeaderWidth = widthDelay;
                _context.ZonesFunctioningWithHeaderWidth = widthFunc;
                _context.ZonesMiddleHeaderWidth = widthMult;
                _context.ZonesDayDependenciesHeaderWidth = widthDay;
                _context.ZonesNightDependenciesHeaderWidth = widthNight;
                _context.ZonesNormalHeaderHeight = heightNormal;
                _context.ZonesMultHeaderHeight = (heightMult ?? 53) - heightNormal + 10;
            }
        }


        private void ctrl_PreviewKeyDown(object sender, KeyEventArgs e) { if (!_context.CheckChangesAreAllowed?.Invoke() ?? true) e.Handled = true; }
        private void ctrl_PreviewMouseDown(object sender, MouseButtonEventArgs e) { if (!_context.CheckChangesAreAllowed?.Invoke() ?? true) e.Handled = true; }
        private void ctrl_PreviewMouseWheel(object sender, MouseWheelEventArgs e) { if (_context.IsReadOnly) e.Handled = true; }


        private void Page_KeyDown(object sender, KeyEventArgs e) { }

        private void ZoneGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)  { if (sender is DataGrid grid) { _context.ChangeZoneSelection(grid.SelectedItems); } }

        private void ZoneGrid_GotFocus(object sender, RoutedEventArgs e) { if (sender is DataGrid grid) { var si = grid.SelectedItems; } }
    }
}
