using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CTecDevices;
using Newtonsoft.Json;
using Xfp.DataTypes.PanelData;
using Xfp.UI.Views.PanelTools;

namespace Xfp.DataTypes
{
    public class XfpData_OldVersion_1 : ConfigData
    {
        internal XfpData_OldVersion_1()
        {
            ToolsVersion = BuildInfo.Details.Version;
            Comments = "";
        }

        
        public LoopConfigData        LoopConfig { get; set; }
        public DeviceNamesConfigData DeviceNamesConfig { get; set; }
        public ZoneConfigData        ZoneConfig { get; set; }
        public ZonePanelConfigData   ZonePanelConfig { get; set; }
        public SetConfigData         SetConfig { get; set; }
        public GroupConfigData       GroupConfig { get; set; }
        public CEConfigData          CEConfigData { get; set; }
        public SiteConfigData        SiteConfig { get; set; }
        public NetworkConfigData     NetworkConfig { get; set; }
        public string                Comments { get; set; }
        
        public const int MinPanelNumber = 1;
        public const int MaxPanelNumber = 8;

        public ObjectTypes Protocol { get; set; }
        public int    PanelNumber { get; set; }
        public string FirmwareVersion { get; set; }
        public string ToolsVersion { get; set; }


                
        //internal static XfpDataVersion1 InitialisedNew()
        //{
        //    var data = new XfpDataVersion1();
        //    data.LoopConfig = LoopConfigData.InitialisedNew();
        //    data.DeviceNamesConfig = DeviceNamesConfigData.InitialisedNew();
        //    data.ZoneConfig = ZoneConfigData.InitialisedNew();
        //    data.ZonePanelConfig = ZonePanelConfigData.InitialisedNew();
        //    data.SetConfig = SetConfigData.InitialisedNew();
        //    data.GroupConfig = GroupConfigData.InitialisedNew();
        //    data.CEConfigData = CEConfigData.InitialisedNew();
        //    data.SiteConfig = SiteConfigData.InitialisedNew();
        //    data.NetworkConfig = NetworkConfigData.InitialisedNew();
        //    return data;
        //}


        //public override bool Equals(ConfigData otherData)
        //{
        //    if (otherData is not XfpDataVersion1 od)
        //        return false;

        //    if (Panels?.Count != od.Panels?.Count)
        //        return false;

        //    foreach (var pk in Panels.Keys)
        //        if (!od.Panels.Keys.Contains(pk))
        //            return false;

        //    foreach (var odpk in od.Panels.Keys)
        //        if (!Panels.Keys.Contains(odpk))
        //            return false;

        //    foreach (var p in Panels)
        //        if (!p.Value.Equals(od.Panels[p.Key]))
        //            return false;
            
        //   return SiteConfig.Equals(od.SiteConfig)
        //       //&& FirmwareVersion == od.FirmwareVersion
        //       //&& ToolsVersion    == od.ToolsVersion
        //       && Comments        == od.Comments;
        //}


        ///// <summary>
        ///// Checks whether the data's FirmwareVersion is the same as the specified version.
        ///// </summary>
        ///// <returns>Null if FirmwareVersion is null</returns>
        //internal bool? FirmwareVersionEquals(string otherFirmwareVersion) => FirmwareVersion is not null ? FirmwareVersion == otherFirmwareVersion : null;

        ///// <summary>
        ///// Checks whether the data's ToolsVersion is the same as the specified version.
        ///// </summary>
        ///// <returns>Null if ToolsVersion is null</returns>
        //internal bool? ToolsVersionEquals(string otherToolsVersion) => ToolsVersion is not null ? ToolsVersion == otherToolsVersion : null;


        ///// <summary>
        ///// Compares FirmwareVersion with the specified version number.
        ///// </summary>
        ///// <returns>1, 0 or -1 as per string Compare(), or null if either value is invalid or null</returns>
        //internal int? FirmwareVersionCompare(string otherFirmwareVersion) => CTecUtil.TextProcessing.CompareFirmwareVersion(otherFirmwareVersion, FirmwareVersion);


        ///// <summary>
        ///// Compares ToolsVersion with the specified version number.
        ///// </summary>
        ///// <returns>1, 0 or -1 as per string Compare(), or null if either value is invalid or null</returns>
        //internal int? ToolsVersionCompare(string otherToolsVersion) => CTecUtil.TextProcessing.CompareSoftwareVersion(otherToolsVersion, ToolsVersion);


        ///// <summary>Current errors or warnings</summary>
        //private List<ConfigErrorPage> _errorsAndWarnings = new();


        //public override bool Validate()
        //{
        //    _errorsAndWarnings.Clear();

        //    foreach (var p in Panels)
        //        if (!p.Value.Validate())
        //            _errorsAndWarnings.Add(p.Value.GetPageErrorDetails());

        //    if (!SiteConfig.Validate())
        //        _errorsAndWarnings.Add(SiteConfig.GetPageErrorDetails());

        //    return !HasErrorsOrWarnings();
        //}


        //internal new bool HasErrorsOrWarnings()
        //{
        //    foreach (var _ in from p in Panels where p.Value.HasErrorsOrWarnings() select new { })
        //        return true;
        //    return SiteConfig.HasErrorsOrWarnings();
        //}

        //internal new bool HasErrors()
        //{
        //    foreach (var _ in from p in Panels where p.Value.HasErrors() select new { })
        //        return true;
        //    return SiteConfig.HasErrors();
        //}

        //internal new bool HasWarnings()
        //{
        //    foreach (var _ in from p in Panels where p.Value.HasWarnings() select new { })
        //        return true;
        //    return SiteConfig.HasWarnings();
        //}
    }
}
