using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CTecUtil.StandardPanelDataTypes;
using CTecUtil.Utils;
using Xfp.UI.Interfaces;

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
            
            //legacy setting from file format with single panel only
            AL2Code = original.AL2Code;
            AL3Code = original.AL3Code;
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


        //legacy setting from file format with single panel only - retained for backward compatibility with old data files
        public int LoopCount { get; set; }
        public bool DateEnabled { get; set; }
        public bool SoundersPulsed { get; set; }
        public bool CopyTime { get; set; }
        public string MaintenanceString { get; set; }
        public string QuiescentString { get; set; }
        public DateTime? MaintenanceDate { get; set; }
        public string AL2Code { get; set; }
        public string AL3Code { get; set; }
        public int MCPDebounce { get; set; } = 1;
        public int IODebounce { get; set; } = 1;
        public int DetectorDebounce { get; set; } = 1;
        public TimeSpan OccupiedBegins { get; set; }
        public TimeSpan OccupiedEnds { get; set; }
        public List<bool> DayStart   { get; set; }
        public List<bool> NightStart { get; set; }
        public bool BlinkPollingLED { get; set; }
        public bool AutoAdjustDST { get; set; }
        public bool RealTimeEventOutput { get; set; }


        /// <summary>
        /// Returns an initialised SiteConfigData object.
        /// </summary>
        public new static SiteConfigData InitialisedNew() => new () {
                                                                        SystemName = "XFP",
                                                                        Client     = NameAndAddressData.InitialisedNew(),
                                                                        Installer  = NameAndAddressData.InitialisedNew(),
                                                                        RecalibrationTime = new(4, 0, 0),

                                                                        //legacy settings
                                                                        QuiescentString   = Cultures.Resources.Default_Quiescent_String,
                                                                        MaintenanceString = Cultures.Resources.Default_Maintenance_String,
                                                                        AL2Code    = "3333",
                                                                        AL3Code    = "4444",
                                                                        BlinkPollingLED = true,
                                                                        AutoAdjustDST = true,
                                                                        DayStart   = new() { false, false, false, false, false, false, false },
                                                                        NightStart = new() { false, false, false, false, false, false, false },
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
                && od.EngineerName == EngineerName
                
                //legacy settings
                && od.EngineerName == EngineerName
                && od.AL2Code == AL2Code
                && od.AL3Code == AL3Code;
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


            //no need to validate legacy settings
            //var al2CodeErr = !ValidateAccessCode(AL2Code);
            //var al3CodeErr = !ValidateAccessCode(AL3Code);

            //if (al2CodeErr || al3CodeErr)
            //{
            //    ConfigErrorPageItems accessCodeErrs = new(0, Cultures.Resources.Access_Codes);
            //    if (al2CodeErr) accessCodeErrs.ValidationCodes.Add(ValidationCodes.SiteConfigAL2CodeError);
            //    if (al3CodeErr) accessCodeErrs.ValidationCodes.Add(ValidationCodes.SiteConfigAL3CodeError);
            //    _pageErrorOrWarningDetails.Items.Add(accessCodeErrs);
            //}

            //var quiescentStrBlank   = string.IsNullOrWhiteSpace(QuiescentString);
            //var maintStrBlank       = string.IsNullOrWhiteSpace(MaintenanceString);
            //var quiescentStrTooLong = QuiescentString?.Length   > PanelConfigData.MaxQuiescentStringLength;
            //var maintStrTooLong     = MaintenanceString?.Length > PanelConfigData.MaxMaintenanceStringLength;

            //if (quiescentStrBlank || quiescentStrTooLong || maintStrBlank || maintStrTooLong)
            //{
            //    ConfigErrorPageItems     deviceStrErrs = new(0, Cultures.Resources.Panel_Strings);
            //    if (quiescentStrBlank)   deviceStrErrs.ValidationCodes.Add(ValidationCodes.SiteConfigQuiescentStringBlank);
            //    if (quiescentStrTooLong) deviceStrErrs.ValidationCodes.Add(ValidationCodes.SiteConfigQuiescentStringTooLong);
            //    if (maintStrBlank)       deviceStrErrs.ValidationCodes.Add(ValidationCodes.SiteConfigMaintenanceStringBlank);
            //    if (maintStrTooLong)     deviceStrErrs.ValidationCodes.Add(ValidationCodes.SiteConfigMaintenanceStringTooLong);
            //    _pageErrorOrWarningDetails.Items.Add(deviceStrErrs);
            //}

            return _pageErrorOrWarningDetails.Items.Count == 0;
        }


        ///// <summary>Verifies that characters within the code are valid</summary>
        //public static bool ValidateAccessCodeChars(string code) => new Regex(@"^[1-4]+$").IsMatch(code);


        ///// <summary>Verifies that the code is valid (non-blank/correct length; valid chars)</summary>
        //public static bool ValidateAccessCode(string code) => !string.IsNullOrWhiteSpace(code) && ValidateAccessCodeChars(code) && code.Length == PanelConfigData.AccessCodeLength;


        //internal byte[] AL2CodeToByteArray() => ByteArrayUtil.StringToByteArray(AL2Code, DeviceNamesConfigData.DeviceNameLength);
        
        
        //public static string ParseAL2Code(byte[] data, Func<byte[], bool> responseTypeCheck)
        //{
        //    try
        //    {
        //        return TextUtil.IntToZeroPaddedString(Integer.Parse(data, responseTypeCheck, 2, 2).Value, PanelConfigData.AccessCodeLength);
        //    }
        //    catch (Exception ex)
        //    {
        //        CTecUtil.Debug.WriteLine(nameof(ParseAL2Code) + " failed: " + ex.ToString());
        //        return "";
        //    }
        //}

    }
}
