using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xfp.DataTypes.PanelData;
using Xfp.UI.Views.PanelTools;

namespace Xfp.DataTypes
{
    public class XfpData : ConfigData
    {
        internal XfpData()
        {
            ToolsVersion = BuildInfo.Details.Version;
            Comments = "";
        }

        internal XfpData(XfpData original) : this()
        {
            if (original is null)
                return;

            foreach (var p in original.Panels)
                Panels.Add(p.Key, new(p.Value));

            SiteConfig      = new(original.SiteConfig);
            Comments        = original.Comments;
            //FirmwareVersion = original.FirmwareVersion;
            ToolsVersion    = original.ToolsVersion;
        }

        /// <summary>
        /// Copy-constructor based on older format of Xfp2 file.
        /// </summary>
        /// <param name="original"></param>
        internal XfpData(XfpData_OldVersion_1 original) : this()
        {
            if (original is null)
                return;

            SiteConfig = new(original.SiteConfig);

            Panels.Add(original.PanelNumber, new XfpPanelData()
            {
                PanelNumber       = original.PanelNumber,
                Protocol          = original.Protocol,
                CEConfig          = new(original.CEConfigData),
                LoopConfig        = new(original.LoopConfig),
                DeviceNamesConfig = new(original.DeviceNamesConfig),
                ZoneConfig        = new(original.ZoneConfig),
                ZonePanelConfig   = new(original.ZonePanelConfig),
                SetConfig         = new(original.SetConfig),
                GroupConfig       = new(original.GroupConfig),
                NetworkConfig     = new(original.NetworkConfig)
            });

            Comments        = original.Comments;
            //FirmwareVersion = original.FirmwareVersion;
            ToolsVersion    = original.ToolsVersion;
        }


        public Dictionary<int, XfpPanelData> Panels { get; set; } = new();

        [JsonIgnore]
        internal static int CurrentPanelNumber { get; set; } = MinPanelNumber;

        [JsonIgnore]
        public XfpPanelData CurrentPanel
        {
            get
            {
                    XfpPanelData result;
                if (Panels is not null && Panels.TryGetValue(CurrentPanelNumber, out result))
                    return result;

                result = Panels.First().Value;
                CurrentPanelNumber = result.PanelNumber;
                return result;
            }
        }

        
        public SiteConfigData     SiteConfig { get; set; }
        public string             Comments { get; set; }

        public string ToolsVersion { get; set; }

        //legacy setting from file format with single panel only - retained for backward compatibility with old data files
        public string FirmwareVersion { get; set; }


        public const int MinPanelNumber = 1;
        public const int MaxPanelNumber = 8;
        public const int EventLogDataLength = 128;


//        internal static new XfpData InitialisedNew() => InitialisedNew(CTecDevices.ObjectTypes.XfpCast, CurrentPanelNumber, true);
        
        internal static XfpData InitialisedNew(CTecDevices.ObjectTypes protocol, int panelNumber, bool setCurrentToThis)
        {
            var data = new XfpData();
            data.Panels.Add(panelNumber, XfpPanelData.InitialisedNew(protocol, panelNumber));
            data.SiteConfig = SiteConfigData.InitialisedNew();
            if (setCurrentToThis)
                CurrentPanelNumber = panelNumber;
            return data;
        }


        public override bool Equals(ConfigData otherData)
        {
            if (otherData is not XfpData od)
                return false;

            if (Panels?.Count != od.Panels?.Count)
                return false;

            foreach (var pk in Panels.Keys)
                if (!od.Panels.Keys.Contains(pk))
                    return false;

            foreach (var odpk in od.Panels.Keys)
                if (!Panels.Keys.Contains(odpk))
                    return false;

            foreach (var p in Panels)
                if (!p.Value.Equals(od.Panels[p.Key]))
                    return false;
            
           return SiteConfig.Equals(od.SiteConfig)
               //&& FirmwareVersion == od.FirmwareVersion
               //&& ToolsVersion    == od.ToolsVersion
               && Comments        == od.Comments;
        }


        ///// <summary>
        ///// Checks whether the data's FirmwareVersion is the same as the specified version.
        ///// </summary>
        ///// <returns>Null if FirmwareVersion is null</returns>
        //internal bool? FirmwareVersionEquals(string otherFirmwareVersion) => FirmwareVersion is not null ? FirmwareVersion == otherFirmwareVersion : null;

        /// <summary>
        /// Checks whether the data's ToolsVersion is the same as the specified version.
        /// </summary>
        /// <returns>Null if ToolsVersion is null</returns>
        internal bool? ToolsVersionEquals(string otherToolsVersion) => ToolsVersion is not null ? ToolsVersion == otherToolsVersion : null;


        ///// <summary>
        ///// Compares FirmwareVersion with the specified version number.
        ///// </summary>
        ///// <returns>1, 0 or -1 as per string Compare(), or null if either value is invalid or null</returns>
        //internal int? FirmwareVersionCompare(string otherFirmwareVersion) => CTecUtil.TextProcessing.CompareFirmwareVersion(otherFirmwareVersion, FirmwareVersion);


        /// <summary>
        /// Compares ToolsVersion with the specified version number.
        /// </summary>
        /// <returns>1, 0 or -1 as per string Compare(), or null if either value is invalid or null</returns>
        internal int? ToolsVersionCompare(string otherToolsVersion) => CTecUtil.TextProcessing.CompareSoftwareVersion(otherToolsVersion, ToolsVersion);


        /// <summary>Current errors or warnings</summary>
        private List<ConfigErrorPage> _errorsAndWarnings = new();


        public override bool Validate()
        {
            _errorsAndWarnings.Clear();

            foreach (var p in Panels)
                if (!p.Value.Validate())
                    _errorsAndWarnings.Add(p.Value.GetPageErrorDetails());

            if (!SiteConfig.Validate())
                _errorsAndWarnings.Add(SiteConfig.GetPageErrorDetails());

            return !HasErrorsOrWarnings();
        }


        internal new bool HasErrorsOrWarnings()
        {
            foreach (var _ in from p in Panels where p.Value.HasErrorsOrWarnings() select new { })
                return true;
            return SiteConfig.HasErrorsOrWarnings();
        }

        internal new bool HasErrors()
        {
            foreach (var _ in from p in Panels where p.Value.HasErrors() select new { })
                return true;
            return SiteConfig.HasErrors();
        }

        internal new bool HasWarnings()
        {
            foreach (var _ in from p in Panels where p.Value.HasWarnings() select new { })
                return true;
            return SiteConfig.HasWarnings();
        }
    }
}
