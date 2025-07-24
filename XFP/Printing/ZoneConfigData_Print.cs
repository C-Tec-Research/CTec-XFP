using System;
using System.Windows.Documents;
using CTecDevices.Protocol;
using System.Windows.Media;
using CTecUtil.Printing;
using System.Windows;
using System.Windows.Controls;
using CTecControls.UI.ViewHelpers;
using CTecUtil;
using Xfp.UI;

namespace Xfp.DataTypes.PanelData
{
    public partial class ZoneConfigData
    {
        public void Print(FlowDocument doc, XfpPanelData panelData, ref int pageNumber)
        {
            if (pageNumber++ > 1)
                PrintUtil.InsertPageBreak(doc);

            _zonePanelData = panelData.ZonePanelConfig;

            var zonesPage = new Section();
            zonesPage.Blocks.Add(PrintUtil.PageHeader(string.Format(Cultures.Resources.Panel_x, panelData.PanelNumber) + " - " + Cultures.Resources.Nav_Zone_Configuration));

            zonesPage.Blocks.Add(headerInfo());
            zonesPage.Blocks.Add(new BlockUIContainer(new TextBlock()));
            zonesPage.Blocks.Add(zoneList());
            doc.Blocks.Add(zonesPage);
        }


        ZonePanelConfigData _zonePanelData;
        private static SolidColorBrush  _headerBorderBrush = Styles.Brush06;


        private BlockUIContainer headerInfo()
        {
            var grid = new Grid();

            GridUtil.AddRowToGrid(grid);
            GridUtil.AddRowToGrid(grid);

            GridUtil.AddColumnToGrid(grid);
            GridUtil.AddColumnToGrid(grid);

            grid.Children.Add(GridUtil.GridCell(Cultures.Resources.Input_Delay, 0, 0));
            grid.Children.Add(GridUtil.GridCell(TextProcessing.TimeSpanToString(InputDelay, true, TextAlignment.Left, "ms", true), 0, 1));
            grid.Children.Add(GridUtil.GridCell(Cultures.Resources.Investigation_Period, 1, 0));
            grid.Children.Add(GridUtil.GridCell(TextProcessing.TimeSpanToString(InvestigationPeriod, true, TextAlignment.Left, "ms", true), 1, 1));

            return new(grid);
        }


