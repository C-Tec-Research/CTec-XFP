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

namespace Xfp.DataTypes.PanelData
{
    public partial class CEConfigData
    {
        public void Print(FlowDocument doc, int panelNumber)
        {
            var svgPathConverter = new SetTriggerTypeToSvgStringConverter();

            var cePage = new Section();
            cePage.Blocks.Add(PrintUtil.PageHeader(string.Format(Cultures.Resources.Panel_x, panelNumber) + " - " + Cultures.Resources.Nav_C_And_E_Configuration));

            cePage.Blocks.Add(timerEventTimes());
            cePage.Blocks.Add(new BlockUIContainer(new TextBlock()));
            cePage.Blocks.Add(ceList());

            doc.Blocks.Add(cePage);
        }
        

        private const int _totalTimerTimesColumns = 9;
        private const int _totalCEColumns = 17;
        private static SolidColorBrush _silenceableSetsBrush     = (SolidColorBrush)Application.Current.FindResource("Brush07");
        private static SolidColorBrush _columnSeparatorBrush     = new SolidColorBrush(Color.FromRgb(0xea, 0xea, 0xea));
        private static SolidColorBrush _outputSetsBorderBrush    = (SolidColorBrush)Application.Current.FindResource("Brush05");
        private static SolidColorBrush _panelRelayHeaderBrush    = (SolidColorBrush)Application.Current.FindResource("BrushPanelRelayPrintHeader");


        private BlockUIContainer timerEventTimes()
        {
            var grid = new Grid();

            for (int i = 0; i < 7; i++)
                grid.RowDefinitions.Add(new RowDefinition() { Height = i == 4 ? new GridLength(5) : GridLength.Auto });

            for (int i = 0; i < _totalTimerTimesColumns; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            grid.Children.Add(PrintUtil.GridCell(Cultures.Resources.Timer_Event_Times, 0, 0, 1, _totalTimerTimesColumns, true));

            grid.Children.Add(PrintUtil.GridBorder(1, 0, 1, _totalTimerTimesColumns, _outputSetsBorderBrush, new(0, 0, 0, 1), new(0), 3));

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
                grid.Children.Add(PrintUtil.GridCell(i + 1, row, col++, HorizontalAlignment.Right));
                grid.Children.Add(PrintUtil.GridCell(Enums.CEActionTypesToString(Events[i].ActionType), row, col++, HorizontalAlignment.Left));

                if (Events[i].ActionType == CEActionTypes.None)
                    continue;

                grid.Children.Add(PrintUtil.GridCell(getActionParamDesc(Events[i].ActionType, Events[i].ActionParam), row, col++, HorizontalAlignment.Right));


            }

            return new(grid);
        }


        private string getActionParamDesc(CEActionTypes action, int param)
        {
            //CEActionTypes.TriggerLoop1Device or CEActionTypes.Loop1DeviceDisable
            /*
            public bool ShowActionLoop1Devices => ActionType switch { CEActionTypes.TriggerLoop1Device or CEActionTypes.Loop1DeviceDisable => true, _ => false };
            public bool ShowActionLoop2Devices => ActionType switch { CEActionTypes.TriggerLoop2Device or CEActionTypes.Loop2DeviceDisable => true, _ => false };
            public bool ShowActionRelays => ActionType switch { CEActionTypes.PanelRelay => true, _ => false };
            public bool ShowActionGroups => ActionType switch { CEActionTypes.SounderAlert or CEActionTypes.SounderEvac or CEActionTypes.GroupDisable or CEActionTypes.TriggerBeacons => true, _ => false };
            public bool ShowActionZones => ActionType switch { CEActionTypes.ZoneDisable or CEActionTypes.PutZoneIntoAlarm => true, _ => false };
            public bool ShowActionSets => ActionType switch { CEActionTypes.TriggerOutputSet => true, _ => false };
            public bool ShowActionSetsAndRelays => ActionType switch { CEActionTypes.OutputDisable => true, _ => false };
            public bool ShowActionEvents => ActionType switch { CEActionTypes.TriggerNetworkEvent => true, _ => false };
             */

            return "";
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
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Action, 0, 1, 1, 2));
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Occurs_When, 0, 3, 1, 4));
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Resets_When, 0, 7, 1, 4));

            return grid;
        }
    }
}
