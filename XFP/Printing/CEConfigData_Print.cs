using CTecUtil.Printing;
using CTecUtil.UI;
using CTecUtil.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;
using Xfp.DataTypes;
using Xfp.UI.ViewHelpers;
using static System.Windows.Forms.Design.AxImporter;

namespace Xfp.DataTypes.PanelData
{
    public partial class CEConfigData
    {
        public void GetReport(FlowDocument doc, int panelNumber, XfpData data, ref int pageNumber)
        {
            if (pageNumber++ > 1)
                PrintUtil.InsertPageBreak(doc);

            _panelNumber = panelNumber;
            _data = data;

            var svgPathConverter = new SetTriggerTypeToSvgPathConverter();

            initLists();
            
            GridUtil.ResetDefaults();
            TableUtil.ResetDefaults();
            TableUtil.SetForeground(PrintUtil.TextForeground);
            TableUtil.SetFontSize(PrintUtil.PrintSmallerFontSize);
            TableUtil.SetFontWeight(FontWeights.Normal);
            TableUtil.SetFontFamily(PrintUtil.PrintDefaultFont);
            TableUtil.SetPadding(PrintUtil.DefaultGridMargin);

            PrintUtil.PageHeader(doc, string.Format(Cultures.Resources.Panel_x, panelNumber) + " - " + Cultures.Resources.Nav_C_And_E_Configuration);

            var headerSection = new Section();
            headerSection.Blocks.Add(timerEventTimes());
            headerSection.Blocks.Add(new BlockUIContainer(new TextBlock()));
            doc.Blocks.Add(headerSection);
            doc.Blocks.Add(ceList());
        
            TableUtil.ResetDefaults();
        }
        

        private int _panelNumber;
        private XfpData _data;
        private const int _totalTimerTimesColumns = 9;
        private const int _totalCEColumns = 11;
        private static SolidColorBrush _timerEventTimesUnderlineBrush = Styles.Brush05;
        private static SolidColorBrush _gridDividerBrush = Styles.Brush08;
        private List<string> _actions;
        private List<string> _triggers;
        private List<string> _inputs;
        private List<string> _loop1Devices;
        private List<string> _loop2Devices;
        private List<string> _zones;
        private List<string> _zones2;
        private List<string> _zonesPanels;
        private List<string> _groups;
        private List<string> _sets;
        private List<string> _events;
        private List<string> _events1;
        private List<string> _events2;
        private List<string> _relays;
        private List<string> _setsRelays;
        private List<string> _times;
        private List<string> _trueOrFalse;

        //private double _wNum;
        //private double _wActionName = 0;
        //private double _wActionParam;
        //private double _wTriggerName;
        //private double _wTriggerParam;
        //private double _wZone;
        //private double _wEvent;
        //private double _wZoneOrEventName;
        //private double _wZoneOrEvent;
        //private double _wAnd; 
        //private double _wIs; 
        //private double _wTrueOrFalse; 
        private List<double> _columnWidths = new();
        private double _triggerWidth = 0.0;
        private double _resetWidth   = 0.0;


        private void initLists()
        {
            _actions      = _data.GetCEActionsList();
            _triggers     = _data.GetCETriggersList();
            _groups       = _data.GetGroupsList();
            _inputs       = _data.GetInputsList();
            _loop1Devices = _data.GetLoop1DeviceList(_panelNumber);
            _loop2Devices = _data.GetLoop2DeviceList(_panelNumber);
            _zones        = _data.GetZonesList();
            _zonesPanels  = _data.GetZonePanelsList();
            _sets         = _data.GetSetsList();
            _events       = _data.GetEventsList();
            _relays       = _data.GetRelaysList();
            _setsRelays   = _data.GetSetsRelaysList();
            _times        = _data.GetCETimerTList();
        }

