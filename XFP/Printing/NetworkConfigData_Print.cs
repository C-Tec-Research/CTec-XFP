using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using CTecUtil.Printing;
using CTecUtil.Utils;

namespace Xfp.DataTypes.PanelData
{
    public partial class NetworkConfigData
    {
        public void GetReport(FlowDocument doc, XfpData data, int panelNumber, ref int pageNumber)
        {
            if (pageNumber++ > 1)
                PrintUtil.InsertPageBreak(doc);

            _data = data;
            _panelNumber = panelNumber;
            var sitePage = new Section();
            sitePage.Blocks.Add(PrintUtil.PageHeader(Cultures.Resources.Nav_Network_Configuration));
            //sitePage.Blocks.Add(PrintUtil.PageHeader(string.Format(Cultures.Resources.Panel_x, panelNumber) + " - " + Cultures.Resources.Nav_C_And_E_Configuration));

            sitePage.Blocks.Add(networkConfigTop());
            sitePage.Blocks.Add(new BlockUIContainer(new TextBlock()));
            sitePage.Blocks.Add(networkConfigBottom());
            doc.Blocks.Add(sitePage);
        }


        private XfpData _data;
        private int _panelNumber;
        private int _totalTopColumns = 4;
        private int _totalBottomColumns = 7;


        public BlockUIContainer networkConfigTop()
        {
            var grid = new Grid();

            for (int i = 0; i < NumPanelSettings + 1; i++)
                GridUtil.AddRowToGrid(grid);

            for (int i = 0; i < _totalTopColumns; i++)
                GridUtil.AddColumnToGrid(grid);

            grid.Children.Add(GridUtil.GridBackground(0, 0, 1, _totalTopColumns, PrintUtil.GridHeaderBackground));

            int col = 1;
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Panel_Name, 0, col++));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Fitted,     0, col++));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Location,   0, col++));

            for (int i = 0; i < NumPanelSettings; i++)
            {
                GridUtil.AddCellToGrid(grid, (i + 1).ToString(), i + 1, 0, true);
                GridUtil.AddCellToGrid(grid, _data.CurrentPanel.ZonePanelConfig.Panels[i].Name, i + 1, 1, true);
                GridUtil.AddCellToGrid(grid, GridUtil.GridCellBool(_data.CurrentPanel.NetworkConfig.RepeaterSettings.Repeaters[i].Fitted, i + 1, 2, false, true));
                GridUtil.AddCellToGrid(grid, _data.CurrentPanel.NetworkConfig.RepeaterSettings.Repeaters[i].Location, i + 1, 3, true);
            }

            //for (int i = 0; i < _data.Panels.Count; i++)
            //{
            //    XfpPanelData p;
            //    if (_data.Panels.TryGetValue(i + 1, out p))
            //    {
            //        GridUtil.AddCellToGrid(grid, GridUtil.GridCellBool(p.NetworkConfig.RepeaterSettings.Repeaters[i].Fitted, i + 2, 2, false, true));
            //        GridUtil.AddCellToGrid(grid, p.NetworkConfig.RepeaterSettings.Repeaters[i].Location, i + 2, 3, true);
            //    }
            //}

            return new(grid);
        }


        public BlockUIContainer networkConfigBottom()
        {
            var grid = new Grid();

            for (int i = 0; i < _totalBottomColumns; i++)
                GridUtil.AddRowToGrid(grid);

            for (int i = 0; i < NumPanelSettings + 1; i++)
                GridUtil.AddColumnToGrid(grid);
            
            grid.Children.Add(GridUtil.GridBackground(0, 0, 2, NumPanelSettings + 1, PrintUtil.GridHeaderBackground));

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
