using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using CTecUtil.Printing;
using CTecUtil.Utils;
using static Xfp.DataTypes.PanelData.GroupConfigData;
using Xfp.UI;
using Xfp.UI.ViewHelpers;

namespace Xfp.DataTypes.PanelData
{
    public partial class SetConfigData
    {
        public void GetReport(FlowDocument doc, XfpPanelData panelData, ref int pageNumber)
        {
            if (pageNumber++ > 1)
                PrintUtil.InsertPageBreak(doc);

            _data = panelData;

            var svgPathConverter = new SetTriggerTypeToSvgPathConverter();
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
        private static SolidColorBrush _silenceableSetsBrush     = Styles.Brush07;
        private static SolidColorBrush _columnSeparatorBrush     = Styles.ColumnSeparatorBrush;
        private static SolidColorBrush _outputSetsBorderBrush    = Styles.Brush06;
        private static SolidColorBrush _panelRelayHeaderBrush    = Styles.PrintPanelRelayHeaderBrush;
        private static SolidColorBrush _triggerPulsedBrush       = Styles.TriggerPulsedBrush;
        private static SolidColorBrush _triggerContinuousBrush   = Styles.TriggerContinuousBrush;
        private static SolidColorBrush _triggerDelayedBrush      = Styles.TriggerDelayedBrush;
        private static SolidColorBrush _triggerNotTriggeredBrush = Styles.TriggerNotTriggeredBrush;
        private static string _triggerPulsedSvgData;
        private static string _triggerContinuousSvgData;
        private static string _triggerDelayedSvgData;
        private static string _triggerNotTriggeredSvgData;


        private BlockUIContainer headerInfo()
        {
            var grid = new Grid();

            GridUtil.AddRowToGrid(grid);
            GridUtil.AddColumnToGrid(grid);
            GridUtil.AddColumnToGrid(grid);

            grid.Children.Add(GridUtil.GridCell(appendColon(Cultures.Resources.Delay_Time), 0, 0));
            grid.Children.Add(GridUtil.GridCellTimeSpan(DelayTimer, 0, 1, "ms", true, true, HorizontalAlignment.Left));

            return new(grid);
        }


        public BlockUIContainer setList()
        {
            var grid = columnHeaders();

            //add the rows for the data
            for (int i = 0; i < _data.ZoneConfig.Zones.Count + _data.ZonePanelConfig.Panels.Count; i++)
                    GridUtil.AddRowToGrid(grid);

            int row = 4;

            //zone & panel names
            for (int z = 0; z < _data.ZoneConfig.Zones.Count; z++)
            {
                grid.Children.Add(GridUtil.GridBackground(row, 0, 1, _totalColumns, Int32.IsOddInteger(row) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));

                grid.Children.Add(GridUtil.GridCell(_data.ZoneConfig.Zones[z].Number, row, 0, HorizontalAlignment.Right));
                grid.Children.Add(GridUtil.GridCell(_data.ZoneConfig.Zones[z].Name,   row++, 1));
            }

            for (int p = 0; p < _data.ZonePanelConfig.Panels.Count; p++)
            {
                grid.Children.Add(GridUtil.GridBackground(row, 0, 1, _totalColumns, Int32.IsOddInteger(row) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));

                grid.Children.Add(GridUtil.GridCell(_data.ZonePanelConfig.Panels[p].Number, row, 0, HorizontalAlignment.Right));
                grid.Children.Add(GridUtil.GridCell(_data.ZonePanelConfig.Panels[p].Name,   row++, 1));
            }


            row = 3;
            int col = _setIdColumns;

            //Silenceable sets
            for (int t = 0; t < NumOutputSetTriggers; t++)
                grid.Children.Add(GridUtil.GridCellBool(_data.ZoneConfig.OutputSetIsSilenceable[t], row, col++, true, true));
            col++;
            for (int t = 0; t < NumPanelRelayTriggers; t++)
                grid.Children.Add(GridUtil.GridCellBool(_data.ZoneConfig.PanelRelayIsSilenceable[t], row, col++, true, true));

            row++;

            //trigger icons
            for (int s = 0; s < Sets.Count; s++)
            {
                col = _setIdColumns;

                for (int t = 0; t < NumOutputSetTriggers; t++)
                    grid.Children.Add(GridCellTriggerIcon(Sets[s].OutputSetTriggers[t], row, col++));
                
                grid.Children.Add(GridUtil.GridBackground(row, col++, 1, 1, _columnSeparatorBrush));

                for (int t = 0; t < NumPanelRelayTriggers; t++)
                    grid.Children.Add(GridCellTriggerIcon(Sets[s].PanelRelayTriggers[t], row, col++));

                row++;
            }

            GridUtil.AddRowToGrid(grid, 10);

            return new(grid);
        }


        private Grid columnHeaders()
        {
            Grid grid = new Grid();

            GridUtil.AddRowToGrid(grid);
            GridUtil.AddRowToGrid(grid, 7);
            GridUtil.AddRowToGrid(grid);
            GridUtil.AddRowToGrid(grid);

            for (int i = 0; i < Sets.Count; i++)
                GridUtil.AddRowToGrid(grid);
            
            for (int i = 0; i < _setIdColumns; i++)
                GridUtil.AddColumnToGrid(grid);

            for (int i = 0; i < NumOutputSetTriggers; i++)
                GridUtil.AddColumnToGrid(grid, 18);
            
            GridUtil.AddColumnToGrid(grid, 8);
            
            for (int i = 0; i < NumPanelRelayTriggers; i++)
                GridUtil.AddColumnToGrid(grid, 18);
 

            //header backgrounds
            grid.Children.Add(GridUtil.GridBackground(0, 0, 1, _totalColumns, PrintUtil.GridHeaderBackground));
            grid.Children.Add(GridUtil.GridBackground(1, 0, 1, _totalColumns, PrintUtil.GridHeaderBackground));
            grid.Children.Add(GridUtil.GridBackground(2, 0, 1, _totalColumns, PrintUtil.GridHeaderBackground));
            grid.Children.Add(GridUtil.GridBackground(3, 0, 1, _totalColumns, _silenceableSetsBrush));


            //header text
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Output_Set_Triggered, 0, 2, 1, NumOutputSetTriggers, HorizontalAlignment.Center));
            GridUtil.AddBorderToGrid(grid, 1, 2, 1, NumOutputSetTriggers, _outputSetsBorderBrush, new Thickness(1, 1, 1, 0), new CornerRadius(0), new(1.5, -2, 1.5, 0), 3);