        private BlockUIContainer zoneList()
        {
            var grid = columnHeaders();

            //add the rows for the data
            for (int i = 0; i < Zones.Count + _zonePanelData.Panels.Count; i++)
                GridUtil.AddRowToGrid(grid);
                
            int row = 3;
            int col = 0;

            foreach (var z in Zones)
            {
                col = 0;

                grid.Children.Add(GridUtil.GridBackground(row, 0, 1, 15, Int32.IsOddInteger(row) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));
                
                grid.Children.Add(GridUtil.GridCell(z.Number, row, col++, HorizontalAlignment.Right));
                grid.Children.Add(GridUtil.GridCell(z.Name, row, col++));
                grid.Children.Add(GridUtil.GridCellTimeSpan(z.SounderDelay, row, col++, "ms", false, false, HorizontalAlignment.Center));
                grid.Children.Add(GridUtil.GridCellTimeSpan(z.Relay1Delay,  row, col++, "ms", false, false, HorizontalAlignment.Center));
                grid.Children.Add(GridUtil.GridCellTimeSpan(z.Relay2Delay,  row, col++, "ms", false, false, HorizontalAlignment.Center));
                grid.Children.Add(GridUtil.GridCellTimeSpan(z.RemoteDelay,  row, col++, "ms", false, false, HorizontalAlignment.Center));
                grid.Children.Add(GridUtil.GridCellBool(z.Detectors, row, col++, false, false));
                grid.Children.Add(GridUtil.GridCellBool(z.MCPs,      row, col++, false, false));
                grid.Children.Add(GridUtil.GridCellBool(z.EndDelays, row, col++, false, false));
                grid.Children.Add(GridUtil.GridCell(Enums.ZoneDependencyOptionToString(z.Day.DependencyOption), row, col++));

                if (z.Day.DependencyOption == ZoneDependencyOptions.A)
                    grid.Children.Add(GridUtil.GridCellTimeSpan(z.Day.DetectorReset, row, col++, "ms", false, false, HorizontalAlignment.Center));
                else
                    grid.Children.Add(GridUtil.GridCell("-", row, col++, false, HorizontalAlignment.Center));

                if (z.Day.DependencyOption == ZoneDependencyOptions.A || z.Day.DependencyOption == ZoneDependencyOptions.B)
                    grid.Children.Add(GridUtil.GridCellTimeSpan(z.Day.AlarmReset,    row, col++, "ms", false, false, HorizontalAlignment.Center));
                else
                    grid.Children.Add(GridUtil.GridCell("-", row, col++, false, HorizontalAlignment.Center));

                grid.Children.Add(GridUtil.GridCell(Enums.ZoneDependencyOptionToString(z.Night.DependencyOption), row, col++));

                if (z.Night.DependencyOption == ZoneDependencyOptions.A)
                    grid.Children.Add(GridUtil.GridCellTimeSpan(z.Night.DetectorReset, row, col++, "ms", false, false, HorizontalAlignment.Center));
                else
                    grid.Children.Add(GridUtil.GridCell("-", row, col++, false, HorizontalAlignment.Center));

                if (z.Night.DependencyOption == ZoneDependencyOptions.A || z.Night.DependencyOption == ZoneDependencyOptions.B)
                    grid.Children.Add(GridUtil.GridCellTimeSpan(z.Night.AlarmReset, row, col++, "ms", false, false, HorizontalAlignment.Center));
                else
                    grid.Children.Add(GridUtil.GridCell("-", row, col++, false, HorizontalAlignment.Center));

                    row++;
            }

            foreach (var p in _zonePanelData.Panels)
            {
                col = 0;
                
                grid.Children.Add(GridUtil.GridBackground(row, 0, 1, 15, Int32.IsEvenInteger(row) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));
                
                grid.Children.Add(GridUtil.GridCell(p.Number, row, col++, HorizontalAlignment.Right));
                grid.Children.Add(GridUtil.GridCell(p.Name, row, col++));
                grid.Children.Add(GridUtil.GridCellTimeSpan(p.SounderDelay, row, col++, "ms", false, false, HorizontalAlignment.Center));
                grid.Children.Add(GridUtil.GridCellTimeSpan(p.Relay1Delay,  row, col++, "ms", false, false, HorizontalAlignment.Center));
                grid.Children.Add(GridUtil.GridCellTimeSpan(p.Relay2Delay,  row, col++, "ms", false, false, HorizontalAlignment.Center));
                grid.Children.Add(GridUtil.GridCellTimeSpan(p.RemoteDelay,  row, col++, "ms", false, false, HorizontalAlignment.Center));
                grid.Children.Add(GridUtil.GridCell("-",      row, col++, false, HorizontalAlignment.Center));
                grid.Children.Add(GridUtil.GridCell("-",      row, col++, false, HorizontalAlignment.Center));
                grid.Children.Add(GridUtil.GridCell("-",      row, col++, false, HorizontalAlignment.Center));
                grid.Children.Add(GridUtil.GridCell("    --", row, col++));
                grid.Children.Add(GridUtil.GridCell("-",      row, col++, false, HorizontalAlignment.Center));
                grid.Children.Add(GridUtil.GridCell("-",      row, col++, false, HorizontalAlignment.Center));
                grid.Children.Add(GridUtil.GridCell("    --", row, col++));
                grid.Children.Add(GridUtil.GridCell("-",      row, col++, false, HorizontalAlignment.Center));
                grid.Children.Add(GridUtil.GridCell("-",      row, col++, false, HorizontalAlignment.Center));
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

            for (int i = 0; i < 15; i++)
            {
                if (i == 8)
                    GridUtil.AddColumnToGrid(grid, 60);
                else
                    GridUtil.AddColumnToGrid(grid);
            }

            grid.Children.Add(GridUtil.GridBackground(0, 0, 1, 15, PrintUtil.GridHeaderBackground));
            grid.Children.Add(GridUtil.GridBackground(1, 0, 1, 15, PrintUtil.GridHeaderBackground));
            grid.Children.Add(GridUtil.GridBackground(2, 0, 1, 15, PrintUtil.GridHeaderBackground));

            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Output_Delays_Mins,         0,  2, 1, 4, HorizontalAlignment.Center));
            grid.Children.Add(GridUtil.GridHeaderCell("→",                                           0,  2, 1, 4, HorizontalAlignment.Right));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Functioning_With,           0,  6, 1, 2, HorizontalAlignment.Center));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Multiple_Alarms_End_Delays, 0,  8, 3, 1, HorizontalAlignment.Center));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Day_Dependencies,           0,  9, 1, 3, HorizontalAlignment.Center));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Night_Dependencies,         0, 12, 1, 3, HorizontalAlignment.Center));
            
            GridUtil.AddBorderToGrid(grid, 1, 2, 1, 4, _headerBorderBrush, new Thickness(1, 1, 1, 0), new CornerRadius(0), new(1.5, -2, 1.5, 0), 3);
            GridUtil.AddBorderToGrid(grid, 1, 6, 1, 2, _headerBorderBrush, new Thickness(1, 1, 1, 0), new CornerRadius(0), new(1.5, -2, 1.5, 0), 3);
            GridUtil.AddBorderToGrid(grid, 1, 9, 1, 3, _headerBorderBrush, new Thickness(1, 1, 1, 0), new CornerRadius(0), new(1.5, -2, 1.5, 0), 3);
            GridUtil.AddBorderToGrid(grid, 1,12, 1, 3, _headerBorderBrush, new Thickness(1, 1, 1, 0), new CornerRadius(0), new(1.5, -2, 1.5, 0), 3);

            int col = 0;
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Zone,      2, col++, 1, 2));
            col++;
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Sounders,  2, col++, HorizontalAlignment.Center));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Relay_1,   2, col++, HorizontalAlignment.Center));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Relay_2,   2, col++, HorizontalAlignment.Center));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Outputs,   2, col++, HorizontalAlignment.Center));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Detectors, 2, col++, HorizontalAlignment.Center));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.MCPs,      2, col++, HorizontalAlignment.Center));
            col++;
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Option,    2, col++));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Detector,  2, col++, HorizontalAlignment.Center));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Alarm,     2, col++, HorizontalAlignment.Center));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Option,    2, col++));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Detector,  2, col++, HorizontalAlignment.Center));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Alarm,     2, col++, HorizontalAlignment.Center));

            return grid;
        }
    }
}
