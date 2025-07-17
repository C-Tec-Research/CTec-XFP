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
using Xfp.UI;

namespace Xfp.DataTypes.PanelData
{
    public partial class GroupConfigData
    {
        public void Print(FlowDocument doc, XfpPanelData panelData, ref int pageNumber)
        {
            if (pageNumber++ > 1)
                PrintUtil.InsertPageBreak(doc);

            _data = panelData;

            var groupsPage = new Section();
            groupsPage.Blocks.Add(PrintUtil.PageHeader(string.Format(Cultures.Resources.Panel_x, panelData.PanelNumber) + " - " + Cultures.Resources.Nav_Group_Configuration));

            groupsPage.Blocks.Add(headerInfo());
            groupsPage.Blocks.Add(new BlockUIContainer(new TextBlock()));
            groupsPage.Blocks.Add(groupList());

            doc.Blocks.Add(groupsPage);
        }
        
        
        private XfpPanelData _data;
        private int _totalColumns = NumSounderGroups + 2;
        private static Style _alertIconStyle = Styles.AlarmIconStyle;
        private static Brush _alarmAlertFill = Styles.AlarmAlertBrush;
        private static Brush _alarmEvacFill  = Styles.AlarmEvacBrush;
        private static Brush _alarmOffFill   = Styles.AlarmOffBrush;


        private BlockUIContainer headerInfo()
        {
            var grid = new Grid();

            GridUtil.AddRowToGrid(grid);
            GridUtil.AddRowToGrid(grid);
            GridUtil.AddRowToGrid(grid);
            GridUtil.AddRowToGrid(grid);
            GridUtil.AddRowToGrid(grid);

            GridUtil.AddColumnToGrid(grid);
            GridUtil.AddColumnToGrid(grid);
            GridUtil.AddColumnToGrid(grid);
            GridUtil.AddColumnToGrid(grid);
            GridUtil.AddColumnToGrid(grid);
            GridUtil.AddColumnToGrid(grid);
            GridUtil.AddColumnToGrid(grid);

            grid.Children.Add(GridUtil.GridCell(appendColon(string.Format(Cultures.Resources.Panel_Sounder_x_Belongs_To_Sounder_Group, 1)), 0, 0, 1, 2));
            grid.Children.Add(GridUtil.GridCell(appendColon(string.Format(Cultures.Resources.Panel_Sounder_x_Belongs_To_Sounder_Group, 2)), 1, 0, 1, 2));
            grid.Children.Add(GridUtil.GridCell(PanelSounder1Group, 0, 2, 1, 2));
            grid.Children.Add(GridUtil.GridCell(PanelSounder2Group, 1, 2, 1, 2));

            grid.Children.Add(GridUtil.GridCell(" ", 0, 3));

            grid.Children.Add(GridUtil.GridCell(appendColon(Cultures.Resources.Evac_Tone),  0, 5));
            grid.Children.Add(GridUtil.GridCell(appendColon(Cultures.Resources.Alert_Tone), 1, 5));
            grid.Children.Add(GridUtil.GridCell(ContinuousTone, 0, 6));
            grid.Children.Add(GridUtil.GridCell(IntermittentTone, 1, 6));

            grid.Children.Add(GridUtil.GridCell(" ", 2, 0));

            grid.Children.Add(GridUtil.GridCell(appendColon(Cultures.Resources.New_Fire_Causes_Resound), 3, 0));
            grid.Children.Add(GridUtil.GridCell(appendColon(Cultures.Resources.Phased_Delay),            4, 0, 1, 2));
            grid.Children.Add(GridUtil.GridCellYesNo(ReSoundFunction, 3, 1, true, false));
            grid.Children.Add(GridUtil.GridCellTimeSpan(PhasedDelay, 4, 1, 1, 3, "ms", true, false, HorizontalAlignment.Left));

            return new(grid);
        }


        public BlockUIContainer groupList()
        {
            var grid = columnHeaders();

            //add the rows for the data
            for (int i = 0; i < _data.ZoneConfig.Zones.Count + _data.ZonePanelConfig.Panels.Count; i++)
                GridUtil.AddRowToGrid(grid);
                
            int row = 2;
            int col = 0;

            foreach (var z in _data.ZoneConfig.Zones)
            {
                col = 0;

                grid.Children.Add(GridUtil.GridBackground(row, 0, 1, _totalColumns, Int32.IsOddInteger(row) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));
            
                grid.Children.Add(GridUtil.GridCell(z.Number, row, col++, HorizontalAlignment.Right));
                grid.Children.Add(GridUtil.GridCell(z.Name, row, col++));

                for (int i = 0; i < NumSounderGroups; i++)
                    grid.Children.Add(GridCellSounderIcon(z.SounderGroups[i], row, col++));

                row++;
            }

            foreach (var p in _data.ZonePanelConfig.Panels)
            {
                col = 0;
                
                grid.Children.Add(GridUtil.GridBackground(row, 0, 1, _totalColumns, Int32.IsOddInteger(row) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));
                
                grid.Children.Add(GridUtil.GridCell(p.Number, row, col++, HorizontalAlignment.Right));
                grid.Children.Add(GridUtil.GridCell(p.Name, row, col++));

                for (int i = 0; i < NumSounderGroups; i++)
                    grid.Children.Add(GridCellSounderIcon(p.SounderGroups[i], row, col++));
                
                row++;
            }

            GridUtil.AddRowToGrid(grid, 10);

            return new(grid);
        }


        private Grid columnHeaders()
        {
            Grid grid = new Grid();

            //two rows for column headers
            GridUtil.AddRowToGrid(grid);
            GridUtil.AddRowToGrid(grid);

            for (int i = 0; i < _totalColumns; i++)
                GridUtil.AddColumnToGrid(grid);

            grid.Children.Add(GridUtil.GridBackground(0, 0, 1, _totalColumns, PrintUtil.GridHeaderBackground));
            grid.Children.Add(GridUtil.GridBackground(1, 0, 1, _totalColumns, PrintUtil.GridHeaderBackground));

            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Triggers_Sounder_Groups, 0, 1, 1, _totalColumns, HorizontalAlignment.Center));
            
            int col = 0;
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Zone, 1, col++, 1, 2));
            col++;

            for (int i = 0; i < NumSounderGroups; i++)
                grid.Children.Add(GridUtil.GridHeaderCell(i + 1, 1, col++, HorizontalAlignment.Center));

            return grid;
        }

        
        public static Grid GridCellSounderIcon(AlarmTypes value, int row, int column)
        {
            var result = new Grid();

            var vb = new Viewbox() { Width = 18, Height = 14, HorizontalAlignment = HorizontalAlignment.Center };

            var colour = value switch
            {
                AlarmTypes.Alert    => _alarmAlertFill,
                AlarmTypes.Evacuate => _alarmEvacFill,
                _                   => _alarmOffFill,
            };

            var p = new System.Windows.Shapes.Path() { Fill = colour, Style = _alertIconStyle };
            p.Margin = PrintUtil.DefaultGridMargin;
            vb.Child = p;

            result.SetValue(Grid.RowProperty, row);
            result.SetValue(Grid.ColumnProperty, column);
            result.Children.Add(vb);
            return result;
        }
    }
}