        private BlockUIContainer timerEventTimes()
        {
            var grid = new Grid();

            for (int i = 0; i < 8; i++)
                GridUtil.AddRowToGrid(grid);

            for (int i = 0; i < _totalTimerTimesColumns; i++)
                GridUtil.AddColumnToGrid(grid);

            grid.Children.Add(GridUtil.GridCell(Cultures.Resources.Timer_Event_Times, 0, 0, 1, _totalTimerTimesColumns, true));

            GridUtil.AddBorderToGrid(grid, 1, 0, 1, _totalTimerTimesColumns, _timerEventTimesUnderlineBrush, new Thickness(0, 1, 0, 0));
            grid.Children.Add(GridUtil.SetMargin(GridUtil.GridCell(Cultures.Resources.Timer_Event, 2, 0), new(3, 2, 10, 2)));
            grid.Children.Add(GridUtil.SetMargin(GridUtil.GridCell(Cultures.Resources.Occurs_At,   3, 0), new(3, 2, 10, 2)));
            GridUtil.AddBorderToGrid(grid, 4, 0, 1, _totalTimerTimesColumns, _gridDividerBrush, new Thickness(0, 0.5, 0, 0));
            grid.Children.Add(GridUtil.SetMargin(GridUtil.GridCell(Cultures.Resources.Timer_Event, 5, 0), new(3, 2, 10, 2)));
            grid.Children.Add(GridUtil.SetMargin(GridUtil.GridCell(Cultures.Resources.Occurs_At,   6, 0), new(3, 2, 10, 2)));
            GridUtil.AddBorderToGrid(grid, 7, 0, 1, _totalTimerTimesColumns, _gridDividerBrush, new Thickness(0, 0.5, 0, 0));

            for (int i = 0; i < 16; i++)
            {
                grid.Children.Add(GridUtil.SetMargin(GridUtil.GridCell(string.Format(Cultures.Resources.Time_T_x, i + 1), 3 * (i / 8) + 2, i % 8 + 1, false, HorizontalAlignment.Center), new(10, 2, 10, 2)));
                grid.Children.Add(GridUtil.GridCellTimeSpan(TimerEventTimes[i],                                           3 * (i / 8) + 3, i % 8 + 1, "hm", false, true, HorizontalAlignment.Center));
            }

            return new(grid);
        }


        private class ReportElement
        {
            public ReportElement() { }
            public ReportElement(string text, TextAlignment align = TextAlignment.Left) { Text = text; Alignment = align; }
            public ReportElement(string text, int columnSpan) { Text = text; ColumnSpan = columnSpan; }
            public ReportElement(bool isError) { IsError = isError; Text = PrintUtil.Errorindicator; }

            public string Text { get; private set; } = string.Empty;
            public int    ColumnSpan { get; private set; } = 1;
            public bool   IsError { get; private set; } = false;
            public TextAlignment Alignment { get; private set; }
        }


        private List<List<ReportElement>> _report = new();


