using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using CTecUtil.Printing;
using Windows.Media.Protection.PlayReady;
using System.Text;
using System.Collections.Generic;

namespace Xfp.DataTypes.PanelData
{
    public partial class PanelConfigData
    {
        public void Print(FlowDocument doc, XfpPanelData data)
        {
            _data = data;

            var sitePage = new Section();
            sitePage.Blocks.Add(PrintUtil.PageHeader(string.Format(Cultures.Resources.Panel_x_Configuration, _data.PanelNumber)));

            sitePage.Blocks.Add(systemHeader());

            doc.Blocks.Add(sitePage);
        }


        XfpPanelData _data;


        public BlockUIContainer systemHeader()
        {
            var grid = new Grid();

            for (int i = 0; i < 12; i++)
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            for (int i = 0; i < 10; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            PrintUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Firmware_Version), 0, 0, HorizontalAlignment.Right, false);
            PrintUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Normal_String), 1, 0, HorizontalAlignment.Right, false);
            PrintUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Maintenance_String), 2, 0, HorizontalAlignment.Right, false);
            PrintUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Maintenance_Date), 3, 0, HorizontalAlignment.Right, false);
            PrintUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.AL2_Code), 4, 0, HorizontalAlignment.Right, false);
            PrintUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.AL3_Code), 5, 0, HorizontalAlignment.Right, false);
            PrintUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Real_Time_Output_Of_Events), 6, 0, HorizontalAlignment.Right, false);
            PrintUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Blink_Polling_LED), 7, 0, HorizontalAlignment.Right, false);
            PrintUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Auto_Adjust_DST), 8, 0, HorizontalAlignment.Right, false);
            PrintUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Day_Begins_At), 10, 0, HorizontalAlignment.Right, false);
            PrintUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Night_Begins_At), 11, 0, HorizontalAlignment.Right, false);

            PrintUtil.AddCellToGrid(grid, FirmwareVersion, 0, 1, 8, HorizontalAlignment.Left, true);
            PrintUtil.AddCellToGrid(grid, QuiescentString, 1, 1, 8, HorizontalAlignment.Left, true, string.IsNullOrWhiteSpace(QuiescentString));
            PrintUtil.AddCellToGrid(grid, MaintenanceString, 2, 1, 8, HorizontalAlignment.Left, true, string.IsNullOrWhiteSpace(MaintenanceString));
            PrintUtil.AddCellToGrid(grid, MaintenanceDate?.ToString("d"), 3, 1, 8, HorizontalAlignment.Left, true, MaintenanceDate is null);
            PrintUtil.AddCellToGrid(grid, AL2Code, 4, 1, 8, HorizontalAlignment.Left, true, string.IsNullOrWhiteSpace(AL2Code) || AL2Code.Length < AccessCodeLength);
            PrintUtil.AddCellToGrid(grid, AL3Code, 5, 1, 8, HorizontalAlignment.Left, true, string.IsNullOrWhiteSpace(AL3Code) || AL3Code.Length < AccessCodeLength);
            PrintUtil.AddCellToGrid(grid, PrintUtil.GridCellYesNo(RealTimeEventOutput, 6, 1, 1, 8, true, true));
            PrintUtil.AddCellToGrid(grid, PrintUtil.GridCellYesNo(BlinkPollingLED, 7, 1, 1, 8, true, true));
            PrintUtil.AddCellToGrid(grid, PrintUtil.GridCellYesNo(AutoAdjustDST, 8, 1, 1, 8, true, true));

            PrintUtil.AddCellToGrid(grid, " ", 0, 2, false);

            PrintUtil.AddCellToGrid(grid, Cultures.Resources.Day_Mon_Abbr, 9, 3, HorizontalAlignment.Center, false);
            PrintUtil.AddCellToGrid(grid, Cultures.Resources.Day_Tue_Abbr, 9, 4, HorizontalAlignment.Center, false);
            PrintUtil.AddCellToGrid(grid, Cultures.Resources.Day_Wed_Abbr, 9, 5, HorizontalAlignment.Center, false);
            PrintUtil.AddCellToGrid(grid, Cultures.Resources.Day_Thu_Abbr, 9, 6, HorizontalAlignment.Center, false);
            PrintUtil.AddCellToGrid(grid, Cultures.Resources.Day_Fri_Abbr, 9, 7, HorizontalAlignment.Center, false);
            PrintUtil.AddCellToGrid(grid, Cultures.Resources.Day_Sat_Abbr, 9, 8, HorizontalAlignment.Center, false);
            PrintUtil.AddCellToGrid(grid, Cultures.Resources.Day_Sun_Abbr, 9, 9, HorizontalAlignment.Center, false);

            PrintUtil.AddCellToGrid(grid, PrintUtil.GridCellTimeSpan(OccupiedBegins, 10, 1, "hm", false, true, HorizontalAlignment.Left));
            PrintUtil.AddCellToGrid(grid, PrintUtil.GridCellTimeSpan(OccupiedEnds,   11, 1, "hm", false, true, HorizontalAlignment.Left));

            dayNightStarts(grid, DayStart,   10, 3);
            dayNightStarts(grid, NightStart, 11, 3);

            return new(grid);
        }

        private void dayNightStarts(Grid grid, List<bool> values, int row, int columnStart)
        {
            for (int i = 0; i < 7; i++)
                PrintUtil.AddCellToGrid(grid, PrintUtil.GridCellBool(values is null ? false : values[i], row, columnStart + i, true, true));
        }
    }
}
