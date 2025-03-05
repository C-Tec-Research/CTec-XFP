using System;
using System.Windows.Documents;
using CTecDevices.Protocol;
using System.Windows.Media;
using CTecUtil.Printing;
using static Xfp.DataTypes.PanelData.GroupConfigData;
using Xfp.UI.ViewHelpers;
using System.Windows;
using Xfp.UI.Views.PanelTools;

namespace Xfp.DataTypes.PanelData
{
    public partial class GroupConfigData
    {
        public void Print(FlowDocument doc)
        {
            var groupsPage = new Section();
            groupsPage.Blocks.Add(PrintUtil.PageHeader(Cultures.Resources.Nav_Group_Configuration));

            groupsPage.Blocks.Add(headerInfo());

            groupsPage.Blocks.Add(groupList());
            
            doc.Blocks.Add(groupsPage);
        }
        
        
        private Section headerInfo()
        {
            var result = new Section();
            var t  = new Table();
            var rg = new TableRowGroup();

            t.Columns.Add(new());

            var tr  = new TableRow(); 
            
            tr.Cells.Add(PrintUtil.CellText(string.Format(Cultures.Resources.Panel_Sounder_x_Belongs_To_Sounder_Group, 1)));
            tr.Cells.Add(PrintUtil.CellInt (PanelSounder1Group, true));

            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.Evac_Tone));
            tr.Cells.Add(PrintUtil.CellInt (ContinuousTone, true));

            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.New_Fire_Causes_Resound));
            tr.Cells.Add(PrintUtil.CellText(ReSoundFunction ? Cultures.Resources.True : Cultures.Resources.False, true));
            tr.Cells.Add(PrintUtil.CellText(""));
            rg.Rows.Add(tr);

            tr  = new TableRow(); 
            tr.Cells.Add(PrintUtil.CellText(string.Format(Cultures.Resources.Panel_Sounder_x_Belongs_To_Sounder_Group, 2)));
            tr.Cells.Add(PrintUtil.CellInt (PanelSounder2Group, true));

            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.Alert_Tone));
            tr.Cells.Add(PrintUtil.CellInt (IntermittentTone, true));

            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.Phased_Delay));
            tr.Cells.Add(PrintUtil.CellTimeSpan(PhasedDelay, false, TextAlignment.Center, "ms", true));
            rg.Rows.Add(tr);

            t.RowGroups.Add(rg);
            result.Blocks.Add(t);

            return result;
        }


        public Table groupList()
        {
            var result = new Table();
            result.RowGroups.Add(columnHeaders());
            
            //foreach (var z in SounderGroups)
            //{
            //    //if (printAllLoopDevices || DeviceTypes.IsValidDeviceType(z.DeviceType, DeviceTypes.CurrentProtocolType))
            //    //{
            //    //    var row = new TableRow();

            //    //    var cell = new TableCell();
            //    //}
            //}

            return result;
        }

        private TableRowGroup columnHeaders()
        {
            var result = new TableRowGroup();
            
            var tr = new TableRow() { Background = PrintUtil.TableHeaderBackground }; 
            tr.Cells.Add(PrintUtil.CellText("", 2));
            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.Triggers_Sounder_Groups, NumSounderGroups, true, TextAlignment.Center));
            result.Rows.Add(tr);

            tr = new TableRow() { Background = PrintUtil.TableHeaderBackground };
            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.Zone, 2, true));
            for (int i=0; i < NumSounderGroups; i++)
                tr.Cells.Add(PrintUtil.CellInt (i, 1, true, TextAlignment.Center));
            result.Rows.Add(tr);

            return result;
        }
    }
}
