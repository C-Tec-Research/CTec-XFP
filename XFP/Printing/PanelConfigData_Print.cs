using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using CTecUtil.Printing;
using CTecUtil.Utils;

namespace Xfp.DataTypes.PanelData
{
    public partial class PanelConfigData
    {
        public void GetReport(FlowDocument doc, XfpPanelData panelData, ref int pageNumber)
        {
            if (pageNumber++ > 1)
                PrintUtil.InsertPageBreak(doc);

            PrintUtil.PageHeader(doc, string.Format(Cultures.Resources.Panel_x_Settings, panelData.PanelNumber));

            var panelPage = new Section();
            panelPage.Blocks.Add(systemHeader());

            doc.Blocks.Add(panelPage);
        }


        public BlockUIContainer systemHeader()
        {
            var grid = new Grid();

            for (int i = 0; i < 12; i++)
                GridUtil.AddRowToGrid(grid);
                
            for (int i = 0; i < 10; i++)
                GridUtil.AddColumnToGrid(grid);

            GridUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Firmware_Version), 0, 0, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Normal_String), 1, 0, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Maintenance_String), 2, 0, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Maintenance_Date), 3, 0, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.AL2_Code), 4, 0, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.AL3_Code), 5, 0, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Real_Time_Output_Of_Events), 6, 0, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Blink_Polling_LED), 7, 0, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Auto_Adjust_DST), 8, 0, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Day_Begins_At), 10, 0, HorizontalAlignment.Right, false);
            GridUtil.AddCellToGrid(grid, appendColon(Cultures.Resources.Night_Begins_At), 11, 0, HorizontalAlignment.Right, false);

            GridUtil.AddCellToGrid(grid, FirmwareVersion, 0, 1, 8, HorizontalAlignment.Left, true);
            GridUtil.AddCellToGrid(grid, QuiescentString, 1, 1, 8, HorizontalAlignment.Left, true, string.IsNullOrWhiteSpace(QuiescentString));
            GridUtil.AddCellToGrid(grid, MaintenanceString, 2, 1, 8, HorizontalAlignment.Left, true, string.IsNullOrWhiteSpace(MaintenanceString));
            GridUtil.AddCellToGrid(grid, MaintenanceDate?.ToString("d"), 3, 1, 8, HorizontalAlignment.Left, true, MaintenanceDate is null);
            GridUtil.AddCellToGrid(grid, AL2Code, 4, 1, 8, HorizontalAlignment.Left, true, !IsValidAccessCode(AL2Code));
            GridUtil.AddCellToGrid(grid, AL3Code, 5, 1, 8, HorizontalAlignment.Left, true, !IsValidAccessCode(AL3Code));
            GridUtil.AddCellToGrid(grid, GridUtil.GridCellYesNo(RealTimeEventOutput, 6, 1, 1, 8, true, true));
            GridUtil.AddCellToGrid(grid, GridUtil.GridCellYesNo(BlinkPollingLED, 7, 1, 1, 8, true, true));
            GridUtil.AddCellToGrid(grid, GridUtil.GridCellYesNo(AutoAdjustDST, 8, 1, 1, 8, true, true));

            GridUtil.AddCellToGrid(grid, " ", 0, 2, false);

            GridUtil.AddCellToGrid(grid, Cultures.Resources.Day_Mon_Abbr, 9, 3, HorizontalAlignment.Center, false);
            GridUtil.AddCellToGrid(grid, Cultures.Resources.Day_Tue_Abbr, 9, 4, HorizontalAlignment.Center, false);
            GridUtil.AddCellToGrid(grid, Cultures.Resources.Day_Wed_Abbr, 9, 5, HorizontalAlignment.Center, false);
            GridUtil.AddCellToGrid(grid, Cultures.Resources.Day_Thu_Abbr, 9, 6, HorizontalAlignment.Center, false);
            GridUtil.AddCellToGrid(grid, Cultures.Resources.Day_Fri_Abbr, 9, 7, HorizontalAlignment.Center, false);
            GridUtil.AddCellToGrid(grid, Cultures.Resources.Day_Sat_Abbr, 9, 8, HorizontalAlignment.Center, false);
            GridUtil.AddCellToGrid(grid, Cultures.Resources.Day_Sun_Abbr, 9, 9, HorizontalAlignment.Center, false);

            GridUtil.AddCellToGrid(grid, GridUtil.GridCellTimeSpan(OccupiedBegins, 10, 1, "hm", false, true, HorizontalAlignment.Left));
            GridUtil.AddCellToGrid(grid, GridUtil.GridCellTimeSpan(OccupiedEnds,   11, 1, "hm", false, true, HorizontalAlignment.Left));

            dayNightStarts(grid, DayStart,   10, 3);
            dayNightStarts(grid, NightStart, 11, 3);

            GridUtil.AddRowToGrid(grid, 10);

            return new(grid);
        }

        private void dayNightStarts(Grid grid, List<bool> values, int row, int columnStart)
        {
            for (int i = 0; i < 7; i++)
                GridUtil.AddCellToGrid(grid, GridUtil.GridCellBool(values is null ? false : values[i], row, columnStart + i, true, true));
        }
    }
}
