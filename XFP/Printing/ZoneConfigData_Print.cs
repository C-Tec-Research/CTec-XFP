using CTecDevices.DeviceTypes;
using CTecDevices.Protocol;
using CTecUtil.Printing;
using CTecUtil.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Xfp.UI;

namespace Xfp.DataTypes.PanelData
{
    public partial class ZoneConfigData
    {
        public void GetReport(FlowDocument doc, XfpPanelData panelData, ref int pageNumber)
        {
            if (pageNumber++ > 1)
                PrintUtil.InsertPageBreak(doc);

            _zonePanelData = panelData.ZonePanelConfig;

            PrintUtil.PageHeader(doc, string.Format(Cultures.Resources.Panel_x, panelData.PanelNumber) + " - " + Cultures.Resources.Nav_Zone_Configuration);
            
            TableUtil.ResetDefaults();
            TableUtil.SetForeground(PrintUtil.TextForeground);
            TableUtil.SetFontSize(PrintUtil.PrintSmallerFontSize);
            TableUtil.SetFontStretch(PrintUtil.FontNarrowWidth);
            TableUtil.SetFontFamily(PrintUtil.PrintDefaultFont);
            TableUtil.SetPadding(PrintUtil.DefaultGridMargin);
            
            var headerSection = new Section();
            headerSection.Blocks.Add(headerInfo());
            doc.Blocks.Add(headerSection);

            printZones(doc);

            TableUtil.ResetDefaults();

            //var zonesPage = new Section();
            //zonesPage.Blocks.Add(headerInfo());
            //zonesPage.Blocks.Add(new BlockUIContainer(new TextBlock()));
            //zonesPage.Blocks.Add(zoneList());
            //doc.Blocks.Add(zonesPage);
        }


        private ZonePanelConfigData _zonePanelData;
        private int _totalColumns    = 0;
        private static SolidColorBrush  _headerBorderBrush = Styles.Brush06;


        private BlockUIContainer headerInfo()
        {
            var grid = new Grid();

            GridUtil.AddRowToGrid(grid);
            GridUtil.AddRowToGrid(grid);

            GridUtil.AddColumnToGrid(grid);
            GridUtil.AddColumnToGrid(grid);

            grid.Children.Add(GridUtil.GridCell(Cultures.Resources.Input_Delay, 0, 0));
            grid.Children.Add(GridUtil.GridCell(TextUtil.TimeSpanToString(InputDelay, true, TextAlignment.Left, "ms", true), 0, 1));
            grid.Children.Add(GridUtil.GridCell(Cultures.Resources.Investigation_Period, 1, 0));
            grid.Children.Add(GridUtil.GridCell(TextUtil.TimeSpanToString(InvestigationPeriod, true, TextAlignment.Left, "ms", true), 1, 1));

            return new(grid);
        }


        //private BlockUIContainer zoneList()
        //{
        //    var grid = columnHeaders();

        //    //add the rows for the data
        //    for (int i = 0; i < Zones.Count + _zonePanelData.Panels.Count; i++)
        //        GridUtil.AddRowToGrid(grid);

        //    int row = 3;
        //    int col = 0;

        //    foreach (var z in Zones)
        //    {
        //        col = 0;

        //        grid.Children.Add(GridUtil.GridBackground(row, 0, 1, 15, Int32.IsOddInteger(row) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));

        //        grid.Children.Add(GridUtil.GridCell(z.Number, row, col++, HorizontalAlignment.Right));
        //        grid.Children.Add(GridUtil.GridCell(z.Name, row, col++));
        //        grid.Children.Add(GridUtil.GridCellTimeSpan(z.SounderDelay, row, col++, "ms", false, false, HorizontalAlignment.Center));
        //        grid.Children.Add(GridUtil.GridCellTimeSpan(z.Relay1Delay,  row, col++, "ms", false, false, HorizontalAlignment.Center));
        //        grid.Children.Add(GridUtil.GridCellTimeSpan(z.Relay2Delay,  row, col++, "ms", false, false, HorizontalAlignment.Center));
        //        grid.Children.Add(GridUtil.GridCellTimeSpan(z.RemoteDelay,  row, col++, "ms", false, false, HorizontalAlignment.Center));
        //        grid.Children.Add(GridUtil.GridCellBool(z.Detectors, row, col++, false, false));
        //        grid.Children.Add(GridUtil.GridCellBool(z.MCPs,      row, col++, false, false));
        //        grid.Children.Add(GridUtil.GridCellBool(z.EndDelays, row, col++, false, false));
        //        grid.Children.Add(GridUtil.GridCell(Enums.ZoneDependencyOptionToString(z.Day.DependencyOption), row, col++));