            grid.Children.Add(GridUtil.SetForeground(GridUtil.GridHeaderCell(Cultures.Resources.Panel_Relay_Triggered, 0, 0, 1, _totalColumns, HorizontalAlignment.Right), _panelRelayHeaderBrush));
            GridUtil.AddBorderToGrid(grid, 1, _totalColumns - 2, 1, NumPanelRelayTriggers, _panelRelayHeaderBrush, new Thickness(1, 1, 1, 0), new CornerRadius(0), new(1.5, -2, 1.5, 0), 3);
            

            //set numbers
            int col = _setIdColumns;
            for (int i = 0; i < NumSounderGroups; i++)
                grid.Children.Add(GridUtil.GridHeaderCell(i + 1, 2, col++, HorizontalAlignment.Center));
            col++;
            for (int i = 0; i < NumPanelRelayTriggers; i++)
                grid.Children.Add(GridUtil.SetForeground(GridUtil.GridHeaderCell(i + 1, 2, col++, HorizontalAlignment.Center), _panelRelayHeaderBrush));

            
            //zone Num and name
            grid.Children.Add(GridUtil.GridHeaderCell(Cultures.Resources.Zone, 3, 0, 1, 2));
            
            
            //Is set silenceable label
            GridUtil.AddColumnToGrid(grid);
            grid.Children.Add(GridUtil.SetMargin(GridUtil.GridCell("← ", 3, _totalColumns, false, 16), new(3,-2,0,0)));
            GridUtil.AddColumnToGrid(grid);
            grid.Children.Add(GridUtil.SetMargin(GridUtil.GridCell(Cultures.Resources.Is_Set_Silenceable, 3, _totalColumns + 1), new(0,2,0,2)));

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