        public Table ceList()
        {
            createReportTable();

            int dataRows = 0;
            
            var reportName = Cultures.Resources.Nav_C_And_E_Configuration;

            try
            {
                var table = TableUtil.NewTable(reportName);

                defineColumnHeaders(table, reportName);

                var bodyGroup = new TableRowGroup();

                for (int r = 0; r < _report.Count; r++)
                {
                    dataRows++;

                    //create the new row
                    var newRow = new TableRow() { Background = Int32.IsEvenInteger(dataRows) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground };
                    bodyGroup.Rows.Add(newRow);

                    for (int c = 0; c < _report[r].Count; c++)
                    {
                        var item = _report[r][c];
                        newRow.Cells.Add(TableUtil.NewCell(item.Text, 1, item.ColumnSpan, item.Alignment, item.IsError ? PrintUtil.ErrorBrush : PrintUtil.TextForeground));
                    }
                }

                //foreach (var e in Events)
                //{
                //    dataRows++;

                //    ////create the new row
                //    //var newRow = new TableRow() { Background = Int32.IsEvenInteger(dataRows) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground };
                //    //bodyGroup.Rows.Add(newRow);
                    
                //    newRow.Cells.Add(TableUtil.NewCell((e.Index + 1).ToString(), TextAlignment.Right));                         //C&E number
                   
                //    newRow.Cells.Add(TableUtil.NewCell(Enums.CEActionTypesToString(e.ActionType).ToString()));                  //action type
                //    if (e.ActionType == CEActionTypes.None)
                //        continue;

                //    if (e.HasActionParam())                                                                                     //action parameter, if any
                //    {
                //        if (e.ActionParam >= 0)
                //        {
                //            newRow.Cells.Add(TableUtil.NewCell(getActionParamDesc(e.ActionType, e.ActionParam).ToString()));
                //        }
                //        else
                //        {
                //            newRow.Cells.Add(TableUtil.NewCell(PrintUtil.Errorindicator, Styles.ErrorBrush));
                //            continue;
                //        }
                //    }
                //    else
                //    {
                //        newRow.Cells.Add(TableUtil.NewCell(""));
                //    }

                //    newRow.Cells.Add(TableUtil.NewCell(""));

                //    if (e.TriggerType != CETriggerTypes.None)
                //    {
                //        newRow.Cells.Add(TableUtil.NewCell((Enums.CETriggerTypesToString(e.TriggerType)).ToString()));          //trigger type
                //    }
                //    else
                //    {
                //        newRow.Cells.Add(TableUtil.NewCell(PrintUtil.Errorindicator, Styles.ErrorBrush));
                //        continue;
                //    }

                //    if (e.HasTriggerParam())                                                                                    //trigger parameter(s), if any
                //    {
                //        if (e.TriggerTypeHasParamPair())
                //        {
                //            newRow.Cells.Add(TableUtil.NewCell(e.TriggerType == CETriggerTypes.EventAnd ? Cultures.Resources.Event : Cultures.Resources.Zone));
                //            newRow.Cells.Add(TableUtil.NewCell(getTriggerParamDesc(e.TriggerType, e.TriggerParam2).ToString()));
                //            newRow.Cells.Add(TableUtil.NewCell(Cultures.Resources.Logical_And));

                //            if (e.TriggerParam >= 0)
                //               newRow.Cells.Add(TableUtil.NewCell(getTriggerParamDesc(e.TriggerType, e.TriggerParam).ToString()));
                            
                //            if (e.TriggerParam < 0 || e.TriggerParam2 < 0)
                //                continue;
                //        }
                //        else
                //        {
                //            newRow.Cells.Add(TableUtil.NewCell(getTriggerParamDesc(e.TriggerType, e.TriggerParam).ToString(), 1, 4));
                //        }

                //        if (e.TriggerParam < 0)
                //            continue;
                //    }
                //    else
                //    {
                //        newRow.Cells.Add(TableUtil.NewCell("", 1, 4));
                //    }

                //    newRow.Cells.Add(TableUtil.NewCell(Cultures.Resources.Trigger_Condition_Is));                               
                //    newRow.Cells.Add(TableUtil.NewCell(isTrueOrFalse(e.TriggerCondition)));                                     //trigger condition
                    
                //    /*

                
                //    //col ++;

                //    ////trigger condition
                //    //GridUtil.AddCellToGrid(grid, isTrueOrFalse(Events[i].TriggerCondition), row, col++);


                //    //grid.Children.Add(GridUtil.GridBackground(1, col++, NumEvents, 1, _gridDividerBrush));

                
                //    ////reset trigger
                //    //var noTrigger = Events[i].ResetType == CETriggerTypes.None;
                //    //GridUtil.AddCellToGrid(grid, Enums.CETriggerTypesToString(Events[i].ResetType), row, col++, false, noTrigger);
                //    //if (noTrigger)
                //    //    continue;

                //    ////reset parameters
                //    //if (Events[i].HasResetParam())
                //    //{
                //    //    if (Events[i].ResetTypeHasParamPair())
                //    //    {
                //    //        Grid paramGrid = new();
                //    //        GridUtil.AddRowToGrid(paramGrid);
                //    //        for (int c = 0; c < 4; c++)
                //    //            GridUtil.AddColumnToGrid(paramGrid);
                //    //        GridUtil.AddCellToGrid(paramGrid, paramDesc(Events[i].ResetType), 0, 0);
                //    //        GridUtil.AddCellToGrid(paramGrid, getTriggerParamDesc(Events[i].ResetType, Events[i].ResetParam2), 0, 1, HorizontalAlignment.Left, false, Events[i].ResetParam2 < 0, new(0, 2, 0, 2));
                //    //        GridUtil.AddCellToGrid(paramGrid, Cultures.Resources.Logical_And, 0, 2);
                //    //        GridUtil.AddCellToGrid(paramGrid, getTriggerParamDesc(Events[i].ResetType, Events[i].ResetParam), 0, 3, HorizontalAlignment.Left, false, Events[i].ResetParam < 0, new(0, 2, 0, 2));
                //    //        paramGrid.SetValue(Grid.RowProperty, row);
                //    //        paramGrid.SetValue(Grid.ColumnProperty, col);
                //    //        GridUtil.AddCellToGrid(grid, paramGrid);
                //    //        if (Events[i].ResetParam < 0 || Events[i].ResetParam2 < 0)
                //    //            continue;
                //    //    }
                //    //    else
                //    //    {
                //    //        GridUtil.AddCellToGrid(grid, getTriggerParamDesc(Events[i].ResetType, Events[i].ResetParam), row, col, false, Events[i].ResetParam < 0);
                //    //        if (Events[i].ResetParam < 0)
                //    //            continue;
                //    //    }
                //    //}
                 
                //    //col++;

                //    ////reset condition
                //    //GridUtil.AddCellToGrid(grid, isTrueOrFalse(Events[i].ResetCondition), row, col++);
                //    */
                //}

                //GridUtil.AddRowToGrid(grid, 10);

                table.RowGroups.Add(bodyGroup);
                return table;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                PrintUtil.ResetFont();
            }
        }