        //        if (z.Day.DependencyOption == ZoneDependencyOptions.A)
        //            grid.Children.Add(GridUtil.GridCellTimeSpan(z.Day.DetectorReset, row, col++, "ms", false, false, HorizontalAlignment.Center));
        //        else
        //            grid.Children.Add(GridUtil.GridCell("-", row, col++, false, HorizontalAlignment.Center));

        //        if (z.Day.DependencyOption == ZoneDependencyOptions.A || z.Day.DependencyOption == ZoneDependencyOptions.B)
        //            grid.Children.Add(GridUtil.GridCellTimeSpan(z.Day.AlarmReset,    row, col++, "ms", false, false, HorizontalAlignment.Center));
        //        else
        //            grid.Children.Add(GridUtil.GridCell("-", row, col++, false, HorizontalAlignment.Center));

        //        grid.Children.Add(GridUtil.GridCell(Enums.ZoneDependencyOptionToString(z.Night.DependencyOption), row, col++));

        //        if (z.Night.DependencyOption == ZoneDependencyOptions.A)
        //            grid.Children.Add(GridUtil.GridCellTimeSpan(z.Night.DetectorReset, row, col++, "ms", false, false, HorizontalAlignment.Center));
        //        else
        //            grid.Children.Add(GridUtil.GridCell("-", row, col++, false, HorizontalAlignment.Center));

        //        if (z.Night.DependencyOption == ZoneDependencyOptions.A || z.Night.DependencyOption == ZoneDependencyOptions.B)
        //            grid.Children.Add(GridUtil.GridCellTimeSpan(z.Night.AlarmReset, row, col++, "ms", false, false, HorizontalAlignment.Center));
        //        else
        //            grid.Children.Add(GridUtil.GridCell("-", row, col++, false, HorizontalAlignment.Center));

        //            row++;
        //    }

        //    foreach (var p in _zonePanelData.Panels)
        //    {
        //        col = 0;

        //        grid.Children.Add(GridUtil.GridBackground(row, 0, 1, 15, Int32.IsEvenInteger(row) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));

        //        grid.Children.Add(GridUtil.GridCell(p.Number, row, col++, HorizontalAlignment.Right));
        //        grid.Children.Add(GridUtil.GridCell(p.Name, row, col++));
        //        grid.Children.Add(GridUtil.GridCellTimeSpan(p.SounderDelay, row, col++, "ms", false, false, HorizontalAlignment.Center));
        //        grid.Children.Add(GridUtil.GridCellTimeSpan(p.Relay1Delay,  row, col++, "ms", false, false, HorizontalAlignment.Center));
        //        grid.Children.Add(GridUtil.GridCellTimeSpan(p.Relay2Delay,  row, col++, "ms", false, false, HorizontalAlignment.Center));
        //        grid.Children.Add(GridUtil.GridCellTimeSpan(p.RemoteDelay,  row, col++, "ms", false, false, HorizontalAlignment.Center));
        //        grid.Children.Add(GridUtil.GridCell("-",      row, col++, false, HorizontalAlignment.Center));
        //        grid.Children.Add(GridUtil.GridCell("-",      row, col++, false, HorizontalAlignment.Center));
        //        grid.Children.Add(GridUtil.GridCell("-",      row, col++, false, HorizontalAlignment.Center));
        //        grid.Children.Add(GridUtil.GridCell("    --", row, col++));
        //        grid.Children.Add(GridUtil.GridCell("-",      row, col++, false, HorizontalAlignment.Center));
        //        grid.Children.Add(GridUtil.GridCell("-",      row, col++, false, HorizontalAlignment.Center));
        //        grid.Children.Add(GridUtil.GridCell("    --", row, col++));
        //        grid.Children.Add(GridUtil.GridCell("-",      row, col++, false, HorizontalAlignment.Center));
        //        grid.Children.Add(GridUtil.GridCell("-",      row, col++, false, HorizontalAlignment.Center));
        //        row++;
        //    }

        //    GridUtil.AddRowToGrid(grid, 10);

        //    return new(grid);
        //}


