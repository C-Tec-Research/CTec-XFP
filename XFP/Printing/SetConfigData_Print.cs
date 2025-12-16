using CTecControls.UI;
using CTecUtil.Printing;
using CTecUtil.Utils;
using System;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;
using Windows.ApplicationModel.Email.DataProvider;
using Xfp.UI.ViewHelpers;
using static Xfp.DataTypes.PanelData.GroupConfigData;

namespace Xfp.DataTypes.PanelData
{
    public partial class SetConfigData
    {
        public void GetReport(FlowDocument doc, XfpPanelData panelData)
        {
            _reportName = Cultures.Resources.Nav_Set_Configuration;
            _data = panelData;

            GridUtil.ResetDefaults();
            TableUtil.ResetDefaults();
            TableUtil.SetForeground(PrintUtil.TextForeground);
            TableUtil.SetFontSize(PrintUtil.PrintSmallerFontSize);
            TableUtil.SetFontWeight(FontWeights.Normal);
            TableUtil.SetFontFamily(PrintUtil.PrintDefaultFont);
            TableUtil.SetPadding(PrintUtil.DefaultTableMargin);

            PrintUtil.PageHeader(doc, string.Format(Cultures.Resources.Panel_x, panelData.PanelNumber) + " - " + _reportName);

            var headerSection = new Section();
            headerSection.Blocks.Add(key());
            doc.Blocks.Add(headerSection);
            doc.Blocks.Add(printSets());

            TableUtil.ResetDefaults();
        }
        

        private string _reportName;
        private XfpPanelData _data;
        private double _wNum;
        private double _wName;
        private double _wNumGroup;
        //private double _wPanelRelayTriggered = 40;
        private double _wArrow;
        private double _wSilenceable;
        private string _arrow = "←";
        private double _arrowFontSize = 10;
        private Size   _iconSize = new(18, 15);
        private int    _wDivider = 10;
        private const int _setIdColumns = 2;
        private const int _separatorColumns = 1;
        private const int _totalColumns = NumOutputSetTriggers + NumPanelRelayTriggers + _setIdColumns + _separatorColumns;
        private static SolidColorBrush _panelRelayHeaderBrush    = Xfp.UI.Styles.PrintPanelRelayHeaderBrush;
        private static string _triggerPulsedSvgData;
        private static string _triggerContinuousSvgData;
        private static string _triggerDelayedSvgData;
        private static string _triggerNotTriggeredSvgData;


        private void initializeIconPathData()
        {
            var svgPathConverter = new SetTriggerTypeToSvgPathConverter();
            _triggerPulsedSvgData       = (string)svgPathConverter.Convert(SetTriggerTypes.Pulsed,       typeof(string), null, null);
            _triggerContinuousSvgData   = (string)svgPathConverter.Convert(SetTriggerTypes.Continuous,   typeof(string), null, null);
            _triggerDelayedSvgData      = (string)svgPathConverter.Convert(SetTriggerTypes.Delayed,      typeof(string), null, null);
            _triggerNotTriggeredSvgData = (string)svgPathConverter.Convert(SetTriggerTypes.NotTriggered, typeof(string), null, null);
        }


