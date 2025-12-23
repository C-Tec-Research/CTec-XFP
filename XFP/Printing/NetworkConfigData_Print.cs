using CTecControls.UI;
using CTecUtil.Printing;
using CTecUtil.Utils;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Xfp.DataTypes.PanelData
{
    public partial class NetworkConfigData
    {
        public void GetReport(FlowDocument doc, XfpData data, int panelNumber)
        {
            _data = data;
            _panelNumber = panelNumber;

            PrintUtil.PageHeader(doc, Cultures.Resources.Nav_Network_Configuration);

            var nwSect = new Section();

            var table = TableUtil.NewTable(Cultures.Resources.Nav_Network_Configuration);
            table.Columns.Add(new TableColumn() { Width = new GridLength(1d, GridUnitType.Auto) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(1d, GridUnitType.Auto) });
            
            var headerRow = new TableRow();
            var dataRow   = new TableRow();

            //TableUtil.SetPadding(new(0, 0, 0, 10));
            //var headerCell = TableUtil.NewCell(Cultures.Resources.Nav_Network_Configuration, 1, 2, TextAlignment.Left, FontWeights.Bold, PrintUtil.PrintPageHeaderFontSize, Styles.Brush00, FontStyles.Normal, new(PrintUtil.PrintHeaderFont));
            //headerRow.Cells.Add(headerCell);
            //TableUtil.ResetDefaults();
            //headerRow.Cells.Add(TableUtil.NewCell("", 1, 1));

            var leftGrid = networkConfigTop();
            dataRow.Cells.Add(new TableCell(leftGrid));

            var rightGrid = networkConfigBottom();
            dataRow.Cells.Add(new TableCell(rightGrid));
            
            dataRow.Cells.Add(TableUtil.NewCell("", 1, 1));
                        
            var nwGroup = new TableRowGroup();
            nwGroup.Rows.Add(headerRow);
            nwGroup.Rows.Add(dataRow);
            table.RowGroups.Add(nwGroup);

            nwSect.Blocks.Add(table);
            doc.Blocks.Add(nwSect);
        }


        private XfpData _data;
        private int _panelNumber;
        private int _totalTopColumns = 4;
        private int _totalBottomColumns = 7;


        public BlockUIContainer networkConfigTop()
        {
            var grid = new Grid();

            for (int i = 0; i < NumPanelSettings + 2; i++)
                GridUtil.AddRowToGrid(grid);

            for (int i = 0; i < _totalTopColumns; i++)
                GridUtil.AddColumnToGrid(grid);

            grid.Children.Add(GridUtil.GridBackground(1, 0, 1, _totalTopColumns, PrintUtil.TableHeaderBackground));

            int col = 1;
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Panel_Name, 1, col++));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Fitted,     1, col++));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Location,   1, col++));

            for (int i = 0; i < NumPanelSettings; i++)
            {
                GridUtil.AddCellToGrid(grid, (i + 1).ToString(), i + 2, 0);
                GridUtil.AddCellToGrid(grid, _data.CurrentPanel.ZonePanelConfig.Panels[i].Name, i + 2, 1);
                GridUtil.AddCellToGrid(grid, GridUtil.GridCellBool(_data.CurrentPanel.NetworkConfig.RepeaterSettings.Repeaters[i].Fitted, i + 2, 2, false, false));

                if (_data.CurrentPanel.NetworkConfig.RepeaterSettings.Repeaters[i].Fitted && string.IsNullOrWhiteSpace(_data.CurrentPanel.NetworkConfig.RepeaterSettings.Repeaters[i].Location))
                    GridUtil.AddCellToGrid(grid, "", i + 2, 3, false, true);
                else
                    GridUtil.AddCellToGrid(grid, _data.CurrentPanel.NetworkConfig.RepeaterSettings.Repeaters[i].Location, i + 2, 3, false);
            }

            return new(grid);
        }


        public BlockUIContainer networkConfigBottom()
        {
            var grid = new Grid();

            for (int i = 0; i < _totalBottomColumns; i++)
                GridUtil.AddRowToGrid(grid);

            for (int i = 0; i < NumPanelSettings + 1; i++)
                GridUtil.AddColumnToGrid(grid);
            
            grid.Children.Add(GridUtil.GridBackground(0, 0, 2, NumPanelSettings + 1, PrintUtil.TableHeaderBackground));

            var panelHeader = GridUtil.GridHeaderCell(string.Format(Cultures.Resources.Panel_x, _data.CurrentPanel.PanelNumber), 0, 0, 2, 1);
            panelHeader.SetValue(Grid.VerticalAlignmentProperty, VerticalAlignment.Center);
            grid.Children.Add(panelHeader);

            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Accepts_From_Panels, 0, 1, 1, NumPanelSettings));

            for (int i = 0; i < NumPanelSettings; i++)
                GridUtil.AddCellToGrid(grid, (i + 1).ToString(), 1, i + 1, HorizontalAlignment.Center, false);

            int row = 2;
            GridUtil.AddCellToGrid(grid, Cultures.Resources.Accepts_Faults,              row++, 0, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, Cultures.Resources.Accepts_Alarms,              row++, 0, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, Cultures.Resources.Accepts_Controls,            row++, 0, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, Cultures.Resources.Accepts_Disablements,        row++, 0, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, Cultures.Resources.Accepts_Occupied_Unoccupied, row++, 0, HorizontalAlignment.Right, false);

            for (int i = 0; i < NumPanelSettings; i++)
            {
                //XfpPanelData p;
                //if (_data.Panels.TryGetValue(i + 1, out p))
                {
                    int col = i + 1;
                    if (col == _panelNumber)
                    {
                        for (int j = 0; j < 5; j++)
                            GridUtil.AddCellToGrid(grid, "-", j + 2, col, HorizontalAlignment.Center, true);
                    }
                    else
                    {
                        row = 2;
                        GridUtil.AddCellToGrid(grid, GridUtil.GridCellBool(_data.CurrentPanel.NetworkConfig.PanelSettings[i].AcceptFaults,       row++, col, 1, 1, true, true, HorizontalAlignment.Center));
                        GridUtil.AddCellToGrid(grid, GridUtil.GridCellBool(_data.CurrentPanel.NetworkConfig.PanelSettings[i].AcceptAlarms,       row++, col, 1, 1, true, true, HorizontalAlignment.Center));
                        GridUtil.AddCellToGrid(grid, GridUtil.GridCellBool(_data.CurrentPanel.NetworkConfig.PanelSettings[i].AcceptControls,     row++, col, 1, 1, true, true, HorizontalAlignment.Center));
                        GridUtil.AddCellToGrid(grid, GridUtil.GridCellBool(_data.CurrentPanel.NetworkConfig.PanelSettings[i].AcceptDisablements, row++, col, 1, 1, true, true, HorizontalAlignment.Center));
                        GridUtil.AddCellToGrid(grid, GridUtil.GridCellBool(_data.CurrentPanel.NetworkConfig.PanelSettings[i].AcceptOccupied,     row++, col, 1, 1, true, true, HorizontalAlignment.Center));
                    }
                }
            }

            GridUtil.AddRowToGrid(grid, 10);

            return new(grid);
        }
    }
}