        public void printZones(FlowDocument doc)
        {
            int dataRows = 0;

            var reportName = Cultures.Resources.Nav_Zone_Configuration;
            var table = new Table() { Name = reportName, BorderThickness = new(0) };

            defineColumnHeaders(table, reportName);

            try
            {
                //    var bodyGroup = new TableRowGroup();

                //    foreach (var z in Zones)
                //    {
                //        dataRows++;

                //        //find number of rows of Mode/Sensitivity/Volume values
                //        var vsmRows = 0;
                //        if (DeviceTypes.IsSensitivityDevice(dev.DeviceType, DeviceTypes.CurrentProtocolType))
                //            vsmRows++;
                //        if (DeviceTypes.IsVolumeDevice(dev.DeviceType, DeviceTypes.CurrentProtocolType))
                //            vsmRows++;
                //        if (DeviceTypes.IsModeDevice(dev.DeviceType, DeviceTypes.CurrentProtocolType))
                //            vsmRows++;
                //        if (vsmRows == 0)
                //            vsmRows = 1;

                //        //find number of I/O settings
                //        int ioRows = dev.IsIODevice ? dev.IOConfig.Count : 1;

                //        //rows required depends on the above
                //        var numRows = Math.Max(ioRows, vsmRows);

                //        //create the required number of table rows
                //        var newRows = new List<TableRow>();
                //        for (int r = 0; r < numRows; r++)
                //            newRows.Add(new TableRow() { Background = Int32.IsEvenInteger(dataRows) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground });
                //        foreach (var row in newRows)
                //            bodyGroup.Rows.Add(row);

                //        //device number
                //        var colNum = TableUtil.NewCell((dev.Index + 1).ToString(), numRows, 1, TextAlignment.Right);
                //        colNum.Padding = new(0,0,5,0);
                //        newRows[0].Cells.Add(colNum);


                //        if (DeviceTypes.IsValidDeviceType(dev.DeviceType, DeviceTypes.CurrentProtocolType))
                //        {
                //            //device icon
                //            newRows[0].Cells.Add(TableUtil.NewCell(getDeviceIcon(dev.DeviceType), numRows, 1));

                //            //device type name
                //            newRows[0].Cells.Add(TableUtil.NewCell(DeviceTypes.DeviceTypeName(dev.DeviceType, DeviceTypes.CurrentProtocolType), numRows, 1));

                //            //zone/group/set
                //            newRows[0].Cells.Add(dev.IsIODevice ? TableUtil.NewCell("  " + Cultures.Resources.See_IO_Configuration_Abbr, numRows, 1, _seeIoSettingsForeground, FontStyles.Italic)
                //                                                : TableUtil.NewCell(zgsDescription(false, dev.IsGroupedDevice, false, dev.IsGroupedDevice ? dev.Group : dev.Zone), numRows, 1));

                //            //name
                //            newRows[0].Cells.Add(TableUtil.NewCell(GetDeviceName?.Invoke(dev.NameIndex), numRows, 1));


                //            //volume/sensitivity/mode & day:night values
                //            int modeSensVolRows = 0;

                //            if (dev.IsModeDevice || dev.IsVolumeDevice || dev.IsSensitivityDevice)
                //            {
                //                var vsmText = new StringBuilder();
                //                var dnText  = new StringBuilder();
                //                if (dev.IsModeDevice)
                //                {
                //                    vsmText.Append(Cultures.Resources.Mode);
                //                    dnText.Append(string.Format("{0}:{1}", dev.DayMode, dev.NightMode ?? 0));
                //                    modeSensVolRows++;
                //                }

                //                if (dev.IsVolumeDevice)
                //                {
                //                    if (modeSensVolRows > 0)
                //                    {
                //                        vsmText.Append("\n");
                //                        dnText.Append("\n");
                //                    }
                //                    vsmText.Append(Cultures.Resources.Volume);
                //                    dnText.Append(string.Format("{0}:{1}", dev.DayVolume, dev.NightVolume ?? 0));
                //                    modeSensVolRows++;
                //                }

                //                if (dev.IsSensitivityDevice)
                //                {
                //                    if (modeSensVolRows > 0)
                //                    {
                //                        vsmText.Append("\n");
                //                        dnText.Append("\n");
                //                    }
                //                    vsmText.Append(Cultures.Resources.Sensitivity);
                //                    dnText.Append(string.Format("{0}:{1}", dev.DaySensitivity, dev.NightSensitivity ?? 0));
                //                    modeSensVolRows++;
                //                }

                //                newRows[0].Cells.Add(TableUtil.NewCell(vsmText.ToString()));
                //                newRows[0].Cells.Add(TableUtil.NewCell(dnText.ToString(), TextAlignment.Center));
                //            }
                //            else
                //            {
                //                newRows[0].Cells.Add(TableUtil.NewCell("--", numRows, 1));
                //                newRows[0].Cells.Add(TableUtil.NewCell("--", numRows, 1, TextAlignment.Center));
                //                modeSensVolRows++;
                //            }

                //            if (DeviceTypes.CurrentProtocolIsXfpApollo)
                //            {
                //                //remote LED
                //                newRows[0].Cells.Add(TableUtil.NewCell(DeviceTypes.CanHaveAncillaryBaseSounder(dev.DeviceType, DeviceTypes.CurrentProtocolType) ? dev.RemoteLEDEnabled ?? false ? "Y" : "N" : "--", numRows, 1, TextAlignment.Center));

                //                //base sounder group
                //                newRows[0].Cells.Add(TableUtil.NewCell((dev.RemoteLEDEnabled ?? false) || dev.AncillaryBaseSounderGroup is null ? "--" : string.Format(Cultures.Resources.Group_x, dev.AncillaryBaseSounderGroup.Value), numRows, 1));
                //            }


                //            // I/O config
                //            int ioRowsUsed = 0;

                //            if (dev.IsIODevice)
                //            {
                //                List<string> subaddressNames = DeviceTypes.CurrentProtocolIsXfpCast && dev.DeviceType == (int)XfpCastDeviceTypeIds.HS2
                //                                                ? _xfpHushSubaddressNames
                //                                                : _defaultSubaddressNames;

                //                for (int io = 0; io < dev.IOConfig.Count; io++)
                //                {
                //                    if (dev.IOConfig[io].InputOutput != IOTypes.NotUsed)
                //                    {
                //                        var isGroup = dev.IOConfig[io].InputOutput == IOTypes.Output && dev.IsGroupedDevice;
                //                        var isSet   = dev.IOConfig[io].InputOutput == IOTypes.Output && !dev.IsZonalDevice;

                //                        if (dev.IOConfig[io].Index >= 0 && dev.IOConfig[io].Index < subaddressNames.Count)
                //                            newRows[io].Cells.Add(TableUtil.NewCell(subaddressNames[dev.IOConfig[io].Index]));
                //                        else
                //                            newRows[io].Cells.Add(TableUtil.NewCell(""));

                //                        newRows[io].Cells.Add(TableUtil.NewCell(CTecDevices.Enums.IOTypeToString(dev.IOConfig[io].InputOutput)));
                //                        newRows[io].Cells.Add(TableUtil.NewCell(((dev.IOConfig[io].Channel ?? 0) + 1).ToString()));
                //                        newRows[io].Cells.Add(TableUtil.NewCell(zgsDescription(true, isGroup, isSet, (int)dev.IOConfig[io].ZoneGroupSet)));
                //                        newRows[io].Cells.Add(TableUtil.NewCell(GetDeviceName?.Invoke(dev.IOConfig[io].NameIndex)));

                //                        ioRowsUsed++;
                //                    }
                //                }
                //            }

                //            var rowSpan = 1 + ioRowsUsed;
                //        }
                //    }

                //    table.RowGroups.Add(bodyGroup);
                doc.Blocks.Add(table);
        }
            catch (Exception ex) { }
            finally
            {
                PrintUtil.ResetFont();
            }
        }