        private void createReportTable()
        {
            //create string table of the report values
            foreach (var e in Events)
            {
                var row = new List<ReportElement>();

                //C&E number
                row.Add(new((e.Index + 1).ToString(), TextAlignment.Right));

                //action type
                row.Add(new(Enums.CEActionTypesToString(e.ActionType).ToString()));

                //action parameter, if any
                if (e.HasActionParam())
                {
                    if (e.ActionParam >= 0)
                    {
                        row.Add(new(getActionParamDesc(e.ActionType, e.ActionParam).ToString()));
                    }
                    else
                    {
                        row.Add(new(true));
                        continue;
                    }
                }
                else
                {
                    row.Add(new());
                }

                row.Add(new());

                //trigger type
                if (e.TriggerType != CETriggerTypes.None)
                {
                    row.Add(new((Enums.CETriggerTypesToString(e.TriggerType)).ToString()));
                }
                else
                {
                    row.Add(new(true));
                    continue;
                }

                //trigger parameter(s), if any
                if (e.HasTriggerParam())
                {
                    if (e.TriggerTypeHasParamPair())
                    {
                        row.Add(new(e.TriggerType == CETriggerTypes.EventAnd ? Cultures.Resources.Event : Cultures.Resources.Zone));
                        row.Add(new(getTriggerParamDesc(e.TriggerType, e.TriggerParam2).ToString()));
                        row.Add(new(Cultures.Resources.Logical_And));

                        if (e.TriggerParam >= 0)
                            row.Add(new(getTriggerParamDesc(e.TriggerType, e.TriggerParam).ToString()));

                        if (e.TriggerParam < 0 || e.TriggerParam2 < 0)
                            continue;
                    }
                    else
                    {
                        row.Add(new(getTriggerParamDesc(e.TriggerType, e.TriggerParam).ToString(), 4));
                    }

                    if (e.TriggerParam < 0)
                        continue;
                }
                else
                {
                    row.Add(new("", 4));
                }

                //trigger condition
                row.Add(new(Cultures.Resources.Trigger_Condition_Is));
                row.Add(new(isTrueOrFalse(e.TriggerCondition)));

                _report.Add(row);
            }
        }


