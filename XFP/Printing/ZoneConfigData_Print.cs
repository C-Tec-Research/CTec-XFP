using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using CTecControls.UI;
using CTecUtil.Printing;
using CTecUtil.Utils;
using Xfp.ViewModels.PanelTools;

namespace Xfp.DataTypes.PanelData
{
    public partial class ZoneConfigData
    {
        public void GetReport(FlowDocument doc, XfpPanelData panelData, ref int pageNumber)
        {
            //if (pageNumber++ > 1)
            //    PrintUtil.InsertPageBreak(doc);
            
            TableUtil.ResetDefaults();
            TableUtil.SetForeground(PrintUtil.TextForeground);
            TableUtil.SetFontSize(PrintUtil.PrintSmallerFontSize);
            TableUtil.SetFontStretch(PrintUtil.FontNarrowWidth);
            TableUtil.SetFontWeight(FontWeights.Normal);
            TableUtil.SetFontFamily(PrintUtil.PrintDefaultFont);
            TableUtil.SetPadding(PrintUtil.DefaultGridMargin);

            GridUtil.ResetDefaults();
            //GridUtil.SetForeground(PrintUtil.TextForeground);
            //GridUtil.SetFontSize(PrintUtil.PrintSmallerFontSize);
            //GridUtil.SetFontStretch(PrintUtil.FontNarrowWidth);
            //GridUtil.SetFontWeight(FontWeights.Normal);
            //GridUtil.SetFontFamily(PrintUtil.PrintDefaultFont);
            //GridUtil.SetPadding(PrintUtil.DefaultGridMargin);

            PrintUtil.PageHeader(doc, string.Format(Cultures.Resources.Panel_x, panelData.PanelNumber) + " - " + Cultures.Resources.Nav_Zone_Configuration);
            
            var headerSection = new Section();
            headerSection.Blocks.Add(headerInfo());
            
            doc.Blocks.Add(headerSection);
            doc.Blocks.Add(printZones());

            TableUtil.ResetDefaults();
            //GridUtil.ResetDefaults();
        }


        private int    _numColumns;

        private double _wNum;
        private double _wName = 0;
        private double _wOutputDelaysCol;
        private double _wFunctioningWithCol;
        private double _wMultipleAlarmsEndDelays;
        private double _wOption; 
        private double _wDetAlarmCol; 
        private double _wArrow;
        private string _arrow = "→";
        private double _arrowFontSize = 10;


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


