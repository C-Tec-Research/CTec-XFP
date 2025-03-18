using System;
using System.Windows.Documents;
using CTecDevices.Protocol;
using System.Windows.Media;
using CTecUtil.Printing;
using static Xfp.DataTypes.PanelData.GroupConfigData;
using Xfp.UI.ViewHelpers;
using System.Windows;
using Xfp.UI.Views.PanelTools;
using CTecUtil;
using System.Windows.Controls;
using System.Security.Policy;
using Newtonsoft.Json.Linq;
using System.Windows.Input;
using Windows.UI.Text;
using System.Windows.Media.Media3D;

namespace Xfp.DataTypes.PanelData
{
    public partial class SetConfigData
    {
        public void Print(FlowDocument doc, XfpPanelData panelData)
        {
            _data = panelData;

            var groupsPage = new Section();
            groupsPage.Blocks.Add(PrintUtil.PageHeader(string.Format(Cultures.Resources.Panel_x, panelData.PanelNumber) + " - " + Cultures.Resources.Nav_Set_Configuration));

            groupsPage.Blocks.Add(headerInfo());
            groupsPage.Blocks.Add(new BlockUIContainer(new TextBlock()));
            groupsPage.Blocks.Add(setList());

            doc.Blocks.Add(groupsPage);
        }
        
        
        private XfpPanelData _data;
        private int _totalColumns = NumOutputSetTriggers + NumPanelRelayTriggers + 3;
        private static Style _alertIconStyle = (Style)Application.Current.FindResource("AlarmIcon");
        private static Brush _alarmAlertFill = (Brush)Application.Current.FindResource("AlarmAlertBrush");
        private static Brush _alarmEvacFill  = (Brush)Application.Current.FindResource("AlarmEvacBrush");
        private static Brush _alarmOffFill   = (Brush)Application.Current.FindResource("AlarmOffBrush");


        private BlockUIContainer headerInfo()
        {
            var grid = new Grid();

            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            grid.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Delay_Time), 0, 0));
            grid.Children.Add(PrintUtil.GridCellTimeSpan(DelayTimer, 0, 1, "ms", true, true, HorizontalAlignment.Left));

            return new(grid);
        }


        public BlockUIContainer setList()
        {
            var grid = columnHeaders();

            //add the rows for the data
            for (int i = 0; i < _data.ZoneConfig.Zones.Count + _data.ZonePanelConfig.Panels.Count; i++)
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            int row = 2;
            int col = 0;

            foreach (var z in _data.ZoneConfig.Zones)
            {
                col = 0;

                grid.Children.Add(PrintUtil.GridBackground(row, 0, 1, _totalColumns, Int32.IsOddInteger(row) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));
            
                grid.Children.Add(PrintUtil.GridCell(z.Number, row, col++, HorizontalAlignment.Right));
                grid.Children.Add(PrintUtil.GridCell(z.Name, row, col++));

                for (int i = 0; i < NumSounderGroups; i++)
                    grid.Children.Add(GridCellSounderIcon(z.SounderGroups[i], row, col++));

                row++;
            }

            foreach (var p in _data.ZonePanelConfig.Panels)
            {
                col = 0;
                
                grid.Children.Add(PrintUtil.GridBackground(row, 0, 1, _totalColumns, Int32.IsOddInteger(row) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));
                
                grid.Children.Add(PrintUtil.GridCell(p.Number, row, col++, HorizontalAlignment.Right));
                grid.Children.Add(PrintUtil.GridCell(p.Name, row, col++));

                for (int i = 0; i < NumSounderGroups; i++)
                    grid.Children.Add(GridCellSounderIcon(p.SounderGroups[i], row, col++));
                
                row++;
            }

            return new(grid);
        }


        private Grid columnHeaders()
        {
            Grid grid = new Grid();

            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            for (int i = 0; i < _totalColumns; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            grid.Children.Add(PrintUtil.GridBackground(0, 0, 1, _totalColumns, PrintUtil.GridHeaderBackground));
            grid.Children.Add(PrintUtil.GridBackground(1, 0, 1, _totalColumns, PrintUtil.GridHeaderBackground));
            grid.Children.Add(PrintUtil.GridBackground(2, 0, 1, _totalColumns, PrintUtil.GridHeaderBackground));

            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Output_Set_Triggered, 0, 2, 1, _totalColumns, HorizontalAlignment.Center));
            
            int col = 0;
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Zone, 1, col++, 1, 2));
            col++;

            for (int i = 0; i < NumSounderGroups; i++)
                grid.Children.Add(PrintUtil.GridHeaderCell(i + 1, 1, col++, HorizontalAlignment.Center));

            return grid;
        }

        
        public static Grid GridCellSounderIcon(AlarmTypes value, int row, int column)
        {
            var result = new Grid();

            var vb = new Viewbox() { Width = 12, Height = 12, HorizontalAlignment = HorizontalAlignment.Center };

            var colour = value switch
            {
                AlarmTypes.Alert    => _alarmAlertFill,
                AlarmTypes.Evacuate => _alarmEvacFill,
                _                   => _alarmOffFill,
            };

            var p = new System.Windows.Shapes.Path() { Fill = colour, Style = _alertIconStyle };
            vb.Child = p;

            result.SetValue(Grid.RowProperty, row);
            result.SetValue(Grid.ColumnProperty, column);
            result.Children.Add(vb);
            return result;
        }
    }
}
