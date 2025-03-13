using CTecUtil;
using CTecUtil.StandardPanelDataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xfp.UI.Interfaces;
using Xfp.UI.Validation;

namespace Xfp.DataTypes.PanelData
{
    public partial class SiteConfigData : ConfigData, IConfigData
    {
        public SiteConfigData() => _pageErrorOrWarningDetails = new(Cultures.Resources.Nav_Site_Configuration);

        public SiteConfigData(SiteConfigData original) : this()
        {
            if (original is null)
                return;
            SystemName = original.SystemName;
            Client = new NameAndAddressData(original.Client);
            Client.Tel = original.Client.Tel;
            InstallDate = original.InstallDate;
            CommissionDate = original.CommissionDate;
            Installer = new NameAndAddressData(original.Installer);
            Installer.Tel = original.Client.Tel;
            EngineerName = original.EngineerName;
        }


        public const int MaxLocationLength = 100;


        public string SystemName { get; set; }
        public NameAndAddressData Client { get; set; }
        public DateTime? InstallDate { get; set; }
        public DateTime? CommissionDate { get; set; }
        public NameAndAddressData Installer { get; set; }
        public string EngineerName { get; set; }
        public string EngineerNo { get; set; }
        public int FaultLockout { get; set; }

        public string FrontPanel { get; set; }
        public TimeSpan RecalibrationTime { get; set; }


        /// <summary>
        /// Returns an initialised SiteConfigData object.
        /// </summary>
        public new static SiteConfigData InitialisedNew() => new () {
                                                                        SystemName = "XFP",
                                                                        Client     = NameAndAddressData.InitialisedNew(),
                                                                        Installer  = NameAndAddressData.InitialisedNew(),
                                                                        RecalibrationTime = new(4, 0, 0),
                                                                    };


        public override bool Equals(ConfigData otherData)
        {
            if (otherData is not SiteConfigData od)
                return false;
            return od.SystemName == SystemName
                && od.Client.Equals(Client)
                && od.Client.Tel == Client.Tel
                && od.InstallDate == InstallDate
                && od.CommissionDate == CommissionDate
                && od.Installer.Equals(Installer)
                && od.Client.Tel == Installer.Tel
                && od.EngineerName == EngineerName;
        }


        public override bool Validate()
        {
            _pageErrorOrWarningDetails.Items.Clear();

            if (string.IsNullOrWhiteSpace(SystemName))
            {
                ConfigErrorPageItems systemErrs = new(0, Cultures.Resources.System);
                systemErrs.ValidationCodes.Add(ValidationCodes.SiteConfigNoSystemNameWarning);
                _pageErrorOrWarningDetails.Items.Add(systemErrs);
            }

            var clientNameErr  = string.IsNullOrWhiteSpace(Client.Name);
            var clientAddrErr  = Client.AddressIsEmpty();
            var clientTelErr   = string.IsNullOrWhiteSpace(Client.Tel);
            var installDateErr = InstallDate is null;
            var commissDateErr = CommissionDate is null;

            if (clientNameErr || clientAddrErr || clientTelErr)
            {
                ConfigErrorPageItems clientErrs = new(0, Cultures.Resources.Client_Details);
                if (clientNameErr)  clientErrs.ValidationCodes.Add(ValidationCodes.SiteConfigNoClientName);
                if (clientAddrErr)  clientErrs.ValidationCodes.Add(ValidationCodes.SiteConfigNoClientAddress);
                if (clientTelErr)   clientErrs.ValidationCodes.Add(ValidationCodes.SiteConfigNoClientTel);
                if (installDateErr) clientErrs.ValidationCodes.Add(ValidationCodes.SiteConfigNoInstallDate);
                if (commissDateErr) clientErrs.ValidationCodes.Add(ValidationCodes.SiteConfigNoCommissionDate);
                _pageErrorOrWarningDetails.Items.Add(clientErrs);
            }

            var installerNameErr = string.IsNullOrWhiteSpace(Installer.Name);
            var installerAddrErr = Installer.AddressIsEmpty();
            var installerTelErr  = string.IsNullOrWhiteSpace(Installer.Tel);
            var engineerNameErr  = string.IsNullOrWhiteSpace(EngineerName);

            if (installerNameErr || installerAddrErr || installerTelErr|| engineerNameErr)
            {
                ConfigErrorPageItems installerErrs = new(0, Cultures.Resources.Installation_Details);
                if (installerNameErr) installerErrs.ValidationCodes.Add(ValidationCodes.SiteConfigNoInstallerName);
                if (installerAddrErr) installerErrs.ValidationCodes.Add(ValidationCodes.SiteConfigNoInstallerAddress);
                if (installerTelErr)  installerErrs.ValidationCodes.Add(ValidationCodes.SiteConfigNoInstallerTel);
                if (engineerNameErr)  installerErrs.ValidationCodes.Add(ValidationCodes.SiteConfigNoEngineerName);
                _pageErrorOrWarningDetails.Items.Add(installerErrs);
            }

            return _pageErrorOrWarningDetails.Items.Count == 0;
        }
    }
}