        private void defineColumnHeaders(Table table, string reportHeader)
        {
            //measure required column widths for subaddress and IO headers
            var cellMargins      = (int)(PrintUtil.DefaultGridMargin.Left + PrintUtil.DefaultGridMargin.Right) + 1;
            
            var wIn  = (int)TableUtil.MeasureText(Cultures.Resources.Input).Width + 1;
            var wOut = (int)TableUtil.MeasureText(Cultures.Resources.Output).Width + 1;
            var ioWidth = Math.Max(wIn, wOut);
            ioWidth += cellMargins;
            
            //define column widths
            var wNum = 18;
            var wName = 30;

            var lenSounders = TableUtil.MeasureText(Cultures.Resources.Sounders).Width;
            var lenRelay1   = TableUtil.MeasureText(Cultures.Resources.Relay1).Width;
            var lenRelay2   = TableUtil.MeasureText(Cultures.Resources.Relay2).Width;
            var lenOutputs  = TableUtil.MeasureText(Cultures.Resources.Relay1).Width;

            //define table's columns
            _totalColumns = 15;
            table.Columns.Add(new TableColumn() { Width = new GridLength(wNum) });      //num
            table.Columns.Add(new TableColumn() { Width = new GridLength(wName) });     //name

            for (int i = 2; i < _totalColumns; i++)
                table.Columns.Add(new TableColumn());
            
            //define rows for the header
            var headerRow1 = new TableRow();
            var headerRow2 = new TableRow();

            headerRow1.Background = headerRow2.Background = PrintUtil.GridHeaderBackground;
            
            //var colNum = TableUtil.NewCell(Cultures.Resources.Number_Symbol, 3, 1, TextAlignment.Right, FontWeights.Bold);
            //colNum.Padding = new(0,0,5,0);
            //headerRow1.Cells.Add(colNum);

            headerRow1.Cells.Add(TableUtil.NewCell("",                                    1, 2, FontWeights.Bold));
            headerRow2.Cells.Add(TableUtil.NewCell(Cultures.Resources.Zone,               1, 2, FontWeights.Bold));
            headerRow1.Cells.Add(TableUtil.NewCell(Cultures.Resources.Output_Delays_Mins, 1, 4, TextAlignment.Center, FontWeights.Bold));
            headerRow1.Cells.Add(TableUtil.NewCell(Cultures.Resources.Functioning_With,   1, 2, TextAlignment.Center, FontWeights.Bold));

            headerRow2.Cells.Add(TableUtil.NewCell(Cultures.Resources.Sounders,  TextAlignment.Center, FontWeights.Bold));
            headerRow2.Cells.Add(TableUtil.NewCell(Cultures.Resources.Relay_1,   TextAlignment.Center, FontWeights.Bold));
            headerRow2.Cells.Add(TableUtil.NewCell(Cultures.Resources.Relay_2,   TextAlignment.Center, FontWeights.Bold));
            headerRow2.Cells.Add(TableUtil.NewCell(Cultures.Resources.Outputs,   TextAlignment.Center, FontWeights.Bold));
            headerRow2.Cells.Add(TableUtil.NewCell(Cultures.Resources.Detectors, TextAlignment.Center, FontWeights.Bold));
            headerRow2.Cells.Add(TableUtil.NewCell(Cultures.Resources.MCPs,      TextAlignment.Center, FontWeights.Bold));

            headerRow1.Cells.Add(TableUtil.NewCell(Cultures.Resources.Multiple_Alarms_End_Delays, 2, 1, FontWeights.Bold));
            headerRow1.Cells.Add(TableUtil.NewCell(Cultures.Resources.Day_Dependencies,           1, 3, TextAlignment.Center, FontWeights.Bold));
            headerRow1.Cells.Add(TableUtil.NewCell(Cultures.Resources.Night_Dependencies,         1, 3, TextAlignment.Center, FontWeights.Bold));

            var dayOptionCell     = TableUtil.NewCell(Cultures.Resources.Option,   TextAlignment.Center, FontWeights.Bold);
            var dayDetectorCell   = TableUtil.NewCell(Cultures.Resources.Detector, TextAlignment.Center, FontWeights.Bold);
            var dayAlarmCell      = TableUtil.NewCell(Cultures.Resources.Alarm,    TextAlignment.Center, FontWeights.Bold);
            var nightOptionCell   = TableUtil.NewCell(Cultures.Resources.Option,   TextAlignment.Center, FontWeights.Bold);
            var nightDetectorCell = TableUtil.NewCell(Cultures.Resources.Detector, TextAlignment.Center, FontWeights.Bold);
            var nightAlarmCell    = TableUtil.NewCell(Cultures.Resources.Alarm,    TextAlignment.Center, FontWeights.Bold);

            dayOptionCell.BorderBrush     = Styles.Brush06; dayOptionCell.BorderThickness     = new(0,0,0,0.25);
            dayDetectorCell.BorderBrush   = Styles.Brush06; dayDetectorCell.BorderThickness   = new(0,0,0,0.25);
            dayAlarmCell.BorderBrush      = Styles.Brush06; dayAlarmCell.BorderThickness      = new(0,0,0,0.25);
            nightOptionCell.BorderBrush   = Styles.Brush06; nightOptionCell.BorderThickness   = new(0,0,0,0.25);
            nightDetectorCell.BorderBrush = Styles.Brush06; nightDetectorCell.BorderThickness = new(0,0,0,0.25);
            nightAlarmCell.BorderBrush    = Styles.Brush06; nightAlarmCell.BorderThickness    = new(0,0,0,0.25);

            headerRow2.Cells.Add(dayOptionCell);
            headerRow2.Cells.Add(dayDetectorCell);
            headerRow2.Cells.Add(dayAlarmCell);
            headerRow2.Cells.Add(nightOptionCell);
            headerRow2.Cells.Add(nightDetectorCell);
            headerRow2.Cells.Add(nightAlarmCell);
           
            var headerGroup = new TableRowGroup();
            headerGroup.Rows.Add(headerRow1);
            headerGroup.Rows.Add(headerRow2);
            table.RowGroups.Add(headerGroup);
        }
    }
}
