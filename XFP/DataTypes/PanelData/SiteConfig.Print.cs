using System.Windows.Documents;
using CTecUtil.Printing;

namespace Xfp.DataTypes.PanelData
{
    public partial class SiteConfigData
    {
        public void Print(FlowDocument doc)
        {
            var sitePage = new Section();
            sitePage.Blocks.Add(PrintUtil.PageHeader(Cultures.Resources.Nav_Site_Configuration));

            sitePage.Blocks.Add(new Paragraph(new Run(Cultures.Resources.System_Name + ": " + SystemName)));

            sitePage.Blocks.Add(nameAndAddress(Client, true));
            sitePage.Blocks.Add(nameAndAddress(Installer, false));

            //sitePage.Blocks.Add(pair(Cultures.Resources.Panel_Location, Loc));

            sitePage.Blocks.Add(new Paragraph(new Run("Blah blah blah")));
            
            doc.Blocks.Add(sitePage);
        }


        public Block nameAndAddress(NameAndAddressData nad, bool clientOrInstaller)
        {
            var result = new Paragraph();

            result.Inlines.Add(nameValuePair(clientOrInstaller ? Cultures.Resources.Client_Name : Cultures.Resources.Installer_Address, nad.Name));
            result.Inlines.Add(new LineBreak());

            if (nad.Address.Count > 0)
            {
                for (int i = 0; i < nad.Address.Count; i++)
                {
                    result.Inlines.Add(nameValuePair(i == 0 ? clientOrInstaller ? Cultures.Resources.Client_Address : Cultures.Resources.Installer_Address : "", nad.Address[i]));
                result.Inlines.Add(new LineBreak());
                }
            }
            else
            {
                result.Inlines.Add(nameValuePair(clientOrInstaller ? Cultures.Resources.Client_Address : Cultures.Resources.Installer_Address, ""));
                result.Inlines.Add(new LineBreak());
            }

            result.Inlines.Add(nameValuePair(Cultures.Resources.Postcode, nad.Postcode));
            result.Inlines.Add(new LineBreak());

            return result;
        }

        public Run nameValuePair(string left, string right)
        {
            var result = new Run(left + " : " + right);
            return result;
        }
    }
}