        private BlockUIContainer key()
        {
            var notTriggeredText = Cultures.Resources.TriggerType_Not_Triggered;
            var pulsedText       = Cultures.Resources.TriggerType_Pulsed;
            var continuousText   = Cultures.Resources.TriggerType_Continuous;
            var delayedText      = Cultures.Resources.TriggerType_Delayed;

            var grid = new Grid();

            GridUtil.AddRowToGrid(grid);
            GridUtil.AddRowToGrid(grid);

            GridUtil.AddColumnToGrid(grid, _iconSize.Width);
            GridUtil.AddColumnToGrid(grid, TableUtil.MeasureText(notTriggeredText).Width + 15);
            GridUtil.AddColumnToGrid(grid, _iconSize.Width);
            GridUtil.AddColumnToGrid(grid, TableUtil.MeasureText(pulsedText).Width + 15);
            GridUtil.AddColumnToGrid(grid, _iconSize.Width);
            GridUtil.AddColumnToGrid(grid, TableUtil.MeasureText(continuousText).Width + 15);
            GridUtil.AddColumnToGrid(grid, _iconSize.Width);
            GridUtil.AddColumnToGrid(grid, TableUtil.MeasureText(delayedText).Width + 5);

            var row0 = GridUtil.GridCell("", 0, 0);
            row0.SetValue(Grid.HeightProperty, 10.0);
            
            grid.Children.Add(row0);
            grid.Children.Add(gridCellTriggerViewbox(SetTriggerTypes.NotTriggered, 1, 0, _iconSize));
            grid.Children.Add(GridUtil.GridCell(notTriggeredText, 1, 1));
            grid.Children.Add(gridCellTriggerViewbox(SetTriggerTypes.Pulsed, 1, 2, _iconSize));
            grid.Children.Add(GridUtil.GridCell(pulsedText, 1, 3));
            grid.Children.Add(gridCellTriggerViewbox(SetTriggerTypes.Continuous, 1, 4, _iconSize));
            grid.Children.Add(GridUtil.GridCell(continuousText, 1, 5));
            grid.Children.Add(gridCellTriggerViewbox(SetTriggerTypes.Delayed, 1, 6, _iconSize));
            grid.Children.Add(GridUtil.GridCell(delayedText, 1, 7));
            
            return new(grid);
        }


        //private Table printKey()
        //{
        //    try
        //    {
        //        var nWidth = TableUtil.MeasureText(Cultures.Resources.TriggerType_Not_Triggered).Width + 5;
        //        var pWidth = TableUtil.MeasureText(Cultures.Resources.TriggerType_Pulsed).Width + 5;
        //        var cWidth = TableUtil.MeasureText(Cultures.Resources.TriggerType_Continuous).Width + 5;
        //        var dWidth = TableUtil.MeasureText(Cultures.Resources.TriggerType_Delayed).Width + 15;

        //        var table = TableUtil.NewTable(_reportName);

        //        table.Columns.Add(new TableColumn() { Width = new GridLength(_iconSize.Width) });
        //        table.Columns.Add(new TableColumn() { Width = new GridLength(nWidth) });
        //        table.Columns.Add(new TableColumn() { Width = new GridLength(_iconSize.Width) });
        //        table.Columns.Add(new TableColumn() { Width = new GridLength(pWidth) });
        //        table.Columns.Add(new TableColumn() { Width = new GridLength(_iconSize.Width) });
        //        table.Columns.Add(new TableColumn() { Width = new GridLength(cWidth) });
        //        table.Columns.Add(new TableColumn() { Width = new GridLength(_iconSize.Width) });
        //        table.Columns.Add(new TableColumn() { Width = new GridLength(dWidth) });
        //        table.Columns.Add(new TableColumn() { Width = new GridLength(TableUtil.MeasureText(Cultures.Resources.Delay_Time).Width) });
        //        table.Columns.Add(new TableColumn());

        //        var bodyGroup = new TableRowGroup();
        //        var newRow = new TableRow();
        //        bodyGroup.Rows.Add(newRow);

        //        TableCell cell;
        //        newRow.Cells.Add(TableUtil.NewCellImage(GridCellTriggerIcon(SetTriggerTypes.NotTriggered), 1, 1, _iconSize));
        //        cell = TableUtil.NewCell(Cultures.Resources.TriggerType_Not_Triggered);     cell.Padding = new(2,5,0,0);  newRow.Cells.Add(cell);
        //        newRow.Cells.Add(TableUtil.NewCellImage(GridCellTriggerIcon(SetTriggerTypes.Pulsed), 1, 1, _iconSize));
        //        cell = TableUtil.NewCell(Cultures.Resources.TriggerType_Pulsed);            cell.Padding = new(2,5,0,0);  newRow.Cells.Add(cell);
        //        newRow.Cells.Add(TableUtil.NewCellImage(GridCellTriggerIcon(SetTriggerTypes.Continuous), 1, 1, _iconSize));
        //        cell = TableUtil.NewCell(Cultures.Resources.TriggerType_Continuous);        cell.Padding = new(2,5,0,0);  newRow.Cells.Add(cell);
        //        newRow.Cells.Add(TableUtil.NewCellImage(GridCellTriggerIcon(SetTriggerTypes.Delayed), 1, 1, _iconSize));
        //        cell = TableUtil.NewCell(Cultures.Resources.TriggerType_Delayed);           cell.Padding = new(2,5,0,0);  newRow.Cells.Add(cell);
        //        cell = TableUtil.NewCell(appendColon(Cultures.Resources.Delay_Time), 1, 1); cell.Padding = new(2,5,0,0);  newRow.Cells.Add(cell);