        private void defineColumnHeaders(Table table, string reportHeader)
        {     
            setColumnWidths();

            //define table's columns
            table.Columns.Add(new TableColumn() { Width = new GridLength(_columnWidths[0]) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_columnWidths[1]) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_columnWidths[2]) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(5) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_columnWidths[3]) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_columnWidths[4]) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_columnWidths[5]) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_columnWidths[6]) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_columnWidths[7]) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_columnWidths[8]) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_columnWidths[9]) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(5) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_columnWidths[10]) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_columnWidths[11]) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_columnWidths[12]) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_columnWidths[13]) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_columnWidths[14]) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_columnWidths[15]) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_columnWidths[16]) });

            var headerRow = new TableRow();

            headerRow.Background = PrintUtil.GridHeaderBackground;

            headerRow.Cells.Add(TableUtil.NewCell(Cultures.Resources.Number_Symbol, TextAlignment.Right, FontWeights.Bold));
            headerRow.Cells.Add(TableUtil.NewCell(Cultures.Resources.Action, 1, 2, FontWeights.Bold));
            headerRow.Cells.Add(TableUtil.NewCell(""));
            headerRow.Cells.Add(TableUtil.NewCell(Cultures.Resources.Occurs_When, 1, 7, FontWeights.Bold));
            headerRow.Cells.Add(TableUtil.NewCell(""));
            headerRow.Cells.Add(TableUtil.NewCell(Cultures.Resources.Resets_When, 1, 7, FontWeights.Bold));

            //headerRow2.Cells.Add(TableUtil.UnderlineCell(TableUtil.NewCell(Cultures.Resources.Output_Set_Triggered, 1, NumOutputSetTriggers, TextAlignment.Center, FontWeights.Bold), Styles.Brush04));
            //headerRow1.Cells.Add(TableUtil.NewCell("", 1, 17));
            //headerRow1.Cells.Add(TableUtil.UnderlineCell(TableUtil.NewCell(Cultures.Resources.Panel_Relay_Triggered, 2, NumPanelRelayTriggers + 1, TextAlignment.Left, FontWeights.Bold), Styles.Brush04));

            //for (int i = 0; i < NumOutputSetTriggers; i++)
            //    headerRow3.Cells.Add(TableUtil.NewCell((i + 1).ToString(), TextAlignment.Center, FontWeights.Bold));

            //headerRow3.Cells.Add(TableUtil.NewCell(""));

            //for (int i = 0; i < NumPanelRelayTriggers; i++)
            //    headerRow3.Cells.Add(TableUtil.NewCell((i + 1).ToString(), TextAlignment.Center, FontWeights.Bold));

            var headerGroup = new TableRowGroup();
            headerGroup.Rows.Add(headerRow);
            //headerGroup.Rows.Add(headerRow2);
            //headerGroup.Rows.Add(headerRow3);

            table.RowGroups.Add(headerGroup);
        }


        private void setColumnWidths()
        {
            var cellMargins = (int)(PrintUtil.DefaultGridMargin.Left + PrintUtil.DefaultGridMargin.Right) + 1;

            _columnWidths.Add(TableUtil.MeasureText("99").Width + 1);

            //_wAnd = TableUtil.MeasureText(Cultures.Resources.Logical_And).Width + 1;
            //_wIs  = TableUtil.MeasureText(Cultures.Resources.Trigger_Condition_Is).Width + 1;
            
            for (int i = 0; i < 17; i++)
                _columnWidths.Add(5);

            for (int row = 0; row < _report.Count; row++)
            {
                for (int col = 0; col < _report[row].Count; col++)
                {
                    if (_report[row][col].ColumnSpan > 1)
                    {
                        if (col < 8)
                            _triggerWidth = Math.Max(_columnWidths[col], TableUtil.MeasureText(_report[row][col].Text).Width + 1);
                        else
                            _resetWidth = Math.Max(_columnWidths[col], TableUtil.MeasureText(_report[row][col].Text).Width + 1);
                    }
                    else
                    {
                        _columnWidths[col] = Math.Max(_columnWidths[col], TableUtil.MeasureText(_report[row][col].Text).Width + 1);
                    }
                }
            }
        }


        //public double GetMaxActionNameLength()
        //{
        //    var result = 0.0;

        //    foreach (var a in _actions)
        //    {
        //        var nameWidth = TableUtil.MeasureText(a).Width + 1;
        //        if (nameWidth > result)
        //            result = nameWidth;
        //    }

        //    return result;
        //}


        //public double GetMaxActionParamLength()
        //{
        //    var result = 0.0;
        //    foreach (var d in _loop1Devices.Concat(_loop2Devices).ToList())
        //        result = Math.Max(result, TableUtil.MeasureText(d).Width + 1);
        //    foreach (var r in _relays)
        //        result = Math.Max(result, TableUtil.MeasureText(r).Width + 1);
        //    foreach (var g in _groups)
        //        result = Math.Max(result, TableUtil.MeasureText(g).Width + 1);
        //    foreach (var z in _zones)
        //        result = Math.Max(result, TableUtil.MeasureText(z).Width + 1);
        //    foreach (var s in _sets)
        //        result = Math.Max(result, TableUtil.MeasureText(s).Width + 1);
        //    foreach (var r in _setsRelays)
        //        result = Math.Max(result, TableUtil.MeasureText(r).Width + 1);
        //    foreach (var e in _events)
        //        result = Math.Max(result, TableUtil.MeasureText(e).Width + 1);
        //    return result;
        //}


        //public double GetMaxTriggerNameLength()
        //{
        //    var result = 0.0;
        //    foreach (var t in _data.GetCETriggersList())
        //        result = Math.Max(result, TableUtil.MeasureText(t).Width + 1);
        //    return result;
        //}


        //public double GetMaxTriggerParamLength()
        //{
        //    var result = 0.0;
        //    foreach (var a in _loop1Devices.Concat(_loop2Devices).ToList())
        //        result = Math.Max(result, TableUtil.MeasureText(a).Width + 1);
        //    foreach (var r in _relays)
        //        result = Math.Max(result, TableUtil.MeasureText(r).Width + 1);
        //    foreach (var z in _zones)
        //        result = Math.Max(result, TableUtil.MeasureText(z).Width + 1);
        //    foreach (var p in _zonesPanels)
        //        result = Math.Max(result, TableUtil.MeasureText(p).Width + 1);
        //    foreach (var i in _groups)
        //        result = Math.Max(result, TableUtil.MeasureText(i).Width + 1);
        //    foreach (var t in _times)
        //        result = Math.Max(result, TableUtil.MeasureText(t).Width + 1);
        //    foreach (var e in _events)
        //        result = Math.Max(result, TableUtil.MeasureText(e).Width + 1);

        //    result = Math.Max(result, _wZoneOrEventName + _wZoneOrEvent * 2 + _wAnd);
        //    return result;
        //}


        //public double GetMaxEventLength()
        //{
        //    var result = 0.0;
        //    foreach (var e in _events)
        //        result = Math.Max(result, TableUtil.MeasureText(e).Width + 1);
        //    return result;
        //}


        //public double GetMaxZoneLength()
        //{
        //    var result = 0.0;
        //    foreach (var z in _zones)
        //        result = Math.Max(result, TableUtil.MeasureText(z).Width + 1);
        //    foreach (var p in _zonesPanels)
        //        result = Math.Max(result, TableUtil.MeasureText(p).Width + 1);
        //    return result;
        //}


        //private Grid columnHeaders()
        //{
        //    Grid grid = new Grid();

        //    GridUtil.AddRowToGrid(grid);

        //    for (int i = 0; i < CEConfigData.NumEvents; i++)
        //        GridUtil.AddRowToGrid(grid);

        //    for (int i = 0; i < _totalCEColumns; i++)
        //    {
        //        if (i == 3  || i == 7)
        //            GridUtil.AddColumnToGrid(grid, 2);
        //        else
        //            GridUtil.AddColumnToGrid(grid);
        //    }

        //    //header background
        //    grid.Children.Add(GridUtil.GridBackground(0, 0, 1, _totalCEColumns, PrintUtil.GridHeaderBackground));

        //    //header text
        //    grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Number_Symbol, 0, 0, HorizontalAlignment.Right));
        //    grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Action,        0, 1, 1, 2));
        //    grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Occurs_When,   0, 3, 1, 3));
        //    grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Resets_When,   0, 8, 1, 3));

        //    return grid;
        //}


        private string paramDesc(CETriggerTypes triggerType) => triggerType == CETriggerTypes.EventAnd ? Cultures.Resources.Event : Cultures.Resources.Zone;
        private string isTrueOrFalse(bool condition) => condition ? Cultures.Resources.True : Cultures.Resources.False;


        private string getActionParamDesc(CEActionTypes action, int param)
        {
            return action switch
            {
                CEActionTypes.TriggerLoop1Device or
                CEActionTypes.Loop1DeviceDisable => getListString(_loop1Devices, param),

                CEActionTypes.TriggerLoop2Device or
                CEActionTypes.Loop2DeviceDisable => getListString(_loop2Devices, param),

                CEActionTypes.PanelRelay => getListString(_relays, param),

                CEActionTypes.SounderAlert or
                CEActionTypes.SounderEvac or
                CEActionTypes.GroupDisable or
                CEActionTypes.TriggerBeacons => getListString(_groups, param),

                CEActionTypes.ZoneDisable or
                CEActionTypes.PutZoneIntoAlarm => getListString(_zones, param),

                CEActionTypes.TriggerOutputSet => getListString(_sets, param),

                CEActionTypes.OutputDisable => getListString(_setsRelays, param),

                CEActionTypes.TriggerNetworkEvent => getListString(_events, param),

                _ => "",
            };
        }


        private string getTriggerParamDesc(CETriggerTypes triggerType, int param)
        {
            return triggerType switch
            {
                CETriggerTypes.EventAnd or
                CETriggerTypes.ZoneAnd => param >= 0 ? (param + 1).ToString() : "",

                CETriggerTypes.Loop1DeviceTriggered or
                CETriggerTypes.Loop1DevicePrealarm => getListString(_loop1Devices, param),

                CETriggerTypes.Loop2DeviceTriggered or
                CETriggerTypes.Loop2DevicePrealarm => getListString(_loop2Devices, param),

                CETriggerTypes.PanelInput => getListString(_inputs, param),

                CETriggerTypes.OtherEventTriggered => getListString(_events, param),

                CETriggerTypes.TimerEventTn => getListString(_times, param),

                CETriggerTypes.ZoneHasDeviceInAlarm or
                CETriggerTypes.ZoneOrPanelInFire => getListString(_zonesPanels, param),

                _ => "",
            };
        }


        private string getListString(List<string> list, int? index)
        {
            if (index is not null && index >= 0 && index < list.Count)
                return list[(int)index];
            return "";
        }
    }
}
