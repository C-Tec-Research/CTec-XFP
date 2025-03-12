using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using CTecUtil.Printing;

namespace Xfp.DataTypes.PanelData
{
    public partial class SiteConfigData
    {
        public void Print(FlowDocument doc, XfpPanelData data)
        {
            _data = data;

            var sitePage = new Section();
            sitePage.Blocks.Add(PrintUtil.PageHeader(Cultures.Resources.Nav_Site_Configuration));

            sitePage.Blocks.Add(new Paragraph(new Run(Cultures.Resources.System_Name + "\t: " + SystemName)));

            sitePage.Blocks.Add(new BlockUIContainer(systemHeader()));
            sitePage.Blocks.Add(new BlockUIContainer(namesAndAddresses()));

            //sitePage.Blocks.Add(pair(Cultures.Resources.Panel_Location, Loc));

            sitePage.Blocks.Add(new Paragraph(new Run("Blah blah blah")));
            
            doc.Blocks.Add(sitePage);
        }


        XfpPanelData _data;


        public Grid systemHeader()
        {
            var result = new Grid();

            result.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            result.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            result.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            result.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            result.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            result.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            result.Children.Add(PrintUtil.GridCell(appendColon(string.Format(Cultures.Resources.System_Name, 1)), 0, 0));
            result.Children.Add(PrintUtil.GridCell(SystemName, 0, 1));
            result.Children.Add(PrintUtil.GridCell(" ", 0, 2));
            result.Children.Add(PrintUtil.GridCell(appendColon(string.Format(Cultures.Resources.Firmware_Version, 1)), 0, 3));
        //    result.Children.Add(PrintUtil.GridCell(_data.Fir, 0, 1));

            return result;
        }


        public Grid namesAndAddresses()
        {
            var result = new Grid();

            result.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            result.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            result.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            result.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            result.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            result.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            result.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            //result.Children.Add(PrintUtil.GridCell(appendColon(string.Format(Cultures.Resources.Panel_Sounder_x_Belongs_To_Sounder_Group, 1)), 0, 0, 1, 2));
            //result.Children.Add(PrintUtil.GridCell(PanelSounder1Group, 0, 2));
            //result.Children.Add(PrintUtil.GridCell(appendColon(string.Format(Cultures.Resources.Panel_Sounder_x_Belongs_To_Sounder_Group, 2)), 1, 0, 1, 2));
            //result.Children.Add(PrintUtil.GridCell(PanelSounder1Group, 1, 2));

            //result.Children.Add(PrintUtil.GridCell(" ", 0, 3));

            //result.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Evac_Tone.ToString()), 0, 4));
            //result.Children.Add(PrintUtil.GridCell(ContinuousTone, 0, 5));
            //result.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Alert_Tone), 1, 4));
            //result.Children.Add(PrintUtil.GridCell(IntermittentTone, 1, 5));

            //result.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.New_Fire_Causes_Resound), 2, 0, 1, 2));
            //result.Children.Add(PrintUtil.GridCell(ReSoundFunction ? CTecControls.Cultures.Resources.Yes : CTecControls.Cultures.Resources.No, 2, 2));
            //result.Children.Add(PrintUtil.GridCell(appendColon(Cultures.Resources.Phased_Delay), 3, 0, 1, 2));
            //result.Children.Add(PrintUtil.GridCellTimeSpan(PhasedDelay, 3, 2, "ms", true, TextAlignment.Left));

            return result;

            //var result = new Paragraph();

            //result.Inlines.Add(nameValuePair(clientOrInstaller ? Cultures.Resources.Client_Name : Cultures.Resources.Installer_Address, nad.Name));
            //result.Inlines.Add(new LineBreak());

            //if (nad.Address.Count > 0)
            //{
            //    for (int i = 0; i < nad.Address.Count; i++)
            //    {
            //        result.Inlines.Add(nameValuePair(i == 0 ? clientOrInstaller ? Cultures.Resources.Client_Address : Cultures.Resources.Installer_Address : "", nad.Address[i]));
            //    result.Inlines.Add(new LineBreak());
            //    }
            //}
            //else
            //{
            //    result.Inlines.Add(nameValuePair(clientOrInstaller ? Cultures.Resources.Client_Address : Cultures.Resources.Installer_Address, ""));
            //    result.Inlines.Add(new LineBreak());
            //}

            //result.Inlines.Add(nameValuePair(Cultures.Resources.Postcode, nad.Postcode));
            //result.Inlines.Add(new LineBreak());

            //return result;
        }

        public Run nameValuePair(string left, string right)
        {
            var result = new Run(left + "\t: " + right);
            return result;
        }
    }
}
