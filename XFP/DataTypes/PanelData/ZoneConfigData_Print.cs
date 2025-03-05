using System;
using System.Windows.Documents;
using CTecDevices.Protocol;
using System.Windows.Media;
using CTecUtil.Printing;
using System.Windows;

namespace Xfp.DataTypes.PanelData
{
    public partial class ZoneConfigData
    {
        public void Print(FlowDocument doc, ZonePanelConfigData zonePanelData)
        {
            _zonePanelData = zonePanelData;

            var zonesPage = new Section();
            zonesPage.Blocks.Add(PrintUtil.PageHeader(Cultures.Resources.Nav_Zone_Configuration));

            zonesPage.Blocks.Add(headerInfo());
            
            zonesPage.Blocks.Add(zoneList());
            
            doc.Blocks.Add(zonesPage);
        }


        ZonePanelConfigData _zonePanelData;


        private Table headerInfo()
        {
            var result = new Table();
            var rg = new TableRowGroup();

            var tr = new TableRow(); 
            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.Input_Delay));
            tr.Cells.Add(PrintUtil.CellTimeSpan(InputDelay, true, TextAlignment.Left, "ms", true));
            rg.Rows.Add(tr);

            tr  = new TableRow(); 
            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.Investigation_Period));
            tr.Cells.Add(PrintUtil.CellTimeSpan(InvestigationPeriod, true, TextAlignment.Left, "ms", true));
            rg.Rows.Add(tr);

            result.RowGroups.Add(rg);
            return result;
        }


        private Table zoneList()
        {
            var result = new Table();

            result.RowGroups.Add(columnHeaders());
            var trg = new TableRowGroup();
            
            int row = 0;

            foreach (var z in Zones)
            {
                var tr = new TableRow() { Background = Int32.IsOddInteger(row) ? PrintUtil.TableAlternatingRowBackground : PrintUtil.TableNoBackground };
                tr.Cells.Add(PrintUtil.CellText(string.Format("{0,2} {1}", z.Number, z.Name)));
                tr.Cells.Add(PrintUtil.CellTimeSpan(z.SounderDelay, false, TextAlignment.Center, "ms"));
                tr.Cells.Add(PrintUtil.CellTimeSpan(z.Relay1Delay, false, TextAlignment.Center, "ms"));
                tr.Cells.Add(PrintUtil.CellTimeSpan(z.Relay2Delay, false, TextAlignment.Center, "ms"));
                tr.Cells.Add(PrintUtil.CellTimeSpan(z.RemoteDelay, false, TextAlignment.Center, "ms"));
                tr.Cells.Add(PrintUtil.CellBool(z.Detectors));
                tr.Cells.Add(PrintUtil.CellBool(z.MCPs));
                tr.Cells.Add(PrintUtil.CellBool(z.EndDelays));
                tr.Cells.Add(PrintUtil.CellText(Enums.ZoneDependencyOptionToString(z.Day.DependencyOption)));
                tr.Cells.Add(PrintUtil.CellTimeSpan(z.Day.DetectorReset, false, TextAlignment.Center, "ms"));
                tr.Cells.Add(PrintUtil.CellTimeSpan(z.Day.AlarmReset, false, TextAlignment.Center, "ms"));
                tr.Cells.Add(PrintUtil.CellText(Enums.ZoneDependencyOptionToString(z.Night.DependencyOption)));
                tr.Cells.Add(PrintUtil.CellTimeSpan(z.Night.DetectorReset, false, TextAlignment.Center, "ms"));
                tr.Cells.Add(PrintUtil.CellTimeSpan(z.Night.AlarmReset, false, TextAlignment.Center, "ms"));
                trg.Rows.Add(tr);
                row++;
            }

            foreach (var p in _zonePanelData.Panels)
            {
                var tr = new TableRow() { Background = Int32.IsOddInteger(row) ? PrintUtil.TableAlternatingRowBackground : PrintUtil.TableNoBackground };
                tr.Cells.Add(PrintUtil.CellText(string.Format("{0,2} {1}", p.Number, p.Name)));
                tr.Cells.Add(PrintUtil.CellTimeSpan(p.SounderDelay, false, TextAlignment.Center, "ms"));
                tr.Cells.Add(PrintUtil.CellTimeSpan(p.Relay1Delay, false, TextAlignment.Center, "ms"));
                tr.Cells.Add(PrintUtil.CellTimeSpan(p.Relay2Delay, false, TextAlignment.Center, "ms"));
                tr.Cells.Add(PrintUtil.CellTimeSpan(p.RemoteDelay, false, TextAlignment.Center, "ms"));
                trg.Rows.Add(tr);
                row++;
            }

            result.RowGroups.Add(trg);

            return result;
        }


        private TableRowGroup columnHeaders()
        {
            var result = new TableRowGroup();
            
            var tr = new TableRow() { Background = PrintUtil.TableHeaderBackground }; 
            tr.Cells.Add(PrintUtil.CellText(""));
            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.Output_Delays_Mins, 4, true, TextAlignment.Center));
            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.Functioning_With, 2, true, TextAlignment.Center));
            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.Multiple_Alarms_End_Delays, 1, 2, true, TextAlignment.Center));
            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.Day_Dependencies, 3, true, TextAlignment.Center));
            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.Night_Dependencies, 3, true, TextAlignment.Center));
            result.Rows.Add(tr);

            tr = new TableRow() { Background = PrintUtil.TableHeaderBackground };
            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.Zone, true));
            
            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.Sounders, true, TextAlignment.Center));
            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.Relay_1, true, TextAlignment.Center));
            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.Relay_2, true, TextAlignment.Center));
            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.Outputs, true, TextAlignment.Center));
            
            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.Detectors, true, TextAlignment.Center));
            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.MCPs, true, TextAlignment.Center));
            
            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.Option, true));
            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.Detector, true, TextAlignment.Right));
            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.Alarm, true, TextAlignment.Right));
            
            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.Option, true));
            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.Detector, true, TextAlignment.Right));
            tr.Cells.Add(PrintUtil.CellText(Cultures.Resources.Alarm, true, TextAlignment.Right));
            result.Rows.Add(tr);

            return result;
        }
    }
}
