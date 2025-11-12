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
            TableUtil.SetPadding(PrintUtil.DefaultTableMargin);
            TableUtil.SetPadding(new(2,2,4,2));

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
        private List<string> _zonesPanels;
        private List<string> _groups;
        private List<string> _sets;
        private List<string> _events;
        private List<string> _relays;
        private List<string> _setsRelays;
        private List<string> _times;

        private List<List<ReportElement>> _report;
        private List<double> _columnWidths = new();
        private const double _spacerColumnWidth = 5;

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


        public Table ceList()
        {
            var reportName = Cultures.Resources.Nav_C_And_E_Configuration;

            try
            {
                var table = TableUtil.NewTable(reportName);

                createReportTable();
                setColumnWidths();
                defineColumnHeaders(table, reportName);

                int dataRows = 0;            

                var bodyGroup = new TableRowGroup();

                for (int r = 0; r < _report.Count; r++)
                {
                    dataRows++;

                    //create the new row
                    var newRow = new TableRow() { Background = Int32.IsEvenInteger(dataRows) ? PrintUtil.TableAlternatingRowBackground : PrintUtil.NoBackground };
                    bodyGroup.Rows.Add(newRow);

                    for (int c = 0; c < _report[r].Count; c++)
                    {
                        var item = _report[r][c];
                        //newRow.Cells.Add(TableUtil.NewCell(item.Text, 1, item.ColumnSpan, item.Alignment, item.IsError ? PrintUtil.ErrorBrush : PrintUtil.TextForeground));
                        var cell = TableUtil.NewCell(item.Text, 1, item.ColumnSpan, item.Alignment, item.IsError ? PrintUtil.ErrorBrush : PrintUtil.TextForeground);
                        cell.Background = (c % 4) switch { 0 => Styles.Brush03, 1 => Styles.ErrorBrush, 2 => Styles.WarnBrush, _ => Styles.OkBrush };
                        newRow.Cells.Add(cell);
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
        private void createReportTable()
        {
            //create string table of the report values
            _report = new();

            foreach (var e in Events)
            {
                var row = new List<ReportElement>();

                //C&E number
                row.Add(new((e.Index + 1).ToString(), TextAlignment.Right));

                //action type
                row.Add(new(Enums.CEActionTypesToString(e.ActionType).ToString()));

                if (e.ActionType == CEActionTypes.None)
                {
                    _report.Add(row);
                    continue;
                }

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
                        _report.Add(row);
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
                    _report.Add(row);
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
                        {
                            _report.Add(row);
                            continue;
                        }
                    }
                    else
                    {
                        row.Add(new(getTriggerParamDesc(e.TriggerType, e.TriggerParam).ToString(), 4));
                    }

                    if (e.TriggerParam < 0)
                    {
                        _report.Add(row);
                        continue;
                    }
                }
                else
                {
                    row.Add(new("", 4));
                }

                //trigger condition
                row.Add(new(Cultures.Resources.Trigger_Condition_Is));
                row.Add(new(getTrueOrFalse(e.TriggerCondition)));

                row.Add(new());

                //reset type
                if (e.ResetType != CETriggerTypes.None)
                {
                    row.Add(new((Enums.CETriggerTypesToString(e.ResetType)).ToString()));
                }
                else
                {
                    row.Add(new(true));
                    _report.Add(row);
                    continue;
                }

                //reset parameter(s), if any
                if (e.HasResetParam())
                {
                    if (e.ResetTypeHasParamPair())
                    {
                        row.Add(new(e.ResetType == CETriggerTypes.EventAnd ? Cultures.Resources.Event : Cultures.Resources.Zone));
                        row.Add(new(getTriggerParamDesc(e.ResetType, e.ResetParam2).ToString()));
                        row.Add(new(Cultures.Resources.Logical_And));

                        if (e.ResetParam >= 0)
                            row.Add(new(getTriggerParamDesc(e.ResetType, e.ResetParam).ToString()));

                        if (e.ResetParam < 0 || e.ResetParam2 < 0)
                        {
                            _report.Add(row);
                            continue;
                        }
                    }
                    else
                    {
                        row.Add(new(getTriggerParamDesc(e.ResetType, e.ResetParam).ToString(), 4));
                    }

                    if (e.ResetParam < 0)
                    {
                        _report.Add(row);
                        continue;
                    }
                }
                else
                {
                    row.Add(new("", 4));
                }

                //reset condition
                row.Add(new(Cultures.Resources.Trigger_Condition_Is));
                row.Add(new(getTrueOrFalse(e.ResetCondition)));
                _report.Add(row);
            }
        }

        private void setColumnWidths()
        {
            var cellLeftRightPadding = TableUtil.Padding.Left + TableUtil.Padding.Right;

            _columnWidths = new();

            for (int row = 0; row < _report.Count; row++)
            {
                int offset = 0;

                for (int col = 0; col < _report[row].Count; col++)
                {
                    if (_report[row][col].ColumnSpan > 1)
                        offset += _report[row][col].ColumnSpan - 1;

                    while (_columnWidths.Count <= col + offset)
                        _columnWidths.Add(_spacerColumnWidth);

                    if (_report[row][col].ColumnSpan == 1)
                        _columnWidths[col + offset] = Math.Max(_columnWidths[col + offset], TableUtil.MeasureText(_report[row][col].Text).Width + cellLeftRightPadding);
                }
            }

            if (_columnWidths.Count < 5)
            {
                _columnWidths.Add(TableUtil.MeasureText(Cultures.Resources.Occurs_When).Width + cellLeftRightPadding);
                for (int i = _columnWidths.Count; i < 13; i++)
                    _columnWidths.Add(_spacerColumnWidth);
            }

            if (_columnWidths.Count < 13)
            {
                _columnWidths.Add(TableUtil.MeasureText(Cultures.Resources.Occurs_When).Width + cellLeftRightPadding);
                for (int i = _columnWidths.Count; i < 17; i++)
                    _columnWidths.Add(_spacerColumnWidth);
            }
        }


        private void defineColumnHeaders(Table table, string reportHeader)
        {     
            foreach (var w in _columnWidths)
                table.Columns.Add(new TableColumn() { Width = new GridLength(w) });

            var headerRow = new TableRow();

            headerRow.Background = PrintUtil.TableHeaderBackground;

            headerRow.Cells.Add(TableUtil.NewCell(Cultures.Resources.Number_Symbol, TextAlignment.Right, FontWeights.Bold));
            headerRow.Cells.Add(TableUtil.NewCell(Cultures.Resources.Action, 1, 3, FontWeights.Bold));
            headerRow.Cells.Add(TableUtil.NewCell(Cultures.Resources.Occurs_When, 1, 8, FontWeights.Bold));
            headerRow.Cells.Add(TableUtil.NewCell(Cultures.Resources.Resets_When, 1, 7, FontWeights.Bold));

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

        private string getTrueOrFalse(bool condition) => condition ? Cultures.Resources.True : Cultures.Resources.False;


        private string getListString(List<string> list, int? index)
        {
            if (index is not null && index >= 0 && index < list.Count)
                return list[(int)index];
            return "";
        }
    }
}
