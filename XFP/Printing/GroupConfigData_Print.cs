using CTecControls.UI;
using CTecUtil.Printing;
using CTecUtil.Utils;
using System;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Xfp.DataTypes.PanelData
{
    public partial class GroupConfigData
    {
        public void GetReport(FlowDocument doc, XfpPanelData panelData, ref int pageNumber)
        {
            //if (pageNumber++ > 1)
            //    PrintUtil.InsertPageBreak(doc);

            _data = panelData;

            GridUtil.ResetDefaults();
            TableUtil.ResetDefaults();
            TableUtil.SetForeground(PrintUtil.TextForeground);
            TableUtil.SetFontSize(PrintUtil.PrintSmallerFontSize);
            TableUtil.SetFontWeight(FontWeights.Normal);
            TableUtil.SetFontFamily(PrintUtil.PrintDefaultFont);
            TableUtil.SetPadding(PrintUtil.DefaultTableMargin);

            PrintUtil.PageHeader(doc, string.Format(Cultures.Resources.Panel_x, panelData.PanelNumber) + " - " + Cultures.Resources.Nav_Group_Configuration);

            var headerSection = new Section();
            headerSection.Blocks.Add(headerInfo());
            doc.Blocks.Add(headerSection);
            doc.Blocks.Add(printGroups());

            TableUtil.ResetDefaults();
        }
        
        
        private XfpPanelData _data;
        //private int    _totalColumns = NumSounderGroups + 2;
        //private int    _numColumns;
        private double _wNum;
        private double _wName;
        private double _wNumGroup;
        private Size   _iconSize = new(18, 14);


        private BlockUIContainer headerInfo()
        {
            var grid = new Grid();

            GridUtil.AddRowToGrid(grid);
            GridUtil.AddRowToGrid(grid);
            GridUtil.AddRowToGrid(grid);
            GridUtil.AddRowToGrid(grid);
            GridUtil.AddRowToGrid(grid);

            GridUtil.AddColumnToGrid(grid);
            GridUtil.AddColumnToGrid(grid);
            GridUtil.AddColumnToGrid(grid);
            GridUtil.AddColumnToGrid(grid);
            GridUtil.AddColumnToGrid(grid);
            GridUtil.AddColumnToGrid(grid);
            GridUtil.AddColumnToGrid(grid);

            grid.Children.Add(GridUtil.GridCell(appendColon(string.Format(Cultures.Resources.Panel_Sounder_x_Belongs_To_Sounder_Group, 1)), 0, 0, 1, 2));
            grid.Children.Add(GridUtil.GridCell(appendColon(string.Format(Cultures.Resources.Panel_Sounder_x_Belongs_To_Sounder_Group, 2)), 1, 0, 1, 2));
            
            GridUtil.AddCellToGrid(grid, PanelSounder1Group.ToString(), 0, 2, 1, 2, HorizontalAlignment.Left, true, !GroupConfigData.IsValidPanelSounderGroup(PanelSounder1Group));
            GridUtil.AddCellToGrid(grid, PanelSounder2Group.ToString(), 1, 2, 1, 2, HorizontalAlignment.Left, true, !GroupConfigData.IsValidPanelSounderGroup(PanelSounder2Group));

            grid.Children.Add(GridUtil.GridCell(" ", 0, 3));

            grid.Children.Add(GridUtil.GridCell(appendColon(Cultures.Resources.Evac_Tone),  0, 5));
            grid.Children.Add(GridUtil.GridCell(appendColon(Cultures.Resources.Alert_Tone), 1, 5));
            GridUtil.AddCellToGrid(grid, ContinuousTone.ToString(),   0, 6, 1, 1, HorizontalAlignment.Left, true, !GroupConfigData.IsValidAlarmTone(ContinuousTone));
            GridUtil.AddCellToGrid(grid, IntermittentTone.ToString(), 1, 6, 1, 1, HorizontalAlignment.Left, true, !GroupConfigData.IsValidAlarmTone(IntermittentTone));

            grid.Children.Add(GridUtil.GridCell(" ", 2, 0));

            grid.Children.Add(GridUtil.GridCell(appendColon(Cultures.Resources.New_Fire_Causes_Resound), 3, 0, false));
            grid.Children.Add(GridUtil.GridCell(appendColon(Cultures.Resources.Phased_Delay),            4, 0, 1, 2, false));
            grid.Children.Add(GridUtil.GridCellYesNo(ReSoundFunction, 3, 1, true, true));
            GridUtil.AddCellToGrid(grid, GridUtil.GridCellTimeSpan(PhasedDelay, 4, 1, 1, 3, "ms", true, true, HorizontalAlignment.Left, !GroupConfigData.IsValidPhasedDelay(PhasedDelay)));

            return new(grid);
        }


        private Table printGroups()
        {
            int dataRows = 0;

            var reportName = Cultures.Resources.Nav_Group_Configuration;

            try
            {
                var table = TableUtil.NewTable(reportName);

                defineColumnHeaders(table, reportName);

                var bodyGroup = new TableRowGroup();

                foreach (var z in _data.ZoneConfig.Zones)
                {
                    dataRows++;

                    //create the new row
                    var newRow = new TableRow() { Background = Int32.IsEvenInteger(dataRows) ? PrintUtil.TableAlternatingRowBackground : PrintUtil.NoBackground };
                    bodyGroup.Rows.Add(newRow);

                    newRow.Cells.Add(TableUtil.NewCell((z.Index + 1).ToString(), 1, 1, TextAlignment.Right));                   //zone number
                    newRow.Cells.Add(TableUtil.NewCell(z.Name));                                                                //zone name

                    for (int i = 0; i < NumSounderGroups; i++)
                        newRow.Cells.Add(TableUtil.NewCellImage(GridCellSounderIcon(z.SounderGroups[i]), 1, 1, _iconSize));     //icon
                }

                foreach (var p in _data.ZoneConfig.ZonePanelData.Panels)
                {
                    dataRows++;

                    //create the new row
                    var newRow = new TableRow() { Background = Int32.IsEvenInteger(dataRows) ? PrintUtil.TableAlternatingRowBackground : PrintUtil.NoBackground };
                    bodyGroup.Rows.Add(newRow);

                    newRow.Cells.Add(TableUtil.NewCell((p.Index + 1).ToString(), 1, 1, TextAlignment.Right));                   //zone number
                    newRow.Cells.Add(TableUtil.NewCell(p.Name));                                                                //zone name

                    for (int i = 0; i < NumSounderGroups; i++)
                        newRow.Cells.Add(TableUtil.NewCellImage(GridCellSounderIcon(p.SounderGroups[i]), 1, 1, _iconSize));     //icon
                }
                
                table.RowGroups.Add(bodyGroup);
                return table;
            }
            catch (Exception ex)
            {
                CTecMessageBox.ShowException(string.Format(CTecUtil.Cultures.Resources.Error_Generating_Report_x, reportName), CTecUtil.Cultures.Resources.Error_Printing, ex);
                return null;
            }
            finally
            {
                PrintUtil.ResetFont();
            }
        }


        private void defineColumnHeaders(Table table, string reportHeader)
        {            
            setColumnWidths();

            //define table's columns
            table.Columns.Add(new TableColumn() { Width = new GridLength(_wNum) });
            table.Columns.Add(new TableColumn() { Width = new GridLength(_wName) });
            for (int i = 0; i < NumSounderGroups; i++)
                table.Columns.Add(new TableColumn() { Width = new GridLength(_iconSize.Width) });

            //define rows for the header
            var headerRow1 = new TableRow();
            var headerRow2 = new TableRow();

            headerRow1.Background = headerRow2.Background = PrintUtil.TableHeaderBackground;
            
            headerRow1.Cells.Add(TableUtil.NewCell("", 1, 2, FontWeights.Bold));
            headerRow2.Cells.Add(TableUtil.NewCell(Cultures.Resources.Zone, 1, 2, FontWeights.Bold));

            headerRow1.Cells.Add(TableUtil.UnderlineCell(TableUtil.NewCell(Cultures.Resources.Triggers_Sounder_Groups, 1, NumSounderGroups, TextAlignment.Center, FontWeights.Bold), Styles.Brush04));
            
            for (int i = 0; i < NumSounderGroups; i++)
                headerRow2.Cells.Add(TableUtil.NewCell((i + 1).ToString(), TextAlignment.Center, FontWeights.Bold));
    
            var headerGroup = new TableRowGroup();
            headerGroup.Rows.Add(headerRow1);
            headerGroup.Rows.Add(headerRow2);

            table.RowGroups.Add(headerGroup);
        }


        private void setColumnWidths()
        {
            var cellMargins = (int)(PrintUtil.DefaultTableMargin.Left + PrintUtil.DefaultTableMargin.Right) + 1;
            
            //measure required column widths for columns
            _wNum      = TableUtil.MeasureText("99").Width + 1;
            _wName     = _data.ZoneConfig.GetMaxZoneNameLength();
            _wNumGroup = Math.Max(_wNum + _wName, TableUtil.MeasureText(Cultures.Resources.Zone).Width) + cellMargins + 1;
        }

        
        public static System.Windows.Shapes.Path GridCellSounderIcon(AlarmTypes value)
        {
            var colour = value switch
            {
                AlarmTypes.Alert    => Xfp.UI.Styles.AlarmAlertBrush,
                AlarmTypes.Evacuate => Xfp.UI.Styles.AlarmEvacBrush,
                _                   => Xfp.UI.Styles.AlarmOffBrush,
            };

            return new System.Windows.Shapes.Path() { Fill = colour, Style = Xfp.UI.Styles.AlarmIconStyle, Margin = PrintUtil.DefaultTableMargin };
        }
    }
}
