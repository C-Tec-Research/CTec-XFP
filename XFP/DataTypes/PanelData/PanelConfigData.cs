using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CTecUtil.StandardPanelDataTypes;
using CTecUtil.Utils;
using Xfp.UI.Interfaces;

namespace Xfp.DataTypes.PanelData
{
    public partial class PanelConfigData : ConfigData, IConfigData
    {
        public PanelConfigData() => _pageErrorOrWarningDetails = new(Cultures.Resources.Nav_Site_Configuration);

        public PanelConfigData(PanelConfigData original) : this()
        {
            if (original is null)
                return;
            
            FirmwareVersion     = original.FirmwareVersion;
            //LoopCount           = original.LoopCount;
            DateEnabled         = original.DateEnabled;
            SoundersPulsed      = original.SoundersPulsed;
            CopyTime            = original.CopyTime;
            MaintenanceString   = original.MaintenanceString;
            QuiescentString     = original.QuiescentString;
            MaintenanceDate     = original.MaintenanceDate;
            AL2Code             = original.AL2Code;
            AL3Code             = original.AL3Code;
            MCPDebounce         = original.MCPDebounce;
            IODebounce          = original.IODebounce;
            DetectorDebounce    = original.DetectorDebounce;
            OccupiedBegins      = original.OccupiedBegins;
            OccupiedEnds        = original.OccupiedEnds;
            BlinkPollingLED     = original.BlinkPollingLED;
            AutoAdjustDST       = original.AutoAdjustDST;
            RealTimeEventOutput = original.RealTimeEventOutput;
            FirmwareVersion     = original.FirmwareVersion;

            DayStart = new();
            if (original.DayStart is not null)
                foreach (var d in original.DayStart)
                    DayStart.Add(d);

            NightStart = new();
            if (original.NightStart is not null)
                foreach (var n in original.NightStart)
                    NightStart.Add(n);
        }

        /// <summary>
        /// initialise panel data from legacy settings.<br/>
        /// Provided for backward compatibility with old data file format for single panel; these settings used to be global.
        /// </summary>
        /// <param name="data"></param>
        public PanelConfigData(XfpData data) : this()
        {
            FirmwareVersion     = data.FirmwareVersion;
            //LoopCount           = LoopConfigData.MaxLoops;
            DateEnabled         = data.SiteConfig.DateEnabled;
            SoundersPulsed      = data.SiteConfig.SoundersPulsed;
            CopyTime            = data.SiteConfig.CopyTime;
            MaintenanceString   = data.SiteConfig.MaintenanceString;
            QuiescentString     = data.SiteConfig.QuiescentString;
            MaintenanceDate     = data.SiteConfig.MaintenanceDate;
            AL2Code             = data.SiteConfig.AL2Code;
            AL3Code             = data.SiteConfig.AL3Code;
            MCPDebounce         = data.SiteConfig.MCPDebounce;
            IODebounce          = data.SiteConfig.IODebounce;
            DetectorDebounce    = data.SiteConfig.DetectorDebounce;
            OccupiedBegins      = data.SiteConfig.OccupiedBegins;
            OccupiedEnds        = data.SiteConfig.OccupiedEnds;
            DayStart            = data.SiteConfig.DayStart;
            NightStart          = data.SiteConfig.NightStart;
            BlinkPollingLED     = data.SiteConfig.BlinkPollingLED;
            AutoAdjustDST       = data.SiteConfig.AutoAdjustDST;
            RealTimeEventOutput = data.SiteConfig.RealTimeEventOutput;
        }


        public const int MaxQuiescentStringLength = 40;
        public const int MaxMaintenanceStringLength = 40;
        public const int AccessCodeLength = 4;


        public string FirmwareVersion { get; set; }
        //public int LoopCount { get; set; }
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

        public List<bool> DayStart { get; set; } = new();
        public List<bool> NightStart { get; set; } = new();

        public bool BlinkPollingLED { get; set; }
        public bool AutoAdjustDST { get; set; }
        public bool RealTimeEventOutput { get; set; }
        
        
        public static bool IsValidPanel(int? panel) => panel.HasValue && panel >= XfpData.MinPanelNumber && panel <= XfpData.MaxPanelNumber;
        public static bool IsValidInput(int? input) => input.HasValue && input >= 0 && input < XfpPanelData.NumPanelInputs;


        /// <summary>
        /// Returns an initialised SiteConfigPanelData object.
        /// </summary>
        public new static PanelConfigData InitialisedNew() => new () {
                                                                        QuiescentString   = Cultures.Resources.Default_Quiescent_String,
                                                                        MaintenanceString = Cultures.Resources.Default_Maintenance_String,
                                                                        AL2Code           = Cultures.Resources.Default_AL2String,
                                                                        AL3Code           = Cultures.Resources.Default_AL3String,
                                                                        BlinkPollingLED   = true,
                                                                        AutoAdjustDST     = true,
                                                                        DayStart          = new() { false, false, false, false, false, false, false },
                                                                        NightStart        = new() { false, false, false, false, false, false, false }
                                                                    };


