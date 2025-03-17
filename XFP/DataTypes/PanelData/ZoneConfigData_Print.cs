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
        public void Print(FlowDocument doc, XfpPanelData panelData)
        {
            _zonePanelData = panelData.ZonePanelConfig;

            var zonesPage = new Section();
            zonesPage.Blocks.Add(PrintUtil.PageHeader(string.Format(Cultures.Resources.Panel_x, panelData.PanelNumber) + " - " + Cultures.Resources.Nav_Zone_Configuration));

            zonesPage.Blocks.Add(headerInfo());
            zonesPage.Blocks.Add(new BlockUIContainer(new TextBlock()));
            zonesPage.Blocks.Add(zoneList());

            doc.Blocks.Add(zonesPage);
        }


        ZonePanelConfigData _zonePanelData;


        private BlockUIContainer headerInfo()
        {
            var grid = new Grid();

            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            grid.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Input_Delay), 0, 0));
            grid.Children.Add(PrintUtil.GridCell(TextProcessing.TimeSpanToString(InputDelay, true, TextAlignment.Left, "ms", true), 0, 1));
            grid.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Investigation_Period), 1, 0));
            grid.Children.Add(PrintUtil.GridCell(TextProcessing.TimeSpanToString(InvestigationPeriod, true, TextAlignment.Left, "ms", true), 1, 1));

            return new(grid);
        }


        private BlockUIContainer zoneList()
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

                grid.Children.Add(PrintUtil.GridBackground(row, 0, 1, 15, Int32.IsOddInteger(row) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));
                
                grid.Children.Add(PrintUtil.GridCell(z.Number, row, col++, HorizontalAlignment.Right));
                grid.Children.Add(PrintUtil.GridCell(z.Name, row, col++));
                grid.Children.Add(PrintUtil.GridCellTimeSpan(z.SounderDelay, row, col++, "ms", false, false, HorizontalAlignment.Center));
                grid.Children.Add(PrintUtil.GridCellTimeSpan(z.Relay1Delay,  row, col++, "ms", false, false, HorizontalAlignment.Center));
                grid.Children.Add(PrintUtil.GridCellTimeSpan(z.Relay2Delay,  row, col++, "ms", false, false, HorizontalAlignment.Center));
                grid.Children.Add(PrintUtil.GridCellTimeSpan(z.RemoteDelay,  row, col++, "ms", false, false, HorizontalAlignment.Center));
                grid.Children.Add(PrintUtil.GridCellYesNo(z.Detectors, row, col++, false, false));
                grid.Children.Add(PrintUtil.GridCellYesNo(z.MCPs,      row, col++, false, false));
                grid.Children.Add(PrintUtil.GridCellYesNo(z.EndDelays, row, col++, false, false));
                grid.Children.Add(PrintUtil.GridCell(Enums.ZoneDependencyOptionToString(z.Day.DependencyOption), row, col++, HorizontalAlignment.Center));
                grid.Children.Add(PrintUtil.GridCellTimeSpan(z.Day.DetectorReset, row, col++, "ms", false, false, HorizontalAlignment.Center));
                grid.Children.Add(PrintUtil.GridCellTimeSpan(z.Day.AlarmReset,    row, col++, "ms", false, false, HorizontalAlignment.Center));
                grid.Children.Add(PrintUtil.GridCell(Enums.ZoneDependencyOptionToString(z.Night.DependencyOption), row, col++, HorizontalAlignment.Center));
                grid.Children.Add(PrintUtil.GridCellTimeSpan(z.Night.DetectorReset, row, col++, "ms", false, false, HorizontalAlignment.Center));
                grid.Children.Add(PrintUtil.GridCellTimeSpan(z.Night.AlarmReset,    row, col++, "ms", false, false, HorizontalAlignment.Center));
                row++;
            }

            foreach (var p in _zonePanelData.Panels)
            {
                col = 0;
                
                grid.Children.Add(PrintUtil.GridBackground(row, 0, 1, 15, Int32.IsOddInteger(row) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));
                
                grid.Children.Add(PrintUtil.GridCell(p.Number, row, col++, HorizontalAlignment.Right));
                grid.Children.Add(PrintUtil.GridCell(p.Name, row, col++, HorizontalAlignment.Left));
                grid.Children.Add(PrintUtil.GridCellTimeSpan(p.SounderDelay, row, col++, "ms", false, false, HorizontalAlignment.Center));
                grid.Children.Add(PrintUtil.GridCellTimeSpan(p.Relay1Delay,  row, col++, "ms", false, false, HorizontalAlignment.Center));
                grid.Children.Add(PrintUtil.GridCellTimeSpan(p.Relay2Delay,  row, col++, "ms", false, false, HorizontalAlignment.Center));
                grid.Children.Add(PrintUtil.GridCellTimeSpan(p.RemoteDelay,  row, col++, "ms", false, false, HorizontalAlignment.Center));
                row++;
            }

            return new(grid);
        }


        private Grid columnHeaders()
        {
            Grid g = new Grid();

            //two rows for column headers
            g.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            g.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            for (int i = 0; i < 15; i++)
                g.ColumnDefinitions.Add(new ColumnDefinition() { Width = i == 8 ? (GridLength)new GridLengthConverter().ConvertFrom(60) : GridLength.Auto });

            g.Children.Add(PrintUtil.GridBackground(0, 0, 1, 15, PrintUtil.GridHeaderBackground));
            g.Children.Add(PrintUtil.GridBackground(1, 0, 1, 15, PrintUtil.GridHeaderBackground));

            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Output_Delays_Mins,         0,  2, 1, 4, HorizontalAlignment.Center));
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Functioning_With,           0,  6, 1, 2, HorizontalAlignment.Center));
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Multiple_Alarms_End_Delays, 0,  8, 2, 1, HorizontalAlignment.Center));
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Day_Dependencies,           0,  9, 1, 3, HorizontalAlignment.Center));
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Night_Dependencies,         0, 12, 1, 3, HorizontalAlignment.Center));
            
            int col = 0;
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Zone,      1, col++, 1, 2));
            col++;
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Sounders,  1, col++, HorizontalAlignment.Center));
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Relay_1,   1, col++, HorizontalAlignment.Center));
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Relay_2,   1, col++, HorizontalAlignment.Center));
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Outputs,   1, col++, HorizontalAlignment.Center));
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Detectors, 1, col++, HorizontalAlignment.Center));
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.MCPs,      1, col++, HorizontalAlignment.Center));
            col++;
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Option,    1, col++));
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Detector,  1, col++, HorizontalAlignment.Center));
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Alarm,     1, col++, HorizontalAlignment.Center));
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Option,    1, col++));
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Detector,  1, col++, HorizontalAlignment.Center));
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Alarm,     1, col++, HorizontalAlignment.Center));

            return g;
        }
    }
}
