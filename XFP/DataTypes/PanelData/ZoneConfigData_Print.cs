using System;
using System.Windows.Documents;
using CTecDevices.Protocol;
using System.Windows.Media;
using CTecUtil.Printing;
using System.Windows;
using System.Windows.Controls;
using CTecControls.UI.ViewHelpers;
using CTecUtil;

namespace Xfp.DataTypes.PanelData
{
    public partial class ZoneConfigData
    {
        public void Print(FlowDocument doc, ZonePanelConfigData zonePanelData)
        {
            _zonePanelData = zonePanelData;

            var zonesPage = new Section();
            zonesPage.Blocks.Add(PrintUtil.PageHeader(Cultures.Resources.Nav_Zone_Configuration));

            zonesPage.Blocks.Add(new BlockUIContainer(headerInfoGrid()));
            zonesPage.Blocks.Add(new BlockUIContainer(new TextBlock()));
            zonesPage.Blocks.Add(new BlockUIContainer(zoneListGrid()));

            doc.Blocks.Add(zonesPage);
        }


        ZonePanelConfigData _zonePanelData;


        private Grid headerInfoGrid()
        {
            var result = new Grid();

            result.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            result.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            result.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            result.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            result.Children.Add(PrintUtil.GridCell(Cultures.Resources.Input_Delay, 0, 0));
            result.Children.Add(PrintUtil.GridCell(TextProcessing.TimeSpanToString(InputDelay, true, TextAlignment.Left, "ms", true), 0, 1));
            result.Children.Add(PrintUtil.GridCell(Cultures.Resources.Investigation_Period, 1, 0));
            result.Children.Add(PrintUtil.GridCell(TextProcessing.TimeSpanToString(InvestigationPeriod, true, TextAlignment.Left, "ms", true), 1, 1));

            return result;
        }


        private Grid zoneListGrid()
        {
            var grid = columnHeaders();

            //add the rows for the data
            for (int i = 0; i < Zones.Count + _zonePanelData.Panels.Count; i++)
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            int row = 2;
            int col = 0;

            foreach (var z in Zones)
            {
                col = 0;
                var background = Int32.IsOddInteger(row) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground;
                grid.Children.Add(PrintUtil.GridCell(string.Format("{0,2} {1}", z.Number, z.Name), row, col++, TextAlignment.Left, background));
                grid.Children.Add(PrintUtil.GridCellTimeSpan(z.SounderDelay, row, col++, "ms", false, TextAlignment.Center, background));
                grid.Children.Add(PrintUtil.GridCellTimeSpan(z.Relay1Delay,  row, col++, "ms", false, TextAlignment.Center, background));
                grid.Children.Add(PrintUtil.GridCellTimeSpan(z.Relay2Delay,  row, col++, "ms", false, TextAlignment.Center, background));
                grid.Children.Add(PrintUtil.GridCellTimeSpan(z.RemoteDelay,  row, col++, "ms", false, TextAlignment.Center, background));
                grid.Children.Add(PrintUtil.GridCellBool(z.Detectors, row, col++, false, background));
                grid.Children.Add(PrintUtil.GridCellBool(z.MCPs,      row, col++, false, background));
                grid.Children.Add(PrintUtil.GridCellBool(z.EndDelays, row, col++, false, background));
                grid.Children.Add(PrintUtil.GridCell(Enums.ZoneDependencyOptionToString(z.Day.DependencyOption), row, col++, TextAlignment.Center, background));
                grid.Children.Add(PrintUtil.GridCellTimeSpan(z.Day.DetectorReset, row, col++, "ms", false, TextAlignment.Center, background));
                grid.Children.Add(PrintUtil.GridCellTimeSpan(z.Day.AlarmReset,    row, col++, "ms", false, TextAlignment.Center, background));
                grid.Children.Add(PrintUtil.GridCell(Enums.ZoneDependencyOptionToString(z.Night.DependencyOption), row, col++, TextAlignment.Center, background));
                grid.Children.Add(PrintUtil.GridCellTimeSpan(z.Night.DetectorReset, row, col++, "ms", false, TextAlignment.Center, background));
                grid.Children.Add(PrintUtil.GridCellTimeSpan(z.Night.AlarmReset,    row, col++, "ms", false, TextAlignment.Center, background));
                row++;
            }

            foreach (var p in _zonePanelData.Panels)
            {
                col = 0;
                var background = Int32.IsOddInteger(row) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground;
                grid.Children.Add(PrintUtil.GridCell(string.Format("{0,2} {1}", p.Number, p.Name), row, col++));
                grid.Children.Add(PrintUtil.GridCellTimeSpan(p.SounderDelay, row, col++, "ms", false, TextAlignment.Center));
                grid.Children.Add(PrintUtil.GridCellTimeSpan(p.Relay1Delay,  row, col++, "ms", false, TextAlignment.Center));
                grid.Children.Add(PrintUtil.GridCellTimeSpan(p.Relay2Delay,  row, col++, "ms", false, TextAlignment.Center));
                grid.Children.Add(PrintUtil.GridCellTimeSpan(p.RemoteDelay,  row, col++, "ms", false, TextAlignment.Center));
                row++;
            }

            return grid;
        }


        private Grid columnHeaders()
        {
            Grid g = new Grid();

            //two rows for column headers
            g.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            g.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            //grid has 14 columns
            for (int i = 0; i < 14; i++)
                g.ColumnDefinitions.Add(new ColumnDefinition() { Width = i == 7 ? (GridLength)new GridLengthConverter().ConvertFrom(60) : GridLength.Auto });

            g.Children.Add(PrintUtil.GridHeaderCell("", 0, 0));
            g.Children.Add(PrintUtil.gGridHeaderCell(Cultures.Resources.Output_Delays_Mins,         0,  1, 1, 4, TextAlignment.Center));
            g.Children.Add(PrintUtil.gGridHeaderCell(Cultures.Resources.Functioning_With,           0,  5, 1, 2, TextAlignment.Center));
            g.Children.Add(PrintUtil.gGridHeaderCell(Cultures.Resources.Multiple_Alarms_End_Delays, 0,  7, 2, 1, TextAlignment.Center));
            g.Children.Add(PrintUtil.gGridHeaderCell(Cultures.Resources.Day_Dependencies,           0,  8, 1, 3, TextAlignment.Center));
            g.Children.Add(PrintUtil.gGridHeaderCell(Cultures.Resources.Night_Dependencies,         0, 11, 1, 3, TextAlignment.Center));
            
            int col = 0;
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Zone,      1, col++));
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Sounders,  1, col++, TextAlignment.Center));
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Relay_1,   1, col++, TextAlignment.Center));
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Relay_2,   1, col++, TextAlignment.Center));
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Outputs,   1, col++, TextAlignment.Center));
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Detectors, 1, col++, TextAlignment.Center));
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.MCPs,      1, col++, TextAlignment.Center));
            col++;
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Option,    1, col++));
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Detector,  1, col++, TextAlignment.Center));
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Alarm,     1, col++, TextAlignment.Center));
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Option,    1, col++));
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Detector,  1, col++, TextAlignment.Center));
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Alarm,     1, col++, TextAlignment.Center));

            return g;
        }
    }
}
