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
            var result = new Grid();

            for (int i = 0; i < 12; i++)
                result.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            for (int i = 0; i < 10; i++)
                result.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            result.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Firmware_Version), 0, 0, TextAlignment.Right));
            result.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Normal_String), 1, 0, TextAlignment.Right));
            result.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Maintenance_String), 2, 0, TextAlignment.Right));
            result.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Maintenance_Date), 3, 0, TextAlignment.Right));
            result.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.AL2_Code), 4, 0, TextAlignment.Right));
            result.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.AL3_Code), 5, 0, TextAlignment.Right));
            result.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Real_Time_Output_Of_Events), 6, 0, TextAlignment.Right));
            result.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Blink_Polling_LED), 7, 0, TextAlignment.Right));
            result.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Auto_Adjust_DST), 8, 0, TextAlignment.Right));
            result.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Day_Begins_At), 10, 0, TextAlignment.Right));
            result.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Night_Begins_At), 11, 0, TextAlignment.Right));

            result.Children.Add(PrintUtil.GridCell(FirmwareVersion, 0, 1, 1, 8, true));
            result.Children.Add(PrintUtil.GridCell(QuiescentString, 1, 1, 1, 8, true));
            result.Children.Add(PrintUtil.GridCell(MaintenanceString, 2, 1, 1, 8, true));
            if (MaintenanceDate is not null)
                result.Children.Add(PrintUtil.GridCell(MaintenanceDate.Value.ToString("d"), 3, 1, 1, 8, true));
            result.Children.Add(PrintUtil.GridCell(AL2Code, 4, 1, 1, 8, true));
            result.Children.Add(PrintUtil.GridCell(AL3Code, 5, 1, 1, 8, true));
            result.Children.Add(PrintUtil.GridCellYesNo(RealTimeEventOutput, 6, 1, 1, 8, true, true));
            result.Children.Add(PrintUtil.GridCellYesNo(BlinkPollingLED, 7, 1, 1, 8, true, true));
            result.Children.Add(PrintUtil.GridCellYesNo(AutoAdjustDST, 8, 1, 1, 8, true, true));

            result.Children.Add(PrintUtil.GridCell(" ", 0, 2));

            result.Children.Add(PrintUtil.GridCell(Cultures.Resources.Day_Mon_Abbr, 9, 3, TextAlignment.Center));
            result.Children.Add(PrintUtil.GridCell(Cultures.Resources.Day_Tue_Abbr, 9, 4, TextAlignment.Center));
            result.Children.Add(PrintUtil.GridCell(Cultures.Resources.Day_Wed_Abbr, 9, 5, TextAlignment.Center));
            result.Children.Add(PrintUtil.GridCell(Cultures.Resources.Day_Thu_Abbr, 9, 6, TextAlignment.Center));
            result.Children.Add(PrintUtil.GridCell(Cultures.Resources.Day_Fri_Abbr, 9, 7, TextAlignment.Center));
            result.Children.Add(PrintUtil.GridCell(Cultures.Resources.Day_Sat_Abbr, 9, 8, TextAlignment.Center));
            result.Children.Add(PrintUtil.GridCell(Cultures.Resources.Day_Sun_Abbr, 9, 9, TextAlignment.Center));

            result.Children.Add(PrintUtil.GridCellTimeSpan(OccupiedBegins, 10, 1, "hm", false, true, TextAlignment.Left));
            result.Children.Add(PrintUtil.GridCellTimeSpan(OccupiedEnds,   11, 1, "hm", false, true, TextAlignment.Left));

            dayNightStarts(result, DayStart,   10, 3);
            dayNightStarts(result, NightStart, 11, 3);

            return new(result);
        }

        private void dayNightStarts(Grid grid, List<bool> values, int row, int columnStart)
        {
            for (int i = 0; i < 7; i++)
                grid.Children.Add(PrintUtil.GridCellBool(values is null ? false : values[i], row, columnStart + i, true, true));
        }
    }
}
