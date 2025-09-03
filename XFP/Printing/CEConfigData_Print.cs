using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using CTecUtil.Printing;
using CTecUtil.UI;
using CTecUtil.Utils;
using Xfp.UI.ViewHelpers;

namespace Xfp.DataTypes.PanelData
{
    public partial class CEConfigData
    {
        public void Print(FlowDocument doc, int panelNumber, XfpData data, ref int pageNumber)
        {
            if (pageNumber++ > 1)
                PrintUtil.InsertPageBreak(doc);

            _panelNumber = panelNumber;
            _data = data;

            var svgPathConverter = new SetTriggerTypeToSvgPathConverter();

            initLists();

            var cePage = new Section();
            cePage.Blocks.Add(PrintUtil.PageHeader(string.Format(Cultures.Resources.Panel_x, panelNumber) + " - " + Cultures.Resources.Nav_C_And_E_Configuration));

            cePage.Blocks.Add(timerEventTimes());
            cePage.Blocks.Add(new BlockUIContainer(new TextBlock()));
            cePage.Blocks.Add(ceList());

            doc.Blocks.Add(cePage);
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


        private void initLists()
        {
            _actions = _data.GetCEActionsList();
            _triggers = _data.GetCETriggersList();
            _groups = _data.GetGroupsList();
            _inputs = _data.GetInputsList();
            _loop1Devices = _data.GetLoop1DeviceList(_data.CurrentPanel.PanelNumber);
            _loop2Devices = _data.GetLoop2DeviceList(_data.CurrentPanel.PanelNumber);
            _zones = _data.GetZonesList();
            _zonesPanels = _data.GetZonePanelsList();
            _sets = _data.GetSetsList();
            _events = _data.GetEventsList();
            _relays = _data.GetRelaysList();
            _setsRelays = _data.GetSetsRelaysList();
            _times = _data.GetCETimerTList();
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


        public BlockUIContainer ceList()
        {
            //PrintUtil.SetFontSmallerSize();
            PrintUtil.SetFontNarrowWidth();

            var grid = columnHeaders();

            try
            {
                //add the rows for the data
                for (int i = 0; i < NumEvents; i++)
                {
                    GridUtil.AddRowToGrid(grid);

                    int row = i + 1;
                    int col = 0;

                    grid.Children.Add(GridUtil.GridBackground(row, 0, 1, 15, Int32.IsEvenInteger(row) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));

                    //line number
                    GridUtil.AddCellToGrid(grid, (i + 1).ToString(), row, col++, HorizontalAlignment.Right, false);

                    //action type
                    GridUtil.AddCellToGrid(grid, Enums.CEActionTypesToString(Events[i].ActionType), row, col++, false, Events[i].ActionType < 0);
                    if (Events[i].ActionType == CEActionTypes.None)
                        continue;

                    //action parameter, if any
                    if (!Events[i].HasActionParam())
                    {
                        GridUtil.AddCellToGrid(grid, getActionParamDesc(Events[i].ActionType, Events[i].ActionParam), row, col++, false, Events[i].ActionParam < 0);
                        if (Events[i].ActionParam < 0)
                            continue;
                    }
                    else
                    {
                        col++;
                    }
                

                    grid.Children.Add(GridUtil.GridBackground(1, col++, NumEvents, 1, _gridDividerBrush));


                    //action trigger
                    GridUtil.AddCellToGrid(grid, Enums.CETriggerTypesToString(Events[i].TriggerType), row, col++, false, Events[i].TriggerType < 0 || Events[i].TriggerType == CETriggerTypes.None);
                    //if (Events[i].TriggerType == CETriggerTypes.None)
                    if (Events[i].HasActionParam())
                        continue;

                    //trigger parameters
                    //if (triggerHasParam(Events[i].TriggerType))
                    if (Events[i].HasTriggerParam())
                    {
                        if (Events[i].TriggerTypeHasParamPair())
                        {
                            Grid paramGrid = new();
                            GridUtil.AddRowToGrid(paramGrid);
                            for (int c = 0; c < 4; c++)
                                GridUtil.AddColumnToGrid(paramGrid);

                            GridUtil.AddCellToGrid(paramGrid, paramDesc(Events[i].TriggerType), 0, 0);
                            GridUtil.AddCellToGrid(paramGrid, getTriggerParamDesc(Events[i].TriggerType, Events[i].TriggerParam2), 0, 1, HorizontalAlignment.Left, false, Events[i].TriggerParam2 < 0, new(0, 2, 0, 2));
                            GridUtil.AddCellToGrid(paramGrid, Cultures.Resources.Logical_And, 0, 2);
                            GridUtil.AddCellToGrid(paramGrid, getTriggerParamDesc(Events[i].TriggerType, Events[i].TriggerParam), 0, 3, HorizontalAlignment.Left, false, Events[i].TriggerParam < 0, new(0, 2, 0, 2));
                            paramGrid.SetValue(Grid.RowProperty, row);
                            paramGrid.SetValue(Grid.ColumnProperty, col);
                            GridUtil.AddCellToGrid(grid, paramGrid);

                            if (Events[i].TriggerParam < 0 || Events[i].TriggerParam2 < 0)
                                continue;
                        }
                        else
                        {
                            GridUtil.AddCellToGrid(grid, getTriggerParamDesc(Events[i].TriggerType, Events[i].TriggerParam), row, col, 1, HorizontalAlignment.Left, false, Events[i].TriggerParam < 0);
                            if (Events[i].TriggerParam < 0)
                                continue;
                        }
                    }
                
                    col ++;

                    //trigger condition
                    GridUtil.AddCellToGrid(grid, isTrueOrFalse(Events[i].TriggerCondition), row, col++);


                    grid.Children.Add(GridUtil.GridBackground(1, col++, NumEvents, 1, _gridDividerBrush));

                
                    //reset trigger
                    var noTrigger = Events[i].ResetType == CETriggerTypes.None;
                    GridUtil.AddCellToGrid(grid, Enums.CETriggerTypesToString(Events[i].ResetType), row, col++, false, noTrigger);
                    if (noTrigger)
                        continue;

                    //reset parameters
                    if (Events[i].HasResetParam())
                    {
                        if (Events[i].ResetTypeHasParamPair())
                        {
                            Grid paramGrid = new();
                            GridUtil.AddRowToGrid(paramGrid);
                            for (int c = 0; c < 4; c++)
                                GridUtil.AddColumnToGrid(paramGrid);
                            GridUtil.AddCellToGrid(paramGrid, paramDesc(Events[i].ResetType), 0, 0);
                            GridUtil.AddCellToGrid(paramGrid, getTriggerParamDesc(Events[i].ResetType, Events[i].ResetParam2), 0, 1, HorizontalAlignment.Left, false, Events[i].ResetParam2 < 0, new(0, 2, 0, 2));
                            GridUtil.AddCellToGrid(paramGrid, Cultures.Resources.Logical_And, 0, 2);
                            GridUtil.AddCellToGrid(paramGrid, getTriggerParamDesc(Events[i].ResetType, Events[i].ResetParam), 0, 3, HorizontalAlignment.Left, false, Events[i].ResetParam < 0, new(0, 2, 0, 2));
                            paramGrid.SetValue(Grid.RowProperty, row);
                            paramGrid.SetValue(Grid.ColumnProperty, col);
                            GridUtil.AddCellToGrid(grid, paramGrid);
                            if (Events[i].ResetParam < 0 || Events[i].ResetParam2 < 0)
                                continue;
                        }
                        else
                        {
                            GridUtil.AddCellToGrid(grid, getTriggerParamDesc(Events[i].ResetType, Events[i].ResetParam), row, col, false, Events[i].ResetParam < 0);
                            if (Events[i].ResetParam < 0)
                                continue;
                        }
                    }
                 
                    col++;

                    //reset condition
                    GridUtil.AddCellToGrid(grid, isTrueOrFalse(Events[i].ResetCondition), row, col++);
                }

                GridUtil.AddRowToGrid(grid, 10);
            }
            catch (Exception ex) { }
            finally
            {
                PrintUtil.ResetFont();
            }

            return new(grid);
        }


        private Grid columnHeaders()
        {
            Grid grid = new Grid();

            GridUtil.AddRowToGrid(grid);

            for (int i = 0; i < CEConfigData.NumEvents; i++)
                GridUtil.AddRowToGrid(grid);

            for (int i = 0; i < _totalCEColumns; i++)
            {
                if (i == 3  || i == 7)
                    GridUtil.AddColumnToGrid(grid, 2);
                else
                    GridUtil.AddColumnToGrid(grid);
            }

            //header background
            grid.Children.Add(GridUtil.GridBackground(0, 0, 1, _totalCEColumns, PrintUtil.GridHeaderBackground));

            //header text
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Number_Symbol, 0, 0, HorizontalAlignment.Right));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Action,        0, 1, 1, 2));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Occurs_When,   0, 3, 1, 3));
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Resets_When,   0, 8, 1, 3));

            return grid;
        }


        private string paramDesc(CETriggerTypes triggerType) => triggerType == CETriggerTypes.EventAnd ? Cultures.Resources.Event : Cultures.Resources.Zone;
        private string isTrueOrFalse(bool condition) => string.Format("{0} {1}", Cultures.Resources.Trigger_Condition_Is, condition ? Cultures.Resources.True : Cultures.Resources.False);


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
