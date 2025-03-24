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
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            for (int i = 0; i < _totalColumns; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto, MinWidth = 40 });


            PrintUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.System_Name), 0, 0, HorizontalAlignment.Right, false);
            PrintUtil.AddCellToGrid(grid, " ",  1, 0, false, false);
            PrintUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Client_Name),       2, 0, HorizontalAlignment.Right, false);
            PrintUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Client_Address),    3, 0, HorizontalAlignment.Right, false);
            PrintUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Postcode),          7, 0, HorizontalAlignment.Right, false);
            PrintUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Tel),               8, 0, HorizontalAlignment.Right, false);
            PrintUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Installer_Name),    2, 3, HorizontalAlignment.Right, false);
            PrintUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Installer_Address), 3, 3, HorizontalAlignment.Right, false);
            PrintUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Postcode),          7, 3, HorizontalAlignment.Right, false);
            PrintUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Tel),               8, 3, HorizontalAlignment.Right, false);
            PrintUtil.AddCellToGrid(grid, " ",  9, 0, false, false);
            PrintUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Installed_On),     10, 0, HorizontalAlignment.Right, false);
            PrintUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Commissioned_On),  11, 0, HorizontalAlignment.Right, false);
            PrintUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Engineer_Name),    10, 3, HorizontalAlignment.Right, false);
            PrintUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Engineer_Number),  11, 3, HorizontalAlignment.Right, false);
            
            PrintUtil.AddCellToGrid(grid, "\t", 1, 2, false, false);

            PrintUtil.AddCellToGrid(grid, SystemName, 0, 1, true, string.IsNullOrWhiteSpace(SystemName));
            addAddress(grid, Client, 2, 1);
            addAddress(grid, Installer, 2, 4);
            PrintUtil.AddCellToGrid(grid, InstallDate?.ToString("d"),    10, 1, true, InstallDate is null);
            PrintUtil.AddCellToGrid(grid, CommissionDate?.ToString("d"), 11, 1, true, CommissionDate is null);
            PrintUtil.AddCellToGrid(grid, EngineerName, 10, 4, true, string.IsNullOrWhiteSpace(EngineerName));
            PrintUtil.AddCellToGrid(grid, EngineerNo,   11, 4, true, string.IsNullOrWhiteSpace(EngineerNo));

            return new(grid);
        }


        private void addAddress(Grid grid, NameAndAddressData nad, int startRow, int col)
        {
            PrintUtil.AddCellToGrid(grid, nad.Name, startRow++, col, true, string.IsNullOrWhiteSpace(nad.Name));
            for (int i = 0; i < NameAndAddressData.NumAddressLines; i++)
                PrintUtil.AddCellToGrid(grid, addressLine(nad.Address, i), startRow + i, col, true, i == 0 && string.IsNullOrWhiteSpace(addressLine(nad.Address, i)));
            startRow += NameAndAddressData.NumAddressLines;
            PrintUtil.AddCellToGrid(grid, nad.Postcode, startRow++, col, true, string.IsNullOrWhiteSpace(nad.Postcode));
            PrintUtil.AddCellToGrid(grid, nad.Tel, startRow, col, true, string.IsNullOrWhiteSpace(nad.Tel));
        }


        public string addressLine(List<string> address, int index) => index < address.Count ? address[index] : "";
    }
}
