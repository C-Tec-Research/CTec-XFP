using CTecControls.UI;
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
            //if (pageNumber++ > 1)
            //    PrintUtil.InsertPageBreak(doc);

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
            TableUtil.SetPadding(new(2,2,4,2));

            PrintUtil.PageHeader(doc, string.Format(Cultures.Resources.Panel_x, panelNumber) + " - " + Cultures.Resources.Nav_C_And_E_Configuration);

            doc.Blocks.Add(timerEventTimes());
            doc.Blocks.Add(ceList());
        
            TableUtil.ResetDefaults();
        }
        

        private int _panelNumber;
        private XfpData _data;
        private const int _totalTimerTimesColumns = 9;
        private const int _totalCEColumns = 11;
        private static SolidColorBrush _timerEventTimesUnderlineBrush = CTecControls.UI.Styles.Brush05;
        private static SolidColorBrush _gridDividerBrush = CTecControls.UI.Styles.Brush08;
        private List<string> _actions;
        private List<string> _triggers;
        private List<string> _inputs;
        private List<string> _loop1Devices;
        private List<string> _loop2Devices;
        private List<string> _zones;
        private List<string> _zonesPanels;
        private List<string> _groups;
        private List<string> _sets;
        private List<string> _events;
        private List<string> _relays;
        private List<string> _setsRelays;
        private List<string> _times;

        private List<List<ReportTextElement>> _reportStrings;
        private List<double> _columnWidths = new();
        private const double _spacerColumnWidth = 2;


        private class ReportTextElement
        {
            public ReportTextElement() { }
            public ReportTextElement(string text, int? dataValue, TextAlignment align = TextAlignment.Left) { Text = text; DataValue = dataValue; Alignment = align; }
            public ReportTextElement(string text, int? dataValue) { Text = text; DataValue = dataValue; }
            public ReportTextElement(bool isError) { IsError = isError; Text = PrintUtil.ErrorNotSet; }

            public string Text { get; private set; } = string.Empty;
            public int?   DataValue { get; private set; }
            public bool   IsError { get; private set; } = false;
            public TextAlignment Alignment { get; private set; }
        }


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


        private Section timerEventTimes()
        {
            var headerSection = new Section();
            
            var grid = new Grid();

            GridUtil.SetFontSize(PrintUtil.PrintSmallerFontSize);

            for (int i = 0; i < 8; i++)
                GridUtil.AddRowToGrid(grid);

            for (int i = 0; i < _totalTimerTimesColumns; i++)
                GridUtil.AddColumnToGrid(grid);

            grid.Children.Add(GridUtil.GridCell(Cultures.Resources.Timer_Event_Times, 0, 0, 1, _totalTimerTimesColumns, true));

            GridUtil.AddBorderToGrid(grid, 1, 0, 1, _totalTimerTimesColumns, _timerEventTimesUnderlineBrush, new Thickness(0, 1, 0, 0));
            grid.Children.Add(GridUtil.SetMargin(GridUtil.GridCell(Cultures.Resources.Timer_Event, 2, 0), new(3, 2, 10, 0)));
            grid.Children.Add(GridUtil.SetMargin(GridUtil.GridCell(Cultures.Resources.Occurs_At,   3, 0), new(3, 0, 10, 2)));
            GridUtil.AddBorderToGrid(grid, 4, 0, 1, _totalTimerTimesColumns, _gridDividerBrush, new Thickness(0, 0.5, 0, 0));
            grid.Children.Add(GridUtil.SetMargin(GridUtil.GridCell(Cultures.Resources.Timer_Event, 5, 0), new(3, 2, 10, 2)));
            grid.Children.Add(GridUtil.SetMargin(GridUtil.GridCell(Cultures.Resources.Occurs_At,   6, 0), new(3, 2, 10, 2)));
            GridUtil.AddBorderToGrid(grid, 7, 0, 1, _totalTimerTimesColumns, _gridDividerBrush, new Thickness(0, 0.5, 0, 0));

            for (int i = 0; i < 16; i++)
            {
                grid.Children.Add(GridUtil.SetMargin(GridUtil.GridCell(string.Format(Cultures.Resources.Time_T_x, i + 1), 3 * (i / 8) + 2, i % 8 + 1, false, HorizontalAlignment.Center), new(10, 0, 10, 0)));
                grid.Children.Add(GridUtil.SetMargin(GridUtil.GridCellTimeSpan(TimerEventTimes[i],                        3 * (i / 8) + 3, i % 8 + 1, "hm", false, true, HorizontalAlignment.Center), new(10, 0, 10, 0)));
            } 

            headerSection.Blocks.Add(new BlockUIContainer(grid));
            return headerSection;
        }


        public Table ceList()
        {
            var reportName = Cultures.Resources.Nav_C_And_E_Configuration;

            try
            {
                var table = TableUtil.NewTable(reportName);

                createReportStringTable();
                setColumnWidths();
                defineColumnHeaders(table, reportName);

                int dataRows = 0;            

                var bodyGroup = new TableRowGroup();

                for (int r = 0; r < _reportStrings.Count; r++)
                {
                    dataRows++;

                    //create the new row
                    var newRow = new TableRow() { Background = Int32.IsEvenInteger(dataRows) ? PrintUtil.TableAlternatingRowBackground : PrintUtil.NoBackground };
                    bodyGroup.Rows.Add(newRow);

                    for (int c = 0; c < _reportStrings[r].Count; c++)
                    {
                        var item = _reportStrings[r][c];
                        var newCell = TableUtil.NewCell(item.Text, 1, 1, item.Alignment);
                        //newCell.Background = (c % 4) switch { 1 => Styles.Brush05, 2 => Styles.ErrorBrush, 3 => Styles.OkBrush, _ => Styles.WarnBrush, };
                        if (item.IsError)
                        {
                            newCell.Foreground = PrintUtil.ErrorBrush;
                            newCell.FontStyle = FontStyles.Italic;
                        }
                        newRow.Cells.Add(newCell);
                    }
                }

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


        /// <summary>
        /// Build a string table of all the report values 
        /// so that the required column widths can be calculated.
        /// </summary>
        private void createReportStringTable()
        {
            //create string table of the report values
            _reportStrings = new();

            foreach (var e in Events)
            {
                var row = new List<ReportTextElement>();

                //C&E number
                row.Add(new((e.Index + 1).ToString(), TextAlignment.Right));

                //action type
                row.Add(new(Enums.CEActionTypesToString(e.ActionType).ToString()));

                if (e.ActionType == CEActionTypes.None)
                {
                    //none on this row, skip to next
                    _reportStrings.Add(row);
                    continue;
                }

                //action parameter, if any
                if (e.HasActionParam())
                {
                    if (IsValidActionParam(e.ActionParam, e.ActionType))
                    {
                        row.Add(new(getActionParamDesc(e.ActionType, e.ActionParam).ToString()));
                    }
                    else
                    {
                        //no more on this row, skip to next
                        row.Add(new(true));
                        _reportStrings.Add(row);
                        continue;
                    }
                }
                else
                {
                    row.Add(new());
                }

                //trigger type
                if (e.TriggerType != CETriggerTypes.None && e.TriggerType >= 0)
                {
                    row.Add(new(Enums.CETriggerTypesToString(e.TriggerType)));
                }
                else
                {
                    //no more on this row, skip to next
                    row.Add(new(true));
                    _reportStrings.Add(row);
                    continue;
                }

                //trigger parameter(s), if any
                if (e.HasTriggerParam())
                {
                    if (e.TriggerTypeHasParamPair())
                    {
                        var error1 = !IsValidTriggerParam(e.TriggerParam, e.TriggerType);
                        var error2 = !IsValidTriggerParam(e.TriggerParam2, e.TriggerType);

                        var text  = string.Format(e.TriggerType == CETriggerTypes.EventAnd ? Cultures.Resources.Event_x_And_y : Cultures.Resources.Zone_x_And_y, 
                                                  error2 ? PrintUtil.ErrorNotSet : e.TriggerParam2, 
                                                  error1 ? PrintUtil.ErrorNotSet : e.TriggerParam);

                        row.Add(error1 || error2 ? new(true) : new(text));

                        if (e.TriggerParam < 0 || e.TriggerParam2 < 0)
                        {
                            _reportStrings.Add(row);
                            continue;
                        }
                    }
                    else
                    {
                        if (IsValidTriggerParam(e.TriggerParam, e.TriggerType))
                            row.Add(new(getTriggerParamDesc(e.TriggerType, e.TriggerParam).ToString()));
                        else
                            row.Add(new(true));
                    }

                    if (e.TriggerParam < 0)
                    {
                        //no more on this row, skip to next
                        _reportStrings.Add(row);
                        continue;
                    }
                }
                else
                {
                    row.Add(new(""));
                }

                //trigger condition
                row.Add(new(getTrueOrFalse(e.TriggerCondition)));

                //reset type
                if (e.ResetType != CETriggerTypes.None && e.ResetType >= 0)
                {
                    row.Add(new(Enums.CETriggerTypesToString(e.ResetType)));
                }
                else
                {
                    //no more on this row, skip to next
                    row.Add(new(true));
                    _reportStrings.Add(row);
                    continue;
                }

                //reset parameter(s), if any
                if (e.HasResetParam())
                {
                    if (e.ResetTypeHasParamPair())
                    {
                        var error1 = !IsValidTriggerParam(e.ResetParam, e.ResetType);
                        var error2 = !IsValidTriggerParam(e.ResetParam2, e.ResetType);

                        var text  = string.Format(e.ResetType == CETriggerTypes.EventAnd ? Cultures.Resources.Event_x_And_y : Cultures.Resources.Zone_x_And_y, 
                                                  error2 ? PrintUtil.ErrorNotSet : e.ResetParam2, 
                                                  error1 ? PrintUtil.ErrorNotSet : e.ResetParam);

                        row.Add(error1 || error2 ? new(true) : new(text));

                        if (e.ResetParam < 0 || e.ResetParam2 < 0)
                        {
                            _reportStrings.Add(row);
                            continue;
                        }
                    }
                    else
                    {
                        if (IsValidTriggerParam(e.ResetParam, e.ResetType))
                            row.Add(new(getTriggerParamDesc(e.ResetType, e.ResetParam).ToString()));
                        else
                            row.Add(new(true));
                    }

                    if (e.ResetParam < 0)
                    {
                        //no more on this row, skip to next
                        _reportStrings.Add(row);
                        continue;
                    }
                }
                else
                {
                    row.Add(new(""));
                }

                //reset condition
                row.Add(new(getTrueOrFalse(e.ResetCondition)));
                _reportStrings.Add(row);
            }
        }

        private void setColumnWidths()
        {
            TableUtil.SetPadding(new(1, 2, 2, 0));
            var cellLeftRightPadding = TableUtil.Padding.Left + TableUtil.Padding.Right;

            _columnWidths = new();

            //measure the text in each cell to get the max widths per column
            for (int row = 0; row < _reportStrings.Count; row++)
            {
                try
                {
                    for (int col = 0; col < _reportStrings[row].Count; col++)
                    {
                        while (_columnWidths.Count < col + 1)
                            _columnWidths.Add(_spacerColumnWidth);

                        _columnWidths[col] = Math.Max(_columnWidths[col], TableUtil.MeasureText(_reportStrings[row][col].Text).Width + cellLeftRightPadding);
                    }
                }
                catch (Exception ex) { }
            }

            //ensure full complement of columns
            if (_columnWidths.Count < 14)
            {
                for (int i = _columnWidths.Count; i < 14; i++)
                {
                    if (i == 3)
                        _columnWidths.Add(TableUtil.MeasureText(Cultures.Resources.Occurs_When).Width + cellLeftRightPadding);
                    else if (i == 9)
                        _columnWidths.Add(TableUtil.MeasureText(Cultures.Resources.Resets_When).Width + cellLeftRightPadding);
                    else
                        _columnWidths.Add(_spacerColumnWidth);
                }
            }
        }


        private void defineColumnHeaders(Table table, string reportHeader)
        {     
            foreach (var w in _columnWidths)
                table.Columns.Add(new TableColumn() { Width = new GridLength(w) });

            var headerRow = new TableRow();

            headerRow.Background = PrintUtil.TableHeaderBackground;

            headerRow.Cells.Add(TableUtil.NewCell(Cultures.Resources.Number_Symbol, TextAlignment.Right, FontWeights.Bold));
            headerRow.Cells.Add(TableUtil.NewCell(Cultures.Resources.Action, 1, 2, FontWeights.Bold));
            headerRow.Cells.Add(TableUtil.NewCell(Cultures.Resources.Occurs_When, 1, 3, FontWeights.Bold));
            headerRow.Cells.Add(TableUtil.NewCell(Cultures.Resources.Resets_When, 1, 3, FontWeights.Bold));

            var headerGroup = new TableRowGroup();
            headerGroup.Rows.Add(headerRow);

            table.RowGroups.Add(headerGroup);
        }


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

        private string getTrueOrFalse(bool condition) => string.Format(Cultures.Resources.Condition_Is_x, condition ? Cultures.Resources.True : Cultures.Resources.False);


        private string getListString(List<string> list, int? index)
        {
            if (index is not null && index >= 0 && index < list.Count)
                return list[(int)index];
            return "";
        }
    }
}