        //        if (ZoneConfigData.IsValidSetDelay(DelayTimer))
        //            cell = TableUtil.NewCellTime(DelayTimer, "ms", true, TextAlignment.Left, true);
        //        else 
        //            cell = TableUtil.NewCellTimeError(DelayTimer, "ms", true, TextAlignment.Left);
        //                                                                                    cell.Padding = new(2,5,0,0);  newRow.Cells.Add(cell);

        //        table.RowGroups.Add(bodyGroup);
        //        return table;
        //    }
        //    catch (Exception ex)
        //    {
        //        CTecMessageBox.ShowException(string.Format(CTecUtil.Cultures.Resources.Error_Generating_Report_x, _reportName), CTecUtil.Cultures.Resources.Error_Printing, ex);
        //        return null;
        //    }
        //}


        private Table printSets()
        {
            initializeIconPathData();

            int dataRows = 0;

            try
            {
                var table = TableUtil.NewTable(_reportName);

                defineColumnHeaders(table, _reportName);
                printSilenceableSets(table);

                var bodyGroup = new TableRowGroup();

                for (int i = 0; i < Sets.Count; i++)
                {
                    dataRows++;

                    //create the new row
                    var newRow = new TableRow() { Background = Int32.IsEvenInteger(dataRows) ? PrintUtil.TableAlternatingRowBackground : PrintUtil.NoBackground };
                    bodyGroup.Rows.Add(newRow);

                    if (i < _data.ZoneConfig.Zones.Count)
                    {
                        newRow.Cells.Add(TableUtil.NewCell((_data.ZoneConfig.Zones[i].Index + 1).ToString(), 1, 1, TextAlignment.Right));                   //zone number
                        newRow.Cells.Add(TableUtil.NewCell(_data.ZoneConfig.Zones[i].Name));                                                                //zone name

                        for (int t = 0; t < NumOutputSetTriggers; t++)
                            newRow.Cells.Add(TableUtil.NewCellImage(GridCellTriggerIcon(Sets[i].OutputSetTriggers[t]), 1, 1, _iconSize));

                        newRow.Cells.Add(TableUtil.NewCell(""));

                        for (int t = 0; t < NumPanelRelayTriggers; t++)
                            newRow.Cells.Add(TableUtil.NewCellImage(GridCellTriggerIcon(Sets[i].PanelRelayTriggers[t]), 1, 1, _iconSize));
                    }
                    else if (i < _data.ZoneConfig.Zones.Count + _data.ZonePanelConfig.Panels.Count)
                    {
                        var ii = i - _data.ZoneConfig.Zones.Count;
                        newRow.Cells.Add(TableUtil.NewCell((_data.ZonePanelConfig.Panels[ii].Index + 1).ToString(), 1, 1, TextAlignment.Right));            //panel number
                        newRow.Cells.Add(TableUtil.NewCell(_data.ZonePanelConfig.Panels[ii].Name));                                                         //panel name

                        for (int t = 0; t < NumOutputSetTriggers; t++)
                            newRow.Cells.Add(TableUtil.NewCellImage(GridCellTriggerIcon(Sets[i].OutputSetTriggers[t]), 1, 1, _iconSize));

                        newRow.Cells.Add(TableUtil.NewCell(""));

                        for (int t = 0; t < NumPanelRelayTriggers; t++)
                            newRow.Cells.Add(TableUtil.NewCellImage(GridCellTriggerIcon(Sets[i].PanelRelayTriggers[t]), 1, 1, _iconSize));
                    }
                }

                table.RowGroups.Add(bodyGroup);
                return table;
            }
            catch (Exception ex)
            {
                CTecMessageBox.ShowException(string.Format(CTecUtil.Cultures.Resources.Error_Generating_Report_x, _reportName), CTecUtil.Cultures.Resources.Error_Printing, ex);
                return null;
            }
            finally
            {
                PrintUtil.ResetFont();
            }
        }