        private Table printZones()
        {
            int dataRows = 0;

            var reportName = Cultures.Resources.Nav_Zone_Configuration;

            try
            {
                var table = TableUtil.NewTable(reportName);

                defineColumnHeaders(table, reportName);

                var bodyGroup = new TableRowGroup();

                foreach (var z in Zones)
                {
                    dataRows++;

                    //create the new row
                    var newRow = new TableRow() { Background = Int32.IsEvenInteger(dataRows) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground };
                    bodyGroup.Rows.Add(newRow);

                    newRow.Cells.Add(TableUtil.NewCell((z.Index + 1).ToString(), 1, 1, TextAlignment.Right));           //zone number
                    newRow.Cells.Add(TableUtil.NewCell(z.Name));                                                        //zone name
                    newRow.Cells.Add(TableUtil.NewCellTime(z.SounderDelay, "ms", false, TextAlignment.Center));         //sounder
                    newRow.Cells.Add(TableUtil.NewCellTime(z.Relay1Delay, "ms", false, TextAlignment.Center));          //relay1
                    newRow.Cells.Add(TableUtil.NewCellTime(z.Relay2Delay, "ms", false, TextAlignment.Center));          //relay2
                    newRow.Cells.Add(TableUtil.NewCellTime(z.RemoteDelay, "ms", false, TextAlignment.Center));          //outputs
                    newRow.Cells.Add(TableUtil.NewCell(""));
                    newRow.Cells.Add(TableUtil.NewCellBool(z.Detectors, false, TextAlignment.Center));                  //functioning with detectors
                    newRow.Cells.Add(TableUtil.NewCellBool(z.MCPs,      false, TextAlignment.Center));                  //functioning with MCPs
                    newRow.Cells.Add(TableUtil.NewCellBool(z.EndDelays, false, TextAlignment.Center));                  //multiple alarms end delays
                    
                    newRow.Cells.Add(TableUtil.NewCell(Enums.ZoneDependencyOptionToString(z.Day.DependencyOption)));    //day dependency option          

                    if (ZoneConfigData.HasDetectorReset(z.Day.DependencyOption))                               //day dependency detector reset
                        newRow.Cells.Add(TableUtil.NewCellTime(z.Day.DetectorReset, "ms", false, TextAlignment.Center));
                    else
                        newRow.Cells.Add(TableUtil.NewCell("-", TextAlignment.Center));

                    if (ZoneConfigData.HasAlarmReset(z.Day.DependencyOption))                                  //day dependency alarm reset
                        newRow.Cells.Add(TableUtil.NewCellTime(z.Day.AlarmReset, "ms", false, TextAlignment.Center));
                    else
                        newRow.Cells.Add(TableUtil.NewCell("-", TextAlignment.Center));

                    newRow.Cells.Add(TableUtil.NewCell(Enums.ZoneDependencyOptionToString(z.Night.DependencyOption)));  //night dependency option

                    if (ZoneConfigData.HasDetectorReset(z.Night.DependencyOption))                             //night dependency detector reset
                        newRow.Cells.Add(TableUtil.NewCellTime(z.Night.DetectorReset, "ms", false, TextAlignment.Center));
                    else
                        newRow.Cells.Add(TableUtil.NewCell("-", TextAlignment.Center));

                    if (ZoneConfigData.HasAlarmReset(z.Night.DependencyOption))                                //night dependency alarm reset
                        newRow.Cells.Add(TableUtil.NewCellTime(z.Night.AlarmReset, "ms", false, TextAlignment.Center));
                    else
                        newRow.Cells.Add(TableUtil.NewCell("-", TextAlignment.Center));
                }
                
                foreach (var p in ZonePanelData?.Panels)
                {
                    dataRows++;

                    //create the new row
                    var newRow = new TableRow() { Background = Int32.IsEvenInteger(dataRows) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground };
                    bodyGroup.Rows.Add(newRow);

                    newRow.Cells.Add(TableUtil.NewCell((p.Index + 1).ToString(), 1, 1, TextAlignment.Right));       //zone number
                    newRow.Cells.Add(TableUtil.NewCell(p.Name));                                                    //zone name

                    newRow.Cells.Add(TableUtil.NewCellTime(p.SounderDelay, "ms", false, TextAlignment.Center));         //sounder delay
                    newRow.Cells.Add(TableUtil.NewCellTime(p.Relay1Delay, "ms", false, TextAlignment.Center));          //sounder delay
                    newRow.Cells.Add(TableUtil.NewCellTime(p.Relay2Delay, "ms", false, TextAlignment.Center));          //sounder delay
                    newRow.Cells.Add(TableUtil.NewCellTime(p.RemoteDelay, "ms", false, TextAlignment.Center));          //sounder delay

                    newRow.Cells.Add(TableUtil.NewCell(""));
                    newRow.Cells.Add(TableUtil.NewCell("-", TextAlignment.Center));
                    newRow.Cells.Add(TableUtil.NewCell("-", TextAlignment.Center));
                    newRow.Cells.Add(TableUtil.NewCell("-", TextAlignment.Center));
                    newRow.Cells.Add(TableUtil.NewCell("  --"));
                    newRow.Cells.Add(TableUtil.NewCell("-", TextAlignment.Center));
                    newRow.Cells.Add(TableUtil.NewCell("-", TextAlignment.Center));
                    newRow.Cells.Add(TableUtil.NewCell("  --"));
                    newRow.Cells.Add(TableUtil.NewCell("-", TextAlignment.Center));
                    newRow.Cells.Add(TableUtil.NewCell("-", TextAlignment.Center));
                }

                table.RowGroups.Add(bodyGroup);
                return table;
            }
            catch (Exception ex)
            {
                CTecMessageBox.ShowException(string.Format(CTecUtil.Cultures.Resources.Error_Generating_Report_x, reportName), CTecUtil.Cultures.Resources.Error_Printing, ex);
                return null;
            }
            finally
            {
                PrintUtil.ResetFont();
            }
        }


