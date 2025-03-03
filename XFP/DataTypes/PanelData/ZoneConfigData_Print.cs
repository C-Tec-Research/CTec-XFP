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

            zonePage.Blocks.Add(zoneList());
            
            doc.Blocks.Add(zonePage);
        }


        public Table zoneList()
        {
            var result = new Table();

            var trg = new TableRowGroup();
            var tr  = new TableRow() { Background = new SolidColorBrush(Color.FromArgb(0x70, 0xe0, 0xe0, 0xe0)) }; 

            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Number_Symbol, PrintUtil.PrintDefaultFont, PrintUtil.PrintDefaultFontSize))));
            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Name, PrintUtil.PrintDefaultFont, PrintUtil.PrintDefaultFontSize))));
            //tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Zone_Group, PrintUtil.PrintDefaultFont, PrintUtil.PrintDefaultFontSize))));
            //tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Device_Name, PrintUtil.PrintDefaultFont, PrintUtil.PrintDefaultFontSize))));
            //tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Volume_Sensitivity_mode, PrintUtil.PrintDefaultFont, PrintUtil.PrintDefaultFontSize))));
            //tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Day_Night, PrintUtil.PrintDefaultFont, PrintUtil.PrintDefaultFontSize))));

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

        public Run nameValuePair(string left, string right)
        {
            var result = new Run(left + "\t: " + right);
            return result;
        }
    }
}
