using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using CTecDevices.Protocol;
using CTecUtil.Printing;
using System;

namespace Xfp.DataTypes.PanelData
{
    public partial class DeviceConfigData
    {
        public void Print(FlowDocument doc, int panelNumber, bool printAllLoopDevices)
        {
            var devicePage = new Section();
            devicePage.Blocks.Add(PrintUtil.PageHeader(string.Format(Cultures.Resources.Loop_x_Devices, LoopNum + 1)));

            devicePage.Blocks.Add(deviceList(printAllLoopDevices));

            doc.Blocks.Add(devicePage);
        }


        private int _totalColumns = 13;
        private static Style _deviceIconStyle = (Style)Application.Current.FindResource("MainGridIcon");


        public BlockUIContainer deviceList(bool printAllLoopDevices)
        {
            var grid = columnHeaders();


            int gridRow = 1;
            int device = 0;

            foreach (var d in Devices)
            {
                if (printAllLoopDevices || DeviceTypes.IsValidDeviceType(d.DeviceType, DeviceTypes.CurrentProtocolType))
                {
                    grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                    grid.Children.Add(PrintUtil.GridBackground(gridRow, 0, 1, _totalColumns, Int32.IsOddInteger(device) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));

                    grid.Children.Add(PrintUtil.GridCell(d.Index + 1, gridRow, 0, TextAlignment.Right));
                    //grid.Children.Add(PrintUtil.GridCell(z.DeviceType, row, 1));
                    grid.Children.Add(PrintUtil.GridCell(DeviceTypes.DeviceTypeName(d.DeviceType, DeviceTypes.CurrentProtocolType), gridRow, 2));

                    if (d.IsIODevice)
                    {
                        //grid.Children.Add(PrintUtil.GridCell(Cultures.Resources.Subaddress,     gridRow, 3));
                        grid.Children.Add(PrintUtil.GridCell(Cultures.Resources.I_O,            gridRow, 4));
                        grid.Children.Add(PrintUtil.GridCell(Cultures.Resources.Channel_Abbr,   gridRow, 5));
                        grid.Children.Add(PrintUtil.GridCell(Cultures.Resources.Zone_Group_Set, gridRow, 6));
                        grid.Children.Add(PrintUtil.GridCell(Cultures.Resources.Description,    gridRow, 7));
                        gridRow++;

                    }
                    else
                    {
                        grid.Children.Add(PrintUtil.GridCell("zone", gridRow, 3, 1, 5));
                    }

                    grid.Children.Add(PrintUtil.GridCell(d.NameIndex, gridRow, 8));
                    grid.Children.Add(PrintUtil.GridCell(d.NameIndex, gridRow, 9));
                    grid.Children.Add(PrintUtil.GridCell(string.Format("{0}:{1}", d.DaySensitivity ?? 0, d.NightSensitivity ?? 0), gridRow, 10, TextAlignment.Center));

                    if (DeviceTypes.CurrentProtocolIsXfpApollo)
                    {
                        grid.Children.Add(PrintUtil.GridCellBool(d.RemoteLEDEnabled ?? false, gridRow, 11, false, false));
                        grid.Children.Add(PrintUtil.GridCell(d.AncillaryBaseSounderGroup is null ? "None" : d.AncillaryBaseSounderGroup.Value.ToString(), gridRow, 12));
                    }

                    gridRow++;
                    device++;
                }
            }

            return new(grid);
        }


        private Grid columnHeaders()
        {
            Grid grid = new Grid();

            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            for (int i = 0; i < _totalColumns; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            grid.Children.Add(PrintUtil.GridBackground(0, 0, 1, _totalColumns, PrintUtil.GridHeaderBackground));

            int col = 0;
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Number_Symbol, 0, col++));
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Device_Type, 0, col++, 1, 2));
            col++;
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Zone_Group, 0, col++, 1, 5));
            col += 5;
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Device_Name, 0, col++));
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Volume_Sensitivity_mode, 0, col++));
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Day_Night, 0, col++));

            if (DeviceTypes.CurrentProtocolIsXfpApollo)
            {
                grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Remote_LED_Header, 0, col++));
                grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Base_Sounder_Header, 0, col++));
            }

            return grid;
        }
    }
}