        private void defineColumnHeaders(Table table, string reportHeader)
        {            
            setColumnWidths();

            //define table's columns
            table.Columns.Add(new TableColumn() { Width = new GridLength(_wNum) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_wName) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_wOutputDelaysCol) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_wOutputDelaysCol) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_wOutputDelaysCol) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_wOutputDelaysCol) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_wArrow) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_wFunctioningWithCol) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_wFunctioningWithCol) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_wMultipleAlarmsEndDelays) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_wOption) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_wDetAlarmCol) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_wDetAlarmCol) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_wOption) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_wDetAlarmCol ) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_wDetAlarmCol) });

            
            //define rows for the header
            var headerRow1 = new TableRow();
            var headerRow2 = new TableRow();
            var headerRow3 = new TableRow();

            headerRow1.Background = headerRow2.Background = headerRow3.Background = PrintUtil.GridHeaderBackground;
            
            headerRow1.Cells.Add(TableUtil.NewCell("", 1, 2, FontWeights.Bold));
            headerRow2.Cells.Add(TableUtil.NewCell("", 1, 2, FontWeights.Bold));
            headerRow3.Cells.Add(TableUtil.NewCell(Cultures.Resources.Zone, 1, 2, FontWeights.Bold));

            headerRow2.Cells.Add(TableUtil.UnderlineCell(TableUtil.NewCell(Cultures.Resources.Output_Delays_Mins, 1, 4, TextAlignment.Center, FontWeights.Bold), Styles.Brush04));
            headerRow2.Cells.Add(TableUtil.NewCell(_arrow));
            headerRow2.Cells.Add(TableUtil.UnderlineCell(TableUtil.NewCell(Cultures.Resources.Functioning_With, 1, 2, TextAlignment.Center, FontWeights.Bold), Styles.Brush04));
            
            headerRow1.Cells.Add(TableUtil.NewCell("", 1, 7));
            headerRow1.Cells.Add(TableUtil.NewCell(Cultures.Resources.Multiple_Alarms_End_Delays, 3, 1, TextAlignment.Center, FontWeights.Bold));

            headerRow3.Cells.Add(TableUtil.NewCell(Cultures.Resources.Sounders, TextAlignment.Center, FontWeights.Bold));
            headerRow3.Cells.Add(TableUtil.NewCell(Cultures.Resources.Relay_1, TextAlignment.Center, FontWeights.Bold));
            headerRow3.Cells.Add(TableUtil.NewCell(Cultures.Resources.Relay_2, TextAlignment.Center, FontWeights.Bold));
            headerRow3.Cells.Add(TableUtil.NewCell(Cultures.Resources.Outputs, TextAlignment.Center, FontWeights.Bold));
            headerRow3.Cells.Add(TableUtil.NewCell(""));
            headerRow3.Cells.Add(TableUtil.NewCell(Cultures.Resources.Detectors, TextAlignment.Center, FontWeights.Bold));
            headerRow3.Cells.Add(TableUtil.NewCell(Cultures.Resources.MCPs, TextAlignment.Center, FontWeights.Bold));

            headerRow2.Cells.Add(TableUtil.UnderlineCell(TableUtil.NewCell(Cultures.Resources.Day_Dependencies, 1, 3, TextAlignment.Center, FontWeights.Bold), Styles.Brush04));
            headerRow2.Cells.Add(TableUtil.UnderlineCell(TableUtil.NewCell(Cultures.Resources.Night_Dependencies, 1, 3, TextAlignment.Center, FontWeights.Bold), Styles.Brush04));

            headerRow3.Cells.Add(TableUtil.NewCell(Cultures.Resources.Option, TextAlignment.Left, FontWeights.Bold));
            headerRow3.Cells.Add(TableUtil.NewCell(Cultures.Resources.Detector, TextAlignment.Center, FontWeights.Bold));
            headerRow3.Cells.Add(TableUtil.NewCell(Cultures.Resources.Alarm, TextAlignment.Center, FontWeights.Bold));
            headerRow3.Cells.Add(TableUtil.NewCell(Cultures.Resources.Option, TextAlignment.Left, FontWeights.Bold));
            headerRow3.Cells.Add(TableUtil.NewCell(Cultures.Resources.Detector, TextAlignment.Center, FontWeights.Bold));
            headerRow3.Cells.Add(TableUtil.NewCell(Cultures.Resources.Alarm, TextAlignment.Center, FontWeights.Bold));

            var headerGroup = new TableRowGroup();
            headerGroup.Rows.Add(headerRow1);
            headerGroup.Rows.Add(headerRow2);
            headerGroup.Rows.Add(headerRow3);

