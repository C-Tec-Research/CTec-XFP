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
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });


            grid.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.System_Name), 0, 0, HorizontalAlignment.Right));
            grid.Children.Add(PrintUtil.GridCell(" ",  1, 0));
            grid.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Client_Name),       2, 0, HorizontalAlignment.Right));
            grid.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Client_Address),    3, 0, HorizontalAlignment.Right));
            grid.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Postcode),          7, 0, HorizontalAlignment.Right));
            grid.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Tel),               8, 0, HorizontalAlignment.Right));
            grid.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Installer_Name),    2, 3, HorizontalAlignment.Right));
            grid.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Installer_Address), 3, 3, HorizontalAlignment.Right));
            grid.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Postcode),          7, 3, HorizontalAlignment.Right));
            grid.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Tel),               8, 3, HorizontalAlignment.Right));
            grid.Children.Add(PrintUtil.GridCell(" ",  9, 0));
            grid.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Installed_On),     10, 0, HorizontalAlignment.Right));
            grid.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Commissioned_On),  11, 0, HorizontalAlignment.Right));
            grid.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Engineer_Name),    10, 3, HorizontalAlignment.Right));
            grid.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Engineer_Number),  11, 3, HorizontalAlignment.Right));
            
            grid.Children.Add(PrintUtil.GridCell("\t", 1, 2));

            grid.Children.Add(PrintUtil.GridCell(SystemName, 0, 1, 1, 3, true));
            addAddress(grid, Client, 2, 1);
            addAddress(grid, Installer, 2, 4);
            if (InstallDate is not null)    grid.Children.Add(PrintUtil.GridCell(InstallDate.Value.ToString("d"),    10, 1,true));
            if (CommissionDate is not null) grid.Children.Add(PrintUtil.GridCell(CommissionDate.Value.ToString("d"), 11, 1, true));
            grid.Children.Add(PrintUtil.GridCell(EngineerName, 10, 4, true));
            grid.Children.Add(PrintUtil.GridCell(EngineerNo,   11, 4, true));

            return new(grid);
        }


        private void addAddress(Grid grid, NameAndAddressData nad, int startRow, int col)
        {
            grid.Children.Add(PrintUtil.GridCell(nad.Name, startRow++, col, true));
            for (int i = 0; i < NameAndAddressData.NumAddressLines; i++)
                grid.Children.Add(PrintUtil.GridCell(addressLine(nad.Address, i), startRow + i, col, true));
            startRow += NameAndAddressData.NumAddressLines;
            grid.Children.Add(PrintUtil.GridCell(nad.Postcode, startRow++, col, true));
            grid.Children.Add(PrintUtil.GridCell(nad.Tel, startRow, col, true));
        }


        public string addressLine(List<string> address, int index) => index < address.Count ? address[index] : "";
    }
}
