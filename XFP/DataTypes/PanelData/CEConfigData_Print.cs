using System;
using System.Windows.Documents;
using CTecDevices.Protocol;
using System.Windows.Media;
using CTecUtil.Printing;
using static Xfp.DataTypes.PanelData.GroupConfigData;
using Xfp.UI.ViewHelpers;
using System.Windows;
using Xfp.UI.Views.PanelTools;
using CTecUtil;
using System.Windows.Controls;
using System.Security.Policy;
using Newtonsoft.Json.Linq;
using System.Windows.Input;
using Windows.UI.Text;
using System.Windows.Media.Media3D;
using System.Globalization;
using System.Collections.Generic;
using System.Data.Common;

namespace Xfp.DataTypes.PanelData
{
    public partial class CEConfigData
    {
        public void Print(FlowDocument doc, int panelNumber, XfpData data)
        {
            _panelNumber = panelNumber;
            _data = data;

            var svgPathConverter = new SetTriggerTypeToSvgStringConverter();

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
        private const int _totalCEColumns = 15;
        private static SolidColorBrush _timerEventTimesUnderlineBrush = (SolidColorBrush)Application.Current.FindResource("Brush05");
        private static SolidColorBrush _errorBrush                    = (SolidColorBrush)Application.Current.FindResource("BrushError");
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

            for (int i = 0; i < 7; i++)
                grid.RowDefinitions.Add(new RowDefinition() { Height = i == 4 ? new GridLength(5) : GridLength.Auto });

            for (int i = 0; i < _totalTimerTimesColumns; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            grid.Children.Add(PrintUtil.GridCell(Cultures.Resources.Timer_Event_Times, 0, 0, 1, _totalTimerTimesColumns, true));

            grid.Children.Add(PrintUtil.GridBorder(1, 0, 1, _totalTimerTimesColumns, _timerEventTimesUnderlineBrush, new(0, 0, 0, 1), new(0), 3));

            grid.Children.Add(PrintUtil.SetMargin(PrintUtil.GridCell(Cultures.Resources.Timer_Event, 2, 0), new(3, 2, 10, 2)));
            grid.Children.Add(PrintUtil.SetMargin(PrintUtil.GridCell(Cultures.Resources.Occurs_At,   3, 0), new(3, 2, 10, 2)));
            grid.Children.Add(PrintUtil.SetMargin(PrintUtil.GridCell(Cultures.Resources.Timer_Event, 5, 0), new(3, 2, 10, 2)));
            grid.Children.Add(PrintUtil.SetMargin(PrintUtil.GridCell(Cultures.Resources.Occurs_At,   6, 0), new(3, 2, 10, 2)));

            for (int i = 0; i < 16; i++)
            {
                grid.Children.Add(PrintUtil.SetMargin(PrintUtil.GridCell(string.Format(Cultures.Resources.Time_T_x, i + 1), 3 * (i / 8) + 2, i % 8 + 1, HorizontalAlignment.Center), new(10, 2, 10, 2)));
                grid.Children.Add(PrintUtil.GridCellTimeSpan(TimerEventTimes[i],                                            3 * (i / 8) + 3, i % 8 + 1, "hm", false, true, HorizontalAlignment.Center));
            }

            return new(grid);
        }


        public BlockUIContainer ceList()
        {
            var grid = columnHeaders();

            //add the rows for the data
            for (int i = 0; i < NumEvents; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                int row = i + 1;
                int col = 0;

                //line number
                grid.Children.Add(PrintUtil.GridCell(i + 1, row, col++, HorizontalAlignment.Right));

                //action type
                grid.Children.Add(PrintUtil.GridCell(Enums.CEActionTypesToString(Events[i].ActionType), row, col++));
                if (Events[i].ActionType == CEActionTypes.None)
                    continue;

                //action parameter, if any
                if (actionHasParam(Events[i].ActionType))
                {
                    grid.Children.Add(PrintUtil.GridCell(getActionParamDesc(Events[i].ActionType, Events[i].ActionParam), row, col++));
                    if (Events[i].ActionParam < 0)
                        continue;
                }
                else
                {
                    col++;
                }
                
                //action trigger
                grid.Children.Add(PrintUtil.GridCell(Enums.CETriggerTypesToString(Events[i].TriggerType), row, col++));
                if (Events[i].TriggerType == CETriggerTypes.None)
                    continue;

                //trigger parameters
                if (triggerHasParam(Events[i].TriggerType))
                {
                    if (triggerHasParamPair(Events[i].TriggerType))
                    {
                        //grid.Children.Add(PrintUtil.GridCell(paramDesc(Events[i].TriggerType), row, col++));
                        var str = Events[i].TriggerType switch { CETriggerTypes.EventAnd => Cultures.Resources.Event_x, CETriggerTypes.ZoneAnd => Cultures.Resources.Zone_x, _ => "" };
                        grid.Children.Add(PrintUtil.GridCell(string.Format(str, Events[i].TriggerParam2), row, col++));

                        grid.Children.Add(PrintUtil.GridCell(getTriggerParamDesc(Events[i].TriggerType, Events[i].TriggerParam2), row, col++));

                        grid.Children.Add(PrintUtil.GridCell(Cultures.Resources.Logical_And, row, col++));
                        //if (Events[i].TriggerParam < 0)
                        //    continue;
                        grid.Children.Add(PrintUtil.GridCell(getTriggerParamDesc(Events[i].TriggerType, Events[i].TriggerParam), row, col++));
                        //addCell(grid, getTriggerParamDesc(Events[i].TriggerType, Events[i].TriggerParam), row, col++, Events[i].TriggerParam < 0);
                    }
                    else
                    {
                        grid.Children.Add(PrintUtil.GridCell(getTriggerParamDesc(Events[i].TriggerType, Events[i].TriggerParam), row, col, 1, 4));
                        col += 4;
                    }
                }
                else
                {
                    col += 4;
                }

                //trigger condition
                grid.Children.Add(PrintUtil.GridCell(isTrueOrFalse(Events[i].TriggerCondition), row, col++));
                if (Events[i].ResetType == CETriggerTypes.None)
                    continue;
                
                //reset trigger
                grid.Children.Add(PrintUtil.GridCell(Enums.CETriggerTypesToString(Events[i].ResetType), row, col++));
                if (Events[i].ResetType == CETriggerTypes.None)
                    continue;

                //reset parameters
                if (triggerHasParam(Events[i].ResetType))
                {
                    if (triggerHasParamPair(Events[i].ResetType))
                    {
                        grid.Children.Add(PrintUtil.GridCell(paramDesc(Events[i].ResetType), row, col++));
                        grid.Children.Add(PrintUtil.GridCell(getTriggerParamDesc(Events[i].ResetType, Events[i].ResetParam), row, col++));
                        grid.Children.Add(PrintUtil.GridCell(Cultures.Resources.Logical_And, row, col++));
                        if (Events[i].ResetParam2 < 0)
                            continue;
                        grid.Children.Add(PrintUtil.GridCell(getTriggerParamDesc(Events[i].ResetType, Events[i].ResetParam2), row, col++));
                    }
                    else
                    {
                        grid.Children.Add(PrintUtil.GridCell(getTriggerParamDesc(Events[i].ResetType, Events[i].ResetParam), row, col, 1, 4));
                        col += 4;
                    }
                }
                else
                {
                    col += 4;
                }

                //reset condition
                grid.Children.Add(PrintUtil.GridCell(isTrueOrFalse(Events[i].ResetCondition), row, col++));
            }

            return new(grid);
        }


        private void addCell(Grid grid, string text, int row, int column) => addCell(grid, text, row, column, 1, 1);
        private void addCell(Grid grid, string text, int row, int column, bool isError = false) => addCell(grid, text, row, column, 1, isError);
        private void addCell(Grid grid, string text, int row, int column, int columnSpan, bool isError = false) => addCell(grid, text, row, column, 1, columnSpan, isError);
        private void addCell(Grid grid, string text, int row, int column, int rowSpan, int columnSpan, bool isError = false)
        {
            if (isError)
                PrintUtil.GridBorder(row, column, rowSpan, columnSpan, _errorBrush, new(1), new(3));
            else 
                grid.Children.Add(PrintUtil.GridCell(text, row, column++, rowSpan, columnSpan));
        }


        private Grid columnHeaders()
        {
            Grid grid = new Grid();

            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            for (int i = 0; i < CEConfigData.NumEvents; i++)
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            for (int i = 0; i < _totalCEColumns; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            //header background
            grid.Children.Add(PrintUtil.GridBackground(0, 0, 1, _totalCEColumns, PrintUtil.GridHeaderBackground));

            //header text
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Number_Symbol, 0, 0, HorizontalAlignment.Right));
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Action,        0, 1, 1, 2));
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Occurs_When,   0, 3, 1, 5));
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Resets_When,   0, 9, 1, 5));

            return grid;
        }


        private bool actionHasParam(CEActionTypes actionType)
            => actionType switch
            {
                CEActionTypes.SilencePanel or
                CEActionTypes.ResetPanel or
                CEActionTypes.SetToOccupied or
                CEActionTypes.AbstractEvent or
                CEActionTypes.MutePanel or
                CEActionTypes.SetToUnoccupied or
                CEActionTypes.OutputDelaysDisable or
                CEActionTypes.EndPhasedEvacuation or
                CEActionTypes.EndZoneDelays => false,
                _ => true,
            };


        private bool triggerHasParam(CETriggerTypes triggerType)
            => triggerType switch
            {
                CETriggerTypes.AnyDeviceInAlarm or
                CETriggerTypes.AnyDisablement or
                CETriggerTypes.AnyDwellingAndCommunal or
                CETriggerTypes.AnyFault or
                CETriggerTypes.AnyPrealarm or
                CETriggerTypes.AnyRemotePanelInFire or
                CETriggerTypes.AnyZoneInFire or
                CETriggerTypes.MoreThanOneAlarm or
                CETriggerTypes.MoreThanOneZoneInAlarm or
                CETriggerTypes.NetworkEventTriggered or
                CETriggerTypes.PanelOccupied or
                CETriggerTypes.PanelReset or
                CETriggerTypes.PanelSilenced or
                CETriggerTypes.PanelUnoccupied => false,
                _ => true,
            };


        private bool triggerHasParamPair(CETriggerTypes triggerType) => triggerType == CETriggerTypes.EventAnd || triggerType == CETriggerTypes.ZoneAnd;
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