        private void printSilenceableSets(Table table)
        {
            var bodyGroup = new TableRowGroup();
            var sisetRow = new TableRow() { Background = Xfp.UI.Styles.Brush07 };
            bodyGroup.Rows.Add(sisetRow);

            sisetRow.Cells.Add(TableUtil.NewCell(Cultures.Resources.Zone, 1, 2, FontWeights.Bold));
            
            for (int t = 0; t < NumOutputSetTriggers; t++)
                sisetRow.Cells.Add(TableUtil.NewCellBool(_data.ZoneConfig.OutputSetIsSilenceable[t], 1, 1, true, 13, FontWeights.Normal, TextAlignment.Center));

            sisetRow.Cells.Add(TableUtil.NewCell(""));
            
            for (int t = 0; t < NumPanelRelayTriggers; t++)
                sisetRow.Cells.Add(TableUtil.NewCellBool(_data.ZoneConfig.OutputSetIsSilenceable[t], 1, 1, true, 13, FontWeights.Normal, TextAlignment.Center));
            
            sisetRow.Cells.Add(TableUtil.NewCell(_arrow, _arrowFontSize, TextAlignment.Right));
            sisetRow.Cells.Add(TableUtil.NewCell(Cultures.Resources.Is_Set_Silenceable));

            table.RowGroups.Add(bodyGroup);
        }


