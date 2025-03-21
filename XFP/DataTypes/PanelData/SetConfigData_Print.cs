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
    public partial class SetConfigData
    {
        public void Print(FlowDocument doc, XfpPanelData panelData)
        {
            _data = panelData;

            var svgPathConverter = new SetTriggerTypeToSvgStringConverter();
            _triggerPulsedSvgData       = (string)svgPathConverter.Convert(SetTriggerTypes.Pulsed,       typeof(string), null, null);
            _triggerContinuousSvgData   = (string)svgPathConverter.Convert(SetTriggerTypes.Continuous,   typeof(string), null, null);
            _triggerDelayedSvgData      = (string)svgPathConverter.Convert(SetTriggerTypes.Delayed,      typeof(string), null, null);
            _triggerNotTriggeredSvgData = (string)svgPathConverter.Convert(SetTriggerTypes.NotTriggered, typeof(string), null, null);

            var setsPage = new Section();
            setsPage.Blocks.Add(PrintUtil.PageHeader(string.Format(Cultures.Resources.Panel_x, panelData.PanelNumber) + " - " + Cultures.Resources.Nav_Set_Configuration));

            setsPage.Blocks.Add(headerInfo());
            setsPage.Blocks.Add(new BlockUIContainer(new TextBlock()));
            setsPage.Blocks.Add(setList());

            doc.Blocks.Add(setsPage);
        }
        

        private XfpPanelData _data;
        private const int _setIdColumns = 2;
        private const int _separatorColumns = 1;
        private const int _totalColumns = NumOutputSetTriggers + NumPanelRelayTriggers + _setIdColumns + _separatorColumns;
        private static SolidColorBrush _silenceableSetsBrush     = (SolidColorBrush)Application.Current.FindResource("Brush07");
        private static SolidColorBrush _columnSeparatorBrush     = new SolidColorBrush(Color.FromRgb(0xea, 0xea, 0xea));
        private static SolidColorBrush _outputSetsBorderBrush    = (SolidColorBrush)Application.Current.FindResource("Brush05");
        private static SolidColorBrush _panelRelayHeaderBrush    = (SolidColorBrush)Application.Current.FindResource("BrushPanelRelayPrintHeader");
        private static SolidColorBrush _triggerPulsedBrush       = (SolidColorBrush)Application.Current.FindResource("TriggerPulsedBrush");
        private static SolidColorBrush _triggerContinuousBrush   = (SolidColorBrush)Application.Current.FindResource("TriggerContinuousBrush");
        private static SolidColorBrush _triggerDelayedBrush      = (SolidColorBrush)Application.Current.FindResource("TriggerDelayedBrush");
        private static SolidColorBrush _triggerNotTriggeredBrush = (SolidColorBrush)Application.Current.FindResource("TriggerNotTriggeredBrush");
        private static string _triggerPulsedSvgData;
        private static string _triggerContinuousSvgData;
        private static string _triggerDelayedSvgData;
        private static string _triggerNotTriggeredSvgData;


        private BlockUIContainer headerInfo()
        {
            var grid = new Grid();

            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            grid.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Delay_Time), 0, 0));
            grid.Children.Add(PrintUtil.GridCellTimeSpan(DelayTimer, 0, 1, "ms", true, true, HorizontalAlignment.Left));

            return new(grid);
        }


        public BlockUIContainer setList()
        {
            var grid = columnHeaders();

            //add the rows for the data
            for (int i = 0; i < _data.ZoneConfig.Zones.Count + _data.ZonePanelConfig.Panels.Count; i++)
                    grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            int row = 4;

            //zone & panel names
            for (int z = 0; z < _data.ZoneConfig.Zones.Count; z++)
            {
                grid.Children.Add(PrintUtil.GridBackground(row, 0, 1, _totalColumns, Int32.IsOddInteger(row) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));

                grid.Children.Add(PrintUtil.GridCell(_data.ZoneConfig.Zones[z].Number, row, 0, HorizontalAlignment.Right));
                grid.Children.Add(PrintUtil.GridCell(_data.ZoneConfig.Zones[z].Name,   row++, 1));
            }

            for (int p = 0; p < _data.ZonePanelConfig.Panels.Count; p++)
            {
                grid.Children.Add(PrintUtil.GridBackground(row, 0, 1, _totalColumns, Int32.IsOddInteger(row) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));

                grid.Children.Add(PrintUtil.GridCell(_data.ZonePanelConfig.Panels[p].Number, row, 0, HorizontalAlignment.Right));
                grid.Children.Add(PrintUtil.GridCell(_data.ZonePanelConfig.Panels[p].Name,   row++, 1));
            }


            row = 3;
            int col = _setIdColumns;

            //Silenceable sets
            for (int t = 0; t < NumOutputSetTriggers; t++)
                grid.Children.Add(PrintUtil.GridCellBool(_data.ZoneConfig.OutputSetIsSilenceable[t], row, col++, true, true));
            col++;
            for (int t = 0; t < NumPanelRelayTriggers; t++)
                grid.Children.Add(PrintUtil.GridCellBool(_data.ZoneConfig.PanelRelayIsSilenceable[t], row, col++, true, true));

            row++;

            //trigger icons
            for (int s = 0; s < Sets.Count; s++)
            {
                col = _setIdColumns;

                for (int t = 0; t < NumOutputSetTriggers; t++)
                    grid.Children.Add(GridCellTriggerIcon(Sets[s].OutputSetTriggers[t], row, col++));
                
                grid.Children.Add(PrintUtil.GridBackground(row, col++, 1, 1, _columnSeparatorBrush));

                for (int t = 0; t < NumPanelRelayTriggers; t++)
                    grid.Children.Add(GridCellTriggerIcon(Sets[s].PanelRelayTriggers[t], row, col++));

                row++;
            }

            return new(grid);
        }


        private Grid columnHeaders()
        {
            Grid grid = new Grid();

            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(7) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            for (int i = 0; i < Sets.Count; i++)
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            
            for (int i = 0; i < _setIdColumns; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            for (int i = 0; i < NumOutputSetTriggers; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(18) });
            
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8) });
            
            for (int i = 0; i < NumPanelRelayTriggers; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(18) });


            //header backgrounds
            grid.Children.Add(PrintUtil.GridBackground(0, 0, 1, _totalColumns, PrintUtil.GridHeaderBackground));
            grid.Children.Add(PrintUtil.GridBackground(1, 0, 1, _totalColumns, PrintUtil.GridHeaderBackground));
            grid.Children.Add(PrintUtil.GridBackground(2, 0, 1, _totalColumns, PrintUtil.GridHeaderBackground));
            grid.Children.Add(PrintUtil.GridBackground(3, 0, 1, _totalColumns, _silenceableSetsBrush));


            //header text
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Output_Set_Triggered, 0, 2, 1, NumOutputSetTriggers, HorizontalAlignment.Center));
            PrintUtil.AddBorderToGrid(grid, 1, 2, 1, NumOutputSetTriggers, _outputSetsBorderBrush, new Thickness(1, 1, 1, 0), new CornerRadius(5, 5, 0, 0), 5);

            grid.Children.Add(PrintUtil.SetForeground(PrintUtil.GridHeaderCell(Cultures.Resources.Panel_Relay_Triggered, 0, 0, 1, _totalColumns, HorizontalAlignment.Right), _panelRelayHeaderBrush));
            PrintUtil.AddBorderToGrid(grid, 1, _totalColumns - 2, 1, NumPanelRelayTriggers, _panelRelayHeaderBrush, new Thickness(1, 1, 1, 0), new CornerRadius(5, 5, 0, 0), 5);
            

            //set numbers
            int col = _setIdColumns;
            for (int i = 0; i < NumSounderGroups; i++)
                grid.Children.Add(PrintUtil.GridHeaderCell(i + 1, 2, col++, HorizontalAlignment.Center));
            col++;
            for (int i = 0; i < NumPanelRelayTriggers; i++)
                grid.Children.Add(PrintUtil.SetForeground(PrintUtil.GridHeaderCell(i + 1, 2, col++, HorizontalAlignment.Center), _panelRelayHeaderBrush));

            
            //zone Num and name
            grid.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Zone, 3, 0, 1, 2));
            
            
            //Is set silenceable label
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.Children.Add(PrintUtil.SetMargin(PrintUtil.GridCell("← ", 3, _totalColumns, HorizontalAlignment.Left, 16, FontStyles.Normal), new(3,-2,0,0)));
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.Children.Add(PrintUtil.SetMargin(PrintUtil.GridCell(Cultures.Resources.Is_Set_Silenceable, 3, _totalColumns + 1), new(0,2,0,2)));

            return grid;
        }

        
        public static Grid GridCellTriggerIcon(SetTriggerTypes value, int row, int column)
        {
            var result = new Grid();

            var vb = new Viewbox() { Width = 14, Height = 14, HorizontalAlignment = HorizontalAlignment.Center };

            var colour = value switch
            {
                SetTriggerTypes.Pulsed     => _triggerPulsedBrush,
                SetTriggerTypes.Continuous => _triggerContinuousBrush,
                SetTriggerTypes.Delayed    => _triggerDelayedBrush,
                _                          => _triggerNotTriggeredBrush,
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
    }
}
