using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using CTecUtil.Printing;
using System.Collections.Generic;

namespace Xfp.DataTypes.PanelData
{
    public partial class SiteConfigData
    {
        public void Print(FlowDocument doc)
        {
            var sitePage = new Section();
            sitePage.Blocks.Add(PrintUtil.PageHeader(Cultures.Resources.Nav_Site_Configuration));

            sitePage.Blocks.Add(systemConfig());
            doc.Blocks.Add(sitePage);
        }


        private int _totalRows    = 12;
        private int _totalColumns = 9;


        public BlockUIContainer systemConfig()
        {
            var grid = new Grid();

            for (int i = 0; i < _totalRows; i++)
                GridUtil.AddRowToGrid(grid);

            for (int i = 0; i < _totalColumns; i++)
                GridUtil.AddColumnToGridMinWidth(grid, 40);

            GridUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.System_Name), 0, 0, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, " ",  1, 0, false, false);
            GridUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Client_Name),       2, 0, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Client_Address),    3, 0, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Postcode),          7, 0, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Tel),               8, 0, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Installer_Name),    2, 3, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Installer_Address), 3, 3, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Postcode),          7, 3, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Tel),               8, 3, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, " ",  9, 0, false, false);
            GridUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Installed_On),     10, 0, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Commissioned_On),  11, 0, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Engineer_Name),    10, 3, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Engineer_Number),  11, 3, HorizontalAlignment.Right, false);
            
            GridUtil.AddCellToGrid(grid, "\t", 1, 2, false, false);

            GridUtil.AddCellToGrid(grid, SystemName, 0, 1, true, string.IsNullOrWhiteSpace(SystemName));
            addAddress(grid, Client, 2, 1);
            addAddress(grid, Installer, 2, 4);
            GridUtil.AddCellToGrid(grid, InstallDate?.ToString("d"),    10, 1, true, false);//InstallDate is null);
            GridUtil.AddCellToGrid(grid, CommissionDate?.ToString("d"), 11, 1, true, false);//CommissionDate is null);
            GridUtil.AddCellToGrid(grid, EngineerName, 10, 4, true, false);//string.IsNullOrWhiteSpace(EngineerName));
            GridUtil.AddCellToGrid(grid, EngineerNo,   11, 4, true, false);//string.IsNullOrWhiteSpace(EngineerNo));

            GridUtil.AddRowToGrid(grid, 10);

            return new(grid);
        }


        private void addAddress(Grid grid, NameAndAddressData nad, int startRow, int col)
        {
            GridUtil.AddCellToGrid(grid, nad.Name, startRow++, col, true, string.IsNullOrWhiteSpace(nad.Name));
            for (int i = 0; i < NameAndAddressData.NumAddressLines; i++)
                GridUtil.AddCellToGrid(grid, addressLine(nad.Address, i), startRow + i, col, true, i == 0 && string.IsNullOrWhiteSpace(addressLine(nad.Address, i)));
            startRow += NameAndAddressData.NumAddressLines;
            GridUtil.AddCellToGrid(grid, nad.Postcode, startRow++, col, true, false);//string.IsNullOrWhiteSpace(nad.Postcode));
            GridUtil.AddCellToGrid(grid, nad.Tel, startRow, col, true, false);//string.IsNullOrWhiteSpace(nad.Tel));
        }


        public string addressLine(List<string> address, int index) => index < address.Count ? address[index] : "";
    }
}