        public override bool Equals(ConfigData otherData)
        {
            if (otherData is not PanelConfigData od)
                return false;

            if (/*od.LoopCount != LoopCount
             || */od.DateEnabled != DateEnabled
             || od.SoundersPulsed != SoundersPulsed
             || od.MaintenanceString != MaintenanceString
             || od.QuiescentString != QuiescentString
             || od.MaintenanceDate != MaintenanceDate
             || od.AL2Code != AL2Code
             || od.AL3Code != AL3Code
             || od.MCPDebounce != MCPDebounce
             || od.IODebounce != IODebounce
             || od.DetectorDebounce != DetectorDebounce
             || od.OccupiedBegins != OccupiedBegins
             || od.OccupiedEnds != OccupiedEnds
             || od.BlinkPollingLED != BlinkPollingLED
             || od.AutoAdjustDST != AutoAdjustDST
             || od.RealTimeEventOutput != RealTimeEventOutput)
                return false;

            if (!(od.FirmwareVersion is null ^ FirmwareVersion is null))
                if (od.FirmwareVersion != FirmwareVersion)
                    return false;

            if ((od.DayStart is null ^ DayStart is null)
             || (od.NightStart is null ^ NightStart is null))
                return false;

            if (od.DayStart is not null
              && !od.DayStart.SequenceEqual(DayStart))
                return false;

            if (od.DayStart is not null
             && !od.NightStart.SequenceEqual(NightStart))
                return false;

            return true;
        }


        public static bool IsValidAccessCode(string code) => !string.IsNullOrWhiteSpace(code) && ValidateAccessCodeChars(code) && code.Length == AccessCodeLength;


        /// <summary>
        /// Checks whether the data's FirmwareVersion is the same as the specified version.
        /// </summary>
        /// <returns>Null if FirmwareVersion is null</returns>
        internal bool? FirmwareVersionEquals(string otherFirmwareVersion) => FirmwareVersion is not null ? FirmwareVersion == otherFirmwareVersion : null;


        /// <summary>
        /// Compares FirmwareVersion with the specified version number.
        /// </summary>
        /// <returns>1, 0 or -1 as per string Compare(), or null if either value is invalid or null</returns>
        internal int? FirmwareVersionCompare(string otherFirmwareVersion) => TextUtil.CompareFirmwareVersion(otherFirmwareVersion, FirmwareVersion);


        public override bool Validate()
        {
            _pageErrorOrWarningDetails.Items.Clear();

            var al2CodeErr = !ValidateAccessCode(AL2Code);
            var al3CodeErr = !ValidateAccessCode(AL3Code);

            if (al2CodeErr || al3CodeErr)
            {
                ConfigErrorPageItems accessCodeErrs = new(0, Cultures.Resources.Access_Codes);
                if (al2CodeErr) accessCodeErrs.ValidationCodes.Add(ValidationCodes.SiteConfigAL2CodeError);
                if (al3CodeErr) accessCodeErrs.ValidationCodes.Add(ValidationCodes.SiteConfigAL3CodeError);
                _pageErrorOrWarningDetails.Items.Add(accessCodeErrs);
            }

            var quiescentStrBlank   = string.IsNullOrWhiteSpace(QuiescentString);
            var maintStrBlank       = string.IsNullOrWhiteSpace(MaintenanceString);
            var quiescentStrTooLong = QuiescentString?.Length   > MaxQuiescentStringLength;
            var maintStrTooLong     = MaintenanceString?.Length > MaxMaintenanceStringLength;

            if (quiescentStrBlank || quiescentStrTooLong || maintStrBlank || maintStrTooLong)
            {
                ConfigErrorPageItems     deviceStrErrs = new(0, Cultures.Resources.Panel_Strings);
                if (quiescentStrBlank)   deviceStrErrs.ValidationCodes.Add(ValidationCodes.SiteConfigQuiescentStringBlank);
                if (quiescentStrTooLong) deviceStrErrs.ValidationCodes.Add(ValidationCodes.SiteConfigQuiescentStringTooLong);
                if (maintStrBlank)       deviceStrErrs.ValidationCodes.Add(ValidationCodes.SiteConfigMaintenanceStringBlank);
                if (maintStrTooLong)     deviceStrErrs.ValidationCodes.Add(ValidationCodes.SiteConfigMaintenanceStringTooLong);
                _pageErrorOrWarningDetails.Items.Add(deviceStrErrs);
            }

            return _pageErrorOrWarningDetails.Items.Count == 0;
        }


        /// <summary>Verifies that characters within the code are valid</summary>
        public static bool ValidateAccessCodeChars(string code) => new Regex(@"^[1-4]+$").IsMatch(code);


        /// <summary>Verifies that the code is valid (non-blank/correct length; valid chars)</summary>
        public static bool ValidateAccessCode(string code) => !string.IsNullOrWhiteSpace(code) && ValidateAccessCodeChars(code) && code.Length == AccessCodeLength;


        internal byte[] AL2CodeToByteArray() => ByteArrayUtil.StringToByteArray(AL2Code, DeviceNamesConfigData.DeviceNameLength);
        
        
        public static string ParseAL2Code(byte[] data, Func<byte[], bool> responseTypeCheck)
        {
            try
            {
                return TextUtil.IntToZeroPaddedString(Integer.Parse(data, responseTypeCheck, 2, 2).Value, AccessCodeLength);
            }
            catch (Exception ex)
            {
                CTecUtil.Debug.WriteLine(nameof(ParseAL2Code) + " failed: " + ex.ToString());
                return "";
            }
        }

    }
}