        private void defineColumnHeaders(Table table, string reportHeader)
        {     
            setColumnWidths();

            //define table's columns
            table.Columns.Add(new TableColumn() { Width = new GridLength(_wNum) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_wName) });
            for (int i = 0; i < NumOutputSetTriggers; i++)
                table.Columns.Add(new TableColumn() { Width = new GridLength(_iconSize.Width) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_wDivider) });
            for (int i = 0; i < NumPanelRelayTriggers; i++)
                table.Columns.Add(new TableColumn() { Width = new GridLength(_iconSize.Width) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_wArrow) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_wSilenceable) });

            //define rows for the header
            var headerRow1 = new TableRow();
            var headerRow2 = new TableRow();
            var headerRow3 = new TableRow();

            headerRow1.Background = headerRow2.Background = headerRow3.Background = PrintUtil.TableHeaderBackground;
            
            headerRow1.Cells.Add(TableUtil.NewCell("", 1, 2));
            headerRow2.Cells.Add(TableUtil.NewCell("", 1, 2));
            headerRow3.Cells.Add(TableUtil.NewCell("", 1, 2));
            
            headerRow2.Cells.Add(TableUtil.UnderlineCell(TableUtil.NewCell(Cultures.Resources.Output_Set_Triggered, 1, NumOutputSetTriggers, TextAlignment.Center, FontWeights.Bold), Styles.Brush05));
            headerRow1.Cells.Add(TableUtil.NewCell("", 1, 17));
            headerRow1.Cells.Add(TableUtil.UnderlineCell(TableUtil.NewCell(Cultures.Resources.Panel_Relay_Triggered, 2, NumPanelRelayTriggers + 1, TextAlignment.Left, FontWeights.Bold), Styles.Brush05));
            
            for (int i = 0; i < NumOutputSetTriggers; i++)
                headerRow3.Cells.Add(TableUtil.NewCell((i + 1).ToString(), TextAlignment.Center, FontWeights.Bold));

            headerRow3.Cells.Add(TableUtil.NewCell(""));

            for (int i = 0; i < NumPanelRelayTriggers; i++)
                headerRow3.Cells.Add(TableUtil.NewCell((i + 1).ToString(), TextAlignment.Center, FontWeights.Bold));
    
            var headerGroup = new TableRowGroup();
            headerGroup.Rows.Add(headerRow1);
            headerGroup.Rows.Add(headerRow2);
            headerGroup.Rows.Add(headerRow3);

            table.RowGroups.Add(headerGroup);
        }


        private void setColumnWidths()
        {
            var cellMargins = (int)(PrintUtil.DefaultTableMargin.Left + PrintUtil.DefaultTableMargin.Right) + 1;
            
            //measure required column widths for columns
            _wNum         = TableUtil.MeasureText("99").Width + 1;
            _wName        = _data.ZoneConfig.GetMaxZoneNameLength();
            _wNumGroup    = Math.Max(_wNum + _wName, TableUtil.MeasureText(Cultures.Resources.Zone).Width) + cellMargins + 1;
            _wArrow       = FontUtil.MeasureText(_arrow, TableUtil.FontFamily, _arrowFontSize, TableUtil.FontStyle, FontWeights.Normal, TableUtil.FontStretch).Width + cellMargins + 1;
            _wSilenceable = TableUtil.MeasureText(Cultures.Resources.Is_Set_Silenceable).Width;
        }

        
        public static Grid getIconPath(SetTriggerTypes value, int row, int column)
        {
            var result = new Grid();

            var vb = new Viewbox() { Width = 14, Height = 14, HorizontalAlignment = HorizontalAlignment.Center };

            var colour = value switch
            {
                SetTriggerTypes.Pulsed     => Xfp.UI.Styles.TriggerPulsedBrush,
                SetTriggerTypes.Continuous => Xfp.UI.Styles.TriggerContinuousBrush,
                SetTriggerTypes.Delayed    => Xfp.UI.Styles.TriggerDelayedBrush,
                _                          => Xfp.UI.Styles.TriggerNotTriggeredBrush,
            };

            var pathData = value switch
            {
                SetTriggerTypes.Pulsed     => _triggerPulsedSvgData,
                SetTriggerTypes.Continuous => _triggerContinuousSvgData,
                SetTriggerTypes.Delayed    => _triggerDelayedSvgData,
                _                          => _triggerNotTriggeredSvgData,
            };

            var p = new System.Windows.Shapes.Path() { Fill = colour, Data = Geometry.Parse(pathData) };
            vb.Child = p;

            result.SetValue(Grid.RowProperty, row);
            result.SetValue(Grid.ColumnProperty, column);
            result.Children.Add(vb);

            return result;
        }


        private static Viewbox gridCellTriggerViewbox(SetTriggerTypes triggerType, int row, int column, Size size)
        {
            var result = new Viewbox() { Width = size.Width, Height = size.Height, HorizontalAlignment = HorizontalAlignment.Center };
            result.Child = SetConfigData.getIconPath(triggerType, row, column);
            result.SetValue(Grid.RowProperty, row);
            result.SetValue(Grid.ColumnProperty, column);
            result.SetValue(Grid.WidthProperty, size.Width);
            result.SetValue(Grid.HeightProperty, size.Height);
            result.SetValue(Grid.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            result.SetValue(Grid.VerticalAlignmentProperty, VerticalAlignment.Center);
            return result;
        }

        
        public static System.Windows.Shapes.Path GridCellTriggerIcon(SetTriggerTypes value)
        {
            
            var colour = value switch
            {
                SetTriggerTypes.Pulsed     => Xfp.UI.Styles.TriggerPulsedBrush,
                SetTriggerTypes.Continuous => Xfp.UI.Styles.TriggerContinuousBrush,
                SetTriggerTypes.Delayed    => Xfp.UI.Styles.TriggerDelayedBrush,
                _                          => Xfp.UI.Styles.TriggerNotTriggeredBrush,
            };

            var pathData = value switch
            {
                SetTriggerTypes.Pulsed     => _triggerPulsedSvgData,
                SetTriggerTypes.Continuous => _triggerContinuousSvgData,
                SetTriggerTypes.Delayed    => _triggerDelayedSvgData,
                _                          => _triggerNotTriggeredSvgData,
            };

            return new System.Windows.Shapes.Path() { Fill = colour, Data = Geometry.Parse(pathData) };
        }
    }
}
