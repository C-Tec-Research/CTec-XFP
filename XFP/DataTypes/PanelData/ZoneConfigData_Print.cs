using System;
using System.Windows.Documents;
using CTecDevices.Protocol;
using System.Windows.Media;
using CTecUtil.Printing;

namespace Xfp.DataTypes.PanelData
{
    public partial class ZoneConfigData
    {
        public void Print(FlowDocument doc)
        {
            var zonePage = new Section();
            zonePage.Blocks.Add(PrintUtil.PageHeader(Cultures.Resources.Nav_Zone_Configuration));

            var delays = new Paragraph();
            delays.Inlines.Add(PrintUtil.Text(Cultures.Resources.Input_Delay          + string.Format(" : {0:00}:{1:00} ", InputDelay.Minutes, InputDelay.Seconds)                   + Cultures.Resources.Minutes_Short));
            delays.Inlines.Add(PrintUtil.Text(Cultures.Resources.Investigation_Period + string.Format(" : {0:00}:{1:00} ", InvestigationPeriod.Minutes, InvestigationPeriod.Seconds) + Cultures.Resources.Minutes_Short));
            zonePage.Blocks.Add(delays);

            zonePage.Blocks.Add(zoneList());
            
            doc.Blocks.Add(zonePage);
        }


        public Table zoneList()
        {
            var result = new Table();

            result.RowGroups.Add(columnHeaders());
            
            foreach (var z in Zones)
            {
                //if (printAllLoopDevices || DeviceTypes.IsValidDeviceType(z.DeviceType, DeviceTypes.CurrentProtocolType))
                //{
                //    var row = new TableRow();

                //    var cell = new TableCell();
                //}
            }

            return result;
        }

        private TableRowGroup columnHeaders()
        {
            var result = new TableRowGroup();
            var tr  = new TableRow() { Background = new SolidColorBrush(Color.FromArgb(0x70, 0xe0, 0xe0, 0xe0)) }; 

            tr.Cells.Add(new TableCell(new Paragraph()) { ColumnSpan = 2 });
            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Output_Delays_Mins))) { ColumnSpan = 4 });
            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Functioning_With))) { ColumnSpan = 2 });
            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Multiple_Alarms_End_Delays))) { RowSpan = 2 });
            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Day_Dependencies))) { ColumnSpan = 3 });
            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Night_Dependencies))) { ColumnSpan = 3 });

            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Number_Symbol))));
            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Description))));
            
            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Sounders))));
            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Relay_1))));
            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Relay_2))));
            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Outputs))));
            
            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Detectors))));
            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.MCPs))));
            
            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Option))));
            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Detector))));
            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Alarm))));
            
            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Option))));
            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Detector))));
            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Alarm))));
            
            result.Rows.Add(tr);
            return result;
        }

        public Run nameValuePair(string left, string right)
        {
            var result = new Run(left + "\t: " + right);
            return result;
        }
    }
}
