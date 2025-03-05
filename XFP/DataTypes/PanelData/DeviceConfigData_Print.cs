using System.Windows.Documents;
using System.Windows.Media;
using CTecDevices.Protocol;
using CTecUtil.Printing;

namespace Xfp.DataTypes.PanelData
{
    public partial class DeviceConfigData
    {
        public void Print(FlowDocument doc, bool printAllLoopDevices)
        {
            var devicePage = new Section();
            devicePage.Blocks.Add(PrintUtil.PageHeader(Cultures.Resources.Nav_Device_Details));

            devicePage.Blocks.Add(deviceList(printAllLoopDevices));

            doc.Blocks.Add(devicePage);
        }


        public Table deviceList(bool printAllLoopDevices)
        {
            var result = new Table();

            var trg = new TableRowGroup();
            var tr  = new TableRow() { Background = PrintUtil.TableHeaderBackground }; 

            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Number_Symbol))));
            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Device_Type))));
            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Zone_Group))));
            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Device_Name))));
            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Volume_Sensitivity_mode))));
            tr.Cells.Add(new TableCell(new Paragraph(PrintUtil.BoldText(Cultures.Resources.Day_Night))));

            int row = 0;

            foreach (var d in Devices)
            {
                if (printAllLoopDevices || DeviceTypes.IsValidDeviceType(d.DeviceType, DeviceTypes.CurrentProtocolType))
                {
                    tr = new TableRow() { Background = row % 2 == 1 ? PrintUtil.TableAlternatingRowBackground : PrintUtil.TableNoBackground };

                    var cell = new TableCell();
                }
            }

            return result;
        }
    }
}