//var headerRow4 = new TableRow() { Background = headerRow1.Background };
//var headerGrid = columnHeadersAsGrid();
//var headerCell = new TableCell() { RowSpan = 1, ColumnSpan = _numColumns };
//headerCell.Blocks.Add(new BlockUIContainer(headerGrid));
//headerRow4.Cells.Add(headerCell);
//headerGroup.Rows.Add(headerRow4);

            table.RowGroups.Add(headerGroup);
        }


        private void setColumnWidths()
        {
            var cellMargins = (int)(PrintUtil.DefaultGridMargin.Left + PrintUtil.DefaultGridMargin.Right) + 1;
            
            //measure required column widths for columns
            _wNum     = TableUtil.MeasureText("99").Width + 1;
            _wName    = GetMaxZoneNameLength();
            //_wNumZone = Math.Max(_wNum + _wName, TableUtil.MeasureText(Cultures.Resources.Zone).Width) + cellMargins + 1;

            var wTime     = TableUtil.MeasureText("00:00").Width + cellMargins + 1;
            var wSounders = TableUtil.MeasureText(Cultures.Resources.Sounders).Width + cellMargins + 1;
            var wRelay1   = TableUtil.MeasureText(Cultures.Resources.Relay_1).Width + cellMargins + 1;
            var wRelay2   = TableUtil.MeasureText(Cultures.Resources.Relay_2).Width + cellMargins + 1;
            var wOutputs  = TableUtil.MeasureText(Cultures.Resources.Outputs).Width + cellMargins + 1;
            _wArrow       = FontUtil.MeasureText(_arrow, TableUtil.FontFamily, _arrowFontSize, TableUtil.FontStyle, FontWeights.Normal, TableUtil.FontStretch).Width + 1;

            _wOutputDelaysCol  = Math.Max(wTime, Math.Max(wSounders, Math.Max(wRelay1, Math.Max(wRelay2, wOutputs)))) + 1;
            //_wOutputDelaysMins = Math.Max(_wOutputDelaysCol * 4, TableUtil.MeasureText(Cultures.Resources.Output_Delays_Mins).Width + cellMargins) + 1;

            var wDetectors = TableUtil.MeasureText(Cultures.Resources.Detectors).Width + cellMargins + 1;
            var wMCPs      = TableUtil.MeasureText(Cultures.Resources.MCPs).Width + cellMargins + 1;
            _wFunctioningWithCol = Math.Max(wDetectors, wMCPs);
            //_wFunctioningWith    = Math.Max(_wFunctioningWithCol * 4, TableUtil.MeasureText(Cultures.Resources.Functioning_With).Width + cellMargins + 1);

            _wMultipleAlarmsEndDelays = 42;

            var wDepOptionNotSet = TableUtil.MeasureText(Cultures.Resources.Not_Set).Width + cellMargins + 1;
            var wDepOptionNormal = TableUtil.MeasureText(Cultures.Resources.Zone_Dependency_Normal).Width + cellMargins + 1;
            var wDepOptionInvest = TableUtil.MeasureText(Cultures.Resources.Zone_Dependency_Investigation).Width + cellMargins + 1;
            var wDepOptionDwell  = TableUtil.MeasureText(Cultures.Resources.Zone_Dependency_Dwelling).Width + cellMargins + 1;
            var wDepOptionA      = TableUtil.MeasureText(Cultures.Resources.Zone_Dependency_A).Width + cellMargins + 1;
            var wDepOptionB      = TableUtil.MeasureText(Cultures.Resources.Zone_Dependency_B).Width + cellMargins + 1;
            var wDepOptionC      = TableUtil.MeasureText(Cultures.Resources.Zone_Dependency_C).Width + cellMargins + 1;
            var wDepOptions      = Math.Max(wDepOptionNotSet, Math.Max(wDepOptionNormal, Math.Max(wDepOptionInvest, Math.Max(wDepOptionDwell, Math.Max(wDepOptionA, Math.Max(wDepOptionB, wDepOptionC))))));
            _wOption   = Math.Max(wDepOptions, TableUtil.MeasureText(Cultures.Resources.Option).Width + cellMargins + 1);
            var wDetector = TableUtil.MeasureText(Cultures.Resources.Detector).Width + cellMargins + 1;
            var wAlarm    = TableUtil.MeasureText(Cultures.Resources.Alarm).Width + cellMargins + 1;
            _wDetAlarmCol = Math.Max(wTime, Math.Max(wDetector, wAlarm));

            var wDayDependenciesText   = TableUtil.MeasureText(Cultures.Resources.Day_Dependencies).Width + cellMargins + 1;
            var wNightDependenciesText = TableUtil.MeasureText(Cultures.Resources.Night_Dependencies).Width + cellMargins + 1;

            if (wDayDependenciesText   > _wOption + _wDetAlarmCol * 2) _wDetAlarmCol = (wDayDependenciesText - _wOption) / 2;
            if (wNightDependenciesText > _wOption + _wDetAlarmCol * 2) _wDetAlarmCol = (wNightDependenciesText - _wOption) / 2;
        }


        public double GetMaxZoneNameLength()
        {
            var result = 0.0;

            foreach (var z in Zones)
            {
                var nameWidth = TableUtil.MeasureText(z.Name).Width + 1;
                if (nameWidth > result)
                    result = nameWidth;
            }
            foreach (var p in  ZonePanelData?.Panels)
            {
                var nameWidth = TableUtil.MeasureText(p.Name).Width + 1;
                if (nameWidth > result)
                    result = nameWidth;
            }

            return result;
        }

        
  //      private Grid columnHeadersAsGrid()
  //      {
  //          Grid grid = new Grid();

  //          //two rows for column headers
  //          GridUtil.AddRowToGrid(grid);
  //          GridUtil.AddRowToGrid(grid);

  //          for (int i = 0; i < 15; i++)
  //          {
  //              if (i == 8)
  //                  GridUtil.AddColumnToGrid(grid, 60);
  //              else
  //                  GridUtil.AddColumnToGrid(grid);
  //          }

  //          GridUtil.SetForeground(PrintUtil.TextForeground);
  //          TableUtil.SetFontSize(PrintUtil.PrintSmallerFontSize);
  //          TableUtil.SetFontStretch(PrintUtil.FontNarrowWidth);
  //          TableUtil.SetFontWeight(FontWeights.Normal);
  //          TableUtil.SetFontFamily(PrintUtil.PrintDefaultFont);
  //          TableUtil.SetPadding(PrintUtil.DefaultGridMargin);


  //          grid.Children.Add(GridUtil.GridBackground(0, 0, 1, 15, PrintUtil.GridHeaderBackground));
  //          grid.Children.Add(GridUtil.GridBackground(1, 0, 1, 15, PrintUtil.GridHeaderBackground));
  //          grid.Children.Add(GridUtil.GridBackground(2, 0, 1, 15, PrintUtil.GridHeaderBackground));

  //          grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Output_Delays_Mins,         0,  2, 1, 4, HorizontalAlignment.Center, VerticalAlignment.Center, _wOutputDelaysMins));
  //          grid.Children.Add(GridUtil.GridHeaderCell("→",                                           0,  2, 1, 4, HorizontalAlignment.Right));
            
  //          grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Functioning_With,           0,  6, 1, 2, HorizontalAlignment.Center, VerticalAlignment.Center, _wFunctioningWith));
            
  //          grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Multiple_Alarms_End_Delays, 0,  8, 3, 1, HorizontalAlignment.Center, VerticalAlignment.Bottom, _wMultipleAlarmsEndDelays));
  //          grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Day_Dependencies,           0,  9, 1, 3, HorizontalAlignment.Center, VerticalAlignment.Center, _wDayDependencies));
  //          grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Night_Dependencies,         0, 12, 1, 3, HorizontalAlignment.Center, VerticalAlignment.Center, _wNightDependencies));
            
  //          GridUtil.AddBorderToGrid(grid, 1, 2, 1, 4, _headerBorderBrush, new Thickness(1, 1, 1, 0), new CornerRadius(0), new(1.5, -5, 1.5, 0), 3);
  //          GridUtil.AddBorderToGrid(grid, 1, 6, 1, 1, _headerBorderBrush, new Thickness(1, 1, 1, 0), new CornerRadius(0), new(1.5, -5, 1.5, 0), 3);
  //          GridUtil.AddBorderToGrid(grid, 1, 9, 1, 3, _headerBorderBrush, new Thickness(1, 1, 1, 0), new CornerRadius(0), new(1.5, -5, 1.5, 0), 3);
  //          GridUtil.AddBorderToGrid(grid, 1,12, 1, 3, _headerBorderBrush, new Thickness(1, 1, 1, 0), new CornerRadius(0), new(1.5, -5, 1.5, 0), 3);

  //          _numColumns = 0;
  //          grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Zone,      2, _numColumns++, 1, 2, HorizontalAlignment.Left, VerticalAlignment.Center, _wNumZone));
  //          _numColumns++;
  //          grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Sounders,  2, _numColumns++, HorizontalAlignment.Left, VerticalAlignment.Bottom, _wNumZone));
  //          grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Relay_1,   2, _numColumns++, HorizontalAlignment.Left, VerticalAlignment.Bottom, _wNumZone));
  //          grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Relay_2,   2, _numColumns++, HorizontalAlignment.Left, VerticalAlignment.Bottom, _wNumZone));
  //          grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Outputs,   2, _numColumns++, HorizontalAlignment.Left, VerticalAlignment.Bottom, _wNumZone));
  //          grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Detectors, 2, _numColumns++, HorizontalAlignment.Left, VerticalAlignment.Bottom, _wNumZone));
  //          grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.MCPs,      2, _numColumns++, HorizontalAlignment.Left, VerticalAlignment.Bottom, _wNumZone));
  //          _numColumns++;
  //          grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Option,    2, _numColumns++, HorizontalAlignment.Center, VerticalAlignment.Bottom, _wNumZone));
  //          grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Detector,  2, _numColumns++, HorizontalAlignment.Center, VerticalAlignment.Bottom, _wNumZone));
  //          grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Alarm,     2, _numColumns++, HorizontalAlignment.Center, VerticalAlignment.Bottom, _wNumZone));
  //          grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Option,    2, _numColumns++, HorizontalAlignment.Center, VerticalAlignment.Bottom, _wNumZone));
  //          grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Detector,  2, _numColumns++, HorizontalAlignment.Center, VerticalAlignment.Bottom, _wNumZone));
  //          grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Alarm,     2, _numColumns++, HorizontalAlignment.Center, VerticalAlignment.Bottom, _wNumZone));

  //          return grid;
		//}
    }
}
