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

namespace Xfp.DataTypes.PanelData
{
    public partial class GroupConfigData
    {
        public void Print(FlowDocument doc, XfpPanelData panelData)
        {
            _data = panelData;

            var groupsPage = new Section();
            groupsPage.Blocks.Add(PrintUtil.PageHeader(Cultures.Resources.Nav_Group_Configuration));

            groupsPage.Blocks.Add(new BlockUIContainer(headerInfo()));
            groupsPage.Blocks.Add(new BlockUIContainer(new TextBlock()));
            groupsPage.Blocks.Add(new BlockUIContainer(groupList()));

            doc.Blocks.Add(groupsPage);
        }
        
        
        private XfpPanelData _data;
        private int totalColumns = NumSounderGroups + 2;
        private static Style _alertIconStyle = (Style)Application.Current.FindResource("AlarmIcon");
        private static Brush _alarmAlertFill = (Brush)Application.Current.FindResource("AlarmAlertBrush");
        private static Brush _alarmEvacFill  = (Brush)Application.Current.FindResource("AlarmEvacBrush");
        private static Brush _alarmOffFill   = (Brush)Application.Current.FindResource("AlarmOffBrush");


        private Grid headerInfo()
        {
            var result = new Grid();

            result.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            result.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            result.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            result.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            result.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            result.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            result.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            result.Children.Add(PrintUtil.GridCell(appendColon(string.Format(Cultures.Resources.Panel_Sounder_x_Belongs_To_Sounder_Group, 1)), 0, 0, 1, 2));
            result.Children.Add(PrintUtil.GridCell(PanelSounder1Group, 0, 2));
            result.Children.Add(PrintUtil.GridCell(appendColon(string.Format(Cultures.Resources.Panel_Sounder_x_Belongs_To_Sounder_Group, 2)), 1, 0, 1, 2));
            result.Children.Add(PrintUtil.GridCell(PanelSounder1Group, 1, 2));

            result.Children.Add(PrintUtil.GridCell(" ", 0, 3));

            result.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Evac_Tone.ToString()), 0, 4));
            result.Children.Add(PrintUtil.GridCell(ContinuousTone, 0, 5));
            result.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Alert_Tone), 1, 4));
            result.Children.Add(PrintUtil.GridCell(IntermittentTone, 1, 5));

            result.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.New_Fire_Causes_Resound), 2, 0, 1, 2));
            result.Children.Add(PrintUtil.GridCell(ReSoundFunction ? CTecControls.Cultures.Resources.Yes : CTecControls.Cultures.Resources.No, 2, 2));
            result.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Phased_Delay), 3, 0, 1, 2));
            result.Children.Add(PrintUtil.GridCellTimeSpan(PhasedDelay, 3, 2, "ms", true, TextAlignment.Left));

            return result;
        }


        public Grid groupList()
        {
            var grid = columnHeaders();

            //add the rows for the data
            for (int i = 0; i < _data.ZoneConfig.Zones.Count + _data.ZonePanelConfig.Panels.Count; i++)
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            int row = 2;
            int col = 0;

            foreach (var z in _data.ZoneConfig.Zones)
            {
                col = 0;

                grid.Children.Add(PrintUtil.GridBackground(row, 0, 1, totalColumns, Int32.IsOddInteger(row) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));
            
                grid.Children.Add(PrintUtil.GridCell(z.Number, row, col++, TextAlignment.Right));
                grid.Children.Add(PrintUtil.GridCell(z.Name, row, col++));

                for (int i = 0; i < NumSounderGroups; i++)
                    grid.Children.Add(GridCellSounderIcon(z.SounderGroups[i], row, col++));

                row++;
            }

            foreach (var p in _data.ZonePanelConfig.Panels)
            {
                col = 0;
                
                grid.Children.Add(PrintUtil.GridBackground(row, 0, 1, totalColumns, Int32.IsOddInteger(row) ? PrintUtil.GridAlternatingRowBackground : PrintUtil.NoBackground));
                
                grid.Children.Add(PrintUtil.GridCell(p.Number, row, col++, TextAlignment.Right));
                grid.Children.Add(PrintUtil.GridCell(p.Name, row, col++));

                for (int i = 0; i < NumSounderGroups; i++)
                    grid.Children.Add(GridCellSounderIcon(p.SounderGroups[i], row, col++));
                
                row++;
            }

            return grid;
        }


        private Grid columnHeaders()
        {
            Grid g = new Grid();

            //two rows for column headers
            g.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            g.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            for (int i = 0; i < totalColumns; i++)
                g.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            g.Children.Add(PrintUtil.GridBackground(0, 0, 1, totalColumns, PrintUtil.GridHeaderBackground));
            g.Children.Add(PrintUtil.GridBackground(1, 0, 1, totalColumns, PrintUtil.GridHeaderBackground));

            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Triggers_Sounder_Groups, 0, 1, 1, totalColumns, TextAlignment.Center));
            
            int col = 0;
            g.Children.Add(PrintUtil.GridHeaderCell(Cultures.Resources.Zone, 1, col++, 1, 2));
            col++;

            for (int i = 0; i < NumSounderGroups; i++)
                g.Children.Add(PrintUtil.GridHeaderCell(i + 1, 1, col++, TextAlignment.Center));

            return g;
        }

        
        public static Grid GridCellSounderIcon(AlarmTypes value, int row, int column)
        {
            var result = new Grid();

            var vb = new Viewbox() { Width = 12, Height = 12, HorizontalAlignment = HorizontalAlignment.Center };

            var colour = value switch
            {
                AlarmTypes.Alert    => _alarmAlertFill,
                AlarmTypes.Evacuate => _alarmEvacFill,
                _                   => _alarmOffFill,
            };

            var p = new System.Windows.Shapes.Path() { Fill = colour, Style = _alertIconStyle };
            vb.Child = p;

            result.SetValue(Grid.RowProperty, row);
            result.SetValue(Grid.ColumnProperty, column);
            result.Children.Add(vb);
            return result;
        }
    }
}
